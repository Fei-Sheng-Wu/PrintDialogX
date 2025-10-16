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

namespace PrintDialogX
{
    internal class DesignTimeInterfaceSettings
    {
        public InterfaceSettings InterfaceSettings { get; set; } = new();
    }

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
                container.Children.Add((UIElement)resources[option switch
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
                }]);
            }
        }
    }

    internal class ValueToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return value != null ? GetDescription(value) : Binding.DoNothing;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static object GetDescription(object value)
        {
            return value.GetType().GetField(value.ToString() ?? string.Empty)?.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute description ? PrintDialogWindow.StringResources[description.Description] : value;
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

        public class PrinterIcon(ImageSource? icon, double opacity, bool isSmall)
        {
            public ImageSource? Icon { get; set; } = icon;
            public double Opacity { get; set; } = opacity;
            public double Size { get; set; } = isSmall ? 18 : 36;
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

        public static PrintQueueCollection CollectionFax = [];
        public static PrintQueueCollection CollectionNetwork = [];

        public object? Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer)
            {
                return Binding.DoNothing;
            }

            //TODO: better type detection
            PrinterType target = PrinterType.Printer;
            PrinterComparer comparer = new();
            bool isNetwork = CollectionNetwork.Contains(printer, comparer);
            if (CollectionFax.Contains(printer, comparer))
            {
                target = isNetwork ? PrinterType.FaxNetwork : PrinterType.Fax;
            }
            else if (isNetwork)
            {
                target = PrinterType.PrinterNetwork;
            }
            bool isSmall = System.Convert.ToBoolean(parameter);

            double opacity = 0.5;
            try
            {
                printer.Refresh();
                opacity = printer.IsOffline ? 0.5 : 1;
            }
            catch { }

            (PrinterType, bool) key = (target, isSmall);
            if (Cache.TryGetValue(key, out ImageSource? icon))
            {
                return new PrinterIcon(icon, opacity, isSmall);
            }
            else
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
                    Cache[key] = icon;
                }
                return new PrinterIcon(icon, opacity, isSmall);
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal class PrinterToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintQueue printer)
            {
                return Binding.DoNothing;
            }

            try
            {
                printer.Refresh();
                return PrintDialogWindow.StringResources[printer.QueueStatus switch
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
                return PrintDialogWindow.StringResources["StringResource_LabelError"];
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal class PrinterToDescriptionConverter : IValueConverter
    {
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
                info.Add($"{PrintDialogWindow.StringResources["StringResource_PrefixDocuments"]}{printer.NumberOfJobs}");
            }
            catch { }
            try
            {
                info.Add($"{PrintDialogWindow.StringResources["StringResource_PrefixLocation"]}{(string.IsNullOrWhiteSpace(printer.Location) ? PrintDialogWindow.StringResources["StringResource_LabelUnknown"] : printer.Location)}");
            }
            catch { }
            try
            {
                if (!string.IsNullOrWhiteSpace(printer.Comment))
                {
                    info.Add($"{PrintDialogWindow.StringResources["StringResource_PrefixComment"]}{printer.Comment}");
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

    internal class CustomPagesValidationParameters : Freezable
    {
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(CustomPagesValidationParameters), new(int.MaxValue));

        public int Maximum
        {
            get => (int)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new CustomPagesValidationParameters();
        }
    }

    internal class CustomPagesValidationRule : ValidationRule
    {
        public CustomPagesValidationParameters? Parameters { get; set; } = null;

        public override System.Windows.Controls.ValidationResult Validate(object value, CultureInfo culture)
        {
            return TryConvert(value, Parameters?.Maximum ?? int.MaxValue).IsValid ? System.Windows.Controls.ValidationResult.ValidResult : new(false, string.Empty);
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
                else
                {
                    result.AddRange(Enumerable.Range(start, end - start + 1));
                }
            }

            return (true, result);
        }
    }

    internal class SizeToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not Enums.Size size)
            {
                return Binding.DoNothing;
            }

            object? name = size.DefinedName != null ? ValueToDescriptionConverter.GetDescription(size.DefinedName.Value) : size.FallbackName;
            string description = $"{Math.Round(size.Width * 2.54 / 96, 2)} × {Math.Round(size.Height * 2.54 / 96, 2)} cm";

            return System.Convert.ToBoolean(parameter) ? description : name ?? $"{PrintDialogWindow.StringResources["StringResource_PrefixCustom"]}{description}";
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

    internal class DocumentColorEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(DocumentColorEffect), 0);
        public static readonly DependencyProperty ViewportLeftProperty = DependencyProperty.Register(nameof(ViewportLeft), typeof(float), typeof(DocumentColorEffect), new(0.0f, PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty ViewportTopProperty = DependencyProperty.Register(nameof(ViewportTop), typeof(float), typeof(DocumentColorEffect), new(0.0f, PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register(nameof(ViewportWidth), typeof(float), typeof(DocumentColorEffect), new(0.0f, PixelShaderConstantCallback(2)));
        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register(nameof(ViewportHeight), typeof(float), typeof(DocumentColorEffect), new(0.0f, PixelShaderConstantCallback(3)));

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

        public string Name { get; set; }

        public DocumentColorEffect(string name)
        {
            Name = name;
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

    internal class DocumentHostControl : Border
    {
        public class DocumentParameters
        {
            public VirtualizingStackPanel? Viewer { get; set; } = null;
            public double Spacing { get; set; } = 0;
            public string? Effect { get; set; } = null;
        }

        public static DocumentParameters Parameters = new();

        private readonly (Canvas Content, double Scale) page;
        private readonly (VisualBrush Brush, Rectangle Container, VisualBrush Visual) brush;

        private Rect viewport = new();
        private DocumentColorEffect? effect = null;

        public DocumentHostControl(Canvas content, double scale)
        {
            page = (content, scale);
            VisualBrush visual = new()
            {
                Visual = page.Content,
                ViewboxUnits = BrushMappingMode.Absolute
            };
            Rectangle container = new()
            {
                Fill = visual
            };
            brush = (new()
            {
                Visual = container
            }, container, visual);

            Width = page.Content.Width * page.Scale;
            Height = page.Content.Height * page.Scale;
            Loaded += (x, e) => CompositionTarget.Rendering += UpdateViewport;
            Unloaded += (x, e) => CompositionTarget.Rendering -= UpdateViewport;
        }

        private void UpdateViewport(object? sender, EventArgs e)
        {
            if (Parameters.Viewer == null)
            {
                return;
            }

            Point origin = Parameters.Viewer.TranslatePoint(new(0, 0), this);
            Point extent = Parameters.Viewer.TranslatePoint(new(Parameters.Viewer.ViewportWidth, Parameters.Viewer.ViewportHeight), this);
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
            if (Parameters.Effect == null)
            {
                effect = null;
            }
            else if (effect == null || effect.Name != Parameters.Effect)
            {
                effect = new(Parameters.Effect);
            }

            Rect clip = new(viewport.X / page.Scale, viewport.Y / page.Scale, viewport.Width / page.Scale, viewport.Height / page.Scale);
            if (effect != null)
            {
                effect.ViewportLeft = (float)clip.X;
                effect.ViewportTop = (float)clip.Y;
                effect.ViewportWidth = (float)clip.Width;
                effect.ViewportHeight = (float)clip.Height;
            }
            brush.Visual.Viewbox = clip;
            brush.Container.Width = viewport.Width;
            brush.Container.Height = viewport.Height;
            brush.Container.Effect = effect;
            context.DrawRectangle(Brushes.White, null, viewport);
            context.DrawRectangle(brush.Brush, null, viewport);

            base.OnRender(context);
        }

        public static Size GetSize(PrintDialogViewModel.ModelDocument document, double? scale = null)
        {
            return new(document.PageSize.Width * (scale ?? document.Zoom) + Parameters.Spacing * 2, document.PageSize.Height * (scale ?? document.Zoom) + Parameters.Spacing * 2);
        }

        public static double GetOffset(PrintDialogViewModel.ModelDocument document, double index, double? unit = null)
        {
            return (unit ?? GetSize(document).Height) * Math.Floor((Math.Max(1, Math.Min(document.PageCount, index)) - 1) / document.ColumnCount);
        }
    }

    internal class DocumentHostPanel : StackPanel
    {
        public class DocumentRow(IEnumerable<PrintDialogViewModel.ModelDocument.Page> pages, double scale)
        {
            public IEnumerable<PrintDialogViewModel.ModelDocument.Page> Pages { get; set; } = pages;
            public double Scale { get; set; } = scale;
        }

        public static readonly DependencyProperty PagesProperty = DependencyProperty.Register(nameof(Pages), typeof(DocumentRow), typeof(DocumentHostPanel), new(null, (x, e) =>
        {
            if (x is not DocumentHostPanel panel || e.NewValue is not DocumentRow row)
            {
                return;
            }

            panel.Children.Clear();
            foreach (PrintDialogViewModel.ModelDocument.Page page in row.Pages)
            {
                panel.Children.Add(new DocumentHostControl(page.Content, row.Scale));
            }
        }));

        public DocumentRow Pages
        {
            get => (DocumentRow)GetValue(PagesProperty);
            set => SetValue(PagesProperty, value);
        }
    }

    internal class DocumentToContentConverter : IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintDialogViewModel.ModelDocument document)
            {
                return Binding.DoNothing;
            }

            List<DocumentHostPanel.DocumentRow> rows = [];
            document.UseDocument(x =>
            {
                for (int i = 0; i < x.Count; i += document.ColumnCount)
                {
                    rows.Add(new(x.GetRange(i, Math.Min(document.ColumnCount, x.Count - i)), document.Zoom));
                }
            }, false);

            return rows;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal class DocumentToDescriptionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values.First() is not double current || values.Last() is not PrintDialogViewModel.ModelDocument document)
            {
                return Binding.DoNothing;
            }

            return $"{PrintDialogWindow.StringResources["StringResource_LabelPage"]} {Math.Floor(current + PrintDialogPage.EPSILON)} / {document.PageCount}";
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. Enumerable.Repeat(Binding.DoNothing, types.Length)];
        }
    }
}