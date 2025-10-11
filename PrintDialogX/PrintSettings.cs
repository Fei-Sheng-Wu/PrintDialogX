using System;
using System.Printing;
using System.ComponentModel;

namespace PrintDialogX
{
    /// <summary>
    /// Defines the print settings that can be used by a <see cref="PrintDialog"/> instance.
    /// </summary>
    public class PrintSettings
    {
        //TODO: documentation
        public class FallbackSettings
        {
            public int FallbackMaximumCopies
            {
                get => copiesMaximum;
                set => copiesMaximum = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(FallbackMaximumCopies), "The value must be positive.");
            }
            private int copiesMaximum = int.MaxValue;
            public bool FallbackIsCollationSupported { get; set; } = true;

            //TODO: create from defined name alone
            public Enums.Size FallbackSize { get; set; } = new()
            {
                DefinedName = Enums.Size.DefinedSize.ISOA4,
                Width = 8.27 * 96,
                Height = 11.69 * 96
            };
            public Enums.Color FallbackColor { get; set; } = Enums.Color.Color;
            public Enums.Quality FallbackQuality { get; set; } = Enums.Quality.Automatic;
            public bool FallbackIsDoubleSidedSupported { get; set; } = true;
            public Enums.Type FallbackType { get; set; } = Enums.Type.AutoSelect;
            public Enums.Source FallbackSource { get; set; } = Enums.Source.AutoSelect;
        }

        public FallbackSettings Fallbacks { get; set; } = new();

        public int Copies
        {
            get => copies;
            set => copies = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(Copies), "The value must be positive.");
        }
        private int copies = 1;

        public Enums.Collation Collation { get; set; } = Enums.Collation.Collated;

        public Enums.Pages Pages { get; set; } = Enums.Pages.AllPages;
        public string CustomPages
        {
            get => pagesCustom;
            set => pagesCustom = CustomPagesValidationRule.TryConvert(value, int.MaxValue).IsValid ? value : throw new ArgumentOutOfRangeException(nameof(CustomPages), "The value is invalid.");
        }
        private string pagesCustom = string.Empty;

        /// <summary>
        /// Gets or sets the value indicating how the page content is oriented for printing.
        /// </summary>
        public Enums.Layout Layout { get; set; } = Enums.Layout.Portrait;

        /// <summary>
        /// Gets or sets the page size for the paper or other print media that the printer uses for printing.
        /// </summary>
        public Enums.Size? Size { get; set; } = null;

        /// <summary>
        /// Gets or sets the value indicating how the printer handles content that has color or shades of gray.
        /// </summary>
        public Enums.Color? Color { get; set; } = null;

        /// <summary>
        /// Gets or sets the value indicating the quality of output for printing.
        /// </summary>
        public Enums.Quality? Quality { get; set; } = null;

        /// <summary>
        /// Gets or sets the number of pages that print on each printed side of a sheet of paper.
        /// </summary>
        public Enums.PagesPerSheet PagesPerSheet { get; set; } = Enums.PagesPerSheet.One;

        /// <summary>
        /// Gets or sets the value indicating how a printer arranges multiple pages that print on each side of a sheet of paper.
        /// </summary>
        public Enums.PageOrder PageOrder { get; set; } = Enums.PageOrder.Horizontal;

        public Enums.Scale Scale { get; set; } = Enums.Scale.AutoFit;
        public int CustomScale
        {
            get => scaleCustom;
            set => scaleCustom = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(CustomScale), "The value cannot be negative.");
        }
        private int scaleCustom = 100;

        public Enums.Margin Margin { get; set; } = Enums.Margin.Default;
        public int CustomMargin
        {
            get => marginCustom;
            set => marginCustom = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(CustomMargin), "The value cannot be negative.");
        }
        private int marginCustom = 0;

        /// <summary>
        /// Gets or sets the value indicating what kind of two-sided printing, if any, that the printer uses for printing.
        /// </summary>
        public Enums.DoubleSided DoubleSided { get; set; } = Enums.DoubleSided.OneSided;

        /// <summary>
        /// Gets or sets the value indicating the type of printing paper or other media that the printer uses for printing.
        /// </summary>
        public Enums.Type? Type { get; set; } = null;

        public Enums.Source? Source { get; set; } = null;
    }
}

namespace PrintDialogX.Enums
{
    /// <summary>
    /// Specifies whether a printer collates output when it prints multiple copies of a multi-page print job.
    /// </summary>
    public enum Collation
    {
        /// <summary>
        /// Collated output.
        /// </summary>
        [Description("StringResource_EntryCollated")]
        Collated,

        /// <summary>
        /// Uncollated output.
        /// </summary>
        [Description("StringResource_EntryUncollated")]
        Uncollated
    }

    /// <summary>
    /// Specifies what pages to include for printing.
    /// </summary>
    public enum Pages
    {
        /// <summary>
        /// All pages.
        /// </summary>
        [Description("StringResource_EntryAllPages")]
        AllPages,

