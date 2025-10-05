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
            return value.GetType().GetField(value.ToString() ?? string.Empty)?.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute description ? PrintPageViewModel.StringResources[description.Description] : value;
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

        public enum PrinterType
        {
            Printer = 16,
            PrinterNetwork = 50,
            PrinterFile = 54,
            Fax = 52,
            FaxNetwork = 53
        }

        public class PrinterIcon(ImageSource? icon, double opacity, bool isSmall)
        {
            public ImageSource? Icon { get; set; } = icon;
            public double Opacity { get; set; } = opacity;
            public double Size { get; set; } = isSmall ? 18 : 36;
        }

        public static readonly PrintQueueCollection CollectionFax = [];
        public static readonly PrintQueueCollection CollectionNetwork = [];
        public static readonly Dictionary<(PrinterType, bool), ImageSource> Cache = [];

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
                return PrintPageViewModel.StringResources[printer.QueueStatus switch
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
                return PrintPageViewModel.StringResources["StringResource_LabelError"];
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
                info.Add($"{PrintPageViewModel.StringResources["StringResource_PrefixDocuments"]}{printer.NumberOfJobs}");
            }
            catch { }
            try
            {
                info.Add($"{PrintPageViewModel.StringResources["StringResource_PrefixLocation"]}{(string.IsNullOrWhiteSpace(printer.Location) ? PrintPageViewModel.StringResources["StringResource_LabelUnknown"] : printer.Location)}");
            }
            catch { }
            try
            {
                if (!string.IsNullOrWhiteSpace(printer.Comment))
                {
                    info.Add($"{PrintPageViewModel.StringResources["StringResource_PrefixComment"]}{printer.Comment}");
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
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(CustomPagesValidationParameters), new(int.MaxValue));

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
            return name ?? $"{PrintPageViewModel.StringResources["StringResource_PrefixCustom"]}{Math.Round(size.Width * 2.54 / 96, 2)} × {Math.Round(size.Height * 2.54 / 96, 2)} cm";
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

    internal class DocumentHostControl : ContentControl
    {
        public DocumentHostControl()
        {
            InheritanceBehavior = InheritanceBehavior.SkipToThemeNow;
        }
    }

    internal class DocumentToContentConverter : IValueConverter
    {
        public class Row(FrameworkElement content)
        {
            public FrameworkElement Content { get; set; } = content;
        }

        public class Cell(FrameworkElement content, double scale)
        {
            public FrameworkElement Content { get; set; } = content;
            public double Scale { get; set; } = scale;
        }

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PrintPageViewModel.ModelDocument document || parameter is not DataTemplate template)
            {
                return Binding.DoNothing;
            }

            List<Row> rows = [];
            lock (document.DocumentLock)
            {
                for (int i = 0; i < document.Document.Count; i += document.ColumnCount)
                {
                    StackPanel container = new()
                    {
                        Orientation = Orientation.Horizontal
                    };
                    foreach (PrintPageViewModel.ModelDocument.ModelPage page in document.Document.GetRange(i, Math.Min(document.ColumnCount, document.Document.Count - i)))
                    {
                        ContentPresenter content = new()
                        {
                            ContentTemplate = template,
                            Content = new Cell(page.Content, document.Zoom)
                        };
                        container.Children.Add(content);
                    }

                    rows.Add(new(container));
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
        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values.First() is not double current || values.Last() is not PrintPageViewModel.ModelDocument viewer)
            {
                return Binding.DoNothing;
            }

            return $"{PrintPageViewModel.StringResources["StringResource_LabelPage"]} {Math.Floor(current + PrintDialogPage.EPSILON)} / {viewer.Document.Count}";
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. Enumerable.Repeat(Binding.DoNothing, types.Length)];
        }
    }

    internal class DocumentColorEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(DocumentColorEffect), 0);

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        public DocumentColorEffect(string name)
        {
            PixelShader = new()
            {
                UriSource = new($"/PrintDialogX;component/Resources/Effects/{name}.ps", UriKind.Relative)
            };
            UpdateShaderValue(InputProperty);
        }
    }
}