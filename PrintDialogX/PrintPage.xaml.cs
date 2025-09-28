using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Linq;

namespace PrintDialogX.Internal
{
    partial class PrintPage : Page
    {
        private bool _isLoadingCompleted;
        private bool _isRefreshRequested;

        private readonly PrintWindow _owner;

        private System.Windows.Documents.FixedDocument _previewDocument;

        private readonly PrintServer _printServer;
        private readonly PrintQueueCollection _printServerCollectionFax;

        private readonly List<PrintDialogX.PrintPage> _originalDocument;
        private readonly Size _originalDocumentSize;
        private readonly double _originalDocumentMargin;
        private readonly string _originalDocumentName;

        private readonly bool _allowPages;
        private readonly bool _allowPageOrder;
        private readonly bool _allowPagesPerSheet;
        private readonly bool _allowScale;
        private readonly bool _allowDoubleSided;
        private readonly bool _allowAddNewPrinter;
        private readonly bool _allowPrinterPreferences;

        private readonly PrintQueue _printerDefault;
        private readonly PrintDialog.PrintDialogSettings _settingsDefault;
        private readonly Func<PrintDialog.DocumentInfo, ICollection<PrintDialogX.PrintPage>> _documentUpdateCallback;

        internal PrintPage(PrintWindow owner, PrintDialog.PrintDialog dialog)
        {
            _isLoadingCompleted = false;
            _isRefreshRequested = false;

            InitializeComponent();

            this.UpdateLayout();
            Common.DoEvents();

            _owner = owner;

            _originalDocument = new List<PrintDialogX.PrintPage>();
            _originalDocument.AddRange(dialog.Document.Pages);
            _originalDocumentSize = dialog.Document.DocumentSize;
            _originalDocumentMargin = dialog.Document.DocumentMargin;
            _originalDocumentName = dialog.Document.DocumentName;

            _allowPages = dialog.AllowPagesOption;
            _allowPageOrder = dialog.AllowPageOrderOption;
            _allowPagesPerSheet = dialog.AllowPagesPerSheetOption;
            _allowScale = dialog.AllowScaleOption;
            _allowDoubleSided = dialog.AllowDoubleSidedOption;
            _allowAddNewPrinter = dialog.AllowAddNewPrinterButton;
            _allowPrinterPreferences = dialog.AllowPrinterPreferencesButton;

            _printerDefault = dialog.DefaultPrinter;
            _settingsDefault = dialog.DefaultSettings;
            _documentUpdateCallback = dialog.ReloadDocumentCallback;

            _printServer = new PrintServer();
            _printServerCollectionFax = _printServer.GetPrintQueues(new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.Fax });
        }

        private async void CreateErrorPopup(string content, Action action)
        {
            try
            {
                Wpf.Ui.Controls.ContentDialog message = new Wpf.Ui.Controls.ContentDialog()
                {
                    Title = "Error",
                    Content = content,
                    DialogMaxHeight = 220,
                    CloseButtonText = "Ok",
                    IsPrimaryButtonEnabled = false,
                    IsSecondaryButtonEnabled = false,
                    DefaultButton = Wpf.Ui.Controls.ContentDialogButton.Primary,
                };
                Grid.SetColumnSpan(message, 2);
                message.ButtonClicked += (s, arg) =>
                {
                    message.Hide();
                    container.Children.Remove(message);
                    action?.Invoke();
                };
                container.Children.Add(message);
                await message.ShowAsync();
            }
            catch { }
        }

        private int GetCurrentPageIndex()
        {
            ScrollViewer scrollViewer = previewer.Template.FindName("PART_ContentHost", previewer) as ScrollViewer;
            return Math.Max(1, Math.Min(previewer.PageCount, previewer.MaxPagesAcross > 1 ? ((int)(scrollViewer.VerticalOffset / (scrollViewer.ExtentHeight / Math.Ceiling(previewer.PageCount / 2.0))) + 1) * 2 - 1 : (int)(scrollViewer.VerticalOffset / (scrollViewer.ExtentHeight / previewer.PageCount)) + 1));
        }

