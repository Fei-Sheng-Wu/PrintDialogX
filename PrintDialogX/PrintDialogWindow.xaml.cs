using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Threading;

namespace PrintDialogX
{
    internal partial class PrintDialogWindow : Wpf.Ui.Controls.FluentWindow, IPrintDialogHost
    {
        private KeyEventHandler? handler = null;
        private PrintDialogResult result = new();

        public PrintDialogWindow()
        {
            InitializeComponent();

            Resources.MergedDictionaries.Add(PrintDialogViewModel.StringResources);
        }

        private void HandleShortcuts(object sender, KeyEventArgs e)
        {
            handler?.Invoke(sender, e);
        }

        public void Start(PrintDialog dialog, bool isDialog, Func<Task<FrameworkElement>> callback)
        {
            handler = null;
            result = new();

            title.Title = dialog.InterfaceSettings.Title;
            title.Icon = dialog.InterfaceSettings.Icon;
            content.Child = new Wpf.Ui.Controls.ProgressRing()
            {
                IsIndeterminate = true
            };

            Dispatcher.InvokeAsync(async () => content.Child = await callback(), DispatcherPriority.Loaded);
            if (isDialog)
            {
                ShowDialog();
            }
            else
            {
                Show();
            }
        }

        public PrintDialogResult GetResult()
        {
            return result;
        }

        public void SetResult(PrintDialogResult value)
        {
            result = value;
            Close();
        }

        public void SetProgress(IPrintDialogHost.PrintDialogProgress progress)
        {
            TaskbarItemInfo.ProgressState = progress.State switch
            {
                IPrintDialogHost.PrintDialogProgressState.Indeterminate => TaskbarItemProgressState.Indeterminate,
                IPrintDialogHost.PrintDialogProgressState.Normal => TaskbarItemProgressState.Normal,
                IPrintDialogHost.PrintDialogProgressState.Error => TaskbarItemProgressState.Error,
                _ => TaskbarItemProgressState.None
            };
            TaskbarItemInfo.ProgressValue = progress.Value / 100;
        }

        public void SetShortcutHandler(KeyEventHandler value)
        {
            handler = value;
        }
    }
}
