using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Windows;
using System.Windows.Xps;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Documents.Serialization;

namespace PrintDialogX
{
    internal sealed class PrintDialogViewModel(Action<Action> invoker, PrintDocument document, PrintSettings settings, InterfaceSettings appearance, PerformanceStrategy strategy, SemaphoreSlim scheduler, Action retriever, Action visualizer, Action informer)
    {
        internal sealed class ModelValue<T>(Action<Action> invoker, T initial, Action? callback = null) : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged = null;

            public T Value
            {
                get => current;
                set
                {
                    if (Equals(current, value))
                    {
                        return;
                    }

                    current = value;

                    OnPropertyChanged();
                    callback?.Invoke();
                }
            }

            private T current = initial;

            public void OnPropertyChanged()
            {
                invoker(() => PropertyChanged?.Invoke(this, new(nameof(Value))));
            }
        }

        internal sealed class ModelCollection<T>(Action<Action> invoker, IEnumerable<T> initial, T? selection, T fallback, Action? callback = null) : INotifyPropertyChanged where T : struct
        {
            internal sealed class Entries(Action<Action> invoker, IEnumerable<T> initial) : List<T>(initial), INotifyCollectionChanged
            {
                public event NotifyCollectionChangedEventHandler? CollectionChanged = null;

                public void Reset(IEnumerable<T?>? entries, T fallback)
                {
                    Clear();
                    foreach (T? entry in entries ?? [])
                    {
                        if (entry != null && !Contains(entry.Value))
                        {
                            Add(entry.Value);
                        }
                    }
                    if (Count <= 0)
                    {
                        Add(fallback);
                    }

                    invoker(() => CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged = null;

            public T Selection
            {
                get => current ?? fallback;
                set
                {
                    if (Equals(current, value))
                    {
                        return;
                    }

                    isCustomized = true;
                    current = value;

                    OnPropertyChanged(nameof(Selection));
                    callback?.Invoke();
                }
            }

            public Entries Collection { get; } = new(invoker, [.. initial]);

            private T? current = selection ?? null;
            private bool isCustomized = false;

            public void Load<T2>(IEnumerable<T2>? collection, T2? target, Func<T2, T?> converter) where T2 : struct
            {
                Collection.Reset(collection?.Select(converter), fallback);
                OnPropertyChanged(nameof(Collection));

                if (!isCustomized && selection != null && Collection.Contains(selection.Value))
                {
                    current = selection;
                }
                else if ((!isCustomized || (current != null && !Collection.Contains(current.Value))) && target != null && converter(target.Value) is T value && Collection.Contains(value))
                {
                    current = value;
                }
                else if (current == null || !Collection.Contains(current.Value))
                {
                    current = Collection.Contains(fallback) ? fallback : Collection.First();
                }
                OnPropertyChanged(nameof(Selection));
            }

            public void Fallback(T value)
            {
                current = value;
                OnPropertyChanged(nameof(Selection));
            }

            public void OnPropertyChanged(string property)
            {
                invoker(() => PropertyChanged?.Invoke(this, new(property)));
            }
        }

        public PrintDocument PrintDocument { get; } = document;
        public PrintSettings PrintSettings { get; } = settings;
        public InterfaceSettings InterfaceSettings { get; } = appearance;
        public PerformanceStrategy PerformanceStrategy { get; } = strategy;

        public ModelValue<DocumentHostControl.Document> PreviewDocument { get; } = new(invoker, new(scheduler));

        public ModelValue<bool> IsPrinting { get; } = new(invoker, false);
        public ModelValue<object> PrintingContent { get; } = new(invoker, string.Empty);
        public ModelValue<double> PrintingProgress { get; } = new(invoker, 0);
        public Action? PrintingCallback { get; set; } = null;

        public ModelValue<bool> IsError { get; } = new(invoker, false);
        public ModelValue<object> ErrorContent { get; } = new(invoker, string.Empty);
        public Action? ErrorCallback { get; set; } = null;

        public ModelValue<bool> IsPrintersReady { get; } = new(invoker, true);
        public ModelValue<bool> IsSettingsReady { get; } = new(invoker, true);
        public ModelValue<bool> IsDocumentReady { get; } = new(invoker, true);

        public ObservableCollection<PrintQueue> PrinterEntries { get; } = [];
        public ModelValue<PrintQueue?> Printer { get; } = new(invoker, null, retriever);
        public ModelValue<int> Copies { get; } = new(invoker, settings.Copies, informer);
        public ModelValue<int> CopiesMaximum { get; } = new(invoker, settings.Fallbacks.FallbackMaximumCopies);
        public ModelCollection<Enums.Collation> CollationEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Collation)).Cast<Enums.Collation>(), settings.Collation, Enums.Collation.Collated, informer);
        public ModelValue<bool> IsCollationSupported { get; } = new(invoker, settings.Fallbacks.FallbackIsCollationSupported);
        public ModelCollection<Enums.Pages> PagesEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Pages)).Cast<Enums.Pages>(), settings.Pages, Enums.Pages.AllPages, visualizer);
        public ModelValue<double> PagesCurrent { get; } = new(invoker, 1);
        public ModelValue<string> PagesCustom { get; } = new(invoker, settings.CustomPages, visualizer);
        public ModelCollection<Enums.Layout> LayoutEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Layout)).Cast<Enums.Layout>(), settings.Layout, Enums.Layout.Portrait, visualizer);
        public ModelCollection<Enums.Size> SizeEntries { get; } = new(invoker, [], settings.Size, settings.Fallbacks.FallbackSize, visualizer);
        public ModelCollection<Enums.Color> ColorEntries { get; } = new(invoker, [], settings.Color, settings.Fallbacks.FallbackColor, informer);
        public ModelCollection<Enums.Quality> QualityEntries { get; } = new(invoker, [], settings.Quality, settings.Fallbacks.FallbackQuality, informer);
        public ModelCollection<Enums.PagesPerSheet> PagesPerSheetEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.PagesPerSheet)).Cast<Enums.PagesPerSheet>(), settings.PagesPerSheet, Enums.PagesPerSheet.One, visualizer);
        public ModelCollection<Enums.PageOrder> PageOrderEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.PageOrder)).Cast<Enums.PageOrder>(), settings.PageOrder, Enums.PageOrder.Horizontal, visualizer);
        public ModelCollection<Enums.Scale> ScaleEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Scale)).Cast<Enums.Scale>(), settings.Scale, Enums.Scale.AutoFit, visualizer);
        public ModelValue<int> ScaleCustom { get; } = new(invoker, settings.CustomScale, visualizer);
        public ModelCollection<Enums.Margin> MarginEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Margin)).Cast<Enums.Margin>(), settings.Margin, Enums.Margin.Default, visualizer);
        public ModelValue<int> MarginCustom { get; } = new(invoker, settings.CustomMargin, visualizer);
        public ModelCollection<Enums.DoubleSided> DoubleSidedEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.DoubleSided)).Cast<Enums.DoubleSided>(), settings.DoubleSided, Enums.DoubleSided.OneSided, informer);
        public ModelValue<bool> IsDoubleSidedSupported { get; } = new(invoker, settings.Fallbacks.FallbackIsDoubleSidedSupported);
        public ModelCollection<Enums.Type> TypeEntries { get; } = new(invoker, [], settings.Type, settings.Fallbacks.FallbackType, informer);
        public ModelCollection<Enums.Source> SourceEntries { get; } = new(invoker, [], settings.Source, settings.Fallbacks.FallbackSource, informer);
    }

    internal partial class PrintDialogControl : UserControl, ILanguageHost
    {
        public const int DURATION_SLEEP = 50;
        public const double EPSILON_INDEX = 0.01;

        public (SemaphoreSlim Task, SemaphoreSlim Document, SemaphoreSlim Preview) Lock { get; } = (new(1, 1), new(1, 1), new(1, 1));

        private readonly IPrintDialogHost host;
        private readonly PrintDialogViewModel model;
        private readonly (PrintServer Server, bool IsCustomized) server;

        private (Task Task, CancellationTokenSource Cancellation)? task = null;

        public PrintDialogControl(PrintDialog dialog, IPrintDialogHost window)
        {
            InitializeComponent();

            if (dialog.Document == null)
            {
                throw new InvalidOperationException("The document is unset.");
            }

            host = window;
            host.SetShortcutHandler(HandleShortcuts);
            model = new(Dispatcher.Invoke, dialog.Document, dialog.PrintSettings, dialog.InterfaceSettings, dialog.PerformanceStrategy, Lock.Preview, LoadSettings, LoadDocument, async () =>
            {
                if (await UpdateDocument())
                {
                    LoadDocument();
                }
            });
            server = (dialog.PrintServer ?? new(), dialog.PrintServer != null);

            DataContext = model;
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(this);
            InterfaceToContentConverter.ApplyLanguage(this, dialog.InterfaceSettings.DisplayLanguage);

            PrinterToIconConverter iconizer = (PrinterToIconConverter)Resources[ConverterResource.PrinterToIcon];
            iconizer.CollectionFax = server.Server.GetPrintQueues([EnumeratedPrintQueueTypes.Fax]);
            iconizer.CollectionNetwork = server.Server.GetPrintQueues([EnumeratedPrintQueueTypes.Shared, EnumeratedPrintQueueTypes.Connections]);
            ((PagesCustomValidationRule)Resources[ValidationResource.PagesCustom]).Maximum = dialog.Document.PageCount;

            LoadPrinters(server.IsCustomized ? dialog.DefaultPrinter : (dialog.DefaultPrinter ?? new Func<PrintQueue?>(() =>
            {
                try
                {
                    return LocalPrintServer.GetDefaultPrintQueue();
                }
                catch
                {
                    return null;
                }
            })()));
        }

        private async void Exit(object sender, RoutedEventArgs e)
        {
            await TaskStop();

            if (!server.IsCustomized)
            {
                server.Server.Dispose();
            }

            DataContext = null;

            Lock.Task.Dispose();
            Lock.Document.Dispose();
            Lock.Preview.Dispose();
        }

        private async void TaskStart(Func<CancellationToken, Task> callback)
        {
            await TaskStop();

            await Lock.Task.WaitAsync();
            try
            {
                CancellationTokenSource cancellation = new();
                task = (Task.Run(async () =>
                {
                    try
                    {
                        cancellation.Token.ThrowIfCancellationRequested();
                        await callback(cancellation.Token);
                    }
                    catch (OperationCanceledException) { }
                    finally
                    {
                        cancellation.Dispose();
                    }
                }), cancellation);
            }
            finally
            {
                Lock.Task.Release();
            }
        }

        private async Task TaskStop()
        {
            await Lock.Task.WaitAsync();
            try
            {
                if (task == null || task.Value.Task.IsCompleted)
                {
                    return;
                }

                try
                {
                    task.Value.Cancellation.Cancel();
                }
                catch { }
                await task.Value.Task;
            }
            finally
            {
                Lock.Task.Release();
            }
        }

        private void CloseDialogError(Wpf.Ui.Controls.ContentDialog sender, Wpf.Ui.Controls.ContentDialogButtonClickEventArgs e)
        {
            model.IsError.Value = false;
            model.ErrorCallback?.Invoke();
        }

        private void CloseDialogPrinting(Wpf.Ui.Controls.ContentDialog sender, Wpf.Ui.Controls.ContentDialogButtonClickEventArgs e)
        {
            model.IsPrinting.Value = false;
            model.PrintingCallback?.Invoke();
        }

        private void UpdateDialog(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            Wpf.Ui.Controls.ContentDialog dialog = (Wpf.Ui.Controls.ContentDialog)sender;
            if (dialog.Visibility == Visibility.Visible)
            {
                Keyboard.Focus(dialog);
            }
            else
            {
                Keyboard.ClearFocus();
            }
        }

        private void LoadPrinters(PrintQueue? selection = null)
        {
            if (!model.IsPrintersReady.Value)
            {
                return;
            }

            model.IsPrintersReady.Value = false;

            foreach (PrintQueue printer in server.Server.GetPrintQueues())
            {
                if (!model.PrinterEntries.Contains(printer, PrinterComparer.Instance))
                {
                    model.PrinterEntries.Add(printer);
                }

                if (selection != null && PrinterComparer.Instance.Equals(printer, selection))
                {
                    model.Printer.Value = printer;
                }
            }

            if (!model.PrinterEntries.Any())
            {
                model.ErrorContent.Value = Resources[StringResource.MessageNoPrinter];
                model.ErrorCallback = () => host.SetResult(new()
                {
                    IsSuccess = false,
                    PaperCount = 0
                });
                model.IsError.Value = true;
                host.SetProgress(new()
                {
                    State = IPrintDialogHost.PrintDialogProgressState.Error,
                    Value = 0
                });
            }
            else
            {
                model.Printer.Value ??= model.PrinterEntries.First();
            }

            model.IsPrintersReady.Value = true;
        }

        private void LoadPrinters(object sender, EventArgs e)
        {
            try
            {
                Dispatcher.InvokeAsync(() => LoadPrinters(), DispatcherPriority.Background);
            }
            catch { }
        }

        private void AddPrinter(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = (Selector)sender;
            if (selector.SelectedItem is PrintQueue || DataContext == null)
            {
                return;
            }

            selector.GetBindingExpression(Selector.SelectedItemProperty).UpdateTarget();

            try
            {
                using Process? process = Process.Start(new ProcessStartInfo()
                {
                    FileName = "ms-settings:printers",
                    UseShellExecute = true
                });
            }
            catch
            {
                model.ErrorContent.Value = Resources[StringResource.MessageFailedAddPrinter];
                model.ErrorCallback = null;
                model.IsError.Value = true;
            }
        }

        private void OpenPrinterPreferences(object sender, RoutedEventArgs e)
        {
            if (model.Printer.Value == null)
            {
                return;
            }

            try
            {
                using Process? process = Process.Start(new ProcessStartInfo()
                {
                    FileName = "rundll32",
                    Arguments = $"printui.dll,PrintUIEntry /p /n \"{model.Printer.Value.FullName}\"",
                    UseShellExecute = true
                });
            }
            catch
            {
                model.ErrorContent.Value = Resources[StringResource.MessageFailedPrinterPreferences];
                model.ErrorCallback = null;
                model.IsError.Value = true;
            }
        }

        private void LoadSettings()
        {
            if (model.Printer.Value == null)
            {
                return;
            }

            TaskStart(async x =>
            {
                model.IsSettingsReady.Value = false;
                model.IsDocumentReady.Value = false;

                PrintTicket? defaults = null;
                try
                {
                    defaults = await Dispatcher.InvokeAsync(() => model.Printer.Value.DefaultPrintTicket);
                }
                catch { }
                x.ThrowIfCancellationRequested();

                PrintCapabilities? capabilities = null;
                try
                {
                    capabilities = await Dispatcher.InvokeAsync(() => model.Printer.Value.GetPrintCapabilities());
                }
                catch { }
                x.ThrowIfCancellationRequested();

                model.CopiesMaximum.Value = Math.Min(model.PrintSettings.Fallbacks.FallbackMaximumCopies, capabilities?.MaxCopyCount ?? int.MaxValue);
                model.Copies.Value = Math.Min(model.CopiesMaximum.Value, model.Copies.Value);
                x.ThrowIfCancellationRequested();

                model.IsCollationSupported.Value = capabilities?.CollationCapability.Any(y => y == Collation.Collated) ?? model.PrintSettings.Fallbacks.FallbackIsCollationSupported;
                if (!model.IsCollationSupported.Value)
                {
                    model.CollationEntries.Fallback(Enums.Collation.Uncollated);
                }
                x.ThrowIfCancellationRequested();

                List<Enums.Size> sizes = [];
                Enums.Size? target = null;
                try
                {
                    using MemoryStream stream = await Dispatcher.InvokeAsync(() => model.Printer.Value.GetPrintCapabilitiesAsXml());
                    XmlDocument xml = new();
                    xml.Load(stream);

                    XmlNamespaceManager namespaces = new(xml.NameTable);
                    namespaces.AddNamespace("psf", "http://schemas.microsoft.com/windows/2003/08/printing/printschemaframework");
                    namespaces.AddNamespace("psk", "http://schemas.microsoft.com/windows/2003/08/printing/printschemakeywords");
                    if (xml.SelectNodes("//psf:Feature[@name='psk:PageMediaSize']/psf:Option", namespaces) is XmlNodeList nodes)
                    {
                        string search = "psf:Property[@name='psk:{0}']/psf:Value | psf:ScoredProperty[@name='psk:{0}']/psf:Value";
                        foreach (XmlNode node in nodes)
                        {
                            x.ThrowIfCancellationRequested();

                            if (!long.TryParse(node.SelectSingleNode(string.Format(search, "MediaSizeWidth"), namespaces)?.InnerText, NumberStyles.Integer, CultureInfo.InvariantCulture, out long width) || !long.TryParse(node.SelectSingleNode(string.Format(search, "MediaSizeHeight"), namespaces)?.InnerText, NumberStyles.Integer, CultureInfo.InvariantCulture, out long height))
                            {
                                continue;
                            }

                            Enums.Size size = new()
                            {
                                DefinedName = ValueMappings.Map(node.Attributes?["name"]?.Value.Split(':').Last(), ValueMappings.XmlSizeNameMapping),
                                FallbackName = node.SelectSingleNode(string.Format(search, "DisplayName"), namespaces)?.InnerText,
                                Width = width / 25400.0 * 96.0,
                                Height = height / 25400.0 * 96.0
                            };
                            sizes.Add(size);

                            if (size.Equals(defaults?.PageMediaSize))
                            {
                                target = size;
                            }
                        }
                    }
                }
                catch { }
                model.SizeEntries.Load(sizes, target, y => y);
                x.ThrowIfCancellationRequested();

                model.ColorEntries.Load(capabilities?.OutputColorCapability, defaults?.OutputColor, y => ValueMappings.Map(y, ValueMappings.ColorMapping));
                model.QualityEntries.Load(capabilities?.OutputQualityCapability, defaults?.OutputQuality, y => ValueMappings.Map(y, ValueMappings.QualityMapping));
                x.ThrowIfCancellationRequested();

                model.IsDoubleSidedSupported.Value = capabilities?.DuplexingCapability.Any(y => y == Duplexing.TwoSidedShortEdge || y == Duplexing.TwoSidedLongEdge) ?? model.PrintSettings.Fallbacks.FallbackIsDoubleSidedSupported;
                if (!model.IsDoubleSidedSupported.Value)
                {
                    model.DoubleSidedEntries.Fallback(Enums.DoubleSided.OneSided);
                }
                x.ThrowIfCancellationRequested();

                model.TypeEntries.Load(capabilities?.PageMediaTypeCapability, defaults?.PageMediaType, y => ValueMappings.Map(y, ValueMappings.TypeMapping));
                model.SourceEntries.Load(capabilities?.InputBinCapability, defaults?.InputBin, y => ValueMappings.Map(y, ValueMappings.SourceMapping));
                x.ThrowIfCancellationRequested();

                //TODO: support for stapling

                model.IsSettingsReady.Value = true;
                LoadDocument();
            });
        }

        private void LoadDocument()
        {
            if (model.Printer.Value == null)
            {
                return;
            }

            TaskStart(async x =>
            {
                model.IsDocumentReady.Value = false;

                bool isLandscape = model.LayoutEntries.Selection == Enums.Layout.Landscape;
                Size size = isLandscape ? new(model.SizeEntries.Selection.Height, model.SizeEntries.Selection.Width) : new(model.SizeEntries.Selection.Width, model.SizeEntries.Selection.Height);
                x.ThrowIfCancellationRequested();

                (int Count, int Columns, int Rows) arrangement = model.PagesPerSheetEntries.Selection switch
                {
                    Enums.PagesPerSheet.Two => (2, 1, 2),
                    Enums.PagesPerSheet.Four => (4, 2, 2),
                    Enums.PagesPerSheet.Six => (6, 2, 3),
                    Enums.PagesPerSheet.Nine => (9, 3, 3),
                    Enums.PagesPerSheet.Sixteen => (16, 4, 4),
                    _ => (1, 1, 1)
                };
                if (isLandscape)
                {
                    arrangement = (arrangement.Count, arrangement.Rows, arrangement.Columns);
                }
                x.ThrowIfCancellationRequested();

                double? scale = model.ScaleEntries.Selection switch
                {
                    Enums.Scale.Percent25 => 0.25,
                    Enums.Scale.Percent50 => 0.5,
                    Enums.Scale.Percent75 => 0.75,
                    Enums.Scale.Percent100 => 1,
                    Enums.Scale.Percent150 => 1.5,
                    Enums.Scale.Percent200 => 2,
                    Enums.Scale.Custom => model.ScaleCustom.Value / 100.0,
                    _ => null
                };
                double margin = model.MarginEntries.Selection switch
                {
                    Enums.Margin.None => 0,
                    Enums.Margin.Minimum => await new Func<Task<double>>(async () =>
                    {
                        try
                        {
                            PageImageableArea area = await Dispatcher.InvokeAsync(() => model.Printer.Value.GetPrintCapabilities(new()
                            {
                                PageMediaSize = new(model.SizeEntries.Selection.Width, model.SizeEntries.Selection.Height),
                                PageOrientation = ValueMappings.Map(model.LayoutEntries.Selection, ValueMappings.LayoutMapping)
                            }).PageImageableArea);

                            return Math.Min(Math.Min(size.Width, size.Height) / 2, Math.Max(area.OriginWidth, area.OriginHeight));
                        }
                        catch
                        {
                            return 0;
                        }
                    })(),
                    Enums.Margin.Custom => model.MarginCustom.Value,
                    _ => model.PrintDocument.DocumentMargin
                };
                x.ThrowIfCancellationRequested();

                List<object>? pages = model.PagesEntries.Selection switch
                {
                    Enums.Pages.CurrentPage => await new Func<Task<List<object>?>>(async () =>
                    {
                        await Lock.Preview.WaitAsync(x);
                        try
                        {
                            return model.PreviewDocument.Value.PageCount > 0 ? [model.PreviewDocument.Value.Pages[Math.Max(0, Math.Min(model.PreviewDocument.Value.PageCount - 1, (int)(model.PagesCurrent.Value + EPSILON_INDEX) - 1))].Index] : null;
                        }
                        finally
                        {
                            Lock.Preview.Release();
                        }
                    })(),
                    Enums.Pages.CustomPages => PagesCustomValidationRule.TryConvert(model.PagesCustom.Value, ((PagesCustomValidationRule)Resources[ValidationResource.PagesCustom]).Maximum).Result,
                    _ => null
                };
                if (!(pages?.Any() ?? true))
                {
                    pages = null;
                }
                x.ThrowIfCancellationRequested();

                model.PrintDocument.MeasuredSize = new(Math.Max(0, size.Width - margin * 2), Math.Max(0, size.Height - margin * 2));
                await UpdateDocument(true);
                x.ThrowIfCancellationRequested();

                Size extent = model.PrintDocument.DocumentSize != null ? new(model.PrintDocument.DocumentSize.Value.Width - model.PrintDocument.DocumentMargin * 2, model.PrintDocument.DocumentSize.Value.Height - model.PrintDocument.DocumentMargin * 2) : model.PrintDocument.MeasuredSize;
                Size cell = new(model.PrintDocument.MeasuredSize.Width / arrangement.Columns, model.PrintDocument.MeasuredSize.Height / arrangement.Rows);
                double factor = scale ?? Math.Min(cell.Width / extent.Width, cell.Height / extent.Height);
                if (double.IsNaN(factor))
                {
                    factor = 0;
                }
                x.ThrowIfCancellationRequested();

                DocumentHostControl.DocumentSettings settings = new(extent, cell, margin, arrangement.Columns, arrangement.Rows, model.PageOrderEntries.Selection, new ScaleTransform(factor, factor, 0, 0), new RectangleGeometry(new(0, 0, cell.Width / factor, cell.Height / factor)));
                settings.Transform.Freeze();
                settings.Clip.Freeze();
                x.ThrowIfCancellationRequested();

                model.PreviewDocument.Value.PageSize = size;
                await Lock.Document.WaitAsync(x);
                await Lock.Preview.WaitAsync(x);
                try
                {
                    model.PreviewDocument.Value.Pages.Clear();

                    int index = 0;
                    (int Start, List<PrintPage> Chunk)? current = null;
                    foreach (PrintPage content in model.PrintDocument.Pages)
                    {
                        x.ThrowIfCancellationRequested();

                        index++;
                        if (!(pages?.Any(y => y switch
                        {
                            int single => single == index,
                            (int start, int end) => start <= index && end >= index,
                            _ => false
                        }) ?? true))
                        {
                            continue;
                        }

                        if (current == null)
                        {
                            List<PrintPage> chunk = [];
                            current = (index, chunk);
                            model.PreviewDocument.Value.Pages.Add((index, new(chunk, settings)));
                        }

                        current.Value.Chunk.Add(content);
                        if (current.Value.Chunk.Count >= arrangement.Count)
                        {
                            current = null;
                        }
                    }

                    if (model.PerformanceStrategy == PerformanceStrategy.FavorsPrinting)
                    {
                        foreach ((int start, DocumentHostControl.DocumentPage page) in model.PreviewDocument.Value.Pages)
                        {
                            x.ThrowIfCancellationRequested();

                            await Dispatcher.InvokeAsync(async () =>
                            {
                                page.GetContent();

                                await Dispatcher.Yield();
                            });
                        }
                    }
                }
                finally
                {
                    Lock.Document.Release();
                    Lock.Preview.Release();
                }

                model.PreviewDocument.Value.ZoomValue = ZoomCurrent();
                model.PreviewDocument.OnPropertyChanged();
                model.IsDocumentReady.Value = true;
            });
        }

        private async Task<bool> UpdateDocument(bool isUpdating = false)
        {
            if (model.Printer.Value == null)
            {
                return isUpdating;
            }

            await Lock.Document.WaitAsync();
            try
            {
                PrintSettingsEventArgs settings = new(model.Printer.Value, new()
                {
                    Fallbacks = model.PrintSettings.Fallbacks,
                    Copies = Math.Max(1, model.Copies.Value),
                    Collation = model.CollationEntries.Selection,
                    Pages = model.PagesEntries.Selection,
                    CustomPages = model.PagesCustom.Value,
                    Layout = model.LayoutEntries.Selection,
                    Size = model.SizeEntries.Selection,
                    Color = model.ColorEntries.Selection,
                    Quality = model.QualityEntries.Selection,
                    PagesPerSheet = model.PagesPerSheetEntries.Selection,
                    PageOrder = model.PageOrderEntries.Selection,
                    Scale = model.ScaleEntries.Selection,
                    CustomScale = Math.Max(0, model.ScaleCustom.Value),
                    Margin = model.MarginEntries.Selection,
                    CustomMargin = Math.Max(0, model.MarginCustom.Value),
                    DoubleSided = model.DoubleSidedEntries.Selection,
                    Type = model.TypeEntries.Selection,
                    Source = model.SourceEntries.Selection
                }, isUpdating);
                model.PrintDocument.OnPrintSettingsChanged(Dispatcher, settings);

                while (settings.IsUpdating == null)
                {
                    await Task.Delay(DURATION_SLEEP);
                }

                ((PagesCustomValidationRule)Resources[ValidationResource.PagesCustom]).Maximum = model.PrintDocument.PageCount;

                return settings.IsUpdating.Value;
            }
            finally
            {
                Lock.Document.Release();
            }
        }

        private void Print()
        {
            if (!model.IsDocumentReady.Value || model.Printer.Value == null)
            {
                return;
            }

            try
            {
                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(model.Printer.Value);
                model.Printer.Value.CurrentJobSettings.CurrentPrintTicket = new()
                {
                    CopyCount = model.Copies.Value,
                    Collation = ValueMappings.Map(model.CollationEntries.Selection, ValueMappings.CollationMapping),
                    PageOrientation = ValueMappings.Map(model.LayoutEntries.Selection, ValueMappings.LayoutMapping),
                    PageMediaSize = new(model.SizeEntries.Selection.DefinedName != null ? ValueMappings.Map(model.SizeEntries.Selection.DefinedName, ValueMappings.SizeNameMapping) : PageMediaSizeName.Unknown, model.SizeEntries.Selection.Width, model.SizeEntries.Selection.Height),
                    OutputColor = ValueMappings.Map(model.ColorEntries.Selection, ValueMappings.ColorMapping),
                    OutputQuality = ValueMappings.Map(model.QualityEntries.Selection, ValueMappings.QualityMapping),
                    PagesPerSheet = 1,
                    PagesPerSheetDirection = PagesPerSheetDirection.RightBottom,
                    PageScalingFactor = 100,
                    Duplexing = ValueMappings.Map(model.DoubleSidedEntries.Selection, ValueMappings.DoubleSidedMapping),
                    PageMediaType = ValueMappings.Map(model.TypeEntries.Selection, ValueMappings.TypeMapping),
                    InputBin = ValueMappings.Map(model.SourceEntries.Selection, ValueMappings.SourceMapping)
                };
                model.Printer.Value.CurrentJobSettings.Description = model.PrintDocument.DocumentName;

                host.SetProgress(new()
                {
                    State = IPrintDialogHost.PrintDialogProgressState.Indeterminate,
                    Value = 0
                });
                model.PrintingContent.Value = Resources[StringResource.LabelInitializing];
                model.PrintingProgress.Value = 0;
                model.PrintingCallback = () =>
                {
                    try
                    {
                        writer.CancelAsync();
                    }
                    catch
                    {
                        Cancel(StringResource.MessagePrintJobCancelled);
                    }
                };
                model.IsPrinting.Value = true;

                writer.WritingProgressChanged += (x, e) =>
                {
                    if (e.WritingLevel != WritingProgressChangeLevel.FixedPageWritingProgress)
                    {
                        return;
                    }

                    double progress = 100.0 * e.Number / model.PreviewDocument.Value.PageCount;
                    model.PrintingContent.Value = string.Format(CultureInfo.InvariantCulture, (string)Resources[StringResource.ConstructionProgress], (int)Math.Round(progress), e.Number, model.PreviewDocument.Value.PageCount);
                    model.PrintingProgress.Value = progress;
                    host.SetProgress(new()
                    {
                        State = IPrintDialogHost.PrintDialogProgressState.Normal,
                        Value = progress
                    });
                };
                writer.WritingCancelled += (x, e) =>
                {
                    Cancel(StringResource.MessagePrintJobCancelled);
                };
                writer.WritingCompleted += (x, e) =>
                {
                    if (e.Cancelled)
                    {
                        Cancel(StringResource.MessagePrintJobCancelled);
                        return;
                    }
                    else if (e.Error != null)
                    {
                        Cancel(StringResource.MessagePrintJobError);
                        return;
                    }

                    int count = model.PreviewDocument.Value.PageCount * model.Copies.Value;
                    host.SetResult(new()
                    {
                        IsSuccess = true,
                        PaperCount = model.DoubleSidedEntries.Selection == Enums.DoubleSided.OneSided ? count : (int)Math.Ceiling(count * 0.5)
                    });
                };
                writer.WriteAsync(model.PreviewDocument.Value);
            }
            catch
            {
                Cancel(StringResource.MessageFailedPrintJob);
            }
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            Print();
        }

        private void Cancel(StringResource message)
        {
            model.ErrorContent.Value = Resources[message];
            model.ErrorCallback = () => host.SetProgress(new()
            {
                State = IPrintDialogHost.PrintDialogProgressState.None,
                Value = 0
            });
            model.IsError.Value = true;
            model.IsPrinting.Value = false;
            host.SetProgress(new()
            {
                State = IPrintDialogHost.PrintDialogProgressState.Error,
                Value = 0
            });
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            host.SetResult(new()
            {
                IsSuccess = false,
                PaperCount = 0
            });
        }

        private void InitializeViewer(object sender, EventArgs e)
        {
            model.PreviewDocument.Value.Viewer = (VirtualizingStackPanel)sender;
        }

        private void UpdateViewerDescription(object sender, ScrollChangedEventArgs e)
        {
            if (!model.IsDocumentReady.Value || model.PreviewDocument.Value.Viewer == null)
            {
                return;
            }

            model.PagesCurrent.Value = Math.Max(1, Math.Min(model.PreviewDocument.Value.PageCount, model.PreviewDocument.Value.Viewer.VerticalOffset / GetPageSize().Height * model.PreviewDocument.Value.ColumnCount + 1));
        }

        private void UpdateViewerScroll(object sender, MouseWheelEventArgs e)
        {
            if (model.PreviewDocument.Value.Viewer == null)
            {
                return;
            }

            switch (Keyboard.Modifiers)
            {
                case ModifierKeys.Shift:
                    model.PreviewDocument.Value.Viewer.SetHorizontalOffset(model.PreviewDocument.Value.Viewer.HorizontalOffset - e.Delta);
                    e.Handled = true;
                    break;
                case ModifierKeys.Control:
                    Point position = e.GetPosition(model.PreviewDocument.Value.Viewer);
                    double x = (model.PreviewDocument.Value.Viewer.HorizontalOffset + position.X) / model.PreviewDocument.Value.ZoomValue;
                    double y = (model.PreviewDocument.Value.Viewer.VerticalOffset + position.Y) / model.PreviewDocument.Value.ZoomValue;
                    double zoom = 0.15 * Math.Sign(e.Delta);
                    model.PreviewDocument.Value.ZoomValue *= 1 + zoom;
                    model.PreviewDocument.Value.ZoomMode = DocumentHostControl.DocumentZoom.Custom;
                    model.PreviewDocument.Value.ZoomTarget = new(x * model.PreviewDocument.Value.ZoomValue - position.X, y * model.PreviewDocument.Value.ZoomValue - position.Y - GetPageOffset(Math.Floor(model.PagesCurrent.Value + EPSILON_INDEX), DocumentHostControl.LENGTH_SPACING * 2) * zoom);
                    model.PreviewDocument.OnPropertyChanged();
                    e.Handled = true;
                    break;
            }
        }

        private void UpdateViewerZoom(object sender, EventArgs e)
        {
            if (model.PreviewDocument.Value.ZoomTarget == null || model.PreviewDocument.Value.Viewer == null)
            {
                return;
            }

            if (model.PreviewDocument.Value.ZoomTarget.Value.X <= model.PreviewDocument.Value.Viewer.ExtentWidth && model.PreviewDocument.Value.ZoomTarget.Value.Y <= model.PreviewDocument.Value.Viewer.ExtentHeight)
            {
                model.PreviewDocument.Value.Viewer.SetHorizontalOffset(model.PreviewDocument.Value.ZoomTarget.Value.X);
                model.PreviewDocument.Value.Viewer.SetVerticalOffset(model.PreviewDocument.Value.ZoomTarget.Value.Y);
                model.PreviewDocument.Value.ZoomTarget = null;
            }
        }

        private Size GetPageSize(double? scale = null)
        {
            return new(model.PreviewDocument.Value.PageSize.Width * (scale ?? model.PreviewDocument.Value.ZoomValue) + DocumentHostControl.LENGTH_SPACING * 2, model.PreviewDocument.Value.PageSize.Height * (scale ?? model.PreviewDocument.Value.ZoomValue) + DocumentHostControl.LENGTH_SPACING * 2);
        }

        private double GetPageOffset(double index, double? unit = null)
        {
            return (unit ?? GetPageSize().Height) * Math.Floor((Math.Max(1, Math.Min(model.PreviewDocument.Value.PageCount, index)) - 1) / model.PreviewDocument.Value.ColumnCount);
        }

        private double ZoomHorizontal(double padding = 16)
        {
            if (model.PreviewDocument.Value.Viewer == null)
            {
                return 1;
            }

            return (model.PreviewDocument.Value.Viewer.ViewportWidth - padding * model.PreviewDocument.Value.ColumnCount) / GetPageSize(1).Width / model.PreviewDocument.Value.ColumnCount;
        }

        private double ZoomVertical(double padding = 12)
        {
            if (model.PreviewDocument.Value.Viewer == null)
            {
                return 1;
            }

            return (model.PreviewDocument.Value.Viewer.ViewportHeight - padding) / GetPageSize(1).Height;
        }

        private double ZoomDelta(double delta)
        {
            double interval = Math.Abs(delta);
            double step = model.PreviewDocument.Value.ZoomValue / interval;
            return delta switch
            {
                > 0 => ((Math.Ceiling(step) - step < 0.35 ? Math.Ceiling(step) : Math.Floor(step)) + 1) * interval,
                < 0 => ((step - Math.Floor(step) < 0.35 ? Math.Floor(step) : Math.Ceiling(step)) - 1) * interval,
                _ => model.PreviewDocument.Value.ZoomValue
            };
        }

        private double ZoomCurrent()
        {
            return model.PreviewDocument.Value.ZoomMode switch
            {
                DocumentHostControl.DocumentZoom.FitToWidth => ZoomHorizontal(),
                DocumentHostControl.DocumentZoom.FitToHeight => ZoomVertical(),
                DocumentHostControl.DocumentZoom.FitToPage => Math.Min(ZoomHorizontal(), ZoomVertical()),
                _ => model.PreviewDocument.Value.ZoomValue
            };
        }

        private void ZoomResume(object sender, SizeChangedEventArgs e)
        {
            if (model.PreviewDocument.Value.ZoomMode == DocumentHostControl.DocumentZoom.Custom)
            {
                return;
            }

            model.PreviewDocument.Value.ZoomValue = ZoomCurrent();
            model.PreviewDocument.OnPropertyChanged();
        }

        private void ZoomIn()
        {
            model.PreviewDocument.Value.ZoomValue = ZoomDelta(0.25);
            model.PreviewDocument.Value.ZoomMode = DocumentHostControl.DocumentZoom.Custom;
            model.PreviewDocument.OnPropertyChanged();
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        private void ZoomOut()
        {
            model.PreviewDocument.Value.ZoomValue = ZoomDelta(-0.25);
            model.PreviewDocument.Value.ZoomMode = DocumentHostControl.DocumentZoom.Custom;
            model.PreviewDocument.OnPropertyChanged();
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        private void ZoomActual()
        {
            model.PreviewDocument.Value.ZoomValue = 1;
            model.PreviewDocument.Value.ZoomMode = DocumentHostControl.DocumentZoom.Custom;
            model.PreviewDocument.Value.ZoomTarget = new(0, GetPageOffset(Math.Round(model.PagesCurrent.Value)));
            model.PreviewDocument.OnPropertyChanged();
        }

        private void ZoomActual(object sender, RoutedEventArgs e)
        {
            ZoomActual();
        }

        private void ZoomFit(object sender, RoutedEventArgs e)
        {
            model.PreviewDocument.Value.ZoomValue = ZoomHorizontal();
            model.PreviewDocument.Value.ZoomMode = DocumentHostControl.DocumentZoom.FitToWidth;
            model.PreviewDocument.Value.ZoomTarget = new(0, GetPageOffset(Math.Round(model.PagesCurrent.Value)));
            model.PreviewDocument.OnPropertyChanged();
        }

        private void ZoomPage(int column)
        {
            model.PreviewDocument.Value.ColumnCount = column;
            model.PreviewDocument.OnPropertyChanged();

            model.PreviewDocument.Value.ZoomValue = Math.Min(ZoomHorizontal(), ZoomVertical());
            model.PreviewDocument.Value.ZoomMode = DocumentHostControl.DocumentZoom.FitToPage;
            model.PreviewDocument.Value.ZoomTarget = new(0, GetPageOffset(Math.Round(model.PagesCurrent.Value)));
            model.PreviewDocument.OnPropertyChanged();
        }

        private void ZoomPageWhole(object sender, RoutedEventArgs e)
        {
            ZoomPage(1);
        }

        private void ZoomPageTwo(object sender, RoutedEventArgs e)
        {
            ZoomPage(2);
        }

        private void NavigatePage(double index)
        {
            if (model.PreviewDocument.Value.Viewer == null)
            {
                return;
            }

            model.PreviewDocument.Value.Viewer.SetVerticalOffset(GetPageOffset(index));
        }

        private void NavigatePageFirst()
        {
            NavigatePage(1);
        }

        private void NavigatePageFirst(object sender, RoutedEventArgs e)
        {
            NavigatePageFirst();
        }

        private void NavigatePagePrevious()
        {
            NavigatePage(Math.Ceiling(model.PagesCurrent.Value - EPSILON_INDEX) - model.PreviewDocument.Value.ColumnCount);
        }

        private void NavigatePagePrevious(object sender, RoutedEventArgs e)
        {
            NavigatePagePrevious();
        }

        private void NavigatePageNext()
        {
            NavigatePage(Math.Floor(model.PagesCurrent.Value + EPSILON_INDEX) + model.PreviewDocument.Value.ColumnCount);
        }

        private void NavigatePageNext(object sender, RoutedEventArgs e)
        {
            NavigatePageNext();
        }

        private void NavigatePageLast()
        {
            NavigatePage(model.PreviewDocument.Value.PageCount);
        }

        private void NavigatePageLast(object sender, RoutedEventArgs e)
        {
            NavigatePageLast();
        }

        private void HandleShortcuts(object sender, KeyEventArgs e)
        {
            Action? action = (Keyboard.Modifiers, e.Key) switch
            {
                (ModifierKeys.Control, Key.P) => Print,
                (ModifierKeys.Control, Key.OemPlus) => ZoomIn,
                (ModifierKeys.Control, Key.Add) => ZoomIn,
                (ModifierKeys.Control, Key.OemMinus) => ZoomOut,
                (ModifierKeys.Control, Key.Subtract) => ZoomOut,
                (ModifierKeys.Control, Key.D0) => ZoomActual,
                (ModifierKeys.Control, Key.NumPad0) => ZoomActual,
                (ModifierKeys.Alt, Key.System) => e.SystemKey switch
                {
                    Key.Home => NavigatePageFirst,
                    Key.Left => NavigatePagePrevious,
                    Key.Right => NavigatePageNext,
                    Key.End => NavigatePageLast,
                    _ => null
                },
                (ModifierKeys.None, Key.PageUp) => NavigatePagePrevious,
                (ModifierKeys.None, Key.PageDown) => NavigatePageNext,
                _ => null
            };

            if (action != null)
            {
                action();
                e.Handled = true;
            }
        }

        public void UpdateLanguage(ResourceDictionary resources, string language)
        {
            Resources.MergedDictionaries.Add(resources);
            foreach (object key in new ConverterResource[] { ConverterResource.ValueToDescription, ConverterResource.PrinterToStatus, ConverterResource.PrinterToDescription, ConverterResource.SizeToDescription, ConverterResource.DocumentToDescription })
            {
                ((ILanguageHost)Resources[key]).UpdateLanguage(resources, language);
            }

            Language = XmlLanguage.GetLanguage(language);
        }
    }
}