        /// <summary>
        /// Current page.
        /// </summary>
        [Description("StringResource_EntryCurrentPage")]
        CurrentPage,

        /// <summary>
        /// Custom pages.
        /// </summary>
        [Description("StringResource_EntryCustomPages")]
        CustomPages
    }

    /// <summary>
    /// Specifies how pages of content are oriented on print media.
    /// </summary>
    public enum Layout
    {
        /// <summary>
        /// Standard orientation.
        /// </summary>
        [Description("StringResource_EntryPortrait")]
        Portrait,

        /// <summary>
        /// Content of the imageable area is rotated on the page 90 degrees counterclockwise from standard (portrait) orientation.
        /// </summary>
        [Description("StringResource_EntryLandscape")]
        Landscape
    }

    /// <summary>
    /// Specifies the page size or roll width of the paper or other print media.
    /// </summary>
    public struct Size
    {
        public enum DefinedSize
        {
            /// <summary>
            /// A0.
            /// </summary>
            [Description("StringResource_EntryISOA0")]
            ISOA0,

            /// <summary>
            /// A1.
            /// </summary>
            [Description("StringResource_EntryISOA1")]
            ISOA1,

            /// <summary>
            /// A10.
            /// </summary>
            [Description("StringResource_EntryISOA10")]
            ISOA10,

            /// <summary>
            /// A2.
            /// </summary>
            [Description("StringResource_EntryISOA2")]
            ISOA2,

            /// <summary>
            /// A3.
            /// </summary>
            [Description("StringResource_EntryISOA3")]
            ISOA3,

            /// <summary>
            /// A3 Rotated.
            /// </summary>
            [Description("StringResource_EntryISOA3Rotated")]
            ISOA3Rotated,

            /// <summary>
            /// A3 Extra.
            /// </summary>
            [Description("StringResource_EntryISOA3Extra")]
            ISOA3Extra,

            /// <summary>
            /// A4.
            /// </summary>
            [Description("StringResource_EntryISOA4")]
            ISOA4,

            /// <summary>
            /// A4 Rotated.
            /// </summary>
            [Description("StringResource_EntryISOA4Rotated")]
            ISOA4Rotated,

            /// <summary>
            /// A4 Extra.
            /// </summary>
            [Description("StringResource_EntryISOA4Extra")]
            ISOA4Extra,

            /// <summary>
            /// A5.
            /// </summary>
            [Description("StringResource_EntryISOA5")]
            ISOA5,

            /// <summary>
            /// A5 Rotated.
            /// </summary>
            [Description("StringResource_EntryISOA5Rotated")]
            ISOA5Rotated,

            /// <summary>
            /// A5 Extra.
            /// </summary>
            [Description("StringResource_EntryISOA5Extra")]
            ISOA5Extra,

            /// <summary>
            /// A6.
            /// </summary>
            [Description("StringResource_EntryISOA6")]
            ISOA6,

            /// <summary>
            /// A6 Rotated.
            /// </summary>
            [Description("StringResource_EntryISOA6Rotated")]
            ISOA6Rotated,

            /// <summary>
            /// A7.
            /// </summary>
            [Description("StringResource_EntryISOA7")]
            ISOA7,

            /// <summary>
            /// A8.
            /// </summary>
            [Description("StringResource_EntryISOA8")]
            ISOA8,

            /// <summary>
            /// A9.
            /// </summary>
            [Description("StringResource_EntryISOA9")]
            ISOA9,

            /// <summary>
            /// B0.
            /// </summary>
            [Description("StringResource_EntryISOB0")]
            ISOB0,

            /// <summary>
            /// B1.
            /// </summary>
            [Description("StringResource_EntryISOB1")]
            ISOB1,

            /// <summary>
            /// B10.
            /// </summary>
            [Description("StringResource_EntryISOB10")]
            ISOB10,

            /// <summary>
            /// B2.
            /// </summary>
            [Description("StringResource_EntryISOB2")]
            ISOB2,

            /// <summary>
            /// B3.
            /// </summary>
            [Description("StringResource_EntryISOB3")]
            ISOB3,

            /// <summary>
            /// B4.
            /// </summary>
            [Description("StringResource_EntryISOB4")]
            ISOB4,

            /// <summary>
            /// B4 Envelope.
            /// </summary>
            [Description("StringResource_EntryISOB4Envelope")]
            ISOB4Envelope,

            /// <summary>
            /// B5 Envelope.
            /// </summary>
            [Description("StringResource_EntryISOB5Envelope")]
            ISOB5Envelope,

            /// <summary>
            /// B5 Extra.
            /// </summary>
            [Description("StringResource_EntryISOB5Extra")]
            ISOB5Extra,

            /// <summary>
            /// B7.
            /// </summary>
            [Description("StringResource_EntryISOB7")]
            ISOB7,

