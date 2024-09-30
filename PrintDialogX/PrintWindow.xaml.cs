using System;
using System.Windows;

namespace PrintDialogX.Internal
{
    partial class PrintWindow : Window
    {
        public bool ReturnValue { get; internal set; } = false;
        public int TotalPapers { get; internal set; } = 0;

        private readonly PrintDialog.PrintDialog _dialog;
        private readonly Action _documentGenerationCallback;

        internal PrintWindow(PrintDialog.PrintDialog dialog, Action generation)
        {
            InitializeComponent();

            _dialog = dialog;
            _documentGenerationCallback = generation;
            if (_documentGenerationCallback == null)
            {
                container.Content = null;
                NavigateToPrintPage();
            }

            this.Width = SystemParameters.PrimaryScreenWidth * 0.65;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.7;
            this.UpdateLayout();
            Common.DoEvents();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_documentGenerationCallback != null)
            {
                this.UpdateLayout();
                Common.DoEvents();

                _documentGenerationCallback();
                NavigateToPrintPage();
            }
        }

        internal void NavigateToPrintPage()
        {
            if (_dialog.Document == null)
            {
                throw new ArgumentNullException("Document is null.");
            }
            else if (_dialog.Document.DocumentMargin < 0)
            {
                throw new ArgumentException("DocumentMargin has to be greater than zero.");
            }

            container.Navigate(new PrintPage(this, _dialog));
            if (container.CanGoBack)
            {
                container.RemoveBackEntry();
            }
        }
    }
}
