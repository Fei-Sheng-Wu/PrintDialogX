using System;
using System.Printing;
using System.Reflection;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintDialogX.PrintDialog
{
    public class PrintDialogSettings
    {
        /// <summary>
        /// Initialize a <see cref="PrintDialogSettings"/> class.
        /// </summary>
        public PrintDialogSettings()
        {
            UsePrinterDefaultSettings = false;
            Layout = PrintSettings.PageOrientation.Portrait;
            Color = PrintSettings.PageColor.Color;
            Quality = PrintSettings.PageQuality.Normal;
            PageSize = PrintSettings.PageSize.ISOA4;
            PageType = PrintSettings.PageType.Plain;
            DoubleSided = PrintSettings.DoubleSided.OneSided;
            PagesPerSheet = 1;
            PageOrder = PrintSettings.PageOrder.Horizontal;
        }

        /// <summary>
        /// Use default settings of the printer or not.
        /// </summary>
        public bool UsePrinterDefaultSettings { get; internal set; } = false;

        /// <summary>
        /// Layout.
        /// </summary>
        public PrintSettings.PageOrientation Layout { get; set; } = PrintSettings.PageOrientation.Portrait;

        /// <summary>
        /// Color.
        /// </summary>
        public PrintSettings.PageColor Color { get; set; } = PrintSettings.PageColor.Color;

        /// <summary>
        /// Quality.
        /// </summary>
        public PrintSettings.PageQuality Quality { get; set; } = PrintSettings.PageQuality.Normal;

        /// <summary>
        /// Page size.
        /// </summary>
        public PrintSettings.PageSize PageSize { get; set; } = PrintSettings.PageSize.ISOA4;

        /// <summary>
        /// Page type.
        /// </summary>
        public PrintSettings.PageType PageType { get; set; } = PrintSettings.PageType.Plain;

        /// <summary>
        /// Double-sided or not.
        /// </summary>
        public PrintSettings.DoubleSided DoubleSided { get; set; } = PrintSettings.DoubleSided.OneSided;

        /// <summary>
        /// Pages per sheet.
        /// </summary>
        public int PagesPerSheet { get; set; } = 1;

        /// <summary>
        /// Page order.
        /// </summary>
        public PrintSettings.PageOrder PageOrder { get; set; } = PrintSettings.PageOrder.Horizontal;

        /// <summary>
        /// Initialize a <see cref="PrintDialogSettings"/> that use printer default settings
        /// </summary>
        /// <returns>The <see cref="PrintDialogSettings"/> that use printer default settings.</returns>
        public static PrintDialogSettings PrinterDefaultSettings()
        {
            return new PrintDialogSettings()
            {
                UsePrinterDefaultSettings = true
            };
        }

        /// <summary>
        /// Change <see cref="PrintDialogSettings"/> property value.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">The value to change to.</param>
        /// <returns>The <see cref="PrintDialogSettings"/> after change.</returns>
        public PrintDialogSettings ChangePropertyValue(string propertyName, object propertyValue)
        {
            PropertyInfo property = this.GetType().GetProperty(propertyName, BindingFlags.Instance);

            if (property == null)
            {
                if (this.GetType().GetProperty(propertyName, BindingFlags.NonPublic) != null)
                {
                    throw new Exception("Can't change private, protected or internal property.");
                }

                throw new Exception("Can't find property. Please make sure the property name is correct.");
            }
            else
            {
                try
                {
                    property.SetValue(this, propertyValue);
                }
                catch
                {
                    throw new Exception("Can't change property's value.");
                }
            }

            return this;
        }
    }

    public class DocumentInfo
    {
        /// <summary>
        /// Initialize a <see cref="DocumentInfo"/> class.
        /// </summary>
        public DocumentInfo()
        {
            Scale = null;
            Size = null;
            Margin = null;
            Pages = null;
            PagesPerSheet = null;
            Color = null;
            PageOrder = null;
            Orientation = null;
        }

        /// <summary>
        /// Page size without consideration of orientation.
        /// </summary>
        public Size? Size { get; internal set; } = null;

        /// <summary>
        /// Pages that need to be displayed. The array include each page number.
        /// </summary>
        public int[] Pages { get; internal set; } = null;

        /// <summary>
        /// Page scale, a percentage value. <see cref="Double.NaN"/> results automatic scaling to fit.
        /// </summary>
        public double? Scale { get; internal set; } = null;

        /// <summary>
        /// Page margin.
        /// </summary>
        public double? Margin { get; internal set; } = null;

        /// <summary>
        /// Pages per sheet.
        /// </summary>
        public int? PagesPerSheet { get; internal set; } = null;

        /// <summary>
        /// Color.
        /// </summary>
        public PrintSettings.PageColor? Color { get; internal set; } = null;

        /// <summary>
        /// Page order.
        /// </summary>
        public PrintSettings.PageOrder? PageOrder { get; internal set; } = null;

        /// <summary>
        /// Page orientation.
        /// </summary>
        public PrintSettings.PageOrientation? Orientation { get; internal set; } = null;
    }

    public class PrintDialog
    {
        /// <summary>
        /// Initialize a <see cref="PrintDialog"/> class.
        /// </summary>
        public PrintDialog()
        {
            Owner = null;
            Title = "Print";
            Icon = null;
            Topmost = false;
            ShowInTaskbar = false;
            AllowScaleOption = true;
            AllowPagesOption = true;
            AllowDoubleSidedOption = true;
            AllowPageOrderOption = true;
            AllowPagesPerSheetOption = true;
            AllowAddNewPrinterButton = true;
            AllowMoreSettingsExpander = true;
            AllowPrinterPreferencesButton = true;
            Document = null;
            DocumentMargin = 0;
            DocumentName = "Untitled Document";
            ResizeMode = ResizeMode.NoResize;
            DefaultSettings = new PrintDialogSettings();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CustomReloadDocumentMethod = null;
        }

        /// <summary>
        /// The print dialog window.
        /// </summary>
        internal PrintControl.PrintWindow PrintWindow = null;

        /// <summary>
        /// <see cref="PrintDialog"/>'s owner window.
        /// </summary>
        public Window Owner { get; set; } = null;

        /// <summary>
        /// <see cref="PrintDialog"/>'s title.
        /// </summary>
        public string Title { get; set; } = "Print";

        /// <summary>
        /// <see cref="PrintDialog"/>'s icon. null results in the use of default icon.
        /// </summary>
        public ImageSource Icon { get; set; } = null;

        /// <summary>
        /// The document's margin info.
        /// </summary>
        public double DocumentMargin { get; set; } = 0;

        /// <summary>
        /// Allow <see cref="PrintDialog"/> to be at topmost or not.
        /// </summary>
        public bool Topmost { get; set; } = false;

        /// <summary>
        /// Alllow <see cref="PrintDialog"/> to show in taskbar or not.
        /// </summary>
        public bool ShowInTaskbar { get; set; } = false;

        /// <summary>
        /// Allow scale option or not.
        /// </summary>
        public bool AllowScaleOption { get; set; } = true;

        /// <summary>
        /// Allow pages option (contains "All Pages", "Current Page", and "Custom Pages") or not.
        /// </summary>
        public bool AllowPagesOption { get; set; } = true;

        /// <summary>
        /// Allow double-sided option or not.
        /// </summary>
        public bool AllowDoubleSidedOption { get; set; } = true;

        /// <summary>
        /// Allow page order option or not. Only works when pages per sheet option is allowed.
        /// </summary>
        public bool AllowPageOrderOption { get; set; } = true;

        /// <summary>
        /// Allow pages per sheet option or not.
        /// </summary>
        public bool AllowPagesPerSheetOption { get; set; } = true;

        /// <summary>
        /// Allow add new printer button in the printer list or not.
        /// </summary>
        public bool AllowAddNewPrinterButton { get; set; } = true;

        /// <summary>
        /// Allow the usage of an expander for more settings or just show all settings at once.
        /// </summary>
        public bool AllowMoreSettingsExpander { get; set; } = true;

        /// <summary>
        /// Allow printer preferences button or not.
        /// </summary>
        public bool AllowPrinterPreferencesButton { get; set; } = true;

        /// <summary>
        /// The document that needs to be printed.
        /// </summary>
        public FixedDocument Document { get; set; } = null;

        /// <summary>
        /// The document name that will be displayed.
        /// </summary>
        public string DocumentName { get; set; } = "Untitled Document";

        /// <summary>
        /// <see cref="PrintDialog"/>'s resize mode.
        /// </summary>
        public ResizeMode ResizeMode { get; set; } = ResizeMode.NoResize;

        /// <summary>
        /// The default settings.
        /// </summary>
        public PrintDialogSettings DefaultSettings { get; set; } = new PrintDialogSettings();

        /// <summary>
        /// <see cref="PrintDialog"/>'s startup location.
        /// </summary>
        public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.CenterScreen;

        /// <summary>
        /// The method that will use to get document when reload document. You can only change the content in the document. The method must return a list of <see cref="PageContent"/> that represents the page contents in order.
        /// </summary>
        public Func<DocumentInfo, List<PageContent>> CustomReloadDocumentMethod { get; set; } = null;

        /// <summary>
        /// The total number of papers that the printer will use.
        /// </summary>
        public int TotalPapers { get; internal set; } = 0;

        /// <summary>
        /// Show <see cref="PrintDialog"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="Document"/> or <see cref="DocumentName"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="DefaultSettings"/>'s pages per sheet value is invalid.</exception>
        /// <exception cref="PrintDialogExceptions.DocumentEmptyException"><see cref="Document"/> doesn't have pages.</exception>
        /// <exception cref="PrintDialogExceptions.UndefinedException"><see cref="PrintDialog"/> meet some undefined exceptions.</exception>
        /// <returns>A boolean value, where true means that the Print button is clicked, false means that the Cancel button is clicked, and null means <see cref="PrintDialog"/> can't be opened or there is already a running <see cref="PrintDialog"/>.</returns>
        [Obsolete]
        public bool? ShowDialog()
        {
            return ShowDialog(false, null);
        }

        /// <summary>
        /// Show <see cref="PrintDialog"/>.
        /// </summary>
        /// <param name="loading">Display loading page or not.</param>
        /// <param name="loadingAction">The method used to loading document if display loading page.</param>
        /// <exception cref="ArgumentNullException"><see cref="Document"/> or <see cref="DocumentName"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="DefaultSettings"/>'s pages per sheet value is invalid.</exception>
        /// <exception cref="PrintDialogExceptions.DocumentEmptyException"><see cref="Document"/> doesn't have pages.</exception>
        /// <exception cref="PrintDialogExceptions.UndefinedException"><see cref="PrintDialog"/> meet some undefined exceptions.</exception>
        /// <returns>A boolean value, true means Print button clicked, false means Cancel button clicked, null means can't open <see cref="PrintDialog"/> or there is already a running <see cref="PrintDialog"/>.</returns>
        public bool? ShowDialog(bool loading, Action loadingAction)
        {
            if (PrintWindow == null)
            {
                if (loading == false)
                {
                    if (Document == null)
                    {
                        throw new ArgumentNullException("The document is null.");
                    }
                    if (DocumentName == null)
                    {
                        throw new ArgumentNullException("The document name is null.");
                    }
                    if (PrintControl.PreviewHelper.MultiPagesPerSheetHelper.GetPagePerSheetCountIndex(DefaultSettings.PagesPerSheet) == -1)
                    {
                        throw new ArgumentException("The default settings's pages per sheet value is invalid. You can only set 1, 2, 4, 6, 9 or 16.");
                    }
                    if (Document.Pages.Count == 0)
                    {
                        throw new PrintDialogExceptions.DocumentEmptyException("PrintDialog can't print an empty document.", Document);
                    }
                }

                try
                {
                    PrintWindow = new PrintControl.PrintWindow(Document, DocumentName, DocumentMargin, DefaultSettings, AllowPagesOption, AllowScaleOption, AllowDoubleSidedOption, AllowPagesPerSheetOption, AllowPageOrderOption, AllowMoreSettingsExpander, AllowAddNewPrinterButton, AllowPrinterPreferencesButton, CustomReloadDocumentMethod, loading, loadingAction)
                    {
                        Title = Title,
                        Owner = Owner,
                        Topmost = Topmost,
                        ResizeMode = ResizeMode,
                        ShowInTaskbar = ShowInTaskbar,
                        WindowStartupLocation = WindowStartupLocation
                    };

                    if (Icon != null)
                    {
                        PrintWindow.Icon = Icon;
                    }

                    PrintWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    throw new PrintDialogExceptions.UndefinedException("PrintDialog meet some undefined exceptions.", ex);
                }

                bool returnValue = PrintWindow.ReturnValue;
                TotalPapers = PrintWindow.TotalPapers;

                PrintWindow = null;

                return returnValue;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Switch the current running <see cref="PrintDialog"/>'s page into settings and preview page.
        /// </summary>
        /// <exception cref="ArgumentNullException"><see cref="Document"/> or <see cref="DocumentName"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="DefaultSettings"/>'s pages per sheet value is invalid.</exception>
        /// <exception cref="PrintDialogExceptions.DocumentEmptyException"><see cref="Document"/> doesn't have pages.</exception>
        /// <exception cref="PrintDialogExceptions.UndefinedException"><see cref="PrintDialog"/> meet some undefined exceptions.</exception>
        public void LoadingEnd()
        {
            if (PrintWindow != null)
            {
                if (Document == null)
                {
                    throw new ArgumentNullException("The document is null.");
                }
                if (DocumentName == null)
                {
                    throw new ArgumentNullException("The document name is null.");
                }
                if (PrintControl.PreviewHelper.MultiPagesPerSheetHelper.GetPagePerSheetCountIndex(DefaultSettings.PagesPerSheet) == -1)
                {
                    throw new ArgumentException("The default settings's pages per sheet value is invalid. You can only set 1, 2, 4, 6, 9 or 16.");
                }
                if (Document.Pages.Count == 0)
                {
                    throw new PrintDialogExceptions.DocumentEmptyException("PrintDialog can't print an empty document.", Document);
                }

                PrintWindow.BeginSettingAndPreviewing(Document, DocumentName, DocumentMargin, DefaultSettings, AllowPagesOption, AllowScaleOption, AllowDoubleSidedOption, AllowPagesPerSheetOption, AllowPageOrderOption, AllowMoreSettingsExpander, AllowAddNewPrinterButton, AllowPrinterPreferencesButton, CustomReloadDocumentMethod);
            }
        }
    }
}

namespace PrintDialogX.PaperHelper
{
    public class PaperHelper
    {
        /// <summary>
        /// Initialize a <see cref="PaperHelper"/> class.
        /// </summary>
        protected PaperHelper()
        {
            return;
        }

        /// <summary>
        /// Get actual paper size.
        /// </summary>
        /// <param name="pageSizeName">Paper size name.</param>
        /// <returns>Paper size.</returns>
        [Obsolete]
        public static Size GetPaperSize(PageMediaSizeName pageSizeName)
        {
            System.Windows.Controls.PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
            PageMediaSize pageMediaSize = new PageMediaSize(pageSizeName);
            printDlg.PrintTicket.PageMediaSize = pageMediaSize;

            return new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight);
        }

        /// <summary>
        /// Get actual paper size.
        /// </summary>
        /// <param name="pageSizeName">Paper size name.</param>
        /// <param name="pageOrientation">Paper orientation.</param>
        /// <returns>Paper size.</returns>
        [Obsolete]
        public static Size GetPaperSize(PageMediaSizeName pageSizeName, PrintSettings.PageOrientation pageOrientation)
        {
            System.Windows.Controls.PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
            PageMediaSize pageMediaSize = new PageMediaSize(pageSizeName);
            printDlg.PrintTicket.PageMediaSize = pageMediaSize;

            if (pageOrientation == PrintSettings.PageOrientation.Portrait)
            {
                return new Size(printDlg.PrintableAreaWidth, printDlg.PrintableAreaHeight);
            }
            else
            {
                return new Size(printDlg.PrintableAreaHeight, printDlg.PrintableAreaWidth);
            }
        }

        /// <summary>
        /// Get actual paper size.
        /// </summary>
        /// <param name="CmWidth">Paper width in cm.</param>
        /// <param name="CmHeight">Paper height in cm.</param>
        /// <returns>Paper size.</returns>
        public static Size GetPaperSizeByCm(double cmWidth, double cmHeight)
        {
            double factor = 37.7952755905512;
            return new Size(cmWidth * factor / 10, cmHeight * factor / 10);
        }

        /// <summary>
        /// Get actual paper size.
        /// </summary>
        /// <param name="CmWidth">Paper width in cm.</param>
        /// <param name="CmHeight">Paper height in cm.</param>
        /// <returns>Paper size.</returns>
        public static Size GetPaperSizeByInch(double inchWidth, double inchHeight)
        {
            double factor = 96;
            return new Size(inchWidth * factor, inchHeight * factor);
        }
    }

    public class PaperSize
    {
        /// <summary>
        /// Initialize a <see cref="PaperSize"/> class.
        /// </summary>
        protected PaperSize()
        {
            return;
        }

        /// <summary>
        /// A1 paper width (mm).
        /// </summary>
        public static double A1Width { get; } = 594;

        /// <summary>
        /// A1 paper height (mm).
        /// </summary>
        public static double A1Height { get; } = 841;

        /// <summary>
        /// A2 paper width (mm).
        /// </summary>
        public static double A2Width { get; } = 420;

        /// <summary>
        /// A2 paper height (mm).
        /// </summary>
        public static double A2Height { get; } = 594;

        /// <summary>
        /// A3 paper width (mm).
        /// </summary>
        public static double A3Width { get; } = 297;

        /// <summary>
        /// A3 paper height (mm).
        /// </summary>
        public static double A3Height { get; } = 420;

        /// <summary>
        /// A4 paper width (mm).
        /// </summary>
        public static double A4Width { get; } = 210;

        /// <summary>
        /// A4 paper height (mm).
        /// </summary>
        public static double A4Height { get; } = 297;

        /// <summary>
        /// A5 paper width (mm).
        /// </summary>
        public static double A5Width { get; } = 148;

        /// <summary>
        /// A5 paper height (mm).
        /// </summary>
        public static double A5Height { get; } = 210;

        /// <summary>
        /// A6 paper width (mm).
        /// </summary>
        public static double A6Width { get; } = 105;

        /// <summary>
        /// A6 paper height (mm).
        /// </summary>
        public static double A6Height { get; } = 148;

        /// <summary>
        /// Search all width and height by name and return the result length. If can't find the length, it will return <see cref="Double.NaN"/>.
        /// </summary>
        /// <param name="name">The length name, allowed white-space and lowercase, like "A4 Height", "A4Height" and "a4 height" both means the A4 paper's height.</param>
        /// <returns>The length.</returns>
        public static double GetLengthWithName(string name)
        {
            foreach (PropertyInfo item in typeof(PaperSize).GetProperties(BindingFlags.Instance | BindingFlags.Static))
            {
                if (item.Name.ToLower() == name.Trim().ToLower())
                {
                    return (double)item.GetValue(new PaperSize());
                }
            }

            return double.NaN;
        }
    }
}

namespace PrintDialogX.PrinterHelper
{
    public class PrinterHelper
    {
        /// <summary>
        /// Initialize a <see cref="PrinterHelper"/> class.
        /// </summary>
        protected PrinterHelper()
        {
            return;
        }

        /// <summary>
        /// Get the printer by the printer name.
        /// </summary>
        /// <param name="printerName">The printer name.</param>
        /// <returns>The printer.</returns>
        public static PrintQueue GetPrinterByName(string printerName)
        {
            return new PrintServer().GetPrintQueue(printerName);
        }

        /// <summary>
        /// Get the printer by the printer name.
        /// </summary>
        /// <param name="printerName">The printer name.</param>
        /// <param name="printServer">The print server that used to get printer.</param>
        /// <returns>The printer.</returns>
        public static PrintQueue GetPrinterByName(string printerName, PrintServer printServer)
        {
            return printServer.GetPrintQueue(printerName);
        }

        /// <summary>
        /// Get local default printer.
        /// </summary>
        /// <returns>The printer.</returns>
        public static PrintQueue GetDefaultPrinter()
        {
            return LocalPrintServer.GetDefaultPrintQueue();
        }

        /// <summary>
        /// Get local printers.
        /// </summary>
        /// <returns>A collection of printers.</returns>
        public static PrintQueueCollection GetLocalPrinters()
        {
            return GetLocalPrinters(new PrintServer());
        }

        /// <summary>
        /// Get local printers.
        /// </summary>
        /// <param name="enumerationFlag">An array of values that represent the types of print queues that are in the collection.</param>
        /// <returns>A collection of printers.</returns>
        public static PrintQueueCollection GetLocalPrinters(EnumeratedPrintQueueTypes[] enumerationFlag)
        {
            return GetLocalPrinters(new PrintServer(), enumerationFlag);
        }

        /// <summary>
        /// Get local printers.
        /// </summary>
        /// <param name="server">The print server.</param>
        /// <returns>A collection of printers.</returns>
        public static PrintQueueCollection GetLocalPrinters(PrintServer server)
        {
            return server.GetPrintQueues();
        }

        /// <summary>
        /// Get local printers.
        /// </summary>
        /// <param name="server">The print server.</param>
        /// <param name="enumerationFlag">An array of values that represent the types of print queues that are in the collection.</param>
        /// <returns>A collection of printers.</returns>
        public static PrintQueueCollection GetLocalPrinters(PrintServer server, EnumeratedPrintQueueTypes[] enumerationFlag)
        {
            return server.GetPrintQueues(enumerationFlag);
        }

        /// <summary>
        /// Get printer's status info.
        /// </summary>
        /// <param name="printerName">Printer's name.</param>
        /// <returns>printer's status info.</returns>
        public static string GetPrinterStatusInfo(string printerName)
        {
            return GetPrinterStatusInfo(new PrintServer().GetPrintQueue(printerName));
        }

        /// <summary>
        /// Get printer's status info.
        /// </summary>
        /// <param name="printer">Printer.</param>
        /// <returns>printer's status info.</returns>
        public static string GetPrinterStatusInfo(PrintQueue printer)
        {
            printer.Refresh();

            return GetPrinterStatusInfo(printer.QueueStatus);
        }

        /// <summary>
        /// Get printer's status info.
        /// </summary>
        /// <param name="printerStatue">Printer's status.</param>
        /// <returns>printer's status info.</returns>
        public static string GetPrinterStatusInfo(PrintQueueStatus printerStatue)
        {
            return printerStatue switch
            {
                PrintQueueStatus.Busy => "Busy",
                PrintQueueStatus.DoorOpen => "Door Open",
                PrintQueueStatus.Error => "Error",
                PrintQueueStatus.Initializing => "Initializing",
                PrintQueueStatus.IOActive => "Exchanging Data",
                PrintQueueStatus.ManualFeed => "Need Manual Feed",
                PrintQueueStatus.NoToner => "No Toner",
                PrintQueueStatus.Offline => "Offline",
                PrintQueueStatus.OutOfMemory => "Out Of Memory",
                PrintQueueStatus.OutputBinFull => "Output Bin Full",
                PrintQueueStatus.PagePunt => "Page Punt",
                PrintQueueStatus.PaperJam => "Paper Jam",
                PrintQueueStatus.PaperOut => "Paper Out",
                PrintQueueStatus.PaperProblem => "Paper Error",
                PrintQueueStatus.Paused => "Paused",
                PrintQueueStatus.PendingDeletion => "Deleting Job",
                PrintQueueStatus.PowerSave => "Power Save",
                PrintQueueStatus.Printing => "Printing",
                PrintQueueStatus.Processing => "Processing",
                PrintQueueStatus.ServerUnknown => "Server Unknown",
                PrintQueueStatus.TonerLow => "Toner Low",
                PrintQueueStatus.UserIntervention => "Need User Intervention",
                PrintQueueStatus.Waiting => "Waiting",
                PrintQueueStatus.WarmingUp => "Warming Up",

                _ => "Ready",
            };
        }

        /// <summary>
        /// Install new printer to the print server.
        /// </summary>
        /// <param name="printerName">The new printer name.</param>
        /// <param name="driverName">The new printer driver name.</param>
        /// <param name="portNames">IDs of the ports that the new queue uses.</param>
        /// <param name="printerProcessorName">The print processor name.</param>
        /// <param name="printerProperties">The new printer properties.</param>
        public static void InstallPrinter(string printerName, string driverName, string[] portNames, string printerProcessorName, PrintQueueAttributes printerProperties)
        {
            LocalPrintServer localPrintServer = new LocalPrintServer();
            localPrintServer.InstallPrintQueue(printerName, driverName, portNames, printerProcessorName, printerProperties);
            localPrintServer.Commit();
        }

        /// <summary>
        /// Install new printer to the print server.
        /// </summary>
        /// <param name="printServer">The print server.</param>
        /// <param name="printerName">The new printer name.</param>
        /// <param name="driverName">The new printer driver name.</param>
        /// <param name="portNames">IDs of the ports that the new queue uses.</param>
        /// <param name="printerProcessorName">The print processor name.</param>
        /// <param name="printerProperties">The new printer properties.</param>
        public static void InstallPrinter(PrintServer printServer, string printerName, string driverName, string[] portNames, string printerProcessorName, PrintQueueAttributes printerProperties)
        {
            printServer.InstallPrintQueue(printerName, driverName, portNames, printerProcessorName, printerProperties);
            printServer.Commit();
        }

        /// <summary>
        /// Install new printer to the print server.
        /// </summary>
        /// <param name="printServer">The print server.</param>
        /// <param name="printerName">The new printer name.</param>
        /// <param name="driverName">The new printer driver name.</param>
        /// <param name="portNames">IDs of the ports that the new queue uses.</param>
        /// <param name="printerProcessorName">The print processor name.</param>
        /// <param name="printerProperties">The new printer properties.</param>
        /// <param name="printerShareName">The new printer share name.</param>
        /// <param name="printerComment">The new printer comment.</param>
        /// <param name="printerLoction">The new printer loction.</param>
        /// <param name="printerSeparatorFile">The path of a file that is inserted at the beginning of each print job.</param>
        /// <param name="printerPriority">A value from 1 through 99 that specifies the priority of the queue relative to other queues that are hosted by the print server.</param>
        /// <param name="printerDefaultPriority">A value from 1 through 99 that specifies the default priority of new print jobs that are sent to the queue.</param>
        public static void InstallPrinter(PrintServer printServer, string printerName, string driverName, string[] portNames, string printerProcessorName, PrintQueueAttributes printerProperties, string printerShareName, string printerComment, string printerLoction, string printerSeparatorFile, int printerPriority, int printerDefaultPriority)
        {
            printServer.InstallPrintQueue(printerName, driverName, portNames, printerProcessorName, printerProperties, printerShareName, printerComment, printerLoction, printerSeparatorFile, printerPriority, printerDefaultPriority);
            printServer.Commit();
        }
    }

    public class PrintJobHelper
    {
        /// <summary>
        /// Initialize a <see cref="PrintJobHelper"/> class.
        /// </summary>
        protected PrintJobHelper()
        {
            return;
        }

        /// <summary>
        /// Get all print jobs of printer.
        /// </summary>
        /// <param name="printerName">The printer name.</param>
        /// <returns>The print jobs.</returns>
        public static PrintJobInfoCollection GetPrintJobs(string printerName)
        {
            PrintQueue printer = new PrintServer().GetPrintQueue(printerName);
            printer.Refresh();

            return printer.GetPrintJobInfoCollection();
        }

        /// <summary>
        /// Get all print jobs of printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <returns>The print jobs.</returns>
        public static PrintJobInfoCollection GetPrintJobs(PrintQueue printer)
        {
            printer.Refresh();

            return printer.GetPrintJobInfoCollection();
        }

        /// <summary>
        /// Get all print jobs of printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <param name="submitter">The print job submitter.</param>
        /// <returns>The print jobs.</returns>
        public static PrintJobInfoCollection GetPrintJobs(PrintQueue printer, string submitter)
        {
            printer.Refresh();

            PrintJobInfoCollection printJobList = new PrintJobInfoCollection(printer, null);

            foreach (PrintSystemJobInfo jobInfo in printer.GetPrintJobInfoCollection())
            {
                if (jobInfo.Submitter == submitter)
                {
                    printJobList.Add(jobInfo);
                }
            }

            return printJobList;
        }

        /// <summary>
        /// Get all error print jobs of printer.
        /// </summary>
        /// <param name="printerName">The printer name.</param>
        /// <returns>The print jobs.</returns>
        public static PrintJobInfoCollection GetErrorPrintJobs(string printerName)
        {
            return GetErrorPrintJobs(new PrintServer().GetPrintQueue(printerName), false);
        }

        /// <summary>
        /// Get all error print jobs of printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <returns>The print jobs.</returns>
        public static PrintJobInfoCollection GetErrorPrintJobs(PrintQueue printer)
        {
            return GetErrorPrintJobs(printer, false);
        }

        /// <summary>
        /// Get all error print jobs of printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <param name="isCancel">Cancel error print jobs or not.</param>
        /// <returns>The print jobs.</returns>
        public static PrintJobInfoCollection GetErrorPrintJobs(PrintQueue printer, bool isCancel)
        {
            printer.Refresh();

            PrintJobInfoCollection errorList = new PrintJobInfoCollection(printer, null);

            foreach (PrintSystemJobInfo jobInfo in printer.GetPrintJobInfoCollection())
            {
                if (jobInfo.JobStatus == PrintJobStatus.Blocked || jobInfo.JobStatus == PrintJobStatus.Error || jobInfo.JobStatus == PrintJobStatus.Offline || jobInfo.JobStatus == PrintJobStatus.PaperOut || jobInfo.JobStatus == PrintJobStatus.UserIntervention)
                {
                    errorList.Add(jobInfo);

                    if (isCancel)
                    {
                        jobInfo.Cancel();
                        jobInfo.Commit();
                    }
                }
            }

            return errorList;
        }

        /// <summary>
        /// Get all error print jobs of printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <param name="submitter">The print job submitter.</param>
        /// <returns>The print jobs.</returns>
        public static PrintJobInfoCollection GetErrorPrintJobs(PrintQueue printer, string submitter)
        {
            return GetErrorPrintJobs(printer, submitter, false);
        }

        /// <summary>
        /// Get all error print jobs of printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <param name="submitter">The print job submitter.</param>
        /// <param name="isCancel">Cancel error print jobs or not.</param>
        /// <returns>The print jobs.</returns>
        public static PrintJobInfoCollection GetErrorPrintJobs(PrintQueue printer, string submitter, bool isCancel)
        {
            printer.Refresh();

            PrintJobInfoCollection errorList = new PrintJobInfoCollection(printer, null);

            foreach (PrintSystemJobInfo jobInfo in printer.GetPrintJobInfoCollection())
            {
                if (jobInfo.Submitter == submitter)
                {
                    if (jobInfo.JobStatus == PrintJobStatus.Blocked || jobInfo.JobStatus == PrintJobStatus.Error || jobInfo.JobStatus == PrintJobStatus.Offline || jobInfo.JobStatus == PrintJobStatus.PaperOut || jobInfo.JobStatus == PrintJobStatus.UserIntervention)
                    {
                        errorList.Add(jobInfo);

                        if (isCancel)
                        {
                            jobInfo.Cancel();
                            jobInfo.Commit();
                        }
                    }
                }
            }

            return errorList;
        }
    }
}

namespace PrintDialogX.PrintSettings
{
    public enum PageOrientation
    {
        /// <summary>
        /// Standard orientation.
        /// </summary>
        Portrait,

        /// <summary>
        /// Content of the imageable area is rotated on the page 90 degrees counterclockwise from standard (portrait) orientation.
        /// </summary>
        Landscape
    }

    public enum PageColor
    {
        /// <summary>
        /// Output that prints in color.
        /// </summary>
        Color,

        /// <summary>
        /// Output that prints in a grayscale.
        /// </summary>
        Grayscale,

        /// <summary>
        /// Output that prints in a single color and with the same degree of intensity.
        /// </summary>
        Monochrome
    }

    public enum PageQuality
    {
        /// <summary>
        /// Automatically selects a quality type that is based on the contents of a print job.
        /// </summary>
        Automatic,

        /// <summary>
        /// Draft quality.
        /// </summary>
        Draft,

        /// <summary>
        /// Fax quality.
        /// </summary>
        Fax,

        /// <summary>
        /// Higher than normal quality.
        /// </summary>
        High,

        /// <summary>
        /// Normal quality.
        /// </summary>
        Normal,

        /// <summary>
        /// Photographic quality.
        /// </summary>
        Photographic,

        /// <summary>
        /// Text quality.
        /// </summary>
        Text
    }

    public enum PageSize
    {
        /// <summary>
        /// A0.
        /// </summary>
        ISOA0,

        /// <summary>
        /// A1.
        /// </summary>
        ISOA1,

        /// <summary>
        /// A10.
        /// </summary>
        ISOA10,

        /// <summary>
        /// A2.
        /// </summary>
        ISOA2,

        /// <summary>
        /// A3.
        /// </summary>
        ISOA3,

        /// <summary>
        /// A3 Rotated.
        /// </summary>
        ISOA3Rotated,

        /// <summary>
        /// A3 Extra.
        /// </summary>
        ISOA3Extra,

        /// <summary>
        /// A4.
        /// </summary>
        ISOA4,

        /// <summary>
        /// A4 Rotated.
        /// </summary>
        ISOA4Rotated,

        /// <summary>
        /// A4 Extra.
        /// </summary>
        ISOA4Extra,

        /// <summary>
        /// A5.
        /// </summary>
        ISOA5,

        /// <summary>
        /// A5 Rotated.
        /// </summary>
        ISOA5Rotated,

        /// <summary>
        /// A5 Extra.
        /// </summary>
        ISOA5Extra,

        /// <summary>
        /// A6.
        /// </summary>
        ISOA6,

        /// <summary>
        /// A6 Rotated.
        /// </summary>
        ISOA6Rotated,

        /// <summary>
        /// A7.
        /// </summary>
        ISOA7,

        /// <summary>
        /// A8.
        /// </summary>
        ISOA8,

        /// <summary>
        /// A9.
        /// </summary>
        ISOA9,

        /// <summary>
        /// B0.
        /// </summary>
        ISOB0,

        /// <summary>
        /// B1.
        /// </summary>
        ISOB1,

        /// <summary>
        /// B10.
        /// </summary>
        ISOB10,

        /// <summary>
        /// B2.
        /// </summary>
        ISOB2,

        /// <summary>
        /// B3.
        /// </summary>
        ISOB3,

        /// <summary>
        /// B4.
        /// </summary>
        ISOB4,

        /// <summary>
        /// B4 Envelope.
        /// </summary>
        ISOB4Envelope,

        /// <summary>
        /// B5 Envelope.
        /// </summary>
        ISOB5Envelope,

        /// <summary>
        /// B5 Extra.
        /// </summary>
        ISOB5Extra,

        /// <summary>
        /// B7.
        /// </summary>
        ISOB7,

        /// <summary>
        /// B8.
        /// </summary>
        ISOB8,

        /// <summary>
        /// B9.
        /// </summary>
        ISOB9,

        /// <summary>
        /// C0.
        /// </summary>
        ISOC0,

        /// <summary>
        /// C1.
        /// </summary>
        ISOC1,

        /// <summary>
        /// C10.
        /// </summary>
        ISOC10,

        /// <summary>
        /// C2.
        /// </summary>
        ISOC2,

        /// <summary>
        /// C3.
        /// </summary>
        ISOC3,

        /// <summary>
        /// C3 Envelope.
        /// </summary>
        ISOC3Envelope,

        /// <summary>
        /// C4.
        /// </summary>
        ISOC4,

        /// <summary>
        /// C4 Envelope.
        /// </summary>
        ISOC4Envelope,

        /// <summary>
        /// C5.
        /// </summary>
        ISOC5,

        /// <summary>
        /// C5 Envelope.
        /// </summary>
        ISOC5Envelope,

        /// <summary>
        /// C6.
        /// </summary>
        ISOC6,

        /// <summary>
        /// C6 Envelope.
        /// </summary>
        ISOC6Envelope,

        /// <summary>
        /// C6C5 Envelope.
        /// </summary>
        ISOC6C5Envelope,

        /// <summary>
        /// C7.
        /// </summary>
        ISOC7,

        /// <summary>
        /// C8.
        /// </summary>
        ISOC8,

        /// <summary>
        /// C9.
        /// </summary>
        ISOC9,

        /// <summary>
        /// DL Envelope.
        /// </summary>
        ISODLEnvelope,

        /// <summary>
        /// DL Envelope Rotated.
        /// </summary>
        ISODLEnvelopeRotated,

        /// <summary>
        /// SRA 3.
        /// </summary>
        ISOSRA3,

        /// <summary>
        /// Quadruple Hagaki Postcard.
        /// </summary>
        JapanQuadrupleHagakiPostcard,

        /// <summary>
        /// Japanese Industrial Standard B0.
        /// </summary>
        JISB0,

        /// <summary>
        /// Japanese Industrial Standard B1.
        /// </summary>
        JISB1,

        /// <summary>
        /// Japanese Industrial Standard B10.
        /// </summary>
        JISB10,

        /// <summary>
        /// Japanese Industrial Standard B2.
        /// </summary>
        JISB2,

        /// <summary>
        /// Japanese Industrial Standard B3.
        /// </summary>
        JISB3,

        /// <summary>
        /// Japanese Industrial Standard B4.
        /// </summary>
        JISB4,

        /// <summary>
        /// Japanese Industrial Standard B4 Rotated.
        /// </summary>
        JISB4Rotated,

        /// <summary>
        /// Japanese Industrial Standard B5.
        /// </summary>
        JISB5,

        /// <summary>
        /// Japanese Industrial Standard B5 Rotated.
        /// </summary>
        JISB5Rotated,

        /// <summary>
        /// Japanese Industrial Standard B6.
        /// </summary>
        JISB6,

        /// <summary>
        /// Japanese Industrial Standard B6 Rotated.
        /// </summary>
        JISB6Rotated,

        /// <summary>
        /// Japanese Industrial Standard B7.
        /// </summary>
        JISB7,

        /// <summary>
        /// Japanese Industrial Standard B8.
        /// </summary>
        JISB8,

        /// <summary>
        /// Japanese Industrial Standard B9.
        /// </summary>
        JISB9,

        /// <summary>
        /// Chou 3 Envelope.
        /// </summary>
        JapanChou3Envelope,

        /// <summary>
        /// Chou 3 Envelope Rotated.
        /// </summary>
        JapanChou3EnvelopeRotated,

        /// <summary>
        /// Chou 4 Envelope.
        /// </summary>
        JapanChou4Envelope,

        /// <summary>
        /// Chou 4 Envelope Rotated.
        /// </summary>
        JapanChou4EnvelopeRotated,

        /// <summary>
        /// Hagaki Postcard.
        /// </summary>
        JapanHagakiPostcard,

        /// <summary>
        /// Hagaki Postcard Rotated.
        /// </summary>
        JapanHagakiPostcardRotated,

        /// <summary>
        /// Kaku 2 Envelope.
        /// </summary>
        JapanKaku2Envelope,

        /// <summary>
        /// Kaku 2 Envelope Rotated.
        /// </summary>
        JapanKaku2EnvelopeRotated,

        /// <summary>
        /// Kaku 3 Envelope.
        /// </summary>
        JapanKaku3Envelope,

        /// <summary>
        /// Kaku 3 Envelope Rotated.
        /// </summary>
        JapanKaku3EnvelopeRotated,

        /// <summary>
        /// You 4 Envelope.
        /// </summary>
        JapanYou4Envelope,

        /// <summary>
        /// 10 x 11.
        /// </summary>
        NorthAmerica10x11,

        /// <summary>
        /// 10 x 14.
        /// </summary>
        NorthAmerica10x14,

        /// <summary>
        /// 11 x 17.
        /// </summary>
        NorthAmerica11x17,

        /// <summary>
        /// 9 x 11.
        /// </summary>
        NorthAmerica9x11,

        /// <summary>
        /// Architecture A Sheet.
        /// </summary>
        NorthAmericaArchitectureASheet,

        /// <summary>
        /// Architecture B Sheet.
        /// </summary>
        NorthAmericaArchitectureBSheet,

        /// <summary>
        /// Architecture C Sheet.
        /// </summary>
        NorthAmericaArchitectureCSheet,

        /// <summary>
        /// Architecture D Sheet.
        /// </summary>
        NorthAmericaArchitectureDSheet,

        /// <summary>
        /// Architecture E Sheet.
        /// </summary>
        NorthAmericaArchitectureESheet,

        /// <summary>
        /// C Sheet.
        /// </summary>
        NorthAmericaCSheet,

        /// <summary>
        /// D Sheet.
        /// </summary>
        NorthAmericaDSheet,

        /// <summary>
        /// E Sheet.
        /// </summary>
        NorthAmericaESheet,

        /// <summary>
        /// Executive.
        /// </summary>
        NorthAmericaExecutive,

        /// <summary>
        /// German Legal Fanfold.
        /// </summary>
        NorthAmericaGermanLegalFanfold,

        /// <summary>
        /// German Standard Fanfold.
        /// </summary>
        NorthAmericaGermanStandardFanfold,

        /// <summary>
        /// Legal.
        /// </summary>
        NorthAmericaLegal,

        /// <summary>
        /// Legal Extra.
        /// </summary>
        NorthAmericaLegalExtra,

        /// <summary>
        /// Letter.
        /// </summary>
        NorthAmericaLetter,

        /// <summary>
        /// Letter Rotated.
        /// </summary>
        NorthAmericaLetterRotated,

        /// <summary>
        /// Letter Extra.
        /// </summary>
        NorthAmericaLetterExtra,

        /// <summary>
        /// Letter Plus.
        /// </summary>
        NorthAmericaLetterPlus,

        /// <summary>
        /// Monarch Envelope.
        /// </summary>
        NorthAmericaMonarchEnvelope,

        /// <summary>
        /// Note.
        /// </summary>
        NorthAmericaNote,

        /// <summary>
        /// #10 Envelope.
        /// </summary>
        NorthAmericaNumber10Envelope,

        /// <summary>
        /// #10 Envelope Rotated.
        /// </summary>
        NorthAmericaNumber10EnvelopeRotated,

        /// <summary>
        /// #9 Envelope.
        /// </summary>
        NorthAmericaNumber9Envelope,

        /// <summary>
        /// #11 Envelope.
        /// </summary>
        NorthAmericaNumber11Envelope,

        /// <summary>
        /// #12 Envelope.
        /// </summary>
        NorthAmericaNumber12Envelope,

        /// <summary>
        /// #14 Envelope.
        /// </summary>
        NorthAmericaNumber14Envelope,

        /// <summary>
        /// Personal Envelope.
        /// </summary>
        NorthAmericaPersonalEnvelope,

        /// <summary>
        /// Quarto.
        /// </summary>
        NorthAmericaQuarto,

        /// <summary>
        /// Statement.
        /// </summary>
        NorthAmericaStatement,

        /// <summary>
        /// Super A.
        /// </summary>
        NorthAmericaSuperA,

        /// <summary>
        /// Super B.
        /// </summary>
        NorthAmericaSuperB,

        /// <summary>
        /// Tabloid.
        /// </summary>
        NorthAmericaTabloid,

        /// <summary>
        /// Tabloid Extra.
        /// </summary>
        NorthAmericaTabloidExtra,

        /// <summary>
        /// A4 Plus.
        /// </summary>
        OtherMetricA4Plus,

        /// <summary>
        /// A3 Plus.
        /// </summary>
        OtherMetricA3Plus,

        /// <summary>
        /// Folio.
        /// </summary>
        OtherMetricFolio,

        /// <summary>
        /// Invite Envelope.
        /// </summary>
        OtherMetricInviteEnvelope,

        /// <summary>
        /// Italian Envelope.
        /// </summary>
        OtherMetricItalianEnvelope,

        /// <summary>
        /// People's Republic of China #1 Envelope.
        /// </summary>
        PRC1Envelope,

        /// <summary>
        /// People's Republic of China #1 Envelope Rotated.
        /// </summary>
        PRC1EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #10 Envelope.
        /// </summary>
        PRC10Envelope,

        /// <summary>
        /// People's Republic of China #10 Envelope Rotated.
        /// </summary>
        PRC10EnvelopeRotated,

        /// <summary>
        /// People's Republic of China 16K.
        /// </summary>
        PRC16K,

        /// <summary>
        /// People's Republic of China 16K Rotated.
        /// </summary>
        PRC16KRotated,

        /// <summary>
        /// People's Republic of China #2 Envelope.
        /// </summary>
        PRC2Envelope,

        /// <summary>
        /// People's Republic of China #2 Envelope Rotated.
        /// </summary>
        PRC2EnvelopeRotated,

        /// <summary>
        /// People's Republic of China 32K.
        /// </summary>
        PRC32K,

        /// <summary>
        /// People's Republic of China 32K Rotated.
        /// </summary>
        PRC32KRotated,

        /// <summary>
        /// People's Republic of China 32K Big.
        /// </summary>
        PRC32KBig,

        /// <summary>
        /// People's Republic of China #3 Envelope.
        /// </summary>
        PRC3Envelope,

        /// <summary>
        /// People's Republic of China #3 Envelope Rotated.
        /// </summary>
        PRC3EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #4 Envelope.
        /// </summary>
        PRC4Envelope,

        /// <summary>
        /// People's Republic of China #4 Envelope Rotated.
        /// </summary>
        PRC4EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #5 Envelope.
        /// </summary>
        PRC5Envelope,

        /// <summary>
        /// People's Republic of China #5 Envelope Rotated.
        /// </summary>
        PRC5EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #6 Envelope.
        /// </summary>
        PRC6Envelope,

        /// <summary>
        /// People's Republic of China #6 Envelope Rotated.
        /// </summary>
        PRC6EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #7 Envelope.
        /// </summary>
        PRC7Envelope,

        /// <summary>
        /// People's Republic of China #7 Envelope Rotated.
        /// </summary>
        PRC7EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #8 Envelope.
        /// </summary>
        PRC8Envelope,

        /// <summary>
        /// People's Republic of China #8 Envelope Rotated.
        /// </summary>
        PRC8EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #9 Envelope.
        /// </summary>
        PRC9Envelope,

        /// <summary>
        /// People's Republic of China #9 Envelope Rotated.
        /// </summary>
        PRC9EnvelopeRotated,

        /// <summary>
        /// 4-inch wide roll.
        /// </summary>
        Roll04Inch,

        /// <summary>
        /// 6-inch wide roll.
        /// </summary>
        Roll06Inch,

        /// <summary>
        /// 8-inch wide roll.
        /// </summary>
        Roll08Inch,

        /// <summary>
        /// 12-inch wide roll.
        /// </summary>
        Roll12Inch,

        /// <summary>
        /// 15-inch wide roll.
        /// </summary>
        Roll15Inch,

        /// <summary>
        /// 18-inch wide roll.
        /// </summary>
        Roll18Inch,

        /// <summary>
        /// 22-inch wide roll.
        /// </summary>
        Roll22Inch,

        /// <summary>
        /// 24-inch wide roll.
        /// </summary>
        Roll24Inch,

        /// <summary>
        /// 30-inch wide roll.
        /// </summary>
        Roll30Inch,

        /// <summary>
        /// 36-inch wide roll.
        /// </summary>
        Roll36Inch,

        /// <summary>
        /// 54-inch wide roll.
        /// </summary>
        Roll54Inch,

        /// <summary>
        /// Double Hagaki Postcard.
        /// </summary>
        JapanDoubleHagakiPostcard,

        /// <summary>
        /// Double Hagaki Postcard Rotated.
        /// </summary>
        JapanDoubleHagakiPostcardRotated,

        /// <summary>
        /// L Photo.
        /// </summary>
        JapanLPhoto,

        /// <summary>
        /// 2L Photo.
        /// </summary>
        Japan2LPhoto,

        /// <summary>
        /// You 1 Envelope.
        /// </summary>
        JapanYou1Envelope,

        /// <summary>
        /// You 2 Envelope.
        /// </summary>
        JapanYou2Envelope,

        /// <summary>
        /// You 3 Envelope.
        /// </summary>
        JapanYou3Envelope,

        /// <summary>
        /// You 4 Envelope Rotated.
        /// </summary>
        JapanYou4EnvelopeRotated,

        /// <summary>
        /// You 6 Envelope.
        /// </summary>
        JapanYou6Envelope,

        /// <summary>
        /// You 6 Envelope Rotated.
        /// </summary>
        JapanYou6EnvelopeRotated,

        /// <summary>
        /// 4 x 6.
        /// </summary>
        NorthAmerica4x6,

        /// <summary>
        /// 4 x 8.
        /// </summary>
        NorthAmerica4x8,

        /// <summary>
        /// 5 x 7.
        /// </summary>
        NorthAmerica5x7,

        /// <summary>
        /// 8 x 10.
        /// </summary>
        NorthAmerica8x10,

        /// <summary>
        /// 10 x 12.
        /// </summary>
        NorthAmerica10x12,

        /// <summary>
        /// 14 x 17.
        /// </summary>
        NorthAmerica14x17,

        /// <summary>
        /// Business card.
        /// </summary>
        BusinessCard,

        /// <summary>
        /// Credit card.
        /// </summary>
        CreditCard
    }

    public enum PageType
    {
        /// <summary>
        /// The print device selects the media.
        /// </summary>
        AutoSelect,

        /// <summary>
        /// Archive-quality media.
        /// </summary>
        Archival,

        /// <summary>
        /// Specialty back-printing film.
        /// </summary>
        BackPrintFilm,

        /// <summary>
        /// Standard bond media.
        /// </summary>
        Bond,

        /// <summary>
        /// Standard card stock.
        /// </summary>
        CardStock,

        /// <summary>
        /// Continuous-feed media.
        /// </summary>
        Continuous,

        /// <summary>
        /// Standard envelope.
        /// </summary>
        EnvelopePlain,

        /// <summary>
        /// Window envelope.
        /// </summary>
        EnvelopeWindow,

        /// <summary>
        /// Fabric media.
        /// </summary>
        Fabric,

        /// <summary>
        /// Specialty high-resolution media.
        /// </summary>
        HighResolution,

        /// <summary>
        /// Label media.
        /// </summary>
        Label,

        /// <summary>
        /// Attached multipart forms.
        /// </summary>
        MultiLayerForm,

        /// <summary>
        /// Individual multipart forms.
        /// </summary>
        MultiPartForm,

        /// <summary>
        /// Standard photographic media.
        /// </summary>
        Photographic,

        /// <summary>
        /// Film photographic media.
        /// </summary>
        PhotographicFilm,

        /// <summary>
        /// Glossy photographic media.
        /// </summary>
        PhotographicGlossy,

        /// <summary>
        /// High-gloss photographic media.
        /// </summary>
        PhotographicHighGloss,

        /// <summary>
        /// Matte photographic media.
        /// </summary>
        PhotographicMatte,

        /// <summary>
        /// Satin photographic media.
        /// </summary>
        PhotographicSatin,

        /// <summary>
        /// Semi-gloss photographic media.
        /// </summary>
        PhotographicSemiGloss,

        /// <summary>
        /// Plain paper.
        /// </summary>
        Plain,

        /// <summary>
        /// Output to a display in continuous form.
        /// </summary>
        Screen,

        /// <summary>
        /// Output to a display in paged form.
        /// </summary>
        ScreenPaged,

        /// <summary>
        /// Specialty stationary.
        /// </summary>
        Stationery,

        /// <summary>
        /// Tab stock, not precut (single tabs).
        /// </summary>
        TabStockFull,

        /// <summary>
        /// Tab stock, precut (multiple tabs).
        /// </summary>
        TabStockPreCut,

        /// <summary>
        /// Transparent sheet.
        /// </summary>
        Transparency,

        /// <summary>
        /// Media that is used to transfer an image to a T-shirt.
        /// </summary>
        TShirtTransfer
    }

    public enum DoubleSided
    {
        /// <summary>
        /// Output prints on only one side of each sheet.
        /// </summary>
        OneSided,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="PrintDocumentImageableArea.MediaSizeWidth"/>.
        /// </summary>
        DoubleSidedShortEdge,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="PrintDocumentImageableArea.MediaSizeHeight"/>.
        /// </summary>
        DoubleSidedLongEdge
    }

    public enum PageOrder
    {
        /// <summary>
        /// Pages appear in rows, from left to right and top to bottom relative to page orientation.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Pages appear in rows, from right to left and top to bottom relative to page orientation.
        /// </summary>
        HorizontalReverse,

        /// <summary>
        /// Pages appear in columns, from top to bottom and left to right relative to page orientation.
        /// </summary>
        Vertical,

        /// <summary>
        /// Pages appear in columns, from bottom to top and left to right relative to page orientation.
        /// </summary>
        VerticalReverse
    }
}

namespace PrintDialogX.SettingsHepler
{
    public class NameInfoHepler
    {
        /// <summary>
        /// Initialize a <see cref="NameInfoHepler"/> class.
        /// </summary>
        protected NameInfoHepler()
        {
            return;
        }

        /// <summary>
        /// Get the name info of <see cref="PageMediaSizeName"/> object.
        /// </summary>
        /// <param name="sizeName">The <see cref="PageMediaSizeName"/> object of page size name.</param>
        /// <returns>The name info.</returns>
        public static string GetPageMediaSizeNameInfo(PageMediaSizeName sizeName)
        {
            return sizeName switch
            {
                PageMediaSizeName.BusinessCard => "Business Card",
                PageMediaSizeName.CreditCard => "Credit Card",
                PageMediaSizeName.ISOA0 => "ISO A0",
                PageMediaSizeName.ISOA1 => "ISO A1",
                PageMediaSizeName.ISOA10 => "ISO A10",
                PageMediaSizeName.ISOA2 => "ISO A2",
                PageMediaSizeName.ISOA3 => "ISO A3",
                PageMediaSizeName.ISOA3Extra => "ISO A3 Extra",
                PageMediaSizeName.ISOA3Rotated => "ISO A3 Rotated",
                PageMediaSizeName.ISOA4 => "ISO A4",
                PageMediaSizeName.ISOA4Extra => "ISO A4 Extra",
                PageMediaSizeName.ISOA4Rotated => "ISO A4 Rotated",
                PageMediaSizeName.ISOA5 => "ISO A5",
                PageMediaSizeName.ISOA5Extra => "ISO A5 Extra",
                PageMediaSizeName.ISOA5Rotated => "ISO A5 Rotated",
                PageMediaSizeName.ISOA6 => "ISO A6",
                PageMediaSizeName.ISOA6Rotated => "ISO A6 Rotated",
                PageMediaSizeName.ISOA7 => "ISO A7",
                PageMediaSizeName.ISOA8 => "ISO A8",
                PageMediaSizeName.ISOA9 => "ISO A9",
                PageMediaSizeName.ISOB0 => "ISO B0",
                PageMediaSizeName.ISOB1 => "ISO B1",
                PageMediaSizeName.ISOB10 => "ISO B10",
                PageMediaSizeName.ISOB2 => "ISO B2",
                PageMediaSizeName.ISOB3 => "ISO B3",
                PageMediaSizeName.ISOB4 => "ISO B4",
                PageMediaSizeName.ISOB4Envelope => "ISO B4 Envelope",
                PageMediaSizeName.ISOB5Envelope => "ISO B5 Envelope",
                PageMediaSizeName.ISOB5Extra => "ISO B5 Extra",
                PageMediaSizeName.ISOB7 => "ISO B7",
                PageMediaSizeName.ISOB8 => "ISO B8",
                PageMediaSizeName.ISOB9 => "ISO B9",
                PageMediaSizeName.ISOC0 => "ISO C0",
                PageMediaSizeName.ISOC1 => "ISO C1",
                PageMediaSizeName.ISOC2 => "ISO C2",
                PageMediaSizeName.ISOC3 => "ISO C3",
                PageMediaSizeName.ISOC3Envelope => "ISO C3 Envelope",
                PageMediaSizeName.ISOC4 => "ISO C4",
                PageMediaSizeName.ISOC4Envelope => "ISO C4 Envelope",
                PageMediaSizeName.ISOC5 => "ISO C5",
                PageMediaSizeName.ISOC5Envelope => "ISO C5 Envelope",
                PageMediaSizeName.ISOC6 => "ISO C6",
                PageMediaSizeName.ISOC6C5Envelope => "ISO C6C5 Envelope",
                PageMediaSizeName.ISOC6Envelope => "ISO C6 Envelope",
                PageMediaSizeName.ISOC7 => "ISO C7",
                PageMediaSizeName.ISOC8 => "ISO C8",
                PageMediaSizeName.ISOC9 => "ISO C9",
                PageMediaSizeName.ISOC10 => "ISO C10",
                PageMediaSizeName.ISODLEnvelope => "ISO DL Envelope",
                PageMediaSizeName.ISODLEnvelopeRotated => "ISO DL Envelope Rotated",
                PageMediaSizeName.ISOSRA3 => "ISO SRA3",
                PageMediaSizeName.Japan2LPhoto => "Japan 2L Photo",
                PageMediaSizeName.JapanChou3Envelope => "Japan Chou 3 Envelope",
                PageMediaSizeName.JapanChou3EnvelopeRotated => "Japan Chou 3 Envelope Rotated",
                PageMediaSizeName.JapanChou4Envelope => "Japan Chou 4 Envelope",
                PageMediaSizeName.JapanChou4EnvelopeRotated => "Japan Chou 4 Envelope Rotated",
                PageMediaSizeName.JapanDoubleHagakiPostcard => "Japan Double Hagaki Postcard",
                PageMediaSizeName.JapanDoubleHagakiPostcardRotated => "Japan Double Hagaki Postcard Rotated",
                PageMediaSizeName.JapanHagakiPostcard => "Japan Hagaki Postcard",
                PageMediaSizeName.JapanHagakiPostcardRotated => "Japan Hagaki Postcard Rotated",
                PageMediaSizeName.JapanKaku2Envelope => "Japan Kaku 2 Envelope",
                PageMediaSizeName.JapanKaku2EnvelopeRotated => "Japan Kaku 2 Envelope Rotated",
                PageMediaSizeName.JapanKaku3Envelope => "Japan Kaku 3 Envelope",
                PageMediaSizeName.JapanKaku3EnvelopeRotated => "Japan Kaku 3 Envelope Rotated",
                PageMediaSizeName.JapanLPhoto => "Japan L Photo",
                PageMediaSizeName.JapanQuadrupleHagakiPostcard => "Japan Quadruple Hagaki Postcard",
                PageMediaSizeName.JapanYou1Envelope => "Japan You 1 Envelope",
                PageMediaSizeName.JapanYou2Envelope => "Japan You 2 Envelope",
                PageMediaSizeName.JapanYou3Envelope => "Japan You 3 Envelope",
                PageMediaSizeName.JapanYou4Envelope => "Japan You 4 Envelope",
                PageMediaSizeName.JapanYou4EnvelopeRotated => "Japan You 4 Envelope Rotated",
                PageMediaSizeName.JapanYou6Envelope => "Japan You 6 Envelope",
                PageMediaSizeName.JapanYou6EnvelopeRotated => "Japan You 6 Envelope Rotated",
                PageMediaSizeName.JISB0 => "JIS B0",
                PageMediaSizeName.JISB1 => "JIS B1",
                PageMediaSizeName.JISB10 => "JIS B10",
                PageMediaSizeName.JISB2 => "JIS B2",
                PageMediaSizeName.JISB3 => "JIS B3",
                PageMediaSizeName.JISB4 => "JIS B4",
                PageMediaSizeName.JISB4Rotated => "JIS B4 Rotated",
                PageMediaSizeName.JISB5 => "JIS B5",
                PageMediaSizeName.JISB5Rotated => "JIS B5 Rotated",
                PageMediaSizeName.JISB6 => "JIS B6",
                PageMediaSizeName.JISB6Rotated => "JIS B6 Rotated",
                PageMediaSizeName.JISB7 => "JIS B7",
                PageMediaSizeName.JISB8 => "JIS B8",
                PageMediaSizeName.JISB9 => "JIS B9",
                PageMediaSizeName.NorthAmerica10x11 => "North America 10 x 11",
                PageMediaSizeName.NorthAmerica10x12 => "North America 10 x 12",
                PageMediaSizeName.NorthAmerica10x14 => "North America 10 x 14",
                PageMediaSizeName.NorthAmerica11x17 => "North America 11 x 17",
                PageMediaSizeName.NorthAmerica14x17 => "North America 14 x 17",
                PageMediaSizeName.NorthAmerica4x6 => "North America 4 x 6",
                PageMediaSizeName.NorthAmerica4x8 => "North America 4 x 8",
                PageMediaSizeName.NorthAmerica5x7 => "North America 5 x 7",
                PageMediaSizeName.NorthAmerica8x10 => "North America 8 x 10",
                PageMediaSizeName.NorthAmerica9x11 => "North America 9 x 11",
                PageMediaSizeName.NorthAmericaArchitectureASheet => "North America Architecture A Sheet",
                PageMediaSizeName.NorthAmericaArchitectureBSheet => "North America Architecture B Sheet",
                PageMediaSizeName.NorthAmericaArchitectureCSheet => "North America Architecture C Sheet",
                PageMediaSizeName.NorthAmericaArchitectureDSheet => "North America Architecture D Sheet",
                PageMediaSizeName.NorthAmericaArchitectureESheet => "North America Architecture E Sheet",
                PageMediaSizeName.NorthAmericaCSheet => "North America C Sheet",
                PageMediaSizeName.NorthAmericaDSheet => "North America D Sheet",
                PageMediaSizeName.NorthAmericaESheet => "North America E Sheet",
                PageMediaSizeName.NorthAmericaExecutive => "North America Executive",
                PageMediaSizeName.NorthAmericaGermanLegalFanfold => "North America German Legal Fanfold",
                PageMediaSizeName.NorthAmericaGermanStandardFanfold => "North America German Standard Fanfold",
                PageMediaSizeName.NorthAmericaLegal => "North America Legal",
                PageMediaSizeName.NorthAmericaLegalExtra => "North America Legal Extra",
                PageMediaSizeName.NorthAmericaLetter => "North America Letter",
                PageMediaSizeName.NorthAmericaLetterExtra => "North America Letter Extra",
                PageMediaSizeName.NorthAmericaLetterPlus => "North America Letter Plus",
                PageMediaSizeName.NorthAmericaLetterRotated => "North America Letter Rotated",
                PageMediaSizeName.NorthAmericaMonarchEnvelope => "North America Monarch Envelope",
                PageMediaSizeName.NorthAmericaNote => "North America Note",
                PageMediaSizeName.NorthAmericaNumber10Envelope => "North America Number 10 Envelope",
                PageMediaSizeName.NorthAmericaNumber10EnvelopeRotated => "North America Number 10 Envelope Rotated",
                PageMediaSizeName.NorthAmericaNumber11Envelope => "North America Number 11 Envelope",
                PageMediaSizeName.NorthAmericaNumber12Envelope => "North America Number 12 Envelope",
                PageMediaSizeName.NorthAmericaNumber14Envelope => "North America Number 14 Envelope",
                PageMediaSizeName.NorthAmericaNumber9Envelope => "North America Number 9 Envelope",
                PageMediaSizeName.NorthAmericaPersonalEnvelope => "North America Personal Envelope",
                PageMediaSizeName.NorthAmericaQuarto => "North America Quarto",
                PageMediaSizeName.NorthAmericaStatement => "North America Statement",
                PageMediaSizeName.NorthAmericaSuperA => "North America Super A",
                PageMediaSizeName.NorthAmericaSuperB => "North America Super B",
                PageMediaSizeName.NorthAmericaTabloid => "North America Tabloid",
                PageMediaSizeName.NorthAmericaTabloidExtra => "North America Tabloid Extra",
                PageMediaSizeName.OtherMetricA3Plus => "A3 Plus",
                PageMediaSizeName.OtherMetricA4Plus => "A4 Plus",
                PageMediaSizeName.OtherMetricFolio => "Folio",
                PageMediaSizeName.OtherMetricInviteEnvelope => "Invite Envelope",
                PageMediaSizeName.OtherMetricItalianEnvelope => "Italian Envelope",
                PageMediaSizeName.PRC10Envelope => "PRC #10 Envelope",
                PageMediaSizeName.PRC10EnvelopeRotated => "PRC #10 Envelope Rotated",
                PageMediaSizeName.PRC16K => "PRC 16K",
                PageMediaSizeName.PRC16KRotated => "PRC 16K Rotated",
                PageMediaSizeName.PRC1Envelope => "PRC #1 Envelope",
                PageMediaSizeName.PRC1EnvelopeRotated => "PRC #1 Envelope Rotated",
                PageMediaSizeName.PRC2Envelope => "PRC #2 Envelope",
                PageMediaSizeName.PRC2EnvelopeRotated => "PRC #2 Envelope Rotated",
                PageMediaSizeName.PRC32K => "PRC 32K",
                PageMediaSizeName.PRC32KBig => "PRC 32K Big",
                PageMediaSizeName.PRC32KRotated => "PRC 32K Rotated",
                PageMediaSizeName.PRC3Envelope => "PRC #3 Envelope",
                PageMediaSizeName.PRC3EnvelopeRotated => "PRC #3 Envelope Rotated",
                PageMediaSizeName.PRC4Envelope => "PRC #4 Envelope",
                PageMediaSizeName.PRC4EnvelopeRotated => "PRC #4 Envelope Rotated",
                PageMediaSizeName.PRC5Envelope => "PRC #5 Envelope",
                PageMediaSizeName.PRC5EnvelopeRotated => "PRC #5 Envelope Rotated",
                PageMediaSizeName.PRC6Envelope => "PRC #6 Envelope",
                PageMediaSizeName.PRC6EnvelopeRotated => "PRC #6 Envelope Rotated",
                PageMediaSizeName.PRC7Envelope => "PRC #7 Envelope",
                PageMediaSizeName.PRC7EnvelopeRotated => "PRC #7 Envelope Rotated",
                PageMediaSizeName.PRC8Envelope => "PRC #8 Envelope",
                PageMediaSizeName.PRC8EnvelopeRotated => "PRC #8 Envelope Rotated",
                PageMediaSizeName.PRC9Envelope => "PRC #9 Envelope",
                PageMediaSizeName.PRC9EnvelopeRotated => "PRC #9 Envelope Rotated",
                PageMediaSizeName.Roll04Inch => "4-inch Wide Roll",
                PageMediaSizeName.Roll06Inch => "6-inch Wide Roll",
                PageMediaSizeName.Roll08Inch => "8-inch Wide Roll",
                PageMediaSizeName.Roll12Inch => "12-inch Wide Roll",
                PageMediaSizeName.Roll15Inch => "15-inch Wide Roll",
                PageMediaSizeName.Roll18Inch => "18-inch Wide Roll",
                PageMediaSizeName.Roll22Inch => "22-inch Wide Roll",
                PageMediaSizeName.Roll24Inch => "24-inch Wide Roll",
                PageMediaSizeName.Roll30Inch => "30-inch Wide Roll",
                PageMediaSizeName.Roll36Inch => "36-inch Wide Roll",
                PageMediaSizeName.Roll54Inch => "54-inch Wide Roll",

                _ => "Unknown Size",
            };
        }

        /// <summary>
        /// Get the name info of <see cref="PrintSettings.PageSize"/> object.
        /// </summary>
        /// <param name="sizeName">The <see cref="PrintSettings.PageSize"/> object of page size name.</param>
        /// <returns>The name info.</returns>
        public static string GetPageMediaSizeNameInfo(PrintSettings.PageSize sizeName)
        {
            return sizeName switch
            {
                PrintSettings.PageSize.BusinessCard => "Business Card",
                PrintSettings.PageSize.CreditCard => "Credit Card",
                PrintSettings.PageSize.ISOA0 => "ISO A0",
                PrintSettings.PageSize.ISOA1 => "ISO A1",
                PrintSettings.PageSize.ISOA10 => "ISO A10",
                PrintSettings.PageSize.ISOA2 => "ISO A2",
                PrintSettings.PageSize.ISOA3 => "ISO A3",
                PrintSettings.PageSize.ISOA3Extra => "ISO A3 Extra",
                PrintSettings.PageSize.ISOA3Rotated => "ISO A3 Rotated",
                PrintSettings.PageSize.ISOA4 => "ISO A4",
                PrintSettings.PageSize.ISOA4Extra => "ISO A4 Extra",
                PrintSettings.PageSize.ISOA4Rotated => "ISO A4 Rotated",
                PrintSettings.PageSize.ISOA5 => "ISO A5",
                PrintSettings.PageSize.ISOA5Extra => "ISO A5 Extra",
                PrintSettings.PageSize.ISOA5Rotated => "ISO A5 Rotated",
                PrintSettings.PageSize.ISOA6 => "ISO A6",
                PrintSettings.PageSize.ISOA6Rotated => "ISO A6 Rotated",
                PrintSettings.PageSize.ISOA7 => "ISO A7",
                PrintSettings.PageSize.ISOA8 => "ISO A8",
                PrintSettings.PageSize.ISOA9 => "ISO A9",
                PrintSettings.PageSize.ISOB0 => "ISO B0",
                PrintSettings.PageSize.ISOB1 => "ISO B1",
                PrintSettings.PageSize.ISOB10 => "ISO B10",
                PrintSettings.PageSize.ISOB2 => "ISO B2",
                PrintSettings.PageSize.ISOB3 => "ISO B3",
                PrintSettings.PageSize.ISOB4 => "ISO B4",
                PrintSettings.PageSize.ISOB4Envelope => "ISO B4 Envelope",
                PrintSettings.PageSize.ISOB5Envelope => "ISO B5 Envelope",
                PrintSettings.PageSize.ISOB5Extra => "ISO B5 Extra",
                PrintSettings.PageSize.ISOB7 => "ISO B7",
                PrintSettings.PageSize.ISOB8 => "ISO B8",
                PrintSettings.PageSize.ISOB9 => "ISO B9",
                PrintSettings.PageSize.ISOC0 => "ISO C0",
                PrintSettings.PageSize.ISOC1 => "ISO C1",
                PrintSettings.PageSize.ISOC2 => "ISO C2",
                PrintSettings.PageSize.ISOC3 => "ISO C3",
                PrintSettings.PageSize.ISOC3Envelope => "ISO C3 Envelope",
                PrintSettings.PageSize.ISOC4 => "ISO C4",
                PrintSettings.PageSize.ISOC4Envelope => "ISO C4 Envelope",
                PrintSettings.PageSize.ISOC5 => "ISO C5",
                PrintSettings.PageSize.ISOC5Envelope => "ISO C5 Envelope",
                PrintSettings.PageSize.ISOC6 => "ISO C6",
                PrintSettings.PageSize.ISOC6C5Envelope => "ISO C6C5 Envelope",
                PrintSettings.PageSize.ISOC6Envelope => "ISO C6 Envelope",
                PrintSettings.PageSize.ISOC7 => "ISO C7",
                PrintSettings.PageSize.ISOC8 => "ISO C8",
                PrintSettings.PageSize.ISOC9 => "ISO C9",
                PrintSettings.PageSize.ISOC10 => "ISO C10",
                PrintSettings.PageSize.ISODLEnvelope => "ISO DL Envelope",
                PrintSettings.PageSize.ISODLEnvelopeRotated => "ISO DL Envelope Rotated",
                PrintSettings.PageSize.ISOSRA3 => "ISO SRA3",
                PrintSettings.PageSize.Japan2LPhoto => "Japan 2L Photo",
                PrintSettings.PageSize.JapanChou3Envelope => "Japan Chou 3 Envelope",
                PrintSettings.PageSize.JapanChou3EnvelopeRotated => "Japan Chou 3 Envelope Rotated",
                PrintSettings.PageSize.JapanChou4Envelope => "Japan Chou 4 Envelope",
                PrintSettings.PageSize.JapanChou4EnvelopeRotated => "Japan Chou 4 Envelope Rotated",
                PrintSettings.PageSize.JapanDoubleHagakiPostcard => "Japan Double Hagaki Postcard",
                PrintSettings.PageSize.JapanDoubleHagakiPostcardRotated => "Japan Double Hagaki Postcard Rotated",
                PrintSettings.PageSize.JapanHagakiPostcard => "Japan Hagaki Postcard",
                PrintSettings.PageSize.JapanHagakiPostcardRotated => "Japan Hagaki Postcard Rotated",
                PrintSettings.PageSize.JapanKaku2Envelope => "Japan Kaku 2 Envelope",
                PrintSettings.PageSize.JapanKaku2EnvelopeRotated => "Japan Kaku 2 Envelope Rotated",
                PrintSettings.PageSize.JapanKaku3Envelope => "Japan Kaku 3 Envelope",
                PrintSettings.PageSize.JapanKaku3EnvelopeRotated => "Japan Kaku 3 Envelope Rotated",
                PrintSettings.PageSize.JapanLPhoto => "Japan L Photo",
                PrintSettings.PageSize.JapanQuadrupleHagakiPostcard => "Japan Quadruple Hagaki Postcard",
                PrintSettings.PageSize.JapanYou1Envelope => "Japan You 1 Envelope",
                PrintSettings.PageSize.JapanYou2Envelope => "Japan You 2 Envelope",
                PrintSettings.PageSize.JapanYou3Envelope => "Japan You 3 Envelope",
                PrintSettings.PageSize.JapanYou4Envelope => "Japan You 4 Envelope",
                PrintSettings.PageSize.JapanYou4EnvelopeRotated => "Japan You 4 Envelope Rotated",
                PrintSettings.PageSize.JapanYou6Envelope => "Japan You 6 Envelope",
                PrintSettings.PageSize.JapanYou6EnvelopeRotated => "Japan You 6 Envelope Rotated",
                PrintSettings.PageSize.JISB0 => "JIS B0",
                PrintSettings.PageSize.JISB1 => "JIS B1",
                PrintSettings.PageSize.JISB10 => "JIS B10",
                PrintSettings.PageSize.JISB2 => "JIS B2",
                PrintSettings.PageSize.JISB3 => "JIS B3",
                PrintSettings.PageSize.JISB4 => "JIS B4",
                PrintSettings.PageSize.JISB4Rotated => "JIS B4 Rotated",
                PrintSettings.PageSize.JISB5 => "JIS B5",
                PrintSettings.PageSize.JISB5Rotated => "JIS B5 Rotated",
                PrintSettings.PageSize.JISB6 => "JIS B6",
                PrintSettings.PageSize.JISB6Rotated => "JIS B6 Rotated",
                PrintSettings.PageSize.JISB7 => "JIS B7",
                PrintSettings.PageSize.JISB8 => "JIS B8",
                PrintSettings.PageSize.JISB9 => "JIS B9",
                PrintSettings.PageSize.NorthAmerica10x11 => "North America 10 x 11",
                PrintSettings.PageSize.NorthAmerica10x12 => "North America 10 x 12",
                PrintSettings.PageSize.NorthAmerica10x14 => "North America 10 x 14",
                PrintSettings.PageSize.NorthAmerica11x17 => "North America 11 x 17",
                PrintSettings.PageSize.NorthAmerica14x17 => "North America 14 x 17",
                PrintSettings.PageSize.NorthAmerica4x6 => "North America 4 x 6",
                PrintSettings.PageSize.NorthAmerica4x8 => "North America 4 x 8",
                PrintSettings.PageSize.NorthAmerica5x7 => "North America 5 x 7",
                PrintSettings.PageSize.NorthAmerica8x10 => "North America 8 x 10",
                PrintSettings.PageSize.NorthAmerica9x11 => "North America 9 x 11",
                PrintSettings.PageSize.NorthAmericaArchitectureASheet => "North America Architecture A Sheet",
                PrintSettings.PageSize.NorthAmericaArchitectureBSheet => "North America Architecture B Sheet",
                PrintSettings.PageSize.NorthAmericaArchitectureCSheet => "North America Architecture C Sheet",
                PrintSettings.PageSize.NorthAmericaArchitectureDSheet => "North America Architecture D Sheet",
                PrintSettings.PageSize.NorthAmericaArchitectureESheet => "North America Architecture E Sheet",
                PrintSettings.PageSize.NorthAmericaCSheet => "North America C Sheet",
                PrintSettings.PageSize.NorthAmericaDSheet => "North America D Sheet",
                PrintSettings.PageSize.NorthAmericaESheet => "North America E Sheet",
                PrintSettings.PageSize.NorthAmericaExecutive => "North America Executive",
                PrintSettings.PageSize.NorthAmericaGermanLegalFanfold => "North America German Legal Fanfold",
                PrintSettings.PageSize.NorthAmericaGermanStandardFanfold => "North America German Standard Fanfold",
                PrintSettings.PageSize.NorthAmericaLegal => "North America Legal",
                PrintSettings.PageSize.NorthAmericaLegalExtra => "North America Legal Extra",
                PrintSettings.PageSize.NorthAmericaLetter => "North America Letter",
                PrintSettings.PageSize.NorthAmericaLetterExtra => "North America Letter Extra",
                PrintSettings.PageSize.NorthAmericaLetterPlus => "North America Letter Plus",
                PrintSettings.PageSize.NorthAmericaLetterRotated => "North America Letter Rotated",
                PrintSettings.PageSize.NorthAmericaMonarchEnvelope => "North America Monarch Envelope",
                PrintSettings.PageSize.NorthAmericaNote => "North America Note",
                PrintSettings.PageSize.NorthAmericaNumber10Envelope => "North America Number 10 Envelope",
                PrintSettings.PageSize.NorthAmericaNumber10EnvelopeRotated => "North America Number 10 Envelope Rotated",
                PrintSettings.PageSize.NorthAmericaNumber11Envelope => "North America Number 11 Envelope",
                PrintSettings.PageSize.NorthAmericaNumber12Envelope => "North America Number 12 Envelope",
                PrintSettings.PageSize.NorthAmericaNumber14Envelope => "North America Number 14 Envelope",
                PrintSettings.PageSize.NorthAmericaNumber9Envelope => "North America Number 9 Envelope",
                PrintSettings.PageSize.NorthAmericaPersonalEnvelope => "North America Personal Envelope",
                PrintSettings.PageSize.NorthAmericaQuarto => "North America Quarto",
                PrintSettings.PageSize.NorthAmericaStatement => "North America Statement",
                PrintSettings.PageSize.NorthAmericaSuperA => "North America Super A",
                PrintSettings.PageSize.NorthAmericaSuperB => "North America Super B",
                PrintSettings.PageSize.NorthAmericaTabloid => "North America Tabloid",
                PrintSettings.PageSize.NorthAmericaTabloidExtra => "North America Tabloid Extra",
                PrintSettings.PageSize.OtherMetricA3Plus => "A3 Plus",
                PrintSettings.PageSize.OtherMetricA4Plus => "A4 Plus",
                PrintSettings.PageSize.OtherMetricFolio => "Folio",
                PrintSettings.PageSize.OtherMetricInviteEnvelope => "Invite Envelope",
                PrintSettings.PageSize.OtherMetricItalianEnvelope => "Italian Envelope",
                PrintSettings.PageSize.PRC10Envelope => "PRC #10 Envelope",
                PrintSettings.PageSize.PRC10EnvelopeRotated => "PRC #10 Envelope Rotated",
                PrintSettings.PageSize.PRC16K => "PRC 16K",
                PrintSettings.PageSize.PRC16KRotated => "PRC 16K Rotated",
                PrintSettings.PageSize.PRC1Envelope => "PRC #1 Envelope",
                PrintSettings.PageSize.PRC1EnvelopeRotated => "PRC #1 Envelope Rotated",
                PrintSettings.PageSize.PRC2Envelope => "PRC #2 Envelope",
                PrintSettings.PageSize.PRC2EnvelopeRotated => "PRC #2 Envelope Rotated",
                PrintSettings.PageSize.PRC32K => "PRC 32K",
                PrintSettings.PageSize.PRC32KBig => "PRC 32K Big",
                PrintSettings.PageSize.PRC32KRotated => "PRC 32K Rotated",
                PrintSettings.PageSize.PRC3Envelope => "PRC #3 Envelope",
                PrintSettings.PageSize.PRC3EnvelopeRotated => "PRC #3 Envelope Rotated",
                PrintSettings.PageSize.PRC4Envelope => "PRC #4 Envelope",
                PrintSettings.PageSize.PRC4EnvelopeRotated => "PRC #4 Envelope Rotated",
                PrintSettings.PageSize.PRC5Envelope => "PRC #5 Envelope",
                PrintSettings.PageSize.PRC5EnvelopeRotated => "PRC #5 Envelope Rotated",
                PrintSettings.PageSize.PRC6Envelope => "PRC #6 Envelope",
                PrintSettings.PageSize.PRC6EnvelopeRotated => "PRC #6 Envelope Rotated",
                PrintSettings.PageSize.PRC7Envelope => "PRC #7 Envelope",
                PrintSettings.PageSize.PRC7EnvelopeRotated => "PRC #7 Envelope Rotated",
                PrintSettings.PageSize.PRC8Envelope => "PRC #8 Envelope",
                PrintSettings.PageSize.PRC8EnvelopeRotated => "PRC #8 Envelope Rotated",
                PrintSettings.PageSize.PRC9Envelope => "PRC #9 Envelope",
                PrintSettings.PageSize.PRC9EnvelopeRotated => "PRC #9 Envelope Rotated",
                PrintSettings.PageSize.Roll04Inch => "4-inch Wide Roll",
                PrintSettings.PageSize.Roll06Inch => "6-inch Wide Roll",
                PrintSettings.PageSize.Roll08Inch => "8-inch Wide Roll",
                PrintSettings.PageSize.Roll12Inch => "12-inch Wide Roll",
                PrintSettings.PageSize.Roll15Inch => "15-inch Wide Roll",
                PrintSettings.PageSize.Roll18Inch => "18-inch Wide Roll",
                PrintSettings.PageSize.Roll22Inch => "22-inch Wide Roll",
                PrintSettings.PageSize.Roll24Inch => "24-inch Wide Roll",
                PrintSettings.PageSize.Roll30Inch => "30-inch Wide Roll",
                PrintSettings.PageSize.Roll36Inch => "36-inch Wide Roll",
                PrintSettings.PageSize.Roll54Inch => "54-inch Wide Roll",

                _ => "Unknown Size",
            };
        }

        /// <summary>
        /// Get the name info of <see cref="PageMediaType"/> object.
        /// </summary>
        /// <param name="type">The <see cref="PageMediaType"/> object of page type.</param>
        /// <returns>The name info.</returns>
        public static string GetPageMediaTypeNameInfo(PageMediaType type)
        {
            return type switch
            {
                PageMediaType.Archival => "Archival",
                PageMediaType.AutoSelect => "Auto Select",
                PageMediaType.BackPrintFilm => "Back Print Film",
                PageMediaType.Bond => "Bond",
                PageMediaType.CardStock => "Card Stock",
                PageMediaType.Continuous => "Continuous",
                PageMediaType.EnvelopePlain => "Envelope Plain",
                PageMediaType.EnvelopeWindow => "Envelope Window",
                PageMediaType.Fabric => "Fabric",
                PageMediaType.HighResolution => "High Resolution",
                PageMediaType.Label => "Label",
                PageMediaType.MultiLayerForm => "Multi Layer Form",
                PageMediaType.MultiPartForm => "Multi Part Form",
                PageMediaType.Photographic => "Photographic",
                PageMediaType.PhotographicFilm => "Photographic Film",
                PageMediaType.PhotographicGlossy => "Photographic Glossy",
                PageMediaType.PhotographicHighGloss => "Photographic High Gloss",
                PageMediaType.PhotographicMatte => "Photographic Matte",
                PageMediaType.PhotographicSatin => "Photographic Satin",
                PageMediaType.PhotographicSemiGloss => "Photographic Semi Gloss",
                PageMediaType.Plain => "Plain",
                PageMediaType.Screen => "Screen",
                PageMediaType.ScreenPaged => "Screen Paged",
                PageMediaType.Stationery => "Stationery",
                PageMediaType.TabStockFull => "Tab Stock Full",
                PageMediaType.TabStockPreCut => "Tab Stock Pre Cut",
                PageMediaType.Transparency => "Transparency",
                PageMediaType.TShirtTransfer => "T-shirt Transfer",

                _ => "Unknown Type",
            };
        }

        /// <summary>
        /// Get the name info of <see cref="PrintSettings.PageType"/> object.
        /// </summary>
        /// <param name="type">The <see cref="PrintSettings.PageType"/> object of page type.</param>
        /// <returns>The name info.</returns>
        public static string GetPageMediaTypeNameInfo(PrintSettings.PageType type)
        {
            return type switch
            {
                PrintSettings.PageType.Archival => "Archival",
                PrintSettings.PageType.AutoSelect => "Auto Select",
                PrintSettings.PageType.BackPrintFilm => "Back Print Film",
                PrintSettings.PageType.Bond => "Bond",
                PrintSettings.PageType.CardStock => "Card Stock",
                PrintSettings.PageType.Continuous => "Continuous",
                PrintSettings.PageType.EnvelopePlain => "Envelope Plain",
                PrintSettings.PageType.EnvelopeWindow => "Envelope Window",
                PrintSettings.PageType.Fabric => "Fabric",
                PrintSettings.PageType.HighResolution => "High Resolution",
                PrintSettings.PageType.Label => "Label",
                PrintSettings.PageType.MultiLayerForm => "Multi Layer Form",
                PrintSettings.PageType.MultiPartForm => "Multi Part Form",
                PrintSettings.PageType.Photographic => "Photographic",
                PrintSettings.PageType.PhotographicFilm => "Photographic Film",
                PrintSettings.PageType.PhotographicGlossy => "Photographic Glossy",
                PrintSettings.PageType.PhotographicHighGloss => "Photographic High Gloss",
                PrintSettings.PageType.PhotographicMatte => "Photographic Matte",
                PrintSettings.PageType.PhotographicSatin => "Photographic Satin",
                PrintSettings.PageType.PhotographicSemiGloss => "Photographic Semi Gloss",
                PrintSettings.PageType.Plain => "Plain",
                PrintSettings.PageType.Screen => "Screen",
                PrintSettings.PageType.ScreenPaged => "Screen Paged",
                PrintSettings.PageType.Stationery => "Stationery",
                PrintSettings.PageType.TabStockFull => "Tab Stock Full",
                PrintSettings.PageType.TabStockPreCut => "Tab Stock Pre Cut",
                PrintSettings.PageType.Transparency => "Transparency",
                PrintSettings.PageType.TShirtTransfer => "T-shirt Transfer",

                _ => "Unknown Type",
            };
        }

        /// <summary>
        /// Get the name info of <see cref="InputBin"/> object.
        /// </summary>
        /// <param name="inputBin">The <see cref="InputBin"/> object of page source.</param>
        /// <returns>The name info.</returns>
        public static string GetInputBinNameInfo(InputBin inputBin)
        {
            return inputBin switch
            {
                InputBin.AutoSelect => "Auto Select",
                InputBin.AutoSheetFeeder => "Auto Sheet Feeder",
                InputBin.Cassette => "Cassette",
                InputBin.Manual => "Manual",
                InputBin.Tractor => "Tractor",

                _ => "Unknown Input Bin",
            };
        }
    }
}

namespace PrintDialogX.DocumentMaker
{
    public class DocumentMaker
    {
        /// <summary>
        /// Initialize a <see cref="DocumentMaker"/> class.
        /// </summary>
        protected DocumentMaker()
        {
            return;
        }

        /// <summary>
        /// Make a document that contains an auto pagination <see cref="System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="xaml">The XAML that represents the <see cref="System.Windows.UIElement"/>.</param>
        /// <param name="elementActualHeight">The <see cref="System.Windows.UIElement"/> actual height.</param>
        /// <returns>The document.</returns>
        public static FixedDocument PaginatonUIElementDocumentMaker(string xaml, double elementActualHeight)
        {
            return PaginatonUIElementDocumentMaker(xaml, elementActualHeight, null, 34, new Size(96 * 8.25, 96 * 11.75));
        }

        /// <summary>
        /// Make a document that contains an auto pagination <see cref="System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="xaml">The XAML that represents the <see cref="System.Windows.UIElement"/>.</param>
        /// <param name="elementActualHeight">The <see cref="System.Windows.UIElement"/> actual height.</param>
        /// <returns>The document.</returns>
        public static FixedDocument PaginatonUIElementDocumentMaker(string xaml, double elementActualHeight, object dataContext)
        {
            return PaginatonUIElementDocumentMaker(xaml, elementActualHeight, dataContext, 34, new Size(96 * 8.25, 96 * 11.75));
        }

        /// <summary>
        /// Make a document that contains an auto pagination <see cref="System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="xaml">The XAML that represents the <see cref="System.Windows.UIElement"/>.</param>
        /// <param name="elementActualHeight">The <see cref="System.Windows.UIElement"/> actual height.</param>
        /// <param name="dataContext">The <see cref="System.Windows.UIElement"/> data context.</param>
        /// <param name="documentMargin">The document margin.</param>
        /// <param name="documentSize">The document size.</param>
        /// <param name="documentOrientation">The document orientation.</param>
        /// <returns>The document.</returns>
        public static FixedDocument PaginatonUIElementDocumentMaker(string xaml, double elementActualHeight, object dataContext, double documentMargin, Size pageSize)
        {
            return PaginatonUIElementDocumentMaker(xaml, elementActualHeight, dataContext, documentMargin, pageSize);
        }

        /// <summary>
        /// Make a document that contains an auto pagination <see cref="System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="xaml">The XAML that represents the <see cref="System.Windows.UIElement"/>.</param>
        /// <param name="elementActualHeight">The <see cref="System.Windows.UIElement"/> actual height.</param>
        /// <param name="dataContext">The <see cref="System.Windows.UIElement"/> data context.</param>
        /// <param name="documentMargin">The document margin.</param>
        /// <param name="documentSize">The document size.</param>
        /// <param name="documentOrientation">The document orientation.</param>
        /// <returns>The document.</returns>
        public static FixedDocument PaginatonUIElementDocumentMaker(string xaml, double elementActualHeight, object dataContext, double documentMargin, Size documentSize, PrintSettings.PageOrientation documentOrientation)
        {
            FixedDocument doc = new FixedDocument();
            if (documentOrientation == PrintSettings.PageOrientation.Portrait)
            {
                doc.DocumentPaginator.PageSize = documentSize;
            }
            else
            {
                doc.DocumentPaginator.PageSize = new Size(documentSize.Height, documentSize.Width);
            }

            int totalPage = (int)Math.Ceiling(elementActualHeight / (doc.DocumentPaginator.PageSize.Height - documentMargin * 2));

            for (int i = 0; i < totalPage; i++)
            {
                FrameworkElement printedElement = XamlReader.Parse(xaml) as FrameworkElement;
                printedElement.DataContext = dataContext;

                FixedPage fixedPage = new FixedPage
                {
                    Width = doc.DocumentPaginator.PageSize.Width,
                    Height = doc.DocumentPaginator.PageSize.Height
                };

                printedElement.Width = fixedPage.Width - documentMargin * 2;
                printedElement.Height = elementActualHeight;

                FixedPage.SetLeft(printedElement, documentMargin);
                FixedPage.SetTop(printedElement, -(i * (fixedPage.Height - documentMargin * 2)) + documentMargin);

                Grid clipGrid1 = new Grid()
                {
                    Width = fixedPage.Width,
                    Height = documentMargin,
                    Background = Brushes.White
                };

                Grid clipGrid2 = new Grid()
                {
                    Width = fixedPage.Width,
                    Height = documentMargin,
                    Background = Brushes.White
                };

                FixedPage.SetLeft(clipGrid1, 0);
                FixedPage.SetTop(clipGrid1, 0);

                FixedPage.SetLeft(clipGrid2, 0);
                FixedPage.SetTop(clipGrid2, fixedPage.Height - documentMargin);

                fixedPage.Children.Add(printedElement);
                fixedPage.Children.Add(clipGrid1);
                fixedPage.Children.Add(clipGrid2);

                fixedPage.Measure(doc.DocumentPaginator.PageSize);
                fixedPage.Arrange(new Rect(new Point(), doc.DocumentPaginator.PageSize));

                doc.Pages.Add(new PageContent() { Child = fixedPage });

                fixedPage.UpdateLayout();
                PrintControl.PrintPage.DoEvents();
            }

            return doc;
        }

        /// <summary>
        /// Make a document that contains an auto pagination <see cref="System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="xaml">The XAML that represents the <see cref="System.Windows.UIElement"/>.</param>
        /// <param name="pageCount">The total page count.</param>
        /// <returns>The document.</returns>
        public static FixedDocument PaginatonUIElementDocumentMaker(string xaml, int pageCount)
        {
            return PaginatonUIElementDocumentMaker(xaml, pageCount, null, 34, new Size(96 * 8.25, 96 * 11.75));
        }

        /// <summary>
        /// Make a document that contains an auto pagination <see cref="System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="xaml">The XAML that represents the <see cref="System.Windows.UIElement"/>.</param>
        /// <param name="pageCount">The total page count.</param>
        /// <returns>The document.</returns>
        public static FixedDocument PaginatonUIElementDocumentMaker(string xaml, int pageCount, object dataContext)
        {
            return PaginatonUIElementDocumentMaker(xaml, pageCount, dataContext, 34, new Size(96 * 8.25, 96 * 11.75));
        }

        /// <summary>
        /// Make a document that contains an auto pagination <see cref="System.Windows.UIElement"/>.
        /// </summary>
        /// <param name="xaml">The XAML that represents the <see cref="System.Windows.UIElement"/>.</param>
        /// <param name="pageCount">The total page count.</param>
        /// <param name="dataContext">The <see cref="System.Windows.UIElement"/> data context.</param>
        /// <param name="documentMargin">The document margin.</param>
        /// <param name="documentSize">The document size.</param>
        /// <param name="documentOrientation">The document orientation.</param>
        /// <returns>The document.</returns>
        public static FixedDocument PaginatonUIElementDocumentMaker(string xaml, int pageCount, object dataContext, double documentMargin, Size pageSize)
        {
            FixedDocument doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = pageSize;

            int totalPage = pageCount;

            for (int i = 0; i < totalPage; i++)
            {
                FrameworkElement printedElement = XamlReader.Parse(xaml) as FrameworkElement;
                printedElement.DataContext = dataContext;

                FixedPage fixedPage = new FixedPage
                {
                    Width = doc.DocumentPaginator.PageSize.Width,
                    Height = doc.DocumentPaginator.PageSize.Height
                };

                printedElement.Width = fixedPage.Width - documentMargin * 2;
                printedElement.Height = double.NaN;

                FixedPage.SetLeft(printedElement, documentMargin);
                FixedPage.SetTop(printedElement, -(i * (fixedPage.Height - documentMargin * 2)) + documentMargin);

                Grid clipGrid1 = new Grid()
                {
                    Width = fixedPage.Width,
                    Height = documentMargin,
                    Background = Brushes.White
                };

                Grid clipGrid2 = new Grid()
                {
                    Width = fixedPage.Width,
                    Height = documentMargin,
                    Background = Brushes.White
                };

                FixedPage.SetLeft(clipGrid1, 0);
                FixedPage.SetTop(clipGrid1, 0);

                FixedPage.SetLeft(clipGrid2, 0);
                FixedPage.SetTop(clipGrid2, fixedPage.Height - documentMargin);

                fixedPage.Children.Add(printedElement);
                fixedPage.Children.Add(clipGrid1);
                fixedPage.Children.Add(clipGrid2);

                fixedPage.Measure(doc.DocumentPaginator.PageSize);
                fixedPage.Arrange(new Rect(new Point(), doc.DocumentPaginator.PageSize));

                doc.Pages.Add(new PageContent() { Child = fixedPage });

                fixedPage.UpdateLayout();
                PrintControl.PrintPage.DoEvents();
            }

            return doc;
        }
    }
}

namespace PrintDialogX.PrintControl.PrinterInfoHelper
{
    internal class PrinterIconHelper
    {
        protected PrinterIconHelper()
        {
            return;
        }

        public static readonly Uri NormalPrinterIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Normal.png", UriKind.Relative);
        public static readonly Uri NormalDefaultPrinterIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Normal (Default).png", UriKind.Relative);
        public static readonly Uri FaxPrinterIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Fax.png", UriKind.Relative);
        public static readonly Uri FaxDefaultPrinterIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Fax (Default).png", UriKind.Relative);
        public static readonly Uri NetworkFaxPrinterIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Network Fax.png", UriKind.Relative);
        public static readonly Uri NetworkFaxDefaultPrinterIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Network Fax (Default).png", UriKind.Relative);
        public static readonly Uri NormalPrinterSmallIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Normal - Small.png", UriKind.Relative);
        public static readonly Uri NormalDefaultPrinterSmallIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Normal (Default) - Small.png", UriKind.Relative);
        public static readonly Uri FaxPrinterSmallIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Fax - Small.png", UriKind.Relative);
        public static readonly Uri FaxDefaultPrinterSmallIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Fax (Default) - Small.png", UriKind.Relative);
        public static readonly Uri NetworkFaxPrinterSmallIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Network Fax - Small.png", UriKind.Relative);
        public static readonly Uri NetworkFaxDefaultPrinterSmallIcon = new Uri("/PrintDialog;component/Resources/PrinterIcons/Network Fax (Default) - Small.png", UriKind.Relative);

        public static ImageSource GetPrinterIcon(PrintQueue printer, PrintServer printServer, bool smallIcon = false)
        {
            string printerType = "Normal";

            foreach (PrintQueue printQueue in printServer.GetPrintQueues(new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.Fax }))
            {
                if (printQueue.FullName == printer.FullName)
                {
                    printerType = "Fax";
                    break;
                }
            }

            if (printerType == "Fax")
            {
                if (printer.IsShared == true)
                {
                    printerType = "NetworkFax";
                }
            }

            if (LocalPrintServer.GetDefaultPrintQueue().FullName == printer.FullName)
            {
                printerType += "_Default";
            }
            if (smallIcon == true)
            {
                printerType += "_Small";
            }

            return printerType switch
            {
                "Normal" => new BitmapImage(NormalPrinterIcon),
                "Normal_Default" => new BitmapImage(NormalDefaultPrinterIcon),
                "Fax" => new BitmapImage(FaxPrinterIcon),
                "Fax_Default" => new BitmapImage(FaxDefaultPrinterIcon),
                "NetworkFax" => new BitmapImage(NetworkFaxPrinterIcon),
                "NetworkFax_Default" => new BitmapImage(NetworkFaxDefaultPrinterIcon),

                "Normal_Small" => new BitmapImage(NormalPrinterSmallIcon),
                "Normal_Default_Small" => new BitmapImage(NormalDefaultPrinterSmallIcon),
                "Fax_Small" => new BitmapImage(FaxPrinterSmallIcon),
                "Fax_Default_Small" => new BitmapImage(FaxDefaultPrinterSmallIcon),
                "NetworkFax_Small" => new BitmapImage(NetworkFaxPrinterSmallIcon),
                "NetworkFax_Default_Small" => new BitmapImage(NetworkFaxDefaultPrinterSmallIcon),

                _ => new BitmapImage(NormalPrinterIcon),
            };
        }
    }
}

