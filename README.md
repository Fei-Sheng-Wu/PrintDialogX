# PrintDialogX v3.1.0-dev

[![C#](https://img.shields.io/badge/C%23-100%25-blue.svg?style=flat-square)](#)
[![Platform](https://img.shields.io/badge/Platform-WPF-green.svg?style=flat-square)](#)
[![Target .Net](https://img.shields.io/badge/.Net-%E2%89%A56.0-green.svg?style=flat-square)](#)
[![Target .Net Framework](https://img.shields.io/badge/.Net%20Framework-%E2%89%A54.7.2-green.svg?style=flat-square)](#)
[![Nuget](https://img.shields.io/nuget/v/PrintDialogX?label=Nuget&style=flat-square&logo=nuget)](https://www.nuget.org/packages/PrintDialogX)
[![Lincense](https://img.shields.io/github/license/Fei-Sheng-Wu/PrintDialogX?label=License&style=flat-square)](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/LICENSE.txt)

> A custom WPF print dialog with lightning-fast real-time preview. Support a full scope of print settings for modern demands, with the flexibility for complete customization. Provide the ability to dynamically adjust documents according to changes in print settings. Empowers the user experience with a responsive, elegant, and configurable interface.

## Preview

![Preview](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/preview.png)

## Features

PrintDialogX is a powerful and user-friendly print dialog tailored for modern demands. It supports all essential and advanced features expected from a next-generation print dialog, delivering real-time document previews at lightning speed. Without relying on the built-in controls for document hosting and printing, this next-level print dialog is able to truly take full control over the print pipeline, enabling more thorough customizations and better performance.

The print settings responsively adapt to the capabilities of specific printers, adhering to industry standards in addition to intelligently maintaining the user's preferences. With the ability to respond to any changes in print settings made by the user, documents remain flexible and dynamically reactive to these adjustments. Powered by [Wpf.Ui](https://wpfui.lepo.co), the compelling interface allows complete personalization to suit specific scenarios, while its carefully crafted structure minimizes lag and ensures a fluid, modern printing experience.

- [x] Comprehensive printer selection
  - [x] Detailed printer information with graphics
  - [x] Options to add new printers or configure existing printers
- [x] Personalizable print settings
  - [x] Full range of configurations for modern print dialogs
  - [x] Modifiable settings organization for personal needs
- [x] Interactive real-time preview
  - [x] Responsive high-resolution zooming
  - [x] Customizable document arrangement and navigation
- [x] Dynamically updatable documents
  - [x] Handler for print setting changes to adjust the contents on the fly

## Dependencies

- Wpf.Ui ≥ 4.0.0

## How to Use

An example project is included under the [PrintDialogX.Test](https://github.com/Fei-Sheng-Wu/PrintDialogX/tree/master/PrintDialogX.Test) folder, with custom configurations to generate the print dialog accordingly, and three template documents to showcase the capability of PrintDialogX.

### Quick Start

The usage of PrintDialogX is straightforward:

```c#
// Create a new document
PrintDialogX.PrintDocument document = new PrintDialogX.PrintDocument();

// Create the pages of the document
for (int i = 0; i < 100; i++)
{
    PrintDialogX.PrintPage page = new PrintDialogX.PrintPage();
    page.Content = GenerateContent(i);
    document.Pages.Add(page);
}

// Initialize the print dialog
PrintDialogX.PrintDialog dialog = new PrintDialogX.PrintDialog();
dialog.Document = document;

// Open the print dialog
dialog.ShowDialog();

// Retrieve the result of the operation
bool isSuccess = dialog.Result.IsSuccess;
int paperCount = dialog.Result.PaperCount;
```

### Asynchronous Document Generation

PrintDialogX supports the ability to delay the document generation until the dialog is loaded, so that a spinner is shown during the generation:

```c#
// Initialize the print dialog
PrintDialogX.PrintDialog dialog = new PrintDialogX.PrintDialog();

// Open the print dialog
dialog.ShowDialog(async () =>
{
    // Create a new document
    PrintDialogX.PrintDocument document = new PrintDialogX.PrintDocument();

    // Create the pages of the document asynchronously
    for (int i = 0; i < 100; i++)
    {
        PrintDialogX.PrintPage page = new PrintDialogX.PrintPage();
        page.Content = await GenerateContentAsync(i);
        document.Pages.Add(page);

        // Allow for other UI updates
        await Dispatcher.Yield();
    }
    dialog.Document = document;
});
```

### Document Configuration

It is easy to customize the document information (by default, the document is dynamically sized and takes up the entirety of the available space, but one may also choose to fix the size of the document):

```c#
document.DocumentName = "Untitled Document";
document.DocumentSize = new PrintDialogX.Enums.Size(PrintDialogX.Enums.Size.DefinedSize.NorthAmericaLetter);
document.DocumentMargin = 50.0;
```

### Dynamically Updatable Documents

PrintDialogX raises an event when the print settings are changed:

```c#
document.PrintSettingsChanged += HandlePrintSettingsChanged;
```
```c#
private async void HandlePrintSettingsChanged(object? sender, PrintDialogX.PrintSettingsEventArgs e)
{
    if (sender is not PrintDialogX.PrintDocument document)
    {
        return;
    }

    // Block the preview generation until the document is updated, due to the use of await
    e.IsBlocking = true;

    int index = 0;
    foreach (PrintDialogX.PrintPage page in document.Pages)
    {
        // Update the content according to the print settings
        page.Content = await UpdateContentAsync(index, e.CurrentSettings);
        index++;

        // Allow for other UI updates
        await Dispatcher.Yield();
    }

    e.IsBlocking = false;
}
```

### Print Settings Customizations

The default settings can be set individually (certain settings accept `null` and use `null` by default, which represents that the default configuration of the selected printer will be used):

```c#
dialog.PrintSettings.Copies = 2;
dialog.PrintSettings.Collation = PrintDialogX.Enums.Collation.Collated;
dialog.PrintSettings.Pages = PrintDialogX.Enums.Pages.CustomPages;
dialog.PrintSettings.CustomPages = "2-5, 8";
dialog.PrintSettings.Layout = PrintDialogX.Enums.Layout.Landscape;
dialog.PrintSettings.Color = PrintDialogX.Enums.Color.Grayscale;
```

### Interface Customizations

PrintDialogX offers the ability to both customize the window of the print dialog and the exact interface to be used within the print dialog:

```c#
// Initialize the print dialog
PrintDialogX.PrintDialog dialog = new PrintDialogX.PrintDialog(window =>
{
    // Customize the dialog window
    window.Topmost = true;
    window.ShowInTaskbar = false;
});

// Customize the interface
dialog.InterfaceSettings.Title = "Test Print";
dialog.InterfaceSettings.BasicSettings = [PrintDialogX.InterfaceSettings.Option.Printer, PrintDialogX.InterfaceSettings.Option.Void, PrintDialogX.InterfaceSettings.Option.Pages, PrintDialogX.InterfaceSettings.Option.Layout, PrintDialogX.InterfaceSettings.Option.Size];
dialog.InterfaceSettings.AdvancedSettings = [PrintDialogX.InterfaceSettings.Option.Color, PrintDialogX.InterfaceSettings.Option.Quality, PrintDialogX.InterfaceSettings.Option.Scale, PrintDialogX.InterfaceSettings.Option.Margin, PrintDialogX.InterfaceSettings.Option.DoubleSided, PrintDialogX.InterfaceSettings.Option.Type, PrintDialogX.InterfaceSettings.Option.Source];
```

## License

This project is under the [MIT License](https://github.com/Fei-Sheng-Wu/PrintDialogX/blob/master/LICENSE.txt).
