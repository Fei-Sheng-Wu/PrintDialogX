using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PrintDialogX
{
    public struct PrintDialogResult
    {
        public bool IsSuccess { get; set; }
        public int PaperCount { get; set; }
    }

    public interface IPrintDialogHost
    {
        public enum PrintDialogProgressState
        {
            None,
            Indeterminate,
            Normal,
            Error
        }

        public struct PrintDialogProgress
        {
            public PrintDialogProgressState State { get; set; }
            public double Value { get; set; }
        }

        public void Start(PrintDialog dialog, bool isDialog, Func<Task<FrameworkElement>> callback);
        public PrintDialogResult GetResult();
        public void SetResult(PrintDialogResult result);
        public void SetProgress(PrintDialogProgress progress);
        public void SetShortcutHandler(KeyEventHandler handler);
    }
}