namespace PrintDialogX.PrintControl.PreviewHelper
{
    internal enum DocumentOrientation
    {
        Portrait,
        Landscape
    }

    internal enum PageOrder
    {
        Horizontal,
        HorizontalReverse,
        Vertical,
        VerticalReverse
    }

    internal class MultiPagesPerSheetHelper
    {
        private readonly int _pagesPerSheet;
        private readonly Size _innerPageSize;
        private readonly PageOrder _pageOrder;
        private readonly FixedDocument _document;
        private readonly DocumentOrientation _documentOrientation;

        private readonly List<int> _allowedPagesPerSheetCount = new List<int> { 1, 2, 4, 6, 9, 16 };
        private readonly List<Size> _allowedPagesPerSheetArrange = new List<Size> { new Size(1, 1), new Size(1, 2), new Size(2, 2), new Size(2, 3), new Size(3, 3), new Size(4, 4) };

        public MultiPagesPerSheetHelper()
        {
            _document = null;
            _pagesPerSheet = 0;
            _innerPageSize = new Size();
            _pageOrder = PageOrder.Horizontal;
            _documentOrientation = DocumentOrientation.Portrait;
        }

        public MultiPagesPerSheetHelper(int pagesPerSheet, FixedDocument document)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _innerPageSize = new Size();
            _pageOrder = PageOrder.Horizontal;
            _documentOrientation = DocumentOrientation.Portrait;
        }

