using System;
using System.Threading.Tasks;
using System.Windows;

namespace PrintDialogX
{
    partial class PrintDialogWindow : Wpf.Ui.Controls.FluentWindow
    {
        public static readonly ResourceDictionary StringResources = new()
        {
            Source = new("/PrintDialogX;component/Resources/Languages/en-US.xaml", UriKind.Relative)
        };

        public (bool IsSuccess, int TotalPaper)? Result { get; set; } = null;

        private PrintDialog? dialog;
        private Func<Task>? generation;

        public void Create(PrintDialog host, Func<Task>? callback)
        {
            InitializeComponent();

            dialog = host;
            generation = callback;
            Resources.MergedDictionaries.Add(StringResources);

            title.Title = dialog.Interface.Title;
            title.Icon = dialog.Interface.Icon;
        }

        private async void Start(object sender, RoutedEventArgs e)
        {
            if (dialog == null)
            {
                return;
            }

            if (generation != null)
            {
                await generation.Invoke();
            }
            content.Child = new PrintDialogPage(this, dialog);
        }
    }
}
