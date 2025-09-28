using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PrintDialogX.Test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Test1ButtonClick (test PrintDialogX by generating the document while starting the dialog)

        private void Test1ButtonClick(object sender, RoutedEventArgs e)
        {
            //Initialize a PrintDialog instance
            PrintDialogX.PrintDialog.PrintDialog printDialog = InitializePrintDialog();

            //Show PrintDialog with a custom document generation function
            //The function will be used to generate the document synchronously while the dialog is opening
            if (printDialog.ShowDialog(() => printDialog.Document = GenerateDocument()) == true)
            {
                //When the "Print" button was clicked, the print job was submitted, and the window was closed
                MessageBox.Show($"Document printed.\nIt uses {printDialog.TotalPapers} sheet(s) of paper.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            else
            {
                //When the "Cancel" button was clicked and the window was closed
                MessageBox.Show("Print job canceled.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        #endregion

        #region Test2ButtonClick (test PrintDialogX by generating the document before starting the dialog)

        private void Test2ButtonClick(object sender, RoutedEventArgs e)
        {
            //Initialize a PrintDialog instance
            PrintDialogX.PrintDialog.PrintDialog printDialog = InitializePrintDialog();

            //Generate the document before showing the dialog
            printDialog.Document = GenerateDocument();

            //Show PrintDialog with the document already generated
            if (printDialog.ShowDialog() == true)
            {
                //When the "Print" button was clicked, the print job was submitted, and the window was closed
                MessageBox.Show($"Document printed.\nIt uses {printDialog.TotalPapers} sheet(s) of paper.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            else
            {
                //When the "Cancel" button was clicked and the window was closed
                MessageBox.Show("Print job canceled.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        #endregion

        #region PrintDialog Initialization

        private PrintDialogX.PrintDialog.PrintDialog InitializePrintDialog()
        {
            //Initialize a PrintDialog instance and set its properties
            PrintDialogX.PrintDialog.PrintDialog printDialog = new PrintDialogX.PrintDialog.PrintDialog()
            {
                Owner = this, //Set the owner of the dialog to the current window
                InterpolationMode = System.Windows.Media.BitmapScalingMode.Linear, //Set the interpolation mode for scaling the preview
                Title = "Test Print", //Set the title of the dialog
                Icon = null, //Set the icon of the dialog, where null means to use the default icon
                Topmost = false, //Don't allow the dialog to appear in the topmost z-order
                ShowInTaskbar = true, //Allow the dialog to have a task bar button
                ResizeMode = ResizeMode.NoResize, //Don't allow the dialog to resize
                WindowStartupLocation = WindowStartupLocation.CenterOwner, //Set the position of the dialog when first shown to the center of the current window

                AllowPagesOption = true, //Allow the "Pages" option (contains "All Pages", "Current Page", and "Custom Pages")
                AllowPagesPerSheetOption = true, //Allow the "Pages Per Sheet" option
                AllowPageOrderOption = true, //Allow the "Page Order" option
                AllowScaleOption = true, //Allow the "Scale" option
                AllowDoubleSidedOption = true, //Allow the "Double-Sided" option
                AllowAddNewPrinterButton = true, //Allow the "Add New Printer" button in the printer list
                AllowPrinterPreferencesButton = true //Allow the "Printer Preferences" button
            };

            //Set the default print settings
            printDialog.DefaultSettings = new PrintDialogX.PrintDialog.PrintDialogSettings()
            {
                Layout = PrintDialogX.PrintSettings.PageOrientation.Portrait,
                Color = PrintDialogX.PrintSettings.PageColor.Color,
                Quality = PrintDialogX.PrintSettings.PageQuality.Normal,
                PagesPerSheet = PrintDialogX.PrintSettings.PagesPerSheet.One,
                PageOrder = PrintDialogX.PrintSettings.PageOrder.Horizontal,
                DoubleSided = PrintDialogX.PrintSettings.DoubleSided.DoubleSidedLongEdge,
                PageSize = PrintDialogX.PrintSettings.PageSize.NorthAmericaLetter,
                PageType = PrintDialogX.PrintSettings.PageType.Plain
            };
            //PrinterDefaultSettings() can also be used to use the default settings of printers
            //printDialog.DefaultSettings = PrintDialogX.PrintDialog.PrintDialogSettings.PrinterDefaultSettings();

            //Set the function that will be used to regenerate the document when the print settings are changed
            printDialog.ReloadDocumentCallback = ReloadDocumentCallback;

            return printDialog;
        }

        #endregion

        #region Document Generation

        private PrintDialogX.PrintDocument GenerateDocument()
        {
            //Create a new document
            //PrintDialogX requires a PrintDocument instance as the document
            PrintDialogX.PrintDocument document = new PrintDialogX.PrintDocument().SetSizeByInch(8.5, 11); //Letter size, 8.5 x 11 in
            document.DocumentMargin = 60; //Default margin, 60 pixels

            //Loop 5 times to add 5 pages
            for (int i = 0; i < 5; i++)
            {
                //Create a new page and add its content
                PrintDialogX.PrintPage page = new PrintDialogX.PrintPage();
                page.Content = GeneratePageContent(i + 1, document.DocumentSize.Width, document.DocumentSize.Height, document.DocumentMargin);

                //Add the page into the document
                document.Pages.Add(page);
            }

            return document;
        }

        private StackPanel GeneratePageContent(int pageNumber, double customWidth, double customHeight, double customMargin)
        {
            //Create a StackPanel that covers the entire page
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Background = System.Windows.Media.Brushes.LightYellow,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            //Add elements to the StackPanel
            //The ability to regenerate and update the document is demonstrated here as values from the "Paper Size" and the "Margin" options are displayed in the texts
            stackPanel.Children.Add(new TextBlock() { Text = $"This is the title of page #{pageNumber}", FontWeight = FontWeights.Bold, FontSize = 28, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 35) });
            stackPanel.Children.Add(new TextBlock() { Text = $"These are some updating text: Width {customWidth:0.##} px | Height {customHeight:0.##} px | Margin {customMargin:0.##} px", FontSize = 18, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 35) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some regular text.", HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some bold text.", FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some italic text.", FontStyle = FontStyles.Italic, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = "These are some different colored text.", Foreground = System.Windows.Media.Brushes.Red, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBlock() { Text = string.Concat(System.Linq.Enumerable.Repeat("This is a very long paragraph. ", 22)), TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Stretch, Margin = new Thickness(10, 5, 10, 35) });
            stackPanel.Children.Add(new Button() { Content = "This is a button.", Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = System.Windows.Media.Brushes.Black, Background = System.Windows.Media.Brushes.DarkGray, Foreground = System.Windows.Media.Brushes.White, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new Button() { Content = "This is a button with different color.", BorderBrush = System.Windows.Media.Brushes.Orange, Background = System.Windows.Media.Brushes.Yellow, Foreground = System.Windows.Media.Brushes.OrangeRed, Width = 250, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });
            stackPanel.Children.Add(new TextBox() { Text = "This is a textbox, but you can't type interact with it in a FixedDocument.", Width = 550, Height = 30, VerticalContentAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 25, 10, 35) });
            stackPanel.Children.Add(new Image() { Source = System.Windows.Media.Imaging.BitmapSource.Create(2, 2, 96, 96, System.Windows.Media.PixelFormats.Bgr24, null, new byte[] { 0, 0, 255, 255, 0, 0, 0, 255, 0, 255, 255, 255 }, 6), Width = 200, Height = 200, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(10, 5, 10, 5) });

            return stackPanel;
        }

        #endregion

        #region Reload Document Callback (regenerate pages with specific print settings)

        private ICollection<PrintDialogX.PrintPage> ReloadDocumentCallback(PrintDialogX.PrintDialog.DocumentInfo documentInfo)
        {
            //Optional callback function to recreate the content of the document with specific settings
            //An instance of PrintDialog.DocumentInfo is received as a parameter, which can be used to retrieve the current print settings set by the user
            //This function will only be called when the print settings are changed, and it must return a collection of PrintPage in the original length and order
            //The function has no need to alter the content to resolve basic changes in print settings, such as to resize the pages based on the new size in the print setting, as PrintDialog will take care of them
            //The function is intended to offer the ability to dynamically update the document for more complicated scenarios, such as styling asjuments or alternative formats for different media
            List<PrintDialogX.PrintPage> pages = new List<PrintDialogX.PrintPage>();

            //All pages must be recreated and add to the collection in the original manner
            //PrintDialog takes care of the "Pages" option regarding which pages are to be printed
            for (int i = 0; i < 5; i++)
            {
                //Create the new page and recreate the content with updated texts
                PrintPage page = new PrintPage();
                page.Content = GeneratePageContent(i + 1, documentInfo.Size.Width, documentInfo.Size.Height, documentInfo.Margin);
                pages.Add(page);
            }

            //Pass the collection of recreated pages back to PrintDialog
            return pages;
        }

        #endregion
    }
}
