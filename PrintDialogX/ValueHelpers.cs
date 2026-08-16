using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Printing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintDialogX
{
    internal abstract class LanguageHostConverter()
    {
        public ResourceDictionary? Resources { get; set; } = null;
    }

    internal sealed class InterfaceToContentConverter() : IValueConverter
    {
        public const string NAME_LANGUAGE_DEFAULT = "en-US";

        public ResourceDictionary Resources { get; set; } = [];

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not InterfaceSettings settings)
            {
                return Binding.DoNothing;
            }

            ControlTemplate template = (ControlTemplate)parameter;
            ContentControl content = new()
            {
                Template = template,
                Focusable = false
            };
            content.ApplyTemplate();

            ApplyInterface((Panel)template.FindName("PART_Basic", content), settings.BasicSettings, Resources);
            ApplyInterface((Panel)template.FindName("PART_Advanced", content), settings.AdvancedSettings, Resources);

            Expander expander = (Expander)template.FindName("PART_Expander", content);
            expander.IsExpanded = settings.IsSettingsExpanded;
            if (settings.AdvancedSettings.Length <= 0)
            {
                expander.Visibility = Visibility.Collapsed;
            }

            return content;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static void ApplyInterface(Panel container, IEnumerable<InterfaceSettings.Option> options, ResourceDictionary resources)
        {
            foreach (InterfaceSettings.Option option in options)
            {
                container.Children.Add(new ContentControl()
                {
                    Template = (ControlTemplate)resources[option],
                    Focusable = false
                });
            }
        }

        public static void ApplyLanguage(InterfaceSettings.Language language, Action<string, FlowDirection, ResourceDictionary> applier)
        {
            if (language == InterfaceSettings.Language.System)
            {
                string[] current = CultureInfo.CurrentUICulture.IetfLanguageTag.Split('-');
                language = (current.First(), current.Length > 1 ? current[1] : null) switch
                {
                    ("en", "CA") => InterfaceSettings.Language.en_CA,
                    ("en", "GB") => InterfaceSettings.Language.en_GB,
                    ("en", _) => InterfaceSettings.Language.en_US,
                    ("pl", _) => InterfaceSettings.Language.pl_PL,
                    ("yue", "HK") => InterfaceSettings.Language.zh_HK,
                    ("yue", "TW") => InterfaceSettings.Language.zh_TW,
                    ("yue", _) => InterfaceSettings.Language.zh_HK,
                    ("zh", "HK") => InterfaceSettings.Language.zh_HK,
                    ("zh", "TW") => InterfaceSettings.Language.zh_TW,
                    ("zh", "Hans") => InterfaceSettings.Language.zh_CN,
                    ("zh", "Hant") => InterfaceSettings.Language.zh_HK,
                    ("zh", _) => InterfaceSettings.Language.zh_CN,
                    _ => InterfaceSettings.Language.en_US
                };
            }

            LanguageAttribute? attribute = Enum.GetName(typeof(InterfaceSettings.Language), language) is string name ? typeof(InterfaceSettings.Language).GetField(name)?.GetCustomAttribute<LanguageAttribute>() : null;
            applier(attribute?.Language ?? NAME_LANGUAGE_DEFAULT, attribute?.Direction ?? FlowDirection.LeftToRight, new()
            {
                Source = new(string.Format(CultureInfo.InvariantCulture, "/PrintDialogX;component/Resources/Languages/{0}.xaml", attribute?.Language ?? NAME_LANGUAGE_DEFAULT), UriKind.Relative)
            });
        }
    }

    internal sealed class CompositeItemTemplateSelector() : DataTemplateSelector()
    {
        public DataTemplate? Data { get; set; } = null;
        public DataTemplate? Element { get; set; } = null;
        public DataTemplate? Decoration { get; set; } = null;

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                Separator => Decoration,
                FrameworkElement => Element,
                _ => Data
            };
        }
    }

    internal sealed class CompositeContainerStyleSelector() : StyleSelector()
    {
        public Style? Data { get; set; } = null;
        public Style? Element { get; set; } = null;
        public Style? Decoration { get; set; } = null;

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            return item switch
            {
                Separator => Decoration,
                FrameworkElement => Element,
                _ => Data
            };
        }
    }

    internal sealed class ValueToDescriptionConverter() : LanguageHostConverter(), IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return value is Enum entry && Resources is not null ? GetDescription(entry, Resources) : Binding.DoNothing;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static object GetDescription(Enum value, ResourceDictionary resources)
        {
            return Enum.GetName(value.GetType(), value) is string name && value.GetType().GetField(name)?.GetCustomAttribute<StringResourceAttribute>()?.Resource is TextResource resource ? resources[resource] : value;
        }
    }

    internal sealed class ComparisonToStateConverter() : IValueConverter
    {
        internal enum Comparison
        {
            Equality,
            Threshold
        }

        public Comparison? Mode { get; set; } = null;
        public object StateTrue { get; set; } = true;
        public object StateFalse { get; set; } = false;

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            IEnumerable<object> targets = parameter is object[] parameters ? (parameters[0] is IEnumerable<object> collection ? collection : [parameters[0]]) : [parameter];

            return (Mode switch
            {
                Comparison.Equality => targets.Any(x => Equals(value, x)),
                Comparison.Threshold => targets.Any(x => value is IComparable threshold && threshold.CompareTo(x) >= 0),
                _ => false
            }) ? StateTrue : StateFalse;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            if (parameter is not object[] parameters)
            {
                return Binding.DoNothing;
            }

            object result = Equals(value, StateTrue) ? parameters[0] : parameters[1];

            return result is IEnumerable<object> collection ? collection.First() : result;
        }
    }

    internal sealed class CollectionToRangeConverter() : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable collection || parameter is not int[] range)
            {
                return Binding.DoNothing;
            }

            List<object> result = [.. collection];

            return result.GetRange(range[0], result.Count - range[0] - range[1]);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class CollectionToBooleanConverter() : IMultiValueConverter
    {
        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            return values.All(x => x as bool? ?? false);
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. Enumerable.Repeat(Binding.DoNothing, types.Length)];
        }
    }

    internal sealed class PrinterComparer() : IEqualityComparer<PrintQueue>
    {
        public static readonly PrinterComparer Instance = new();

        public bool Equals(PrintQueue? x, PrintQueue? y)
        {
            try
            {
                return x is not null && y is not null && (ReferenceEquals(x, y) || StringComparer.Ordinal.Equals(x.FullName, y.FullName));
            }
            catch
            {
                return false;
            }
        }

        public int GetHashCode(PrintQueue value)
        {
            try
            {
                return StringComparer.Ordinal.GetHashCode(value.FullName);
            }
            catch
            {
                return value.GetHashCode();
            }
        }
    }

    internal sealed class PrinterToIconConverter() : LanguageHostConverter(), IValueConverter
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct N_IconInfo
        {
            public uint Size;
            public IntPtr Icon;
            public int SystemIndex;
            public int ResourceIndex;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string ResourcePath;
        }

        [DllImport("shell32.dll", EntryPoint = "SHGetStockIconInfo")]
        private static extern int N_GetIcon(uint index, uint flags, ref N_IconInfo info);

        [DllImport("user32.dll", EntryPoint = "DestroyIcon")]
        private static extern bool N_ReleaseIcon(IntPtr icon);

        internal sealed class PrinterIcon(ImageSource? icon, string name, double opacity, double size)
        {
            public ImageSource? Icon { get; } = icon;
            public string Name { get; } = name;
            public double Opacity { get; } = opacity;
            public double Size { get; } = size;
        }

        public const int RESULT_SUCCESS = 0;
        public const uint INDEX_PRINTER = 16u;
        public const uint INDEX_PRINTER_NETWORK = 50u;
        public const uint INDEX_PRINTER_FILE = 54u;
        public const uint INDEX_FAX = 52u;
        public const uint INDEX_FAX_NETWORK = 53u;
        public const uint FLAG_RECEIVE_ICON = 256u;
        public const uint FLAG_SIZE_SMALL = 1u;
        public const uint FLAG_SIZE_LARGE = 0u;

        public static readonly string[] FILTER_NETWORK = ["ip_", "wsd-"];
        public static readonly string[] FILTER_FILE = ["file:", "portprompt:", "nul:", "xpsport:", "c:\\", "d:\\"];
        public static readonly Dictionary<(uint, bool), ImageSource> CACHE = [];

        public double SizeSmall { get; set; } = 0;
        public double SizeLarge { get; set; } = 0;
        public PrintQueueCollection CollectionFax { get; set; } = [];
        public PrintQueueCollection CollectionNetwork { get; set; } = [];

        public object? Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer || parameter is not bool isSmall || Resources is null)
            {
                return Binding.DoNothing;
            }

            (uint index, TextResource name) = (CollectionFax.Contains(printer, PrinterComparer.Instance), CollectionNetwork.Contains(printer, PrinterComparer.Instance) || CheckFilter(printer, FILTER_NETWORK), CheckFilter(printer, FILTER_FILE)) switch
            {
                (true, true, _) => (INDEX_FAX_NETWORK, TextResource.LabelFaxNetwork),
                (true, _, _) => (INDEX_FAX, TextResource.LabelFax),
                (_, true, _) => (INDEX_PRINTER_NETWORK, TextResource.LabelPrinterNetwork),
                (_, _, true) => (INDEX_PRINTER_FILE, TextResource.LabelPrinterFile),
                _ => (INDEX_PRINTER, TextResource.LabelPrinter),
            };

            (uint, bool) key = (index, isSmall);
            if (!CACHE.TryGetValue(key, out ImageSource? icon))
            {
                try
                {
                    N_IconInfo info = new()
                    {
                        Size = (uint)Marshal.SizeOf<N_IconInfo>()
                    };
                    if (N_GetIcon(index, FLAG_RECEIVE_ICON | (isSmall ? FLAG_SIZE_SMALL : FLAG_SIZE_LARGE), ref info) == RESULT_SUCCESS && info.Icon != IntPtr.Zero)
                    {
                        icon = Imaging.CreateBitmapSourceFromHIcon(info.Icon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        N_ReleaseIcon(info.Icon);
                    }
                }
                catch { }

                if (icon is not null)
                {
                    icon.Freeze();
                    CACHE[key] = icon;
                }
            }

            bool isFaded = true;
            try
            {
                printer.Refresh();
                isFaded = printer.IsOffline;
            }
            catch { }

            return new PrinterIcon(icon, (string)Resources[name], isFaded ? 0.5 : 1, isSmall ? SizeSmall : SizeLarge);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static bool CheckFilter(PrintQueue printer, string[] filter)
        {
            try
            {
                return filter.Any(x => printer.QueuePort.Name.StartsWith(x, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }

    internal sealed class PrinterToStatusConverter() : LanguageHostConverter(), IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer || Resources is null)
            {
                return Binding.DoNothing;
            }

            try
            {
                printer.Refresh();
                return Resources[printer.QueueStatus switch
                {
                    PrintQueueStatus.None => TextResource.LabelReady,
                    PrintQueueStatus.Busy => TextResource.LabelBusy,
                    PrintQueueStatus.DoorOpen => TextResource.LabelDoorOpen,
                    PrintQueueStatus.Initializing => TextResource.LabelInitializing,
                    PrintQueueStatus.IOActive => TextResource.LabelIOActive,
                    PrintQueueStatus.ManualFeed => TextResource.LabelManualFeed,
                    PrintQueueStatus.NoToner => TextResource.LabelNoToner,
                    PrintQueueStatus.NotAvailable => TextResource.LabelNotAvailable,
                    PrintQueueStatus.Offline => TextResource.LabelOffline,
                    PrintQueueStatus.OutOfMemory => TextResource.LabelOutOfMemory,
                    PrintQueueStatus.OutputBinFull => TextResource.LabelOutputBinFull,
                    PrintQueueStatus.PagePunt => TextResource.LabelPagePunt,
                    PrintQueueStatus.PaperJam => TextResource.LabelPaperJam,
                    PrintQueueStatus.PaperOut => TextResource.LabelPaperOut,
                    PrintQueueStatus.PaperProblem => TextResource.LabelPaperProblem,
                    PrintQueueStatus.Paused => TextResource.LabelPaused,
                    PrintQueueStatus.PendingDeletion => TextResource.LabelPendingDeletion,
                    PrintQueueStatus.PowerSave => TextResource.LabelPowerSave,
                    PrintQueueStatus.Printing => TextResource.LabelPrinting,
                    PrintQueueStatus.Processing => TextResource.LabelProcessing,
                    PrintQueueStatus.ServerUnknown => TextResource.LabelServerUnknown,
                    PrintQueueStatus.TonerLow => TextResource.LabelTonerLow,
                    PrintQueueStatus.UserIntervention => TextResource.LabelUserIntervention,
                    PrintQueueStatus.Waiting => TextResource.LabelWaiting,
                    PrintQueueStatus.WarmingUp => TextResource.LabelWarmingUp,
                    _ => TextResource.LabelError
                }];
            }
            catch
            {
                return Resources[TextResource.LabelError];
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class PrinterToDescriptionConverter() : LanguageHostConverter(), IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer || Resources is null)
            {
                return Binding.DoNothing;
            }

            List<string> info = [];
            try
            {
                printer.Refresh();
            }
            catch { }

            try
            {
                info.Add(string.Format(culture, (string)Resources[TextResource.ConstructionDocuments], printer.NumberOfJobs));
            }
            catch { }
            try
            {
                info.Add(string.Format(culture, (string)Resources[TextResource.ConstructionLocation], string.IsNullOrWhiteSpace(printer.Location) ? Resources[TextResource.LabelUnknown] : printer.Location));
            }
            catch { }
            try
            {
                if (!string.IsNullOrWhiteSpace(printer.Comment))
                {
                    info.Add(string.Format(culture, (string)Resources[TextResource.ConstructionComment], printer.Comment));
                }
            }
            catch { }

            return string.Join(Environment.NewLine, info);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class PagesCustomValidationRule() : ValidationRule
    {
        public int Maximum { get; set; } = int.MaxValue;

        public override System.Windows.Controls.ValidationResult Validate(object value, CultureInfo culture)
        {
            return value is string pages && TryConvert(pages, Maximum, true).IsValid ? System.Windows.Controls.ValidationResult.ValidResult : new(false, string.Empty);
        }

        public static (bool IsValid, List<object>? Result) TryConvert(string value, int maximum, bool isValidation)
        {
            List<object>? pages = isValidation ? null : [];
            foreach (string entry in value.Split(',', ';', '，', '、', '､', '﹑', '،', '؛', '﹐').Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                string[] range = entry.Split('-', '\u2010', '\u2011', '\u2012', '\u2013', '\u2014', '\u2015', '\ufe58', '\ufe63', '\uff0d');
                switch (range.Length)
                {
                    case 1 when int.TryParse(range[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int single) && single > 0 && single <= maximum:
                        pages?.Add(single);
                        break;
                    case 2 when int.TryParse(range[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int first) && int.TryParse(range[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int second) && Math.Min(first, second) > 0 && Math.Max(first, second) <= maximum:
                        pages?.Add((Math.Min(first, second), Math.Max(first, second)));
                        break;
                    default:
                        return (false, null);
                }
            }

            return (true, pages);
        }
    }

    internal sealed class SizeToDescriptionConverter() : LanguageHostConverter(), IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not Enums.Size size || parameter is not bool isVerbose || Resources is null)
            {
                return Binding.DoNothing;
            }

            object? name = size.DefinedName is not null ? ValueToDescriptionConverter.GetDescription(size.DefinedName.Value, Resources) : size.FallbackName;
            string description = string.Format(culture, (string)Resources[TextResource.ConstructionSize], size.Width * PrintDialogControl.RATIO_CENTIMETER, size.Height * PrintDialogControl.RATIO_CENTIMETER);

            return isVerbose ? description : (name ?? string.Format(culture, (string)Resources[TextResource.ConstructionCustom], description));
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class SizeToMarginMaximumConverter() : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return value is Enums.Size size ? (int)Math.Min(size.Width / 2, size.Height / 2) : Binding.DoNothing;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class DocumentHostControl : Border
    {
        internal enum DocumentZoom
        {
            Custom,
            FitToWidth,
            FitToHeight,
            FitToPage
        }

        internal sealed class Document(PrintDialogViewModel.ModelLocker locker) : DocumentPaginator
        {
            public const double PERCENTAGE_ZOOM_MINIMUM = 0.05;
            public const double PERCENTAGE_ZOOM_MAXIMUM = 10000;

            public PrintDialogViewModel.ModelLocker Locker { get; } = locker;
            public List<(int Index, DocumentPage Page)> Pages { get; } = [];

            public VirtualizingStackPanel? Viewer { get; set; } = null;

            public double ZoomValue
            {
                get;
                set => field = Math.Max(PERCENTAGE_ZOOM_MINIMUM, Math.Min(PERCENTAGE_ZOOM_MAXIMUM, value));
            } = 1;
            public DocumentZoom ZoomMode { get; set; } = DocumentZoom.FitToWidth;
            public Point? ZoomTarget { get; set; } = null;
            public int ColumnCount { get; set; } = 1;

            public override bool IsPageCountValid { get => true; }
            public override int PageCount { get => Pages.Count; }
            public override Size PageSize { get; set; } = new();
            public override IDocumentPaginatorSource? Source { get => null; }

            public override System.Windows.Documents.DocumentPage GetPage(int index)
            {
                using (Locker.Lock())
                {
                    if (index < 0 || index >= PageCount)
                    {
                        return System.Windows.Documents.DocumentPage.Missing;
                    }

                    Canvas content = Pages[index].Page.UpdateContent();
                    content.Measure(PageSize);
                    content.Arrange(new(PageSize));

                    return new(content, PageSize, new(PageSize), new(PageSize));
                }
            }
        }

        internal sealed class DocumentPage(IEnumerable<PrintPage> chunk, DocumentSettings settings)
        {
            private Canvas? content = null;

            public Canvas UpdateContent()
            {
                if (content is not null)
                {
                    return content;
                }

                content = new();

                int index = 0;
                foreach (PrintPage page in chunk)
                {
                    (int column, int row) = settings.Order switch
                    {
                        Enums.PageOrder.HorizontalReverse => (settings.Arrangement.Columns - index % settings.Arrangement.Columns - 1, index / settings.Arrangement.Columns),
                        Enums.PageOrder.Vertical => (index / settings.Arrangement.Rows, index % settings.Arrangement.Rows),
                        Enums.PageOrder.VerticalReverse => (index / settings.Arrangement.Rows, settings.Arrangement.Rows - index % settings.Arrangement.Rows - 1),
                        _ => (index % settings.Arrangement.Columns, index / settings.Arrangement.Columns)
                    };
                    index++;

                    if (page.Content is null)
                    {
                        continue;
                    }
                    if (page.Content.Parent is not null)
                    {
                        if (page.Content.Parent is not Decorator parent)
                        {
                            throw new PrintDocumentException(page.Content, "The content is already the child of another element.");
                        }
                        parent.Child = null;
                    }

                    Decorator container = new()
                    {
                        Child = page.Content,
                        Width = settings.Size.Extent.Width,
                        Height = settings.Size.Extent.Height,
                        RenderTransform = settings.Transform,
                        Clip = settings.Clip
                    };

                    Canvas.SetLeft(container, settings.Margin + column * settings.Size.Cell.Width);
                    Canvas.SetTop(container, settings.Margin + row * settings.Size.Cell.Height);
                    content.Children.Add(container);
                }

                return content;
            }
        }

        internal sealed class DocumentSettings(Size extent, Size cell, double margin, int columns, int rows, Enums.PageOrder order, Transform transform, Geometry clip)
        {
            public (Size Extent, Size Cell) Size { get; } = (extent, cell);
            public double Margin { get; } = margin;
            public (int Columns, int Rows) Arrangement { get; } = (columns, rows);
            public Enums.PageOrder Order { get; } = order;
            public Transform Transform { get; } = transform;
            public Geometry Clip { get; } = clip;
        }

        internal sealed class DocumentEffect() : ShaderEffect
        {
            public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(DocumentEffect), 0);
            public static readonly DependencyProperty ViewportLeftProperty = DependencyProperty.Register(nameof(ViewportLeft), typeof(float), typeof(DocumentEffect), new(0.0f, PixelShaderConstantCallback(0)));
            public static readonly DependencyProperty ViewportTopProperty = DependencyProperty.Register(nameof(ViewportTop), typeof(float), typeof(DocumentEffect), new(0.0f, PixelShaderConstantCallback(1)));
            public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register(nameof(ViewportWidth), typeof(float), typeof(DocumentEffect), new(0.0f, PixelShaderConstantCallback(2)));
            public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register(nameof(ViewportHeight), typeof(float), typeof(DocumentEffect), new(0.0f, PixelShaderConstantCallback(3)));

            public Brush Input
            {
                get => (Brush)GetValue(InputProperty);
                set => SetValue(InputProperty, value);
            }
            public float ViewportLeft
            {
                get => (float)GetValue(ViewportLeftProperty);
                set => SetValue(ViewportLeftProperty, value);
            }
            public float ViewportTop
            {
                get => (float)GetValue(ViewportTopProperty);
                set => SetValue(ViewportTopProperty, value);
            }
            public float ViewportWidth
            {
                get => (float)GetValue(ViewportWidthProperty);
                set => SetValue(ViewportWidthProperty, value);
            }
            public float ViewportHeight
            {
                get => (float)GetValue(ViewportHeightProperty);
                set => SetValue(ViewportHeightProperty, value);
            }

            public DocumentEffect(string name) : this()
            {
                PixelShader = new()
                {
                    UriSource = new(string.Format(CultureInfo.InvariantCulture, "/PrintDialogX;component/Resources/Effects/{0}", name), UriKind.Relative)
                };
                UpdateShaderValue(InputProperty);
                UpdateShaderValue(ViewportLeftProperty);
                UpdateShaderValue(ViewportTopProperty);
                UpdateShaderValue(ViewportWidthProperty);
                UpdateShaderValue(ViewportHeightProperty);
            }
        }

        public static readonly DependencyProperty ViewerProperty = DependencyProperty.Register(nameof(Viewer), typeof(VirtualizingStackPanel), typeof(DocumentHostControl), new(null));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(DocumentPage), typeof(DocumentHostControl), new(null, (x, e) =>
        {
            DocumentHostControl host = (DocumentHostControl)x;
            if (e.NewValue is not DocumentPage page)
            {
                host.Brush?.Visual.Visual = null;
                host.Brush?.Container.Fill = null;
                host.Brush = null;
                return;
            }

            VisualBrush visual = new(page.UpdateContent())
            {
                ViewboxUnits = BrushMappingMode.Absolute
            };
            Rectangle container = new()
            {
                Fill = visual
            };
            host.Brush = (new(container), container, visual);
        }));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(DocumentHostControl), new(1.0));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Enums.Color), typeof(DocumentHostControl), new FrameworkPropertyMetadata(Enums.Color.Color, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ColorEmulationLevelProperty = DependencyProperty.Register(nameof(ColorEmulationLevel), typeof(ColorEmulationLevel), typeof(DocumentHostControl), new(ColorEmulationLevel.Simple));

        public VirtualizingStackPanel? Viewer
        {
            get => (VirtualizingStackPanel?)GetValue(ViewerProperty);
            set => SetValue(ViewerProperty, value);
        }
        public DocumentPage? Content
        {
            get => (DocumentPage?)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        public double Zoom
        {
            get => (double)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }
        public Enums.Color Color
        {
            get => (Enums.Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public ColorEmulationLevel ColorEmulationLevel
        {
            get => (ColorEmulationLevel)GetValue(ColorEmulationLevelProperty);
            set => SetValue(ColorEmulationLevelProperty, value);
        }

        public (VisualBrush Brush, Rectangle Container, VisualBrush Visual)? Brush { get; set; } = null;

        private Rect viewport = new();
        private (Enums.Color Color, DocumentEffect? Effect) effect = (Enums.Color.Color, null);

        public DocumentHostControl()
        {
            Loaded += (x, e) => CompositionTarget.Rendering += UpdateViewport;
            Unloaded += (x, e) =>
            {
                CompositionTarget.Rendering -= UpdateViewport;

                Brush?.Visual.Visual = null;
                Brush?.Container.Fill = null;
                Brush = null;
            };
        }

        private void UpdateViewport(object? sender, EventArgs e)
        {
            if (Viewer is null)
            {
                return;
            }

            Point origin = Viewer.TranslatePoint(new(0, 0), this);
            Point extent = Viewer.TranslatePoint(new(Viewer.ViewportWidth, Viewer.ViewportHeight), this);
            if ((extent - origin).Length <= 0)
            {
                return;
            }

            Rect clip = Rect.Intersect(new(0, 0, Width, Height), new(origin, extent));
            if (clip.IsEmpty)
            {
                clip = new();
            }
            if (clip != viewport)
            {
                viewport = clip;
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext context)
        {
            if (Brush is null)
            {
                return;
            }

            Rect clip = new(viewport.X / Zoom, viewport.Y / Zoom, viewport.Width / Zoom, viewport.Height / Zoom);
            if (effect.Color != Color)
            {
                effect = (Color, ColorEmulationLevel switch
                {
                    ColorEmulationLevel.Simple => Color != Enums.Color.Color ? new("Grayscale.ps") : null,
                    ColorEmulationLevel.Full => Color switch
                    {
                        Enums.Color.Grayscale => new("Grayscale.ps"),
                        Enums.Color.Monochrome => new("Monochrome.ps"),
                        _ => null
                    },
                    _ => null
                });
            }
            if (effect.Effect is not null)
            {
                effect.Effect.ViewportLeft = (float)clip.X;
                effect.Effect.ViewportTop = (float)clip.Y;
                effect.Effect.ViewportWidth = (float)clip.Width;
                effect.Effect.ViewportHeight = (float)clip.Height;
            }
            Brush?.Visual.Viewbox = clip;
            Brush?.Container.Width = viewport.Width;
            Brush?.Container.Height = viewport.Height;
            Brush?.Container.Effect = effect.Effect;

            context.DrawRectangle(Brushes.White, null, viewport);
            context.DrawRectangle(Brush?.Brush, null, viewport);

            base.OnRender(context);
        }
    }

    internal sealed class DocumentToContentConverter() : LanguageHostConverter(), IValueConverter
    {
        internal sealed class Content(VirtualizingStackPanel viewer, DocumentHostControl.Document document, DocumentHostControl.DocumentPage page, string name, ColorEmulationLevel color)
        {
            public VirtualizingStackPanel? Viewer { get; } = viewer;
            public object DataContext { get; } = viewer.DataContext;

            public DocumentHostControl.DocumentPage Page { get; } = page;
            public string Name { get; } = name;
            public Size Size { get; } = new(document.PageSize.Width * document.ZoomValue, document.PageSize.Height * document.ZoomValue);
            public double Zoom { get; } = document.ZoomValue;
            public ColorEmulationLevel ColorEmulationLevel { get; } = color;
        }

        public PerformanceStrategy PerformanceStrategy { get; set; } = PerformanceStrategy.FavorsPreview;
        public ColorEmulationLevel ColorEmulationLevel { get; set; } = ColorEmulationLevel.Simple;

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not DocumentHostControl.Document document || document.Viewer is not VirtualizingStackPanel viewer || Resources is null)
            {
                return Binding.DoNothing;
            }

            List<IEnumerable<Content>> rows = [];
            using (document.Locker.Lock())
            {
                int index = 0;
                for (int i = 0; i < document.PageCount; i += document.ColumnCount)
                {
                    rows.Add(document.Pages.GetRange(i, Math.Min(document.ColumnCount, document.PageCount - i)).Select(x =>
                    {
                        if (PerformanceStrategy == PerformanceStrategy.FavorsPrinting)
                        {
                            x.Page.UpdateContent();
                        }

                        index++;

                        return new Content(viewer, document, x.Page, string.Format(culture, (string)Resources[TextResource.ConstructionPage], index, document.PageCount), ColorEmulationLevel);
                    }));
                }
            }

            return rows;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class DocumentToDescriptionConverter() : LanguageHostConverter(), IMultiValueConverter
    {
        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            return values[0] is double current && values[1] is DocumentHostControl.Document document && Resources is not null ? string.Format(culture, (string)Resources[TextResource.ConstructionPage], (int)Math.Floor(current + PrintDialogControl.EPSILON_INDEX), document.PageCount) : Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. Enumerable.Repeat(Binding.DoNothing, types.Length)];
        }
    }
}
