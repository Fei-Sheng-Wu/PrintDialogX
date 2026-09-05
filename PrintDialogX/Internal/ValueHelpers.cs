using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Interop;
using System.Windows.Controls;

namespace PrintDialogX.Internal
{
    internal static class Common
    {
        public const double EPSILON_DECIMAL = 0.000001;
        public const double RATIO_QUANTIZATION = 10;

        public static void Execute(Action executor)
        {
            executor();
        }

        public static T Execute<T>(Func<T> executor)
        {
            return executor();
        }

        public static void Try(Action executor, Action? alternator)
        {
            try
            {
                executor();
            }
            catch
            {
                alternator?.Invoke();
            }
        }

        public static T Try<T>(Func<T> executor, T fallback)
        {
            try
            {
                return executor();
            }
            catch
            {
                return fallback;
            }
        }

        public static T Validate<T>(T value, Func<T, bool> validator, T fallback)
        {
            return validator(value) ? value : fallback;
        }

        public static int Partition(int total, int unit)
        {
            return (total + unit - 1) / unit;
        }

        public static int Clamp(int minimum, int maximum, int value)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        public static double Clamp(double minimum, double maximum, double value)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        public static int Floor(double value, double? epsilon)
        {
            return (int)Math.Floor(value + (epsilon ?? EPSILON_DECIMAL));
        }

        public static int Ceiling(double value, double? epsilon)
        {
            return (int)Math.Ceiling(value - (epsilon ?? EPSILON_DECIMAL));
        }

        public static double Quantize(double value, double? ratio)
        {
            return Math.Round(value * (ratio ?? RATIO_QUANTIZATION));
        }

        public static string Format(string template, string[] values)
        {
            return string.Format(CultureInfo.InvariantCulture, template, values);
        }
    }

    internal sealed class DataContextProxy() : Freezable()
    {
        public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register(nameof(DataContext), typeof(object), typeof(DataContextProxy), new(null));

        public object? DataContext
        {
            get => (object?)GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new DataContextProxy();
        }
    }

    internal sealed class BooleanExtension() : MarkupExtension()
    {
        public bool Value { get; set; } = false;

        public override object ProvideValue(IServiceProvider provider)
        {
            return Value;
        }
    }

    internal sealed class IntegerExtension() : MarkupExtension()
    {
        public int Value { get; set; } = 0;

        public override object ProvideValue(IServiceProvider provider)
        {
            return Value;
        }
    }

    internal sealed class ValidatableBinding() : Binding()
    {
        public ValidationRule? Validation
        {
            set
            {
                ValidationRules.Clear();
                if (value is not ValidationRule validation)
                {
                    return;
                }

                ValidationRules.Add(validation);
            }
        }
    }

    internal sealed class ValueToDescriptionConverter() : LanguageHostConverter(), IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return value is Enum entry && TextResourceAttribute.Parse(entry) is TextResource resource ? GetText(resource) : Binding.DoNothing;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class ComparisonToStateConverter() : IValueConverter
    {
        internal enum Comparison
        {
            Equality,
            Threshold
        }

        internal sealed class Parameter(IEnumerable<object> trues, IEnumerable<object> falses)
        {
            public IEnumerable<object> ValuesTrue { get; set; } = trues;
            public IEnumerable<object> ValuesFalse { get; set; } = falses;
        }

        public Comparison? Mode { get; set; } = null;
        public object StateTrue { get; set; } = true;
        public object StateFalse { get; set; } = false;

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            IEnumerable<object> targets = (parameter as Parameter)?.ValuesTrue ?? [parameter];