        public MultiPagesPerSheetHelper(int pagesPerSheet, FixedDocument document, Size innerPageSize)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _innerPageSize = innerPageSize;
            _pageOrder = PageOrder.Horizontal;
            _documentOrientation = DocumentOrientation.Portrait;
        }

        public MultiPagesPerSheetHelper(int pagesPerSheet, FixedDocument document, Size innerPageSize, DocumentOrientation documentOrientation)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _innerPageSize = innerPageSize;
            _pageOrder = PageOrder.Horizontal;
            _documentOrientation = documentOrientation;
        }

        public MultiPagesPerSheetHelper(int pagesPerSheet, FixedDocument document, Size innerPageSize, DocumentOrientation documentOrientation, PageOrder pageOrder)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _innerPageSize = innerPageSize;
            _pageOrder = pageOrder;
            _documentOrientation = documentOrientation;
        }

        public static int GetPagePerSheetCountIndex(int pagesPerSheetCount)
        {
            List<int> _allowedPagesPerSheetCount = new List<int> { 1, 2, 4, 6, 9, 16 };

            if (_allowedPagesPerSheetCount.Contains(pagesPerSheetCount))
            {
                return _allowedPagesPerSheetCount.IndexOf(pagesPerSheetCount);
            }
            else
            {
                return -1;
            }
        }

        public FixedDocument GetMultiPagesPerSheetDocument(double scale)
        {
            if (_document == null && _document.Pages.Count == 0 && _pagesPerSheet == 0)
            {
                return new FixedDocument();
            }
            if (_allowedPagesPerSheetCount.Contains(_pagesPerSheet) == false)
            {
                return new FixedDocument();
            }

            FixedDocument doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = _document.DocumentPaginator.PageSize;

            int currentPageCount = 0;
            int pagesPerSheetIndex = _allowedPagesPerSheetCount.IndexOf(_pagesPerSheet);
            int arrangeRows;
            int arrangeColumns;
            if (_documentOrientation == DocumentOrientation.Portrait)
            {
                arrangeRows = (int)_allowedPagesPerSheetArrange[pagesPerSheetIndex].Height;
                arrangeColumns = (int)_allowedPagesPerSheetArrange[pagesPerSheetIndex].Width;
            }
            else
            {
                arrangeRows = (int)_allowedPagesPerSheetArrange[pagesPerSheetIndex].Width;
                arrangeColumns = (int)_allowedPagesPerSheetArrange[pagesPerSheetIndex].Height;
            }
            double innerPageHeight = doc.DocumentPaginator.PageSize.Height / arrangeRows;
            double innerPageWidth = doc.DocumentPaginator.PageSize.Width / arrangeColumns;

            while (currentPageCount < _document.Pages.Count)
            {
                FixedPage outerPage = new FixedPage
                {
                    Width = doc.DocumentPaginator.PageSize.Width,
                    Height = doc.DocumentPaginator.PageSize.Height
                };

                Grid outergrid = new Grid()
                {
                    Width = outerPage.Width,
                    Height = outerPage.Height
                };

                for (int i = 0; i < arrangeRows; i++)
                {
                    outergrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                }
                for (int i = 0; i < arrangeColumns; i++)
                {
                    outergrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }

                if (_pageOrder == PageOrder.Horizontal)
                {
                    for (int i = 0; i < arrangeRows; i++)
                    {
                        for (int j = 0; j < arrangeColumns && currentPageCount < _document.Pages.Count; j++)
                        {
                            outergrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PageOrder.HorizontalReverse)
                {
                    for (int i = 0; i < arrangeRows; i++)
                    {
                        for (int j = arrangeColumns - 1; j >= 0 && currentPageCount < _document.Pages.Count; j--)
                        {
                            outergrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PageOrder.Vertical)
                {
                    for (int j = 0; j < arrangeColumns; j++)
                    {
                        for (int i = 0; i < arrangeRows && currentPageCount < _document.Pages.Count; i++)
                        {
                            outergrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PageOrder.VerticalReverse)
                {
                    for (int j = 0; j < arrangeColumns; j++)
                    {
                        for (int i = arrangeRows - 1; i >= 0 && currentPageCount < _document.Pages.Count; i--)
                        {
                            outergrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else
                {
                    return new FixedDocument();
                }

                outerPage.Children.Add(outergrid);

                if (scale != double.NaN)
                {
                    outerPage.RenderTransformOrigin = new Point(0, 0);
                    outerPage.RenderTransform = new ScaleTransform(scale / 100.0, scale / 100.0);
                }

                doc.Pages.Add(new PageContent { Child = outerPage });
            }

            return doc;
        }

        private Grid GetMultiPagesPerSheetGrid(int column, int row, int currentPageCount, double innerPageWidth, double innerPageHeight, double outerPageWidth, double outerPageHeight)
        {
            Grid innerGrid = new Grid()
            {
                Width = innerPageWidth,
                Height = innerPageHeight,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            innerGrid.SetValue(Grid.RowProperty, column);
            innerGrid.SetValue(Grid.ColumnProperty, row);

            string xaml = XamlWriter.Save(_document.Pages[currentPageCount].Child);
            FixedPage innerPage = XamlReader.Parse(xaml) as FixedPage;

            if (_innerPageSize.Width == 0 || _innerPageSize.Height == 0 || _innerPageSize.Width == double.NaN || _innerPageSize.Height == double.NaN)
            {
                innerPage.Width = outerPageWidth;
                innerPage.Height = outerPageHeight;
            }
            else
            {
                innerPage.Width = _innerPageSize.Width;
                innerPage.Height = _innerPageSize.Height;
            }

            innerPage.VerticalAlignment = VerticalAlignment.Center;
            innerPage.HorizontalAlignment = HorizontalAlignment.Center;

            double widthRatio;
            double heightRatio;

            widthRatio = innerPageWidth / innerPage.Width;
            heightRatio = innerPageHeight / innerPage.Height;

            if (innerPage.Height * widthRatio <= innerPageHeight)
            {
                innerPage.Width *= widthRatio;
                innerPage.Height *= widthRatio;

                innerPage.RenderTransform = new ScaleTransform(widthRatio, widthRatio);
            }
            else
            {
                innerPage.Width *= heightRatio;
                innerPage.Height *= heightRatio;

                innerPage.RenderTransform = new ScaleTransform(heightRatio, heightRatio);
            }

            innerGrid.Children.Add(innerPage);
            return innerGrid;
        }
    }
}

namespace PrintDialogX.PrintDialogExceptions
{
    public class DocumentEmptyException : Exception
    {
        /// <summary>
        /// The message that describes the current exception.
        /// </summary>
        public override string Message { get; }

        /// <summary>
        /// The <see cref="FixedDocument"/> that is empty.
        /// </summary>
        public FixedDocument Document { get; }

        /// <summary>
        /// Initialize a <see cref="DocumentEmptyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the current exception.</param>
        /// <param name="document">The FixedDocument that is empty.</param>
        public DocumentEmptyException(string message, FixedDocument document)
        {
            Message = message;
            Document = document;
        }
    }

    public class UndefinedException : Exception
    {
        /// <summary>
        /// The message that describes the current exception.
        /// </summary>
        public override string Message { get; }

        /// <summary>
        /// The <see cref="System.Exception"/> instance that caused the current exception.
        /// </summary>
        public new Exception InnerException { get; }

        /// <summary>
        /// The time that exception throw out.
        /// </summary>
        public DateTime ExceptionTime { get; }

        /// <summary>
        /// Initialize a <see cref="UndefinedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the current exception.</param>
        public UndefinedException(string message)
        {
            Message = message;
            ExceptionTime = DateTime.Now;
        }

        /// <summary>
        /// Initialize a <see cref="UndefinedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the current exception.</param>
        /// <param name="innerException">The undefined exception.</param>
        public UndefinedException(string message, Exception innerException)
        {
            Message = message;
            InnerException = innerException;
            ExceptionTime = DateTime.Now;
        }
    }
}
