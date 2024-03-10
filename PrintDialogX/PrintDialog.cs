using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
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
        }

        /// <summary>
        /// Use the default settings of the printer or not. This will override all other settings.
        /// </summary>
        public bool UsePrinterDefaultSettings { get; internal set; } = false;

        public PrintSettings.PageOrientation Layout { get; set; } = PrintSettings.PageOrientation.Portrait;
        public PrintSettings.PageColor Color { get; set; } = PrintSettings.PageColor.Color;
        public PrintSettings.PageQuality Quality { get; set; } = PrintSettings.PageQuality.Normal;
        public PrintSettings.PageSize PageSize { get; set; } = PrintSettings.PageSize.ISOA4;
        public PrintSettings.PageType PageType { get; set; } = PrintSettings.PageType.Plain;
        public PrintSettings.DoubleSided DoubleSided { get; set; } = PrintSettings.DoubleSided.OneSided;
        public PrintSettings.PagesPerSheet PagesPerSheet { get; set; } = PrintSettings.PagesPerSheet.One;
        public PrintSettings.PageOrder PageOrder { get; set; } = PrintSettings.PageOrder.Horizontal;

        /// <summary>
        /// Initialize a <see cref="PrintDialogSettings"/> that uses the default settings of the printer
        /// </summary>
        public static PrintDialogSettings PrinterDefaultSettings()
        {
            return new PrintDialogSettings()
            {
                UsePrinterDefaultSettings = true
            };
        }
    }

    public class DocumentInfo
    {
        /// <summary>
        /// Page size without consideration of orientation.
        /// </summary>
        public Size Size { get; internal set; }

        /// <summary>
        /// Pages that will be printed. The array include each page number.
        /// </summary>
        public int[] Pages { get; internal set; }

        /// <summary>
        /// Page scale in a percentage value. <see cref="Double.NaN"/> means automatic scaling to fit.
        /// </summary>
        public double Scale { get; internal set; }

        /// <summary>
        /// Page margin.
        /// </summary>
        public double Margin { get; internal set; }

        /// <summary>
        /// Pages per sheet.
        /// </summary>
        public PrintSettings.PagesPerSheet PagesPerSheet { get; internal set; }

        /// <summary>
        /// Color.
        /// </summary>
        public PrintSettings.PageColor Color { get; internal set; }

        /// <summary>
        /// Page order.
        /// </summary>
        public PrintSettings.PageOrder PageOrder { get; internal set; }

        /// <summary>
        /// Page orientation.
        /// </summary>
        public PrintSettings.PageOrientation Orientation { get; internal set; }
    }

    public class PrintDialog
    {
        /// <summary>
        /// <see cref="PrintDialog"/>'s owner window.
        /// </summary>
        public Window Owner { get; set; } = null;

        /// <summary>
        /// <see cref="PrintDialog"/>'s title.
        /// </summary>
        public string Title { get; set; } = "Print";

        /// <summary>
        /// <see cref="PrintDialog"/>'s icon.
        /// </summary>
        public ImageSource Icon { get; set; } = null;

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
        public PrintDocument Document { get; set; } = null;


        /// <summary>
        /// <see cref="PrintDialog"/>'s resize mode.
        /// </summary>
        public ResizeMode ResizeMode { get; set; } = ResizeMode.NoResize;

        /// <summary>
        /// The default print settings.
        /// </summary>
        public PrintDialogSettings DefaultSettings { get; set; } = new PrintDialogSettings();

        /// <summary>
        /// <see cref="PrintDialog"/>'s startup location.
        /// </summary>
        public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.CenterScreen;

        /// <summary>
        /// The function that will be used to reload the document based on specific print settings The function needs return a list of <see cref="PageContent"/> that represents the page contents in order.
        /// </summary>
        public Func<DocumentInfo, ICollection<PrintPage>> ReloadDocumentCallback { get; set; } = null;

        /// <summary>
        /// The total number of papers that the printer will use.
        /// </summary>
        public int TotalPapers { get; private set; } = 0;

        private Internal.PrintWindow PrintWindow = null;

        /// <summary>
        /// Show <see cref="PrintDialog"/>.
        /// </summary>
        /// <returns>A boolean value, where true means that the Print button is clicked, false means that the Cancel button is clicked, and <see cref="null"/> means <see cref="PrintDialog"/> can't be opened or there is already a running <see cref="PrintDialog"/>.</returns>
        public bool? ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        /// Show <see cref="PrintDialog"/> with synchronized document generation.
        /// </summary>
        /// <param name="documentGeneration">The function to load the document. <see cref="null"/> if the document is already generated.</param>
        /// <returns>A boolean value, where true means that the Print button is clicked, false means that the Cancel button is clicked, and <see cref="null"/> means <see cref="PrintDialog"/> can't be opened or there is already a running <see cref="PrintDialog"/>.</returns>
        public bool? ShowDialog(Action documentGeneration)
        {
            if (PrintWindow == null)
            {
                if (documentGeneration == null && Document == null)
                {
                    throw new ArgumentNullException("Document is null.");
                }
                else if (Document != null && Document.DocumentMargin < 0)
                {
                    throw new ArgumentException("DocumentMargin has to be greater than zero.");
                }

                try
                {
                    PrintWindow = new Internal.PrintWindow(this, documentGeneration)
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
                    throw ex;
                }

                TotalPapers = PrintWindow.TotalPapers;
                bool returnValue = PrintWindow.ReturnValue;

                PrintWindow = null;

                return returnValue;
            }
            else
            {
                return null;
            }
        }
    }
}