            /// <summary>
            /// B8.
            /// </summary>
            [Description("StringResource_EntryISOB8")]
            ISOB8,

            /// <summary>
            /// B9.
            /// </summary>
            [Description("StringResource_EntryISOB9")]
            ISOB9,

            /// <summary>
            /// C0.
            /// </summary>
            [Description("StringResource_EntryISOC0")]
            ISOC0,

            /// <summary>
            /// C1.
            /// </summary>
            [Description("StringResource_EntryISOC1")]
            ISOC1,

            /// <summary>
            /// C10.
            /// </summary>
            [Description("StringResource_EntryISOC10")]
            ISOC10,

            /// <summary>
            /// C2.
            /// </summary>
            [Description("StringResource_EntryISOC2")]
            ISOC2,

            /// <summary>
            /// C3.
            /// </summary>
            [Description("StringResource_EntryISOC3")]
            ISOC3,

            /// <summary>
            /// C3 Envelope.
            /// </summary>
            [Description("StringResource_EntryISOC3Envelope")]
            ISOC3Envelope,

            /// <summary>
            /// C4.
            /// </summary>
            [Description("StringResource_EntryISOC4")]
            ISOC4,

            /// <summary>
            /// C4 Envelope.
            /// </summary>
            [Description("StringResource_EntryISOC4Envelope")]
            ISOC4Envelope,

            /// <summary>
            /// C5.
            /// </summary>
            [Description("StringResource_EntryISOC5")]
            ISOC5,

            /// <summary>
            /// C5 Envelope.
            /// </summary>
            [Description("StringResource_EntryISOC5Envelope")]
            ISOC5Envelope,

            /// <summary>
            /// C6.
            /// </summary>
            [Description("StringResource_EntryISOC6")]
            ISOC6,

            /// <summary>
            /// C6 Envelope.
            /// </summary>
            [Description("StringResource_EntryISOC6Envelope")]
            ISOC6Envelope,

            /// <summary>
            /// C6C5 Envelope.
            /// </summary>
            [Description("StringResource_EntryISOC6C5Envelope")]
            ISOC6C5Envelope,

            /// <summary>
            /// C7.
            /// </summary>
            [Description("StringResource_EntryISOC7")]
            ISOC7,

            /// <summary>
            /// C8.
            /// </summary>
            [Description("StringResource_EntryISOC8")]
            ISOC8,

            /// <summary>
            /// C9.
            /// </summary>
            [Description("StringResource_EntryISOC9")]
            ISOC9,

            /// <summary>
            /// DL Envelope.
            /// </summary>
            [Description("StringResource_EntryISODLEnvelope")]
            ISODLEnvelope,

            /// <summary>
            /// DL Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryISODLEnvelopeRotated")]
            ISODLEnvelopeRotated,

            /// <summary>
            /// SRA 3.
            /// </summary>
            [Description("StringResource_EntryISOSRA3")]
            ISOSRA3,

            /// <summary>
            /// Quadruple Hagaki Postcard.
            /// </summary>
            [Description("StringResource_EntryJapanQuadrupleHagakiPostcard")]
            JapanQuadrupleHagakiPostcard,

            /// <summary>
            /// Japanese Industrial Standard B0.
            /// </summary>
            [Description("StringResource_EntryJISB0")]
            JISB0,

            /// <summary>
            /// Japanese Industrial Standard B1.
            /// </summary>
            [Description("StringResource_EntryJISB1")]
            JISB1,

            /// <summary>
            /// Japanese Industrial Standard B10.
            /// </summary>
            [Description("StringResource_EntryJISB10")]
            JISB10,

            /// <summary>
            /// Japanese Industrial Standard B2.
            /// </summary>
            [Description("StringResource_EntryJISB2")]
            JISB2,

            /// <summary>
            /// Japanese Industrial Standard B3.
            /// </summary>
            [Description("StringResource_EntryJISB3")]
            JISB3,

            /// <summary>
            /// Japanese Industrial Standard B4.
            /// </summary>
            [Description("StringResource_EntryJISB4")]
            JISB4,

            /// <summary>
            /// Japanese Industrial Standard B4 Rotated.
            /// </summary>
            [Description("StringResource_EntryJISB4Rotated")]
            JISB4Rotated,

            /// <summary>
            /// Japanese Industrial Standard B5.
            /// </summary>
            [Description("StringResource_EntryJISB5")]
            JISB5,

            /// <summary>
            /// Japanese Industrial Standard B5 Rotated.
            /// </summary>
            [Description("StringResource_EntryJISB5Rotated")]
            JISB5Rotated,

            /// <summary>
            /// Japanese Industrial Standard B6.
            /// </summary>
            [Description("StringResource_EntryJISB6")]
            JISB6,

            /// <summary>
            /// Japanese Industrial Standard B6 Rotated.
            /// </summary>
            [Description("StringResource_EntryJISB6Rotated")]
            JISB6Rotated,

