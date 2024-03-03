using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintDialogX.Test
{
    public partial class MainWindow : Window
    {
        //Global variable used to store the instance of PrintDialog
        PrintDialogX.PrintDialog.PrintDialog printDialog;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Test1ButtonClick (test PrintDialogX with the built-in show-while-generate-document feature)

        private void Test1ButtonClick(object sender, RoutedEventArgs e)
        {
            //Initialize a PrintDialog
            InitializePrintDialog();

            //Show PrintDialog with document generation function
            //The document will be generated while the dialog loads
            if (printDialog.ShowDialog(GeneratingDocument) == true)
            {
                //When the Print button is clicked, the document is printed, and the window is closed
                MessageBox.Show("Document printed.\nIt uses " + printDialog.TotalPapers + " sheet(s) of paper.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            else
            {
                //When the Cancel button is clicked and the window is closed
                MessageBox.Show("Print job canceled.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        #endregion

        #region Test2ButtonClick (test PrintDialogX by generating the document before display the dialog)

        private void Test2ButtonClick(object sender, RoutedEventArgs e)
        {
            //Initialize a PrintDialog
            InitializePrintDialog();

            //Generate the document before showing the dialog
            GeneratingDocument();

            //Show PrintDialog with the document already generated
            if (printDialog.ShowDialog() == true)
            {
                //When the Print button clicked, the document is printed, and the window is closed
                MessageBox.Show("Document printed.\nIt uses " + printDialog.TotalPapers + " sheet(s) of paper.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            else
            {
                //When the Cancel button is clicked and the window is closed
                MessageBox.Show("Print job canceled.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        #endregion

        #region PrintDialog Initialization

        private void InitializePrintDialog()
        {
            //Initialize a PrintDialog and set its properties
            printDialog = new PrintDialogX.PrintDialog.PrintDialog()
            {
                Owner = this, //Set PrintDialog's owner
                Title = "Test Print", //Set PrintDialog's title
                Icon = null, //Set PrintDialog's icon (null means use the default icon)
                Topmost = false, //Don't allow PrintDialog to be at topmost
                ShowInTaskbar = true, //Allow PrintDialog to show in taskbar
                ResizeMode = ResizeMode.NoResize, //Don't allow PrintDialog to resize
                WindowStartupLocation = WindowStartupLocation.CenterOwner, //PrintDialog's startup location is the center of the owner

                AllowScaleOption = true, //Allow scale option
                AllowPagesOption = true, //Allow pages option (contains "All Pages", "Current Page", and "Custom Pages")
                AllowDoubleSidedOption = true, //Allow double-sided option
                AllowPagesPerSheetOption = true, //Allow pages per sheet option
                AllowPageOrderOption = true, //Allow page order option
                AllowAddNewPrinterButton = true, //Allow add new printer button in the printer list
                AllowMoreSettingsExpander = true, //Allow more settings expander
                AllowPrinterPreferencesButton = true //Allow printer preferences button
            };

            //Set default print settings
            printDialog.DefaultSettings = new PrintDialogX.PrintDialog.PrintDialogSettings()
            {
                Layout = PrintDialogX.PrintSettings.PageOrientation.Portrait,
                Color = PrintDialogX.PrintSettings.PageColor.Color,
                Quality = PrintDialogX.PrintSettings.PageQuality.Normal,
                PageSize = PrintDialogX.PrintSettings.PageSize.ISOA4,
                PageType = PrintDialogX.PrintSettings.PageType.Plain,
                DoubleSided = PrintDialogX.PrintSettings.DoubleSided.DoubleSidedLongEdge,
                PagesPerSheet = PrintDialogX.PrintSettings.PagesPerSheet.One,
                PageOrder = PrintDialogX.PrintSettings.PageOrder.Horizontal
            };
            //PrinterDefaultSettings() can also be used to use default settings of the printer
            //printDialog.DefaultSettings = PrintDialog.PrintDialogSettings.PrinterDefaultSettings()
        }

        #endregion

        #region Document Generation

        private void GeneratingDocument()
        {
            //Create a new document
            //PrintDialogX requires a PrintDocument instance as the document
            PrintDialogX.PrintDocument document = new PrintDialogX.PrintDocument();
            document.DocumentSize = new Size(96 * 8.25, 96 * 11.75); //A4 paper size, 8.25 inch x 11.75 inch
            document.DocumentMargin = 60; //Default margin

            //Loop 5 times to add 5 pages
            for (int i = 0; i < 5; i++)
            {
                //Create a new page and add content to it
                PrintDialogX.PrintPage page = new PrintDialogX.PrintPage();
                page.Content = CreateContent(i + 1, document.DocumentSize.Width, document.DocumentSize.Height, document.DocumentMargin);

                //Add the page into the document
                document.Pages.Add(page);
            }

            //Set the PrintDialog's document
            printDialog.Document = document;

            //Set the function that will use to recreate the document when the print settings changed
            printDialog.ReloadDocumentCallback = ReloadDocumentCallback;
        }

        private StackPanel CreateContent(int pageNumber, double width, double height, double margin)
        {
            //Create a StackPanel and make it covers the entire page
            //VerticalAlignment="Stretch" and HorizontalAlignment="Stretch" don't work with FixedPage, so the size needs to be calculated manually
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Background = Brushes.LightYellow,
                Width = 96 * 8.25 - margin * 2, //Width = page width - (left margin + right margin)
                Height = 96 * 11.75 - margin * 2 //Height = page height - (top margin + bottom margin)
            };

            stackPanel.Children.Add(new TextBlock() { Text = "This is the title of page #" + pageNumber, FontWeight = FontWeights.Bold, FontSize = 28, TextAlignment = TextAlignment.Center, Margin = new Thickness(10, 5, 10, 35) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some updating text: Width " + width + " | Height " + height + " | Margin " + margin, FontSize = 18, TextAlignment = TextAlignment.Center, Margin = new Thickness(10, 5, 10, 35) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some regular text.", Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some bold text.", FontWeight = FontWeights.Bold, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some italic text.", FontStyle = FontStyles.Italic, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some different colored text.", Foreground = Brushes.Red, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock()
            {
                Text = "This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph.This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph.",
                MaxWidth = stackPanel.Width,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 5, 10, 35)
            });
            stackPanel.Children.Add(new Button() { Content = "This is a button.", Margin = new Thickness(10, 5, 10, 5), Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });
            stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Black, Background = Brushes.DarkGray, Foreground = Brushes.White, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Orange, Background = Brushes.Yellow, Foreground = Brushes.OrangeRed, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBox() { Text = "This is a textbox, but you can't type interact with it in a FixedDocument.", Margin = new Thickness(10, 25, 10, 35), Width = 550, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });
            stackPanel.Children.Add(new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("/PrintDialogX;component/Resources/LoadingImage.png", UriKind.Relative)), Margin = new Thickness(10, 5, 10, 5), Width = 200, Height = 200, HorizontalAlignment = HorizontalAlignment.Center });

            FixedPage.SetTop(stackPanel, 60);
            FixedPage.SetLeft(stackPanel, 60);

            return stackPanel;
        }

        #endregion

        #region Reload Document Callback (recreate pages with specific print settings)

        private List<PrintDialogX.PrintPage> ReloadDocumentCallback(PrintDialogX.PrintDialog.DocumentInfo documentInfo)
        {
            //Optinal callback function to recreate the page contents with the specific settings
            //An instance of PrintDialog.DocumentInfo should be received as the parameter
            //You can use this parameter to get the current print settings setted by user
            //This function will only be called when the print settings changed
            //This function must return a list of PrintPage that include each page content in order
            List<PrintDialogX.PrintPage> pages = new List<PrintDialogX.PrintPage>();

            //All pages should be recreated
            //PrintDialog will take care of the pages setting regarding of which pages need to be printed
            //The DocumentInfo.Pages info can still be used such as to adjust pages that will be printed
            for (int i = 0; i < 5; i++)
            {
                //Create the new page and recreate the content with the specific margin
                PrintPage page = new PrintPage();
                page.Content = CreateContent(i + 1, (int)documentInfo.Size.Width, (int)documentInfo.Size.Height, (int)documentInfo.Margin);
                pages.Add(page);
            }

            //Passed the recreated document back to the PrintDialog
            return pages;
        }

        #endregion
    }
}
