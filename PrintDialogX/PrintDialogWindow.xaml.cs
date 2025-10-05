using System;
using System.Threading.Tasks;
using System.Windows;

namespace PrintDialogX
{
    partial class PrintDialogWindow : Wpf.Ui.Controls.FluentWindow
    {
        public bool ReturnValue { get; set; } = false;
        public int TotalPapers { get; set; } = 0;

        private PrintDialog? dialog;
        private Func<Task>? generation;

        public void Create(PrintDialog host, Func<Task>? callback)
        {
            InitializeComponent();

            dialog = host;
            generation = callback;
            Resources.MergedDictionaries.Add(PrintPageViewModel.StringResources);
        }

        private async void Start(object sender, RoutedEventArgs e)
        {
            if (dialog == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(Title))
            {
                title.Title = Title;
            }
            if (Icon != null)
            {
                title.Icon = new Wpf.Ui.Controls.ImageIcon()
                {
                    Source = Icon
                };
            }

            if (generation != null)
            {
                await generation.Invoke();
            }
            content.Child = new PrintDialogPage(this, dialog);
        }
    }
}
