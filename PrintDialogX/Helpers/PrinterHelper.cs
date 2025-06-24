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
                return printer.QueueStatus switch
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
            return (printerType + (isSmallSized ? "_Small" : string.Empty)) switch
            {
                "Normal" => new System.Windows.Media.Imaging.BitmapImage(NormalPrinterIcon),
                "Normal_Small" => new System.Windows.Media.Imaging.BitmapImage(NormalPrinterSmallIcon),
                "Normal_Default" => new System.Windows.Media.Imaging.BitmapImage(NormalDefaultPrinterIcon),
                "Normal_Default_Small" => new System.Windows.Media.Imaging.BitmapImage(NormalDefaultPrinterSmallIcon),
                "Fax" => new System.Windows.Media.Imaging.BitmapImage(FaxPrinterIcon),
                "Fax_Small" => new System.Windows.Media.Imaging.BitmapImage(FaxPrinterSmallIcon),
                "Fax_Default" => new System.Windows.Media.Imaging.BitmapImage(FaxDefaultPrinterIcon),
                "Fax_Default_Small" => new System.Windows.Media.Imaging.BitmapImage(FaxDefaultPrinterSmallIcon),
                "NetworkFax" => new System.Windows.Media.Imaging.BitmapImage(NetworkFaxPrinterIcon),
                "NetworkFax_Small" => new System.Windows.Media.Imaging.BitmapImage(NetworkFaxPrinterSmallIcon),
                "NetworkFax_Default" => new System.Windows.Media.Imaging.BitmapImage(NetworkFaxDefaultPrinterIcon),
                "NetworkFax_Default_Small" => new System.Windows.Media.Imaging.BitmapImage(NetworkFaxDefaultPrinterSmallIcon),
                _ => new System.Windows.Media.Imaging.BitmapImage(NormalPrinterIcon),
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