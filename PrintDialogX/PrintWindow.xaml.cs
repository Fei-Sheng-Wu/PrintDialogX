using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;

namespace PrintDialogX.Internal
{
    partial class PrintWindow : Window
    {
        public bool ReturnValue { get; internal set; }
        public int TotalPapers { get; internal set; }

        private readonly PrintDialog.PrintDialog _printDialog;
        private readonly Action _documentGeneration;

        internal PrintWindow(PrintDialog.PrintDialog printDialog, Action documentGeneration)
        {
            InitializeComponent();

            _printDialog = printDialog;
            _documentGeneration = documentGeneration;

            if (_documentGeneration == null)
            {
                mainFrame.Content = null;
                NavigatePrintPage();
            }

            this.Width = SystemParameters.PrimaryScreenWidth * 0.65;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.7;

            this.UpdateLayout();
            Common.DoEvents();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_documentGeneration != null)
            {
                this.UpdateLayout();
                Common.DoEvents();

                _documentGeneration();

                NavigatePrintPage();
            }
        }

        internal void NavigatePrintPage()
        {
            if (_printDialog.Document == null)
            {
                throw new ArgumentNullException("Document is null.");
            }
            else if (_printDialog.Document.DocumentMargin < 0)
            {
                throw new ArgumentException("DocumentMargin has to be greater than zero.");
            }

            mainFrame.Navigate(new PrintPage(this, _printDialog));

            if (mainFrame.CanGoBack)
            {
                mainFrame.RemoveBackEntry();
            }
        }
    }
}
