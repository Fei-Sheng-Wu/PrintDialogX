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

        #region Test1ButtonClick (test PrintDialogX with built-in show-while-generate-document feature)

        private void Test1ButtonClick(object sender, RoutedEventArgs e)
        {
            //Test1ButtonClick method
            //Test PrintDialogX with built-in show-while-generate-document feature

            //Initialize a PrintDialog and set its properties
            printDialog = new PrintDialogX.PrintDialog.PrintDialog()
            {
                Owner = this, //Set PrintDialog's owner
                Title = "Test Print", //Set PrintDialog's title
                Icon = null, //Set PrintDialog's icon (null means use the default icon)
                Topmost = false, //Don't allow PrintDialog to be at topmost
                ShowInTaskbar = true, //Allow PrintDialog to show in taskbar
                ResizeMode = ResizeMode.NoResize, //Don't allow PrintDialog to resize
                WindowStartupLocation = WindowStartupLocation.CenterOwner //PrintDialog's startup location is the center of the owner
            };

            //Show PrintDialog and begin to generate document
            if (printDialog.ShowDialog(true, GeneratingDocument) == true)
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

        private void GeneratingDocument()
        {
            //Create a new document
            //PrintDialog can only print and preview a FixedDocument
            //Here are some codes to make a document, if you already know how to do it, you can skip it
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.DocumentPaginator.PageSize = new Size(96 * 8.25, 96 * 11.75); //A4 paper size, 8.25 inch x 11.75 inch

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
                StackPanel stackPanel = CreateContent(i + 1, fixedDocument.DocumentPaginator.PageSize.Width, fixedDocument.DocumentPaginator.PageSize.Height, margin);

                //Add the StackPanel into the page
                //You can add as many elements as you want into the page, but at here we only need to add one
                fixedPage.Children.Add(stackPanel);

                //Add the page into the document
                //You can't just add FixedPage into FixedDocument, you need use PageContent to host the FixedPage
                fixedDocument.Pages.Add(new PageContent() { Child = fixedPage });
            }

            //Setup PrintDialog's properties
            printDialog.Document = fixedDocument; //Set document that needs to be printed
            printDialog.DocumentName = "Test Document"; //Set document name that will be displayed
            printDialog.DocumentMargin = margin; //Set document margin info
            printDialog.DefaultSettings = new PrintDialogX.PrintDialog.PrintDialogSettings() //Set default settings. Or you can just use PrintDialog.PrintDialogSettings.PrinterDefaultSettings() to get a PrintDialogSettings that uses the printer's default settings
            {
                Layout = PrintDialogX.PrintSettings.PageOrientation.Portrait,
                Color = PrintDialogX.PrintSettings.PageColor.Color,
                Quality = PrintDialogX.PrintSettings.PageQuality.Normal,
                PageSize = PrintDialogX.PrintSettings.PageSize.ISOA4,
                PageType = PrintDialogX.PrintSettings.PageType.Plain,
                DoubleSided = PrintDialogX.PrintSettings.DoubleSided.DoubleSidedLongEdge,
                PagesPerSheet = 1,
                PageOrder = PrintDialogX.PrintSettings.PageOrder.Horizontal
            };
            //Or printDialog.DefaultSettings = PrintDialog.PrintDialogSettings.PrinterDefaultSettings()

            printDialog.AllowScaleOption = true; //Allow scale option
            printDialog.AllowPagesOption = true; //Allow pages option (contains "All Pages", "Current Page", and "Custom Pages")
            printDialog.AllowDoubleSidedOption = true; //Allow double-sided option
            printDialog.AllowPagesPerSheetOption = true; //Allow pages per sheet option
            printDialog.AllowPageOrderOption = true; //Allow page order option
            printDialog.AllowAddNewPrinterButton = true; //Allow add new printer button in the printer list
            printDialog.AllowMoreSettingsExpander = true; //Allow more settings expander
            printDialog.AllowPrinterPreferencesButton = true; //Allow printer preferences button

            printDialog.CustomReloadDocumentMethod = ReloadDocumentMethod; //Set the method that will use to recreate the document when print settings changed.

            //Switch the current running PrintDialog's page into settings and preview page
            printDialog.LoadingEnd();
        }

        #endregion

        #region Test2ButtonClick (test PrintDialogX by generate document before display the print dialog)

        private void Test2ButtonClick(object sender, RoutedEventArgs e)
        {
            //Test2ButtonClick method
            //Test PrintDialogX by generate document before display the print dialog

            //Initialize a PrintDialog and set its properties
            printDialog = new PrintDialogX.PrintDialog.PrintDialog()
            {
                Owner = this, //Set PrintDialog's owner
                Title = "Test Print", //Set PrintDialog's title
                Icon = null, //Set PrintDialog's icon (null means the default icon)
                Topmost = false, //Don't allow PrintDialog to be at topmost
                ShowInTaskbar = true, //Allow PrintDialog to show in taskbar
                ResizeMode = ResizeMode.NoResize, //Don't allow PrintDialog to resize
                WindowStartupLocation = WindowStartupLocation.CenterOwner //PrintDialog's startup location is the center of the owner
            };

            //Create a new document
            //PrintDialog can only print and preview a FixedDocument
            //Here are some codes to make a document, if you already know how to do it, you can skip it
            FixedDocument fixedDocument = new FixedDocument();
            fixedDocument.DocumentPaginator.PageSize = new Size(96 * 8.25, 96 * 11.75); //A4 paper size, 8.25 inch x 11.75 inch

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
                StackPanel stackPanel = CreateContent(i + 1, fixedDocument.DocumentPaginator.PageSize.Width, fixedDocument.DocumentPaginator.PageSize.Height, margin);

                //Add the StackPanel into the page
                //You can add as many elements as you want into the page, but at here we only need to add one
                fixedPage.Children.Add(stackPanel);

                //Add the page into the document
                //You can't just add FixedPage into FixedDocument, you need use PageContent to host the FixedPage
                fixedDocument.Pages.Add(new PageContent() { Child = fixedPage });
            }

            //Setup PrintDialog's properties
            printDialog.Document = fixedDocument; //Set document that needs to be printed
            printDialog.DocumentName = "Test Document"; //Set document name that will be displayed
            printDialog.DocumentMargin = margin; //Set document margin info
            printDialog.DefaultSettings = new PrintDialogX.PrintDialog.PrintDialogSettings() //Set default settings. Or you can just use PrintDialog.PrintDialogSettings.PrinterDefaultSettings() to get a PrintDialogSettings that uses the printer's default settings
            {
                Layout = PrintDialogX.PrintSettings.PageOrientation.Portrait,
                Color = PrintDialogX.PrintSettings.PageColor.Color,
                Quality = PrintDialogX.PrintSettings.PageQuality.Normal,
                PageSize = PrintDialogX.PrintSettings.PageSize.ISOA4,
                PageType = PrintDialogX.PrintSettings.PageType.Plain,
                DoubleSided = PrintDialogX.PrintSettings.DoubleSided.DoubleSidedLongEdge,
                PagesPerSheet = 1,
                PageOrder = PrintDialogX.PrintSettings.PageOrder.Horizontal
            };
            //Or printDialog.DefaultSettings = PrintDialog.PrintDialogSettings.PrinterDefaultSettings()

            printDialog.AllowScaleOption = true; //Allow scale option
            printDialog.AllowPagesOption = true; //Allow pages option (contains "All Pages", "Current Page", and "Custom Pages")
            printDialog.AllowDoubleSidedOption = true; //Allow double-sided option
            printDialog.AllowPagesPerSheetOption = true; //Allow pages per sheet option
            printDialog.AllowPageOrderOption = true; //Allow page order option
            printDialog.AllowAddNewPrinterButton = true; //Allow add new printer button in the printer list
            printDialog.AllowMoreSettingsExpander = true; //Allow more settings expander
            printDialog.AllowPrinterPreferencesButton = true; //Allow printer preferences button

            printDialog.CustomReloadDocumentMethod = ReloadDocumentMethod; //Set the method that will use to recreate the document when print settings changed

            //Show PrintDialog
            if (printDialog.ShowDialog(false, null) == true)
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

        #region Common Methods (generate page content for document)

        private StackPanel CreateContent(int pageNumber, double width, double height, double margin)
        {
            //Create a StackPanel and make it cover entire page
            //FixedPage can contains any UIElement. But VerticalAlignment="Stretch" or HorizontalAlignment="Stretch" doesn't work, so you need calculate its size to make it cover the entire page
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Background = Brushes.LightYellow,
                Width = width - margin * 2, //Width = Page width - (left margin + right margin)
                Height = height - margin * 2 //Height = Page height - (top margin + bottom margin)
            };

            //Put some elements into StackPanel (as same way as normal and they have styles, but triggers and animations don't work)
            //You can include any control that override the UIElement class
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
            }); //You need to set MaxWidth and TextWrapping properties to make a multi-line paragraph.
            //Buttons, textboxes, and other controls can also works
            stackPanel.Children.Add(new Button() { Content = "This is a button.", Margin = new Thickness(10, 5, 10, 5), Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });
            stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Black, Background = Brushes.DarkGray, Foreground = Brushes.White, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = Brushes.Orange, Background = Brushes.Yellow, Foreground = Brushes.OrangeRed, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBox() { Text = "This is a textbox, but you can't type interact with it in a FixedDocument.", Margin = new Thickness(10, 25, 10, 35), Width = 550, Height = 30, VerticalContentAlignment = VerticalAlignment.Center });
            stackPanel.Children.Add(new Image() { Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("/PrintDialogX;component/Resources/LoadingImage.png", UriKind.Relative)), Margin = new Thickness(10, 5, 10, 5), Width = 200, Height = 200, HorizontalAlignment = HorizontalAlignment.Center });

            //Set element's margin (you can set both top, bottom, left and right. But usually, we only set top and left)
            //FixedPage doesn't have Margin or Padding property, so if you want a inner margin, you can use a container control to contains any element in the page and set its margin.
            //You can use Margin property to archive same effect, but the best way is use FixedPage.SetTop, FixedPage.SetLeft, FixedPage.SetBottom and FixedPage.SetRight methods
            FixedPage.SetTop(stackPanel, 60); //Top margin
            FixedPage.SetLeft(stackPanel, 60); //Left margin

            //Return the StackPanel
            return stackPanel;
        }

        #endregion

        #region Print Dialog Callback (recreate pages with specific print settings)

        private List<PageContent> ReloadDocumentMethod(PrintDialogX.PrintDialog.DocumentInfo documentInfo)
        {
            //Callback method used to recreate the page contents follow the specific settings
            //Not necessary for some documents
            //You need to receive an instance of PrintDialog.DocumentInfo as the parameter
            //You can use this parameter to get the current print settings setted by user
            //This method will only be called when the print settings changed
            //And this method must return a list of PageContent that include each page content in order
            List<PageContent> pages = new List<PageContent>();

            for (int i = 0; i < 5; i++)
            {
                //Calculate standard A4 paper size (8.25 inch x 11.75 inch).
                //pixel = inch x 96
                double pageWidth = 96 * 8.25;
                double pageHeight = 96 * 11.75;

                //Calculate the page size
                Size pageSize = new Size(pageWidth, pageHeight);

                //Create a new page
                FixedPage fixedPage = new FixedPage()
                {
                    Width = pageSize.Width,
                    Height = pageSize.Height
                };

                //Recreate the StackPanel by the specific margin passed from the print dialog
                StackPanel stackPanel = CreateContent(i + 1, pageSize.Width, pageSize.Height, documentInfo.Margin.Value);

                //Add the page into the document
                fixedPage.Children.Add(stackPanel);
                pages.Add(new PageContent() { Child = fixedPage });
            }

            //Passed the recreated document back to the print dialog so the print dialog can rerender the preview with the new one
            return pages;
        }

        #endregion
    }
}