            /// <summary>
            /// Japanese Industrial Standard B7.
            /// </summary>
            [Description("StringResource_EntryJISB7")]
            JISB7,

            /// <summary>
            /// Japanese Industrial Standard B8.
            /// </summary>
            [Description("StringResource_EntryJISB8")]
            JISB8,

            /// <summary>
            /// Japanese Industrial Standard B9.
            /// </summary>
            [Description("StringResource_EntryJISB9")]
            JISB9,

            /// <summary>
            /// Chou 3 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanChou3Envelope")]
            JapanChou3Envelope,

            /// <summary>
            /// Chou 3 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryJapanChou3EnvelopeRotated")]
            JapanChou3EnvelopeRotated,

            /// <summary>
            /// Chou 4 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanChou4Envelope")]
            JapanChou4Envelope,

            /// <summary>
            /// Chou 4 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryJapanChou4EnvelopeRotated")]
            JapanChou4EnvelopeRotated,

            /// <summary>
            /// Hagaki Postcard.
            /// </summary>
            [Description("StringResource_EntryJapanHagakiPostcard")]
            JapanHagakiPostcard,

            /// <summary>
            /// Hagaki Postcard Rotated.
            /// </summary>
            [Description("StringResource_EntryJapanHagakiPostcardRotated")]
            JapanHagakiPostcardRotated,

            /// <summary>
            /// Kaku 2 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanKaku2Envelope")]
            JapanKaku2Envelope,

            /// <summary>
            /// Kaku 2 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryJapanKaku2EnvelopeRotated")]
            JapanKaku2EnvelopeRotated,

            /// <summary>
            /// Kaku 3 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanKaku3Envelope")]
            JapanKaku3Envelope,

            /// <summary>
            /// Kaku 3 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryJapanKaku3EnvelopeRotated")]
            JapanKaku3EnvelopeRotated,

            /// <summary>
            /// You 4 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanYou4Envelope")]
            JapanYou4Envelope,

            /// <summary>
            /// 10 x 11.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica10x11")]
            NorthAmerica10x11,

            /// <summary>
            /// 10 x 14.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica10x14")]
            NorthAmerica10x14,

            /// <summary>
            /// 11 x 17.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica11x17")]
            NorthAmerica11x17,

            /// <summary>
            /// 9 x 11.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica9x11")]
            NorthAmerica9x11,

            /// <summary>
            /// Architecture A Sheet.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaArchitectureASheet")]
            NorthAmericaArchitectureASheet,

            /// <summary>
            /// Architecture B Sheet.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaArchitectureBSheet")]
            NorthAmericaArchitectureBSheet,

            /// <summary>
            /// Architecture C Sheet.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaArchitectureCSheet")]
            NorthAmericaArchitectureCSheet,

            /// <summary>
            /// Architecture D Sheet.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaArchitectureDSheet")]
            NorthAmericaArchitectureDSheet,

            /// <summary>
            /// Architecture E Sheet.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaArchitectureESheet")]
            NorthAmericaArchitectureESheet,

            /// <summary>
            /// C Sheet.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaCSheet")]
            NorthAmericaCSheet,

            /// <summary>
            /// D Sheet.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaDSheet")]
            NorthAmericaDSheet,

            /// <summary>
            /// E Sheet.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaESheet")]
            NorthAmericaESheet,

            /// <summary>
            /// Executive.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaExecutive")]
            NorthAmericaExecutive,

            /// <summary>
            /// German Legal Fanfold.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaGermanLegalFanfold")]
            NorthAmericaGermanLegalFanfold,

            /// <summary>
            /// German Standard Fanfold.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaGermanStandardFanfold")]
            NorthAmericaGermanStandardFanfold,

            /// <summary>
            /// Legal.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaLegal")]
            NorthAmericaLegal,

            /// <summary>
            /// Legal Extra.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaLegalExtra")]
            NorthAmericaLegalExtra,

            /// <summary>
            /// Letter.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaLetter")]
            NorthAmericaLetter,

            /// <summary>
            /// Letter Rotated.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaLetterRotated")]
            NorthAmericaLetterRotated,

            /// <summary>
            /// Letter Extra.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaLetterExtra")]
            NorthAmericaLetterExtra,

            /// <summary>
            /// Letter Plus.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaLetterPlus")]
            NorthAmericaLetterPlus,

            /// <summary>
            /// Monarch Envelope.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaMonarchEnvelope")]
            NorthAmericaMonarchEnvelope,

            /// <summary>
            /// Note.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaNote")]
            NorthAmericaNote,

            /// <summary>
            /// #10 Envelope.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaNumber10Envelope")]
            NorthAmericaNumber10Envelope,

            /// <summary>
            /// #10 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaNumber10EnvelopeRotated")]
            NorthAmericaNumber10EnvelopeRotated,

            /// <summary>
            /// #9 Envelope.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaNumber9Envelope")]
            NorthAmericaNumber9Envelope,

