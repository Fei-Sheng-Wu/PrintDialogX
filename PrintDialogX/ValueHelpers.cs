using System;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
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
    internal class InterfaceToContentConverter : IValueConverter
    {
        public ResourceDictionary Resources { get; set; } = [];

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not InterfaceSettings settings || parameter is not ControlTemplate template)
            {
                return Binding.DoNothing;
            }

            ContentControl content = new()
            {
                Template = template,
                Focusable = false
            };
            content.ApplyTemplate();
            if (template.FindName("PART_Basic", content) is Panel basic)
            {
                ApplyInterface(basic, settings.BasicSettings, Resources);
            }
            if (template.FindName("PART_Advanced", content) is Panel advanced)
            {
                ApplyInterface(advanced, settings.AdvancedSettings, Resources);
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
                    Template = (ControlTemplate)resources[option switch
                    {
                        InterfaceSettings.Option.Printer => "OptionPrinter",
                        InterfaceSettings.Option.PrinterPreferences => "OptionPrinterPreferences",
                        InterfaceSettings.Option.Copies => "OptionCopies",
                        InterfaceSettings.Option.Collation => "OptionCollation",
                        InterfaceSettings.Option.Pages => "OptionPages",
                        InterfaceSettings.Option.Layout => "OptionLayout",
                        InterfaceSettings.Option.Size => "OptionSize",
                        InterfaceSettings.Option.Color => "OptionColor",
                        InterfaceSettings.Option.Quality => "OptionQuality",
                        InterfaceSettings.Option.PagesPerSheet => "OptionPagesPerSheet",
                        InterfaceSettings.Option.PageOrder => "OptionPageOrder",
                        InterfaceSettings.Option.Scale => "OptionScale",
                        InterfaceSettings.Option.Margin => "OptionMargin",
                        InterfaceSettings.Option.DoubleSided => "OptionDoubleSided",
                        InterfaceSettings.Option.Type => "OptionType",
                        InterfaceSettings.Option.Source => "OptionSource",
                        _ => "OptionVoid"
                    }],
                    Focusable = false
                });
            }
        }

        public static void ApplyLanguage(ResourceDictionary resources, InterfaceSettings.Language language)
        {
            resources.MergedDictionaries.Add(new()
            {
                Source = new($"/PrintDialogX;component/Resources/Languages/{language switch
                {
                    InterfaceSettings.Language.zh_CN => "zh-CN",
                    _ => "en-US"
                }}.xaml", UriKind.Relative)
            });
        }
    }

    internal class ValueToDescriptionConverter : IValueConverter
    {
        public ResourceDictionary Resources { get; set; } = [];

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return value != null ? GetDescription(value, Resources) : Binding.DoNothing;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static object GetDescription(object value, ResourceDictionary resources)
        {
            return value.GetType().GetField(value.ToString() ?? string.Empty)?.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute description ? resources[description.Description] : value;
        }
    }

    internal class ComparisonToStateConverter : IValueConverter
    {
        public enum Comparison
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
            bool comparison = Mode switch
            {
                Comparison.Threshold => System.Convert.ToInt32(value) >= System.Convert.ToInt32(parameter),
                _ => Equals(value, parameter)
            };
            return IsInverted ^ comparison ? StateTrue : StateFalse;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return IsInverted ^ Equals(value, StateTrue) ? parameter : Fallback;
        }
    }

    internal class CollectionToRangeConverter : IValueConverter
    {
        public int TrimStart { get; set; } = 0;
        public int TrimEnd { get; set; } = 0;

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not IEnumerable collection)
            {
                return Binding.DoNothing;
            }

            List<object> list = [.. collection];
            return list.GetRange(TrimStart, list.Count - TrimStart - TrimEnd);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal class MultiBooleanConverter : IMultiValueConverter
    {
        public enum Condition
        {
            All,
            Any
        }

        public Condition Mode { get; set; } = Condition.All;

        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            return Mode switch
            {
                Condition.Any => values.Any(x => x is bool value && value),
                _ => values.All(x => x is bool value && value)
            };
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. Enumerable.Repeat(Binding.DoNothing, types.Length)];
        }
    }

    internal class PrinterComparer : IEqualityComparer<PrintQueue>
    {
        public static readonly PrinterComparer Instance = new();

        public bool Equals(PrintQueue? x, PrintQueue? y)
        {
            try
            {
                return x != null && y != null && x.FullName == y.FullName;
            }
            catch
            {
                return false;
            }
        }

        public int GetHashCode(PrintQueue value)
        {
            return value.GetHashCode();
        }
    }

    internal class PrinterToIconConverter : IValueConverter
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

        public class PrinterIcon(ImageSource? icon, double opacity, double size)
        {
            public ImageSource? Icon { get; set; } = icon;
            public double Opacity { get; set; } = opacity;
            public double Size { get; set; } = size;
        }

        public enum PrinterType
        {
            Printer = 16,
            PrinterNetwork = 50,
            PrinterFile = 54,
            Fax = 52,
            FaxNetwork = 53
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

            PrinterType target = (CollectionFax.Contains(printer, PrinterComparer.Instance), CollectionNetwork.Contains(printer, PrinterComparer.Instance), ((Func<bool>)(() =>
            {
                try
                {
                    return FilterFile.Any(x => printer.QueuePort.Name.StartsWith(x, StringComparison.OrdinalIgnoreCase));
                }
                catch
                {
                    return false;
                }
            }))()) switch
            {
                (true, false, _) => PrinterType.Fax,
                (true, true, _) => PrinterType.FaxNetwork,
                (_, _, true) => PrinterType.PrinterFile,
                (_, true, _) => PrinterType.PrinterNetwork,
                _ => PrinterType.Printer,
            };
            bool isSmall = System.Convert.ToBoolean(parameter);

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
    }

    internal class PrinterToStatusConverter : IValueConverter
    {
        public ResourceDictionary Resources { get; set; } = [];

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer)
            {
                return Binding.DoNothing;
            }

            try
            {
                printer.Refresh();
                return Resources[printer.QueueStatus switch
                {
                    PrintQueueStatus.None => "StringResource_LabelReady",
                    PrintQueueStatus.Busy => "StringResource_LabelBusy",
                    PrintQueueStatus.DoorOpen => "StringResource_LabelDoorOpen",
                    PrintQueueStatus.Initializing => "StringResource_LabelInitializing",
                    PrintQueueStatus.IOActive => "StringResource_LabelIOActive",
                    PrintQueueStatus.ManualFeed => "StringResource_LabelManualFeed",
                    PrintQueueStatus.NoToner => "StringResource_LabelNoToner",
                    PrintQueueStatus.NotAvailable => "StringResource_LabelNotAvailable",
                    PrintQueueStatus.Offline => "StringResource_LabelOffline",
                    PrintQueueStatus.OutOfMemory => "StringResource_LabelOutOfMemory",
                    PrintQueueStatus.OutputBinFull => "StringResource_LabelOutputBinFull",
                    PrintQueueStatus.PagePunt => "StringResource_LabelPagePunt",
                    PrintQueueStatus.PaperJam => "StringResource_LabelPaperJam",
                    PrintQueueStatus.PaperOut => "StringResource_LabelPaperOut",
                    PrintQueueStatus.PaperProblem => "StringResource_LabelPaperProblem",
                    PrintQueueStatus.Paused => "StringResource_LabelPaused",
                    PrintQueueStatus.PendingDeletion => "StringResource_LabelPendingDeletion",
                    PrintQueueStatus.PowerSave => "StringResource_LabelPowerSave",
                    PrintQueueStatus.Printing => "StringResource_LabelPrinting",
                    PrintQueueStatus.Processing => "StringResource_LabelProcessing",
                    PrintQueueStatus.ServerUnknown => "StringResource_LabelServerUnknown",
                    PrintQueueStatus.TonerLow => "StringResource_LabelTonerLow",
                    PrintQueueStatus.UserIntervention => "StringResource_LabelUserIntervention",
                    PrintQueueStatus.Waiting => "StringResource_LabelWaiting",
                    PrintQueueStatus.WarmingUp => "StringResource_LabelWarmingUp",
                    _ => "StringResource_LabelError"
                }];
            }
            catch
            {
                return Resources["StringResource_LabelError"];
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal class PrinterToDescriptionConverter : IValueConverter
    {
        public ResourceDictionary Resources { get; set; } = [];

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer)
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
                info.Add($"{Resources["StringResource_PrefixDocuments"]}{printer.NumberOfJobs}");
            }
            catch { }
            try
            {
                info.Add($"{Resources["StringResource_PrefixLocation"]}{(string.IsNullOrWhiteSpace(printer.Location) ? Resources["StringResource_LabelUnknown"] : printer.Location)}");
            }
            catch { }
            try
            {
                if (!string.IsNullOrWhiteSpace(printer.Comment))
                {
                    info.Add($"{Resources["StringResource_PrefixComment"]}{printer.Comment}");
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

    internal class CustomPagesValidationRule : ValidationRule
    {
        public int Maximum { get; set; } = int.MaxValue;

        public override System.Windows.Controls.ValidationResult Validate(object value, CultureInfo culture)
        {
            return TryConvert(value, Maximum).IsValid ? System.Windows.Controls.ValidationResult.ValidResult : new(false, string.Empty);
        }

        public static (bool IsValid, List<int> Result) TryConvert(object value, int maximum)
        {
            List<int> result = [];
            foreach (string entry in (value.ToString() ?? string.Empty).Split(',').Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                string[] range = entry.Split('-');
                if (range.Length > 2 || !int.TryParse(range.First(), out int start) || !int.TryParse(range.Last(), out int end) || start < 1 || end < start || maximum < end)
                {
                    return (false, []);
                }

                result.AddRange(Enumerable.Range(start, end - start + 1));
            }

            return (true, result);
        }
    }

    internal class SizeToDescriptionConverter : IValueConverter
    {
        public ResourceDictionary Resources { get; set; } = [];

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not Enums.Size size)
            {
                return Binding.DoNothing;
            }

            object? name = size.DefinedName != null ? ValueToDescriptionConverter.GetDescription(size.DefinedName.Value, Resources) : size.FallbackName;
            string description = $"{Math.Round(size.Width * 2.54 / 96, 2)} × {Math.Round(size.Height * 2.54 / 96, 2)} cm";

            return System.Convert.ToBoolean(parameter) ? description : name ?? $"{Resources["StringResource_PrefixCustom"]}{description}";
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal class SizeToMarginMaximumConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not Enums.Size size)
            {
                return Binding.DoNothing;
            }

            return (int)Math.Min(size.Width / 2, size.Height / 2);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal class DocumentHostControl : Border
    {
        public class Document : DocumentPaginator
        {
            public enum Zoom
            {
                Custom,
                FitToWidth,
                FitToHeight,
                FitToPage
            }

            public List<(int Index, Canvas Content)> Pages { get; set; } = [];
            public object Lock { get; set; } = new();

            public VirtualizingStackPanel? Viewer { get; set; } = null;

            public double ZoomValue
            {
                get;
                set => field = Math.Max(0.05, Math.Min(10000, value));
            } = 1;
            public Zoom ZoomMode { get; set; } = Zoom.FitToWidth;
            public Point? ZoomTarget { get; set; } = null;
            public int ColumnCount { get; set; } = 1;

            public override bool IsPageCountValid { get => true; }
            public override int PageCount { get => Pages.Count; }
            public override Size PageSize { get; set; } = new();
            public override IDocumentPaginatorSource? Source { get => null; }
            public override DocumentPage GetPage(int index)
            {
                lock (Lock)
                {
                    if (index < 0 || index >= Pages.Count)
                    {
                        return DocumentPage.Missing;
                    }

                    Canvas content = Pages[index].Content;
                    content.Measure(PageSize);
                    content.Arrange(new(PageSize));
                    return new(content, PageSize, new(PageSize), new(PageSize));
                }
            }
        }

        public class DocumentEffect : ShaderEffect
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

            public DocumentEffect(string name)
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

        public const double SPACING = 8;

        public static readonly DependencyProperty ViewerProperty = DependencyProperty.Register(nameof(Viewer), typeof(VirtualizingStackPanel), typeof(DocumentHostControl), new(null));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(Canvas), typeof(DocumentHostControl), new(null, (x, e) =>
        {
            if (x is not DocumentHostControl host || e.NewValue is not Canvas content)
            {
                return;
            }

            VisualBrush visual = new(content)
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

        public VirtualizingStackPanel? Viewer
        {
            get => (VirtualizingStackPanel?)GetValue(ViewerProperty);
            set => SetValue(ViewerProperty, value);
        }
        public Canvas? Content
        {
            get => (Canvas?)GetValue(ContentProperty);
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

    internal class DocumentToContentConverter : IValueConverter
    {
        public class Content(DocumentHostControl.Document document, Canvas page)
        {
            public VirtualizingStackPanel? Viewer { get; set; } = document.Viewer;
            public Canvas Page { get; set; } = page;
            public Size Size { get; set; } = new(document.PageSize.Width * document.ZoomValue, document.PageSize.Height * document.ZoomValue);
            public double Zoom { get; set; } = document.ZoomValue;
        }

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not DocumentHostControl.Document document)
            {
                return Binding.DoNothing;
            }

            List<IEnumerable<Content>> rows = [];
            lock (document.Lock)
            {
                for (int i = 0; i < document.PageCount; i += document.ColumnCount)
                {
                    rows.Add(document.Pages.GetRange(i, Math.Min(document.ColumnCount, document.PageCount - i)).Select(x => new Content(document, x.Content)));
                }
            }

            return rows;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal class DocumentToDescriptionConverter : IMultiValueConverter
    {
        public ResourceDictionary Resources { get; set; } = [];

        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values.First() is not double current || values.Last() is not DocumentHostControl.Document document)
            {
                return Binding.DoNothing;
            }

            return $"{Resources["StringResource_LabelPage"]} {Math.Floor(current + PrintDialogControl.EPSILON_INDEX)} / {document.PageCount}";
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. Enumerable.Repeat(Binding.DoNothing, types.Length)];
        }
    }
}