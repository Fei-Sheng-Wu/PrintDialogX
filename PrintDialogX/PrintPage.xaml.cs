using System;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintDialogX.Internal
{
    partial class PrintPage : Page
    {
        private bool _isLoaded;
        private bool _needsRefresh;

        private Package _package;
        private FixedDocument _fixedDocument;

        private readonly PrintWindow _owner;

        private readonly Uri _xpsUrl;
        private readonly PrintServer _localPrintServer;

        private readonly Size _documentSize;
        private readonly double _documentMargin;
        private readonly string _documentName;
        private readonly List<PrintDialogX.PrintPage> _pageContents;

        private readonly bool _allowPagesOption;
        private readonly bool _allowScaleOption;
        private readonly bool _allowDoubleSidedOption;
        private readonly bool _allowPageOrderOption;
        private readonly bool _allowPagesPerSheetOption;
        private readonly bool _allowMoreSettingsExpander;
        private readonly bool _allowAddNewPrinerButton;
        private readonly bool _allowPrinterPreferencesButton;

        private readonly PrintDialog.PrintDialogSettings _defaultSettings;
        private readonly Func<PrintDialog.DocumentInfo, ICollection<PrintDialogX.PrintPage>> _reloadDocumentCallback;

        internal PrintPage(PrintWindow owner, PrintDialog.PrintDialog printDialog)
        {
            _isLoaded = false;
            _needsRefresh = false;

            InitializeComponent();

            this.UpdateLayout();
            Common.DoEvents();

            _owner = owner;

            _defaultSettings = printDialog.DefaultSettings;
            _allowPagesOption = printDialog.AllowPagesOption;
            _allowScaleOption = printDialog.AllowScaleOption;
            _allowDoubleSidedOption = printDialog.AllowDoubleSidedOption;
            _allowPageOrderOption = printDialog.AllowPageOrderOption;
            _allowPagesPerSheetOption = printDialog.AllowPagesPerSheetOption;
            _allowMoreSettingsExpander = printDialog.AllowMoreSettingsExpander;
            _allowAddNewPrinerButton = printDialog.AllowAddNewPrinterButton;
            _allowPrinterPreferencesButton = printDialog.AllowPrinterPreferencesButton;
            _reloadDocumentCallback = printDialog.ReloadDocumentCallback;

            _documentSize = printDialog.Document.DocumentSize;
            _documentMargin = printDialog.Document.DocumentMargin;
            _documentName = printDialog.Document.DocumentName;

            _pageContents = new List<PrintDialogX.PrintPage>();
            _pageContents.AddRange(printDialog.Document.Pages);

            _xpsUrl = new Uri("memorystream://" + Guid.NewGuid().ToString() + ".xps");
            _localPrintServer = new PrintServer();

        }

        private static Wpf.Ui.Controls.ContentDialog CreateErrorDialog(string content)
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

                status = PrinterHelper.PrinterHelper.GetPrinterStatusInfo(_localPrintServer.GetPrintQueue(printer.FullName));
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
                Source = printer == null ? new System.Windows.Media.Imaging.BitmapImage(new Uri("/PrintDialogX;component/Resources/AddPrinter.png", UriKind.Relative)) : PrinterHelper.PrinterIconHelper.GetPrinterIcon(printer, _localPrintServer),
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

                if (_allowAddNewPrinerButton == true)
                {
                    printerComboBox.Items.Insert(0, GeneratePrinterComboBoxItem(null));
                }

                foreach (PrintQueue printer in _localPrintServer.GetPrintQueues())
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

                        _owner.ReturnValue = false;
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
            _needsRefresh = false;

            string originalColor = "";
            string originalQuality = "";
            string originalSize = "";
            string originalType = "";
            string originalSource = "";

            if (useDefaults == false)
            {
                originalColor = colorComboBox.SelectedItem == null ? _defaultSettings.Color.ToString() : (colorComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                originalQuality = qualityComboBox.SelectedItem == null ? _defaultSettings.Quality.ToString() : (qualityComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                originalSize = sizeComboBox.SelectedItem == null ? PrintSettings.SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(_defaultSettings.PageSize) : (sizeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                originalType = typeComboBox.SelectedItem == null ? PrintSettings.SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(_defaultSettings.PageType) : (typeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                originalSource = sourceComboBox.SelectedItem == null ? PrintSettings.SettingsHepler.NameInfoHepler.GetInputBinNameInfo(InputBin.AutoSelect) : (sourceComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            }

            PrintQueue printer = _localPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

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
                    Content = PrintSettings.SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(size.PageMediaSizeName.Value)
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
                    if (PrintSettings.SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(size.PageMediaSizeName.Value) == originalSize)
                    {
                        sizeComboBoxSelectedIndex = sizeComboBox.Items.Count - 1;
                    }
                }
            }
            if (sizeComboBox.HasItems == false)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = PrintSettings.SettingsHepler.NameInfoHepler.GetPageMediaSizeNameInfo(_defaultSettings.PageSize)
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
                    Content = PrintSettings.SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(type)
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
                    if (PrintSettings.SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(type) == originalType)
                    {
                        typeComboBoxSelectedIndex = typeComboBox.Items.Count - 1;
                    }
                }
            }
            if (typeComboBox.HasItems == false)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = PrintSettings.SettingsHepler.NameInfoHepler.GetPageMediaTypeNameInfo(_defaultSettings.PageType)
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
                    Content = PrintSettings.SettingsHepler.NameInfoHepler.GetInputBinNameInfo(source)
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
                    if (PrintSettings.SettingsHepler.NameInfoHepler.GetInputBinNameInfo(source) == originalSource)
                    {
                        sourceComboBoxSelectedIndex = sourceComboBox.Items.Count - 1;
                    }
                }
            }
            if (sourceComboBox.HasItems == false)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = PrintSettings.SettingsHepler.NameInfoHepler.GetInputBinNameInfo(InputBin.AutoSelect)
                };
                sourceComboBox.Items.Add(item);
            }
            sourceComboBox.SelectedIndex = sourceComboBoxSelectedIndex;

            _needsRefresh = true;

            ReloadDocument();
        }

        private async void ReloadDocument()
        {
            if (_isLoaded == true && _needsRefresh == true)
            {
                ((Grid)documentPreviewer.Template.FindName("loadingGrid", documentPreviewer)).Visibility = Visibility.Visible;
                Common.DoEvents();

                try
                {
                    PrintQueue printer = _localPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

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

                    FixedDocument newDocument = new FixedDocument();
                    newDocument.DocumentPaginator.PageSize = orientationComboBox.SelectedIndex == 0 ? new Size(size.Width.Value, size.Height.Value) : new Size(size.Height.Value, size.Width.Value);

                    List<int> pageList = new List<int>();
                    if (pagesComboBox.SelectedIndex == 1)
                    {
                        pageList.Add(GetCurrentPageIndex());
                    }
                    else if (pagesComboBox.SelectedIndex == 2 && string.IsNullOrWhiteSpace(customPagesTextBox.Text) == false)
                    {
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
                                    if (valid == false || number1 > number2 || number1 <= 0 || number2 > _pageContents.Count)
                                    {
                                        Wpf.Ui.Controls.ContentDialog dialog = CreateErrorDialog("The value for custom pages is invalid!");
                                        dialog.ButtonClicked += (s, arg) =>
                                        {
                                            dialog.Hide();
                                            contentHolder.Children.Remove(dialog);
                                        };
                                        contentHolder.Children.Add(dialog);
                                        await dialog.ShowAsync();

                                        pageList.Clear();
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
                                    if (valid == false || number <= 0 || number > _pageContents.Count)
                                    {
                                        Wpf.Ui.Controls.ContentDialog dialog = CreateErrorDialog("The value for custom pages is invalid!");
                                        dialog.ButtonClicked += (s, arg) =>
                                        {
                                            dialog.Hide();
                                            contentHolder.Children.Remove(dialog);
                                        };
                                        contentHolder.Children.Add(dialog);
                                        await dialog.ShowAsync();

                                        pageList.Clear();
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
                        for (int i = 1; i <= _pageContents.Count; i++)
                        {
                            pageList.Add(i);
                        }
                    }
                    pageList.Sort();

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
                        margin = _documentMargin;
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
                        scale = (int)customScaleNumberPicker.Value;
                    }
                    else
                    {
                        scale = (new int[] { 25, 50, 75, 100, 150, 200 })[scaleComboBox.SelectedIndex - 1];
                    }

                    if (_reloadDocumentCallback != null)
                    {
                        _pageContents.Clear();
                        _pageContents.AddRange(_reloadDocumentCallback(new PrintDialog.DocumentInfo()
                        {
                            Color = color,
                            Margin = margin,
                            Orientation = (PrintSettings.PageOrientation)orientationComboBox.SelectedIndex,
                            PageOrder = (PrintSettings.PageOrder)pageOrderComboBox.SelectedIndex,
                            Pages = pageList.ToArray(),
                            PagesPerSheet = (PrintSettings.PagesPerSheet)pagesPerSheetComboBox.SelectedIndex,
                            Scale = scale,
                            Size = new Size(size.Width.Value, size.Height.Value),
                        }));
                    }

                    int pageIndex = 1;
                    List<FixedPage> newPages = new List<FixedPage>();
                    foreach (PrintDialogX.PrintPage originalPage in _pageContents)
                    {
                        if (pageList.Contains(pageIndex) == true)
                        {
                            UIElement originalContent = originalPage.Content;

                            DependencyObject parent = VisualTreeHelper.GetParent(originalContent);
                            if (parent != null && parent is FixedPage)
                            {
                                ((FixedPage)parent).Children.Remove(originalContent);
                            }
                            else if (parent != null)
                            {
                                throw new Exception("PrintPage's Content is already the child of another element.");
                            }

                            FixedPage fixedPage = new FixedPage();
                            fixedPage.Children.Add(originalContent);

                            fixedPage.Width = newDocument.DocumentPaginator.PageSize.Width;
                            fixedPage.Height = newDocument.DocumentPaginator.PageSize.Height;
                            fixedPage.RenderTransformOrigin = new Point(0, 0);

                            if (pagesPerSheetComboBox.SelectedIndex == 0)
                            {
                                if (scaleComboBox.SelectedIndex == 0)
                                {
                                    if (_documentSize.Height * (fixedPage.Width / _documentSize.Width) <= fixedPage.Height)
                                    {
                                        fixedPage.RenderTransform = new ScaleTransform(fixedPage.Width / _documentSize.Width, fixedPage.Width / _documentSize.Width);
                                    }
                                    else
                                    {
                                        fixedPage.RenderTransform = new ScaleTransform(fixedPage.Height / _documentSize.Height, fixedPage.Height / _documentSize.Height);
                                    }
                                }
                                else
                                {
                                    fixedPage.RenderTransform = new ScaleTransform(scale / 100.0, scale / 100.0);
                                }

                                FixedPage.SetLeft(originalContent, margin);
                                FixedPage.SetTop(originalContent, margin);
                            }

                            newPages.Add(fixedPage);
                        }

                        pageIndex++;
                    }

                    if (pagesPerSheetComboBox.SelectedIndex != 0)
                    {
                        PreviewHelper.MultiPagesPerSheetHelper multiPagesPerSheetHelper = new PreviewHelper.MultiPagesPerSheetHelper(newPages, (PrintSettings.PagesPerSheet)pagesPerSheetComboBox.SelectedIndex, newDocument.DocumentPaginator.PageSize, (PrintSettings.PageOrder)pageOrderComboBox.SelectedIndex, (PrintSettings.PageOrientation)orientationComboBox.SelectedIndex);
                        _fixedDocument = multiPagesPerSheetHelper.GetMultiPagesPerSheetDocument(scale);
                    }
                    else
                    {
                        foreach (FixedPage page in newPages)
                        {
                            newDocument.Pages.Add(new PageContent { Child = page });
                            page.UpdateLayout();
                            Common.DoEvents();
                        }
                        _fixedDocument = newDocument;
                    }

                    PackageStore.RemovePackage(_xpsUrl);
                    MemoryStream stream = new MemoryStream();
                    _package = Package.Open(stream, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                    PackageStore.AddPackage(_xpsUrl, _package);
                    XpsDocument xpsDoc = new XpsDocument(_package);
                    try
                    {
                        xpsDoc.Uri = _xpsUrl;
                        XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                        writer.Write(((IDocumentPaginatorSource)_fixedDocument).DocumentPaginator);

                        documentPreviewer.Document = xpsDoc.GetFixedDocumentSequence();
                    }
                    finally
                    {
                        xpsDoc?.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    ((Grid)documentPreviewer.Template.FindName("loadingGrid", documentPreviewer)).Visibility = Visibility.Collapsed;
                    Common.DoEvents();
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

            PrintQueue printer = _localPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            System.Windows.Controls.PrintDialog systemPrintDialog = new System.Windows.Controls.PrintDialog();

            PrintTicket printTicket = systemPrintDialog.PrintTicket;
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
                _owner.TotalPapers = documentPreviewer.PageCount * (int)copiesNumberPicker.Value;
            }
            else
            {
                _owner.TotalPapers = (int)Math.Ceiling(documentPreviewer.PageCount * (int)copiesNumberPicker.Value / 2.0);
            }

            systemPrintDialog.PrintQueue = printer;
            systemPrintDialog.PrintDocument(_fixedDocument.DocumentPaginator, _documentName);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Common.DoEvents();

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
            Common.DoEvents();

            LoadPrinters();
            if (printerComboBox.Items.Count > 0)
            {
                printerPreviewIcon.Source = PrinterHelper.PrinterIconHelper.GetPrinterIcon(_localPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localPrintServer, true);
                printerPreviewText.Text = (printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
                LoadPrinterSettings();

                PrintQueue printer = _localPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

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
            }

            pagesPerSheetComboBox.SelectedIndex = (int)_defaultSettings.PagesPerSheet;
            pageOrderComboBox.SelectedIndex = (int)_defaultSettings.PageOrder;

            customMarginNumberPicker.Maximum = (int)Math.Min(_documentSize.Width / 2, _documentSize.Height / 2) - 15;
            customMarginNumberPicker.Value = _documentMargin;

            _isLoaded = true;
            Common.DoEvents();

            ReloadDocument();
            documentPreviewer.FitToWidth();

            ((Grid)documentPreviewer.Template.FindName("loadingGrid", documentPreviewer)).Visibility = Visibility.Collapsed;
            Common.DoEvents();

            documentPreviewer.FitToWidth();
            Common.DoEvents();

            ((TextBlock)documentPreviewer.Template.FindName("currentPageTextBlock", documentPreviewer)).Text = "Page " + GetCurrentPageIndex().ToString() + " / " + documentPreviewer.PageCount.ToString();
            printButton.Focus();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            PackageStore.RemovePackage(_xpsUrl);
            _package?.Close();
            _localPrintServer.Dispose();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.IsEnabled = false;
            printButton.IsEnabled = false;
            Common.DoEvents();

            PrintDocument();

            _owner.ReturnValue = true;
            Window.GetWindow(this).Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancelButton.IsEnabled = false;
            printButton.IsEnabled = false;
            Common.DoEvents();

            _owner.ReturnValue = false;
            Window.GetWindow(this).Close();
        }

        private async void PrinterPreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string printerName = (printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = "/C rundll32 printui.dll,PrintUIEntry /p /n \"" + printerName + "\"",
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                });
            }
            catch
            {
                try
                {
                    Wpf.Ui.Controls.ContentDialog dialog = CreateErrorDialog("Unable to open the preferences dialog of the printer!");
                    dialog.ButtonClicked += (s, arg) =>
                    {
                        dialog.Hide();
                        contentHolder.Children.Remove(dialog);
                    };
                    contentHolder.Children.Add(dialog);
                    await dialog.ShowAsync();
                }
                catch
                {

                }
            }
        }

        private void PrinterComboBox_DropDownOpened(object sender, EventArgs e)
        {
            _needsRefresh = false;

            _localPrintServer.Refresh();
            LoadPrinters((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString());

            _needsRefresh = true;
        }

        private async void PrinterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (printerComboBox.SelectedItem == null)
            {
                return;
            }

            if (_needsRefresh == true)
            {
                if (_allowAddNewPrinerButton == true && (printerComboBox.SelectedItem as ComboBoxItem).Tag == null)
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
                    printerPreviewIcon.Source = PrinterHelper.PrinterIconHelper.GetPrinterIcon(_localPrintServer.GetPrintQueue((printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString()), _localPrintServer, true);
                    printerPreviewText.Text = (printerComboBox.SelectedItem as ComboBoxItem).Tag.ToString();

                    LoadPrinterSettings(false);
                }
            }
        }

        private void SettingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_needsRefresh == true)
            {
                if (sender is ComboBox comboBox)
                {
                    comboBox.IsDropDownOpen = false;
                }

                customScaleNumberPicker.Visibility = scaleComboBox.SelectedIndex == 7 ? Visibility.Visible : Visibility.Collapsed;
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
            if (_needsRefresh == true)
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
            if (_needsRefresh == true)
            {
                if (customScaleNumberPicker.Value.HasValue == false)
                {
                    customScaleNumberPicker.Value = 100;
                }
                ReloadDocument();
            }
        }

        private void CustomMarginNumberPicker_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (_needsRefresh == true)
            {
                if (customMarginNumberPicker.Value.HasValue == false)
                {
                    customMarginNumberPicker.Value = _documentMargin;
                }
                ReloadDocument();
            }
        }

        private void CustomPagesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_needsRefresh == true)
            {
                ReloadDocument();
            }
        }
    }
}