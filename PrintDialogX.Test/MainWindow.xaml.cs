using System;
using System.Linq;
using System.Printing;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace PrintDialogX.Test
{
    public partial class MainWindow : Window
    {
        private static readonly Dictionary<string, (object? Initial, Func<object?> Callback)> configurations = [];
        private static readonly Dictionary<object, (Func<int, PrintDocument, PrintSettings, FrameworkElement> Callback, bool IsDynamic, bool IsSensitive)> templates = new()
        {
            { "Debug Information Test", (GenerateContentDebugInformation, true, true) },
            { "UI Library Test", (GenerateContentUILibrary, false, false) },
            { "Mock Dataset Test", (GenerateContentMockDataset, true, false) },
        };

        #region Core Logic

        public MainWindow()
        {
            InitializeComponent();

            foreach (object entry in templates.Keys)
            {
                optionTemplate.Items.Add(entry);
            }
            optionTemplate.SelectedItem = "UI Library Test";

            AddOption(containerDocument, "documentAsynchronous", "Generation", CreateCheck("Asynchronous"), true);
            AddOption(containerDocument, "documentName", "Name", new TextBox(), string.Empty);
            AddOption(containerDocument, "documentSize", "Size", CreateCombo<Enums.Size.DefinedSize>("(Dynamic Size)"), "(Dynamic Size)");
            AddOption(containerDocument, "documentMargin", "Margin", new TextBox(), "60.0");

            AddOption(containerWindow, "windowDialog", "Window", CreateCheck("Show as Dialog"), true);
            AddOption(containerWindow, "windowTopmost", null, CreateCheck("Topmost"), false);
            AddOption(containerWindow, "windowResizable", null, CreateCheck("Resizable"), true);
            AddOption(containerWindow, "windowTaskbar", null, CreateCheck("Show In Taskbar"), true);
            AddOption(containerWindow, "windowWidth", "Window Width", new TextBox(), string.Empty);
            AddOption(containerWindow, "windowHeight", "Window Height", new TextBox(), string.Empty);

            AddOption(containerInterface, "interfaceTitle", "Title", new TextBox(), string.Empty);
            AddOption(containerInterface, "interfaceIcon", "Icon", new TextBox(), string.Empty);
            AddOption(containerInterface, "interfaceLanguage", "Display Language", CreateCombo<InterfaceSettings.Language>(), InterfaceSettings.Language.en_US);
            AddOption(containerInterface, "interfaceSettingsBasic", "Basic Settings", new TextBox(), "Printer, PrinterPreferences, Void, Copies, Collation, Pages, Layout, Size");
            AddOption(containerInterface, "interfaceSettingsAdvanced", "Advanced Settings", new TextBox(), "Color, Quality, PagesPerSheet, PageOrder, Scale, Margin, DoubleSided, Type, Source");

            AddOption(containerPrinter, "printerServer", "Print Server", new TextBox(), string.Empty);
            AddOption(containerPrinter, "printerDefault", "Default Printer", CreateCombo(new PrintServer().GetPrintQueues().Select(x => x.FullName), "(System Default)"), "(System Default)");

            AddOption(containerDefault, "defaultCopies", "Default Copies", new TextBox(), string.Empty);
            AddOption(containerDefault, "defaultCollation", "Default Collation", CreateCombo<Enums.Collation>(), null);
            AddOption(containerDefault, "defaultPages", "Default Pages", CreateCombo<Enums.Pages>(), null);
            AddOption(containerDefault, "defaultPagesCustom", "Default Custom Pages", new TextBox(), string.Empty);
            AddOption(containerDefault, "defaultLayout", "Default Layout", CreateCombo<Enums.Layout>(), null);
            AddOption(containerDefault, "defaultSize", "Default Size", CreateCombo<Enums.Size.DefinedSize>("(Printer Default)"), "(Printer Default)");
            AddOption(containerDefault, "defaultColor", "Default Color", CreateCombo<Enums.Color>("(Printer Default)"), "(Printer Default)");
            AddOption(containerDefault, "defaultQuality", "Default Quality", CreateCombo<Enums.Quality>("(Printer Default)"), "(Printer Default)");
            AddOption(containerDefault, "defaultPagesPerSheet", "Default Pages per Sheet", CreateCombo<Enums.PagesPerSheet>(), null);
            AddOption(containerDefault, "defaultPageOrder", "Default Page Order", CreateCombo<Enums.PageOrder>(), null);
            AddOption(containerDefault, "defaultScale", "Default Scale", CreateCombo<Enums.Scale>(), null);
            AddOption(containerDefault, "defaultScaleCustom", "Default Custom Scale", new TextBox(), string.Empty);
            AddOption(containerDefault, "defaultMargin", "Default Margin", CreateCombo<Enums.Margin>(), null);
            AddOption(containerDefault, "defaultMarginCustom", "Default Custom Margin", new TextBox(), string.Empty);
            AddOption(containerDefault, "defaultDoubleSided", "Default Double-Sided", CreateCombo<Enums.DoubleSided>(), null);
            AddOption(containerDefault, "defaultType", "Default Type", CreateCombo<Enums.Type>("(Printer Default)"), "(Printer Default)");
            AddOption(containerDefault, "defaultSource", "Default Source", CreateCombo<Enums.Source>("(Printer Default)"), "(Printer Default)");
        }

        private static CheckBox CreateCheck(string content)
        {
            return new CheckBox()
            {
                Content = content
            };
        }

        private static ComboBox CreateCombo<T>(object? initial = null)
        {
            return CreateCombo(Enum.GetValues(typeof(T)), initial);
        }

        private static ComboBox CreateCombo(IEnumerable collection, object? initial = null)
        {
            ComboBox combo = new();
            foreach (object entry in initial != null ? (IEnumerable<object>)[initial, .. collection] : collection)
            {
                combo.Items.Add(entry);
            }

            return combo;
        }

        private static void AddOption<T>(Panel parent, string name, string? header, T content, object? initial) where T : FrameworkElement
        {
            DockPanel container = new()
            {
                Margin = new(0, 8, 0, 0)
            };

            TextBlock description = new()
            {
                Width = 140,
                Text = header != null ? $"{header}:" : string.Empty
            };
            DockPanel.SetDock(description, Dock.Left);
            container.Children.Add(description);

            switch (content)
            {
                case TextBox text:
                    text.Text = Convert.ToString(initial);
                    configurations.Add(name, (initial, () => text.Text));
                    break;
                case CheckBox check:
                    check.IsChecked = Convert.ToBoolean(initial);
                    configurations.Add(name, (initial, () => check.IsChecked));
                    break;
                case ComboBox combo:
                    combo.SelectedItem = initial;
                    configurations.Add(name, (initial, () => combo.SelectedItem));
                    break;
            }
            DockPanel.SetDock(content, Dock.Right);
            container.Children.Add(content);

            parent.Children.Add(container);
        }

        private static bool CheckConfiguration(string name)
        {
            return !Equals(configurations[name].Callback(), configurations[name].Initial);
        }

        private static void HandleConfiguration<T>(string name, Action<T> callback)
        {
            object? value = configurations[name].Callback();
            if (value is T parameter && !Equals(value, configurations[name].Initial))
            {
                callback(parameter);
            }
        }

        private void OpenDialog(object sender, RoutedEventArgs e)
        {
            textCode.Text = string.Empty;

            GenerateCode();
            GenerateCode("// Create a new document");

            PrintDocument document = new();
            GenerateCode("PrintDialogX.PrintDocument document = new PrintDialogX.PrintDocument();");

            HandleConfiguration<string>("documentName", x =>
            {
                document.DocumentName = x;
                GenerateCode($"document.DocumentName = \"{x}\";");
            });
            HandleConfiguration<Enums.Size.DefinedSize>("documentSize", x =>
            {
                document.DocumentSize = new(x);
                GenerateCode($"document.DocumentSize = new PrintDialogX.Enums.Size(PrintDialogX.Enums.Size.DefinedSize.{x});");
            });
            HandleConfiguration<string>("documentMargin", x =>
            {
                document.DocumentMargin = double.Parse(x);
                GenerateCode($"document.DocumentMargin = {x};");
            });

            bool isAsynchronous = true;
            HandleConfiguration<bool>("documentAsynchronous", x =>
            {
                isAsynchronous = false;

                GenerateCode();
                GenerateCode("// Create the pages of the document");

                GenerateCode($"for (int i = 0; i < {optionCount.Text}; i++)");
                GenerateCode("{");
                GenerateCode("PrintDialogX.PrintPage page = new PrintDialogX.PrintPage();", 1);
                GenerateCode("page.Content = GenerateContent(i);", 1);
                GenerateCode("document.Pages.Add(page);", 1);
                GenerateCode("}");
            });

            if (templates[optionTemplate.SelectedItem].IsDynamic)
            {
                GenerateCode();
                GenerateCode("// Add the event listener for updates to print settings");

                document.PrintSettingsChanged += HandlePrintSettingsChanged;
                GenerateCode("document.PrintSettingsChanged += HandlePrintSettingsChanged;");
            }

            GenerateCode();
            GenerateCode("// Initialize the print dialog");

            PrintDialog dialog = new();
            if (CheckConfiguration("windowTopmost") || CheckConfiguration("windowResizable") || CheckConfiguration("windowTaskbar") || CheckConfiguration("windowWidth") || CheckConfiguration("windowHeight"))
            {
                GenerateCode("PrintDialogX.PrintDialog dialog = new PrintDialogX.PrintDialog(window =>");
                GenerateCode("{");
                GenerateCode("// Customize the dialog window", 1);

                HandleConfiguration<bool>("windowTopmost", x =>
                {
                    ((Window)dialog.Host).Topmost = true;
                    GenerateCode("window.Topmost = true;", 1);
                });
                HandleConfiguration<bool>("windowResizable", x =>
                {
                    ((Window)dialog.Host).ResizeMode = ResizeMode.NoResize;
                    GenerateCode("window.ResizeMode = ResizeMode.NoResize;", 1);
                });
                HandleConfiguration<bool>("windowTaskbar", x =>
                {
                    ((Window)dialog.Host).ShowInTaskbar = false;
                    GenerateCode("window.ShowInTaskbar = false;", 1);
                });
                HandleConfiguration<string>("windowWidth", x =>
                {
                    ((Window)dialog.Host).Width = double.Parse(x);
                    GenerateCode($"window.Width = {x};", 1);
                });
                HandleConfiguration<string>("windowHeight", x =>
                {
                    ((Window)dialog.Host).Height = double.Parse(x);
                    GenerateCode($"window.Height = {x};", 1);
                });

                GenerateCode("});");
            }
            else
            {
                GenerateCode("PrintDialogX.PrintDialog dialog = new PrintDialogX.PrintDialog();");
            }
            if (!isAsynchronous)
            {
                dialog.Document = document;
                GenerateCode("dialog.Document = document;");
            }

            GenerateCode();
            GenerateCode("// Customize the interface");

            HandleConfiguration<string>("interfaceTitle", x =>
            {
                dialog.InterfaceSettings.Title = x;
                GenerateCode($"dialog.InterfaceSettings.Title = \"{x}\";");
            });
            HandleConfiguration<string>("interfaceIcon", x =>
            {
                dialog.InterfaceSettings.Icon = new Wpf.Ui.Controls.ImageIcon()
                {
                    Source = BitmapFrame.Create(new Uri(x))
                };
                GenerateCode("dialog.InterfaceSettings.Icon = new Wpf.Ui.Controls.ImageIcon()");
                GenerateCode("{");
                GenerateCode($"Source = BitmapFrame.Create(new Uri(\"{x}\"))", 1);
                GenerateCode("};");
            });
            HandleConfiguration<InterfaceSettings.Language>("interfaceLanguage", x =>
            {
                dialog.InterfaceSettings.DisplayLanguage = x;
                GenerateCode($"dialog.InterfaceSettings.DisplayLanguage = PrintDialogX.InterfaceSettings.Language.{x};");
            });
            HandleConfiguration<string>("interfaceSettingsBasic", x =>
            {
                dialog.InterfaceSettings.BasicSettings = [.. x.Split(',').Select(x => Enum.Parse<InterfaceSettings.Option>(x))];
                GenerateCode($"dialog.InterfaceSettings.BasicSettings = [{string.Join(", ", x.Split(',').Select(x => $"PrintDialogX.InterfaceSettings.Option.{x.Trim()}"))}];");
            });
            HandleConfiguration<string>("interfaceSettingsAdvanced", x =>
            {
                dialog.InterfaceSettings.AdvancedSettings = [.. x.Split(',').Select(x => Enum.Parse<InterfaceSettings.Option>(x))];
                GenerateCode($"dialog.InterfaceSettings.AdvancedSettings = [{string.Join(", ", x.Split(',').Select(x => $"PrintDialogX.InterfaceSettings.Option.{x.Trim()}"))}];");
            });

            if (CheckConfiguration("printerServer") || CheckConfiguration("printerDefault"))
            {
                GenerateCode();
                GenerateCode("// Configure the printers to use");

                HandleConfiguration<string>("printerServer", x =>
                {
                    dialog.PrintServer = new(x);
                    GenerateCode($"dialog.PrintServer = new PrintServer(\"{x}\"); // Remember to dispose the PrintServer instance when appropriate");
                });
                HandleConfiguration<string>("printerDefault", x =>
                {
                    dialog.DefaultPrinter = new PrintServer().GetPrintQueue(x);
                    GenerateCode($"dialog.DefaultPrinter = GetPrintQueue(\"{x}\");");
                });
            }

            GenerateCode();
            GenerateCode("// Customize the default settings");

            HandleConfiguration<string>("defaultCopies", x =>
            {
                dialog.PrintSettings.Copies = int.Parse(x);
                GenerateCode($"dialog.PrintSettings.Copies = {x};");
            });
            HandleConfiguration<Enums.Collation>("defaultCollation", x =>
            {
                dialog.PrintSettings.Collation = x;
                GenerateCode($"dialog.PrintSettings.Collation = PrintDialogX.Enums.Collation.{x};");
            });
            HandleConfiguration<Enums.Pages>("defaultPages", x =>
            {
                dialog.PrintSettings.Pages = x;
                GenerateCode($"dialog.PrintSettings.Pages = PrintDialogX.Enums.Pages.{x};");
            });
            HandleConfiguration<string>("defaultPagesCustom", x =>
            {
                dialog.PrintSettings.CustomPages = x;
                GenerateCode($"dialog.PrintSettings.CustomPages = \"{x}\";");
            });
            HandleConfiguration<Enums.Layout>("defaultLayout", x =>
            {
                dialog.PrintSettings.Layout = x;
                GenerateCode($"dialog.PrintSettings.Layout = PrintDialogX.Enums.Layout.{x};");
            });
            HandleConfiguration<Enums.Size.DefinedSize>("defaultSize", x =>
            {
                dialog.PrintSettings.Size = new(x);
                GenerateCode($"dialog.PrintSettings.Size = new PrintDialogX.Enums.Size(PrintDialogX.Enums.Size.DefinedSize.{x});");
            });
            HandleConfiguration<Enums.Color>("defaultColor", x =>
            {
                dialog.PrintSettings.Color = x;
                GenerateCode($"dialog.PrintSettings.Color = PrintDialogX.Enums.Color.{x};");
            });
            HandleConfiguration<Enums.Quality>("defaultQuality", x =>
            {
                dialog.PrintSettings.Quality = x;
                GenerateCode($"dialog.PrintSettings.Quality = PrintDialogX.Enums.Quality.{x};");
            });
            HandleConfiguration<Enums.PagesPerSheet>("defaultPagesPerSheet", x =>
            {
                dialog.PrintSettings.PagesPerSheet = x;
                GenerateCode($"dialog.PrintSettings.PagesPerSheet = PrintDialogX.Enums.PagesPerSheet.{x};");
            });
            HandleConfiguration<Enums.PageOrder>("defaultPageOrder", x =>
            {
                dialog.PrintSettings.PageOrder = x;
                GenerateCode($"dialog.PrintSettings.PageOrder = PrintDialogX.Enums.PageOrder.{x};");
            });
            HandleConfiguration<Enums.Scale>("defaultScale", x =>
            {
                dialog.PrintSettings.Scale = x;
                GenerateCode($"dialog.PrintSettings.Scale = PrintDialogX.Enums.Scale.{x};");
            });
            HandleConfiguration<string>("defaultScaleCustom", x =>
            {
                dialog.PrintSettings.CustomScale = int.Parse(x);
                GenerateCode($"dialog.PrintSettings.CustomScale = {x};");
            });
            HandleConfiguration<Enums.Margin>("defaultMargin", x =>
            {
                dialog.PrintSettings.Margin = x;
                GenerateCode($"dialog.PrintSettings.Margin = PrintDialogX.Enums.Margin.{x};");
            });
            HandleConfiguration<string>("defaultMarginCustom", x =>
            {
                dialog.PrintSettings.CustomMargin = int.Parse(x);
                GenerateCode($"dialog.PrintSettings.CustomMargin = {x};");
            });
            HandleConfiguration<Enums.DoubleSided>("defaultDoubleSided", x =>
            {
                dialog.PrintSettings.DoubleSided = x;
                GenerateCode($"dialog.PrintSettings.DoubleSided = PrintDialogX.Enums.DoubleSided.{x};");
            });
            HandleConfiguration<Enums.Type>("defaultType", x =>
            {
                dialog.PrintSettings.Type = x;
                GenerateCode($"dialog.PrintSettings.Type = PrintDialogX.Enums.Type.{x};");
            });
            HandleConfiguration<Enums.Source>("defaultSource", x =>
            {
                dialog.PrintSettings.Source = x;
                GenerateCode($"dialog.PrintSettings.Source = PrintDialogX.Enums.Source.{x};");
            });
            if (!isAsynchronous)
            {
                GenerateDocument(document, dialog.PrintSettings);
            }

            GenerateCode();
            GenerateCode("// Open the print dialog");

            bool isDialog = true;
            HandleConfiguration<bool>("windowDialog", x => isDialog = false);
            Func<Task> callback = () => Task.CompletedTask;
            if (isAsynchronous)
            {
                callback = async () =>
                {
                    await GenerateDocumentAsync(document, dialog.PrintSettings);
                    dialog.Document = document;
                };
                GenerateCode($"dialog.{(isDialog ? "ShowDialog" : "Show")}(async () =>");
                GenerateCode("{");
                GenerateCode("// Create the pages of the document asynchronously", 1);
                GenerateCode($"for (int i = 0; i < {optionCount.Text}; i++)", 1);
                GenerateCode("{", 1);
                GenerateCode("PrintDialogX.PrintPage page = new PrintDialogX.PrintPage();", 2);
                GenerateCode("page.Content = await GenerateContentAsync(i);", 2);
                GenerateCode("document.Pages.Add(page);", 2);
                GenerateCode("}", 1);
                GenerateCode("dialog.Document = document;", 1);
                GenerateCode("});");
            }
            else
            {
                GenerateCode($"dialog.{(isDialog ? "ShowDialog" : "Show")}();");
            }
            ((Action)(isDialog ? () => dialog.ShowDialog(callback) : () => dialog.Show(callback)))();
        }

        private void GenerateCode(string? code = null, int level = 0)
        {
            textCode.Text += $"{(string.IsNullOrEmpty(textCode.Text) ? string.Empty : Environment.NewLine)}{new string(' ', level * 4)}{code}";
        }

        private void GenerateDocument(PrintDocument document, PrintSettings settings)
        {
            for (int i = 0; i < int.Parse(optionCount.Text); i++)
            {
                document.Pages.Add(GeneratePage(i, document, settings));
            }
        }

        private async Task GenerateDocumentAsync(PrintDocument document, PrintSettings settings)
        {
            for (int i = 0; i < int.Parse(optionCount.Text); i++)
            {
                document.Pages.Add(GeneratePage(i, document, settings));

                await Dispatcher.Yield();
            }
        }

        private PrintPage GeneratePage(int index, PrintDocument document, PrintSettings settings)
        {
            return new PrintPage()
            {
                Content = templates[optionTemplate.SelectedItem].Callback(index, document, settings)
            };
        }

        private async void HandlePrintSettingsChanged(object? sender, PrintSettingsEventArgs e)
        {
            if (sender is not PrintDocument document)
            {
                return;
            }

            if (templates[optionTemplate.SelectedItem].IsSensitive)
            {
                e.IsUpdating = true;
            }
            e.IsBlocking = true;

            int index = 0;
            foreach (PrintPage page in document.Pages)
            {
                page.Content = templates[optionTemplate.SelectedItem].Callback(index, document, e.CurrentSettings);
                index++;

                await Dispatcher.Yield();
            }

            e.IsBlocking = false;
        }

        #endregion

        #region Debug Information Test

        private static FrameworkElement GenerateContentDebugInformation(int index, PrintDocument document, PrintSettings settings)
        {
            double sizeGuideline = 24;

            Brush brushPlaceholder = Brushes.WhiteSmoke;
            Brush brushDeadzone = Brushes.LightPink;
            Brush brushGuideline = Brushes.Orange;
            Brush brushGuidelineFill = Brushes.LightYellow;
            Brush brushCenterline = Brushes.LightBlue;

            Border container = new()
            {
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new(1)
            };

            Grid grid = new();
            container.Child = grid;

            StackPanel panel = new() { Orientation = Orientation.Vertical };
            grid.Children.Add(new Border() { Margin = new(sizeGuideline - 1), Padding = new(12), BorderBrush = brushGuideline, BorderThickness = new(1), Child = panel });

            panel.Children.Add(new TextBlock() { Padding = new(8, 4, 8, 4), FontSize = 24, FontWeight = FontWeights.Bold, Background = brushPlaceholder, Text = $"Test Page #{index + 1}" });
            panel.Children.Add(new TextBlock() { Margin = new(0, 16, 0, 0), FontSize = 16, FontWeight = FontWeights.Medium, Text = "Document Information" });
            panel.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Document Name:",-25} \"{document.DocumentName}\"" });
            panel.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Document Size:",-25} {(document.DocumentSize != null ? $"\"{document.DocumentSize.Value.DefinedName?.ToString() ?? document.DocumentSize.Value.FallbackName}\" ({document.DocumentSize.Value.Width} × {document.DocumentSize.Value.Height} px)" : "(Dynamic Size)")}" });
            panel.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Document Margin:",-25} {document.DocumentMargin}" });
            panel.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Document Page Count:",-25} {document.PageCount}" });
            panel.Children.Add(new TextBlock() { Margin = new(0, 32, 0, 0), FontSize = 16, FontWeight = FontWeights.Medium, Text = "Print Settings Information" });

            Grid columns = new();
            columns.ColumnDefinitions.Add(new());
            columns.ColumnDefinitions.Add(new());
            panel.Children.Add(columns);

            StackPanel left = new() { Orientation = Orientation.Vertical };
            Grid.SetColumn(left, 0);
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Copies:",-20} {settings.Copies}" });
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Collation:",-20} {settings.Collation}" });
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Pages:",-20} {settings.Pages}" });
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Custom Pages:",-20} {settings.CustomPages}" });
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Layout:",-20} {settings.Layout}" });
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Size:",-20} {(settings.Size != null ? settings.Size.Value.DefinedName?.ToString() ?? settings.Size.Value.FallbackName : string.Empty)}" });
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Color:",-20} {settings.Color}" });
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Quality:",-20} {settings.Quality}" });
            left.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Pages per Sheet:",-20} {settings.PagesPerSheet}" });
            columns.Children.Add(left);

            StackPanel right = new() { Orientation = Orientation.Vertical };
            Grid.SetColumn(right, 1);
            right.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Page Order:",-20} {settings.PageOrder}" });
            right.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Scale:",-20} {settings.Scale}" });
            right.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Custom Scale:",-20} {settings.CustomScale}" });
            right.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Margin:",-20} {settings.Margin}" });
            right.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Custom Margin:",-20} {settings.CustomMargin}" });
            right.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Double-Sided:",-20} {settings.DoubleSided}" });
            right.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Type:",-20} {settings.Type}" });
            right.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Source:",-20} {settings.Source}" });
            columns.Children.Add(right);

            panel.Children.Add(new TextBlock() { Margin = new(0, 32, 0, 0), FontSize = 16, FontWeight = FontWeights.Medium, Text = "Miscellaneous" });
            panel.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 0), FontFamily = new("Consolas"), Text = $"{"Page Generation Time:",-25} {DateTime.Now:yyyy-MM-dd (UTCzzz) HH\\:mm\\:ss\\:fffffff}" });

            grid.Children.Add(new Rectangle() { Margin = new(sizeGuideline + 12), Height = sizeGuideline, VerticalAlignment = VerticalAlignment.Bottom, Fill = brushDeadzone });
            grid.Children.Add(new Border() { Width = sizeGuideline, Height = sizeGuideline, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Background = brushGuidelineFill, BorderBrush = brushGuideline, BorderThickness = new(0, 0, 1, 1) });
            grid.Children.Add(new Border() { Width = sizeGuideline, Height = sizeGuideline, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top, Background = brushGuidelineFill, BorderBrush = brushGuideline, BorderThickness = new(1, 0, 0, 1) });
            grid.Children.Add(new Border() { Width = sizeGuideline, Height = sizeGuideline, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom, Background = brushGuidelineFill, BorderBrush = brushGuideline, BorderThickness = new(0, 1, 1, 0) });
            grid.Children.Add(new Border() { Width = sizeGuideline, Height = sizeGuideline, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Background = brushGuidelineFill, BorderBrush = brushGuideline, BorderThickness = new(1, 1, 0, 0) });
            grid.Children.Add(new Line() { X1 = 0, Y1 = 0, X2 = 1, Y2 = 0, Stretch = Stretch.Fill, VerticalAlignment = VerticalAlignment.Center, Stroke = brushCenterline, StrokeThickness = 1 });
            grid.Children.Add(new Line() { X1 = 0, Y1 = 0, X2 = 0, Y2 = 1, Stretch = Stretch.Fill, HorizontalAlignment = HorizontalAlignment.Center, Stroke = brushCenterline, StrokeThickness = 1 });
            grid.Children.Add(new Rectangle() { Width = sizeGuideline, Height = sizeGuideline, Stroke = brushCenterline, StrokeThickness = 1 });

            return container;
        }

        #endregion

        #region UI Library Test

        private static FrameworkElement GenerateContentUILibrary(int index, PrintDocument document, PrintSettings settings)
        {
            Brush brushPrimary = Brushes.SlateGray;
            Brush brushContrast = Brushes.White;

            Border container = new()
            {
                Background = new LinearGradientBrush(Colors.SlateGray, Colors.DimGray, 90)
            };

            StackPanel panel = new() { Margin = new(32), Orientation = Orientation.Vertical };
            container.Child = new Border() { Margin = new(32), CornerRadius = new(16), Background = Brushes.WhiteSmoke, BorderBrush = brushPrimary, BorderThickness = new(1), Opacity = 0.9, Child = panel };

            panel.Children.Add(new TextBlock() { FontSize = 36, FontWeight = FontWeights.Bold, Foreground = brushPrimary, Text = "Lorem Ipsum" });
            panel.Children.Add(new TextBlock() { Margin = new(0, 24, 0, 0), FontSize = 14, TextWrapping = TextWrapping.Wrap, Foreground = brushPrimary, Text = "Lorem ipsum dolor sit amet consectetur adipiscing elit. Quisque faucibus ex sapien vitae pellentesque sem placerat. In id cursus mi pretium tellus duis convallis. Tempus leo eu aenean sed diam urna tempor. Pulvinar vivamus fringilla lacus nec metus bibendum egestas. Iaculis massa nisl malesuada lacinia integer nunc posuere. Ut hendrerit semper vel class aptent taciti sociosqu. Ad litora torquent per conubia nostra inceptos himenaeos." });

            StackPanel buttons = new() { Margin = new(0, 16, 0, 0), HorizontalAlignment = HorizontalAlignment.Right, Orientation = Orientation.Horizontal };
            buttons.Children.Add(new Button() { Width = 128, Height = 32, Background = brushPrimary, Foreground = brushContrast, BorderThickness = new(0), Content = "Get Started" });
            buttons.Children.Add(new Button() { Margin = new(16, 0, 0, 0), Width = 128, Height = 32, Background = brushContrast, Foreground = brushPrimary, BorderBrush = brushPrimary, Content = "Learn More" });
            panel.Children.Add(buttons);

            WriteableBitmap bitmap = new(128, 32, 96, 96, PixelFormats.Bgr32, null);
            byte[] bitmapPixels = new byte[128 * 32 * 4];
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    int i = y * 128 * 4 + x * 4;
                    bitmapPixels[i + 0] = (byte)((Math.Sin(x * 0.15) * 0.5 + 0.5) * 255);
                    bitmapPixels[i + 1] = (byte)((Math.Sin(y * 0.15) * 0.5 + 0.5) * 255);
                    bitmapPixels[i + 2] = (byte)((Math.Sin((x + y) * 0.15) * 0.5 + 0.5) * 255);
                }
            }
            bitmap.WritePixels(new(0, 0, 128, 32), bitmapPixels, 128 * 4, 0);
            bitmap.Freeze();
            panel.Children.Add(new Image() { Margin = new(0, 32, 0, 0), Source = bitmap });

            DockPanel input = new() { Margin = new(0, 16, 0, 0) };
            input.Children.Add(new CheckBox() { Width = 140, Foreground = brushPrimary, VerticalAlignment = VerticalAlignment.Center, IsChecked = true, Content = "TextBox:" });
            input.Children.Add(new TextBox() { Margin = new(8, 0, 0, 0), Padding = new(8), FontStyle = FontStyles.Italic, Foreground = brushPrimary, Text = "Lorem ipsum dolor sit amet consectetur adipiscing elit." });
            panel.Children.Add(input);

            DockPanel combo = new() { Margin = new(0, 8, 0, 0) };
            combo.Children.Add(new CheckBox() { Width = 140, Foreground = brushPrimary, VerticalAlignment = VerticalAlignment.Center, IsChecked = null, Content = "ComboBox:" });
            combo.Children.Add(new ComboBox() { Margin = new(8, 0, 0, 0), Padding = new(8), FontWeight = FontWeights.Bold, Foreground = brushPrimary, SelectedIndex = 0, ItemsSource = new string[] { "Lorem ipsum dolor sit amet consectetur adipiscing elit." } });
            panel.Children.Add(combo);

            DockPanel slider = new() { Margin = new(0, 8, 0, 0) };
            slider.Children.Add(new RadioButton() { Width = 140, Foreground = brushPrimary, VerticalAlignment = VerticalAlignment.Center, IsChecked = true, Content = "Slider:" });
            slider.Children.Add(new Slider() { Margin = new(8, 0, 0, 0), Padding = new(8), Foreground = brushPrimary, TickPlacement = TickPlacement.Both, Value = 5 });
            panel.Children.Add(slider);

            DockPanel progress = new() { Margin = new(0, 8, 0, 0) };
            progress.Children.Add(new RadioButton() { Width = 140, Foreground = brushPrimary, VerticalAlignment = VerticalAlignment.Center, Content = "ProgressBar:" });
            progress.Children.Add(new ProgressBar() { Margin = new(8, 0, 0, 0), Padding = new(8), Foreground = brushPrimary, Value = 75 });
            panel.Children.Add(progress);

            DockPanel group = new() { Margin = new(4) };
            group.Children.Add(new GroupBox() { Margin = new(4), Header = "Calendar", Content = new Calendar() { Margin = new(4), Foreground = brushPrimary } });
            group.Children.Add(new GroupBox() { Margin = new(4), Header = "ListBox", Content = new ListBox() { Margin = new(4), Foreground = brushPrimary, SelectedIndex = 1, ItemsSource = new string[] { "Lorem ipsum", "Dolor sit", "Amet consectetur", "Adipiscing elit", "Quisque faucibus", "Ex sapien", "Vitae pellentesque", "Sem placerat", "In id", "Cursus mi", "Pretium tellus", "Duis convallis" } } });

            StackPanel palette = new() { Margin = new(4), Orientation = Orientation.Vertical };
            palette.Children.Add(new TextBlock() { Padding = new(4), Background = Brushes.LightBlue, Foreground = brushPrimary, HorizontalAlignment = HorizontalAlignment.Left, Text = "Lorem ipsum dolor sit" });
            palette.Children.Add(new TextBlock() { Margin = new(0, 4, 0, 0), Padding = new(4), Background = Brushes.LightSeaGreen, Foreground = brushContrast, HorizontalAlignment = HorizontalAlignment.Center, Text = "Lorem ipsum dolor sit" });
            palette.Children.Add(new TextBlock() { Margin = new(0, 4, 0, 0), Padding = new(4), Background = Brushes.LemonChiffon, Foreground = brushPrimary, HorizontalAlignment = HorizontalAlignment.Right, Text = "Lorem ipsum dolor sit" });
            palette.Children.Add(new TextBlock() { Margin = new(0, 4, 0, 0), Padding = new(4), Background = Brushes.LightCoral, Foreground = brushContrast, Text = "Lorem ipsum dolor sit" });
            palette.Children.Add(new TextBlock() { Margin = new(0, 16, 0, 0), Padding = new(4), Background = brushPrimary, Foreground = brushContrast, FontSize = 8, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Justify, Text = "Lorem ipsum dolor sit amet consectetur adipiscing elit. Quisque faucibus ex sapien vitae pellentesque sem placerat. In id cursus mi pretium tellus duis convallis. Tempus leo eu aenean sed diam urna tempor." });
            group.Children.Add(palette);

            TabControl tabs = new() { Margin = new(0, 16, 0, 0), Height = 240 };
            tabs.Items.Add(new TabItem() { Foreground = brushPrimary, Header = "Lorem", Content = group });
            tabs.Items.Add(new TabItem() { FontStyle = FontStyles.Italic, Foreground = brushPrimary, Header = "Ipsum" });
            tabs.Items.Add(new TabItem() { FontWeight = FontWeights.Bold, Foreground = brushPrimary, Header = "Dolor" });
            tabs.Items.Add(new TabItem() { FontWeight = FontWeights.Bold, FontStyle = FontStyles.Italic, Foreground = brushPrimary, Header = "Sit" });
            panel.Children.Add(tabs);

            return container;
        }

        #endregion

        #region Mock Dataset Test

        private static int parameterMockDataset = 0;

        private static FrameworkElement GenerateContentMockDataset(int index, PrintDocument document, PrintSettings settings)
        {
            if (document.MeasuredSize == Size.Empty)
            {
                return new();
            }

            double sizeRow = 36;
            double sizeFooter = 24;

            Brush brushBorder = Brushes.LightGray;
            Brush brushHeader = Brushes.WhiteSmoke;

            StackPanel container = new()
            {
                Orientation = Orientation.Vertical
            };

            double height = document.MeasuredSize.Height - sizeFooter;
            if (index <= 0)
            {
                parameterMockDataset = 0;

                container.Children.Add(new TextBlock() { FontSize = 24, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, Text = $"Mock Dataset" });
                container.Children.Add(new TextBlock() { Margin = new(0, 8, 0, 32), HorizontalAlignment = HorizontalAlignment.Center, Text = $"ID: #{Guid.NewGuid()}" });

                container.Measure(new(double.PositiveInfinity, double.PositiveInfinity));
                height -= container.DesiredSize.Height;
            }

            int count = (int)Math.Floor(height / sizeRow);
            for (int j = 0; j < count; j++)
            {
                if (j > 0)
                {
                    parameterMockDataset++;
                }

                Grid row = new();
                row.ColumnDefinitions.Add(new() { Width = new(60, GridUnitType.Pixel) });
                row.ColumnDefinitions.Add(new() { Width = new(60, GridUnitType.Pixel) });
                row.ColumnDefinitions.Add(new() { Width = new(2, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new() { Width = new(2, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new() { Width = new(1, GridUnitType.Star) });
                container.Children.Add(new Border() { Height = sizeRow, BorderBrush = brushBorder, BorderThickness = new(0, j <= 0 ? 1 : 0, 0, 1), Child = row });

                for (int k = 0; k < row.ColumnDefinitions.Count; k++)
                {
                    Border cell = new()
                    {
                        Background = j <= 0 ? brushHeader : Brushes.Transparent,
                        BorderBrush = brushBorder,
                        BorderThickness = new(k <= 0 ? 1 : 0, 0, 1, 0),
                        Child = new TextBlock()
                        {
                            Margin = new(8, 0, 8, 0),
                            FontWeight = j <= 0 ? FontWeights.Bold : FontWeights.Normal,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            Foreground = (j <= 0 ? -1 : k) switch
                            {
                                1 => new SolidColorBrush(Color.FromRgb((byte)((1 - (parameterMockDataset - 1) % 26 / 26.0) * 255), (byte)(Math.Sin((parameterMockDataset - 1) % 26 / 26.0 * Math.PI) * 255), (byte)((parameterMockDataset - 1) % 26 / 26.0 * 255))),
                                3 => $"{parameterMockDataset}".GetHashCode() < 0 ? Brushes.Red : Brushes.Green,
                                4 => Brushes.DarkGray,
                                _ => Brushes.Black
                            },
                            VerticalAlignment = VerticalAlignment.Center,
                            Text = $"{(j <= 0 ? (new string[] { "Index", "Label", "Calculation", "Hash", "Random" })[k] : k switch
                            {
                                0 => parameterMockDataset,
                                1 => (char)('A' + (parameterMockDataset - 1) % 26),
                                2 => $"f({parameterMockDataset}) = {Math.Sin(parameterMockDataset * 12.34) * 4321.1234 % 1:0.000000000000}",
                                3 => $"{parameterMockDataset}".GetHashCode(),
                                4 => Random.Shared.Next(),
                                _ => string.Empty
                            })}"
                        }
                    };
                    Grid.SetColumn(cell, k);
                    row.Children.Add(cell);
                }
            }

            container.Children.Add(new Border() { Margin = new(0, height - sizeRow * count, 0, 0), Height = sizeFooter, Child = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom, Text = $"Automatically Generated at {DateTime.Now}." } });

            return container;
        }

        #endregion
    }
}
