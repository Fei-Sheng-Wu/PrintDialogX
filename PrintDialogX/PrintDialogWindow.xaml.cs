using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace PrintDialogX
{
    internal partial class PrintDialogWindow : Wpf.Ui.Controls.FluentWindow, IPrintDialogHost
    {
        private Func<Task<FrameworkElement>>? loader = null;
        private KeyEventHandler? handler = null;
        private PrintDialogResult result = new();

        public PrintDialogWindow()
        {
            InitializeComponent();

            Resources.MergedDictionaries.Add(InterfaceSettings.StringResources);
        }

        private async void AttachControl(object sender, EventArgs e)
        {
            if (loader == null)
            {
                return;
            }

            content.Child = await loader();
        }

        private void HandleShortcuts(object sender, KeyEventArgs e)
        {
            handler?.Invoke(sender, e);
        }

        public void Start(PrintDialog dialog, bool isDialog, Func<Task<FrameworkElement>> callback)
        {
            loader = callback;

            title.Title = dialog.InterfaceSettings.Title;
            title.Icon = dialog.InterfaceSettings.Icon;

            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(this);
            Wpf.Ui.Appearance.ApplicationThemeManager.Changed += (x, e) => Wpf.Ui.Appearance.ApplicationThemeManager.Apply(this);
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);

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
