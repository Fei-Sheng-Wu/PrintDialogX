using System;
using System.Printing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
