using System;
using System.Linq;
using System.Printing;

namespace PrintDialogX.Internal
{
    internal class PrinterHelper
    {
        public static readonly Uri AddPrinterIcon = new Uri("/PrintDialogX;component/Resources/AddPrinter.png", UriKind.Relative);
        public static readonly Uri NormalPrinterIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Normal.png", UriKind.Relative);
        public static readonly Uri NormalDefaultPrinterIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Normal (Default).png", UriKind.Relative);
        public static readonly Uri FaxPrinterIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Fax.png", UriKind.Relative);
        public static readonly Uri FaxDefaultPrinterIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Fax (Default).png", UriKind.Relative);
        public static readonly Uri NetworkFaxPrinterIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Network Fax.png", UriKind.Relative);
        public static readonly Uri NetworkFaxDefaultPrinterIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Network Fax (Default).png", UriKind.Relative);
        public static readonly Uri NormalPrinterSmallIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Normal - Small.png", UriKind.Relative);
        public static readonly Uri NormalDefaultPrinterSmallIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Normal (Default) - Small.png", UriKind.Relative);
        public static readonly Uri FaxPrinterSmallIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Fax - Small.png", UriKind.Relative);
        public static readonly Uri FaxDefaultPrinterSmallIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Fax (Default) - Small.png", UriKind.Relative);
        public static readonly Uri NetworkFaxPrinterSmallIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Network Fax - Small.png", UriKind.Relative);
        public static readonly Uri NetworkFaxDefaultPrinterSmallIcon = new Uri("/PrintDialogX;component/Resources/PrinterIcons/Network Fax (Default) - Small.png", UriKind.Relative);

        public static PrintQueue GetDefaultPrinter()
        {
            try
            {
                return LocalPrintServer.GetDefaultPrintQueue();
            }
            catch
            {
                return null;
            }
        }

        public static bool RefreshPrinter(PrintQueue printer)
        {
            try
            {
                printer.Refresh();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetPrinterStatus(PrintQueue printer)
        {
            RefreshPrinter(printer);
            try
            {
                switch (printer.QueueStatus)
                {
                    case PrintQueueStatus.Busy: return "Busy";
                    case PrintQueueStatus.DoorOpen: return "Door Open";
                    case PrintQueueStatus.Error: return "Error";
                    case PrintQueueStatus.Initializing: return "Initializing";
                    case PrintQueueStatus.IOActive: return "Exchanging Data";
                    case PrintQueueStatus.ManualFeed: return "Need Manual Feed";
                    case PrintQueueStatus.NoToner: return "No Toner";
                    case PrintQueueStatus.NotAvailable: return "Not Available";
                    case PrintQueueStatus.Offline: return "Offline";
                    case PrintQueueStatus.OutOfMemory: return "Out Of Memory";
                    case PrintQueueStatus.OutputBinFull: return "Output Bin Full";
                    case PrintQueueStatus.PagePunt: return "Page Punt";
                    case PrintQueueStatus.PaperJam: return "Paper Jam";
                    case PrintQueueStatus.PaperOut: return "Paper Out";
                    case PrintQueueStatus.PaperProblem: return "Paper Error";
                    case PrintQueueStatus.Paused: return "Paused";
                    case PrintQueueStatus.PendingDeletion: return "Deleting Job";
                    case PrintQueueStatus.PowerSave: return "Power Save";
                    case PrintQueueStatus.Printing: return "Printing";
                    case PrintQueueStatus.Processing: return "Processing";
                    case PrintQueueStatus.ServerUnknown: return "Server Unknown";
                    case PrintQueueStatus.TonerLow: return "Toner Low";
                    case PrintQueueStatus.UserIntervention: return "Need User Intervention";
                    case PrintQueueStatus.Waiting: return "Waiting";
                    case PrintQueueStatus.WarmingUp: return "Warming Up";
                    default: return "Ready";
                };
            }
            catch
            {
                return "Error";
            }
        }

        public static System.Windows.Media.ImageSource GetPrinterIcon(PrintQueue printer, PrintQueueCollection fax, bool isSmallSized = false)
        {
            string printerType = fax.Any(x => x.FullName == printer.FullName) ? (printer.IsShared ? "NetworkFax" : "Fax") : "Normal";

            PrintQueue printerDefault = GetDefaultPrinter();
            printerType += printerDefault != null && printerDefault.FullName == printer.FullName ? "_Default" : string.Empty;
            switch (printerType + (isSmallSized ? "_Small" : string.Empty))
            {
                case "Normal": return new System.Windows.Media.Imaging.BitmapImage(NormalPrinterIcon);
                case "Normal_Small": return new System.Windows.Media.Imaging.BitmapImage(NormalPrinterSmallIcon);
                case "Normal_Default": return new System.Windows.Media.Imaging.BitmapImage(NormalDefaultPrinterIcon);
                case "Normal_Default_Small": return new System.Windows.Media.Imaging.BitmapImage(NormalDefaultPrinterSmallIcon);
                case "Fax": return new System.Windows.Media.Imaging.BitmapImage(FaxPrinterIcon);
                case "Fax_Small": return new System.Windows.Media.Imaging.BitmapImage(FaxPrinterSmallIcon);
                case "Fax_Default": return new System.Windows.Media.Imaging.BitmapImage(FaxDefaultPrinterIcon);
                case "Fax_Default_Small": return new System.Windows.Media.Imaging.BitmapImage(FaxDefaultPrinterSmallIcon);
                case "NetworkFax": return new System.Windows.Media.Imaging.BitmapImage(NetworkFaxPrinterIcon);
                case "NetworkFax_Small": return new System.Windows.Media.Imaging.BitmapImage(NetworkFaxPrinterSmallIcon);
                case "NetworkFax_Default": return new System.Windows.Media.Imaging.BitmapImage(NetworkFaxDefaultPrinterIcon);
                case "NetworkFax_Default_Small": return new System.Windows.Media.Imaging.BitmapImage(NetworkFaxDefaultPrinterSmallIcon);
                default: return new System.Windows.Media.Imaging.BitmapImage(NormalPrinterIcon);
            };
        }

        public static double GetPrinterMinimumMargin(PrintQueue printer, PageMediaSize size)
        {
            try
            {
                PageImageableArea imageableArea = printer.GetPrintCapabilities(new PrintTicket() { PageMediaSize = size }).PageImageableArea;
                return Math.Max(imageableArea.OriginHeight, imageableArea.OriginWidth);
            }
            catch
            {
                return 0;
            }
        }
    }
}