            /// <summary>
            /// #11 Envelope.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaNumber11Envelope")]
            NorthAmericaNumber11Envelope,

            /// <summary>
            /// #12 Envelope.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaNumber12Envelope")]
            NorthAmericaNumber12Envelope,

            /// <summary>
            /// #14 Envelope.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaNumber14Envelope")]
            NorthAmericaNumber14Envelope,

            /// <summary>
            /// Personal Envelope.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaPersonalEnvelope")]
            NorthAmericaPersonalEnvelope,

            /// <summary>
            /// Quarto.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaQuarto")]
            NorthAmericaQuarto,

            /// <summary>
            /// Statement.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaStatement")]
            NorthAmericaStatement,

            /// <summary>
            /// Super A.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaSuperA")]
            NorthAmericaSuperA,

            /// <summary>
            /// Super B.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaSuperB")]
            NorthAmericaSuperB,

            /// <summary>
            /// Tabloid.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaTabloid")]
            NorthAmericaTabloid,

            /// <summary>
            /// Tabloid Extra.
            /// </summary>
            [Description("StringResource_EntryNorthAmericaTabloidExtra")]
            NorthAmericaTabloidExtra,

            /// <summary>
            /// A4 Plus.
            /// </summary>
            [Description("StringResource_EntryOtherMetricA4Plus")]
            OtherMetricA4Plus,

            /// <summary>
            /// A3 Plus.
            /// </summary>
            [Description("StringResource_EntryOtherMetricA3Plus")]
            OtherMetricA3Plus,

            /// <summary>
            /// Folio.
            /// </summary>
            [Description("StringResource_EntryOtherMetricFolio")]
            OtherMetricFolio,

            /// <summary>
            /// Invite Envelope.
            /// </summary>
            [Description("StringResource_EntryOtherMetricInviteEnvelope")]
            OtherMetricInviteEnvelope,

            /// <summary>
            /// Italian Envelope.
            /// </summary>
            [Description("StringResource_EntryOtherMetricItalianEnvelope")]
            OtherMetricItalianEnvelope,

            /// <summary>
            /// People's Republic of China #1 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC1Envelope")]
            PRC1Envelope,

            /// <summary>
            /// People's Republic of China #1 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC1EnvelopeRotated")]
            PRC1EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #10 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC10Envelope")]
            PRC10Envelope,

            /// <summary>
            /// People's Republic of China #10 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC10EnvelopeRotated")]
            PRC10EnvelopeRotated,

            /// <summary>
            /// People's Republic of China 16K.
            /// </summary>
            [Description("StringResource_EntryPRC16K")]
            PRC16K,

            /// <summary>
            /// People's Republic of China 16K Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC16KRotated")]
            PRC16KRotated,

            /// <summary>
            /// People's Republic of China #2 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC2Envelope")]
            PRC2Envelope,

            /// <summary>
            /// People's Republic of China #2 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC2EnvelopeRotated")]
            PRC2EnvelopeRotated,

            /// <summary>
            /// People's Republic of China 32K.
            /// </summary>
            [Description("StringResource_EntryPRC32K")]
            PRC32K,

            /// <summary>
            /// People's Republic of China 32K Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC32KRotated")]
            PRC32KRotated,

            /// <summary>
            /// People's Republic of China 32K Big.
            /// </summary>
            [Description("StringResource_EntryPRC32KBig")]
            PRC32KBig,

            /// <summary>
            /// People's Republic of China #3 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC3Envelope")]
            PRC3Envelope,

            /// <summary>
            /// People's Republic of China #3 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC3EnvelopeRotated")]
            PRC3EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #4 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC4Envelope")]
            PRC4Envelope,

            /// <summary>
            /// People's Republic of China #4 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC4EnvelopeRotated")]
            PRC4EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #5 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC5Envelope")]
            PRC5Envelope,

            /// <summary>
            /// People's Republic of China #5 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC5EnvelopeRotated")]
            PRC5EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #6 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC6Envelope")]
            PRC6Envelope,

            /// <summary>
            /// People's Republic of China #6 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC6EnvelopeRotated")]
            PRC6EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #7 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC7Envelope")]
            PRC7Envelope,

            /// <summary>
            /// People's Republic of China #7 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC7EnvelopeRotated")]
            PRC7EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #8 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC8Envelope")]
            PRC8Envelope,

            /// <summary>
            /// People's Republic of China #8 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC8EnvelopeRotated")]
            PRC8EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #9 Envelope.
            /// </summary>
            [Description("StringResource_EntryPRC9Envelope")]
            PRC9Envelope,

            /// <summary>
            /// People's Republic of China #9 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryPRC9EnvelopeRotated")]
            PRC9EnvelopeRotated,

            /// <summary>
            /// 4-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll04Inch")]
            Roll04Inch,

            /// <summary>
            /// 6-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll06Inch")]
            Roll06Inch,

            /// <summary>
            /// 8-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll08Inch")]
            Roll08Inch,

