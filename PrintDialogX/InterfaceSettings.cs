﻿using System;

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
        public enum Option
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
        /// Specifies the interface language.
        /// </summary>
        public enum Language
        {
            /// <summary>
            /// English (United States).
            /// </summary>
            en_US
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
        public Option[] BasicSettings { get; set; } = [Option.Printer, Option.PrinterPreferences, Option.Void, Option.Copies, Option.Collation, Option.Pages, Option.Layout, Option.Size];

        /// <summary>
        /// Gets or sets the collection of advanced interface controls to be placed inside the print settings expander.
        /// </summary>
        public Option[] AdvancedSettings { get; set; } = [Option.Color, Option.Quality, Option.PagesPerSheet, Option.PageOrder, Option.Scale, Option.Margin, Option.DoubleSided, Option.Type, Option.Source];

        /// <summary>
        /// Gets or sets the display language of the interface.
        /// </summary>
        public Language DisplayLanguage
        {
            get => language;
            set
            {
                language = value;
                PrintDialogViewModel.StringResources = new()
                {
                    Source = new($"/PrintDialogX;component/Resources/Languages/{language switch
                    {
                        _ => "en-US"
                    }}.xaml", UriKind.Relative)
                };
            }
        }
        private Language language = Language.en_US;
    }
}