            return (Mode switch
            {
                Comparison.Equality => targets.Any(x => Equals(value, x)),
                Comparison.Threshold => targets.Any(x => value is IComparable threshold && threshold.CompareTo(x) >= 0),
                _ => false
            }) ? StateTrue : StateFalse;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return parameter is Parameter { ValuesTrue: IEnumerable<object> trues, ValuesFalse: IEnumerable<object> falses } ? (Equals(value, StateTrue) ? trues : falses).First() : Binding.DoNothing;
        }
    }

    internal sealed class ComparisonToStateParameterExtension() : MarkupExtension()
    {
        public object? TrueFirst { get; set; } = null;
        public object? TrueSecond { get; set; } = null;
        public object? FalseFirst { get; set; } = null;
        public object? FalseSecond { get; set; } = null;

        public override object ProvideValue(IServiceProvider provider)
        {
            return new ComparisonToStateConverter.Parameter((new object?[] { TrueFirst, TrueSecond }).OfType<object>(), (new object?[] { FalseFirst, FalseSecond }).OfType<object>());
        }
    }

    internal sealed class CollectionToRangeConverter() : IValueConverter
    {
        internal sealed class Parameter(int start, int end)
        {
            public int TrimStart { get; set; } = start;
            public int TrimEnd { get; set; } = end;
        }

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if ((value, parameter) is not (IEnumerable originals, Parameter { TrimStart: int start, TrimEnd: int end }))
            {
                return Binding.DoNothing;
            }

            object[] selections = [.. originals];

            return selections.Skip(start).Take(selections.Length - start - end);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class CollectionToRangeParameterExtension() : MarkupExtension()
    {
        public int TrimStart { get; set; } = 0;
        public int TrimEnd { get; set; } = 0;

        public override object ProvideValue(IServiceProvider provider)
        {
            return new CollectionToRangeConverter.Parameter(TrimStart, TrimEnd);
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
            return (x, y) is (PrintQueue first, PrintQueue second) && (ReferenceEquals(first, second) || Common.Try(() => StringComparer.Ordinal.Equals(first.FullName, second.FullName), false));
        }

        public int GetHashCode(PrintQueue value)
        {
            return Common.Try(() => StringComparer.Ordinal.GetHashCode(value.FullName), value.GetHashCode());
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
            public double Size { get; } = size;
            public double Opacity { get; } = opacity;
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
            if ((value, parameter) is not (PrintQueue printer, bool isSmall))
            {
                return Binding.DoNothing;
            }

            (uint index, TextResource name) = (CollectionFax.Contains(printer, PrinterComparer.Instance), CollectionNetwork.Contains(printer, PrinterComparer.Instance) || Common.Try(() => FILTER_NETWORK.Any(x => printer.QueuePort.Name.StartsWith(x, StringComparison.OrdinalIgnoreCase)), false), Common.Try(() => FILTER_FILE.Any(x => printer.QueuePort.Name.StartsWith(x, StringComparison.OrdinalIgnoreCase)), false)) switch
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
                Common.Try(() =>
                {
                    N_IconInfo info = new()
                    {
                        Size = (uint)Marshal.SizeOf<N_IconInfo>()
                    };
                    if (N_GetIcon(index, FLAG_RECEIVE_ICON | (isSmall ? FLAG_SIZE_SMALL : FLAG_SIZE_LARGE), ref info) == RESULT_SUCCESS && info.Icon != IntPtr.Zero)
                    {
                        icon = Imaging.CreateBitmapSourceFromHIcon(info.Icon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        icon.Freeze();
                        CACHE[key] = icon;

                        N_ReleaseIcon(info.Icon);
                    }
                }, null);
            }

            return new PrinterIcon(icon, GetText(name), Common.Try(() =>
            {
                printer.Refresh();
                return printer.IsOffline;
            }, true) ? 0.5 : 1, isSmall ? SizeSmall : SizeLarge);
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class PrinterToStatusConverter() : LanguageHostConverter(), IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            return value is PrintQueue printer ? Common.Try(() =>
            {
                printer.Refresh();
                return GetText(printer.QueueStatus switch
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
                });
            }, GetText(TextResource.LabelError)) : Binding.DoNothing;
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
            if (value is not PrintQueue printer)
            {
                return Binding.DoNothing;
            }

            StringBuilder builder = new();
            Common.Try(printer.Refresh, null);
            Common.Try(() => builder.AppendLine(string.Format(culture, GetText(TextResource.ConstructionDocuments), printer.NumberOfJobs)), null);
            Common.Try(() => builder.AppendLine(string.Format(culture, GetText(TextResource.ConstructionLocation), string.IsNullOrWhiteSpace(printer.Location) ? GetText(TextResource.LabelUnknown) : printer.Location)), null);
            Common.Try(() =>
            {
                if (!string.IsNullOrWhiteSpace(printer.Comment))
                {
                    builder.AppendLine(string.Format(culture, GetText(TextResource.ConstructionComment), printer.Comment));
                }
            }, null);

            return builder.ToString();
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class PagesCustomValidationRule() : ValidationRule()
    {
        public int Maximum { get; set; } = int.MaxValue;

        public override System.Windows.Controls.ValidationResult Validate(object value, CultureInfo culture)
        {
            return value is string pages && TryConvert(pages, Maximum, true).IsValid ? System.Windows.Controls.ValidationResult.ValidResult : new(false, string.Empty);
        }

        public static (bool IsValid, List<object>? Result) TryConvert(string value, int maximum, bool isValidating)
        {
            List<object>? pages = isValidating ? null : new();
            foreach (string[] range in value.Split(',', ';', '，', '、', '､', '﹑', '،', '؛', '﹐').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Split('-', '\u2010', '\u2011', '\u2012', '\u2013', '\u2014', '\u2015', '\ufe58', '\ufe63', '\uff0d')))
            {
                switch (range.Length)
                {
                    case 1 when int.TryParse(range[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int single) && single > 0 && single <= maximum:
                        pages?.Add(single);
                        break;
                    case 2 when int.TryParse(range[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int first) && int.TryParse(range[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int second) && Math.Min(first, second) > 0 && Math.Max(first, second) <= maximum:
                        pages?.Add((Math.Min(first, second), Math.Max(first, second)));
                        break;
                    default:
                        if (isValidating)
                        {
                            return (false, null);
                        }
                        break;
                }
            }

            return (true, pages);
        }

        public static bool CheckIndex(int index, IEnumerable<object>? source)
        {
            return source is not IEnumerable<object> pages || !pages.Any() || pages.Any(x => x switch
            {
                int single => single == index,
                (int start, int end) => start <= index && end >= index,
                _ => false
            });
        }
    }

    internal sealed class SizeToDescriptionConverter() : LanguageHostConverter(), IValueConverter
    {
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if ((value, parameter) is not (Enums.Size size, bool isVerbose))
            {
                return Binding.DoNothing;
            }

            object? name = size.DefinedName is Enums.Size.DefinedSize defined && TextResourceAttribute.Parse(defined) is TextResource resource ? GetText(resource) : size.FallbackName;
            string description = string.Format(culture, GetText(TextResource.ConstructionSize), size.Width * ValueMappings.RATIO_CENTIMETER, size.Height * ValueMappings.RATIO_CENTIMETER);

            return isVerbose ? description : (name ?? string.Format(culture, GetText(TextResource.ConstructionCustom), description));
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
            return value is Enums.Size size ? Common.Floor(Math.Min(size.Width, size.Height) / 2, null) : Binding.DoNothing;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