            /// <summary>
            /// 12-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll12Inch")]
            Roll12Inch,

            /// <summary>
            /// 15-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll15Inch")]
            Roll15Inch,

            /// <summary>
            /// 18-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll18Inch")]
            Roll18Inch,

            /// <summary>
            /// 22-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll22Inch")]
            Roll22Inch,

            /// <summary>
            /// 24-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll24Inch")]
            Roll24Inch,

            /// <summary>
            /// 30-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll30Inch")]
            Roll30Inch,

            /// <summary>
            /// 36-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll36Inch")]
            Roll36Inch,

            /// <summary>
            /// 54-inch wide roll.
            /// </summary>
            [Description("StringResource_EntryRoll54Inch")]
            Roll54Inch,

            /// <summary>
            /// Double Hagaki Postcard.
            /// </summary>
            [Description("StringResource_EntryJapanDoubleHagakiPostcard")]
            JapanDoubleHagakiPostcard,

            /// <summary>
            /// Double Hagaki Postcard Rotated.
            /// </summary>
            [Description("StringResource_EntryJapanDoubleHagakiPostcardRotated")]
            JapanDoubleHagakiPostcardRotated,

            /// <summary>
            /// L Photo.
            /// </summary>
            [Description("StringResource_EntryJapanLPhoto")]
            JapanLPhoto,

            /// <summary>
            /// 2L Photo.
            /// </summary>
            [Description("StringResource_EntryJapan2LPhoto")]
            Japan2LPhoto,

            /// <summary>
            /// You 1 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanYou1Envelope")]
            JapanYou1Envelope,

            /// <summary>
            /// You 2 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanYou2Envelope")]
            JapanYou2Envelope,

            /// <summary>
            /// You 3 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanYou3Envelope")]
            JapanYou3Envelope,

            /// <summary>
            /// You 4 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryJapanYou4EnvelopeRotated")]
            JapanYou4EnvelopeRotated,

            /// <summary>
            /// You 6 Envelope.
            /// </summary>
            [Description("StringResource_EntryJapanYou6Envelope")]
            JapanYou6Envelope,

            /// <summary>
            /// You 6 Envelope Rotated.
            /// </summary>
            [Description("StringResource_EntryJapanYou6EnvelopeRotated")]
            JapanYou6EnvelopeRotated,

            /// <summary>
            /// 4 x 6.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica4x6")]
            NorthAmerica4x6,

            /// <summary>
            /// 4 x 8.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica4x8")]
            NorthAmerica4x8,

            /// <summary>
            /// 5 x 7.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica5x7")]
            NorthAmerica5x7,

            /// <summary>
            /// 8 x 10.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica8x10")]
            NorthAmerica8x10,

            /// <summary>
            /// 10 x 12.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica10x12")]
            NorthAmerica10x12,

            /// <summary>
            /// 14 x 17.
            /// </summary>
            [Description("StringResource_EntryNorthAmerica14x17")]
            NorthAmerica14x17,

            /// <summary>
            /// Business card.
            /// </summary>
            [Description("StringResource_EntryBusinessCard")]
            BusinessCard,

            /// <summary>
            /// Credit card.
            /// </summary>
            [Description("StringResource_EntryCreditCard")]
            CreditCard
        }

        public DefinedSize? DefinedName { get; set; }

        public string? FallbackName { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public readonly bool Equals(DefinedSize? name, double? width, double? height)
        {
            return (name != null && name == DefinedName) || (width != null && height != null && Math.Max(Math.Abs(width.Value - Width), Math.Abs(height.Value - Height)) < 0.5);
        }

        public readonly bool Equals(PageMediaSize? size)
        {
            return size != null && Equals(size.PageMediaSizeName != null ? ValueMappings.Map(size.PageMediaSizeName.Value, ValueMappings.SizeNameMapping) : null, size.Width, size.Height);
        }

        public override readonly bool Equals(object? value)
        {
            return value is Size size && Equals(size.DefinedName, size.Width, size.Height);
        }

        public override readonly int GetHashCode()
        {
            //TODO: implement
            return base.GetHashCode();
        }

        public static bool operator ==(Size? x, Size? y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Size? x, Size? y)
        {
            return !x.Equals(y);
        }
    }

    /// <summary>
    /// Specifies how to print content that contains color or shades of gray.
    /// </summary>
    public enum Color
    {
        /// <summary>
        /// Output that prints in color.
        /// </summary>
        [Description("StringResource_EntryColor")]
        Color,

        /// <summary>
        /// Output that prints in a grayscale.
        /// </summary>
        [Description("StringResource_EntryGrayscale")]
        Grayscale,

        /// <summary>
        /// Output that prints in a single color and with the same degree of intensity.
        /// </summary>
        [Description("StringResource_EntryMonochrome")]
        Monochrome
    }

