# PrintDialogX

[![C#](https://img.shields.io/badge/C%23-100%25-blue.svg?style=flat-square)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Subsystem](https://img.shields.io/badge/Platform-WPF-green.svg?style=flat-square)](https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf)
[![Nuget](https://img.shields.io/badge/Nuget-v2.0.2-blue.svg?style=flat-square)](https://www.nuget.org/packages/PrintDialogX/2.0.2)
[![Lincense](https://img.shields.io/badge/Lincense-MIT-orange.svg?style=flat-square)](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/LICENSE.txt)

> A custom PrintDialog for WPF with preview in realtime. Full options with printer settings, include copies, custom pages, orientation, color, quality, scale, pages-per-sheet, double-sided, paper size, paper type, paper source, etc. Support realtime updates to the content according to the changes in settings. Fast and elegant user interface.

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

## Dependencies

- .Net Framework >= 4.8
- Wpf.Ui = 3.0.0

## Preview

![Screenshot](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/8c1c32120c5ba5ec3e6547d825c56a5b27fb5ee2/Screenshot.png)

## How to Use

The example project is included in the [PrintDialogX.Test](https://github.com/Fei-Sheng-Wu/PrintDialogX/tree/2.0.2/PrintDialogX.Test) subfolder, with both examples of the show-while-generate-document feature, where the document is generated while the print dialog is showing, and the old method of generating the document beforehand and showing the print dialog after.

Show-while-generate-document feature, where `GeneratingDocument` is the function callback used to generate the document:

```c#
//Initialize a PrintDialog and set its properties
PrintDialogX.PrintDialog.PrintDialog printDialog = new PrintDialogX.PrintDialog.PrintDialog()
{
    Owner = this, //Set PrintDialog's owner
    Title = "Test Print", //Set PrintDialog's title
    Icon = null, //Set PrintDialog's icon (null means use the default icon)
    Topmost = false, //Don't allow PrintDialog to be at topmost
    ShowInTaskbar = true,//Don't allow PrintDialog to show in taskbar
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
```

Default settings of the print dialog can be set as well:

```c#
printDialog.DefaultSettings = new PrintDialogX.PrintDialog.PrintDialogSettings() //Set default settings
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
//Or you can just use PrintDialog.PrintDialogSettings.PrinterDefaultSettings() to get a PrintDialogSettings that uses the printer's default settings
//printDialog.DefaultSettings = PrintDialog.PrintDialogSettings.PrinterDefaultSettings()
```

## License

This project is under the [MIT License](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/2.0.2/README.md).
