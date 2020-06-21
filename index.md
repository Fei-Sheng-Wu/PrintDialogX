# PrintDialog

Welcome to use **[PrintDialog](https://github.com/Jet20070731/PrintDialog/)**, this is a open source project. Free to commercial use.

[Click here to download the DLL library file](https://github.com/Jet20070731/PrintDialog/blob/1.3.7.0/PrintDialog.dll)

## Why You Use

This is powerful and beautiful custom print dialog. It can almost provide any print settings, in the [latest vension](https://github.com/Jet20070731/PrintDialog/tree/1.3.7.0/). It can almost do anything what the Windows default print dialog can do. But the different between them is this custom print dialog have preview in real time. You can preview the print result when you adjust the settings. So you can use this instead the Windows default print dialog, and this is even better than it.

## How to Use

It is easy to use **[PrintDialog](https://github.com/Jet20070731/PrintDialog/)**, the codes below shows an example.

Initialize the document.

```c#
//Define document inner margin;
int margin = 60;

//Create a new document
//PrintDialog can only print and preview a FixedDocument
FixedDocument fixedDocument = new FixedDocument();
fixedDocument.DocumentPaginator.PageSize = PaperHelper.PaperHelper.GetPaperSize(System.Printing.PageMediaSizeName.ISOA4, true); //Use PaperHelper class to get A4 page size

//Create a new page and set its size
FixedPage fixedPage = new FixedPage()
{
    Width = fixedDocument.DocumentPaginator.PageSize.Width,
    Height = fixedDocument.DocumentPaginator.PageSize.Height
};

//Put somthing into fixedPage...

//Put page into document
fixedDocument.Pages.Add(new PageContent() { Child = fixedPage });
```

And show dialog.

```c#
//Initialize PrintDialog and set its properties
PrintDialog.PrintDialog printDialog = new PrintDialog.PrintDialog()
{
    Owner = this, //Set PrintDialog's owner
    Title = "Test Print", //Set PrintDialog's title
    Icon = null, //Set PrintDialog's icon ( Null means use default icon )
    Topmost = true, //Allow PrintDialog at top most
    ShowInTaskbar = false, //Don't allow PrintDialog show in taskbar
    ResizeMode = ResizeMode.NoResize, //Don't allow PrintDialog resize
    WindowStartupLocation = WindowStartupLocation.CenterScreen, //PrintDialog's startup location is center of the screen

    PrintDocument = fixedDocument, //Set document that need to print
    DocumentName = "Test Document", //Set document name that will display in print list.
    DocumentMargin = margin, //Set document margin info.
    DefaultSettings = new PrintDialog.PrintDialogSettings() //Set default settings.
    {
        Layout = PrintSettings.PageOrientation.Portrait,
        Color = PrintSettings.PageColor.Color,
        Quality = PrintSettings.PageQuality.Normal,
        PageSize = PrintSettings.PageSize.ISOA4,
        PageType = PrintSettings.PageType.Plain,
        TwoSided = PrintSettings.TwoSided.TwoSidedLongEdge
    },
    //Or DefaultSettings = PrintDialog.PrintDialogSettings.PrinterDefaultSettings(),

    AllowScaleOption = true, //Allow scale option
    AllowPagesOption = true, //Allow pages option (contains "All Pages", "Current Page", and "Custom Pages")
    AllowTwoSidedOption = true, //Allow two-sided option
    AllowPagesPerSheetOption = true, //Allow pages per sheet option
    AllowPageOrderOption = true, //Allow page order option
    AllowMoreSettingsExpander = true //Allow more settings expander
};

//Show PrintDialog
//It may need a longer time to connect printers
//But after first time, it will faster
if (printDialog.ShowDialog() == true)
{
    //When Print button clicked, document printed and window closed
    MessageBox.Show("Document printed.\nIt need " + printDialog.TotalSheets + " sheet(s) of paper.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
}
else
{
    //When Cancel button clicked and window closed
    MessageBox.Show("Print job canceled.", "PrintDialog", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
}
```

It will look like this. _(Only difference is the document.)_

![The PrintDialog Example Image](https://repository-images.githubusercontent.com/237794840/431f8000-660c-11ea-9936-f4ef89565bca)

For full example project see [PrintDialog 1.3.7 Example Project](https://github.com/Jet20070731/PrintDialog/tree/1.3.7.0/PrintDialogExample).

### Others

There are also some other helpers in the [DLL file](https://github.com/Jet20070731/PrintDialog/blob/1.3.7.0/PrintDialog.dll), such as **PaperHelper**(Get the actual size of all specified paper sizes), **PrinterHelper**(Get default printer, printer list, printer status), **NameInfoHelper**(Get the name info of enum members for some print settings).

## License

This project is under the [MIT License](https://github.com/Jet20070731/PrintDialog/blob/master/LICENSE.txt).