    /// <summary>
    /// Specifies the type of output quality for a print device.
    /// </summary>
    public enum Quality
    {
        /// <summary>
        /// Automatically selects a quality type that is based on the contents of a print job.
        /// </summary>
        [Description("StringResource_EntryAutomatic")]
        Automatic,

        /// <summary>
        /// Draft quality.
        /// </summary>
        [Description("StringResource_EntryDraft")]
        Draft,

        /// <summary>
        /// Fax quality.
        /// </summary>
        [Description("StringResource_EntryFax")]
        Fax,

        /// <summary>
        /// Higher than normal quality.
        /// </summary>
        [Description("StringResource_EntryHigh")]
        High,

        /// <summary>
        /// Normal quality.
        /// </summary>
        [Description("StringResource_EntryNormal")]
        Normal,

        /// <summary>
        /// Photographic quality.
        /// </summary>
        [Description("StringResource_EntryPhotographic")]
        Photographic,

        /// <summary>
        /// Text quality.
        /// </summary>
        [Description("StringResource_EntryText")]
        Text
    }

    /// <summary>
    /// Specifies the number of pages that print on each printed side of a sheet of paper.
    /// </summary>
    public enum PagesPerSheet
    {
        /// <summary>
        /// 1 page per sheet.
        /// </summary>
        [Description("StringResource_EntryOne")]
        One,

        /// <summary>
        /// 2 pages per sheet.
        /// </summary>
        [Description("StringResource_EntryTwo")]
        Two,

        /// <summary>
        /// 4 pages per sheet.
        /// </summary>
        [Description("StringResource_EntryFour")]
        Four,

        /// <summary>
        /// 6 pages per sheet.
        /// </summary>
        [Description("StringResource_EntrySix")]
        Six,

        /// <summary>
        /// 9 pages per sheet.
        /// </summary>
        [Description("StringResource_EntryNine")]
        Nine,

        /// <summary>
        /// 16 pages per sheet.
        /// </summary>
        [Description("StringResource_EntrySixteen")]
        Sixteen
    }

    /// <summary>
    /// Specifies the arrangement of pages when more than one page of content appears on a single side of print media.
    /// </summary>
    public enum PageOrder
    {
        /// <summary>
        /// Pages appear in rows, from left to right and top to bottom, relative to the page orientation.
        /// </summary>
        [Description("StringResource_EntryHorizontal")]
        Horizontal,

        /// <summary>
        /// Pages appear in rows, from right to left and top to bottom, relative to the page orientation.
        /// </summary>
        [Description("StringResource_EntryHorizontalReverse")]
        HorizontalReverse,

        /// <summary>
        /// Pages appear in columns, from top to bottom and left to right, relative to the page orientation.
        /// </summary>
        [Description("StringResource_EntryVertical")]
        Vertical,

        /// <summary>
        /// Pages appear in columns, from bottom to top and left to right, relative to the page orientation.
        /// </summary>
        [Description("StringResource_EntryVerticalReverse")]
        VerticalReverse
    }

    public enum Scale
    {
        [Description("StringResource_EntryAutoFit")]
        AutoFit,

        [Description("StringResource_EntryPercent25")]
        Percent25,

        [Description("StringResource_EntryPercent50")]
        Percent50,

        [Description("StringResource_EntryPercent75")]
        Percent75,

        [Description("StringResource_EntryPercent100")]
        Percent100,

        [Description("StringResource_EntryPercent150")]
        Percent150,

        [Description("StringResource_EntryPercent200")]
        Percent200,

        [Description("StringResource_EntryCustom")]
        Custom
    }

    public enum Margin
    {
        [Description("StringResource_EntryDefault")]
        Default,

        [Description("StringResource_EntryNone")]
        None,

        [Description("StringResource_EntryMinimum")]
        Minimum,

        [Description("StringResource_EntryCustom")]
        Custom
    }

    /// <summary>
    /// Specifies whether a printer uses one-sided printing or some type of two-sided (duplex) printing.
    /// </summary>
    public enum DoubleSided
    {
        /// <summary>
        /// Output prints on only one side of each sheet.
        /// </summary>
        [Description("StringResource_EntryOneSided")]
        OneSided,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="System.Printing.PrintDocumentImageableArea.MediaSizeWidth"/>.
        /// </summary>
        [Description("StringResource_EntryDoubleSidedShortEdge")]
        DoubleSidedShortEdge,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="System.Printing.PrintDocumentImageableArea.MediaSizeHeight"/>.
        /// </summary>
        [Description("StringResource_EntryDoubleSidedLongEdge")]
        DoubleSidedLongEdge
    }

    /// <summary>
    /// Specifies the type of printing paper or other media.
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// The print device selects the media.
        /// </summary>
        [Description("StringResource_EntryAutoSelect")]
        AutoSelect,

        /// <summary>
        /// Archive-quality media.
        /// </summary>
        [Description("StringResource_EntryArchival")]
        Archival,

