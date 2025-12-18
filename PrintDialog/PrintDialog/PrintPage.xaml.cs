using System;
using System.IO;
using System.IO.Packaging;
using System.Runtime.InteropServices;
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
    /// <summary>
    /// The PrintDialog page.
    /// </summary>
    partial class PrintPage : Page
    {
        //Properties

        #region Public properties

        /// <summary>
        /// A boolean value, true means Print button clicked, false means Cancel button clicked.
        /// </summary>
        public bool ReturnValue { get; internal set; } = false;

        /// <summary>
        /// The total sheets number that the printer will use.
        /// </summary>
        public int? TotalSheets
        {
            get
            {
                try
                {
                    if (twoSidedCheckBox.IsChecked == false)
                    {
                        return documentPreviewer.PageCount * int.Parse(copiesNumberPicker.Text);
                    }
                    else
                    {
                        return (int)Math.Ceiling(documentPreviewer.PageCount * int.Parse(copiesNumberPicker.Text) / 2.0);
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

        #region Private properties

        private bool isLoaded;
        private bool isRefresh;
        private Package package;
        private int lastPrinterIndex;
        private List<int> lastPageList;
        private FixedDocument fixedDocument;

        private readonly Uri _xpsUrl;
        private readonly int _pageCount;
        private readonly int[] _zoomList;
        private readonly double _pageMargin;
        private readonly string _documentName;
        private readonly Size _orginalPageSize;
        private readonly bool _allowPagesOption;
        private readonly bool _allowScaleOption;
        private readonly bool _allowTwoSidedOption;
        private readonly bool _allowPageOrderOption;
        private readonly bool _allowPagesPerSheetOption;
        private readonly bool _allowMoreSettingsExpander;
        private readonly bool _allowAddNewPrinerComboBoxItem;
        private readonly bool _allowPrinterPreferencesButton;
        private readonly PrintServer _localDefaultPrintServer;
        private readonly List<PageContent> _orginalPagesContentList;
        private readonly PrintDialog.PrintDialogSettings _defaultSettings;
        private readonly System.Windows.Controls.PrintDialog _systemPrintDialog;
        private readonly Func<PrintDialog.DocumentInfo, List<PageContent>> _getDocumentWhenReloadDocumentMethod;

        private static readonly DispatcherOperationCallback _exitFrameCallback = new DispatcherOperationCallback(ExitFrame);

        #endregion

        //Dll Import

        #region Printer Preferences Dialog

        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesW", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter,
        [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName,
        IntPtr pDevModeOutput, ref IntPtr pDevModeInput, int fMode);
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        static extern bool GlobalUnlock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        static extern bool GlobalFree(IntPtr hMem);

        private void OpenPrinterPropertiesDialog(System.Drawing.Printing.PrinterSettings printerSettings)
        {
            Window dialog = new Window();

            IntPtr handle = new System.Windows.Interop.WindowInteropHelper(dialog).Handle;
            IntPtr hDevMode = printerSettings.GetHdevmode(printerSettings.DefaultPageSettings);
            IntPtr pDevMode = GlobalLock(hDevMode);
            int sizeNeeded = DocumentProperties(handle, IntPtr.Zero, printerSettings.PrinterName, pDevMode, ref pDevMode, 0);
            IntPtr devModeData = Marshal.AllocHGlobal(sizeNeeded);
            DocumentProperties(handle, IntPtr.Zero, printerSettings.PrinterName, devModeData, ref pDevMode, 14);
            GlobalUnlock(hDevMode);
            printerSettings.SetHdevmode(devModeData);
            printerSettings.DefaultPageSettings.SetHdevmode(devModeData);
            GlobalFree(hDevMode);
            Marshal.FreeHGlobal(devModeData);
        }

        #endregion

        //Initialize

        #region Initialize the window

        /// <summary>
        /// Initialize PrintDialog.
        /// </summary>
        /// <param name="document">The document that need to print.</param>
        /// <param name="documentName">The document name that will display in print list.</param>
        /// <param name="pageMargin">The page margin info.</param>
        /// <param name="defaultSettings">The default settings.</param>
        /// <param name="allowPagesOption">Allow pages option or not.</param>
        /// <param name="allowScaleOption">Allow scale option or not.</param>
        /// <param name="allowTwoSidedOption">Allow two-sided option or not.</param>
        /// <param name="allowPagesPerSheetOption">Allow pages per sheet option or not.</param>
        /// <param name="allowPageOrderOption">Allow page order option or not if allow pages per sheet option.</param>
        /// <param name="allowMoreSettingsExpander">Allow more settings expander or just show all settings.</param>
        /// <param name="allowAddNewPrinerComboBoxItem">Allow add new printer button in printer list or not.</param>
        /// <param name="allowPrinterPreferencesButton">Allow printer preferences button or not.</param>
        /// <param name="getDocumentWhenReloadDocumentMethod">The method that will use to get document when reload document. You can only change the content in the document. The method must return a list of PageContent that include the page content in order.</param>
        public PrintPage(FixedDocument document, string documentName, double pageMargin, PrintDialog.PrintDialogSettings defaultSettings, bool allowPagesOption, bool allowScaleOption, bool allowTwoSidedOption, bool allowPagesPerSheetOption, bool allowPageOrderOption, bool allowMoreSettingsExpander, bool allowAddNewPrinerComboBoxItem, bool allowPrinterPreferencesButton, Func<PrintDialog.DocumentInfo, List<PageContent>> getDocumentWhenReloadDocumentMethod)
        {
            isLoaded = false;

            InitializeComponent();

            this.UpdateLayout();
            DoEvents();

            isRefresh = true;
            lastPageList = new List<int>();
            fixedDocument = document;

            _pageMargin = pageMargin;
            _documentName = documentName;
            _defaultSettings = defaultSettings;
            _allowPagesOption = allowPagesOption;
            _allowScaleOption = allowScaleOption;
            _allowTwoSidedOption = allowTwoSidedOption;
            _allowPageOrderOption = allowPageOrderOption;
            _allowPagesPerSheetOption = allowPagesPerSheetOption;
            _allowMoreSettingsExpander = allowMoreSettingsExpander;
            _allowAddNewPrinerComboBoxItem = allowAddNewPrinerComboBoxItem;
            _allowPrinterPreferencesButton = allowPrinterPreferencesButton;
            _getDocumentWhenReloadDocumentMethod = getDocumentWhenReloadDocumentMethod;

            _pageCount = document.Pages.Count;
            _orginalPageSize = document.DocumentPaginator.PageSize;

            _xpsUrl = new Uri("memorystream://" + Guid.NewGuid().ToString() + ".xps");
            _zoomList = new int[] { 25, 50, 75, 100, 150, 200 };
            _localDefaultPrintServer = new PrintServer();
            _orginalPagesContentList = new List<PageContent>();
            _systemPrintDialog = new System.Windows.Controls.PrintDialog();

            foreach (PageContent page in document.Pages)
            {
                string xaml = XamlWriter.Save(page);
                PageContent pageClone = XamlReader.Parse(xaml) as PageContent;

                _orginalPagesContentList.Add(pageClone);
            }
        }

        #endregion

        //Methods

        #region Define DoEvents() method to refresh the interface

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

        #endregion

        #region Define GetChildren() method to get all chilren of control

        internal static List<DependencyObject> GetChildren(DependencyObject element)
        {
            List<DependencyObject> elements = new List<DependencyObject>();

            if (VisualTreeHelper.GetChildrenCount(element) > 0)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    DependencyObject control = VisualTreeHelper.GetChild(element, i);
                    if (control != null)
                    {
                        elements.Add(control);
                        elements.AddRange(GetChildren(control));
                    }
                }
            }

            return elements;
        }

        #endregion

        #region Define GetCurrentPage() method to get the page index which user is looking

        private int GetCurrentPage()
        {
            int page;
            ScrollViewer scrollViewer = (ScrollViewer)documentPreviewer.Template.FindName("PART_ContentHost", documentPreviewer);

            if (documentPreviewer.MaxPagesAcross == 1)
            {
                page = (int)(scrollViewer.VerticalOffset / (scrollViewer.ExtentHeight / documentPreviewer.PageCount)) + 1;
            }
            else
            {
                page = ((int)(scrollViewer.VerticalOffset / (scrollViewer.ExtentHeight / Math.Ceiling((double)documentPreviewer.PageCount / 2))) + 1) * 2 - 1;
            }

            if (page < 1)
            {
                page = 1;
            }
            if (page > documentPreviewer.PageCount)
            {
                page = documentPreviewer.PageCount;
            }

            return page;
        }

        #endregion

        #region Define LoadPrinters() method to find all printers

        private void LoadPrinters()
        {
            try
            {
                int equipmentComboBoxSelectedIndex = 0;
                equipmentComboBox.Items.Clear();

                if (_allowAddNewPrinerComboBoxItem == true)
                {
                    Grid itemMainGrid = new Grid();
                    itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(55) });
                    itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10) });
                    itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    TextBlock textInfoBlock = new TextBlock()
                    {
                        Text = "Add New Printer",
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Image itemIcon = new Image()
                    {
                        Width = 55,
                        Height = 55,
                        Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("/PrintDialog;component/Resources/AddPrinter.png", UriKind.Relative)),
                        Stretch = Stretch.Fill
                    };

                    itemIcon.SetValue(Grid.ColumnProperty, 0);
                    textInfoBlock.SetValue(Grid.ColumnProperty, 2);

                    itemMainGrid.Children.Add(itemIcon);
                    itemMainGrid.Children.Add(textInfoBlock);

                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = itemMainGrid,
                        Height = 55,
                        ToolTip = "Add New Printer"
                    };

                    equipmentComboBox.Items.Insert(0, item);
                }

                foreach (PrintQueue printer in _localDefaultPrintServer.GetPrintQueues())
                {
                    printer.Refresh();

                    string status = PrinterHelper.PrinterHelper.GetPrinterStatusInfo(_localDefaultPrintServer.GetPrintQueue(printer.FullName));
                    string loction = printer.Location;
                    string comment = printer.Comment;

                    if (String.IsNullOrWhiteSpace(loction) == true)
                    {
                        loction = "Unknown";
                    }
                    if (String.IsNullOrWhiteSpace(comment) == true)
                    {
                        comment = "Unknown";
                    }

                    Grid itemMainGrid = new Grid();
                    itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(55) });
                    itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10) });
                    itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    StackPanel textInfoPanel = new StackPanel()
                    {
                        Orientation = Orientation.Vertical,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    textInfoPanel.Children.Add(new TextBlock() { Text = printer.FullName, FontSize = 14 });
                    textInfoPanel.Children.Add(new TextBlock() { Text = status, Margin = new Thickness(0, 7, 0, 0) });

                    Image printerIcon = new Image()
                    {
                        Width = 55,
                        Height = 55,
                        Source = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(printer, _localDefaultPrintServer),
                        Stretch = Stretch.Fill
                    };

                    if (printer.IsOffline == true)
                    {
                        printerIcon.Opacity = 0.5;
                    }

                    printerIcon.SetValue(Grid.ColumnProperty, 0);
                    textInfoPanel.SetValue(Grid.ColumnProperty, 2);

                    itemMainGrid.Children.Add(printerIcon);
                    itemMainGrid.Children.Add(textInfoPanel);

                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = itemMainGrid,
                        Height = 55,
                        ToolTip = "Name: " + printer.FullName + "\nStatus: " + status + "\nDocument: " + printer.NumberOfJobs + "\nLoction: " + loction + "\nComment: " + comment,
                        Tag = printer.FullName
                    };

                    equipmentComboBox.Items.Insert(0, item);

                    if (LocalPrintServer.GetDefaultPrintQueue().FullName == printer.FullName)
                    {
                        equipmentComboBoxSelectedIndex = equipmentComboBox.Items.Count;
                    }
                }

                if (equipmentComboBox.Items.Count == 0)
                {
                    MessageWindow window = new MessageWindow("There is no printer support.", "Error", "OK", null, MessageWindow.MessageIcon.Error)
                    {
                        Owner = Window.GetWindow(this),
                        Icon = Window.GetWindow(this).Icon
                    };
                    window.ShowDialog();

                    Window.GetWindow(this).Close();
                }

                equipmentComboBoxSelectedIndex = equipmentComboBox.Items.Count - equipmentComboBoxSelectedIndex;

                lastPrinterIndex = equipmentComboBoxSelectedIndex;
                equipmentComboBox.SelectedIndex = equipmentComboBoxSelectedIndex;

                equipmentComboBox.Tag = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(_localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localDefaultPrintServer, true);
                equipmentComboBox.Text = (equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            }
            catch
            {
                return;
            }
        }

        #endregion

        #region Define LoadPrinterSettings() method to load the printer's settings

        private void LoadPrinterSettings(bool useDefaults = true)
        {
            string lastColor = "";
            string lastQuality = "";
            string lastSize = "";
            string lastType = "";
            string lastSource = "";

            if (useDefaults == false)
            {
                try
                {
                    lastColor = (colorComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    lastQuality = (qualityComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    lastSize = (sizeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    lastType = (typeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    lastSource = (sourceComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                }
                catch
                {
                    lastColor = _defaultSettings.Color.ToString();
                    lastQuality = _defaultSettings.Quality.ToString();
                    lastSize = SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(_defaultSettings.PageSize);
                    lastType = SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(_defaultSettings.PageType);
                    lastSource = SettingsHepler.NameInfoHepler.GetInputBinNameInfo(InputBin.AutoSelect);
                }
            }

            PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            if (printer.GetPrintCapabilities().MaxCopyCount.HasValue)
            {
                copiesNumberPicker.MaxValue = printer.GetPrintCapabilities().MaxCopyCount.Value;
            }
            else
            {
                copiesNumberPicker.MaxValue = 1;
            }

            copiesNumberPicker.ChangeButtonEnabled();

            if (int.Parse(copiesNumberPicker.Text) > 1)
            {
                collateCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                collateCheckBox.Visibility = Visibility.Collapsed;
            }

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
                    if (color.ToString() == lastColor)
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
                    if (quality.ToString() == lastQuality)
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
                        twoSidedCheckBox.IsChecked = true;
                        twoSidedTypeComboBox.SelectedIndex = 0;
                    }
                    else if (printer.DefaultPrintTicket.Duplexing.Value == Duplexing.TwoSidedShortEdge)
                    {
                        twoSidedCheckBox.IsChecked = true;
                        twoSidedTypeComboBox.SelectedIndex = 1;
                    }
                    else
                    {
                        twoSidedCheckBox.IsChecked = false;
                    }
                }
                else
                {
                    twoSidedCheckBox.IsChecked = false;
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
                    if (SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(size.PageMediaSizeName.Value) == lastSize)
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
                    if (SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(type) == lastType)
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
                    if (SettingsHepler.NameInfoHepler.GetInputBinNameInfo(source) == lastSource)
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
        }

        #endregion

        #region Define LoadPreview() method to make XPS document and load document preview

        private void LoadPreview(FixedDocument doc)
        {
            PackageStore.RemovePackage(_xpsUrl);
            MemoryStream stream = new MemoryStream();
            package = Package.Open(stream, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            PackageStore.AddPackage(_xpsUrl, package);
            XpsDocument xpsDoc = new XpsDocument(package);
            try
            {
                xpsDoc.Uri = _xpsUrl;
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                writer.Write(((IDocumentPaginatorSource)doc).DocumentPaginator);

                documentPreviewer.Document = xpsDoc.GetFixedDocumentSequence();
            }
            finally
            {
                if (xpsDoc != null)
                {
                    xpsDoc.Close();
                }
            }
        }

        #endregion

        #region Define ReloadDocument() method to reload document with user settings

        private void ReloadDocument()
        {
            if (isRefresh == true)
            {
                loadingGrid.Visibility = Visibility.Visible;
                DoEvents();

                try
                {
                    PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

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
                            }

                            index++;
                        }

                        size = new PageMediaSize(sizeName);
                    }

                    FixedDocument doc = new FixedDocument();

                    if (orientationComboBox.SelectedIndex == 0)
                    {
                        doc.DocumentPaginator.PageSize = new Size(size.Width.Value, size.Height.Value);
                    }
                    else
                    {
                        doc.DocumentPaginator.PageSize = new Size(size.Height.Value, size.Width.Value);
                    }

                    List<int> pageList = new List<int>();
                    bool isCustomPages = false;
                    if (pagesComboBox.SelectedIndex == 1)
                    {
                        isCustomPages = true;

                        if (lastPageList.Count > 0)
                        {
                            pageList.Add(lastPageList[GetCurrentPage() - 1]);
                        }
                        else
                        {
                            pageList.Add(GetCurrentPage());
                        }
                    }
                    else if (pagesComboBox.SelectedIndex == 2 && String.IsNullOrWhiteSpace(customPagesTextBox.Text) == false)
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

                                    if (pageRange.Length != 2 || int.Parse(pageRange[0]) > int.Parse(pageRange[1]) || int.Parse(pageRange[0]) <= 0 || int.Parse(pageRange[1]) > _pageCount)
                                    {
                                        MessageWindow window = new MessageWindow("The custom pages value is invalid.", "Error", "OK", null, MessageWindow.MessageIcon.Error)
                                        {
                                            Owner = Window.GetWindow(this),
                                            Icon = Window.GetWindow(this).Icon
                                        };
                                        window.ShowDialog();

                                        pageList = new List<int>();
                                        isCustomPages = false;
                                        break;
                                    }
                                    else
                                    {
                                        for (int i = int.Parse(pageRange[0]); i <= int.Parse(pageRange[1]); i++)
                                        {
                                            pageList.Add(i);
                                        }
                                    }
                                }
                                catch
                                {
                                    MessageWindow window = new MessageWindow("The custom pages value is invalid.", "Error", "OK", null, MessageWindow.MessageIcon.Error)
                                    {
                                        Owner = Window.GetWindow(this),
                                        Icon = Window.GetWindow(this).Icon
                                    };
                                    window.ShowDialog();

                                    pageList = new List<int>();
                                    isCustomPages = false;
                                    break;
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (int.Parse(str) <= 0 || int.Parse(str) > _pageCount)
                                    {
                                        MessageWindow window = new MessageWindow("The custom pages value is invalid.", "Error", "OK", null, MessageWindow.MessageIcon.Error)
                                        {
                                            Owner = Window.GetWindow(this),
                                            Icon = Window.GetWindow(this).Icon
                                        };
                                        window.ShowDialog();

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
                                    MessageWindow window = new MessageWindow("The custom pages value is invalid.", "Error", "OK", null, MessageWindow.MessageIcon.Error)
                                    {
                                        Owner = Window.GetWindow(this),
                                        Icon = Window.GetWindow(this).Icon
                                    };
                                    window.ShowDialog();

                                    pageList = new List<int>();
                                    isCustomPages = false;
                                    break;
                                }
                            }
                        }
                    }

                    lastPageList = pageList;

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
                        try
                        {
                            margin = int.Parse(customMarginNumberPicker.Text);
                        }
                        catch
                        {
                            margin = 0;
                        }
                    }

                    if (scaleComboBox.SelectedIndex == 0)
                    {
                        scale = double.NaN;
                    }
                    else if (scaleComboBox.SelectedIndex == 7)
                    {
                        try
                        {
                            scale = int.Parse(customZoomNumberPicker.Text);
                        }
                        catch
                        {
                            scale = 100;
                        }
                    }
                    else
                    {
                        scale = _zoomList[scaleComboBox.SelectedIndex - 1];
                    }

                    List<PageContent> pageContentList;

                    if (_getDocumentWhenReloadDocumentMethod != null)
                    {
                        pageContentList = _getDocumentWhenReloadDocumentMethod(new PrintDialog.DocumentInfo()
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
                        pageContentList = _orginalPagesContentList;
                    }

                    int pageCount = 1;

                    foreach (PageContent orginalPage in pageContentList)
                    {
                        if ((isCustomPages == true && pageList.Count >= 1 && pageList.Contains(pageCount) == true) || isCustomPages == false)
                        {
                            FixedPage fixedPage = XamlReader.Parse(XamlWriter.Save(orginalPage.Child)) as FixedPage;

                            fixedPage.Width = doc.DocumentPaginator.PageSize.Width;
                            fixedPage.Height = doc.DocumentPaginator.PageSize.Height;
                            fixedPage.RenderTransformOrigin = new Point(0, 0);

                            if (pagesPerSheetComboBox.SelectedIndex == 0)
                            {
                                if (scaleComboBox.SelectedIndex == 0)
                                {
                                    if (_orginalPageSize.Height * (fixedPage.Width / _orginalPageSize.Width) <= fixedPage.Height)
                                    {
                                        fixedPage.RenderTransform = new ScaleTransform(fixedPage.Width / _orginalPageSize.Width, fixedPage.Width / _orginalPageSize.Width);
                                    }
                                    else
                                    {
                                        fixedPage.RenderTransform = new ScaleTransform(fixedPage.Height / _orginalPageSize.Height, fixedPage.Height / _orginalPageSize.Height);
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
                        PreviewHelper.MultiPagesPerSheetHelper multiPagesPerSheetHelper = new PreviewHelper.MultiPagesPerSheetHelper(int.Parse((pagesPerSheetComboBox.SelectedItem as ComboBoxItem).Content.ToString()), doc, _orginalPageSize, (PreviewHelper.DocumentOrientation)orientationComboBox.SelectedIndex, (PreviewHelper.PageOrder)pageOrderComboBox.SelectedIndex);
                        fixedDocument = multiPagesPerSheetHelper.GetMultiPagesPerSheetDocument(scale);
                    }
                    else
                    {
                        fixedDocument = doc;
                    }

                    LoadPreview(fixedDocument);
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

        #endregion

        #region Define PrintDocument() method to print the document with user settings

        private void PrintDocument()
        {
            if (equipmentComboBox.SelectedItem == null)
            {
                return;
            }

            ReloadDocument();

            PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            PrintTicket printTicket = _systemPrintDialog.PrintTicket;
            printTicket.CopyCount = int.Parse(copiesNumberPicker.Text);
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
            if (twoSidedCheckBox.IsChecked == true)
            {
                if (twoSidedTypeComboBox.SelectedIndex == 0)
                {
                    printTicket.Duplexing = Duplexing.TwoSidedLongEdge;
                }
                else
                {
                    printTicket.Duplexing = Duplexing.TwoSidedShortEdge;
                }
            }
            else
            {
                printTicket.Duplexing = Duplexing.OneSided;
            }
            printTicket.PageScalingFactor = 100;
            printTicket.PagesPerSheet = 1;
            printTicket.PagesPerSheetDirection = PagesPerSheetDirection.RightBottom;

            _systemPrintDialog.PrintQueue = printer;
            _systemPrintDialog.PrintDocument(fixedDocument.DocumentPaginator, _documentName);
        }

        #endregion

        //Events

        #region The window loaded event to load printers, settings and document

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

            if (_allowTwoSidedOption == false)
            {
                twoSidedTextBlock.Visibility = Visibility.Collapsed;
                twoSidedCheckBox.Visibility = Visibility.Collapsed;
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
            LoadPrinterSettings();

            PrintQueue printer = _localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            if (_defaultSettings.Layout == PrintSettings.PageOrientation.Portrait)
            {
                orientationComboBox.SelectedIndex = 0;
            }
            else
            {
                orientationComboBox.SelectedIndex = 1;
            }

            if (_allowTwoSidedOption == true)
            {
                if (_defaultSettings.UsePrinterDefaultSettings == false)
                {
                    if (_defaultSettings.TwoSided == PrintSettings.TwoSided.OneSided)
                    {
                        twoSidedCheckBox.IsChecked = false;
                    }
                    else
                    {
                        twoSidedCheckBox.IsChecked = true;

                        if (_defaultSettings.TwoSided == PrintSettings.TwoSided.TwoSidedLongEdge)
                        {
                            twoSidedTypeComboBox.SelectedIndex = 0;
                        }
                        else
                        {
                            twoSidedTypeComboBox.SelectedIndex = 1;
                        }
                    }
                }
                else
                {
                    if (printer.DefaultPrintTicket.Duplexing.HasValue)
                    {
                        if (printer.DefaultPrintTicket.Duplexing.Value == Duplexing.TwoSidedLongEdge)
                        {
                            twoSidedCheckBox.IsChecked = true;
                            twoSidedTypeComboBox.SelectedIndex = 0;
                        }
                        else if (printer.DefaultPrintTicket.Duplexing.Value == Duplexing.TwoSidedShortEdge)
                        {
                            twoSidedCheckBox.IsChecked = true;
                            twoSidedTypeComboBox.SelectedIndex = 1;
                        }
                        else
                        {
                            twoSidedCheckBox.IsChecked = false;
                        }
                    }
                    else
                    {
                        twoSidedCheckBox.IsChecked = false;
                    }
                }
            }

            pagesPerSheetComboBox.SelectedIndex = PreviewHelper.MultiPagesPerSheetHelper.GetPagePerSheetCountIndex(_defaultSettings.PagesPerSheet);
            pageOrderComboBox.SelectedIndex = (int)_defaultSettings.PageOrder;

            customMarginNumberPicker.MaxValue = (int)Math.Min(_orginalPageSize.Width / 2, _orginalPageSize.Height / 2) - 15;
            customMarginNumberPicker.Text = _pageMargin.ToString();

            DoEvents();

            ReloadDocument();
            documentPreviewer.FitToWidth();

            loadingGrid.Visibility = Visibility.Collapsed;
            DoEvents();

            documentPreviewer.FitToWidth();
            DoEvents();

            isLoaded = true;

            ((TextBlock)documentPreviewer.Template.FindName("currentPageTextBlock", documentPreviewer)).Text = "Page " + GetCurrentPage().ToString() + " / " + documentPreviewer.PageCount.ToString();
            printButton.Focus();
        }

        #endregion

        #region The window closed event to release properties

        private void Window_Closed(object sender, EventArgs e)
        {
            PackageStore.RemovePackage(_xpsUrl);
            if (package != null)
            {
                package.Close();
            }

            _localDefaultPrintServer.Dispose();
        }

        #endregion

        #region The print button click event to print document

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.IsEnabled = false;
            printButton.IsEnabled = false;
            DoEvents();

            PrintDocument();

            ReturnValue = true;
            Window.GetWindow(this).Close();
        }

        #endregion

        #region The cancel button click event to close the window

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.IsEnabled = false;
            printButton.IsEnabled = false;
            DoEvents();

            ReturnValue = false;
            Window.GetWindow(this).Close();
        }

        #endregion

        #region The printer preferences button click event to open printer preferences dialog

        private void PrinterPreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Drawing.Printing.PrinterSettings settings = new System.Drawing.Printing.PrinterSettings
                {
                    PrinterName = (equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString()
                };
                OpenPrinterPropertiesDialog(settings);
            }
            catch
            {
                return;
            }
        }

        #endregion

        #region The printer combo box drop down opened event to refresh printer list

        private void EquipmentComboBox_DropDownOpened(object sender, EventArgs e)
        {
            isRefresh = false;

            _localDefaultPrintServer.Refresh();

            string equipmentComboBoxSelectedItem = (equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            int equipmentComboBoxSelectedIndex = -1;
            int defaultEquipmentComboBoxSelectedIndex = -1;
            equipmentComboBox.Items.Clear();

            if (_allowAddNewPrinerComboBoxItem == true)
            {
                Grid itemMainGrid = new Grid();
                itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(55) });
                itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10) });
                itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock textInfoBlock = new TextBlock()
                {
                    Text = "Add New Printer",
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };

                Image itemIcon = new Image()
                {
                    Width = 55,
                    Height = 55,
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("/PrintDialog;component/Resources/AddPrinter.png", UriKind.Relative)),
                    Stretch = Stretch.Fill
                };

                itemIcon.SetValue(Grid.ColumnProperty, 0);
                textInfoBlock.SetValue(Grid.ColumnProperty, 2);

                itemMainGrid.Children.Add(itemIcon);
                itemMainGrid.Children.Add(textInfoBlock);

                ComboBoxItem item = new ComboBoxItem
                {
                    Content = itemMainGrid,
                    Height = 55,
                    ToolTip = "Add New Printer"
                };

                equipmentComboBox.Items.Insert(0, item);
            }

            foreach (PrintQueue printer in _localDefaultPrintServer.GetPrintQueues())
            {
                printer.Refresh();

                string status = PrinterHelper.PrinterHelper.GetPrinterStatusInfo(_localDefaultPrintServer.GetPrintQueue(printer.FullName));
                string loction = printer.Location;
                string comment = printer.Comment;

                if (String.IsNullOrWhiteSpace(loction) == true)
                {
                    loction = "Unknown";
                }
                if (String.IsNullOrWhiteSpace(comment) == true)
                {
                    comment = "Unknown";
                }

                Grid itemMainGrid = new Grid();
                itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(55) });
                itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10) });
                itemMainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                StackPanel textInfoPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center
                };
                textInfoPanel.Children.Add(new TextBlock() { Text = printer.FullName, FontSize = 14 });
                textInfoPanel.Children.Add(new TextBlock() { Text = status, Margin = new Thickness(0, 7, 0, 0) });

                Image printerIcon = new Image()
                {
                    Width = 55,
                    Height = 55,
                    Source = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(printer, _localDefaultPrintServer),
                    Stretch = Stretch.Fill
                };

                if (printer.IsOffline == true)
                {
                    printerIcon.Opacity = 0.5;
                }

                printerIcon.SetValue(Grid.ColumnProperty, 0);
                textInfoPanel.SetValue(Grid.ColumnProperty, 2);

                itemMainGrid.Children.Add(printerIcon);
                itemMainGrid.Children.Add(textInfoPanel);

                ComboBoxItem item = new ComboBoxItem
                {
                    Content = itemMainGrid,
                    Height = 55,
                    ToolTip = "Name: " + printer.FullName + "\nStatus: " + status + "\nDocument: " + printer.NumberOfJobs + "\nLoction: " + loction + "\nComment: " + comment,
                    Tag = printer.FullName
                };

                equipmentComboBox.Items.Insert(0, item);

                if (equipmentComboBoxSelectedItem == printer.FullName)
                {
                    equipmentComboBoxSelectedIndex = equipmentComboBox.Items.Count;
                }
                if (LocalPrintServer.GetDefaultPrintQueue().FullName == printer.FullName)
                {
                    defaultEquipmentComboBoxSelectedIndex = equipmentComboBox.Items.Count;
                }
            }

            if (equipmentComboBoxSelectedIndex == -1)
            {
                if (defaultEquipmentComboBoxSelectedIndex == -1)
                {
                    equipmentComboBoxSelectedIndex = 0;
                }
                else
                {
                    equipmentComboBoxSelectedIndex = equipmentComboBox.Items.Count - defaultEquipmentComboBoxSelectedIndex;
                }
            }
            else
            {
                equipmentComboBoxSelectedIndex = equipmentComboBox.Items.Count - equipmentComboBoxSelectedIndex;
            }

            lastPrinterIndex = equipmentComboBoxSelectedIndex;
            equipmentComboBox.SelectedIndex = equipmentComboBoxSelectedIndex;

            equipmentComboBox.Tag = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(_localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localDefaultPrintServer, true);
            equipmentComboBox.Text = (equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString();

            isRefresh = true;
        }

        #endregion

        #region The printer combo box selection changed event to load printer's settings

        private async void EquipmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (equipmentComboBox.SelectedItem == null)
            {
                return;
            }

            if (isLoaded == true && isRefresh == true)
            {
                equipmentComboBox.IsDropDownOpen = false;

                if (equipmentComboBox.SelectedIndex == equipmentComboBox.Items.Count - 1 && (equipmentComboBox.SelectedItem as ComboBoxItem).Tag == null)
                {
                    equipmentComboBox.SelectedIndex = lastPrinterIndex;

                    equipmentComboBox.Tag = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(_localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localDefaultPrintServer, true);
                    equipmentComboBox.Text = (equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString();

                    Windows.System.LauncherOptions option = new Windows.System.LauncherOptions()
                    {
                        TreatAsUntrusted = false
                    };
                    bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:printers"), option);

                    if (result == false)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", "shell:::{A8A91A66-3A7D-4424-8D24-04E180695C7A}");
                    }
                }
                else
                {
                    if (lastPrinterIndex != equipmentComboBox.SelectedIndex)
                    {
                        equipmentComboBox.Tag = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(_localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localDefaultPrintServer, true);
                        equipmentComboBox.Text = (equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString();

                        lastPrinterIndex = equipmentComboBox.SelectedIndex;
                        LoadPrinterSettings(false);

                        equipmentComboBox.Tag = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(_localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localDefaultPrintServer, true);
                        equipmentComboBox.Text = (equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
                    }
                    else
                    {
                        equipmentComboBox.Tag = PrinterInfoHelper.PrinterIconHelper.GetPrinterIcon(_localDefaultPrintServer.GetPrintQueue((equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localDefaultPrintServer, true);
                        equipmentComboBox.Text = (equipmentComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
                    }
                }
            }
        }

        #endregion

        #region The setting combo boxes selection changed event to reload document

        private void SettingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoaded == true)
            {
                if (sender is ComboBox comboBox)
                {
                    comboBox.IsDropDownOpen = false;
                }

                if (scaleComboBox.SelectedIndex == 7)
                {
                    customZoomNumberPicker.Visibility = Visibility.Visible;
                }
                else
                {
                    customZoomNumberPicker.Visibility = Visibility.Collapsed;
                }

                if (pagesComboBox.SelectedIndex == 2)
                {
                    customPagesTextBox.Visibility = Visibility.Visible;
                }
                else
                {
                    customPagesTextBox.Visibility = Visibility.Collapsed;
                }

                if (marginComboBox.SelectedIndex == 3)
                {
                    customMarginNumberPicker.Visibility = Visibility.Visible;
                }
                else
                {
                    customMarginNumberPicker.Visibility = Visibility.Collapsed;
                }

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

        #endregion

        #region The document viewer context menu opening event to cancel it

        private void DocumentPreviewer_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region The document viewer manipulation boundary feedback event to cancel it

        private void DocumentPreviewer_ManipulationBoundaryFeedback(object sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region The document viewer scroll changed event to refresh the current page text block content

        private void DocumentPreviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ((TextBlock)documentPreviewer.Template.FindName("currentPageTextBlock", documentPreviewer)).Text = "Page " + GetCurrentPage().ToString() + " / " + documentPreviewer.PageCount.ToString();
        }

        #endregion

        #region The document viewer navigate buttons click events to navigate

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

        #endregion

        #region The document viewer actual size button click event to cancel 2 pages across mode

        private void ActualSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            documentPreviewer.FitToMaxPagesAcross(1);
            documentPreviewer.Zoom = 100.0;
        }

        #endregion

        #region The copies count number picker input completed event to show collate check box or not

        private void CopiesNumberPicker_InputCompleted(object sender, RoutedEventArgs e)
        {
            if (isLoaded == true)
            {
                if (int.Parse(copiesNumberPicker.Text) > 1)
                {
                    collateCheckBox.Visibility = Visibility.Visible;
                }
                else
                {
                    collateCheckBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region The custom zoom number picker input completed event to reload document

        private void CustomZoomNumberPicker_InputComplated(object sender, RoutedEventArgs e)
        {
            if (isLoaded == true)
            {
                ReloadDocument();
            }
        }

        #endregion

        #region The custom margin number picker input completed event to reload document

        private void CustomMarginNumberPicker_InputCompleted(object sender, RoutedEventArgs e)
        {
            if (isLoaded == true)
            {
                ReloadDocument();
            }
        }

        #endregion

        #region The custom pages text box lost focus event to reload document

        private void CustomPagesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isLoaded == true)
            {
                ReloadDocument();
            }
        }

        #endregion
    }
}