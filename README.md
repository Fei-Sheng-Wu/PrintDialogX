# PrintDialogX v2.1.4-dev

[![C#](https://img.shields.io/badge/C%23-100%25-blue.svg?style=flat-square)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Platform](https://img.shields.io/badge/Platform-WPF-green.svg?style=flat-square)](https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf)
[![Nuget](https://img.shields.io/badge/Nuget-v2.1.2-blue.svg?style=flat-square)](https://www.nuget.org/packages/PrintDialogX/2.1.2)
[![Lincense](https://img.shields.io/badge/Lincense-MIT-orange.svg?style=flat-square)](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/LICENSE.txt)

> A custom PrintDialog for WPF with preview in real-time. Full options with print settings, including copies, custom pages, orientation, color, quality, scale, pages-per-sheet, double-siding, paper size, paper type, paper source, etc. Support updatable documents according to the changes in settings. Equipped with a fast and elegant user interface.

## Preview

![Screenshot](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/8c1c32120c5ba5ec3e6547d825c56a5b27fb5ee2/Screenshot.png)

## Features

PrintDialogX is a powerful and beautiful customized print dialog. It basically supports all functions with the default Windows print dialog, but also provides extra functions and realtime previews. The printer settings only use the given printer's allowed options. The document being printed is also flexible and available for changes in content according to the adjusted settings by the user. The show-while-generate-document feature also allows a fast and user-friendly experience, where the document is generated dynamically while the print dialog is preparing itself.

- [X] Printer list
  - [X] Printer icons & status
  - [X] "Add New Printer" button
  - [X] Tooltips for detailed printer information
- [X] Print settings
  - [X] Copies and collate
  - [X] Pages (all, current, or custom)
  - [X] Orientation
  - [X] Color and quality
  - [X] Pages per sheet and order
  - [X] Scale and margin
  - [X] Doubled-sided and flipping
  - [X] Paper size, type, and source
- [X] Interactable real-time preview
  - [X] Zooming and text selection
  - [X] Page position and navigation
- [X] Updatable documents
  - [X] Document reloading callback for specfic print settings
  - [X] Real-time updates on the content
- [X] Result callbacks
  - [X] Whether the "Print" or the "Cancel" button is clicked
  - [X] The number of papers used
- [X] Beautiful user interface
  - [X] Uses [Wpf.Ui](https://wpfui.lepo.co/index.html)
  - [X] Customizable disabling of certain settings

## Dependencies

- .Net Framework ≥ 4.7.2
- Wpf.Ui = 3.0.0

## How to Use

The example project is included in the [PrintDialogX.Test](https://github.com/Fei-Sheng-Wu/PrintDialogX/tree/master/PrintDialogX.Test) subfolder, with both examples of the generating the document before or during the starting stage of the print dialog.

Initialize a `PrintDialog` instance.

```c#
//Initialize a PrintDialog and set its properties
PrintDialogX.PrintDialog.PrintDialog printDialog = new PrintDialogX.PrintDialog.PrintDialog()
{
    Owner = this, //Set PrintDialog's owner
    Title = "Test Print", //Set PrintDialog's title
};
```

### Synchronized Document Generation

The document may be generated while the `PrintDialog` is loading, which is beneficial for larger documents.

```
//Show PrintDialog with a custom document generation function
//The function will be used to generate the document while the dialog is loading
if (printDialog.ShowDialog(() => printDialog.Document = GenerateDocument()) == true)
{
    //When the Print button is clicked, the document is printed, and the window is closed
    MessageBox.Show("Document printed.\nIt uses " + printDialog.TotalPapers + " sheet(s) of paper.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
}
else
{
    //When the Cancel button is clicked, and the window is closed
    MessageBox.Show("Print job canceled.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
}
```
```c#
private PrintDialogX.PrintDocument GenerateDocument()
{
    //Create a new document
    //PrintDialogX requires a PrintDocument instance as the document
    PrintDialogX.PrintDocument document = new PrintDialogX.PrintDocument();
    document.DocumentSize = new Size(96 * 8.25, 96 * 11.75); //A4 paper size, 8.25 inch x 11.75 inch
    document.DocumentMargin = 60; //Default margin

    //Loop 5 times to add 5 pages
    for (int i = 0; i < 5; i++)
    {
        //Create a new page and add its content
        PrintDialogX.PrintPage page = new PrintDialogX.PrintPage();
        page.Content = GeneratePageContent();

        //Add the page into the document
        document.Pages.Add(page);
    }

    return document;
}
```

### Pre-Generated Documents

If the document is already created, `PrintDialog.Document` can be directly set before calling `PrintDialog.ShowDialog()`.

```c#
//Generate the document before showing the dialog
printDialog.Document = document;

//Show PrintDialog with the document already generated
if (printDialog.ShowDialog() == true)
{
    //...
}
else
{
    //...
}
```

### PrintDialog Configurations

The defulat print settings that `PrintDialog` uses can be configured.

```c#
//Set the default print settings
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
//PrinterDefaultSettings() can also be used to use the default settings of the printer
//printDialog.DefaultSettings = PrintDialogX.PrintDialog.PrintDialogSettings.PrinterDefaultSettings()
```

The interface of `PrintDialog` can be customized to disable certain settings.

```c#
printDialog.AllowScaleOption = true; //Allow scale option
printDialog.AllowPagesOption = true; //Allow pages option (contains "All Pages", "Current Page", and "Custom Pages")
printDialog.AllowDoubleSidedOption = true; //Allow double-sided option
printDialog.AllowPagesPerSheetOption = true; //Allow pages per sheet option
printDialog.AllowPageOrderOption = true; //Allow page order option
printDialog.AllowAddNewPrinterButton = true; //Allow add new printer button in the printer list
printDialog.AllowMoreSettingsExpander = true; //Allow more settings expander
printDialog.AllowPrinterPreferencesButton = true; //Allow printer preferences button
```

### Dynamic Updatable Documents

`PrintDialog.ReloadDocumentCallback` can be set for updatable documents, where the content of the document can be updated based on the print settings. The callback function receives an instance of `PrintDialog.DocumentInfo` as a parameter and must return a list of updated `PrintPage`.

```c#
//Set the function that will be uses to regenerate the document when the print settings are changed
printDialog.ReloadDocumentCallback = ReloadDocumentCallback;
```
```c#
private ICollection<PrintDialogX.PrintPage> ReloadDocumentCallback(PrintDialogX.PrintDialog.DocumentInfo documentInfo)
{
    //Optional callback function to recreate the content of the document with specific settings
    //An instance of PrintDialog.DocumentInfo is received as a parameter, which can be used to retrieve the current print settings set by the user
    //This function will only be called when the print settings are changed, and it must return a list of PrintPage that include each page in the original order
    List<PrintDialogX.PrintPage> pages = new List<PrintDialogX.PrintPage>();

    //All pages must be recreated and add to the list
    //PrintDialog takes care of the pages setting regarding which pages are to be printed
    for (int i = 0; i < 5; i++)
    {
        //Create the new page and recreate the content with the specific margin
        PrintPage page = new PrintPage();
        page.Content = GeneratePageContent();
        pages.Add(page);
    }

    //Pass the recreated document back to PrintDialog
    return pages;
}
```

## License

This project is under the [MIT License](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/LICENSE.txt).