        private void LoadPrinters(PrintQueue printerSelected)
        {
            ComboBoxItem actionCreateItem(PrintQueue printer)
            {
                if (printer != null)
                {
                    PrinterHelper.RefreshPrinter(printer);
                }
                string printerName = printer != null ? printer.FullName : string.Empty;
                string printerStatus = printer != null ? PrinterHelper.GetPrinterStatus(printer) : "Ready";
                string printerLocation = printer != null && !string.IsNullOrEmpty(printer.Location) ? printer.Location : "Unknown";
                string printerComment = printer != null && !string.IsNullOrEmpty(printer.Comment) ? printer.Comment : "Unknown";

                bool printerOffline = true;
                int printerJobs = 0;
                try
                {
                    printerOffline = printer != null && printer.IsOffline;
                    printerJobs = printer != null ? printer.NumberOfJobs : 0;
                }
                catch { }

                Grid itemContainer = new Grid();
                itemContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(55) });
                itemContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10) });
                itemContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                System.Windows.Media.ImageSource itemIconSource = printer != null ? PrinterHelper.GetPrinterIcon(printer, _printServerCollectionFax) : new System.Windows.Media.Imaging.BitmapImage(PrinterHelper.AddPrinterIcon);
                Image itemIcon = new Image() { Width = 55, Height = 55, Source = itemIconSource, Stretch = System.Windows.Media.Stretch.Fill, Opacity = printerOffline ? 0.5 : 1 };
                itemIcon.SetValue(Grid.ColumnProperty, 0);
                StackPanel itemInfo = new StackPanel() { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center };
                itemInfo.Children.Add(new TextBlock() { Text = printer != null ? printerName : "Add New Printer", FontSize = 14 });
                if (printer != null)
                {
                    itemInfo.Children.Add(new TextBlock() { Text = printerStatus, Margin = new Thickness(0, 7, 0, 0) });
                }
                itemInfo.SetValue(Grid.ColumnProperty, 2);
                itemContainer.Children.Add(itemIcon);
                itemContainer.Children.Add(itemInfo);

                string itemTooltip = printer != null ? $"Name: {printerName}\nStatus: {printerStatus}\nDocuments: {printerJobs}\nLoction: {printerLocation}\nComment: {printerComment}" : "Add New Printer";
                return new ComboBoxItem() { Content = itemContainer, Padding = new Thickness(10), ToolTip = itemTooltip, Tag = printer };
            }

            int optionPrinterSelectedIndex = 0;
            optionPrinter.Items.Clear();
            PrintQueue printerDefault = _printerDefault ?? PrinterHelper.GetDefaultPrinter();
            foreach (PrintQueue printer in _printServer.GetPrintQueues())
            {
                optionPrinter.Items.Add(actionCreateItem(printer));
                if (printerSelected != null ? printer.FullName == printerSelected.FullName : (printerDefault != null && printer.FullName == printerDefault.FullName))
                {
                    optionPrinterSelectedIndex = optionPrinter.Items.Count - 1;
                }
            }
            if (optionPrinter.Items.Count == 0)
            {
                CreateErrorPopup("No printer is detected!", () =>
                {
                    _owner.ReturnValue = false;
                    Window.GetWindow(this).Close();
                });
            }
            else if (_allowAddNewPrinter)
            {
                optionPrinter.Items.Add(actionCreateItem(null));
            }
            optionPrinter.SelectedIndex = optionPrinterSelectedIndex;
        }

        private void LoadPrinterSettings(bool isInitializing)
        {
            _isRefreshRequested = false;

            PrintQueue printer = (optionPrinter.SelectedItem as ComboBoxItem).Tag as PrintQueue;
            OutputColor? colorSelected = !isInitializing ? (optionColor.SelectedItem != null ? (optionColor.SelectedItem as ComboBoxItem).Tag as OutputColor? : SettingsHelper.ConvertPageColor(_settingsDefault.Color)) : null;
            OutputQuality? qualitySelected = !isInitializing ? (optionQuality.SelectedItem != null ? (optionQuality.SelectedItem as ComboBoxItem).Tag as OutputQuality? : SettingsHelper.ConvertPageQuality(_settingsDefault.Quality)) : null;
            PageMediaSize sizeSelected = !isInitializing ? (optionSize.SelectedItem != null ? (optionSize.SelectedItem as ComboBoxItem).Tag as PageMediaSize : SettingsHelper.ConvertPageSize(_settingsDefault.PageSize)) : null;
            PageMediaType? typeSelected = !isInitializing ? (optionType.SelectedItem != null ? (optionType.SelectedItem as ComboBoxItem).Tag as PageMediaType? : SettingsHelper.ConvertPageType(_settingsDefault.PageType)) : null;
            InputBin? sourceSelected = !isInitializing ? (optionSource.SelectedItem != null ? (optionSource.SelectedItem as ComboBoxItem).Tag as InputBin? : InputBin.AutoSelect) : null;

            PrintTicket printerDefaults = new PrintTicket();
            try
            {
                printerDefaults = printer.DefaultPrintTicket;
            }
            catch { }

            try
            {
                PrintCapabilities printerCapabilities = printer.GetPrintCapabilities();

                optionCopies.Maximum = printerCapabilities.MaxCopyCount ?? 1;
                optionCollate.Visibility = optionCopies.Maximum > 1 && optionCopies.Value > 1 ? Visibility.Visible : Visibility.Collapsed;

                int optionColorSelectedIndex = 0;
                optionColor.Items.Clear();
                foreach (OutputColor color in printerCapabilities.OutputColorCapability)
                {
                    optionColor.Items.Add(new ComboBoxItem() { Content = color.ToString(), Tag = color });
                    if (!isInitializing && colorSelected.HasValue && color == colorSelected.Value)
                    {
                        optionColorSelectedIndex = optionColor.Items.Count - 1;
                    }
                    else if (isInitializing && (!_settingsDefault.UsePrinterDefaultSettings ? color.ToString() == _settingsDefault.Color.ToString() : color == printerDefaults.OutputColor))
                    {
                        optionColorSelectedIndex = optionColor.Items.Count - 1;
                    }
                }
                if (!optionColor.HasItems)
                {
                    optionColor.Items.Add(new ComboBoxItem() { Content = _settingsDefault.Color.ToString(), Tag = SettingsHelper.ConvertPageColor(_settingsDefault.Color) });
                }
                optionColor.SelectedIndex = optionColorSelectedIndex;

                int optionQualitySelectedIndex = 0;
                optionQuality.Items.Clear();
                foreach (OutputQuality quality in printerCapabilities.OutputQualityCapability)
                {
                    optionQuality.Items.Add(new ComboBoxItem() { Content = quality.ToString(), Tag = quality });
                    if (!isInitializing && qualitySelected.HasValue && quality == qualitySelected.Value)
                    {
                        optionQualitySelectedIndex = optionQuality.Items.Count - 1;
                    }
                    else if (isInitializing && (!_settingsDefault.UsePrinterDefaultSettings ? quality.ToString() == _settingsDefault.Quality.ToString() : quality == printerDefaults.OutputQuality))
                    {
                        optionQualitySelectedIndex = optionQuality.Items.Count - 1;
                    }
                }
                if (!optionQuality.HasItems)
                {
                    optionQuality.Items.Add(new ComboBoxItem() { Content = _settingsDefault.Quality.ToString(), Tag = SettingsHelper.ConvertPageQuality(_settingsDefault.Quality) });
                }
                optionQuality.SelectedIndex = optionQualitySelectedIndex;

                switch (_allowDoubleSided ? (!_settingsDefault.UsePrinterDefaultSettings ? SettingsHelper.ConvertDoubleSided(_settingsDefault.DoubleSided) : (printerDefaults.Duplexing ?? Duplexing.OneSided)) : Duplexing.OneSided)
                {
                    case Duplexing.TwoSidedLongEdge:
                        optionDoubleSided.IsChecked = true;
                        optionDoubleSidedType.SelectedIndex = 0;
                        break;
                    case Duplexing.TwoSidedShortEdge:
                        optionDoubleSided.IsChecked = true;
                        optionDoubleSidedType.SelectedIndex = 1;
                        break;
                    default:
                        optionDoubleSided.IsChecked = false;
                        break;
                }

                int optionSizeSelectedIndex = 0;
                optionSize.Items.Clear();
                using (System.IO.MemoryStream printCapabilitiesXml = printer.GetPrintCapabilitiesAsXml())
                {
                    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                    xmlDocument.Load(printCapabilitiesXml);

                    System.Xml.XmlNamespaceManager xmlManager = new System.Xml.XmlNamespaceManager(xmlDocument.NameTable);
                    xmlManager.AddNamespace(xmlDocument.DocumentElement.Prefix, xmlDocument.DocumentElement.NamespaceURI);

                    foreach (System.Xml.XmlNode optionNode in xmlDocument.SelectNodes("//psf:Feature[@name='psk:PageMediaSize']/psf:Option", xmlManager))
                    {
                        // Determine the actual prefix of the Printer Schema Keywords namespace.
                        // Include the separator character to simplify string manipulation.
                        string pskPrefix = optionNode.GetPrefixOfNamespace("http://schemas.microsoft.com/windows/2003/08/printing/printschemakeywords");

                        pskPrefix += ":";

                        var properties = ParseOptionProperties(optionNode, xmlManager);

                        string name = optionNode.Attributes["name"]?.Value;

                        // Only process a node if it has all the bits we need to make sense of it.
                        if ((name != null) && properties.ContainsKey("MediaSizeWidth") && properties.ContainsKey("MediaSizeHeight"))
                        {
                            // Trim off the psk: prefix. Note that it could be something other than "psk:";
                            // the document isn't required to use any particular prefix. Only the URI is
                            // required to match.
                            if (!name.StartsWith(pskPrefix))
                            {
                                name = name.Substring(pskPrefix.Length);
                            }

                            PageMediaSizeName sizeName = SettingsHelper.GetPageSizeName(name);

                            // These properties must be of type integer per the specification.
                            int width = Convert.ToInt32(properties["MediaSizeWidth"]);
                            int height = Convert.ToInt32(properties["MediaSizeHeight"]);

                            // Use the display name provided by the driver, if there is one.
                            // Fall back to the description for the PageMediaSizeName enum value, if there is one.
                            // Fall back to just emitting the size in cm.
                            string displayName = properties.ContainsKey("DisplayName")
                                ? properties["DisplayName"].ToString()
                                : (sizeName != PageMediaSizeName.Unknown ? SettingsHelper.GetPageSizeName(sizeName) : $"Custom: {Math.Round(width / 10000.0, 2)} × {Math.Round(height / 10000.0, 2)} cm");

                            const double DIPsPerMicron = (96 / 25.4 / 1000);

                            PageMediaSize size = new PageMediaSize(sizeName, width * DIPsPerMicron, height * DIPsPerMicron);

                            optionSize.Items.Add(new ComboBoxItem() { Content = displayName, Tag = size });

                            if (!isInitializing && sizeSelected != null && sizeSelected.PageMediaSizeName != PageMediaSizeName.Unknown && size.PageMediaSizeName == sizeSelected.PageMediaSizeName)
                            {
                                optionSizeSelectedIndex = optionSize.Items.Count - 1;
                            }
                            else if (isInitializing && (!_settingsDefault.UsePrinterDefaultSettings ? size.PageMediaSizeName.ToString() == _settingsDefault.PageSize.ToString() : (size.PageMediaSizeName == printerDefaults.PageMediaSize.PageMediaSizeName || (size.Width == printerDefaults.PageMediaSize.Width && size.Height == printerDefaults.PageMediaSize.Height))))
                            {
                                optionSizeSelectedIndex = optionSize.Items.Count - 1;
                            }
                        }
                    }
                }
                if (!optionSize.HasItems)
                {
                    optionSize.Items.Add(new ComboBoxItem() { Content = SettingsHelper.GetPageSizeName(_settingsDefault.PageSize), Tag = SettingsHelper.ConvertPageSize(_settingsDefault.PageSize) });
                }
                optionSize.SelectedIndex = optionSizeSelectedIndex;

                int optionTypeSelectedIndex = 0;
                optionType.Items.Clear();
                foreach (PageMediaType type in printerCapabilities.PageMediaTypeCapability)
                {
                    optionType.Items.Add(new ComboBoxItem() { Content = SettingsHelper.GetPageTypeName(type), Tag = type });
                    if (!isInitializing && typeSelected.HasValue && type.ToString() == typeSelected.Value.ToString())
                    {
                        optionTypeSelectedIndex = optionType.Items.Count - 1;
                    }
                    else if (isInitializing && (!_settingsDefault.UsePrinterDefaultSettings ? type.ToString() == _settingsDefault.PageType.ToString() : type == printerDefaults.PageMediaType))
                    {
                        optionTypeSelectedIndex = optionType.Items.Count - 1;
                    }
                }
                if (!optionType.HasItems)
                {
                    optionType.Items.Add(new ComboBoxItem() { Content = SettingsHelper.GetPageTypeName(_settingsDefault.PageType), Tag = SettingsHelper.ConvertPageType(_settingsDefault.PageType) });
                }
                optionType.SelectedIndex = optionTypeSelectedIndex;

                int optionSourceSelectedIndex = 0;
                optionSource.Items.Clear();
                foreach (InputBin source in printerCapabilities.InputBinCapability)
                {
                    optionSource.Items.Add(new ComboBoxItem() { Content = SettingsHelper.GetInputBinName(source), Tag = source });
                    if (!isInitializing && sourceSelected.HasValue && source.ToString() == sourceSelected.Value.ToString())
                    {
                        optionSourceSelectedIndex = optionSource.Items.Count - 1;
                    }
                    else if (isInitializing && source == printerDefaults.InputBin)
                    {
                        optionSourceSelectedIndex = optionSource.Items.Count - 1;
                    }
                }
                if (!optionSource.HasItems)
                {
                    optionSource.Items.Add(new ComboBoxItem() { Content = SettingsHelper.GetInputBinName(InputBin.AutoSelect), Tag = InputBin.AutoSelect });
                }
                optionSource.SelectedIndex = optionSourceSelectedIndex;
            }
            catch { }

            _isRefreshRequested = true;
            RefreshDocument();
        }

        private static object ParseValue(XmlNode valueNode)
        {
            // Get the actual prefix of the XML Schema namespace. This is embedded into string
            // values, so we can't just use the XmlDocument URI-based front end or assign our
            // own prefix with XmlNamespaceManager.
            string schemaPrefix = valueNode.GetPrefixOfNamespace("http://www.w3.org/2001/XMLSchema");

            schemaPrefix += ":";

            var typeAttribute = valueNode.Attributes["type", "http://www.w3.org/2001/XMLSchema-instance"];

            if ((typeAttribute != null) && typeAttribute.Value.StartsWith(schemaPrefix))
            {
                string typeName = typeAttribute.Value.Substring(schemaPrefix.Length);

                string valueString = valueNode.InnerText;

                switch (typeName)
                {
                    case "string": return valueString;
                    case "boolean": return XmlConvert.ToBoolean(valueString);
                    case "decimal": return XmlConvert.ToDecimal(valueString);
                    case "integer": return XmlConvert.ToInt32(valueString);
                    case "float": return XmlConvert.ToSingle(valueString);
                    case "double": return XmlConvert.ToDouble(valueString);
                    case "date":
                    case "time":
                    case "dateTime": return XmlConvert.ToDateTime(valueString, XmlDateTimeSerializationMode.Unspecified);
                    case "duration": return XmlConvert.ToTimeSpan(valueString);
                    case "anyURI": return new Uri(valueString);
                    case "QName": throw new NotSupportedException();
                }
            }

            throw new NotSupportedException();
        }

        private static Dictionary<string, object> ParseOptionProperties(System.Xml.XmlNode optionNode, System.Xml.XmlNamespaceManager xmlManager)
        {
            // Determine the actual prefix of the Printer Schema Keywords namespace.
            // Include the separator character to simplify string manipulation.
            string pskPrefix = optionNode.GetPrefixOfNamespace("http://schemas.microsoft.com/windows/2003/08/printing/printschemakeywords");

            pskPrefix += ":";

            var ret = new Dictionary<string, object>();

            foreach (System.Xml.XmlNode propertyNode in optionNode.SelectNodes("psf:Property|psf:ScoredProperty", xmlManager))
            {
                var attribute = propertyNode.Attributes["name"];

                // Only process properties in the Printer Schema Keyword namespace.
                // Some drivers add extra things, such as PaperID, XOffset and
                // YOffset provided by Samsung drivers. These are in different
                // namespaces.
                if ((attribute != null) && attribute.Value.StartsWith(pskPrefix))
                {
                    string propertyName = attribute.Value.Substring(pskPrefix.Length);

                    var valueNode = propertyNode.SelectSingleNode("psf:Value", xmlManager);

                    if (valueNode != null)
                    {
                        try
                        {
                            ret[propertyName] = ParseValue(valueNode);
                        }
                        catch (NotSupportedException) { }
                    }
                }
            }

            return ret;
        }

        private void RefreshDocument()
        {
            if (!_isLoadingCompleted || !_isRefreshRequested)
            {
                return;
            }

            (previewer.Template.FindName("PART_LoadingOverlay", previewer) as FrameworkElement).Visibility = Visibility.Visible;
            Common.DoEvents();

            try
            {
                PrintQueue printer = (optionPrinter.SelectedItem as ComboBoxItem).Tag as PrintQueue;

                System.Windows.Documents.FixedDocument document = new System.Windows.Documents.FixedDocument();
                PageMediaSize documentSize = (optionSize.SelectedItem as ComboBoxItem).Tag as PageMediaSize;
                document.DocumentPaginator.PageSize = optionOrientation.SelectedIndex > 0 ? new Size(documentSize.Height.Value, documentSize.Width.Value) : new Size(documentSize.Width.Value, documentSize.Height.Value);

                List<int> documentPages = new List<int>();
                switch (optionPages.SelectedIndex)
                {
                    case 1:
                        documentPages.Add(GetCurrentPageIndex());
                        break;
                    case 2:
                        foreach (string pagesPart in optionPagesCustom.Text.Split(','))
                        {
                            if (pagesPart.Contains("-"))
                            {
                                string[] pagesRange = pagesPart.Split('-');
                                if (pagesRange.Length < 2 || !int.TryParse(pagesRange[0], out int numberStart) || !int.TryParse(pagesRange[1], out int numberEnd) || numberStart > numberEnd || numberStart <= 0 || numberEnd > _originalDocument.Count)
                                {
                                    CreateErrorPopup("The value for custom pages is invalid!", () => documentPages.Clear());
                                    break;
                                }
                                else
                                {
                                    documentPages.AddRange(System.Linq.Enumerable.Range(numberStart, numberEnd - numberStart + 1));
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(pagesPart))
                            {
                                if (!int.TryParse(pagesPart, out int number) || number <= 0 || number > _originalDocument.Count)
                                {
                                    CreateErrorPopup("The value for custom pages is invalid!", () => documentPages.Clear());
                                    break;
                                }
                                else
                                {
                                    documentPages.Add(number);
                                }
                            }
                        }
                        break;
                }
                if (documentPages.Count == 0)
                {
                    documentPages.AddRange(System.Linq.Enumerable.Range(1, _originalDocument.Count));
                }
                documentPages.Sort();

                OutputColor documentColor = (optionColor.SelectedItem as ComboBoxItem).Tag as OutputColor? ?? OutputColor.Color;
                (previewer.Template.FindName("PART_ContentHost", previewer) as ScrollViewer).Effect = documentColor == OutputColor.Grayscale || documentColor == OutputColor.Monochrome ? new Effects.GrayscaleEffect() : null;

                double documentScale = double.NaN;
                switch (optionScale.SelectedIndex)
                {
                    case 1:
                        documentScale = 25;
                        break;
                    case 2:
                        documentScale = 50;
                        break;
                    case 3:
                        documentScale = 75;
                        break;
                    case 4:
                        documentScale = 100;
                        break;
                    case 5:
                        documentScale = 150;
                        break;
                    case 6:
                        documentScale = 200;
                        break;
                    case 7:
                        documentScale = (int)optionScaleCustom.Value;
                        break;
                };
                double documentMargin = _originalDocumentMargin;
                switch (optionMargin.SelectedIndex)
                {
                    case 1:
                        documentMargin = 0;
                        break;
                    case 2:
                        documentMargin = PrinterHelper.GetPrinterMinimumMargin(printer, documentSize);
                        break;
                    case 3:
                        documentMargin = (int)optionMarginCustom.Value;
                        break;
                };

                PrintDialog.DocumentInfo documentInfo = new PrintDialog.DocumentInfo(documentPages.ToArray(), (PrintSettings.PageOrientation)optionOrientation.SelectedIndex, (PrintSettings.PageColor)((int)documentColor - 1), (PrintSettings.PagesPerSheet)optionPagesPerSheet.SelectedIndex, (PrintSettings.PageOrder)optionPageOrder.SelectedIndex, documentScale, documentMargin, new Size(documentSize.Width.Value, documentSize.Height.Value));
                if (_documentUpdateCallback != null)
                {
                    _originalDocument.Clear();
                    _originalDocument.AddRange(_documentUpdateCallback(documentInfo));
                }

                List<System.Windows.Documents.FixedPage> documentContent = new List<System.Windows.Documents.FixedPage>();
                for (int i = 0; i < _originalDocument.Count; i++)
                {
                    if (!documentPages.Contains(i + 1))
                    {
                        continue;
                    }

                    FrameworkElement element = _originalDocument[i].Content;
                    DependencyObject elementParent = System.Windows.Media.VisualTreeHelper.GetParent(element);
                    if (elementParent != null && elementParent is Decorator parentDecorator)
                    {
                        parentDecorator.Child = null;
                    }
                    else if (elementParent != null)
                    {
                        throw new PrintDocumentException("The content of PrintPage is already the child of another element.");
                    }

                    System.Windows.Documents.FixedPage page = new System.Windows.Documents.FixedPage() { Width = document.DocumentPaginator.PageSize.Width, Height = document.DocumentPaginator.PageSize.Height, IsHitTestVisible = false };
                    System.Windows.Media.RenderOptions.SetBitmapScalingMode(page, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
                    Decorator elementContainer = new Decorator() { Child = element, Width = _originalDocumentSize.Width - _originalDocumentMargin * 2, Height = _originalDocumentSize.Height - _originalDocumentMargin * 2, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
                    Decorator elementBoundary = new Decorator() { Child = elementContainer, Width = page.Width - documentMargin * 2, Height = page.Height - documentMargin * 2 };
                    if (optionPagesPerSheet.SelectedIndex <= 0)
                    {
                        double factor = optionScale.SelectedIndex <= 0 ? (_originalDocumentSize.Height * (page.Width / _originalDocumentSize.Width) <= page.Height ? page.Width / _originalDocumentSize.Width : page.Height / _originalDocumentSize.Height) : documentScale / 100.0;
                        elementContainer.LayoutTransform = new System.Windows.Media.ScaleTransform(factor, factor);
                        elementContainer.RenderTransformOrigin = new Point(0, 0);
                        elementBoundary.Width = page.Width - documentMargin * factor * 2;
                        elementBoundary.Height = page.Height - documentMargin * factor * 2;
                        System.Windows.Documents.FixedPage.SetLeft(elementBoundary, documentMargin * factor);
                        System.Windows.Documents.FixedPage.SetTop(elementBoundary, documentMargin * factor);
                    }
                    else
                    {
                        System.Windows.Documents.FixedPage.SetLeft(elementBoundary, documentMargin);
                        System.Windows.Documents.FixedPage.SetTop(elementBoundary, documentMargin);
                    }
                    page.Children.Add(elementBoundary);
                    documentContent.Add(page);
                }
                if (optionPagesPerSheet.SelectedIndex > 0)
                {
                    _previewDocument = DocumentHelper.GetPagesPerSheetDocument(documentContent, document.DocumentPaginator.PageSize, documentInfo);
                }
                else
                {
                    foreach (System.Windows.Documents.FixedPage page in documentContent)
                    {
                        document.Pages.Add(new System.Windows.Documents.PageContent() { Child = page });
                    }
                    _previewDocument = document;
                }

                System.Windows.Documents.FixedDocumentSequence sequence = new System.Windows.Documents.FixedDocumentSequence();
                System.Windows.Documents.DocumentReference reference = new System.Windows.Documents.DocumentReference();
                reference.SetDocument(_previewDocument);
                sequence.References.Add(reference);
                previewer.Document = sequence;
            }
            catch (PrintDocumentException)
            {
                throw;
            }
            catch { }

            (previewer.Template.FindName("PART_LoadingOverlay", previewer) as FrameworkElement).Visibility = Visibility.Collapsed;
            Common.DoEvents();
        }

        private void PrintDocument()
        {
            if (optionPrinter.SelectedItem == null)
            {
                return;
            }

            RefreshDocument();

            PrintQueue printer = (optionPrinter.SelectedItem as ComboBoxItem).Tag as PrintQueue;
            System.Windows.Controls.PrintDialog systemPrintDialog = new System.Windows.Controls.PrintDialog();

            PrintTicket printTicket = systemPrintDialog.PrintTicket;
            printTicket.CopyCount = (int)optionCopies.Value;
            printTicket.Collation = optionCollate.IsChecked.HasValue && optionCollate.IsChecked.Value ? Collation.Collated : Collation.Uncollated;
            printTicket.PageOrientation = optionOrientation.SelectedIndex > 0 ? PageOrientation.Landscape : PageOrientation.Portrait;
            printTicket.OutputColor = (optionColor.SelectedItem as ComboBoxItem).Tag as OutputColor?;
            printTicket.OutputQuality = (optionQuality.SelectedItem as ComboBoxItem).Tag as OutputQuality?;
            printTicket.PagesPerSheet = 1;
            printTicket.PagesPerSheetDirection = PagesPerSheetDirection.RightBottom;
            printTicket.PageScalingFactor = 100;
            printTicket.Duplexing = optionDoubleSided.IsChecked.HasValue && optionDoubleSided.IsChecked.Value ? (optionDoubleSidedType.SelectedIndex > 0 ? Duplexing.TwoSidedShortEdge : Duplexing.TwoSidedLongEdge) : Duplexing.OneSided;
            printTicket.PageMediaSize = (optionSize.SelectedItem as ComboBoxItem).Tag as PageMediaSize;
            printTicket.PageMediaType = (optionType.SelectedItem as ComboBoxItem).Tag as PageMediaType?;
            printTicket.InputBin = (optionSource.SelectedItem as ComboBoxItem).Tag as InputBin?;

            _owner.TotalPapers = optionDoubleSided.IsChecked.HasValue && !optionDoubleSided.IsChecked.Value ? previewer.PageCount * (int)optionCopies.Value : (int)Math.Ceiling(previewer.PageCount * (int)optionCopies.Value / 2.0);
            systemPrintDialog.PrintQueue = printer;
            systemPrintDialog.PrintDocument(_previewDocument.DocumentPaginator, _originalDocumentName);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            buttonPrinterPreference.Visibility = _allowPrinterPreferences ? Visibility.Visible : Visibility.Collapsed;
            optionPages.Visibility = _allowPages ? Visibility.Visible : Visibility.Collapsed;
            optionPagesLabel.Visibility = _allowPages ? Visibility.Visible : Visibility.Collapsed;
            optionScale.Visibility = _allowScale ? Visibility.Visible : Visibility.Collapsed;
            optionScale.SelectedIndex = _allowScale ? 0 : 4;
            optionScaleLabel.Visibility = _allowScale ? Visibility.Visible : Visibility.Collapsed;
            optionDoubleSided.Visibility = _allowDoubleSided ? Visibility.Visible : Visibility.Collapsed;
            optionDoubleSidedLabel.Visibility = _allowDoubleSided ? Visibility.Visible : Visibility.Collapsed;
            optionPagesPerSheet.Visibility = _allowPagesPerSheet ? Visibility.Visible : Visibility.Collapsed;
            optionPagesPerSheetLabel.Visibility = _allowPagesPerSheet ? Visibility.Visible : Visibility.Collapsed;
            optionPageOrder.Visibility = _allowPagesPerSheet && _allowPageOrder ? Visibility.Visible : Visibility.Collapsed;
            optionPageOrderLabel.Visibility = _allowPagesPerSheet && _allowPageOrder ? Visibility.Visible : Visibility.Collapsed;
            this.UpdateLayout();
            Common.DoEvents();

            LoadPrinters(_printerDefault);
            PrintQueue printer = (optionPrinter.SelectedItem as ComboBoxItem).Tag as PrintQueue;
            optionPrinterPreviewIcon.Source = PrinterHelper.GetPrinterIcon(printer, _printServerCollectionFax, true);
            optionPrinterPreviewText.Text = printer.FullName;
            LoadPrinterSettings(true);

            optionOrientation.SelectedIndex = _settingsDefault.Layout == PrintSettings.PageOrientation.Portrait ? 0 : 1;
            optionPagesPerSheet.SelectedIndex = (int)_settingsDefault.PagesPerSheet;
            optionPageOrder.SelectedIndex = (int)_settingsDefault.PageOrder;
            optionMarginCustom.Maximum = (int)Math.Min(_originalDocumentSize.Width / 2, _originalDocumentSize.Height / 2) - 15;
            optionMarginCustom.Value = _originalDocumentMargin;

            _isLoadingCompleted = true;
            Common.DoEvents();

            RefreshDocument();
            previewer.FitToWidth();

            (previewer.Template.FindName("PART_LoadingOverlay", previewer) as FrameworkElement).Visibility = Visibility.Collapsed;
            Common.DoEvents();
            previewer.FitToWidth();
            Common.DoEvents();

            (previewer.Template.FindName("PART_PageInfo", previewer) as TextBlock).Text = $"Page {GetCurrentPageIndex()} / {previewer.PageCount}";
            buttonPrint.Focus();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _printServer.Dispose();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            buttonCancel.IsEnabled = false;
            buttonPrint.IsEnabled = false;
            Common.DoEvents();

            PrintDocument();

            _owner.ReturnValue = true;
            Window.GetWindow(this).Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            buttonCancel.IsEnabled = false;
            buttonPrint.IsEnabled = false;
            Common.DoEvents();

            _owner.ReturnValue = false;
            Window.GetWindow(this).Close();
        }

        private void PrinterPreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C rundll32 printui.dll,PrintUIEntry /p /n \"{((optionPrinter.SelectedItem as ComboBoxItem).Tag as PrintQueue).FullName}\"",
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                });
            }
            catch
            {
                CreateErrorPopup("Unable to open the printer's preferences dialog!", null);
            }
        }

        private void PrinterComboBox_DropDownOpened(object sender, EventArgs e)
        {
            _isRefreshRequested = false;

            _printServer.Refresh();
            LoadPrinters((optionPrinter.SelectedItem as ComboBoxItem).Tag as PrintQueue);

            _isRefreshRequested = true;
        }

        private void PrinterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isRefreshRequested || optionPrinter.SelectedItem == null)
            {
                return;
            }

            if (_allowAddNewPrinter && (optionPrinter.SelectedItem as ComboBoxItem).Tag == null)
            {
                optionPrinter.IsDropDownOpen = false;
                if (e.RemovedItems.Count > 0)
                {
                    optionPrinter.SelectedItem = e.RemovedItems[0];
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "ms-settings:printers",
                    UseShellExecute = true
                });
            }
            else
            {
                PrintQueue printer = (optionPrinter.SelectedItem as ComboBoxItem).Tag as PrintQueue;
                optionPrinterPreviewIcon.Source = PrinterHelper.GetPrinterIcon(printer, _printServerCollectionFax, true);
                optionPrinterPreviewText.Text = printer.FullName;
                LoadPrinterSettings(false);
            }
        }

        private void SettingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isRefreshRequested)
            {
                return;
            }
            else if (sender is ComboBox comboBox)
            {
                comboBox.IsDropDownOpen = false;
            }

            optionPages.SelectedIndex = optionPagesPerSheet.SelectedIndex > 0 && (optionPages.Items[1] as ComboBoxItem).IsSelected ? 0 : optionPages.SelectedIndex;
            (optionPages.Items[1] as ComboBoxItem).Visibility = optionPagesPerSheet.SelectedIndex <= 0 ? Visibility.Visible : Visibility.Collapsed;
            optionPagesCustom.Visibility = optionPages.SelectedIndex > 1 ? Visibility.Visible : Visibility.Collapsed;
            optionScaleCustom.Visibility = optionScale.SelectedIndex > 6 ? Visibility.Visible : Visibility.Collapsed;
            optionMargin.IsEnabled = optionPagesPerSheet.SelectedIndex <= 0;
            optionMarginCustom.IsEnabled = optionPagesPerSheet.SelectedIndex <= 0;
            optionMarginCustom.Visibility = optionMargin.SelectedIndex > 2 ? Visibility.Visible : Visibility.Collapsed;
            RefreshDocument();
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
            (previewer.Template.FindName("PART_PageInfo", previewer) as TextBlock).Text = $"Page {GetCurrentPageIndex()} / {previewer.PageCount}";
        }

        private void FirstPageButton_Click(object sender, RoutedEventArgs e)
        {
            previewer.FirstPage();
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            previewer.PreviousPage();
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            previewer.NextPage();
        }

        private void LastPageButton_Click(object sender, RoutedEventArgs e)
        {
            previewer.LastPage();
        }

        private void ActualSizeButton_Click(object sender, RoutedEventArgs e)
        {
            previewer.FitToMaxPagesAcross(1);
            previewer.Zoom = 100.0;
        }

        private void CopiesNumberPicker_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (_isRefreshRequested)
            {
                if (!optionCopies.Value.HasValue)
                {
                    optionCopies.Value = 1;
                }
                optionCollate.Visibility = optionCopies.Value > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void CustomZoomNumberPicker_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (_isRefreshRequested)
            {
                if (!optionScaleCustom.Value.HasValue)
                {
                    optionScaleCustom.Value = 100;
                }
                RefreshDocument();
            }
        }

        private void CustomMarginNumberPicker_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (_isRefreshRequested)
            {
                if (!optionMarginCustom.Value.HasValue)
                {
                    optionMarginCustom.Value = _originalDocumentMargin;
                }
                RefreshDocument();
            }
        }

        private void CustomPagesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_isRefreshRequested)
            {
                RefreshDocument();
            }
        }
    }
}