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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Documents.Serialization;

namespace PrintDialogX
{
    internal sealed class PrintDialogViewModel(Action<Action> invoker, PrintDialog dialog, PrintDocument document, PrintSettings settings, Internal.PreviewDocument preview, Action retriever, Action visualizer, Action informer)
    {
        internal sealed class ModelValue<T>(Action<Action> invoker, T initial, Action? updater) : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged = null;

            public T Value
            {
                get;
                set
                {
                    if (Equals(field, value))
                    {
                        return;
                    }

                    field = value;

                    OnPropertyChanged();
                    updater?.Invoke();
                }
            } = initial;

            public void OnPropertyChanged()
            {
                invoker(() => PropertyChanged?.Invoke(this, new(nameof(Value))));
            }
        }

        internal sealed class ModelCollection<T>(Action<Action> invoker, IEnumerable<T> initial, Func<T?> defaulter, Func<T> alternator, Action? updater) : INotifyPropertyChanged where T : struct
        {
            internal sealed class Collection(Action<Action> invoker, IEnumerable<T> initial) : List<T>(initial), INotifyCollectionChanged
            {
                public event NotifyCollectionChangedEventHandler? CollectionChanged = null;

                public void Reset(IEnumerable<T?>? items, T fallback)
                {
                    Clear();
                    foreach (T? item in items ?? [])
                    {
                        if (item is not T value || Contains(value))
                        {
                            continue;
                        }

                        Add(value);
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
                get => selection ?? alternator();
                set
                {
                    if (Equals(selection, value))
                    {
                        return;
                    }

                    selection = value;
                    isCustomized = true;

                    OnPropertyChanged(nameof(Selection));
                    updater?.Invoke();
                }
            }

            public Collection Items { get; } = new(invoker, [.. initial]);

            private T? selection = defaulter() ?? null;
            private bool isCustomized = false;

            public void Load<TSource>(IEnumerable<TSource>? items, TSource? target, Func<TSource, T?> converter) where TSource : struct
            {
                Load(items?.Select(converter), target is TSource value ? converter(value) : null);
            }

            public void Load(IEnumerable<T?>? items, T? target)
            {
                T fallback = alternator();
                Items.Reset(items, fallback);
                OnPropertyChanged(nameof(Items));

                bool isAbsent = selection is T current && !Items.Contains(current);
                if (!isCustomized && defaulter() is T primary && Items.Contains(primary))
                {
                    selection = primary;
                }
                else if ((!isCustomized || isAbsent) && target is T secondary && Items.Contains(secondary))
                {
                    selection = secondary;
                }
                else if (selection is null || isAbsent)
                {
                    selection = Items.Contains(fallback) ? fallback : Items.First();
                }
                OnPropertyChanged(nameof(Selection));
            }

            public void Fallback(T value)
            {
                selection = value;
                OnPropertyChanged(nameof(Selection));
            }

            public void OnPropertyChanged(string property)
            {
                invoker(() => PropertyChanged?.Invoke(this, new(property)));
            }
        }

        internal sealed class ModelLocker() : IDisposable
        {
            internal sealed class Scope(SemaphoreSlim locker) : IDisposable
            {
                public void Dispose()
                {
                    locker.Release();
                }
            }

            private readonly SemaphoreSlim locker = new(1, 1);

            public Scope Lock()
            {
                locker.Wait();

                return new(locker);
            }

            public async Task<Scope> LockAsync()
            {
                await locker.WaitAsync();

                return new(locker);
            }

            public void Dispose()
            {
                locker.Dispose();
            }
        }

        internal sealed class ModelCommand(Action executer) : ICommand
        {
            public event EventHandler? CanExecuteChanged = null;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                executer();
            }
        }

        public PrintDocument PrintDocument { get; } = document;
        public PrintSettings PrintSettings { get; } = settings;
        public InterfaceSettings InterfaceSettings { get; } = dialog.InterfaceSettings;
        public PerformanceStrategy PerformanceStrategy { get; } = dialog.PerformanceStrategy;
        public ColorEmulationLevel ColorEmulationLevel { get; } = dialog.ColorEmulationLevel;

