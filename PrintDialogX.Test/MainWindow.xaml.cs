using System;
using System.Linq;
using System.Printing;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PrintDialogX.Test
{
    public partial class MainWindow : Window
    {
        private static readonly Dictionary<string, (object? Initial, Func<object?> Callback)> configurations = [];
        private static readonly Dictionary<object, (Func<int, PrintSettings, FrameworkElement> Callback, bool IsDynamic)> templates = new()
        {
            { "Debug Information Test", (GenerateContentDebugInformation, true) },
            { "UI Library Test", (GenerateContentUILibrary, false) },
            { "Mock Dataset Test", (GenerateContentMockDataset, true) },
        };

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
                document.Pages.Add(GeneratePage(i, settings));
            }
        }

        private async Task GenerateDocumentAsync(PrintDocument document, PrintSettings settings)
        {
            for (int i = 0; i < int.Parse(optionCount.Text); i++)
            {
                document.Pages.Add(GeneratePage(i, settings));

                await Dispatcher.Yield();
            }
        }

        private PrintPage GeneratePage(int index, PrintSettings settings)
        {
            return new PrintPage()
            {
                Content = templates[optionTemplate.SelectedItem].Callback(index, settings)
            };
        }

        private void HandlePrintSettingsChanged(object? sender, PrintSettings e)
        {
            if (sender is not PrintDocument document)
            {
                return;
            }

            int index = 0;
            foreach (PrintPage page in document.Pages)
            {
                page.Content = templates[optionTemplate.SelectedItem].Callback(index, e);
                index++;
            }
        }

        private static FrameworkElement GenerateContentDebugInformation(int index, PrintSettings settings)
        {
            return new();
        }

        private static FrameworkElement GenerateContentUILibrary(int index, PrintSettings settings)
        {
            return new();
        }

        private static FrameworkElement GenerateContentMockDataset(int index, PrintSettings settings)
        {
            return new();
        }
    }
}
