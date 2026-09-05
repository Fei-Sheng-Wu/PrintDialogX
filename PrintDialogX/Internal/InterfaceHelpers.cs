using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;

namespace PrintDialogX.Internal
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class LanguageAttribute(string language, FlowDirection direction) : Attribute()
    {
        public string Language { get; } = language;
        public FlowDirection Direction { get; } = direction;

        public static (XmlLanguage Language, FlowDirection Direction, ResourceDictionary Resources) Parse(InterfaceSettings.Language language)
        {
            if (language is InterfaceSettings.Language.System)
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

            LanguageAttribute attribute = (Enum.GetName(typeof(InterfaceSettings.Language), language) is string name ? typeof(InterfaceSettings.Language).GetField(name)?.GetCustomAttribute<LanguageAttribute>() : null) ?? new("en-US", FlowDirection.LeftToRight);
            return (XmlLanguage.GetLanguage(attribute.Language), attribute.Direction, new()
            {
                Source = new(Common.Format("/PrintDialogX;component/Resources/Languages/{0}.xaml", [attribute.Language]), UriKind.Relative)
            });
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class TextResourceAttribute(TextResource resource) : Attribute()
    {
        public TextResource Resource { get; } = resource;

        public static TextResource? Parse(Enum value)
        {
            return Enum.GetName(value.GetType(), value) is string name ? value.GetType().GetField(name)?.GetCustomAttribute<TextResourceAttribute>()?.Resource : null;
        }
    }

    internal abstract class LanguageHostConverter()
    {
        public ResourceDictionary? Resources { get; set; } = null;

        public string GetText(TextResource resource)
        {
            return Resources?[resource] as string ?? string.Empty;
        }
    }

    internal sealed class InterfaceToContentConverter() : IValueConverter
    {
        public ResourceDictionary Components { get; set; } = [];

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not InterfaceSettings settings)
            {
                return Binding.DoNothing;
            }

            FrameworkElement element = (FrameworkElement)((ControlTemplate)parameter).LoadContent();
            foreach ((string name, InterfaceSettings.Option[] options) in new (string, InterfaceSettings.Option[])[] { ("PART_Basic", settings.BasicSettings), ("PART_Advanced", settings.AdvancedSettings) })
            {
                Panel area = (Panel)element.FindName(name);
                foreach (ControlTemplate template in options.SelectMany(x => Components[x] switch
                {
                    ControlTemplate single => [single],
                    IEnumerable<ControlTemplate> siblings => siblings,
                    _ => []
                }))
                {
                    area.Children.Add((FrameworkElement)template.LoadContent());
                }
            }

            Expander expander = (Expander)element.FindName("PART_Expander");
            expander.IsExpanded = settings.IsSettingsExpanded;
            if (settings.AdvancedSettings.Length <= 0)
            {
                expander.Visibility = Visibility.Collapsed;
            }

            return element;
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class CompositeItemStyleSelector() : StyleSelector()
    {
        public Style? Preview { get; set; } = null;
        public Style? Data { get; set; } = null;
        public Style? Element { get; set; } = null;
        public Style? Decoration { get; set; } = null;

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            while (container is DependencyObject element)
            {
                if (element is ComboBoxItem)
                {
                    return item switch
                    {
                        Separator => Decoration,
                        FrameworkElement => Element,
                        _ => Data
                    };
                }

                container = VisualTreeHelper.GetParent(element);
            }

            return Preview;
        }
    }

    internal sealed class CompositeItemTemplateSelector() : DataTemplateSelector()
    {
        public DataTemplate? Preview { get; set; } = null;
        public DataTemplate? Data { get; set; } = null;
        public DataTemplate? Element { get; set; } = null;
        public DataTemplate? Decoration { get; set; } = null;

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            while (container is DependencyObject element)
            {
                if (element is ComboBoxItem)
                {
                    return item switch
                    {
                        Separator => Decoration,
                        FrameworkElement => Element,
                        _ => Data
                    };
                }

                container = VisualTreeHelper.GetParent(element);
            }

            return Preview;
        }
    }
}
