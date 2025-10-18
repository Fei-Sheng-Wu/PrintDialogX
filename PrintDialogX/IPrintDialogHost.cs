using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PrintDialogX
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrintDialogResult"/> struct.
    /// </summary>
    public struct PrintDialogResult()
    {
        /// <summary>
        /// Gets or sets whether the document was successfully printed or the operation was cancelled.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the number of papers calculated to be used.
        /// </summary>
        public int PaperCount { get; set; }
    }

    /// <summary>
    /// Represents a host that can contain the actual control for the print operation.
    /// </summary>
    public interface IPrintDialogHost
    {
        /// <summary>
        /// Specifies the state of the print job progress.
        /// </summary>
        public enum PrintDialogProgressState
        {
            /// <summary>
            /// There is no available print job.
            /// </summary>
            None,

            /// <summary>
            /// The print job is initializing.
            /// </summary>
            Indeterminate,

            /// <summary>
            /// The print job is in progress.
            /// </summary>
            Normal,

            /// <summary>
            /// The print job failed or is cancelled.
            /// </summary>
            Error
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDialogProgress"/> struct.
        /// </summary>
        public struct PrintDialogProgress()
        {
            /// <summary>
            /// Gets or sets the state of the print job progress.
            /// </summary>
            public PrintDialogProgressState State { get; set; }

            /// <summary>
            /// Gets or sets the percentage value of the print job progress, from 0 to 100.
            /// </summary>
            public double Value { get; set; }
        }

        /// <summary>
        /// Starts the host with the specified settings and a callback to attach the actual control for the print operation.
        /// </summary>
        /// <param name="dialog">The <see cref="PrintDialog"/> instance with the specified settings.</param>
        /// <param name="isDialog"><see langword="true"/> to start the host in a dialog style and return only when the operation is finished; otherwise, <see langword="false"/>.</param>
        /// <param name="callback">The callback function to be invoked to attain the actual control for the print operation to be attached to the host.</param>
        public void Start(PrintDialog dialog, bool isDialog, Func<Task<FrameworkElement>> callback);

        /// <summary>
        /// Gets the result of the print operation.
        /// </summary>
        /// <returns></returns>
        public PrintDialogResult GetResult();

        /// <summary>
        /// Sets the result of the print operation.
        /// </summary>
        /// <param name="result">The result to be set to.</param>
        public void SetResult(PrintDialogResult result);

        /// <summary>
        /// Sets the progress of the current print job.
        /// </summary>
        /// <param name="progress">The progress to be set to.</param>
        public void SetProgress(PrintDialogProgress progress);

        /// <summary>
        /// Sets the event handler that handles keyboard shortcuts.
        /// </summary>
        /// <param name="handler">The event handler to be used.</param>
        public void SetShortcutHandler(KeyEventHandler handler);
    }
}
