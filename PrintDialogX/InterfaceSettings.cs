namespace PrintDialogX
{
    public class InterfaceSettings
    {
        public enum Options
        {
            Void = -1,
            Printer,
            PrinterPreferences,
            Copies,
            Collation,
            Pages,
            Layout,
            Size,
            Color,
            Quality,
            PagesPerSheet,
            PageOrder,
            Scale,
            Margin,
            DoubleSided,
            Type,
            Source
        }

        public string Title { get; set; } = (string)PrintDialogViewModel.StringResources["StringResource_TitlePrint"];

        public Wpf.Ui.Controls.IconElement? Icon { get; set; } = new Wpf.Ui.Controls.SymbolIcon()
        {
            Symbol = Wpf.Ui.Controls.SymbolRegular.Print20,
            FontSize = 18
        };

        public Options[] BasicSettings { get; set; } = [Options.Printer, Options.PrinterPreferences, Options.Void, Options.Copies, Options.Collation, Options.Pages, Options.Layout, Options.Size];

        public Options[] AdvancedSettings { get; set; } = [Options.Color, Options.Quality, Options.PagesPerSheet, Options.PageOrder, Options.Scale, Options.Margin, Options.DoubleSided, Options.Type, Options.Source];
    }
}
