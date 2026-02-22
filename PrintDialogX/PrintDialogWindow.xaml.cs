using System;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Controls;

namespace PrintDialogX
{
    internal partial class PrintDialogWindow : Wpf.Ui.Controls.FluentWindow, IPrintDialogHost
    {
        private bool isAvailable = true;
        private Func<Task<FrameworkElement>>? loader = null;
        private KeyEventHandler? handler = null;
        private PrintDialogResult result = new();

        public PrintDialogWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Wpf.Ui.Appearance.ApplicationThemeManager.Changed -= UpdateTheme;
            Wpf.Ui.Appearance.SystemThemeWatcher.UnWatch(this);

            base.OnClosing(e);
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
            if (!isAvailable)
            {
                throw new InvalidOperationException("The print dialog has already been used.");
            }

            isAvailable = false;
            loader = callback;

            InterfaceToContentConverter.ApplyLanguage(Resources, dialog.InterfaceSettings.DisplayLanguage);
            if (dialog.InterfaceSettings.Title != null)
            {
                Title = dialog.InterfaceSettings.Title;
            }
            title.Header = new TextBlock()
            {
                Margin = new(dialog.InterfaceSettings.Icon == null ? 16 : 0, 10, 0, 10),
                FontSize = title.FontSize,
                Text = Title
            };
            title.Icon = dialog.InterfaceSettings.Icon;

            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(this);
            Wpf.Ui.Appearance.ApplicationThemeManager.Changed += UpdateTheme;
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

        public void UpdateTheme(Wpf.Ui.Appearance.ApplicationTheme theme, Color accent)
        {
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(this);

            if (content.Child is FrameworkElement element)
            {
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(element);
            }
        }
    }
}
