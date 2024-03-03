# PrintDialogX

[![C#](https://img.shields.io/badge/C%23-100%25-blue.svg?style=flat-square)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Subsystem](https://img.shields.io/badge/Platform-WPF-green.svg?style=flat-square)](https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf)
[![Nuget](https://img.shields.io/badge/Nuget-v2.1.1-blue.svg?style=flat-square)](https://www.nuget.org/packages/PrintDialogX/2.1.1)
[![Lincense](https://img.shields.io/badge/Lincense-MIT-orange.svg?style=flat-square)](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/2.1.1/LICENSE.txt)

> A custom PrintDialog for WPF with preview in realtime. Full options with printer settings, include copies, custom pages, orientation, color, quality, scale, pages-per-sheet, double-sided, paper size, paper type, paper source, etc. Support realtime updates to the content according to the changes in settings. Fast and elegant user interface.

## Preview

![Screenshot](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/8c1c32120c5ba5ec3e6547d825c56a5b27fb5ee2/Screenshot.png)

## Features

PrintDialogX is a powerful and beautiful customized print dialog. It basically supports all functions with the default Windows print dialog, but also provides extra functions and realtime previews. The printer settings only use the given printer's allowed options. The document being printed is also flexible and available for changes in content according to the adjusted settings by the user. The show-while-generate-document feature also allows a fast and user-friendly experience, where the document is generated dynamically while the print dialog is preparing itself.

- [X] Printer list
  - [X] Printer icons & status
  - [X] "Add New Printer" button
  - [X] Tooltip on printer options for detailed information
- [X] Printer settings
  - [X] Copies and collate
  - [X] Pages (all, current, or custom)
  - [X] Orientation
  - [X] Color and quality
  - [X] Pages per sheet and page order
  - [X] Scale and margin
  - [X] Doubled-sided and flipping
  - [X] Paper size, type, and source
- [X] Interactable realtime preview
  - [X] Zooming and text selection
  - [X] Page position and navigation
- [X] Updatable document
  - [X] Document reloading callback for specfic printer settings
  - [X] Realtime update on the content
- [X] Result callback
  - [X] Whether the "Print" button is clicked or the "Cancel" button
  - [X] The number of papers used
- [X] Beautiful user interface
  - [X] Uses [Wpf.Ui](https://wpfui.lepo.co/index.html)
  - [X] Customizable disabling of certain settings

## Dependencies

- .Net Framework >= 4.8
- Wpf.Ui = 3.0.0

## How to Use

The example project is included in the [PrintDialogX.Test](https://github.com/Fei-Sheng-Wu/PrintDialogX/tree/2.1.1/PrintDialogX.Test) subfolder, with both examples of the show-while-generate-document feature, where the document is generated while the print dialog is showing, and the old method of generating the document beforehand and showing the print dialog after.

Initialize a `PrintDialog` instance.

```c#
//Initialize a PrintDialog and set its properties
PrintDialogX.PrintDialog.PrintDialog printDialog = new PrintDialogX.PrintDialog.PrintDialog()
{
    Owner = this, //Set PrintDialog's owner
    Title = "Test Print", //Set PrintDialog's title
};
```

The show-while-generate-document feature allows the document to be generated while the `PrintDialog` is loading. `GeneratingDocument` is a function that will be called to generate the document.

```
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
```

Example of the `GeneratingDocument` function, where the document is created.

```c#
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
        page.Content = CreateContent(); //Create any content as you wish

        //Add the page into the document
        document.Pages.Add(page);
    }

    //Set the PrintDialog's document
    printDialog.Document = document;
}
```

If the document is already created, `PrintDialog.ShowDialog()` can be called to skip the loading part.

```c#
//Generate the document before showing the dialog
printDialog.Document = document;

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
```

Default print settings of the `PrintDialog` can be set as well.

```c#
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
```

The interface of the `PrintDialog` can be customized and certain settings can be disabled.

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

`PrintDialog.ReloadDocumentCallback` can be set for updatable documents, where the content of the document can be updated based on print settings. The callback function needs to receive a `PrintDialog.DocumentInfo` as the parameter and needs to return a list of `PrintPage`.

```c#
//Set the function that will use to recreate the document when the print settings changed
printDialog.ReloadDocumentCallback = ReloadDocumentCallback;
```
```c#
private List<PrintDialogX.PrintPage> ReloadDocumentCallback(PrintDialogX.PrintDialog.DocumentInfo documentInfo)
{
    //Optinal callback function to recreate the page contents with the specific settings
    List<PrintDialogX.PrintPage> pages = new List<PrintDialogX.PrintPage>();

    //All pages should be recreated
    //PrintDialog will take care of the pages setting regarding of which pages need to be printed
    //The DocumentInfo.Pages info can still be used such as to adjust pages that will be printed
    for (int i = 0; i < 5; i++)
    {
        //Create the new page and recreate the content with the specific margin
        PrintPage page = new PrintPage();
        page.Content = CreateContent(); //Things like documentInfo.Size and documentInfo.Margin can be used
        pages.Add(page);
    }

    //Passed the recreated document back to the PrintDialog
    return pages;
}
```

## License

This project is under the [MIT License](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/2.1.1/LICENSE.txt).
