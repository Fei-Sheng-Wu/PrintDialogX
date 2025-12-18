using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintDialogX.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PrintDialogX.PrintDialog.PrintDialog printDialog;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestButtonClick(object sender, RoutedEventArgs e)
        {
            //Initialize a PrintDialog and set its properties
            printDialog = new PrintDialogX.PrintDialog.PrintDialog
            {
                Owner = this, //Set PrintDialog's owner
                Title = "Test Print", //Set PrintDialog's title
                Icon = null, //Set PrintDialog's icon ( Null means use default icon )
                Topmost = false, //Don't allow PrintDialog at top most
                ShowInTaskbar = true,//Don't allow PrintDialog show in taskbar
                ResizeMode = ResizeMode.NoResize, //Don't allow PrintDialog resize
                WindowStartupLocation = WindowStartupLocation.CenterOwner //PrintDialog's startup location is center of the owner
            };

            //Show PrintDialog and begin to loading document
            if (printDialog.ShowDialog(true, LoadingDocument) == true)
            {
                //When Print button clicked, document printed and window closed
                MessageBox.Show("Document printed.\nIt need " + printDialog.TotalSheets + " sheet(s) of paper.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            else
            {
                //When Cancel button clicked and window closed
                MessageBox.Show("Print job canceled.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        private void LoadingDocument()
        {
            //Create a new document ( A document contains many pages )
            //PrintDialog can only print and preview a FixedDocument
            //Here are some codes to make a document, if you already know how to do it, you can skip it or put your document instead
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.DocumentPaginator.PageSize = PrintDialogX.PaperHelper.PaperHelper.GetPaperSize(System.Printing.PageMediaSizeName.ISOA4, true); //Use PaperHelper class to get A4 page size

            //Define document inner margin;
            double margin = 60;

            //Loop 5 times to add 5 pages.
            for (int i = 0; i < 5; i++)
            {
                //Create a new page and set its size
                //Page's size is equals document's size
                FixedPage fixedPage = new FixedPage()
                {
                    Width = fixedDocument.DocumentPaginator.PageSize.Width,
                    Height = fixedDocument.DocumentPaginator.PageSize.Height
                };

                //Create a StackPanel and make it cover entire page
                //FixedPage can contains any UIElement. But VerticalAlignment="Stretch" or HorizontalAlignment="Stretch" doesn't work, so you need calculate its size to make it cover page
                StackPanel stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Background = Brushes.LightYellow,
                    Width = fixedDocument.DocumentPaginator.PageSize.Width - margin * 2, //Width = Page width - (Left margin + Right margin)
                    Height = fixedDocument.DocumentPaginator.PageSize.Height - margin * 2 //Height = Page height - (Top margin + Bottom margin)
                };

                //Put some elements into StackPanel ( As same way as normal and it may have styles, but triggers and animations don't work )
                stackPanel.Children.Add(new TextBlock() { Text = "This is " + (i + 1).ToString() + " Page Title", FontWeight = FontWeights.Bold, FontSize = 28, TextAlignment = TextAlignment.Center, Margin = new Thickness(0, 5, 0, 35) });
                stackPanel.Children.Add(new TextBlock() { Text = "These are some regular text.", Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBlock() { Text = "These are some bold text.", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBlock() { Text = "These are some italic text.", FontStyle = FontStyles.Italic, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBlock() { Text = "These are some different color text.", Foreground = Brushes.Red, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBlock()
                {
                    Text = "This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph.This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph.",
                    MaxWidth = stackPanel.Width,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 5, 0, 5)
                }); //You need to set MaxWidth and TextWrapping properties to make a multi-line paragraph.
                stackPanel.Children.Add(new Button() { Content = "This is a button.", Margin = new Thickness(0, 5, 0, 5), Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });
                stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Black, Background = Brushes.DarkGray, Foreground = Brushes.White, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Orange, Background = Brushes.Yellow, Foreground = Brushes.OrangeRed, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBox() { Text = "This is a textbox, but you can't type text in FixedDocument.", Margin = new Thickness(0, 5, 0, 5), Width = 550, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });

                //Set element's margin ( You can set top, bottom, left and right. But usually, we only set top and left )
                //FixedPage doesn't have Margin or Padding property, so if you want a inner margin, you can use a container control ( Like Grid, StackPanel, WrapPanel, etc ) to contains any element in the page and set its margin.
                //You can use Margin property to make same thing, but the best way is use FixedPage.SetTop, FixedPage.SetLeft, FixedPage.SetBottom and FixedPage.SetRight methods
                FixedPage.SetTop(stackPanel, margin); //Top margin
                FixedPage.SetLeft(stackPanel, margin); //Left margin

                //Add element into page
                //You can add many elements into page, but at here we only add one
                fixedPage.Children.Add(stackPanel);

                //Add page into document
                //You can't just add FixedPage into FixedDocument, you need use PageContent to contains FixedPage
                fixedDocument.Pages.Add(new PageContent() { Child = fixedPage });
            }

            //Set PrintDialog's properties
            printDialog.Document = fixedDocument; //Set document that need to print
            printDialog.DocumentName = "Test Document"; //Set document name that will display in print list.
            printDialog.DocumentMargin = margin; //Set document margin info.
            printDialog.DefaultSettings = new PrintDialogX.PrintDialog.PrintDialogSettings() //Set default settings. Or you can just use PrintDialog.PrintDialogSettings.PrinterDefaultSettings() to get a PrintDialogSettings that use printer default settings
            {
                Layout = PrintDialogX.PrintSettings.PageOrientation.Portrait,
                Color = PrintDialogX.PrintSettings.PageColor.Color,
                Quality = PrintDialogX.PrintSettings.PageQuality.Normal,
                PageSize = PrintDialogX.PrintSettings.PageSize.ISOA4,
                PageType = PrintDialogX.PrintSettings.PageType.Plain,
                TwoSided = PrintDialogX.PrintSettings.TwoSided.TwoSidedLongEdge,
                PagesPerSheet = 1,
                PageOrder = PrintDialogX.PrintSettings.PageOrder.Horizontal
            };
            //Or DefaultSettings = PrintDialog.PrintDialogSettings.PrinterDefaultSettings(),

            printDialog.AllowScaleOption = true; //Allow scale option
            printDialog.AllowPagesOption = true; //Allow pages option (contains "All Pages", "Current Page", and "Custom Pages")
            printDialog.AllowTwoSidedOption = true; //Allow two-sided option
            printDialog.AllowPagesPerSheetOption = true; //Allow pages per sheet option
            printDialog.AllowPageOrderOption = true; //Allow page order option
            printDialog.AllowAddNewPrinterButton = true; //Allow add new printer button in printer list
            printDialog.AllowMoreSettingsExpander = true; //Allow more settings expander
            printDialog.AllowPrinterPreferencesButton = true; //Allow printer preferences button

            printDialog.CustomReloadDocumentMethod = ReloadDocumentMethod; //Set the method that will use to get document when reload document. You can only change the content in the pages.

            //Switch the current running PrintDialog's page into settings and preview page
            printDialog.LoadingEnd();
        }

        private List<PageContent> ReloadDocumentMethod(PrintDialogX.PrintDialog.DocumentInfo documentInfo)
        {
            //The PrintDialog.CustomReloadDocumentMethod property's value set as this method
            //This method can make the StackPanel cover the page with different margin
            //You must set a parameter and its type is PrintDialog.DocumentInfo
            //You can use this parameter to get the current document settings
            //And this method must return a list of PageContent that include each page content in order
            List<PageContent> pages = new List<PageContent>();

            for (int i = 0; i < 5; i++)
            {
                Size pageSize = PrintDialogX.PaperHelper.PaperHelper.GetPaperSize(System.Printing.PageMediaSizeName.ISOA4, true);

                FixedPage fixedPage = new FixedPage()
                {
                    Width = pageSize.Width,
                    Height = pageSize.Height
                };

                StackPanel stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Background = Brushes.LightYellow,
                    Width = pageSize.Width - documentInfo.Margin.Value * 2, //Important code, it makes StackPanel cover the page with different margin
                    Height = pageSize.Height - documentInfo.Margin.Value * 2 //Important code, it makes StackPanel cover the page with different margin
                };

                stackPanel.Children.Add(new TextBlock() { Text = "This is " + (i + 1).ToString() + " Page Title", FontWeight = FontWeights.Bold, FontSize = 28, TextAlignment = TextAlignment.Center, Margin = new Thickness(0, 5, 0, 35) });
                stackPanel.Children.Add(new TextBlock() { Text = "These are some regular text.", Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBlock() { Text = "These are some bold text.", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBlock() { Text = "These are some italic text.", FontStyle = FontStyles.Italic, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBlock() { Text = "These are some different color text.", Foreground = Brushes.Red, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBlock()
                {
                    Text = "This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph.This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph. This is a very long paragraph.",
                    MaxWidth = stackPanel.Width,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 5, 0, 5)
                });
                stackPanel.Children.Add(new Button() { Content = "This is a button.", Margin = new Thickness(0, 5, 0, 5), Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });
                stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Black, Background = Brushes.DarkGray, Foreground = Brushes.White, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Orange, Background = Brushes.Yellow, Foreground = Brushes.OrangeRed, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 5, 0, 5) });
                stackPanel.Children.Add(new TextBox() { Text = "This is a textbox, but you can't type text in FixedDocument.", Margin = new Thickness(0, 5, 0, 5), Width = 550, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });

                FixedPage.SetTop(stackPanel, 60);
                FixedPage.SetLeft(stackPanel, 60);

                fixedPage.Children.Add(stackPanel);
                pages.Add(new PageContent() { Child = fixedPage });
            }

            return pages;
        }
    }
}
