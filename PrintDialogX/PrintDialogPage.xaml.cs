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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Documents;

namespace PrintDialogX
{
    internal class PrintPageViewModel(Dispatcher dispatcher, PrintDocument document, InterfaceSettings appearance, PrintSettings settings, Action printerCallback, Action settingsCallback)
    {
        public static readonly ResourceDictionary StringResources = new()
        {
            Source = new("/PrintDialogX;component/Resources/Languages/en-US.xaml", UriKind.Relative)
        };

        public class ModelValue<T>(Dispatcher dispatcher, T initial, Action? callback = null) : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged = null;

            private T current = initial;

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

                    NotifyUpdate();
                    callback?.Invoke();
                }
            }

            public void NotifyUpdate()
            {
                dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new(nameof(Value))), DispatcherPriority.Render);
            }
        }

        public class ModelCollection<T>(Dispatcher dispatcher, IEnumerable<T> initial, T? selection, T fallback, Action? callback = null) : INotifyPropertyChanged where T : struct
        {
            public event PropertyChangedEventHandler? PropertyChanged = null;

            public class Entries(Dispatcher dispatcher, IEnumerable<T> initial) : List<T>(initial), INotifyCollectionChanged
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

                    dispatcher.Invoke(() => CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset)), DispatcherPriority.Render);
                }
            }

            private T? current = selection ?? null;
            private bool isCustomized = false;

            public T? Selection
            {
                get => current;
                set
                {
                    if (value == null || Equals(current, value))
                    {
                        return;
                    }

                    isCustomized = true;
                    current = value;

                    NotifyUpdate(nameof(Selection));
                    callback?.Invoke();
                }
            }

            public Entries Collection { get; } = new(dispatcher, [.. initial]);

            public void Load<T2>(IEnumerable<T2>? capability, T2? target, Func<T2, T?> converter) where T2 : struct
            {
                Collection.Reset(capability?.Select(converter), fallback);
                NotifyUpdate(nameof(Collection));

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
                NotifyUpdate(nameof(Selection));
            }

            public void SilentSelect(T value)
            {
                current = value;
                NotifyUpdate(nameof(Selection));
            }

            public void NotifyUpdate(string property)
            {
                dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new(property)), DispatcherPriority.Render);
            }
        }

        public class ModelDocument
        {
            public class ModelPage(int index, FixedPage content)
            {
                public int Index { get; set; } = index;
                public FixedPage Content { get; set; } = content;
            }

            public enum Mode
            {
                None,
                FitToWidth,
                FitToHeight,
                FitToPage
            }

            public List<ModelPage> Document { get; } = [];
            public object DocumentLock { get; } = new();

            public int ColumnCount { get; set; } = 1;
            public double Zoom
            {
                get => zoom;
                set => zoom = Math.Max(0.05, value);
            }
            private double zoom = 1;
            public Mode ZoomMode { get; set; } = Mode.None;
            public Point? ZoomTarget { get; set; } = null;
        }

        public InterfaceSettings InterfaceSettings { get; } = appearance;
        public PrintSettings PrintSettings { get; } = settings;
        public ModelValue<PrintDocument> PrintDocument { get; } = new(dispatcher, document);
        public ModelValue<ModelDocument> PreviewDocument { get; } = new(dispatcher, new());

        public ModelValue<bool> IsError { get; } = new(dispatcher, false);
        public ModelValue<object> ErrorContent { get; } = new(dispatcher, string.Empty);
        public Action? ErrorCallback { get; set; } = null;

        public ModelValue<bool> IsPrintersReady { get; } = new(dispatcher, true);
        public ModelValue<bool> IsSettingsReady { get; } = new(dispatcher, true);
        public ModelValue<bool> IsDocumentReady { get; } = new(dispatcher, true);

        public ObservableCollection<PrintQueue> PrinterEntries { get; } = [];
        public ModelValue<PrintQueue?> Printer { get; } = new(dispatcher, null, printerCallback);
        public ModelValue<int> CopiesMaximum { get; } = new(dispatcher, settings.Fallbacks.FallbackMaximumCopies);
        public ModelValue<int> Copies { get; } = new(dispatcher, settings.Copies);
        public ModelCollection<Enums.Collation> CollationEntries { get; } = new(dispatcher, Enum.GetValues(typeof(Enums.Collation)).Cast<Enums.Collation>(), settings.Collation, Enums.Collation.Collated);
        public ModelValue<bool> IsCollationSupported { get; } = new(dispatcher, settings.Fallbacks.FallbackIsCollationSupported);
        public ModelCollection<Enums.Pages> PagesEntries { get; } = new(dispatcher, Enum.GetValues(typeof(Enums.Pages)).Cast<Enums.Pages>(), settings.Pages, Enums.Pages.AllPages, settingsCallback);
        public ModelValue<double> PagesCurrent { get; } = new(dispatcher, 1);
        public ModelValue<string> PagesCustom { get; } = new(dispatcher, settings.CustomPages, settingsCallback);
        public ModelCollection<Enums.Layout> LayoutEntries { get; } = new(dispatcher, Enum.GetValues(typeof(Enums.Layout)).Cast<Enums.Layout>(), settings.Layout, Enums.Layout.Portrait, settingsCallback);
        public ModelCollection<Enums.Size> SizeEntries { get; } = new(dispatcher, [], settings.Size, settings.Fallbacks.FallbackSize, settingsCallback);
        public ModelCollection<Enums.Color> ColorEntries { get; } = new(dispatcher, [], settings.Color, settings.Fallbacks.FallbackColor, settingsCallback);
        public ModelCollection<Enums.Quality> QualityEntries { get; } = new(dispatcher, [], settings.Quality, settings.Fallbacks.FallbackQuality);
        public ModelCollection<Enums.PagesPerSheet> PagesPerSheetEntries { get; } = new(dispatcher, Enum.GetValues(typeof(Enums.PagesPerSheet)).Cast<Enums.PagesPerSheet>(), settings.PagesPerSheet, Enums.PagesPerSheet.One, settingsCallback);
        public ModelCollection<Enums.PageOrder> PageOrderEntries { get; } = new(dispatcher, Enum.GetValues(typeof(Enums.PageOrder)).Cast<Enums.PageOrder>(), settings.PageOrder, Enums.PageOrder.Horizontal, settingsCallback);
        public ModelCollection<Enums.Scale> ScaleEntries { get; } = new(dispatcher, Enum.GetValues(typeof(Enums.Scale)).Cast<Enums.Scale>(), settings.Scale, Enums.Scale.AutoFit, settingsCallback);
        public ModelValue<int> ScaleCustom { get; } = new(dispatcher, settings.CustomScale, settingsCallback);
        public ModelCollection<Enums.Margin> MarginEntries { get; } = new(dispatcher, Enum.GetValues(typeof(Enums.Margin)).Cast<Enums.Margin>(), settings.Margin, Enums.Margin.Default, settingsCallback);
        public ModelValue<int> MarginCustom { get; } = new(dispatcher, settings.CustomMargin, settingsCallback);
        public ModelCollection<Enums.DoubleSided> DoubleSidedEntries { get; } = new(dispatcher, Enum.GetValues(typeof(Enums.DoubleSided)).Cast<Enums.DoubleSided>(), settings.DoubleSided, Enums.DoubleSided.OneSided);
        public ModelValue<bool> IsDoubleSidedSupported { get; } = new(dispatcher, settings.Fallbacks.FallbackIsDoubleSidedSupported);
        public ModelCollection<Enums.Type> TypeEntries { get; } = new(dispatcher, [], settings.Type, settings.Fallbacks.FallbackType);
        public ModelCollection<Enums.Source> SourceEntries { get; } = new(dispatcher, [], settings.Source, settings.Fallbacks.FallbackSource);
    }

    partial class PrintDialogPage : UserControl
    {
        public const double EPSILON = 0.01;

        private readonly PrintDialogWindow owner;
        private readonly PrintPageViewModel model;
        private readonly (PrintServer Server, bool IsProvided) server;

        private double margin = 0;
        private VirtualizingStackPanel? viewer = null;
        private (Task Task, CancellationTokenSource Cancellation)? task = null;

        public PrintDialogPage(PrintDialogWindow window, PrintDialog dialog)
        {
            InitializeComponent();

            if (dialog.Document == null)
            {
                throw new InvalidOperationException("The document is unset.");
            }

            owner = window;
            model = new(Dispatcher, dialog.Document, dialog.Interface, dialog.DefaultSettings, LoadSettings, LoadDocument);
            server = (dialog.PrintServer ?? new(), dialog.PrintServer != null);
            DataContext = model;

            foreach (PrintQueue printer in server.Server.GetPrintQueues([EnumeratedPrintQueueTypes.Fax]))
            {
                PrinterToIconConverter.CollectionFax.Add(printer);
            }
            foreach (PrintQueue printer in server.Server.GetPrintQueues([EnumeratedPrintQueueTypes.Shared, EnumeratedPrintQueueTypes.Connections]))
            {
                PrinterToIconConverter.CollectionNetwork.Add(printer);
            }
            LoadPrinters(dialog.DefaultPrinter);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            StopTask(false);
            if (!server.IsProvided)
            {
                server.Server.Dispose();
            }
        }

        private void CloseErrorDialog(Wpf.Ui.Controls.ContentDialog sender, Wpf.Ui.Controls.ContentDialogButtonClickEventArgs e)
        {
            model.IsError.Value = false;
            model.ErrorCallback?.Invoke();
        }

        private async void StopTask(bool wait = true)
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
            if (wait)
            {
                await task.Value.Task;
            }
        }

        private async void RunTask(Func<CancellationToken, Task> callback)
        {
            StopTask();

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
            await task.Value.Task;
        }

        private void LoadPrinters(PrintQueue? selection = null)
        {
            if (!model.IsPrintersReady.Value)
            {
                return;
            }

            model.IsPrintersReady.Value = false;

            PrinterComparer comparer = new();
            foreach (PrintQueue printer in server.Server.GetPrintQueues())
            {
                if (!model.PrinterEntries.Contains(printer, comparer))
                {
                    model.PrinterEntries.Add(printer);
                }

                if (selection != null && comparer.Equals(printer, selection))
                {
                    model.Printer.Value = printer;
                }
            }

            if (model.PrinterEntries.Count <= 0)
            {
                model.ErrorContent.Value = PrintPageViewModel.StringResources["StringResource_MessageNoPrinter"];
                model.ErrorCallback = () =>
                {
                    owner.ReturnValue = false;
                    owner.Close();
                };
                model.IsError.Value = true;
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
            if (sender is not Selector container || container.SelectedItem is PrintQueue)
            {
                return;
            }

            container.GetBindingExpression(Selector.SelectedItemProperty).UpdateTarget();

            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "ms-settings:printers",
                    UseShellExecute = true
                });
            }
            catch
            {
                model.ErrorContent.Value = PrintPageViewModel.StringResources["StringResource_MessageFailedAddPrinter"];
                model.ErrorCallback = null;
                model.IsError.Value = true;
            }
        }

        private void PrinterPreferences(object sender, RoutedEventArgs e)
        {
            if (model.Printer.Value == null)
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C rundll32 printui.dll,PrintUIEntry /p /n \"{model.Printer.Value.FullName}\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                });
            }
            catch
            {
                model.ErrorContent.Value = PrintPageViewModel.StringResources["StringResource_MessageFailedPrinterPreferences"];
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

            RunTask(async token =>
            {
                model.IsSettingsReady.Value = false;
                model.IsDocumentReady.Value = false;

                PrintTicket? defaults = null;
                try
                {
                    defaults = await Dispatcher.InvokeAsync(() => model.Printer.Value.DefaultPrintTicket);
                }
                catch { }
                token.ThrowIfCancellationRequested();

                PrintCapabilities? capabilities = null;
                try
                {
                    capabilities = await Dispatcher.InvokeAsync(() => model.Printer.Value.GetPrintCapabilities());
                }
                catch { }
                token.ThrowIfCancellationRequested();

                model.CopiesMaximum.Value = Math.Min(model.PrintSettings.Fallbacks.FallbackMaximumCopies, capabilities?.MaxCopyCount ?? int.MaxValue);
                model.Copies.Value = Math.Min(model.CopiesMaximum.Value, model.Copies.Value);
                token.ThrowIfCancellationRequested();

                model.IsCollationSupported.Value = capabilities?.CollationCapability.Any(x => x == Collation.Collated) ?? model.PrintSettings.Fallbacks.FallbackIsCollationSupported;
                if (!model.IsCollationSupported.Value)
                {
                    model.CollationEntries.SilentSelect(Enums.Collation.Uncollated);
                }
                token.ThrowIfCancellationRequested();

                List<Enums.Size> sizes = [];
                Enums.Size? sizeDefault = null;
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
                            token.ThrowIfCancellationRequested();

                            string? widthText = node.SelectSingleNode(string.Format(search, "MediaSizeWidth"), namespaces)?.InnerText;
                            string? heightText = node.SelectSingleNode(string.Format(search, "MediaSizeHeight"), namespaces)?.InnerText;
                            if (widthText == null || heightText == null || !double.TryParse(widthText, out double widthValue) || !double.TryParse(heightText, out double heightValue))
                            {
                                continue;
                            }

                            Enums.Size size = new()
                            {
                                DefinedName = ValueMappings.Map(node.Attributes?["name"]?.Value.Split(':').Last().ToLowerInvariant(), ValueMappings.XmlSizeNameMapping),
                                FallbackName = node.SelectSingleNode(string.Format(search, "DisplayName"), namespaces)?.InnerText,
                                Width = widthValue / 10000 / 2.54 * 96,
                                Height = heightValue / 10000 / 2.54 * 96
                            };
                            sizes.Add(size);

                            if (size.Equals(defaults?.PageMediaSize))
                            {
                                sizeDefault = size;
                            }
                        }
                    }
                }
                catch { }
                model.SizeEntries.Load(sizes, sizeDefault, x => x);
                token.ThrowIfCancellationRequested();

                model.ColorEntries.Load(capabilities?.OutputColorCapability, defaults?.OutputColor, x => ValueMappings.Map(x, ValueMappings.ColorMapping));
                model.QualityEntries.Load(capabilities?.OutputQualityCapability, defaults?.OutputQuality, x => ValueMappings.Map(x, ValueMappings.QualityMapping));
                token.ThrowIfCancellationRequested();

                model.IsDoubleSidedSupported.Value = capabilities?.DuplexingCapability.Any(x => x == Duplexing.TwoSidedShortEdge || x == Duplexing.TwoSidedLongEdge) ?? model.PrintSettings.Fallbacks.FallbackIsDoubleSidedSupported;
                if (!model.IsDoubleSidedSupported.Value)
                {
                    model.DoubleSidedEntries.SilentSelect(Enums.DoubleSided.OneSided);
                }
                token.ThrowIfCancellationRequested();

                model.TypeEntries.Load(capabilities?.PageMediaTypeCapability, defaults?.PageMediaType, x => ValueMappings.Map(x, ValueMappings.TypeMapping));
                model.SourceEntries.Load(capabilities?.InputBinCapability, defaults?.InputBin, x => ValueMappings.Map(x, ValueMappings.SourceMapping));
                token.ThrowIfCancellationRequested();

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

            RunTask(async token =>
            {
                model.IsDocumentReady.Value = false;

                //TODO: document update callback
                model.PrintDocument.NotifyUpdate();
                token.ThrowIfCancellationRequested();

                bool isLandscape = model.LayoutEntries.Selection == Enums.Layout.Landscape;
                Enums.Size dimension = model.SizeEntries.Selection ?? model.PrintSettings.Fallbacks.FallbackSize;
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
                    Enums.Margin.Minimum => await ((Func<Task<double>>)(async () =>
                    {
                        try
                        {
                            PageImageableArea area = await Dispatcher.InvokeAsync(() => model.Printer.Value.GetPrintCapabilities(new()
                            {
                                PageMediaSize = new(dimension.Width, dimension.Height),
                                PageOrientation = ValueMappings.Map(model.LayoutEntries.Selection, ValueMappings.LayoutMapping)
                            }).PageImageableArea);
                            return (new double[] { area.OriginWidth, area.OriginHeight, (isLandscape ? dimension.Height : dimension.Width) - area.OriginWidth - area.ExtentWidth, (isLandscape ? dimension.Width : dimension.Height) - area.OriginHeight - area.ExtentHeight }).Max();
                        }
                        catch
                        {
                            return 0;
                        }
                    }))(),
                    Enums.Margin.Custom => model.MarginCustom.Value,
                    _ => model.PrintDocument.Value.DocumentMargin
                };
                token.ThrowIfCancellationRequested();

                //TODO: performance enhancement (clip on the ContentControl does not appear to work)
                Effect? color = model.ColorEntries.Selection switch
                {
                    Enums.Color.Grayscale => await Dispatcher.InvokeAsync(() => new DocumentColorEffect("Grayscale")),
                    Enums.Color.Monochrome => await Dispatcher.InvokeAsync(() => new DocumentColorEffect("Monochrome")),
                    _ => null
                };
                if (color != null)
                {
                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (color.CanFreeze)
                        {
                            color.Freeze();
                        }
                    });
                }
                token.ThrowIfCancellationRequested();

                List<int> pages = model.PagesEntries.Selection switch
                {
                    Enums.Pages.CurrentPage => [((Func<int>)(() => {
                        lock (model.PreviewDocument.Value.DocumentLock) {
                            return model.PreviewDocument.Value.Document[Math.Max(0, Math.Min(model.PreviewDocument.Value.Document.Count - 1, (int)Math.Floor(model.PagesCurrent.Value + EPSILON) - 1))].Index;
                        }
                    }))()],
                    Enums.Pages.CustomPages => CustomPagesValidationRule.TryConvert(model.PagesCustom.Value, model.PrintDocument.Value.PageCount).Result,
                    _ => []
                };
                token.ThrowIfCancellationRequested();

                lock (model.PreviewDocument.Value.DocumentLock)
                {
                    model.PreviewDocument.Value.Document.Clear();
                }
                Size size = isLandscape ? new(dimension.Height, dimension.Width) : new(dimension.Width, dimension.Height);
                Size boundary = new(size.Width - margin * 2, size.Height - margin * 2);
                token.ThrowIfCancellationRequested();

                int start = 0;
                int index = 0;
                int subindex = 0;
                FixedPage? page = null;
                foreach (PrintPage content in model.PrintDocument.Value.Pages)
                {
                    token.ThrowIfCancellationRequested();

                    index++;
                    if (pages.Count > 0 && !pages.Contains(index))
                    {
                        continue;
                    }

                    subindex++;
                    int step = (subindex - 1) % arrangement.Count;
                    (int column, int row) = model.PageOrderEntries.Selection switch
                    {
                        Enums.PageOrder.HorizontalReverse => (arrangement.Columns - step % arrangement.Columns - 1, step / arrangement.Columns),
                        Enums.PageOrder.Vertical => (step / arrangement.Rows, step % arrangement.Rows),
                        Enums.PageOrder.VerticalReverse => (step / arrangement.Rows, arrangement.Rows - step % arrangement.Rows - 1),
                        _ => (step % arrangement.Columns, step / arrangement.Columns)
                    };
                    double width = boundary.Width / arrangement.Columns;
                    double height = boundary.Height / arrangement.Rows;

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (page == null)
                        {
                            start = index;
                            page = new()
                            {
                                Width = size.Width,
                                Height = size.Height,
                                Effect = color
                            };
                        }

                        FrameworkElement? element = content.Content;
                        if (element != null)
                        {
                            if (element.Parent != null)
                            {
                                if (element.Parent is not Decorator parent)
                                {
                                    throw new PrintDocumentException(element, "The content is already the child of another element.");
                                }
                                parent.Child = null;
                            }

                            Decorator container = new()
                            {
                                Child = element,
                                Width = model.PrintDocument.Value.DocumentSize != null ? model.PrintDocument.Value.DocumentSize.Value.Width - model.PrintDocument.Value.DocumentMargin * 2 : boundary.Width,
                                Height = model.PrintDocument.Value.DocumentSize != null ? model.PrintDocument.Value.DocumentSize.Value.Height - model.PrintDocument.Value.DocumentMargin * 2 : boundary.Height
                            };

                            double factor = scale ?? Math.Min(width / container.Width, height / container.Height);
                            factor = double.IsNaN(factor) ? 0 : factor;

                            container.RenderTransform = new ScaleTransform(factor, factor, 0, 0);
                            container.Clip = new RectangleGeometry(new(0, 0, width / factor, height / factor));

                            FixedPage.SetLeft(container, margin + column * width);
                            FixedPage.SetTop(container, margin + row * height);
                            page.Children.Add(container);
                        }
                    });

                    if (page != null && subindex % arrangement.Count <= 0)
                    {
                        lock (model.PreviewDocument.Value.DocumentLock)
                        {
                            model.PreviewDocument.Value.Document.Add(new(start, page));
                        }
                        page = null;
                    }
                }
                if (page != null)
                {
                    lock (model.PreviewDocument.Value.DocumentLock)
                    {
                        model.PreviewDocument.Value.Document.Add(new(start, page));
                    }
                }
                token.ThrowIfCancellationRequested();

                model.PreviewDocument.Value.Zoom = ZoomHorizontal();
                model.PreviewDocument.Value.ZoomMode = PrintPageViewModel.ModelDocument.Mode.FitToWidth;
                model.PreviewDocument.NotifyUpdate();
                model.IsDocumentReady.Value = true;
            });
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            if (model.Printer.Value == null)
            {
                return;
            }

            model.IsSettingsReady.Value = false;

            FixedDocument document = new();
            Enums.Size dimension = model.SizeEntries.Selection ?? model.PrintSettings.Fallbacks.FallbackSize;
            document.DocumentPaginator.PageSize = model.LayoutEntries.Selection == Enums.Layout.Landscape ? new(dimension.Height, dimension.Width) : new(dimension.Width, dimension.Height);
            foreach (PrintPageViewModel.ModelDocument.ModelPage page in model.PreviewDocument.Value.Document)
            {
                if (page.Content.Parent != null)
                {
                    if (page.Content.Parent is not ContentPresenter parent)
                    {
                        throw new PrintDocumentException(page.Content, "The content is already the child of another element.");
                    }
                    parent.Content = null;
                }

                document.Pages.Add(new()
                {
                    Child = page.Content
                });
            }

            //TODO: use own writer to the print queue to make this async
            new System.Windows.Controls.PrintDialog()
            {
                PrintQueue = model.Printer.Value,
                PrintTicket = new()
                {
                    CopyCount = model.Copies.Value,
                    Collation = ValueMappings.Map(model.CollationEntries.Selection, ValueMappings.CollationMapping),
                    PageOrientation = ValueMappings.Map(model.LayoutEntries.Selection, ValueMappings.LayoutMapping),
                    PageMediaSize = new(dimension.DefinedName != null ? ValueMappings.Map(dimension.DefinedName, ValueMappings.SizeNameMapping) : PageMediaSizeName.Unknown, dimension.Width, dimension.Height),
                    OutputColor = ValueMappings.Map(model.ColorEntries.Selection, ValueMappings.ColorMapping),
                    OutputQuality = ValueMappings.Map(model.QualityEntries.Selection, ValueMappings.QualityMapping),
                    PagesPerSheet = 1,
                    PagesPerSheetDirection = PagesPerSheetDirection.RightBottom,
                    PageScalingFactor = 100,
                    Duplexing = ValueMappings.Map(model.DoubleSidedEntries.Selection, ValueMappings.DoubleSidedMapping),
                    PageMediaType = ValueMappings.Map(model.TypeEntries.Selection, ValueMappings.TypeMapping),
                    InputBin = ValueMappings.Map(model.SourceEntries.Selection, ValueMappings.SourceMapping)
                }
            }.PrintDocument(document.DocumentPaginator, model.PrintDocument.Value.DocumentName);

            owner.ReturnValue = true;
            owner.TotalPapers = (int)Math.Ceiling(model.PreviewDocument.Value.Document.Count * model.Copies.Value * (model.DoubleSidedEntries.Selection == Enums.DoubleSided.OneSided ? 1 : 0.5));
            owner.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            owner.ReturnValue = false;
            owner.Close();
        }

        private void InitializeViewer(object sender, RoutedEventArgs e)
        {
            //TODO: support for keyboard shorcuts
            if (sender is VirtualizingStackPanel target)
            {
                viewer = target;
            }

            if (FindResource("SizePageMargin") is Thickness space && FindResource("SizePageBorder") is Thickness border)
            {
                margin = space.Left + border.Left;
            }
        }

        private void UpdateViewer(object sender, ScrollChangedEventArgs e)
        {
            if (!model.IsDocumentReady.Value || viewer == null)
            {
                return;
            }

            if (model.PreviewDocument.Value.ZoomTarget != null)
            {
                if (e.ExtentHeightChange != 0)
                {
                    if (!double.IsNaN(model.PreviewDocument.Value.ZoomTarget.Value.X))
                    {
                        viewer.SetHorizontalOffset(model.PreviewDocument.Value.ZoomTarget.Value.X);
                    }
                    if (!double.IsNaN(model.PreviewDocument.Value.ZoomTarget.Value.Y))
                    {
                        viewer.SetVerticalOffset(model.PreviewDocument.Value.ZoomTarget.Value.Y);
                    }
                }
                else
                {
                    model.PreviewDocument.Value.ZoomTarget = null;
                }
            }
            model.PagesCurrent.Value = Math.Max(1, Math.Min(model.PreviewDocument.Value.Document.Count, viewer.VerticalOffset / FindPageHeight() * model.PreviewDocument.Value.ColumnCount + 1));
        }

        private double FindPageWidth(double? zoom = null)
        {
            if (model.SizeEntries.Selection == null)
            {
                return 0;
            }

            return (model.LayoutEntries.Selection switch
            {
                Enums.Layout.Landscape => model.SizeEntries.Selection.Value.Height,
                _ => model.SizeEntries.Selection.Value.Width
            }) * (zoom ?? model.PreviewDocument.Value.Zoom) + margin * 2;
        }

        private double FindPageHeight(double? zoom = null)
        {
            if (model.SizeEntries.Selection == null)
            {
                return 0;
            }

            return (model.LayoutEntries.Selection switch
            {
                Enums.Layout.Landscape => model.SizeEntries.Selection.Value.Width,
                _ => model.SizeEntries.Selection.Value.Height
            }) * (zoom ?? model.PreviewDocument.Value.Zoom) + margin * 2;
        }

        private double ZoomDelta(double delta)
        {
            double interval = Math.Abs(delta);
            double step = model.PreviewDocument.Value.Zoom / interval;
            return delta switch
            {
                > 0 => ((Math.Ceiling(step) - step < 0.35 ? Math.Ceiling(step) : Math.Floor(step)) + 1) * interval,
                < 0 => ((step - Math.Floor(step) < 0.35 ? Math.Floor(step) : Math.Ceiling(step)) - 1) * interval,
                _ => model.PreviewDocument.Value.Zoom
            };
        }

        private double ZoomHorizontal(double padding = 16)
        {
            if (model.SizeEntries.Selection == null || viewer == null)
            {
                return 1;
            }

            return (viewer.ViewportWidth - padding * model.PreviewDocument.Value.ColumnCount) / FindPageWidth(1) / model.PreviewDocument.Value.ColumnCount;
        }


        private double ZoomVertical(double padding = 12)
        {
            if (model.SizeEntries.Selection == null || viewer == null)
            {
                return 1;
            }

            return (viewer.ViewportHeight - padding) / FindPageHeight(1);
        }

        private void ZoomResume(object sender, SizeChangedEventArgs e)
        {
            if (model.PreviewDocument.Value.ZoomMode == PrintPageViewModel.ModelDocument.Mode.None)
            {
                return;
            }

            model.PreviewDocument.Value.Zoom = model.PreviewDocument.Value.ZoomMode switch
            {
                PrintPageViewModel.ModelDocument.Mode.FitToWidth => ZoomHorizontal(),
                PrintPageViewModel.ModelDocument.Mode.FitToHeight => ZoomVertical(),
                PrintPageViewModel.ModelDocument.Mode.FitToPage => Math.Min(ZoomHorizontal(), ZoomVertical()),
                _ => 1
            };
            model.PreviewDocument.NotifyUpdate();
        }

        private void ZoomCustom(object sender, MouseWheelEventArgs e)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) || viewer == null)
            {
                return;
            }

            Point position = e.GetPosition(viewer);
            double x = (viewer.HorizontalOffset + position.X) / model.PreviewDocument.Value.Zoom;
            double y = (viewer.VerticalOffset + position.Y) / model.PreviewDocument.Value.Zoom;
            double zoom = 0.15 * Math.Sign(e.Delta);
            model.PreviewDocument.Value.Zoom *= 1 + zoom;
            model.PreviewDocument.Value.ZoomMode = PrintPageViewModel.ModelDocument.Mode.None;
            model.PreviewDocument.Value.ZoomTarget = new(x * model.PreviewDocument.Value.Zoom - position.X, y * model.PreviewDocument.Value.Zoom - position.Y - Math.Floor(model.PagesCurrent.Value + EPSILON) * margin * zoom * 2);
            model.PreviewDocument.NotifyUpdate();
            e.Handled = true;
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            model.PreviewDocument.Value.Zoom = ZoomDelta(0.25);
            model.PreviewDocument.Value.ZoomMode = PrintPageViewModel.ModelDocument.Mode.None;
            model.PreviewDocument.NotifyUpdate();
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            model.PreviewDocument.Value.Zoom = ZoomDelta(-0.25);
            model.PreviewDocument.Value.ZoomMode = PrintPageViewModel.ModelDocument.Mode.None;
            model.PreviewDocument.NotifyUpdate();
        }

        private void ZoomActual(object sender, RoutedEventArgs e)
        {
            model.PreviewDocument.Value.Zoom = 1;
            model.PreviewDocument.Value.ZoomMode = PrintPageViewModel.ModelDocument.Mode.None;
            model.PreviewDocument.NotifyUpdate();
        }

        private void ZoomFit(object sender, RoutedEventArgs e)
        {
            model.PreviewDocument.Value.Zoom = ZoomHorizontal();
            model.PreviewDocument.Value.ZoomMode = PrintPageViewModel.ModelDocument.Mode.FitToWidth;
            model.PreviewDocument.NotifyUpdate();
        }

        private void ZoomPage(int column)
        {
            model.PreviewDocument.Value.ColumnCount = column;
            model.PreviewDocument.Value.Zoom = Math.Min(ZoomHorizontal(), ZoomVertical());
            model.PreviewDocument.Value.ZoomMode = PrintPageViewModel.ModelDocument.Mode.FitToPage;
            model.PreviewDocument.Value.ZoomTarget = new(double.NaN, FindPageHeight(model.PreviewDocument.Value.Zoom) * Math.Floor((Math.Round(model.PagesCurrent.Value) - 1) / column));
            model.PreviewDocument.NotifyUpdate();
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
            if (index < 1 || index > model.PreviewDocument.Value.Document.Count || viewer == null)
            {
                return;
            }

            viewer.SetVerticalOffset(FindPageHeight() * Math.Floor((index - 1) / model.PreviewDocument.Value.ColumnCount));
        }

        private void NavigatePageFirst(object sender, RoutedEventArgs e)
        {
            NavigatePage(1);
        }

        private void NavigatePagePrevious(object sender, RoutedEventArgs e)
        {
            NavigatePage(Math.Ceiling(model.PagesCurrent.Value - EPSILON) - model.PreviewDocument.Value.ColumnCount);
        }

        private void NavigatePageNext(object sender, RoutedEventArgs e)
        {
            NavigatePage(Math.Floor(model.PagesCurrent.Value + EPSILON) + model.PreviewDocument.Value.ColumnCount);
        }

        private void NavigatePageLast(object sender, RoutedEventArgs e)
        {
            NavigatePage(model.PreviewDocument.Value.Document.Count);
        }
    }
}