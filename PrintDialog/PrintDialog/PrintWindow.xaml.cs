using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace PrintDialogX.PrintControl
{
    partial class PrintWindow : Window
    {
        public bool ReturnValue
        {
            get
            {
                return printPage.ReturnValue;
            }
        }

        public int? TotalSheets
        {
            get
            {
                return printPage.TotalSheets;
            }
        }

        private PrintPage printPage;

        private readonly FixedDocument _document;
        private readonly string _documentName;
        private readonly double _pageMargin;
        private readonly PrintDialog.PrintDialogSettings _defaultSettings;
        private readonly Action _loadingAction;
        private readonly bool _loading;
        private readonly bool _allowPagesOption;
        private readonly bool _allowScaleOption;
        private readonly bool _allowTwoSidedOption;
        private readonly bool _allowPageOrderOption;
        private readonly bool _allowPagesPerSheetOption;
        private readonly bool _allowMoreSettingsExpander;
        private readonly bool _allowAddNewPrinerComboBoxItem;
        private readonly bool _allowPrinterPreferencesButton;
        private readonly Func<PrintDialog.DocumentInfo, List<PageContent>> _getDocumentWhenReloadDocumentMethod;

        public PrintWindow(FixedDocument document, string documentName, double pageMargin, PrintDialog.PrintDialogSettings defaultSettings, bool allowPagesOption, bool allowScaleOption, bool allowTwoSidedOption, bool allowPagesPerSheetOption, bool allowPageOrderOption, bool allowMoreSettingsExpander, bool allowAddNewPrinerComboBoxItem, bool allowPrinterPreferencesButton, Func<PrintDialog.DocumentInfo, List<PageContent>> getDocumentWhenReloadDocumentMethod, bool loading, Action loadingAction)
        {
            InitializeComponent();

            _document = document;
            _documentName = documentName;
            _pageMargin = pageMargin;
            _loading = loading;
            _loadingAction = loadingAction;
            _defaultSettings = defaultSettings;
            _allowPagesOption = allowPagesOption;
            _allowScaleOption = allowScaleOption;
            _allowTwoSidedOption = allowTwoSidedOption;
            _allowPageOrderOption = allowPageOrderOption;
            _allowPagesPerSheetOption = allowPagesPerSheetOption;
            _allowMoreSettingsExpander = allowMoreSettingsExpander;
            _allowAddNewPrinerComboBoxItem = allowAddNewPrinerComboBoxItem;
            _allowPrinterPreferencesButton = allowPrinterPreferencesButton;
            _getDocumentWhenReloadDocumentMethod = getDocumentWhenReloadDocumentMethod;

            if (_loading == false)
            {
                mainFrame.Content = null;
                BeginSettingAndPreviewing();
            }

            this.Width = SystemParameters.PrimaryScreenWidth * 0.65;
            this.Height = SystemParameters.PrimaryScreenHeight * 0.7;

            this.UpdateLayout();
            PrintPage.DoEvents();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_loading == true)
            {
                this.UpdateLayout();
                PrintPage.DoEvents();

                _loadingAction();
            }
        }

        internal void BeginSettingAndPreviewing()
        {
            printPage = new PrintPage(_document, _documentName, _pageMargin, _defaultSettings, _allowPagesOption, _allowScaleOption, _allowTwoSidedOption, _allowPagesPerSheetOption, _allowPageOrderOption, _allowMoreSettingsExpander, _allowAddNewPrinerComboBoxItem, _allowPrinterPreferencesButton, _getDocumentWhenReloadDocumentMethod);

            mainFrame.Navigate(printPage);

            if (mainFrame.CanGoBack)
            {
                mainFrame.RemoveBackEntry();
            }
        }

        internal void BeginSettingAndPreviewing(FixedDocument document, string documentName, double pageMargin, PrintDialog.PrintDialogSettings defaultSettings, bool allowPagesOption, bool allowScaleOption, bool allowTwoSidedOption, bool allowPagesPerSheetOption, bool allowPageOrderOption, bool allowMoreSettingsExpander, bool allowAddNewPrinerComboBoxItem, bool allowPrinterPreferencesButton, Func<PrintDialog.DocumentInfo, List<PageContent>> getDocumentWhenReloadDocumentMethod)
        {
            printPage = new PrintPage(document, documentName, pageMargin, defaultSettings, allowPagesOption, allowScaleOption, allowTwoSidedOption, allowPagesPerSheetOption, allowPageOrderOption, allowMoreSettingsExpander, allowAddNewPrinerComboBoxItem, allowPrinterPreferencesButton, getDocumentWhenReloadDocumentMethod);

            mainFrame.Navigate(printPage);

            if (mainFrame.CanGoBack)
            {
                mainFrame.RemoveBackEntry();
            }
        }
    }
}