        public ModelValue<Internal.PreviewDocument> PreviewDocument { get; } = new(invoker, preview, null);
        public ModelValue<int> PreviewIndex { get; } = new(invoker, 1, null);
        public Internal.PreviewDocumentControl? PreviewViewer { get; set; } = null;

        public ModelValue<bool> IsPrompting { get; } = new(invoker, false, null);
        public ModelValue<object> PromptingContent { get; } = new(invoker, string.Empty, null);
        public Action? PromptingDismisser { get; set; } = null;

        public ModelValue<bool> IsWorking { get; } = new(invoker, false, null);
        public ModelValue<object> WorkingContent { get; } = new(invoker, string.Empty, null);
        public ModelValue<double> WorkingProgress { get; } = new(invoker, 0, null);
        public Action? WorkingDismisser { get; set; } = null;

        public ModelValue<bool> IsPrinterReady { get; } = new(invoker, true, null);
        public ModelValue<bool> IsSettingReady { get; } = new(invoker, true, null);
        public ModelValue<bool> IsPreviewReady { get; } = new(invoker, true, null);

        public ObservableCollection<PrintQueue> PrinterEntries { get; } = [];
        public ModelValue<PrintQueue?> Printer { get; } = new(invoker, null, retriever);
        public ModelValue<int> Copies { get; } = new(invoker, settings.Copies, informer);
        public ModelValue<int> CopiesMaximum { get; } = new(invoker, settings.Fallbacks.FallbackMaximumCopies, null);
        public ModelCollection<Enums.Collation> CollationEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Collation)).Cast<Enums.Collation>(), () => settings.Collation, () => Enums.Collation.Collated, informer);
        public ModelValue<bool> IsCollationSupported { get; } = new(invoker, settings.Fallbacks.FallbackIsCollationSupported, null);
        public ModelCollection<Enums.Pages> PagesEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Pages)).Cast<Enums.Pages>(), () => settings.Pages, () => Enums.Pages.AllPages, visualizer);
        public ModelValue<string> PagesCustom { get; } = new(invoker, settings.CustomPages, visualizer);
        public ModelCollection<Enums.Layout> LayoutEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Layout)).Cast<Enums.Layout>(), () => settings.Layout, () => Enums.Layout.Portrait, visualizer);
        public ModelCollection<Enums.Size> SizeEntries { get; } = new(invoker, [], () => settings.Size, () => settings.Fallbacks.FallbackSize, visualizer);
        public ModelCollection<Enums.Color> ColorEntries { get; } = new(invoker, [], () => settings.Color, () => settings.Fallbacks.FallbackColor, informer);
        public ModelCollection<Enums.Quality> QualityEntries { get; } = new(invoker, [], () => settings.Quality, () => settings.Fallbacks.FallbackQuality, informer);
        public ModelCollection<Enums.PagesPerSheet> PagesPerSheetEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.PagesPerSheet)).Cast<Enums.PagesPerSheet>(), () => settings.PagesPerSheet, () => Enums.PagesPerSheet.One, visualizer);
        public ModelCollection<Enums.PageOrder> PageOrderEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.PageOrder)).Cast<Enums.PageOrder>(), () => settings.PageOrder, () => Enums.PageOrder.Horizontal, visualizer);
        public ModelCollection<Enums.Scale> ScaleEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Scale)).Cast<Enums.Scale>(), () => settings.Scale, () => Enums.Scale.AutoFit, visualizer);
        public ModelValue<int> ScaleCustom { get; } = new(invoker, settings.CustomScale, visualizer);
        public ModelCollection<Enums.Margin> MarginEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.Margin)).Cast<Enums.Margin>(), () => settings.Margin, () => Enums.Margin.Default, visualizer);
        public ModelValue<int> MarginCustom { get; } = new(invoker, settings.CustomMargin, visualizer);
        public ModelCollection<Enums.DoubleSided> DoubleSidedEntries { get; } = new(invoker, Enum.GetValues(typeof(Enums.DoubleSided)).Cast<Enums.DoubleSided>(), () => settings.DoubleSided, () => Enums.DoubleSided.OneSided, informer);
        public ModelValue<bool> IsDoubleSidedSupported { get; } = new(invoker, settings.Fallbacks.FallbackIsDoubleSidedSupported, null);
        public ModelCollection<Enums.Type> TypeEntries { get; } = new(invoker, [], () => settings.Type, () => settings.Fallbacks.FallbackType, informer);
        public ModelCollection<Enums.Source> SourceEntries { get; } = new(invoker, [], () => settings.Source, () => settings.Fallbacks.FallbackSource, informer);
    }

    internal partial class PrintDialogControl : UserControl
    {
        public const int DURATION_WAIT = 50;

        private readonly IPrintDialogHost host;
        private readonly PrintDialogViewModel model;
        private readonly (PrintServer Server, bool IsCustomized) server;

        private (Task Current, CancellationTokenSource Cancellation)? task = null;
        private (PrintDialogViewModel.ModelLocker Task, PrintDialogViewModel.ModelLocker Source, PrintDialogViewModel.ModelLocker Preview) lockers = (new(), new(), new());

        public PrintDialogControl(PrintDialog dialog, IPrintDialogHost window) : base()
        {
            InitializeComponent();

            if (dialog.Document is not PrintDocument document)
            {
                throw new InvalidOperationException("The document is unset.");
            }

            host = window;
            host.AddShortcutHandlers([new(new PrintDialogViewModel.ModelCommand(StartPrinting), new(Key.P, ModifierKeys.Control))]);
            model = new(Dispatcher.Invoke, dialog, document, dialog.PrintSettings, new(lockers.Preview), LoadSettings, LoadPreview, async () =>
            {
                if (!await UpdateDocument(false))
                {
                    return;
                }

                LoadPreview();
            });
            server = (dialog.PrintServer ?? new(), dialog.PrintServer is not null);

            DataContext = model;
            Wpf.Ui.Appearance.ApplicationAccentColorManager.ApplySystemAccent();
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(this);
            (Language, FlowDirection, ResourceDictionary resources) = Internal.LanguageAttribute.Parse(model.InterfaceSettings.DisplayLanguage);
            Resources.MergedDictionaries.Add(resources);
            foreach (object key in new Internal.ConverterResource[] { Internal.ConverterResource.ValueToDescription, Internal.ConverterResource.PrinterToIcon, Internal.ConverterResource.PrinterToStatus, Internal.ConverterResource.PrinterToDescription, Internal.ConverterResource.SizeToDescription, Internal.ConverterResource.DocumentToContent, Internal.ConverterResource.DocumentToDescription })
            {
                ((Internal.LanguageHostConverter)Resources[key]).Resources = resources;
            }

            Internal.PrinterToIconConverter iconizer = (Internal.PrinterToIconConverter)Resources[Internal.ConverterResource.PrinterToIcon];
            iconizer.CollectionFax = server.Server.GetPrintQueues([EnumeratedPrintQueueTypes.Fax]);
            iconizer.CollectionNetwork = server.Server.GetPrintQueues([EnumeratedPrintQueueTypes.Connections]);
            ((Internal.PagesCustomValidationRule)Resources[Internal.ValidationResource.PagesCustom]).Maximum = model.PrintDocument.Pages.Count;
            ((Internal.DocumentToContentConverter)Resources[Internal.ConverterResource.DocumentToContent]).PerformanceStrategy = model.PerformanceStrategy;

            LoadPrinters(server.IsCustomized ? dialog.DefaultPrinter : (dialog.DefaultPrinter ?? Internal.Common.Try(LocalPrintServer.GetDefaultPrintQueue, null)));
        }

        private async void Exit(object sender, RoutedEventArgs e)
        {
            await StopTask();

            DataContext = null;

            if (!server.IsCustomized)
            {
                server.Server.Dispose();
            }
            lockers.Task.Dispose();
            lockers.Source.Dispose();
            lockers.Preview.Dispose();
        }

        private async void StartTask(Func<CancellationToken, Task> executor)
        {
            await StopTask();

            using (await lockers.Task.LockAsync())
            {
                CancellationTokenSource cancellation = new();
                task = (Task.Run(async () =>
                {
                    try
                    {
                        cancellation.Token.ThrowIfCancellationRequested();
                        await executor(cancellation.Token);
                    }
                    catch (OperationCanceledException) { }
                    finally
                    {
                        cancellation.Dispose();
                    }
                }), cancellation);
            }
        }

        private async Task StopTask()
        {
            using (await lockers.Task.LockAsync())
            {
                if (task is not (Task current, CancellationTokenSource cancellation) { Current.IsCompleted: false })
                {
                    return;
                }

                Internal.Common.Try(cancellation.Cancel, null);
                await current;
            }
        }

        private void DismissDialogPrompting(Wpf.Ui.Controls.ContentDialog sender, Wpf.Ui.Controls.ContentDialogButtonClickEventArgs e)
        {
            model.IsPrompting.Value = false;
            model.PromptingDismisser?.Invoke();
        }

        private void DismissDialogWorking(Wpf.Ui.Controls.ContentDialog sender, Wpf.Ui.Controls.ContentDialogButtonClickEventArgs e)
        {
            model.IsWorking.Value = false;
            model.WorkingDismisser?.Invoke();
        }

        private async Task<bool> UpdateDocument(bool isInitiator)
        {
            if (model is not { Printer.Value: PrintQueue printer })
            {
                return isInitiator;
            }

            using (await lockers.Source.LockAsync())
            {
                PrintSettingsEventArgs arguments = new(printer, new()
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
                }, isInitiator);
                Dispatcher.Invoke(() => model.PrintDocument.OnPrintSettingsChanged(arguments));

                while (arguments.IsUpdating is null)
                {
                    await Task.Delay(DURATION_WAIT);
                }

                if (arguments.IsUpdating.Value)
                {
                    ((Internal.PagesCustomValidationRule)Resources[Internal.ValidationResource.PagesCustom]).Maximum = model.PrintDocument.Pages.Count;
                }

                return arguments.IsUpdating.Value;
            }
        }

        private void LoadPrinters(PrintQueue? selection)
        {
            if (model is not { IsPrinterReady.Value: true })
            {
                return;
            }

            model.IsPrinterReady.Value = false;

            foreach (PrintQueue printer in server.Server.GetPrintQueues())
            {
                if (!model.PrinterEntries.Contains(printer, Internal.PrinterComparer.Instance))
                {
                    model.PrinterEntries.Add(printer);
                }

                if (Internal.PrinterComparer.Instance.Equals(printer, selection))
                {
                    model.Printer.Value = printer;
                }
            }

            if (!model.PrinterEntries.Any())
            {
                model.PromptingContent.Value = Resources[Internal.TextResource.MessageNoPrinter];
                model.PromptingDismisser = () => host.SetResult(new()
                {
                    IsSuccess = false,
                    PaperCount = 0
                });
                model.IsPrompting.Value = true;
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

            model.IsPrinterReady.Value = true;
        }

        private void RefreshPrinters(object sender, EventArgs e)
        {
            Internal.Common.Try(async () => await Dispatcher.InvokeAsync(() => LoadPrinters(null), DispatcherPriority.Background), null);
        }

        private void AddPrinter(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = (Selector)sender;
            if (selector.SelectedItem is PrintQueue || DataContext is null)
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
                model.PromptingContent.Value = Resources[Internal.TextResource.MessageFailedPrinterAdd];
                model.PromptingDismisser = null;
                model.IsPrompting.Value = true;
            }
        }

        private void OpenPrinter(object sender, RoutedEventArgs e)
        {
            if (model is not { Printer.Value: PrintQueue printer })
            {
                return;
            }

            try
            {
                using Process? process = Process.Start(new ProcessStartInfo()
                {
                    FileName = "rundll32",
                    Arguments = Internal.Common.Format("printui.dll,PrintUIEntry /p /n \"{0}\"", [printer.FullName]),
                    UseShellExecute = true
                });
            }
            catch
            {
                model.PromptingContent.Value = Resources[Internal.TextResource.MessageFailedPrinterPreferences];
                model.PromptingDismisser = null;
                model.IsPrompting.Value = true;
            }
        }

        private void LoadSettings()
        {
            if (model is not { Printer.Value: PrintQueue printer })
            {
                return;
            }

            StartTask(async x =>
            {
                model.IsSettingReady.Value = false;
                model.IsPreviewReady.Value = false;

                PrintTicket? defaults = await Dispatcher.InvokeAsync(() => Internal.Common.Try(() => printer.DefaultPrintTicket, null));
                PrintCapabilities? capabilities = await Dispatcher.InvokeAsync(() => Internal.Common.Try(printer.GetPrintCapabilities, null));
                x.ThrowIfCancellationRequested();

                model.CopiesMaximum.Value = capabilities?.MaxCopyCount ?? model.PrintSettings.Fallbacks.FallbackMaximumCopies;
                model.Copies.Value = Math.Min(model.CopiesMaximum.Value, model.Copies.Value);
                model.IsCollationSupported.Value = capabilities?.CollationCapability.Any(y => y is Collation.Collated) ?? model.PrintSettings.Fallbacks.FallbackIsCollationSupported;
                if (!model.IsCollationSupported.Value)
                {
                    model.CollationEntries.Fallback(Enums.Collation.Uncollated);
                }
                x.ThrowIfCancellationRequested();

                List<Enums.Size?> sizes = [];
                try
                {
                    using MemoryStream stream = await Dispatcher.InvokeAsync(() => printer.GetPrintCapabilitiesAsXml());
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
                            string? fallback = node.SelectSingleNode(Internal.Common.Format(search, ["DisplayName"]), namespaces)?.InnerText;
                            Enums.Size.DefinedSize? mapped = Internal.ValueMappings.Map(node.Attributes?["name"]?.Value.Split(':').Last(), Internal.ValueMappings.MAPPING_SIZE_XML);
                            sizes.Add(long.TryParse(node.SelectSingleNode(Internal.Common.Format(search, ["MediaSizeWidth"]), namespaces)?.InnerText, NumberStyles.Integer, CultureInfo.InvariantCulture, out long width) && long.TryParse(node.SelectSingleNode(Internal.Common.Format(search, ["MediaSizeHeight"]), namespaces)?.InnerText, NumberStyles.Integer, CultureInfo.InvariantCulture, out long height) ? new()
                            {
                                DefinedName = mapped,
                                FallbackName = fallback,
                                Width = width * Internal.ValueMappings.RATIO_MICRON,
                                Height = height * Internal.ValueMappings.RATIO_MICRON
                            } : (mapped is Enums.Size.DefinedSize defined ? new(defined)
                            {
                                FallbackName = fallback
                            } : null));
                        }
                    }
                }
                catch
                {
                    sizes.Clear();

                    foreach (PageMediaSize size in capabilities?.PageMediaSizeCapability ?? new([]))
                    {
                        Enums.Size.DefinedSize? mapped = Internal.ValueMappings.Map(size.PageMediaSizeName ?? PageMediaSizeName.Unknown, Internal.ValueMappings.MAPPING_SIZE);
                        sizes.Add(size is { Width: double width, Height: double height } ? new()
                        {
                            DefinedName = mapped,
                            Width = width,
                            Height = height
                        } : (mapped is Enums.Size.DefinedSize defined ? new(defined) : null));
                    }
                }
                model.SizeEntries.Load(sizes, sizes.FirstOrDefault(y => y?.Equals(defaults?.PageMediaSize) ?? false));
                x.ThrowIfCancellationRequested();

                model.ColorEntries.Load(capabilities?.OutputColorCapability, defaults?.OutputColor, y => Internal.ValueMappings.Map(y, Internal.ValueMappings.MAPPING_COLOR));
                model.QualityEntries.Load(capabilities?.OutputQualityCapability, defaults?.OutputQuality, y => Internal.ValueMappings.Map(y, Internal.ValueMappings.MAPPING_QUALITY));
                model.IsDoubleSidedSupported.Value = capabilities?.DuplexingCapability.Any(y => y is Duplexing.TwoSidedShortEdge or Duplexing.TwoSidedLongEdge) ?? model.PrintSettings.Fallbacks.FallbackIsDoubleSidedSupported;
                if (!model.IsDoubleSidedSupported.Value)
                {
                    model.DoubleSidedEntries.Fallback(Enums.DoubleSided.OneSided);
                }
                model.TypeEntries.Load(capabilities?.PageMediaTypeCapability, defaults?.PageMediaType, y => Internal.ValueMappings.Map(y, Internal.ValueMappings.MAPPING_TYPE));
                model.SourceEntries.Load(capabilities?.InputBinCapability, defaults?.InputBin, y => Internal.ValueMappings.Map(y, Internal.ValueMappings.MAPPING_SOURCE));
                x.ThrowIfCancellationRequested();

                // @TODO: support for stapling

                model.IsSettingReady.Value = true;
                LoadPreview();
            });
        }

        private void IntializePreview(object sender, EventArgs e)
        {
            Internal.PreviewDocumentControl viewer = (Internal.PreviewDocumentControl)sender;
            model.PreviewViewer = viewer;

            host.AddShortcutHandlers((new (Action Executer, IEnumerable<KeyGesture> Gestures)[]
            {
                (viewer.ZoomIn, [new(Key.OemPlus, ModifierKeys.Control), new(Key.Add, ModifierKeys.Control)]),
                (viewer.ZoomOut, [new(Key.OemMinus, ModifierKeys.Control), new(Key.Subtract, ModifierKeys.Control)]),
                (viewer.ZoomActual, [new(Key.D0, ModifierKeys.Control), new(Key.NumPad0, ModifierKeys.Control)]),
                (() => viewer.ZoomColumns(1), [new(Key.D1, ModifierKeys.Control), new(Key.NumPad1, ModifierKeys.Control)]),
                (() => viewer.ZoomColumns(2), [new(Key.D2, ModifierKeys.Control), new(Key.NumPad2, ModifierKeys.Control)]),
                (viewer.NavigateFirst, [new(Key.Home, ModifierKeys.Alt)]),
                (viewer.NavigatePrevious, [new(Key.PageUp, ModifierKeys.Alt), new(Key.Left, ModifierKeys.Alt)]),
                (viewer.NavigateNext, [new(Key.PageDown, ModifierKeys.Alt), new(Key.Right, ModifierKeys.Alt)]),
                (viewer.NavigateLast, [new(Key.End, ModifierKeys.Alt)]),
            }).SelectMany(x => x.Gestures.Select(y => new KeyBinding(new PrintDialogViewModel.ModelCommand(x.Executer), y))));
        }

        private void LoadPreview()
        {
            if (model is not { Printer.Value: PrintQueue printer })
            {
                return;
            }

            StartTask(async x =>
            {
                model.IsPreviewReady.Value = false;

                IEnumerable<object>? pages = model.PagesEntries.Selection switch
                {
                    Enums.Pages.CurrentPage => await Internal.Common.Execute(async () =>
                    {
                        using (await lockers.Preview.LockAsync())
                        {
                            return new object[] { model.PreviewDocument.Value.Pages.Count > 0 ? model.PreviewDocument.Value.Pages[Internal.Common.Clamp(0, model.PreviewDocument.Value.Pages.Count - 1, model.PreviewIndex.Value - 1)].Index : 1 };
                        }
                    }),
                    Enums.Pages.CustomPages => Internal.PagesCustomValidationRule.TryConvert(model.PagesCustom.Value, int.MaxValue, false).Result,
                    _ => null
                };
                bool isLandscape = model.LayoutEntries.Selection is Enums.Layout.Landscape;
                Size size = isLandscape ? new(model.SizeEntries.Selection.Height, model.SizeEntries.Selection.Width) : new(model.SizeEntries.Selection.Width, model.SizeEntries.Selection.Height);
                (int cells, int columns, int rows) = model.PagesPerSheetEntries.Selection switch
                {
                    Enums.PagesPerSheet.Two => isLandscape ? (2, 1, 2) : (2, 2, 1),
                    Enums.PagesPerSheet.Four => (4, 2, 2),
                    Enums.PagesPerSheet.Six => isLandscape ? (6, 2, 3) : (6, 3, 2),
                    Enums.PagesPerSheet.Nine => (9, 3, 3),
                    Enums.PagesPerSheet.Sixteen => (16, 4, 4),
                    _ => (1, 1, 1)
                };
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
                    Enums.Margin.Minimum => await Dispatcher.InvokeAsync(() => Internal.Common.Try(() => printer.GetPrintCapabilities(new()
                    {
                        PageMediaSize = new(model.SizeEntries.Selection.Width, model.SizeEntries.Selection.Height),
                        PageOrientation = Internal.ValueMappings.Map(model.LayoutEntries.Selection, Internal.ValueMappings.MAPPING_LAYOUT)
                    }).PageImageableArea, null) is PageImageableArea { OriginWidth: double left, OriginHeight: double top } ? Math.Min(Math.Min(size.Width, size.Height) / 2, Math.Max(left, top)) : 0),
                    Enums.Margin.Custom => model.MarginCustom.Value,
                    _ => model.PrintDocument.DocumentMargin
                };
                x.ThrowIfCancellationRequested();

                Size measurement = new(Math.Max(0, size.Width - 2 * margin), Math.Max(0, size.Height - 2 * margin));
                model.PrintDocument.UpdateMeasurement(measurement);
                await UpdateDocument(true);
                x.ThrowIfCancellationRequested();

                Size extent = model.PrintDocument.DocumentSize is Enums.Size { Width: double width, Height: double height } ? new(width - 2 * model.PrintDocument.DocumentMargin, height - 2 * model.PrintDocument.DocumentMargin) : measurement;
                Size cell = new(measurement.Width / columns, measurement.Height / rows);
                double factor = Internal.Common.Validate(scale ?? Math.Min(cell.Width / extent.Width, cell.Height / extent.Height), y => !double.IsNaN(y), 0);
                Internal.PreviewPage.Construction construction = new(extent, cell, margin, columns, rows, model.PageOrderEntries.Selection, new(factor, factor), new(new(0, 0, cell.Width / factor, cell.Height / factor)));
                construction.Scaling.Freeze();
                construction.Clip.Freeze();
                x.ThrowIfCancellationRequested();

                using (await lockers.Preview.LockAsync())
                {
                    model.PreviewDocument.Value.PageSize = size;
                    model.PreviewDocument.Value.Pages.Clear();

                    using (await lockers.Source.LockAsync())
                    {
                        int index = 0;
                        List<PrintPage>? subpages = null;
                        foreach (PrintPage page in model.PrintDocument.Pages)
                        {
                            index++;
                            if (!Internal.PagesCustomValidationRule.CheckIndex(index, pages))
                            {
                                continue;
                            }

                            if (subpages is null)
                            {
                                subpages = [];
                                model.PreviewDocument.Value.Pages.Add(new(index, subpages, construction));
                            }

                            subpages.Add(page);
                            if (subpages.Count >= cells)
                            {
                                subpages = null;
                                x.ThrowIfCancellationRequested();
                            }
                        }
                    }
                }

                int original = model.PreviewIndex.Value;
                model.PreviewDocument.OnPropertyChanged();
                model.IsPreviewReady.Value = true;

                await Dispatcher.InvokeAsync(() => model.PreviewViewer?.NavigateIndex(original));
            });
        }

        private void ZoomPreviewIn(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.ZoomIn();
        }

        private void ZoomPreviewOut(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.ZoomOut();
        }

        private void ZoomPreviewActual(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.ZoomActual();
        }

        private void ZoomPreviewFitWidth(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.ZoomMode(Internal.PreviewDocumentControl.Zoom.FitToWidth);
        }

        private void ZoomPreviewPageWhole(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.ZoomColumns(1);
        }

        private void ZoomPreviewPageTwo(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.ZoomColumns(2);
        }

        private void NavigatePreviewPageFirst(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.NavigateFirst();
        }

        private void NavigatePreviewPagePrevious(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.NavigatePrevious();
        }

        private void NavigatePreviewPageNext(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.NavigateNext();
        }

        private void NavigatePreviewPageLast(object sender, RoutedEventArgs e)
        {
            model.PreviewViewer?.NavigateLast();
        }

        private async void StartPrinting()
        {
            if (model is not { IsPreviewReady.Value: true, Printer.Value: PrintQueue printer })
            {
                return;
            }

            PrintDialogViewModel.ModelLocker.Scope scope = await lockers.Preview.LockAsync();
            try
            {
                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printer);
                printer.CurrentJobSettings.CurrentPrintTicket = new()
                {
                    CopyCount = model.Copies.Value,
                    Collation = Internal.ValueMappings.Map(model.CollationEntries.Selection, Internal.ValueMappings.MAPPING_COLLATION),
                    PageOrientation = Internal.ValueMappings.Map(model.LayoutEntries.Selection, Internal.ValueMappings.MAPPING_LAYOUT),
                    PageMediaSize = new(model.SizeEntries.Selection.DefinedName is Enums.Size.DefinedSize defined ? Internal.ValueMappings.Map(defined, Internal.ValueMappings.MAPPING_SIZE) : PageMediaSizeName.Unknown, model.SizeEntries.Selection.Width, model.SizeEntries.Selection.Height),
                    OutputColor = Internal.ValueMappings.Map(model.ColorEntries.Selection, Internal.ValueMappings.MAPPING_COLOR),
                    OutputQuality = Internal.ValueMappings.Map(model.QualityEntries.Selection, Internal.ValueMappings.MAPPING_QUALITY),
                    PagesPerSheet = 1,
                    PagesPerSheetDirection = PagesPerSheetDirection.RightBottom,
                    PageScalingFactor = 100,
                    Duplexing = Internal.ValueMappings.Map(model.DoubleSidedEntries.Selection, Internal.ValueMappings.MAPPING_DOUBLE_SIDED),
                    PageMediaType = Internal.ValueMappings.Map(model.TypeEntries.Selection, Internal.ValueMappings.MAPPING_TYPE),
                    InputBin = Internal.ValueMappings.Map(model.SourceEntries.Selection, Internal.ValueMappings.MAPPING_SOURCE)
                };
                printer.CurrentJobSettings.Description = model.PrintDocument.DocumentName;

                host.SetProgress(new()
                {
                    State = IPrintDialogHost.PrintDialogProgressState.Indeterminate,
                    Value = 0
                });
                model.WorkingContent.Value = Resources[Internal.TextResource.LabelInitializing];
                model.WorkingProgress.Value = 0;
                model.WorkingDismisser = () => Internal.Common.Try(writer.CancelAsync, () =>
                {
                    scope.Dispose();
                    StopPrinting(Internal.TextResource.MessageCancelledPrintJob);
                });
                model.IsWorking.Value = true;

                writer.WritingProgressChanged += (x, e) =>
                {
                    if (e is not { WritingLevel: WritingProgressChangeLevel.FixedPageWritingProgress })
                    {
                        return;
                    }

                    double progress = 100.0 * e.Number / model.PreviewDocument.Value.Pages.Count;
                    model.WorkingContent.Value = string.Format(Language.GetSpecificCulture(), (string)Resources[Internal.TextResource.ConstructionProgress], (int)Math.Round(progress), e.Number, model.PreviewDocument.Value.Pages.Count);
                    model.WorkingProgress.Value = progress;
                    host.SetProgress(new()
                    {
                        State = IPrintDialogHost.PrintDialogProgressState.Normal,
                        Value = progress
                    });
                };
                writer.WritingCancelled += (x, e) =>
                {
                    scope.Dispose();
                    StopPrinting(Internal.TextResource.MessageCancelledPrintJob);
                };
                writer.WritingCompleted += (x, e) =>
                {
                    scope.Dispose();
                    if (e is { Cancelled: true })
                    {
                        StopPrinting(Internal.TextResource.MessageCancelledPrintJob);
                        return;
                    }
                    else if (e is { Error: not null })
                    {
                        StopPrinting(Internal.TextResource.MessageErrorPrintJob);
                        return;
                    }

                    int count = model.PreviewDocument.Value.Pages.Count * model.Copies.Value;
                    host.SetResult(new()
                    {
                        IsSuccess = true,
                        PaperCount = model.DoubleSidedEntries.Selection is Enums.DoubleSided.OneSided ? count : Internal.Common.Partition(count, 2)
                    });
                };
                writer.WriteAsync(model.PreviewDocument.Value);
            }
            catch
            {
                scope.Dispose();
                StopPrinting(Internal.TextResource.MessageFailedPrintJob);
            }
        }

        private void StopPrinting(Internal.TextResource message)
        {
            model.PromptingContent.Value = Resources[message];
            model.PromptingDismisser = () => host.SetProgress(new()
            {
                State = IPrintDialogHost.PrintDialogProgressState.None,
                Value = 0
            });
            model.IsPrompting.Value = true;
            model.IsWorking.Value = false;
            host.SetProgress(new()
            {
                State = IPrintDialogHost.PrintDialogProgressState.Error,
                Value = 0
            });
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            StartPrinting();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            host.SetResult(new()
            {
                IsSuccess = false,
                PaperCount = 0
            });
        }
    }
}
