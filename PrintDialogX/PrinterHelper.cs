using System;
using System.Printing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrintDialogX.PrinterHelper
{
    public class PrinterHelper
    {
        protected PrinterHelper()
        {
            return;
        }

        /// <summary>
        /// Get the printer by its name.
        /// </summary>
        /// <param name="printerName">The printer's name.</param>
        public static PrintQueue GetPrinterByName(string printerName)
        {
            return new PrintServer().GetPrintQueue(printerName);
        }

        /// <summary>
        /// Get the printer by its name.
        /// </summary>
        /// <param name="printerName">The printer's name.</param>
        /// <param name="printServer">The print server that should be used to get the printer.</param>
        public static PrintQueue GetPrinterByName(string printerName, PrintServer printServer)
        {
            return printServer.GetPrintQueue(printerName);
        }

        /// <summary>
        /// Get the local default printer.
        /// </summary>
        public static PrintQueue GetDefaultPrinter()
        {
            return LocalPrintServer.GetDefaultPrintQueue();
        }

        /// <summary>
        /// Get local printers.
        /// </summary>
        public static PrintQueueCollection GetLocalPrinters()
        {
            return GetLocalPrinters(new PrintServer());
        }

        /// <summary>
        /// Get local printers.
        /// </summary>
        /// <param name="enumerationFlag">An array of values that represent the types of print queues that are in the collection.</param>
        public static PrintQueueCollection GetLocalPrinters(EnumeratedPrintQueueTypes[] enumerationFlag)
        {
            return GetLocalPrinters(new PrintServer(), enumerationFlag);
        }

        /// <summary>
        /// Get local printers.
        /// </summary>
        /// <param name="server">The print server.</param>
        public static PrintQueueCollection GetLocalPrinters(PrintServer server)
        {
            return server.GetPrintQueues();
        }

        /// <summary>
        /// Get local printers.
        /// </summary>
        /// <param name="server">The print server.</param>
        /// <param name="enumerationFlag">An array of values that represent the types of print queues that are in the collection.</param>
        public static PrintQueueCollection GetLocalPrinters(PrintServer server, EnumeratedPrintQueueTypes[] enumerationFlag)
        {
            return server.GetPrintQueues(enumerationFlag);
        }

        /// <summary>
        /// Get the printer's status info.
        /// </summary>
        /// <param name="printerName">The printer's name.</param>
        public static string GetPrinterStatusInfo(string printerName)
        {
            return GetPrinterStatusInfo(GetPrinterByName(printerName));
        }

        /// <summary>
        /// Get the printer's status info.
        /// </summary>
        /// <param name="printer">The printer.</param>
        public static string GetPrinterStatusInfo(PrintQueue printer)
        {
            printer.Refresh();
            return GetPrinterStatusInfo(printer.QueueStatus);
        }

        /// <summary>
        /// Get the printer's status info.
        /// </summary>
        /// <param name="printerStatue">The printer's status.</param>
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
        /// Install a new printer to the print server.
        /// </summary>
        /// <param name="printerName">The new printer's name.</param>
        /// <param name="driverName">The new printer's driver name.</param>
        /// <param name="portNames">IDs of the ports that the new queue uses.</param>
        /// <param name="printerProcessorName">The print processor name.</param>
        /// <param name="printerProperties">The new printer's properties.</param>
        public static void InstallPrinter(string printerName, string driverName, string[] portNames, string printerProcessorName, PrintQueueAttributes printerProperties)
        {
            LocalPrintServer localPrintServer = new LocalPrintServer();
            localPrintServer.InstallPrintQueue(printerName, driverName, portNames, printerProcessorName, printerProperties);
            localPrintServer.Commit();
        }

        /// <summary>
        /// Install a new printer to the print server.
        /// </summary>
        /// <param name="printServer">The print server.</param>
        /// <param name="printerName">The new printer's name.</param>
        /// <param name="driverName">The new printer's driver name.</param>
        /// <param name="portNames">IDs of the ports that the new queue uses.</param>
        /// <param name="printerProcessorName">The print processor name.</param>
        /// <param name="printerProperties">The new printer's properties.</param>
        public static void InstallPrinter(PrintServer printServer, string printerName, string driverName, string[] portNames, string printerProcessorName, PrintQueueAttributes printerProperties)
        {
            printServer.InstallPrintQueue(printerName, driverName, portNames, printerProcessorName, printerProperties);
            printServer.Commit();
        }

        /// <summary>
        /// Install a new printer to the print server.
        /// </summary>
        /// <param name="printServer">The print server.</param>
        /// <param name="printerName">The new printer's name.</param>
        /// <param name="driverName">The new printer driver's name.</param>
        /// <param name="portNames">IDs of the ports that the new queue uses.</param>
        /// <param name="printerProcessorName">The print processor name.</param>
        /// <param name="printerProperties">The new printer's properties.</param>
        /// <param name="printerShareName">The new printer's share name.</param>
        /// <param name="printerComment">The new printer's comment.</param>
        /// <param name="printerLoction">The new printer's loction.</param>
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
        protected PrintJobHelper()
        {
            return;
        }

        /// <summary>
        /// Get all print jobs of the printer.
        /// </summary>
        /// <param name="printerName">The printer's name.</param>
        public static PrintJobInfoCollection GetPrintJobs(string printerName)
        {
            PrintQueue printer = new PrintServer().GetPrintQueue(printerName);
            printer.Refresh();

            return printer.GetPrintJobInfoCollection();
        }

        /// <summary>
        /// Get all print jobs of the printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        public static PrintJobInfoCollection GetPrintJobs(PrintQueue printer)
        {
            printer.Refresh();

            return printer.GetPrintJobInfoCollection();
        }

        /// <summary>
        /// Get all print jobs of the printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <param name="submitter">The print job submitter.</param>
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
        /// Get all error print jobs of the printer.
        /// </summary>
        /// <param name="printerName">The printer's name.</param>
        public static PrintJobInfoCollection GetErrorPrintJobs(string printerName)
        {
            return GetErrorPrintJobs(new PrintServer().GetPrintQueue(printerName), false);
        }

        /// <summary>
        /// Get all error print jobs of the printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        public static PrintJobInfoCollection GetErrorPrintJobs(PrintQueue printer)
        {
            return GetErrorPrintJobs(printer, false);
        }

        /// <summary>
        /// Get all error print jobs of the printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <param name="cancelJob">Cancel error print jobs or not.</param>
        public static PrintJobInfoCollection GetErrorPrintJobs(PrintQueue printer, bool cancelJob)
        {
            printer.Refresh();

            PrintJobInfoCollection errorList = new PrintJobInfoCollection(printer, null);

            foreach (PrintSystemJobInfo jobInfo in printer.GetPrintJobInfoCollection())
            {
                if (jobInfo.JobStatus == PrintJobStatus.Blocked || jobInfo.JobStatus == PrintJobStatus.Error || jobInfo.JobStatus == PrintJobStatus.Offline || jobInfo.JobStatus == PrintJobStatus.PaperOut || jobInfo.JobStatus == PrintJobStatus.UserIntervention)
                {
                    errorList.Add(jobInfo);

                    if (cancelJob)
                    {
                        jobInfo.Cancel();
                        jobInfo.Commit();
                    }
                }
            }

            return errorList;
        }

        /// <summary>
        /// Get all error print jobs of the printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <param name="submitter">The print job submitter.</param>
        public static PrintJobInfoCollection GetErrorPrintJobs(PrintQueue printer, string submitter)
        {
            return GetErrorPrintJobs(printer, submitter, false);
        }

        /// <summary>
        /// Get all error print jobs of the printer.
        /// </summary>
        /// <param name="printer">The printer.</param>
        /// <param name="submitter">The print job submitter.</param>
        /// <param name="cancelJob">Cancel error print jobs or not.</param>
        public static PrintJobInfoCollection GetErrorPrintJobs(PrintQueue printer, string submitter, bool cancelJob)
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

                        if (cancelJob)
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

    internal class PrinterIconHelper
    {
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