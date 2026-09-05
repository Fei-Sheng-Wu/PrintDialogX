using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace PrintDialogX
{
    internal partial class PrintDialogWindow : Wpf.Ui.Controls.FluentWindow, IPrintDialogHost
    {
        private bool isAvailable = true;
        private PrintDialogResult result = new();
        private Func<Task<FrameworkElement>>? executor = null;

        public PrintDialogWindow() : base()
        {
            InitializeComponent();
        }

        private void Exit(object sender, CancelEventArgs e)
        {
            Wpf.Ui.Appearance.ApplicationThemeManager.Changed -= UpdateTheme;
            Wpf.Ui.Appearance.SystemThemeWatcher.UnWatch(this);
        }

        private async void LoadContent(object sender, EventArgs e)
        {
            content.Content = executor is Func<Task<FrameworkElement>> instantiator ? await instantiator() : null;
        }

        private void UpdateTheme(Wpf.Ui.Appearance.ApplicationTheme theme, Color accent)
        {
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(this);

            if (content.Content is FrameworkElement element)
            {
                Wpf.Ui.Appearance.ApplicationThemeManager.Apply(element);
            }
        }

        public void Start(PrintDialog dialog, bool isDialog, Func<Task<FrameworkElement>> instantiator)
        {
            if (!isAvailable)
            {
                throw new InvalidOperationException("The print dialog has already been used.");
            }

            isAvailable = false;
            executor = instantiator;

            Application application = Application.Current ?? new();
            Wpf.Ui.Appearance.ApplicationAccentColorManager.ApplySystemAccent();
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(this);
            Wpf.Ui.Appearance.ApplicationThemeManager.Changed += UpdateTheme;
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            (Language, FlowDirection, ResourceDictionary resources) = Internal.LanguageAttribute.Parse(dialog.InterfaceSettings.DisplayLanguage);
            Resources.MergedDictionaries.Add(resources);

            if (dialog.InterfaceSettings.Title is string text)
            {
                Title = text;
                header.Text = text;
            }
            if (dialog.InterfaceSettings.Icon is Wpf.Ui.Controls.IconElement icon)
            {
                title.Icon = icon;
                header.Margin = new(0, 10, 0, 10);
            }

            Internal.Common.Execute(isDialog ? () => ShowDialog() : Show);
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
            TaskbarItemInfo.ProgressValue = progress.Value / 100.0;
        }

        public void AddShortcutHandlers(IEnumerable<KeyBinding> handlers)
        {
            foreach (KeyBinding handler in handlers)
            {
                InputBindings.Add(handler);
            }
        }
    }
}
