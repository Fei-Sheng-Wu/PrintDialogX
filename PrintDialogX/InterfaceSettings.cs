namespace PrintDialogX
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InterfaceSettings"/> class.
    /// </summary>
    public class InterfaceSettings()
    {
        /// <summary>
        /// Specifies the interface control of a specific print setting.
        /// </summary>
        public enum Options
        {
            /// <summary>
            /// An empty gap with a height of 20 pixels.
            /// </summary>
            Void = -1,

            /// <summary>
            /// An option to select a printer.
            /// </summary>
            Printer,

            /// <summary>
            /// An option to open the printer preferences dialog.
            /// </summary>
            PrinterPreferences,

            /// <summary>
            /// An option to select the number of copies.
            /// </summary>
            Copies,

            /// <summary>
            /// An option to select a collation choice.
            /// </summary>
            Collation,

            /// <summary>
            /// An option to select the pages to be printed.
            /// </summary>
            Pages,

            /// <summary>
            /// An option to select a layout.
            /// </summary>
            Layout,

            /// <summary>
            /// An option to select a size.
            /// </summary>
            Size,

            /// <summary>
            /// An option to select a color.
            /// </summary>
            Color,

            /// <summary>
            /// An option to select a quality.
            /// </summary>
            Quality,

            /// <summary>
            /// An option to select the number of pages per sheet.
            /// </summary>
            PagesPerSheet,

            /// <summary>
            /// An option to select a page order.
            /// </summary>
            PageOrder,

            /// <summary>
            /// An option to select a scale.
            /// </summary>
            Scale,

            /// <summary>
            /// An option to select a margin.
            /// </summary>
            Margin,

            /// <summary>
            /// An option to select a double-sided choice.
            /// </summary>
            DoubleSided,

            /// <summary>
            /// An option to select a type.
            /// </summary>
            Type,

            /// <summary>
            /// An option to select a source.
            /// </summary>
            Source
        }

        /// <summary>
        /// Gets or sets the title of the interface.
        /// </summary>
        public string Title { get; set; } = (string)PrintDialogViewModel.StringResources["StringResource_TitlePrint"];

        /// <summary>
        /// Gets or sets the icon of the interface.
        /// </summary>
        public Wpf.Ui.Controls.IconElement? Icon { get; set; } = new Wpf.Ui.Controls.SymbolIcon()
        {
            Symbol = Wpf.Ui.Controls.SymbolRegular.Print20,
            FontSize = 18
        };

        /// <summary>
        /// Gets or sets the collection of basic interface controls to be placed outside the print settings expander.
        /// </summary>
        public Options[] BasicSettings { get; set; } = [Options.Printer, Options.PrinterPreferences, Options.Void, Options.Copies, Options.Collation, Options.Pages, Options.Layout, Options.Size];

        /// <summary>
        /// Gets or sets the collection of advanced interface controls to be placed inside the print settings expander.
        /// </summary>
        public Options[] AdvancedSettings { get; set; } = [Options.Color, Options.Quality, Options.PagesPerSheet, Options.PageOrder, Options.Scale, Options.Margin, Options.DoubleSided, Options.Type, Options.Source];
    }
}