        /// <summary>
        /// Specialty back-printing film.
        /// </summary>
        [Description("StringResource_EntryBackPrintFilm")]
        BackPrintFilm,

        /// <summary>
        /// Standard bond media.
        /// </summary>
        [Description("StringResource_EntryBond")]
        Bond,

        /// <summary>
        /// Standard card stock.
        /// </summary>
        [Description("StringResource_EntryCardStock")]
        CardStock,

        /// <summary>
        /// Continuous-feed media.
        /// </summary>
        [Description("StringResource_EntryContinuous")]
        Continuous,

        /// <summary>
        /// Standard envelope.
        /// </summary>
        [Description("StringResource_EntryEnvelopePlain")]
        EnvelopePlain,

        /// <summary>
        /// Window envelope.
        /// </summary>
        [Description("StringResource_EntryEnvelopeWindow")]
        EnvelopeWindow,

        /// <summary>
        /// Fabric media.
        /// </summary>
        [Description("StringResource_EntryFabric")]
        Fabric,

        /// <summary>
        /// Specialty high-resolution media.
        /// </summary>
        [Description("StringResource_EntryHighResolution")]
        HighResolution,

        /// <summary>
        /// Label media.
        /// </summary>
        [Description("StringResource_EntryLabel")]
        Label,

        /// <summary>
        /// Attached multipart forms.
        /// </summary>
        [Description("StringResource_EntryMultiLayerForm")]
        MultiLayerForm,

        /// <summary>
        /// Individual multipart forms.
        /// </summary>
        [Description("StringResource_EntryMultiPartForm")]
        MultiPartForm,

        /// <summary>
        /// Standard photographic media.
        /// </summary>
        [Description("StringResource_EntryPhotographic")]
        Photographic,

        /// <summary>
        /// Film photographic media.
        /// </summary>
        [Description("StringResource_EntryPhotographicFilm")]
        PhotographicFilm,

        /// <summary>
        /// Glossy photographic media.
        /// </summary>
        [Description("StringResource_EntryPhotographicGlossy")]
        PhotographicGlossy,

        /// <summary>
        /// High-gloss photographic media.
        /// </summary>
        [Description("StringResource_EntryPhotographicHighGloss")]
        PhotographicHighGloss,

        /// <summary>
        /// Matte photographic media.
        /// </summary>
        [Description("StringResource_EntryPhotographicMatte")]
        PhotographicMatte,

        /// <summary>
        /// Satin photographic media.
        /// </summary>
        [Description("StringResource_EntryPhotographicSatin")]
        PhotographicSatin,

        /// <summary>
        /// Semi-gloss photographic media.
        /// </summary>
        [Description("StringResource_EntryPhotographicSemiGloss")]
        PhotographicSemiGloss,

        /// <summary>
        /// Plain paper.
        /// </summary>
        [Description("StringResource_EntryPlain")]
        Plain,

        /// <summary>
        /// Output to a display in continuous form.
        /// </summary>
        [Description("StringResource_EntryScreen")]
        Screen,

        /// <summary>
        /// Output to a display in paged form.
        /// </summary>
        [Description("StringResource_EntryScreenPaged")]
        ScreenPaged,

        /// <summary>
        /// Specialty stationary.
        /// </summary>
        [Description("StringResource_EntryStationery")]
        Stationery,

        /// <summary>
        /// Tab stock, not precut (single tabs).
        /// </summary>
        [Description("StringResource_EntryTabStockFull")]
        TabStockFull,

        /// <summary>
        /// Tab stock, precut (multiple tabs).
        /// </summary>
        [Description("StringResource_EntryTabStockPreCut")]
        TabStockPreCut,

        /// <summary>
        /// Transparent sheet.
        /// </summary>
        [Description("StringResource_EntryTransparency")]
        Transparency,

        /// <summary>
        /// Media that is used to transfer an image to a T-shirt.
        /// </summary>
        [Description("StringResource_EntryTShirtTransfer")]
        TShirtTransfer
    }

    /// <summary>
    /// Specifies the input bin that is used as the source of blank paper or other print media.
    /// </summary>
    public enum Source
    {
        /// <summary>
        /// The automatic selection of an input bin according to the page size and media output type.
        /// </summary>
        [Description("StringResource_EntryAutoSelect")]
        AutoSelect,

        /// <summary>
        /// A removable paper bin is used.
        /// </summary>
        [Description("StringResource_EntryCassette")]
        Cassette,

        /// <summary>
        /// A tractor feed (also called a pin feed) of continuous-feed paper is used.
        /// </summary>
        [Description("StringResource_EntryTractor")]
        Tractor,

        /// <summary>
        /// The automatic sheet feeder is used.
        /// </summary>
        [Description("StringResource_EntryAutoSheetFeeder")]
        AutoSheetFeeder,

        /// <summary>
        /// The manual input bin is used.
        /// </summary>
        [Description("StringResource_EntryManual")]
        Manual
    }
}
