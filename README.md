# PrintDialogX v3.0.0-dev

[![C#](https://img.shields.io/badge/C%23-100%25-blue.svg?style=flat-square)](#)
[![Platform](https://img.shields.io/badge/Platform-WPF-green.svg?style=flat-square)](#)
[![Target .Net](https://img.shields.io/badge/.Net-%E2%89%A56.0-green.svg?style=flat-square)](#)
[![Target .Net Framework](https://img.shields.io/badge/.Net%20Framework-%E2%89%A54.7.2-green.svg?style=flat-square)](#)
[![Nuget](https://img.shields.io/nuget/v/PrintDialogX?label=Nuget&style=flat-square&logo=nuget)](https://www.nuget.org/packages/PrintDialogX)
[![Lincense](https://img.shields.io/github/license/Fei-Sheng-Wu/PrintDialogX?label=License&style=flat-square)](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/LICENSE.txt)

> A custom PrintDialog for WPF with preview in real-time. Full options with print settings, including copies, custom pages, orientation, color, quality, scale, pages-per-sheet, double-siding, paper size, paper type, paper source, etc. Support updatable documents according to the changes in settings. Equipped with a fast and elegant user interface.

## Preview

![Screenshot](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/screenshot.png)

## Features

PrintDialogX is a powerful and beautiful customized print dialog. It basically supports all functions with the default Windows print dialog, but also provides extra functions and real-time previews. The printer settings only use the given printer's allowed options. The document being printed is also flexible and available for changes in content according to the adjusted settings by the user. The show-while-generate-document feature also allows a fast and user-friendly experience, where the document is generated dynamically while the print dialog is preparing itself.

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

- Wpf.Ui ≥ 4.0.0

## How to Use

> [!IMPORTANT]
> Please note that the following examples only apply to the previous version v2.1.6. The current development version introduces breaking changes that may invalidate the following code snippets. Furthermore, aspects such as the preview and the feature list may not accurately reflect the current state of the repository.

An example project is included in the [PrintDialogX.Test](https://github.com/Fei-Sheng-Wu/PrintDialogX/tree/master/PrintDialogX.Test) subfolder, with step-by-step configurations of `PrintDialog` and both examples of generating the document before or during the opening of `PrintDialog`.

Initialize a `PrintDialog` instance.

```c#
//Initialize a PrintDialog instance and set its properties
PrintDialogX.PrintDialog.PrintDialog printDialog = new PrintDialogX.PrintDialog.PrintDialog()
{
    Owner = this, //Set the owner of the dialog
    Title = "Print", //Set the title of the dialog
};
```

### Synchronized Document Generation

The document may be generated while the `PrintDialog` is loading, which is beneficial for larger documents.

```
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
```
```c#
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
```

### Pre-Generated Documents

If the document is already created, `PrintDialog.Document` can be directly set before calling `PrintDialog.ShowDialog()`.

```c#
printDialog.Document = document;

//Show PrintDialog with the document already generated
if (printDialog.ShowDialog() == true)
{
    //...
}
```

### PrintDialog Configurations

The defulat print settings that `PrintDialog` uses can be configured.

```c#
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
```

The interface of `PrintDialog` can be customized to disable certain settings.

```c#
printDialog.AllowPagesOption = true; //Allow the "Pages" option (contains "All Pages", "Current Page", and "Custom Pages")
printDialog.AllowPagesPerSheetOption = true; //Allow the "Pages Per Sheet" option
printDialog.AllowPageOrderOption = true; //Allow the "Page Order" option
printDialog.AllowScaleOption = true; //Allow the "Scale" option
printDialog.AllowDoubleSidedOption = true; //Allow the "Double-Sided" option
printDialog.AllowAddNewPrinterButton = true; //Allow the "Add New Printer" button in the printer list
printDialog.AllowPrinterPreferencesButton = true; //Allow the "Printer Preferences" button
```

### Dynamic Updatable Documents

`PrintDialog.ReloadDocumentCallback` can be set for updatable documents, where the content of the document can be updated based on the print settings. The callback function receives an instance of `PrintDialog.DocumentInfo` and must return a collection of updated `PrintPage` in the original length and order.

```c#
//Set the function that will be used to regenerate the document when the print settings are changed
printDialog.ReloadDocumentCallback = ReloadDocumentCallback;
```
```c#
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
```

## License

This project is under the [MIT License](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/LICENSE.txt).
