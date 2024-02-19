using System;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace PrintDialogX.PrintControl
{
    partial class PrintPage : Page
    {
        //Properties

        #region Public properties

        /// <summary>
        /// A boolean value, where true means that the Print button is clicked, false means that the Cancel button is clicked.
        /// </summary>
        public bool ReturnValue { get; internal set; } = false;

        /// <summary>
        /// The total number of papers that the printer will use.
        /// </summary>
        public int TotalPapers { get; internal set; } = 0;

        #endregion

        #region Private properties

        private bool isLoaded;
        private bool isRefreshed;
        private Package package;
        private FixedDocument fixedDocument;

        private readonly Uri _xpsUrl;
        private readonly int _pageCount;
        private readonly int[] _zoomList;
        private readonly double _pageMargin;
        private readonly string _documentName;
        private readonly Size _originalPageSize;
        private readonly bool _allowPagesOption;
        private readonly bool _allowScaleOption;
        private readonly bool _allowDoubleSidedOption;
        private readonly bool _allowPageOrderOption;
        private readonly bool _allowPagesPerSheetOption;
        private readonly bool _allowMoreSettingsExpander;
        private readonly bool _allowAddNewPrinerComboBoxItem;
        private readonly bool _allowPrinterPreferencesButton;
        private readonly PrintServer _localDefaultPrintServer;
        private readonly List<PageContent> _originalPagesContentList;
        private readonly PrintDialog.PrintDialogSettings _defaultSettings;
        private readonly System.Windows.Controls.PrintDialog _systemPrintDialog;
        private readonly Func<PrintDialog.DocumentInfo, List<PageContent>> _reloadDocumentCallback;

        private static readonly DispatcherOperationCallback _exitFrameCallback = new DispatcherOperationCallback(ExitFrame);

        #endregion

        //Dll Import

        #region Printer Preferences Dialog

        private void OpenPrinterPropertiesDialog(System.Drawing.Printing.PrinterSettings printerSettings)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/C rundll32 printui.dll,PrintUIEntry /p /n \"" + printerSettings.PrinterName + "\"",
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        }

        #endregion

        //Initialize

        /// <summary>
        /// Initialize PrintDialog.
        /// </summary>
        /// <param name="document">The document that needs to be printed.</param>
        /// <param name="documentName">The document name that will be displayed.</param>
        /// <param name="pageMargin">The page margin info.</param>
        /// <param name="defaultSettings">The default settings.</param>
        /// <param name="allowPagesOption">Allow pages option or not.</param>
        /// <param name="allowScaleOption">Allow scale option or not.</param>
        /// <param name="allowDoubleSidedOption">Allow double-sided option or not.</param>
        /// <param name="allowPagesPerSheetOption">Allow pages per sheet option or not.</param>
        /// <param name="allowPageOrderOption">Allow page order option or not. Only works when pages per sheet option is allowed.</param>
        /// <param name="allowMoreSettingsExpander">Allow the usage of an expander for more settings or just show all settings at once.</param>
        /// <param name="allowAddNewPrinerComboBoxItem">Allow add new printer button in the printer list or not.</param>
        /// <param name="allowPrinterPreferencesButton">Allow printer preferences button or not.</param>
        /// <param name="getDocumentWhenReloadDocumentMethod">The method that will use to get document when reload document. You can only change the content in the document. The method must return a list of <see cref="PageContent"/> that represents the page contents in order.</param>
        public PrintPage(FixedDocument document, string documentName, double pageMargin, PrintDialog.PrintDialogSettings defaultSettings, bool allowPagesOption, bool allowScaleOption, bool allowDoubleSidedOption, bool allowPagesPerSheetOption, bool allowPageOrderOption, bool allowMoreSettingsExpander, bool allowAddNewPrinerComboBoxItem, bool allowPrinterPreferencesButton, Func<PrintDialog.DocumentInfo, List<PageContent>> getDocumentWhenReloadDocumentMethod)
        {
            isLoaded = false;

            InitializeComponent();

            this.UpdateLayout();
            DoEvents();

            isRefreshed = true;
            fixedDocument = document;

            _pageMargin = pageMargin;
            _documentName = documentName;
            _defaultSettings = defaultSettings;
            _allowPagesOption = allowPagesOption;
            _allowScaleOption = allowScaleOption;
            _allowDoubleSidedOption = allowDoubleSidedOption;
            _allowPageOrderOption = allowPageOrderOption;
            _allowPagesPerSheetOption = allowPagesPerSheetOption;
            _allowMoreSettingsExpander = allowMoreSettingsExpander;
            _allowAddNewPrinerComboBoxItem = allowAddNewPrinerComboBoxItem;
            _allowPrinterPreferencesButton = allowPrinterPreferencesButton;
            _reloadDocumentCallback = getDocumentWhenReloadDocumentMethod;

            _pageCount = document.Pages.Count;
            _originalPageSize = document.DocumentPaginator.PageSize;

            _xpsUrl = new Uri("memorystream://" + Guid.NewGuid().ToString() + ".xps");
            _zoomList = new int[] { 25, 50, 75, 100, 150, 200 };
            _localDefaultPrintServer = new PrintServer();
            _originalPagesContentList = new List<PageContent>();
            _systemPrintDialog = new System.Windows.Controls.PrintDialog();

            foreach (PageContent page in document.Pages)
            {
                PageContent pageClone = XamlReader.Parse(XamlWriter.Save(page)) as PageContent;
                _originalPagesContentList.Add(pageClone);
            }
        }

        //Methods

        internal static void DoEvents()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, _exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static object ExitFrame(object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;
            frame.Continue = false;
            return null;
        }

        public static Wpf.Ui.Controls.ContentDialog CreateErrorDialog(string content)
        {
            Wpf.Ui.Controls.ContentDialog dialog = new Wpf.Ui.Controls.ContentDialog()
            {
                Title = "Error",
                Content = content,
                DialogMaxHeight = 220,
                CloseButtonText = "Ok",
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false,
                DefaultButton = Wpf.Ui.Controls.ContentDialogButton.Primary,
            };
            Grid.SetColumnSpan(dialog, 2);
            return dialog;
        }

        internal static List<UIElement> GetChildren(DependencyObject element)
        {
            List<UIElement> elements = new List<UIElement>();

            if (VisualTreeHelper.GetChildrenCount(element) > 0)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    DependencyObject control = VisualTreeHelper.GetChild(element, i);
                    if (control != null && control is UIElement)
                    {
                        elements.Add((UIElement)control);
                        elements.AddRange(GetChildren(control));
                    }
                }
            }

            return elements;
        }

        private int GetCurrentPageIndex()
        {
            ScrollViewer scrollViewer = (ScrollViewer)documentPreviewer.Template.FindName("PART_ContentHost", documentPreviewer);

            int page;
            if (documentPreviewer.MaxPagesAcross == 1)
            {
                page = (int)(scrollViewer.VerticalOffset / (scrollViewer.ExtentHeight / documentPreviewer.PageCount)) + 1;
            }
            else
            {
                page = ((int)(scrollViewer.VerticalOffset / (scrollViewer.ExtentHeight / Math.Ceiling((double)documentPreviewer.PageCount / 2))) + 1) * 2 - 1;
            }

            return Math.Max(1, Math.Min(page, documentPreviewer.PageCount));
        }

        private ComboBoxItem GeneratePrinterComboBoxItem(PrintQueue printer = null)
        {
            string status = "";
            string location = "";
            string comment = "";

            if (printer != null)
            {
                printer.Refresh();

                status = PrinterHelper.PrinterHelper.GetPrinterStatusInfo(_localDefaultPrintServer.GetPrintQueue(printer.FullName));
                location = printer.Location;
                comment = printer.Comment;

                location = string.IsNullOrWhiteSpace(location) == true ? "Unknown" : location;
                comment = string.IsNullOrWhiteSpace(comment) == true ? "Unknown" : comment;
            }

            Grid itemMainGrid = new Grid();
            itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(55) });
            itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10) });
            itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            Image printerIcon = new Image()
            {
                Width = 55,
                Height = 55,
                Source = printer == null ? new System.Windows.Media.Imaging.BitmapImage(new Uri("/PrintDialog;component/Resources/AddPrinter.png", UriKind.Relative)) : PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(printer, _localDefaultPrintServer),
                Stretch = Stretch.Fill
            };

            StackPanel textInfoPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center
            };
            textInfoPanel.Children.Add(new TextBlock()
            {
                Text = printer == null ? "Add New Printer" : printer.FullName,
                FontSize = 14
            });
            if (printer != null)
            {
                textInfoPanel.Children.Add(new TextBlock()
                {
                    Text = status,
                    Margin = new Thickness(0, 7, 0, 0)
                });

                if (printer.IsOffline == true)
                {
                    printerIcon.Opacity = 0.5;
                }
            }

            printerIcon.SetValue(Grid.ColumnProperty, 0);
            textInfoPanel.SetValue(Grid.ColumnProperty, 2);

            itemMainGrid.Children.Add(printerIcon);
            itemMainGrid.Children.Add(textInfoPanel);

            return new ComboBoxItem()
            {
                Content = itemMainGrid,
                Padding = new Thickness(10),
                ToolTip = printer == null ? "Add New Printer" : "Name: " + printer.FullName + "\nStatus: " + status + "\nDocument: " + printer.NumberOfJobs + "\nLoction: " + location + "\nComment: " + comment,
                Tag = printer?.FullName
            };
        }

        private async void LoadPrinters(string selectedPrinter = "")
        {
            try
            {
                int selectedIndex = 0;
                printerComboBox.Items.Clear();

                if (_allowAddNewPrinerComboBoxItem == true)
                {
                    printerComboBox.Items.Insert(0, GeneratePrinterComboBoxItem(null));
                }

                foreach (PrintQueue printer in _localDefaultPrintServer.GetPrintQueues())
                {
                    printerComboBox.Items.Insert(0, GeneratePrinterComboBoxItem(printer));

                    if (printer.FullName == selectedPrinter || (string.IsNullOrWhiteSpace(selectedPrinter) && LocalPrintServer.GetDefaultPrintQueue().FullName == printer.FullName))
                    {
                        selectedIndex = printerComboBox.Items.Count;
                    }
                }

                if (printerComboBox.Items.Count == 0)
                {
                    Wpf.Ui.Controls.ContentDialog dialog = CreateErrorDialog("No printer is detected!");
                    dialog.ButtonClicked += (s, arg) =>
                    {
                        dialog.Hide();
                        contentHolder.Children.Remove(dialog);

                        Window.GetWindow(this).Close();
                    };
                    contentHolder.Children.Add(dialog);
                    await dialog.ShowAsync();
                }

                printerComboBox.SelectedIndex = printerComboBox.Items.Count - selectedIndex;
            }
            catch
            {
                return;
            }
        }

        private void LoadPrinterSettings(bool useDefaults = true)
        {
            isRefreshed = false;

            string originalColor = "";
            string originalQuality = "";
            string originalSize = "";
            string originalType = "";
            string originalSource = "";

            if (useDefaults == false)
            {
                originalColor = colorComboBox.SelectedItem == null ? _defaultSettings.Color.ToString() : (colorComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                originalQuality = qualityComboBox.SelectedItem == null ? _defaultSettings.Quality.ToString() : (qualityComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                originalSize = sizeComboBox.SelectedItem == null ? SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(_defaultSettings.PageSize) : (sizeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                originalType = typeComboBox.SelectedItem == null ? SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(_defaultSettings.PageType) : (typeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                originalSource = sourceComboBox.SelectedItem == null ? SettingsHepler.NameInfoHepler.GetInputBinNameInfo(InputBin.AutoSelect) : (sourceComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            }

            PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            copiesNumberPicker.Maximum = printer.GetPrintCapabilities().MaxCopyCount ?? 1;
            collateCheckBox.Visibility = copiesNumberPicker.Value > 1 ? Visibility.Visible : Visibility.Collapsed;

            int colorComboBoxSelectedIndex = 0;
            colorComboBox.Items.Clear();
            foreach (OutputColor color in printer.GetPrintCapabilities().OutputColorCapability)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = color.ToString()
                };
                colorComboBox.Items.Add(item);

                if (useDefaults == true)
                {
                    if (_defaultSettings.UsePrinterDefaultSettings == false)
                    {
                        if (color.ToString() == _defaultSettings.Color.ToString())
                        {
                            colorComboBoxSelectedIndex = colorComboBox.Items.Count - 1;
                        }
                    }
                    else
                    {
                        if (color == printer.DefaultPrintTicket.OutputColor)
                        {
                            colorComboBoxSelectedIndex = colorComboBox.Items.Count - 1;
                        }
                    }
                }
                else
                {
                    if (color.ToString() == originalColor)
                    {
                        colorComboBoxSelectedIndex = colorComboBox.Items.Count - 1;
                    }
                }
            }
            if (colorComboBox.HasItems == false)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = _defaultSettings.Color.ToString()
                };
                colorComboBox.Items.Add(item);
            }
            colorComboBox.SelectedIndex = colorComboBoxSelectedIndex;

            int qualityComboBoxSelectedIndex = 0;
            qualityComboBox.Items.Clear();
            foreach (OutputQuality quality in printer.GetPrintCapabilities().OutputQualityCapability)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = quality.ToString()
                };
                qualityComboBox.Items.Add(item);

                if (useDefaults == true)
                {
                    if (_defaultSettings.UsePrinterDefaultSettings == false)
                    {
                        if (quality.ToString() == _defaultSettings.Quality.ToString())
                        {
                            qualityComboBoxSelectedIndex = qualityComboBox.Items.Count - 1;
                        }
                    }
                    else
                    {
                        if (quality == printer.DefaultPrintTicket.OutputQuality)
                        {
                            qualityComboBoxSelectedIndex = qualityComboBox.Items.Count - 1;
                        }
                    }
                }
                else
                {
                    if (quality.ToString() == originalQuality)
                    {
                        qualityComboBoxSelectedIndex = qualityComboBox.Items.Count - 1;
                    }
                }
            }
            if (qualityComboBox.HasItems == false)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = _defaultSettings.Quality.ToString()
                };
                qualityComboBox.Items.Add(item);
            }
            qualityComboBox.SelectedIndex = qualityComboBoxSelectedIndex;

            if (_defaultSettings.UsePrinterDefaultSettings == true)
            {
                if (printer.DefaultPrintTicket.Duplexing.HasValue)
                {
                    if (printer.DefaultPrintTicket.Duplexing.Value == Duplexing.TwoSidedLongEdge)
                    {
                        doubleSidedCheckBox.IsChecked = true;
                        doubleSidedTypeComboBox.SelectedIndex = 0;
                    }
                    else if (printer.DefaultPrintTicket.Duplexing.Value == Duplexing.TwoSidedShortEdge)
                    {
                        doubleSidedCheckBox.IsChecked = true;
                        doubleSidedTypeComboBox.SelectedIndex = 1;
                    }
                    else
                    {
                        doubleSidedCheckBox.IsChecked = false;
                    }
                }
                else
                {
                    doubleSidedCheckBox.IsChecked = false;
                }
            }

            int sizeComboBoxSelectedIndex = 0;
            sizeComboBox.Items.Clear();
            foreach (PageMediaSize size in printer.GetPrintCapabilities().PageMediaSizeCapability)
            {
                if (size == null || size.PageMediaSizeName == null)
                {
                    continue;
                }

                ComboBoxItem item = new ComboBoxItem
                {
                    Content = SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(size.PageMediaSizeName.Value)
                };
                sizeComboBox.Items.Add(item);

                if (useDefaults == true)
                {
                    if (_defaultSettings.UsePrinterDefaultSettings == false)
                    {
                        if (size.PageMediaSizeName.ToString() == _defaultSettings.PageSize.ToString())
                        {
                            sizeComboBoxSelectedIndex = sizeComboBox.Items.Count - 1;
                        }
                    }
                    else
                    {
                        if (size == printer.DefaultPrintTicket.PageMediaSize)
                        {
                            sizeComboBoxSelectedIndex = sizeComboBox.Items.Count - 1;
                        }
                    }
                }
                else
                {
                    if (SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(size.PageMediaSizeName.Value) == originalSize)
                    {
                        sizeComboBoxSelectedIndex = sizeComboBox.Items.Count - 1;
                    }
                }
            }
            if (sizeComboBox.HasItems == false)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(_defaultSettings.PageSize)
                };
                sizeComboBox.Items.Add(item);
            }
            sizeComboBox.SelectedIndex = sizeComboBoxSelectedIndex;

            int typeComboBoxSelectedIndex = 0;
            typeComboBox.Items.Clear();
            foreach (PageMediaType type in printer.GetPrintCapabilities().PageMediaTypeCapability)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(type)
                };
                typeComboBox.Items.Add(item);

                if (useDefaults == true)
                {
                    if (_defaultSettings.UsePrinterDefaultSettings == false)
                    {
                        if (type.ToString() == _defaultSettings.PageType.ToString())
                        {
                            typeComboBoxSelectedIndex = typeComboBox.Items.Count - 1;
                        }
                    }
                    else
                    {
                        if (type == printer.DefaultPrintTicket.PageMediaType)
                        {
                            typeComboBoxSelectedIndex = typeComboBox.Items.Count - 1;
                        }
                    }
                }
                else
                {
                    if (SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(type) == originalType)
                    {
                        typeComboBoxSelectedIndex = typeComboBox.Items.Count - 1;
                    }
                }
            }
            if (typeComboBox.HasItems == false)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(_defaultSettings.PageType)
                };
                typeComboBox.Items.Add(item);
            }
            typeComboBox.SelectedIndex = typeComboBoxSelectedIndex;

            int sourceComboBoxSelectedIndex = 0;
            sourceComboBox.Items.Clear();
            foreach (InputBin source in printer.GetPrintCapabilities().InputBinCapability)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = SettingsHepler.NameInfoHepler.GetInputBinNameInfo(source)
                };
                sourceComboBox.Items.Add(item);

                if (useDefaults == true)
                {
                    if (source == printer.DefaultPrintTicket.InputBin)
                    {
                        sourceComboBoxSelectedIndex = sourceComboBox.Items.Count - 1;
                    }
                }
                else
                {
                    if (SettingsHepler.NameInfoHepler.GetInputBinNameInfo(source) == originalSource)
                    {
                        sourceComboBoxSelectedIndex = sourceComboBox.Items.Count - 1;
                    }
                }
            }
            if (sourceComboBox.HasItems == false)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = SettingsHepler.NameInfoHepler.GetInputBinNameInfo(InputBin.AutoSelect)
                };
                sourceComboBox.Items.Add(item);
            }
            sourceComboBox.SelectedIndex = sourceComboBoxSelectedIndex;

            isRefreshed = true;

            ReloadDocument();
        }

        private async void ReloadDocument()
        {
            if (isRefreshed == true)
            {
                loadingGrid.Visibility = Visibility.Visible;
                DoEvents();

                try
                {
                    PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

                    PageMediaSize size;
                    if (printer.GetPrintCapabilities().PageMediaSizeCapability.Count > 0)
                    {
                        size = printer.GetPrintCapabilities().PageMediaSizeCapability[sizeComboBox.SelectedIndex];
                    }
                    else
                    {
                        PageMediaSizeName sizeName = PageMediaSizeName.ISOA4;

                        int index = 0;
                        foreach (string name in sizeName.GetType().GetEnumNames())
                        {
                            if (name == _defaultSettings.PageSize.ToString())
                            {
                                sizeName = (PageMediaSizeName)index;
                                break;
                            }
                            index++;
                        }
                        size = new PageMediaSize(sizeName);
                    }

                    FixedDocument doc = new FixedDocument();
                    doc.DocumentPaginator.PageSize = orientationComboBox.SelectedIndex == 0 ? new Size(size.Width.Value, size.Height.Value) : new Size(size.Height.Value, size.Width.Value);

                    List<int> pageList = new List<int>();
                    bool isCustomPages = false;
                    if (pagesComboBox.SelectedIndex == 1)
                    {
                        isCustomPages = true;
                        pageList.Add(GetCurrentPageIndex());
                    }
                    else if (pagesComboBox.SelectedIndex == 2 && string.IsNullOrWhiteSpace(customPagesTextBox.Text) == false)
                    {
                        isCustomPages = true;

                        string[] customPageInputList = customPagesTextBox.Text.Split(',');
                        foreach (string str in customPageInputList)
                        {
                            if (str.Contains("-"))
                            {
                                try
                                {
                                    string[] pageRange = str.Split('-');

                                    int number1 = 0;
                                    int number2 = 0;
                                    bool valid = pageRange.Length == 2;
                                    if (valid == true)
                                    {
                                        valid = int.TryParse(pageRange[0], out number1) && int.TryParse(pageRange[1], out number2);
                                    }
                                    if (valid == false || number1 > number2 || number1 <= 0 || number2 > _pageCount)
                                    {
                                        Wpf.Ui.Controls.ContentDialog dialog = CreateErrorDialog("The value for custom pages is invalid!");
                                        dialog.ButtonClicked += (s, arg) =>
                                        {
                                            dialog.Hide();
                                            contentHolder.Children.Remove(dialog);
                                        };
                                        contentHolder.Children.Add(dialog);
                                        await dialog.ShowAsync();

                                        pageList = new List<int>();
                                        isCustomPages = false;
                                        break;
                                    }
                                    else
                                    {
                                        for (int i = number1; i <= number2; i++)
                                        {
                                            pageList.Add(i);
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                try
                                {
                                    bool valid = int.TryParse(str, out int number);
                                    if (valid == false || number <= 0 || number > _pageCount)
                                    {
                                        Wpf.Ui.Controls.ContentDialog dialog = CreateErrorDialog("The value for custom pages is invalid!");
                                        dialog.ButtonClicked += (s, arg) =>
                                        {
                                            dialog.Hide();
                                            contentHolder.Children.Remove(dialog);
                                        };
                                        contentHolder.Children.Add(dialog);
                                        await dialog.ShowAsync();

                                        pageList = new List<int>();
                                        isCustomPages = false;
                                        break;
                                    }
                                    else
                                    {
                                        pageList.Add(int.Parse(str));
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    if (pageList.Count == 0)
                    {
                        for (int i = 1; i <= _pageCount; i++)
                        {
                            pageList.Add(i);
                        }
                    }

                    double scale;
                    double margin;
                    PrintSettings.PageColor color;

                    if (colorComboBox.SelectedItem != null && (colorComboBox.SelectedItem as ComboBoxItem).Content.ToString() == PrintSettings.PageColor.Grayscale.ToString())
                    {
                        ScrollViewer scrollViewer = (ScrollViewer)documentPreviewer.Template.FindName("PART_ContentHost", documentPreviewer);
                        scrollViewer.Effect = new PreviewHelper.GrayscaleEffect();

                        color = PrintSettings.PageColor.Grayscale;
                    }
                    else if (colorComboBox.SelectedItem != null && (colorComboBox.SelectedItem as ComboBoxItem).Content.ToString() == PrintSettings.PageColor.Monochrome.ToString())
                    {
                        ScrollViewer scrollViewer = (ScrollViewer)documentPreviewer.Template.FindName("PART_ContentHost", documentPreviewer);
                        scrollViewer.Effect = new PreviewHelper.GrayscaleEffect();

                        color = PrintSettings.PageColor.Monochrome;
                    }
                    else
                    {
                        ScrollViewer scrollViewer = (ScrollViewer)documentPreviewer.Template.FindName("PART_ContentHost", documentPreviewer);
                        scrollViewer.Effect = null;

                        color = PrintSettings.PageColor.Color;
                    }

                    if (marginComboBox.SelectedIndex == 0)
                    {
                        margin = _pageMargin;
                    }
                    else if (marginComboBox.SelectedIndex == 1)
                    {
                        margin = 0;
                    }
                    else if (marginComboBox.SelectedIndex == 2)
                    {
                        try
                        {
                            PageImageableArea imageableArea = printer.GetPrintCapabilities(new PrintTicket() { PageMediaSize = size }).PageImageableArea;
                            margin = Math.Max(imageableArea.OriginHeight, imageableArea.OriginWidth);
                        }
                        catch
                        {
                            margin = 0;
                        }
                    }
                    else
                    {
                        margin = (int)customMarginNumberPicker.Value;
                    }

                    if (scaleComboBox.SelectedIndex == 0)
                    {
                        scale = double.NaN;
                    }
                    else if (scaleComboBox.SelectedIndex == 7)
                    {
                        scale = (int)customZoomNumberPicker.Value;
                    }
                    else
                    {
                        scale = _zoomList[scaleComboBox.SelectedIndex - 1];
                    }

                    List<PageContent> pageContentList;
                    if (_reloadDocumentCallback != null)
                    {
                        pageContentList = _reloadDocumentCallback(new PrintDialog.DocumentInfo()
                        {
                            Color = color,
                            Margin = margin,
                            Orientation = (PrintSettings.PageOrientation)orientationComboBox.SelectedIndex,
                            PageOrder = (PrintSettings.PageOrder)pageOrderComboBox.SelectedIndex,
                            Pages = pageList.ToArray(),
                            PagesPerSheet = int.Parse((pagesPerSheetComboBox.SelectedItem as ComboBoxItem).Content.ToString()),
                            Scale = scale,
                            Size = new Size(size.Width.Value, size.Height.Value),
                        });
                    }
                    else
                    {
                        pageContentList = _originalPagesContentList;
                    }

                    int pageCount = 1;

                    foreach (PageContent originalPage in pageContentList)
                    {
                        if ((isCustomPages == true && pageList.Count >= 1 && pageList.Contains(pageCount) == true) || isCustomPages == false)
                        {
                            List<ImageSource> imageSources = new List<ImageSource>();
                            foreach (UIElement element in GetChildren(originalPage.Child))
                            {
                                if (element is Image)
                                {
                                    imageSources.Add(((Image)element).Source);
                                }
                            }

                            FixedPage fixedPage = XamlReader.Parse(XamlWriter.Save(originalPage.Child)) as FixedPage;

                            foreach (UIElement element in GetChildren(fixedPage))
                            {
                                if (element is Image)
                                {
                                    ((Image)element).Source = imageSources[0];
                                    imageSources.RemoveAt(0);
                                }
                            }

                            fixedPage.Width = doc.DocumentPaginator.PageSize.Width;
                            fixedPage.Height = doc.DocumentPaginator.PageSize.Height;
                            fixedPage.RenderTransformOrigin = new Point(0, 0);

                            if (pagesPerSheetComboBox.SelectedIndex == 0)
                            {
                                if (scaleComboBox.SelectedIndex == 0)
                                {
                                    if (_originalPageSize.Height * (fixedPage.Width / _originalPageSize.Width) <= fixedPage.Height)
                                    {
                                        fixedPage.RenderTransform = new ScaleTransform(fixedPage.Width / _originalPageSize.Width, fixedPage.Width / _originalPageSize.Width);
                                    }
                                    else
                                    {
                                        fixedPage.RenderTransform = new ScaleTransform(fixedPage.Height / _originalPageSize.Height, fixedPage.Height / _originalPageSize.Height);
                                    }
                                }
                                else
                                {
                                    fixedPage.RenderTransform = new ScaleTransform(scale / 100.0, scale / 100.0);
                                }

                                double finalMargin = 0 - _pageMargin + margin;
                                foreach (UIElement element in GetChildren(fixedPage))
                                {
                                    FixedPage.SetLeft(element, FixedPage.GetLeft(element) + finalMargin);
                                    FixedPage.SetTop(element, FixedPage.GetTop(element) + finalMargin);
                                }
                            }

                            doc.Pages.Add(new PageContent { Child = fixedPage });

                            fixedPage.UpdateLayout();
                            DoEvents();
                        }

                        pageCount++;
                    }

                    if (pagesPerSheetComboBox.SelectedIndex != 0)
                    {
                        PreviewHelper.MultiPagesPerSheetHelper multiPagesPerSheetHelper = new PreviewHelper.MultiPagesPerSheetHelper(int.Parse((pagesPerSheetComboBox.SelectedItem as ComboBoxItem).Content.ToString()), doc, _originalPageSize, (PreviewHelper.DocumentOrientation)orientationComboBox.SelectedIndex, (PreviewHelper.PageOrder)pageOrderComboBox.SelectedIndex);
                        fixedDocument = multiPagesPerSheetHelper.GetMultiPagesPerSheetDocument(scale);
                    }
                    else
                    {
                        fixedDocument = doc;
                    }

                    PackageStore.RemovePackage(_xpsUrl);
                    MemoryStream stream = new MemoryStream();
                    package = Package.Open(stream, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                    PackageStore.AddPackage(_xpsUrl, package);
                    XpsDocument xpsDoc = new XpsDocument(package);
                    try
                    {
                        xpsDoc.Uri = _xpsUrl;
                        XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                        writer.Write(((IDocumentPaginatorSource)fixedDocument).DocumentPaginator);

                        documentPreviewer.Document = xpsDoc.GetFixedDocumentSequence();
                    }
                    finally
                    {
                        xpsDoc?.Close();
                    }
                }
                catch
                {

                }
                finally
                {
                    loadingGrid.Visibility = Visibility.Collapsed;
                    DoEvents();
                }
            }
        }

        private void PrintDocument()
        {
            if (printerComboBox.SelectedItem == null)
            {
                return;
            }

            ReloadDocument();

            PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            PrintTicket printTicket = _systemPrintDialog.PrintTicket;
            printTicket.CopyCount = (int)copiesNumberPicker.Value;
            if (collateCheckBox.IsChecked == true)
            {
                printTicket.Collation = Collation.Collated;
            }
            else
            {
                printTicket.Collation = Collation.Uncollated;
            }
            if (orientationComboBox.SelectedIndex == 0)
            {
                printTicket.PageOrientation = PageOrientation.Portrait;
            }
            else
            {
                printTicket.PageOrientation = PageOrientation.Landscape;
            }
            if (printer.GetPrintCapabilities().PageMediaSizeCapability.Count > 0)
            {
                printTicket.PageMediaSize = printer.GetPrintCapabilities().PageMediaSizeCapability[sizeComboBox.SelectedIndex];
            }
            if (printer.GetPrintCapabilities().PageMediaTypeCapability.Count > 0)
            {
                printTicket.PageMediaType = printer.GetPrintCapabilities().PageMediaTypeCapability[typeComboBox.SelectedIndex];
            }
            if (printer.GetPrintCapabilities().OutputColorCapability.Count > 0)
            {
                printTicket.OutputColor = printer.GetPrintCapabilities().OutputColorCapability[colorComboBox.SelectedIndex];
            }
            if (printer.GetPrintCapabilities().OutputQualityCapability.Count > 0)
            {
                printTicket.OutputQuality = printer.GetPrintCapabilities().OutputQualityCapability[qualityComboBox.SelectedIndex];
            }
            if (printer.GetPrintCapabilities().InputBinCapability.Count > 0)
            {
                printTicket.InputBin = printer.GetPrintCapabilities().InputBinCapability[sourceComboBox.SelectedIndex];
            }
            if (doubleSidedCheckBox.IsChecked == true)
            {
                printTicket.Duplexing = doubleSidedTypeComboBox.SelectedIndex == 0 ? Duplexing.TwoSidedLongEdge : Duplexing.TwoSidedShortEdge;
            }
            else
            {
                printTicket.Duplexing = Duplexing.OneSided;
            }
            printTicket.PageScalingFactor = 100;
            printTicket.PagesPerSheet = 1;
            printTicket.PagesPerSheetDirection = PagesPerSheetDirection.RightBottom;

            if (doubleSidedCheckBox.IsChecked == false)
            {
                TotalPapers = documentPreviewer.PageCount * (int)copiesNumberPicker.Value;
            }
            else
            {
                TotalPapers = (int)Math.Ceiling(documentPreviewer.PageCount * (int)copiesNumberPicker.Value / 2.0);
            }

            _systemPrintDialog.PrintQueue = printer;
            _systemPrintDialog.PrintDocument(fixedDocument.DocumentPaginator, _documentName);
        }

        //Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoEvents();

            if (_allowPagesOption == false)
            {
                pagesTextBlock.Visibility = Visibility.Collapsed;
                pagesComboBox.Visibility = Visibility.Collapsed;
            }

            if (_allowScaleOption == false)
            {
                scaleTextBlock.Visibility = Visibility.Collapsed;
                scaleComboBox.Visibility = Visibility.Collapsed;
                scaleComboBox.SelectedIndex = 4;
            }

            if (_allowDoubleSidedOption == false)
            {
                doubleSidedTextBlock.Visibility = Visibility.Collapsed;
                doubleSidedCheckBox.Visibility = Visibility.Collapsed;
            }

            if (_allowPagesPerSheetOption == false)
            {
                pagesPerSheetTextBlock.Visibility = Visibility.Collapsed;
                pagesPerSheetComboBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (_allowPageOrderOption == false)
                {
                    pageOrderTextBlock.Visibility = Visibility.Collapsed;
                    pageOrderComboBox.Visibility = Visibility.Collapsed;
                }
            }

            if (_allowMoreSettingsExpander == false)
            {
                moreSettingsExpander.MinHeight = 0;
                moreSettingsExpander.IsExpanded = true;
            }

            if (_allowPrinterPreferencesButton == false)
            {
                printerPreferencesBtn.Visibility = Visibility.Collapsed;
            }

            this.UpdateLayout();
            DoEvents();

            LoadPrinters();
            printerPreviewIcon.Source = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(_localDefaultPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localDefaultPrintServer, true);
            printerPreviewText.Text = (printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            LoadPrinterSettings();

            PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            orientationComboBox.SelectedIndex = _defaultSettings.Layout == PrintSettings.PageOrientation.Portrait ? 0 : 1;

            if (_allowDoubleSidedOption == true)
            {
                if (_defaultSettings.UsePrinterDefaultSettings == false)
                {
                    if (_defaultSettings.DoubleSided == PrintSettings.DoubleSided.OneSided)
                    {
                        doubleSidedCheckBox.IsChecked = false;
                    }
                    else
                    {
                        doubleSidedCheckBox.IsChecked = true;
                        doubleSidedTypeComboBox.SelectedIndex = _defaultSettings.DoubleSided == PrintSettings.DoubleSided.DoubleSidedLongEdge ? 0 : 1;
                    }
                }
                else
                {
                    if (printer.DefaultPrintTicket.Duplexing.HasValue)
                    {
                        if (printer.DefaultPrintTicket.Duplexing.Value == Duplexing.TwoSidedLongEdge)
                        {
                            doubleSidedCheckBox.IsChecked = true;
                            doubleSidedTypeComboBox.SelectedIndex = 0;
                        }
                        else if (printer.DefaultPrintTicket.Duplexing.Value == Duplexing.TwoSidedShortEdge)
                        {
                            doubleSidedCheckBox.IsChecked = true;
                            doubleSidedTypeComboBox.SelectedIndex = 1;
                        }
                        else
                        {
                            doubleSidedCheckBox.IsChecked = false;
                        }
                    }
                    else
                    {
                        doubleSidedCheckBox.IsChecked = false;
                    }
                }
            }

            pagesPerSheetComboBox.SelectedIndex = PreviewHelper.MultiPagesPerSheetHelper.GetPagePerSheetCountIndex(_defaultSettings.PagesPerSheet);
            pageOrderComboBox.SelectedIndex = (int)_defaultSettings.PageOrder;

            customMarginNumberPicker.Maximum = (int)Math.Min(_originalPageSize.Width / 2, _originalPageSize.Height / 2) - 15;
            customMarginNumberPicker.Value = _pageMargin;

            DoEvents();

            ReloadDocument();
            documentPreviewer.FitToWidth();

            loadingGrid.Visibility = Visibility.Collapsed;
            DoEvents();

            documentPreviewer.FitToWidth();
            DoEvents();

            isLoaded = true;

            ((TextBlock)documentPreviewer.Template.FindName("currentPageTextBlock", documentPreviewer)).Text = "Page " + GetCurrentPageIndex().ToString() + " / " + documentPreviewer.PageCount.ToString();
            printButton.Focus();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            PackageStore.RemovePackage(_xpsUrl);
            package?.Close();
            _localDefaultPrintServer.Dispose();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.IsEnabled = false;
            printButton.IsEnabled = false;
            DoEvents();

            PrintDocument();

            ReturnValue = true;
            Window.GetWindow(this).Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.IsEnabled = false;
            printButton.IsEnabled = false;
            DoEvents();

            ReturnValue = false;
            Window.GetWindow(this).Close();
        }

        private void PrinterPreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Drawing.Printing.PrinterSettings settings = new System.Drawing.Printing.PrinterSettings
                {
                    PrinterName = (printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString()
                };
                OpenPrinterPropertiesDialog(settings);
            }
            catch
            {
                return;
            }
        }

        private void PrinterComboBox_DropDownOpened(object sender, EventArgs e)
        {
            isRefreshed = false;

            _localDefaultPrintServer.Refresh();
            LoadPrinters((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            isRefreshed = true;
        }

        private async void PrinterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (printerComboBox.SelectedItem == null)
            {
                return;
            }

            if (isLoaded == true && isRefreshed == true)
            {
                if (_allowAddNewPrinerComboBoxItem == true && (printerComboBox.SelectedItem as ComboBoxItem).Tag == null)
                {
                    printerComboBox.IsDropDownOpen = false;

                    if (e.RemovedItems.Count > 0)
                    {
                        printerComboBox.SelectedItem = e.RemovedItems[0];
                    }

                    Windows.System.LauncherOptions option = new Windows.System.LauncherOptions()
                    {
                        TreatAsUntrusted = false
                    };
                    if (await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:printers"), option) == false)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", "shell:::{A8A91A66-3A7D-4424-8D24-04E180695C7A}");
                    }
                }
                else
                {
                    printerPreviewIcon.Source = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(_localDefaultPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localDefaultPrintServer, true);
                    printerPreviewText.Text = (printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString();

                    LoadPrinterSettings(false);
                }
            }
        }

        private void SettingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == true)
            {
                if (sender is ComboBox comboBox)
                {
                    comboBox.IsDropDownOpen = false;
                }

                customZoomNumberPicker.Visibility = scaleComboBox.SelectedIndex == 7 ? Visibility.Visible : Visibility.Collapsed;
                customPagesTextBox.Visibility = pagesComboBox.SelectedIndex == 2 ? Visibility.Visible : Visibility.Collapsed;
                customMarginNumberPicker.Visibility = marginComboBox.SelectedIndex == 3 ? Visibility.Visible : Visibility.Collapsed;

                if (pagesPerSheetComboBox.SelectedIndex == 0)
                {
                    marginComboBox.IsEnabled = true;
                    customMarginNumberPicker.IsEnabled = true;
                    (pagesComboBox.Items[1] as ComboBoxItem).Visibility = Visibility.Visible;
                }
                else
                {
                    marginComboBox.IsEnabled = false;
                    customMarginNumberPicker.IsEnabled = false;

                    if (pagesComboBox.SelectedIndex != 1)
                    {
                        (pagesComboBox.Items[1] as ComboBoxItem).Visibility = Visibility.Collapsed;
                    }
                }

                ReloadDocument();
            }
        }

        private void DocumentPreviewer_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        private void DocumentPreviewer_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void DocumentPreviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ((TextBlock)documentPreviewer.Template.FindName("currentPageTextBlock", documentPreviewer)).Text = "Page " + GetCurrentPageIndex().ToString() + " / " + documentPreviewer.PageCount.ToString();
        }

        private void FirstPageBtn_Click(object sender, RoutedEventArgs e)
        {
            documentPreviewer.FirstPage();
        }

        private void PreviousPageBtn_Click(object sender, RoutedEventArgs e)
        {
            documentPreviewer.PreviousPage();
        }

        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            documentPreviewer.NextPage();
        }

        private void LastPageBtn_Click(object sender, RoutedEventArgs e)
        {
            documentPreviewer.LastPage();
        }

        private void ActualSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            documentPreviewer.FitToMaxPagesAcross(1);
            documentPreviewer.Zoom = 100.0;
        }

        private void CopiesNumberPicker_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (isLoaded == true)
            {
                if (copiesNumberPicker.Value.HasValue == false)
                {
                    copiesNumberPicker.Value = 1;
                }
                collateCheckBox.Visibility = copiesNumberPicker.Value > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void CustomZoomNumberPicker_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (isLoaded == true)
            {
                if (customZoomNumberPicker.Value.HasValue == false)
                {
                    customZoomNumberPicker.Value = 100;
                }
                ReloadDocument();
            }
        }

        private void CustomMarginNumberPicker_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (isLoaded == true)
            {
                if (customMarginNumberPicker.Value.HasValue == false)
                {
                    customMarginNumberPicker.Value = 60;
                }
                ReloadDocument();
            }
        }

        private void CustomPagesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isLoaded == true)
            {
                ReloadDocument();
            }
        }
    }
}