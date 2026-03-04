using System;
using System.Linq;
using System.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
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
    internal abstract class LanguageHostConverter() : ILanguageHost
    {
        public ResourceDictionary? Resources { get; set; } = null;

        public void UpdateLanguage(ResourceDictionary resources, string language)
        {
            Resources = resources;
        }
    }

    internal sealed class InterfaceToContentConverter() : IValueConverter
    {
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
            if (!settings.AdvancedSettings.Any())
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

        public static void ApplyLanguage(ILanguageHost host, InterfaceSettings.Language language)
        {
            string code = ValueMappings.Attribute<LanguageAttribute>(language != InterfaceSettings.Language.System ? language : new Func<InterfaceSettings.Language>(() =>
            {
                string[] current = CultureInfo.CurrentUICulture.IetfLanguageTag.Split('-');
                return (current[0], current.Length > 1 ? current[1] : null) switch
                {
                    ("en", "CA") => InterfaceSettings.Language.en_CA,
                    ("en", "GB") => InterfaceSettings.Language.en_GB,
                    ("en", _) => InterfaceSettings.Language.en_US,
                    ("zh", "HK") => InterfaceSettings.Language.zh_HK,
                    ("zh", "TW") => InterfaceSettings.Language.zh_TW,
                    ("zh", "Hans") => InterfaceSettings.Language.zh_CN,
                    ("zh", "Hant") => InterfaceSettings.Language.zh_HK,
                    ("zh", _) => InterfaceSettings.Language.zh_CN,
                    ("yue", "HK") => InterfaceSettings.Language.zh_HK,
                    ("yue", "TW") => InterfaceSettings.Language.zh_TW,
                    ("yue", _) => InterfaceSettings.Language.zh_HK,
                    _ => InterfaceSettings.Language.en_US
                };
            })())?.Language ?? "en-US";
            host.UpdateLanguage(new()
            {
                Source = new($"/PrintDialogX;component/Resources/Languages/{code}.xaml", UriKind.Relative)
            }, code);
        }
    }

    internal sealed class ValueToDescriptionConverter() : LanguageHostConverter, IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return value != null && Resources != null ? GetDescription(value, Resources) : Binding.DoNothing;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static object GetDescription(object value, ResourceDictionary resources)
        {
            return ValueMappings.Attribute<StringResourceAttribute>(value)?.Resource is StringResource resource ? resources[resource] : value;
        }
    }

    internal sealed class ComparisonToStateConverter() : IValueConverter, IMultiValueConverter
    {
        internal enum Comparison
        {
            Equality,
            Threshold
        }

        public Comparison Mode { get; set; } = Comparison.Equality;
        public object StateTrue { get; set; } = true;
        public object StateFalse { get; set; } = false;
        public object Fallback { get; set; } = false;
        public bool IsInverted { get; set; } = false;

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return Convert([value], type, parameter, culture);
        }

        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            return IsInverted ^ (Mode switch
            {
                Comparison.Threshold => values.All(x => System.Convert.ToInt32(x, CultureInfo.InvariantCulture) >= System.Convert.ToInt32(parameter, CultureInfo.InvariantCulture)),
                _ => values.All(x => Equals(x, parameter))
            }) ? StateTrue : StateFalse;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return IsInverted ^ Equals(value, StateTrue) ? parameter : Fallback;
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. types.Select(x => ConvertBack(value, x, parameter, culture))];
        }
    }

    internal sealed class CollectionToRangeConverter() : IValueConverter
    {
        public int TrimStart { get; set; } = 0;
        public int TrimEnd { get; set; } = 0;

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable enumerable)
            {
                return Binding.DoNothing;
            }

            List<object> collection = [.. enumerable];

            return collection.GetRange(TrimStart, collection.Count - TrimStart - TrimEnd);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class PrinterComparer() : IEqualityComparer<PrintQueue>
    {
        public static readonly PrinterComparer Instance = new();

        public bool Equals(PrintQueue? x, PrintQueue? y)
        {
            try
            {
                return x != null && y != null && (ReferenceEquals(x, y) || StringComparer.Ordinal.Equals(x.FullName, y.FullName));
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

    internal sealed class PrinterToIconConverter() : IValueConverter
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysImageIndex;
            public int iIcon;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetStockIconInfo(uint siid, uint uFlags, ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        internal enum PrinterType
        {
            Printer = 16,
            PrinterNetwork = 50,
            PrinterFile = 54,
            Fax = 52,
            FaxNetwork = 53
        }

        internal sealed class PrinterIcon(ImageSource? icon, double opacity, double size)
        {
            public ImageSource? Icon { get; } = icon;
            public double Opacity { get; } = opacity;
            public double Size { get; } = size;
        }

        public static readonly Dictionary<(PrinterType, bool), ImageSource> Cache = [];
        public static readonly string[] FilterFile = ["portprompt", "nul", "file"];

        public PrintQueueCollection CollectionFax { get; set; } = [];
        public PrintQueueCollection CollectionNetwork { get; set; } = [];

        public object? Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer)
            {
                return Binding.DoNothing;
            }

            PrinterType target = (CollectionFax.Contains(printer, PrinterComparer.Instance), CollectionNetwork.Contains(printer, PrinterComparer.Instance), CheckFilter(printer, FilterFile)) switch
            {
                (true, false, _) => PrinterType.Fax,
                (true, true, _) => PrinterType.FaxNetwork,
                (_, _, true) => PrinterType.PrinterFile,
                (_, true, _) => PrinterType.PrinterNetwork,
                _ => PrinterType.Printer,
            };
            bool isSmall = System.Convert.ToBoolean(parameter, CultureInfo.InvariantCulture);

            double opacity = 0.5;
            try
            {
                printer.Refresh();
                opacity = printer.IsOffline ? 0.5 : 1;
            }
            catch { }

            (PrinterType, bool) key = (target, isSmall);
            if (!Cache.TryGetValue(key, out ImageSource? icon))
            {
                try
                {
                    SHSTOCKICONINFO info = new()
                    {
                        cbSize = (uint)Marshal.SizeOf(typeof(SHSTOCKICONINFO))
                    };
                    if (SHGetStockIconInfo((uint)target, 256 | (uint)(isSmall ? 1 : 4), ref info) == 0 && info.hIcon != IntPtr.Zero)
                    {
                        icon = Imaging.CreateBitmapSourceFromHIcon(info.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        DestroyIcon(info.hIcon);
                    }
                }
                catch { }

                if (icon != null)
                {
                    icon.Freeze();
                    Cache[key] = icon;
                }
            }

            return new PrinterIcon(icon, opacity, isSmall ? 18 : 36);
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

    internal sealed class PrinterToStatusConverter() : LanguageHostConverter, IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer || Resources == null)
            {
                return Binding.DoNothing;
            }

            try
            {
                printer.Refresh();
                return Resources[printer.QueueStatus switch
                {
                    PrintQueueStatus.None => StringResource.LabelReady,
                    PrintQueueStatus.Busy => StringResource.LabelBusy,
                    PrintQueueStatus.DoorOpen => StringResource.LabelDoorOpen,
                    PrintQueueStatus.Initializing => StringResource.LabelInitializing,
                    PrintQueueStatus.IOActive => StringResource.LabelIOActive,
                    PrintQueueStatus.ManualFeed => StringResource.LabelManualFeed,
                    PrintQueueStatus.NoToner => StringResource.LabelNoToner,
                    PrintQueueStatus.NotAvailable => StringResource.LabelNotAvailable,
                    PrintQueueStatus.Offline => StringResource.LabelOffline,
                    PrintQueueStatus.OutOfMemory => StringResource.LabelOutOfMemory,
                    PrintQueueStatus.OutputBinFull => StringResource.LabelOutputBinFull,
                    PrintQueueStatus.PagePunt => StringResource.LabelPagePunt,
                    PrintQueueStatus.PaperJam => StringResource.LabelPaperJam,
                    PrintQueueStatus.PaperOut => StringResource.LabelPaperOut,
                    PrintQueueStatus.PaperProblem => StringResource.LabelPaperProblem,
                    PrintQueueStatus.Paused => StringResource.LabelPaused,
                    PrintQueueStatus.PendingDeletion => StringResource.LabelPendingDeletion,
                    PrintQueueStatus.PowerSave => StringResource.LabelPowerSave,
                    PrintQueueStatus.Printing => StringResource.LabelPrinting,
                    PrintQueueStatus.Processing => StringResource.LabelProcessing,
                    PrintQueueStatus.ServerUnknown => StringResource.LabelServerUnknown,
                    PrintQueueStatus.TonerLow => StringResource.LabelTonerLow,
                    PrintQueueStatus.UserIntervention => StringResource.LabelUserIntervention,
                    PrintQueueStatus.Waiting => StringResource.LabelWaiting,
                    PrintQueueStatus.WarmingUp => StringResource.LabelWarmingUp,
                    _ => StringResource.LabelError
                }];
            }
            catch
            {
                return Resources[StringResource.LabelError];
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class PrinterToDescriptionConverter() : LanguageHostConverter, IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer || Resources == null)
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
                info.Add(string.Format(CultureInfo.InvariantCulture, (string)Resources[StringResource.ConstructionDocuments], printer.NumberOfJobs));
            }
            catch { }
            try
            {
                info.Add(string.Format(CultureInfo.InvariantCulture, (string)Resources[StringResource.ConstructionLocation], string.IsNullOrWhiteSpace(printer.Location) ? Resources[StringResource.LabelUnknown] : printer.Location));
            }
            catch { }
            try
            {
                if (!string.IsNullOrWhiteSpace(printer.Comment))
                {
                    info.Add(string.Format(CultureInfo.InvariantCulture, (string)Resources[StringResource.ConstructionComment], printer.Comment));
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

        public static (bool IsValid, List<object>? Result) TryConvert(string value, int maximum, bool isValidation = false)
        {
            List<object>? result = isValidation ? null : [];
            foreach (string entry in value.Split(',', ';', '，', '、', '､', '﹑', '،', '؛', '﹐').Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                string[] range = entry.Split('-', '\u2010', '\u2011', '\u2012', '\u2013', '\u2014', '\u2015', '\ufe58', '\ufe63', '\uff0d');
                switch (range.Length)
                {
                    case 1 when int.TryParse(range[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int single) && single > 0 && single <= maximum:
                        result?.Add(single);
                        break;
                    case 2 when int.TryParse(range[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int start) && int.TryParse(range[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int end) && start > 0 && start <= end && end <= maximum:
                        result?.Add((start, end));
                        break;
                    default:
                        return (false, null);
                }
            }

            return (true, result);
        }
    }

    internal sealed class SizeToDescriptionConverter() : LanguageHostConverter, IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not Enums.Size size || Resources == null)
            {
                return Binding.DoNothing;
            }

            object? name = size.DefinedName != null ? ValueToDescriptionConverter.GetDescription(size.DefinedName.Value, Resources) : size.FallbackName;
            string description = string.Format(CultureInfo.InvariantCulture, (string)Resources[StringResource.ConstructionSize], size.Width * 2.54 / 96.0, size.Height * 2.54 / 96.0);

            return System.Convert.ToBoolean(parameter, CultureInfo.InvariantCulture) ? description : (name ?? string.Format(CultureInfo.InvariantCulture, (string)Resources[StringResource.ConstructionCustom], description));
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

        internal sealed class Document() : DocumentPaginator
        {
            public override bool IsPageCountValid { get => true; }
            public override int PageCount { get => Pages.Count; }
            public override Size PageSize { get; set; } = new();
            public override IDocumentPaginatorSource? Source { get => null; }
            public override System.Windows.Documents.DocumentPage GetPage(int index)
            {
                lock (Lock)
                {
                    if (index < 0 || index >= Pages.Count)
                    {
                        return System.Windows.Documents.DocumentPage.Missing;
                    }

                    Canvas content = Pages[index].Page.Content;
                    content.Measure(PageSize);
                    content.Arrange(new(PageSize));

                    return new(content, PageSize, new(PageSize), new(PageSize));
                }
            }

            public object Lock { get; } = new();
            public List<(int Index, DocumentPage Page)> Pages { get; } = [];

            public VirtualizingStackPanel? Viewer { get; set; } = null;

            public double ZoomValue
            {
                get;
                set => field = Math.Max(0.05, Math.Min(10000, value));
            } = 1;
            public DocumentZoom ZoomMode { get; set; } = DocumentZoom.FitToWidth;
            public Point? ZoomTarget { get; set; } = null;
            public int ColumnCount { get; set; } = 1;
        }

        internal sealed class DocumentPage(List<PrintPage> chunk, DocumentSettings settings)
        {
            public Canvas Content
            {
                get
                {
                    if (content != null)
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

                        if (page.Content == null)
                        {
                            continue;
                        }
                        if (page.Content.Parent != null)
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
                            Width = settings.Size.Container.Width,
                            Height = settings.Size.Container.Height,
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

            private Canvas? content = null;
        }

        internal sealed class DocumentSettings(int columns, int rows, Enums.PageOrder order, double margin, Size cell, Size container, Transform transform, Geometry clip)
        {
            public (int Columns, int Rows) Arrangement { get; } = (columns, rows);
            public Enums.PageOrder Order { get; } = order;
            public double Margin { get; } = margin;
            public (Size Cell, Size Container) Size { get; } = (cell, container);
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
                    UriSource = new($"/PrintDialogX;component/Resources/Effects/{name}.ps", UriKind.Relative)
                };
                UpdateShaderValue(InputProperty);
                UpdateShaderValue(ViewportLeftProperty);
                UpdateShaderValue(ViewportTopProperty);
                UpdateShaderValue(ViewportWidthProperty);
                UpdateShaderValue(ViewportHeightProperty);
            }
        }

        public const double LENGTH_SPACING = 8;

        public static readonly DependencyProperty ViewerProperty = DependencyProperty.Register(nameof(Viewer), typeof(VirtualizingStackPanel), typeof(DocumentHostControl), new(null));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(DocumentPage), typeof(DocumentHostControl), new(null, (x, e) =>
        {
            if (e.NewValue is not DocumentPage page)
            {
                return;
            }

            VisualBrush visual = new(page.Content)
            {
                ViewboxUnits = BrushMappingMode.Absolute
            };
            Rectangle container = new()
            {
                Fill = visual
            };
            ((DocumentHostControl)x).Brush = (new(container), container, visual);
        }));
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(DocumentHostControl), new(1.0));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Enums.Color), typeof(DocumentHostControl), new FrameworkPropertyMetadata(Enums.Color.Color, FrameworkPropertyMetadataOptions.AffectsRender));

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
            };
        }

        private void UpdateViewport(object? sender, EventArgs e)
        {
            if (Viewer == null)
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
            if (Brush == null)
            {
                return;
            }

            Rect clip = new(viewport.X / Zoom, viewport.Y / Zoom, viewport.Width / Zoom, viewport.Height / Zoom);
            if (true || effect.Color != Color)
            {
                effect = (Color, Color switch
                {
                    Enums.Color.Grayscale => new("Grayscale"),
                    Enums.Color.Monochrome => new("Monochrome"),
                    _ => null
                });
            }
            if (effect.Effect != null)
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

    internal sealed class DocumentToContentConverter() : IValueConverter
    {
        internal sealed class Content(DocumentHostControl.Document document, VirtualizingStackPanel viewer, DocumentHostControl.DocumentPage page)
        {
            public VirtualizingStackPanel? Viewer { get; } = viewer;
            public object DataContext { get; } = viewer.DataContext;

            public DocumentHostControl.DocumentPage Page { get; } = page;
            public Size Size { get; } = new(document.PageSize.Width * document.ZoomValue, document.PageSize.Height * document.ZoomValue);
            public double Zoom { get; } = document.ZoomValue;
        }

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not DocumentHostControl.Document document || document.Viewer is not VirtualizingStackPanel viewer)
            {
                return Binding.DoNothing;
            }

            List<IEnumerable<Content>> rows = [];
            lock (document.Lock)
            {
                for (int i = 0; i < document.PageCount; i += document.ColumnCount)
                {
                    rows.Add(document.Pages.GetRange(i, Math.Min(document.ColumnCount, document.PageCount - i)).Select(x => new Content(document, viewer, x.Page)));
                }
            }

            return rows;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class DocumentToDescriptionConverter() : LanguageHostConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            if (values[0] is not double current || values[1] is not DocumentHostControl.Document document || Resources == null)
            {
                return Binding.DoNothing;
            }

            return string.Format(CultureInfo.InvariantCulture, (string)Resources[StringResource.ConstructionPage], (int)(current + PrintDialogControl.EPSILON_INDEX), document.PageCount);
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. Enumerable.Repeat(Binding.DoNothing, types.Length)];
        }
    }
}
