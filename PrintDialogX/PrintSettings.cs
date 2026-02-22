using System;
using System.Printing;
using System.ComponentModel;

namespace PrintDialogX
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrintSettings"/> class.
    /// </summary>
    public class PrintSettings()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackSettings"/> class.
        /// </summary>
        public class FallbackSettings()
        {
            /// <summary>
            /// Gets or sets the fallback value of maximum copies.
            /// </summary>
            public int FallbackMaximumCopies
            {
                get;
                set => field = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(FallbackMaximumCopies), "The value must be positive.");
            } = int.MaxValue;

            /// <summary>
            /// Gets or sets the fallback value of whether collation is supported.
            /// </summary>
            public bool FallbackIsCollationSupported { get; set; } = true;

            /// <summary>
            /// Gets or sets the fallback value of size.
            /// </summary>
            public Enums.Size FallbackSize { get; set; } = new(Enums.Size.DefinedSize.ISOA4);

            /// <summary>
            /// Gets or sets the fallback value of color.
            /// </summary>
            public Enums.Color FallbackColor { get; set; } = Enums.Color.Color;

            /// <summary>
            /// Gets or sets the fallback value of quality.
            /// </summary>
            public Enums.Quality FallbackQuality { get; set; } = Enums.Quality.Automatic;

            /// <summary>
            /// Gets or sets the fallback value of whether double-sided is supported.
            /// </summary>
            public bool FallbackIsDoubleSidedSupported { get; set; } = true;

            /// <summary>
            /// Gets or sets the fallback value of type.
            /// </summary>
            public Enums.Type FallbackType { get; set; } = Enums.Type.AutoSelect;

            /// <summary>
            /// Gets or sets the fallback value of source.
            /// </summary>
            public Enums.Source FallbackSource { get; set; } = Enums.Source.AutoSelect;
        }

        /// <summary>
        /// Gets or sets the fallback settings.
        /// </summary>
        public FallbackSettings Fallbacks { get; set; } = new();

        /// <summary>
        /// Gets or sets the number of copies.
        /// </summary>
        public int Copies
        {
            get;
            set => field = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(Copies), "The value must be positive.");
        } = 1;

        /// <summary>
        /// Gets or sets the collation choice.
        /// </summary>
        public Enums.Collation Collation { get; set; } = Enums.Collation.Collated;

        /// <summary>
        /// Gets or sets the pages to be printed.
        /// </summary>
        public Enums.Pages Pages { get; set; } = Enums.Pages.AllPages;

        /// <summary>
        /// Gets or sets the custom pages to be printed, if <see cref="Pages"/> is set to <see cref="Enums.Pages.CustomPages"/>.
        /// </summary>
        public string CustomPages
        {
            get;
            set => field = PagesCustomValidationRule.TryConvert(value, int.MaxValue).IsValid ? value : throw new ArgumentOutOfRangeException(nameof(CustomPages), "The value is invalid.");
        } = string.Empty;

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        public Enums.Layout Layout { get; set; } = Enums.Layout.Portrait;

        /// <summary>
        /// Gets or sets the size. If set to <see langword="null"/>, the default setting of the printer is used.
        /// </summary>
        public Enums.Size? Size { get; set; } = null;

        /// <summary>
        /// Gets or sets the color. If set to <see langword="null"/>, the default setting of the printer is used.
        /// </summary>
        public Enums.Color? Color { get; set; } = null;

        /// <summary>
        /// Gets or sets the quality. If set to <see langword="null"/>, the default setting of the printer is used.
        /// </summary>
        public Enums.Quality? Quality { get; set; } = null;

        /// <summary>
        /// Gets or sets the number of pages per sheet.
        /// </summary>
        public Enums.PagesPerSheet PagesPerSheet { get; set; } = Enums.PagesPerSheet.One;

        /// <summary>
        /// Gets or sets the page order.
        /// </summary>
        public Enums.PageOrder PageOrder { get; set; } = Enums.PageOrder.Horizontal;

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public Enums.Scale Scale { get; set; } = Enums.Scale.AutoFit;

        /// <summary>
        /// Gets or sets the custom scale, if <see cref="Scale"/> is set to <see cref="Enums.Scale.Custom"/>.
        /// </summary>
        public int CustomScale
        {
            get;
            set => field = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(CustomScale), "The value cannot be negative.");
        } = 100;

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        public Enums.Margin Margin { get; set; } = Enums.Margin.Default;

        /// <summary>
        /// Gets or sets the custom margin, if <see cref="Margin"/> is set to <see cref="Enums.Margin.Custom"/>.
        /// </summary>
        public int CustomMargin
        {
            get;
            set => field = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(CustomMargin), "The value cannot be negative.");
        } = 0;

        /// <summary>
        /// Gets or sets the double-sided choice.
        /// </summary>
        public Enums.DoubleSided DoubleSided { get; set; } = Enums.DoubleSided.OneSided;

        /// <summary>
        /// Gets or sets the type. If set to <see langword="null"/>, the default setting of the printer is used.
        /// </summary>
        public Enums.Type? Type { get; set; } = null;

        /// <summary>
        /// Gets or sets the source. If set to <see langword="null"/>, the default setting of the printer is used.
        /// </summary>
        public Enums.Source? Source { get; set; } = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintSettingsEventArgs"/> class.
    /// </summary>
    /// <param name="printer">The current printer.</param>
    /// <param name="settings">The current print settings.</param>
    /// <param name="isUpdating">Whether the document preview should be updated.</param>
    public class PrintSettingsEventArgs(PrintQueue printer, PrintSettings settings, bool isUpdating) : EventArgs
    {
        /// <summary>
        /// Gets or sets the current printer.
        /// </summary>
        public PrintQueue CurrentPrinter { get; set; } = printer;

        /// <summary>
        /// Gets or sets the current print settings.
        /// </summary>
        public PrintSettings CurrentSettings { get; set; } = settings;

        /// <summary>
        /// Gets or sets whether the document preview should be updated. If set to <see langword="null"/>, the operation is delayed until a value of <see langword="true"/> or <see langword="false"/> is assigned.
        /// </summary>
        public bool? IsUpdating { get; set; } = isUpdating;
    }
}

namespace PrintDialogX.Enums
{
    /// <summary>
    /// Specifies whether the printer collates output when it prints multiple copies of a multi-page print job.
    /// </summary>
    public enum Collation
    {
        /// <summary>
        /// Collated output.
        /// </summary>
        Collated,

        /// <summary>
        /// Uncollated output.
        /// </summary>
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
        [StringResource(StringResource.EntryAllPages)]
        AllPages,

        /// <summary>
        /// Current page.
        /// </summary>
        [StringResource(StringResource.EntryCurrentPage)]
        CurrentPage,

        /// <summary>
        /// Custom pages.
        /// </summary>
        [StringResource(StringResource.EntryCustomPages)]
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
        [StringResource(StringResource.EntryPortrait)]
        Portrait,

        /// <summary>
        /// Content of the imageable area is rotated on the page 90 degrees counterclockwise from standard (portrait) orientation.
        /// </summary>
        [StringResource(StringResource.EntryLandscape)]
        Landscape
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Size"/> struct.
    /// </summary>
    public struct Size()
    {
        /// <summary>
        /// Specifies the defined page size or roll width of the paper or other print media.
        /// </summary>
        public enum DefinedSize
        {
            /// <summary>
            /// A0.
            /// </summary>
            [StringResource(StringResource.EntryISOA0)]
            ISOA0,

            /// <summary>
            /// A1.
            /// </summary>
            [StringResource(StringResource.EntryISOA1)]
            ISOA1,

            /// <summary>
            /// A10.
            /// </summary>
            [StringResource(StringResource.EntryISOA10)]
            ISOA10,

            /// <summary>
            /// A2.
            /// </summary>
            [StringResource(StringResource.EntryISOA2)]
            ISOA2,

            /// <summary>
            /// A3.
            /// </summary>
            [StringResource(StringResource.EntryISOA3)]
            ISOA3,

            /// <summary>
            /// A3 Rotated.
            /// </summary>
            [StringResource(StringResource.EntryISOA3Rotated)]
            ISOA3Rotated,

            /// <summary>
            /// A3 Extra.
            /// </summary>
            [StringResource(StringResource.EntryISOA3Extra)]
            ISOA3Extra,

            /// <summary>
            /// A4.
            /// </summary>
            [StringResource(StringResource.EntryISOA4)]
            ISOA4,

            /// <summary>
            /// A4 Rotated.
            /// </summary>
            [StringResource(StringResource.EntryISOA4Rotated)]
            ISOA4Rotated,

            /// <summary>
            /// A4 Extra.
            /// </summary>
            [StringResource(StringResource.EntryISOA4Extra)]
            ISOA4Extra,

            /// <summary>
            /// A5.
            /// </summary>
            [StringResource(StringResource.EntryISOA5)]
            ISOA5,

            /// <summary>
            /// A5 Rotated.
            /// </summary>
            [StringResource(StringResource.EntryISOA5Rotated)]
            ISOA5Rotated,

            /// <summary>
            /// A5 Extra.
            /// </summary>
            [StringResource(StringResource.EntryISOA5Extra)]
            ISOA5Extra,

            /// <summary>
            /// A6.
            /// </summary>
            [StringResource(StringResource.EntryISOA6)]
            ISOA6,

            /// <summary>
            /// A6 Rotated.
            /// </summary>
            [StringResource(StringResource.EntryISOA6Rotated)]
            ISOA6Rotated,

            /// <summary>
            /// A7.
            /// </summary>
            [StringResource(StringResource.EntryISOA7)]
            ISOA7,

            /// <summary>
            /// A8.
            /// </summary>
            [StringResource(StringResource.EntryISOA8)]
            ISOA8,

            /// <summary>
            /// A9.
            /// </summary>
            [StringResource(StringResource.EntryISOA9)]
            ISOA9,

            /// <summary>
            /// B0.
            /// </summary>
            [StringResource(StringResource.EntryISOB0)]
            ISOB0,

            /// <summary>
            /// B1.
            /// </summary>
            [StringResource(StringResource.EntryISOB1)]
            ISOB1,

            /// <summary>
            /// B10.
            /// </summary>
            [StringResource(StringResource.EntryISOB10)]
            ISOB10,

            /// <summary>
            /// B2.
            /// </summary>
            [StringResource(StringResource.EntryISOB2)]
            ISOB2,

            /// <summary>
            /// B3.
            /// </summary>
            [StringResource(StringResource.EntryISOB3)]
            ISOB3,

            /// <summary>
            /// B4.
            /// </summary>
            [StringResource(StringResource.EntryISOB4)]
            ISOB4,

            /// <summary>
            /// B4 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryISOB4Envelope)]
            ISOB4Envelope,

            /// <summary>
            /// B5 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryISOB5Envelope)]
            ISOB5Envelope,

            /// <summary>
            /// B5 Extra.
            /// </summary>
            [StringResource(StringResource.EntryISOB5Extra)]
            ISOB5Extra,

            /// <summary>
            /// B7.
            /// </summary>
            [StringResource(StringResource.EntryISOB7)]
            ISOB7,

            /// <summary>
            /// B8.
            /// </summary>
            [StringResource(StringResource.EntryISOB8)]
            ISOB8,

            /// <summary>
            /// B9.
            /// </summary>
            [StringResource(StringResource.EntryISOB9)]
            ISOB9,

            /// <summary>
            /// C0.
            /// </summary>
            [StringResource(StringResource.EntryISOC0)]
            ISOC0,

            /// <summary>
            /// C1.
            /// </summary>
            [StringResource(StringResource.EntryISOC1)]
            ISOC1,

            /// <summary>
            /// C10.
            /// </summary>
            [StringResource(StringResource.EntryISOC10)]
            ISOC10,

            /// <summary>
            /// C2.
            /// </summary>
            [StringResource(StringResource.EntryISOC2)]
            ISOC2,

            /// <summary>
            /// C3.
            /// </summary>
            [StringResource(StringResource.EntryISOC3)]
            ISOC3,

            /// <summary>
            /// C3 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryISOC3Envelope)]
            ISOC3Envelope,

            /// <summary>
            /// C4.
            /// </summary>
            [StringResource(StringResource.EntryISOC4)]
            ISOC4,

            /// <summary>
            /// C4 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryISOC4Envelope)]
            ISOC4Envelope,

            /// <summary>
            /// C5.
            /// </summary>
            [StringResource(StringResource.EntryISOC5)]
            ISOC5,

            /// <summary>
            /// C5 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryISOC5Envelope)]
            ISOC5Envelope,

            /// <summary>
            /// C6.
            /// </summary>
            [StringResource(StringResource.EntryISOC6)]
            ISOC6,

            /// <summary>
            /// C6 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryISOC6Envelope)]
            ISOC6Envelope,

            /// <summary>
            /// C6C5 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryISOC6C5Envelope)]
            ISOC6C5Envelope,

            /// <summary>
            /// C7.
            /// </summary>
            [StringResource(StringResource.EntryISOC7)]
            ISOC7,

            /// <summary>
            /// C8.
            /// </summary>
            [StringResource(StringResource.EntryISOC8)]
            ISOC8,

            /// <summary>
            /// C9.
            /// </summary>
            [StringResource(StringResource.EntryISOC9)]
            ISOC9,

            /// <summary>
            /// DL Envelope.
            /// </summary>
            [StringResource(StringResource.EntryISODLEnvelope)]
            ISODLEnvelope,

            /// <summary>
            /// DL Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryISODLEnvelopeRotated)]
            ISODLEnvelopeRotated,

            /// <summary>
            /// SRA 3.
            /// </summary>
            [StringResource(StringResource.EntryISOSRA3)]
            ISOSRA3,

            /// <summary>
            /// Quadruple Hagaki Postcard.
            /// </summary>
            [StringResource(StringResource.EntryJapanQuadrupleHagakiPostcard)]
            JapanQuadrupleHagakiPostcard,

            /// <summary>
            /// Japanese Industrial Standard B0.
            /// </summary>
            [StringResource(StringResource.EntryJISB0)]
            JISB0,

            /// <summary>
            /// Japanese Industrial Standard B1.
            /// </summary>
            [StringResource(StringResource.EntryJISB1)]
            JISB1,

            /// <summary>
            /// Japanese Industrial Standard B10.
            /// </summary>
            [StringResource(StringResource.EntryJISB10)]
            JISB10,

            /// <summary>
            /// Japanese Industrial Standard B2.
            /// </summary>
            [StringResource(StringResource.EntryJISB2)]
            JISB2,

            /// <summary>
            /// Japanese Industrial Standard B3.
            /// </summary>
            [StringResource(StringResource.EntryJISB3)]
            JISB3,

            /// <summary>
            /// Japanese Industrial Standard B4.
            /// </summary>
            [StringResource(StringResource.EntryJISB4)]
            JISB4,

            /// <summary>
            /// Japanese Industrial Standard B4 Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJISB4Rotated)]
            JISB4Rotated,

            /// <summary>
            /// Japanese Industrial Standard B5.
            /// </summary>
            [StringResource(StringResource.EntryJISB5)]
            JISB5,

            /// <summary>
            /// Japanese Industrial Standard B5 Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJISB5Rotated)]
            JISB5Rotated,

            /// <summary>
            /// Japanese Industrial Standard B6.
            /// </summary>
            [StringResource(StringResource.EntryJISB6)]
            JISB6,

            /// <summary>
            /// Japanese Industrial Standard B6 Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJISB6Rotated)]
            JISB6Rotated,

            /// <summary>
            /// Japanese Industrial Standard B7.
            /// </summary>
            [StringResource(StringResource.EntryJISB7)]
            JISB7,

            /// <summary>
            /// Japanese Industrial Standard B8.
            /// </summary>
            [StringResource(StringResource.EntryJISB8)]
            JISB8,

            /// <summary>
            /// Japanese Industrial Standard B9.
            /// </summary>
            [StringResource(StringResource.EntryJISB9)]
            JISB9,

            /// <summary>
            /// Chou 3 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanChou3Envelope)]
            JapanChou3Envelope,

            /// <summary>
            /// Chou 3 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJapanChou3EnvelopeRotated)]
            JapanChou3EnvelopeRotated,

            /// <summary>
            /// Chou 4 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanChou4Envelope)]
            JapanChou4Envelope,

            /// <summary>
            /// Chou 4 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJapanChou4EnvelopeRotated)]
            JapanChou4EnvelopeRotated,

            /// <summary>
            /// Hagaki Postcard.
            /// </summary>
            [StringResource(StringResource.EntryJapanHagakiPostcard)]
            JapanHagakiPostcard,

            /// <summary>
            /// Hagaki Postcard Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJapanHagakiPostcardRotated)]
            JapanHagakiPostcardRotated,

            /// <summary>
            /// Kaku 2 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanKaku2Envelope)]
            JapanKaku2Envelope,

            /// <summary>
            /// Kaku 2 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJapanKaku2EnvelopeRotated)]
            JapanKaku2EnvelopeRotated,

            /// <summary>
            /// Kaku 3 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanKaku3Envelope)]
            JapanKaku3Envelope,

            /// <summary>
            /// Kaku 3 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJapanKaku3EnvelopeRotated)]
            JapanKaku3EnvelopeRotated,

            /// <summary>
            /// You 4 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanYou4Envelope)]
            JapanYou4Envelope,

            /// <summary>
            /// 10 x 11.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica10x11)]
            NorthAmerica10x11,

            /// <summary>
            /// 10 x 14.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica10x14)]
            NorthAmerica10x14,

            /// <summary>
            /// 11 x 17.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica11x17)]
            NorthAmerica11x17,

            /// <summary>
            /// 9 x 11.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica9x11)]
            NorthAmerica9x11,

            /// <summary>
            /// Architecture A Sheet.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaArchitectureASheet)]
            NorthAmericaArchitectureASheet,

            /// <summary>
            /// Architecture B Sheet.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaArchitectureBSheet)]
            NorthAmericaArchitectureBSheet,

            /// <summary>
            /// Architecture C Sheet.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaArchitectureCSheet)]
            NorthAmericaArchitectureCSheet,

            /// <summary>
            /// Architecture D Sheet.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaArchitectureDSheet)]
            NorthAmericaArchitectureDSheet,

            /// <summary>
            /// Architecture E Sheet.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaArchitectureESheet)]
            NorthAmericaArchitectureESheet,

            /// <summary>
            /// C Sheet.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaCSheet)]
            NorthAmericaCSheet,

            /// <summary>
            /// D Sheet.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaDSheet)]
            NorthAmericaDSheet,

            /// <summary>
            /// E Sheet.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaESheet)]
            NorthAmericaESheet,

            /// <summary>
            /// Executive.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaExecutive)]
            NorthAmericaExecutive,

            /// <summary>
            /// German Legal Fanfold.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaGermanLegalFanfold)]
            NorthAmericaGermanLegalFanfold,

            /// <summary>
            /// German Standard Fanfold.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaGermanStandardFanfold)]
            NorthAmericaGermanStandardFanfold,

            /// <summary>
            /// Legal.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaLegal)]
            NorthAmericaLegal,

            /// <summary>
            /// Legal Extra.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaLegalExtra)]
            NorthAmericaLegalExtra,

            /// <summary>
            /// Letter.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaLetter)]
            NorthAmericaLetter,

            /// <summary>
            /// Letter Rotated.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaLetterRotated)]
            NorthAmericaLetterRotated,

            /// <summary>
            /// Letter Extra.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaLetterExtra)]
            NorthAmericaLetterExtra,

            /// <summary>
            /// Letter Plus.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaLetterPlus)]
            NorthAmericaLetterPlus,

            /// <summary>
            /// Monarch Envelope.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaMonarchEnvelope)]
            NorthAmericaMonarchEnvelope,

            /// <summary>
            /// Note.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaNote)]
            NorthAmericaNote,

            /// <summary>
            /// #10 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaNumber10Envelope)]
            NorthAmericaNumber10Envelope,

            /// <summary>
            /// #10 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaNumber10EnvelopeRotated)]
            NorthAmericaNumber10EnvelopeRotated,

            /// <summary>
            /// #9 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaNumber9Envelope)]
            NorthAmericaNumber9Envelope,

            /// <summary>
            /// #11 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaNumber11Envelope)]
            NorthAmericaNumber11Envelope,

            /// <summary>
            /// #12 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaNumber12Envelope)]
            NorthAmericaNumber12Envelope,

            /// <summary>
            /// #14 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaNumber14Envelope)]
            NorthAmericaNumber14Envelope,

            /// <summary>
            /// Personal Envelope.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaPersonalEnvelope)]
            NorthAmericaPersonalEnvelope,

            /// <summary>
            /// Quarto.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaQuarto)]
            NorthAmericaQuarto,

            /// <summary>
            /// Statement.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaStatement)]
            NorthAmericaStatement,

            /// <summary>
            /// Super A.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaSuperA)]
            NorthAmericaSuperA,

            /// <summary>
            /// Super B.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaSuperB)]
            NorthAmericaSuperB,

            /// <summary>
            /// Tabloid.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaTabloid)]
            NorthAmericaTabloid,

            /// <summary>
            /// Tabloid Extra.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmericaTabloidExtra)]
            NorthAmericaTabloidExtra,

            /// <summary>
            /// A4 Plus.
            /// </summary>
            [StringResource(StringResource.EntryOtherMetricA4Plus)]
            OtherMetricA4Plus,

            /// <summary>
            /// A3 Plus.
            /// </summary>
            [StringResource(StringResource.EntryOtherMetricA3Plus)]
            OtherMetricA3Plus,

            /// <summary>
            /// Folio.
            /// </summary>
            [StringResource(StringResource.EntryOtherMetricFolio)]
            OtherMetricFolio,

            /// <summary>
            /// Invite Envelope.
            /// </summary>
            [StringResource(StringResource.EntryOtherMetricInviteEnvelope)]
            OtherMetricInviteEnvelope,

            /// <summary>
            /// Italian Envelope.
            /// </summary>
            [StringResource(StringResource.EntryOtherMetricItalianEnvelope)]
            OtherMetricItalianEnvelope,

            /// <summary>
            /// People's Republic of China #1 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC1Envelope)]
            PRC1Envelope,

            /// <summary>
            /// People's Republic of China #1 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC1EnvelopeRotated)]
            PRC1EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #10 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC10Envelope)]
            PRC10Envelope,

            /// <summary>
            /// People's Republic of China #10 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC10EnvelopeRotated)]
            PRC10EnvelopeRotated,

            /// <summary>
            /// People's Republic of China 16K.
            /// </summary>
            [StringResource(StringResource.EntryPRC16K)]
            PRC16K,

            /// <summary>
            /// People's Republic of China 16K Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC16KRotated)]
            PRC16KRotated,

            /// <summary>
            /// People's Republic of China #2 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC2Envelope)]
            PRC2Envelope,

            /// <summary>
            /// People's Republic of China #2 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC2EnvelopeRotated)]
            PRC2EnvelopeRotated,

            /// <summary>
            /// People's Republic of China 32K.
            /// </summary>
            [StringResource(StringResource.EntryPRC32K)]
            PRC32K,

            /// <summary>
            /// People's Republic of China 32K Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC32KRotated)]
            PRC32KRotated,

            /// <summary>
            /// People's Republic of China 32K Big.
            /// </summary>
            [StringResource(StringResource.EntryPRC32KBig)]
            PRC32KBig,

            /// <summary>
            /// People's Republic of China #3 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC3Envelope)]
            PRC3Envelope,

            /// <summary>
            /// People's Republic of China #3 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC3EnvelopeRotated)]
            PRC3EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #4 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC4Envelope)]
            PRC4Envelope,

            /// <summary>
            /// People's Republic of China #4 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC4EnvelopeRotated)]
            PRC4EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #5 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC5Envelope)]
            PRC5Envelope,

            /// <summary>
            /// People's Republic of China #5 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC5EnvelopeRotated)]
            PRC5EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #6 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC6Envelope)]
            PRC6Envelope,

            /// <summary>
            /// People's Republic of China #6 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC6EnvelopeRotated)]
            PRC6EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #7 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC7Envelope)]
            PRC7Envelope,

            /// <summary>
            /// People's Republic of China #7 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC7EnvelopeRotated)]
            PRC7EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #8 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC8Envelope)]
            PRC8Envelope,

            /// <summary>
            /// People's Republic of China #8 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC8EnvelopeRotated)]
            PRC8EnvelopeRotated,

            /// <summary>
            /// People's Republic of China #9 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryPRC9Envelope)]
            PRC9Envelope,

            /// <summary>
            /// People's Republic of China #9 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryPRC9EnvelopeRotated)]
            PRC9EnvelopeRotated,

            /// <summary>
            /// 4-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll04Inch)]
            Roll04Inch,

            /// <summary>
            /// 6-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll06Inch)]
            Roll06Inch,

            /// <summary>
            /// 8-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll08Inch)]
            Roll08Inch,

            /// <summary>
            /// 12-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll12Inch)]
            Roll12Inch,

            /// <summary>
            /// 15-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll15Inch)]
            Roll15Inch,

            /// <summary>
            /// 18-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll18Inch)]
            Roll18Inch,

            /// <summary>
            /// 22-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll22Inch)]
            Roll22Inch,

            /// <summary>
            /// 24-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll24Inch)]
            Roll24Inch,

            /// <summary>
            /// 30-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll30Inch)]
            Roll30Inch,

            /// <summary>
            /// 36-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll36Inch)]
            Roll36Inch,

            /// <summary>
            /// 54-inch wide roll.
            /// </summary>
            [StringResource(StringResource.EntryRoll54Inch)]
            Roll54Inch,

            /// <summary>
            /// Double Hagaki Postcard.
            /// </summary>
            [StringResource(StringResource.EntryJapanDoubleHagakiPostcard)]
            JapanDoubleHagakiPostcard,

            /// <summary>
            /// Double Hagaki Postcard Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJapanDoubleHagakiPostcardRotated)]
            JapanDoubleHagakiPostcardRotated,

            /// <summary>
            /// L Photo.
            /// </summary>
            [StringResource(StringResource.EntryJapanLPhoto)]
            JapanLPhoto,

            /// <summary>
            /// 2L Photo.
            /// </summary>
            [StringResource(StringResource.EntryJapan2LPhoto)]
            Japan2LPhoto,

            /// <summary>
            /// You 1 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanYou1Envelope)]
            JapanYou1Envelope,

            /// <summary>
            /// You 2 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanYou2Envelope)]
            JapanYou2Envelope,

            /// <summary>
            /// You 3 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanYou3Envelope)]
            JapanYou3Envelope,

            /// <summary>
            /// You 4 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJapanYou4EnvelopeRotated)]
            JapanYou4EnvelopeRotated,

            /// <summary>
            /// You 6 Envelope.
            /// </summary>
            [StringResource(StringResource.EntryJapanYou6Envelope)]
            JapanYou6Envelope,

            /// <summary>
            /// You 6 Envelope Rotated.
            /// </summary>
            [StringResource(StringResource.EntryJapanYou6EnvelopeRotated)]
            JapanYou6EnvelopeRotated,

            /// <summary>
            /// 4 x 6.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica4x6)]
            NorthAmerica4x6,

            /// <summary>
            /// 4 x 8.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica4x8)]
            NorthAmerica4x8,

            /// <summary>
            /// 5 x 7.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica5x7)]
            NorthAmerica5x7,

            /// <summary>
            /// 8 x 10.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica8x10)]
            NorthAmerica8x10,

            /// <summary>
            /// 10 x 12.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica10x12)]
            NorthAmerica10x12,

            /// <summary>
            /// 14 x 17.
            /// </summary>
            [StringResource(StringResource.EntryNorthAmerica14x17)]
            NorthAmerica14x17,

            /// <summary>
            /// Business card.
            /// </summary>
            [StringResource(StringResource.EntryBusinessCard)]
            BusinessCard,

            /// <summary>
            /// Credit card.
            /// </summary>
            [StringResource(StringResource.EntryCreditCard)]
            CreditCard
        }

        /// <summary>
        /// Gets or sets the defined size associated with the size.
        /// </summary>
        public DefinedSize? DefinedName { get; set; }

        /// <summary>
        /// Gets or sets the fallback name of the size.
        /// </summary>
        public string? FallbackName { get; set; }

        /// <summary>
        /// Gets or sets the width of the size.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the size.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="name">The defined size to set to.</param>
        public Size(DefinedSize name) : this()
        {
            (double width, double height) = name switch
            {
                DefinedSize.ISOA0 => (3178.5826772, 4493.8582677),
                DefinedSize.ISOA1 => (2245.0393701, 3178.5826772),
                DefinedSize.ISOA10 => (98.267716535, 139.84251969),
                DefinedSize.ISOA2 => (1587.4015748, 2245.0393701),
                DefinedSize.ISOA3 => (1122.519685, 1587.4015748),
                DefinedSize.ISOA3Rotated => (1587.4015748, 1122.519685),
                DefinedSize.ISOA3Extra => (1217.007874, 1681.8897638),
                DefinedSize.ISOA4 => (793.7007874, 1122.519685),
                DefinedSize.ISOA4Rotated => (1122.519685, 793.7007874),
                DefinedSize.ISOA4Extra => (889.92, 1218.24),
                DefinedSize.ISOA5 => (559.37007874, 793.7007874),
                DefinedSize.ISOA5Rotated => (793.7007874, 559.37007874),
                DefinedSize.ISOA5Extra => (657.63779528, 888.18897638),
                DefinedSize.ISOA6 => (396.8503937, 559.37007874),
                DefinedSize.ISOA6Rotated => (559.37007874, 396.8503937),
                DefinedSize.ISOA7 => (279.68503937, 396.8503937),
                DefinedSize.ISOA8 => (196.53543307, 279.68503937),
                DefinedSize.ISOA9 => (139.84251969, 196.53543307),
                DefinedSize.ISOB0 => (3779.5275591, 5344.2519685),
                DefinedSize.ISOB1 => (2672.1259843, 3779.5275591),
                DefinedSize.ISOB10 => (117.16535433, 166.2992126),
                DefinedSize.ISOB2 => (1889.7637795, 2672.1259843),
                DefinedSize.ISOB3 => (1334.1732283, 1889.7637795),
                DefinedSize.ISOB4 => (944.88188976, 1334.1732283),
                DefinedSize.ISOB4Envelope => (944.88188976, 1334.1732283),
                DefinedSize.ISOB5Envelope => (665.19685039, 944.88188976),
                DefinedSize.ISOB5Extra => (759.68503937, 1043.1496063),
                DefinedSize.ISOB7 => (332.5984252, 472.44094488),
                DefinedSize.ISOB8 => (234.33070866, 332.5984252),
                DefinedSize.ISOB9 => (166.2992126, 234.33070866),
                DefinedSize.ISOC0 => (3465.8267717, 4902.0472441),
                DefinedSize.ISOC1 => (2449.1338583, 3465.8267717),
                DefinedSize.ISOC10 => (105.82677165, 151.18110236),
                DefinedSize.ISOC2 => (1731.023622, 2449.1338583),
                DefinedSize.ISOC3 => (1224.5669291, 1731.023622),
                DefinedSize.ISOC3Envelope => (1224.5669291, 1731.023622),
                DefinedSize.ISOC4 => (865.51181102, 1224.5669291),
                DefinedSize.ISOC4Envelope => (865.51181102, 1224.5669291),
                DefinedSize.ISOC5 => (612.28346457, 865.51181102),
                DefinedSize.ISOC5Envelope => (612.28346457, 865.51181102),
                DefinedSize.ISOC6 => (430.86614173, 612.28346457),
                DefinedSize.ISOC6Envelope => (430.86614173, 612.28346457),
                DefinedSize.ISOC6C5Envelope => (430.86614173, 865.51181102),
                DefinedSize.ISOC7 => (306.14173228, 430.86614173),
                DefinedSize.ISOC8 => (215.43307087, 306.14173228),
                DefinedSize.ISOC9 => (151.18110236, 215.43307087),
                DefinedSize.ISODLEnvelope => (415.7480315, 831.49606299),
                DefinedSize.ISODLEnvelopeRotated => (831.49606299, 415.7480315),
                DefinedSize.ISOSRA3 => (1209.4488189, 1700.7874016),
                DefinedSize.JapanQuadrupleHagakiPostcard => (755.90551181, 559.37007874),
                DefinedSize.JISB0 => (3892.9133858, 5502.992126),
                DefinedSize.JISB1 => (2751.496063, 3892.9133858),
                DefinedSize.JISB10 => (120.94488189, 170.07874016),
                DefinedSize.JISB2 => (1946.4566929, 2751.496063),
                DefinedSize.JISB3 => (1375.7480315, 1946.4566929),
                DefinedSize.JISB4 => (971.33858268, 1375.7480315),
                DefinedSize.JISB4Rotated => (1375.7480315, 971.33858268),
                DefinedSize.JISB5 => (687.87401575, 971.33858268),
                DefinedSize.JISB5Rotated => (971.33858268, 687.87401575),
                DefinedSize.JISB6 => (483.77952756, 687.87401575),
                DefinedSize.JISB6Rotated => (687.87401575, 483.77952756),
                DefinedSize.JISB7 => (343.93700787, 483.77952756),
                DefinedSize.JISB8 => (241.88976378, 343.93700787),
                DefinedSize.JISB9 => (170.07874016, 241.88976378),
                DefinedSize.JapanChou3Envelope => (453.54330709, 888.18897638),
                DefinedSize.JapanChou3EnvelopeRotated => (888.18897638, 453.54330709),
                DefinedSize.JapanChou4Envelope => (340.15748031, 774.80314961),
                DefinedSize.JapanChou4EnvelopeRotated => (774.80314961, 340.15748031),
                DefinedSize.JapanHagakiPostcard => (377.95275591, 559.37007874),
                DefinedSize.JapanHagakiPostcardRotated => (559.37007874, 377.95275591),
                DefinedSize.JapanKaku2Envelope => (907.08661417, 1254.8031496),
                DefinedSize.JapanKaku2EnvelopeRotated => (1254.8031496, 907.08661417),
                DefinedSize.JapanKaku3Envelope => (816.37795276, 1046.9291339),
                DefinedSize.JapanKaku3EnvelopeRotated => (1046.9291339, 816.37795276),
                DefinedSize.JapanYou4Envelope => (396.8503937, 888.18897638),
                DefinedSize.NorthAmerica10x11 => (960, 1056),
                DefinedSize.NorthAmerica10x14 => (960, 1344),
                DefinedSize.NorthAmerica11x17 => (1056, 1632),
                DefinedSize.NorthAmerica9x11 => (864, 1056),
                DefinedSize.NorthAmericaArchitectureASheet => (864, 1152),
                DefinedSize.NorthAmericaArchitectureBSheet => (1152, 1728),
                DefinedSize.NorthAmericaArchitectureCSheet => (1728, 2304),
                DefinedSize.NorthAmericaArchitectureDSheet => (2304, 3456),
                DefinedSize.NorthAmericaArchitectureESheet => (3456, 4608),
                DefinedSize.NorthAmericaCSheet => (1632, 2112),
                DefinedSize.NorthAmericaDSheet => (2112, 3264),
                DefinedSize.NorthAmericaESheet => (3264, 4224),
                DefinedSize.NorthAmericaExecutive => (696, 1008),
                DefinedSize.NorthAmericaGermanLegalFanfold => (816, 1248),
                DefinedSize.NorthAmericaGermanStandardFanfold => (816, 1152),
                DefinedSize.NorthAmericaLegal => (816, 1344),
                DefinedSize.NorthAmericaLegalExtra => (912, 1440),
                DefinedSize.NorthAmericaLetter => (816, 1056),
                DefinedSize.NorthAmericaLetterRotated => (1056, 816),
                DefinedSize.NorthAmericaLetterExtra => (912, 1152),
                DefinedSize.NorthAmericaLetterPlus => (816, 1218.24),
                DefinedSize.NorthAmericaMonarchEnvelope => (372, 720),
                DefinedSize.NorthAmericaNote => (816, 1056),
                DefinedSize.NorthAmericaNumber10Envelope => (396, 912),
                DefinedSize.NorthAmericaNumber10EnvelopeRotated => (912, 396),
                DefinedSize.NorthAmericaNumber9Envelope => (372, 852),
                DefinedSize.NorthAmericaNumber11Envelope => (432, 996),
                DefinedSize.NorthAmericaNumber12Envelope => (456, 1056),
                DefinedSize.NorthAmericaNumber14Envelope => (480, 1104),
                DefinedSize.NorthAmericaPersonalEnvelope => (348, 624),
                DefinedSize.NorthAmericaQuarto => (812.5984252, 1039.3700787),
                DefinedSize.NorthAmericaStatement => (528, 816),
                DefinedSize.NorthAmericaSuperA => (857.95275591, 1345.511811),
                DefinedSize.NorthAmericaSuperB => (1152.7559055, 1840.6299213),
                DefinedSize.NorthAmericaTabloid => (1056, 1632),
                DefinedSize.NorthAmericaTabloidExtra => (1122.24, 1728),
                DefinedSize.OtherMetricA4Plus => (793.7007874, 1247.2440945),
                DefinedSize.OtherMetricA3Plus => (1243.4645669, 1825.511811),
                DefinedSize.OtherMetricFolio => (816, 1248),
                DefinedSize.OtherMetricInviteEnvelope => (831.49606299, 831.49606299),
                DefinedSize.OtherMetricItalianEnvelope => (415.7480315, 869.29133858),
                DefinedSize.PRC1Envelope => (385.51181102, 623.62204724),
                DefinedSize.PRC1EnvelopeRotated => (623.62204724, 385.51181102),
                DefinedSize.PRC10Envelope => (1224.5669291, 1731.023622),
                DefinedSize.PRC10EnvelopeRotated => (1731.023622, 1224.5669291),
                DefinedSize.PRC16K => (551.81102362, 812.5984252),
                DefinedSize.PRC16KRotated => (812.5984252, 551.81102362),
                DefinedSize.PRC2Envelope => (385.51181102, 665.19685039),
                DefinedSize.PRC2EnvelopeRotated => (665.19685039, 385.51181102),
                DefinedSize.PRC32K => (366.61417323, 570.70866142),
                DefinedSize.PRC32KRotated => (570.70866142, 366.61417323),
                DefinedSize.PRC32KBig => (366.61417323, 570.70866142),
                DefinedSize.PRC3Envelope => (472.44094488, 665.19685039),
                DefinedSize.PRC3EnvelopeRotated => (665.19685039, 472.44094488),
                DefinedSize.PRC4Envelope => (415.7480315, 786.14173228),
                DefinedSize.PRC4EnvelopeRotated => (786.14173228, 415.7480315),
                DefinedSize.PRC5Envelope => (415.7480315, 831.49606299),
                DefinedSize.PRC5EnvelopeRotated => (831.49606299, 415.7480315),
                DefinedSize.PRC6Envelope => (453.54330709, 869.29133858),
                DefinedSize.PRC6EnvelopeRotated => (869.29133858, 453.54330709),
                DefinedSize.PRC7Envelope => (604.72440945, 869.29133858),
                DefinedSize.PRC7EnvelopeRotated => (869.29133858, 604.72440945),
                DefinedSize.PRC8Envelope => (453.54330709, 1167.8740157),
                DefinedSize.PRC8EnvelopeRotated => (1167.8740157, 453.54330709),
                DefinedSize.PRC9Envelope => (865.51181102, 1224.5669291),
                DefinedSize.PRC9EnvelopeRotated => (1224.5669291, 865.51181102),
                DefinedSize.Roll04Inch => (384, 1056),
                DefinedSize.Roll06Inch => (576, 1056),
                DefinedSize.Roll08Inch => (768, 1056),
                DefinedSize.Roll12Inch => (1152, 2304),
                DefinedSize.Roll15Inch => (1440, 2304),
                DefinedSize.Roll18Inch => (1728, 2304),
                DefinedSize.Roll22Inch => (2112, 2304),
                DefinedSize.Roll24Inch => (2304, 2304),
                DefinedSize.Roll30Inch => (2880, 3456),
                DefinedSize.Roll36Inch => (3456, 3456),
                DefinedSize.Roll54Inch => (5184, 3456),
                DefinedSize.JapanDoubleHagakiPostcard => (755.90551181, 559.37007874),
                DefinedSize.JapanDoubleHagakiPostcardRotated => (559.37007874, 755.90551181),
                DefinedSize.JapanLPhoto => (336.37795276, 480),
                DefinedSize.Japan2LPhoto => (480, 672.75590551),
                DefinedSize.JapanYou1Envelope => (453.54330709, 888.18897638),
                DefinedSize.JapanYou2Envelope => (396.8503937, 559.37007874),
                DefinedSize.JapanYou3Envelope => (370.39370079, 559.37007874),
                DefinedSize.JapanYou4EnvelopeRotated => (888.18897638, 396.8503937),
                DefinedSize.JapanYou6Envelope => (370.39370079, 718.11023622),
                DefinedSize.JapanYou6EnvelopeRotated => (718.11023622, 370.39370079),
                DefinedSize.NorthAmerica4x6 => (384, 576),
                DefinedSize.NorthAmerica4x8 => (384, 768),
                DefinedSize.NorthAmerica5x7 => (480, 672),
                DefinedSize.NorthAmerica8x10 => (768, 960),
                DefinedSize.NorthAmerica10x12 => (960, 1152),
                DefinedSize.NorthAmerica14x17 => (1344, 1632),
                DefinedSize.BusinessCard => (279.68503937, 196.53543307),
                DefinedSize.CreditCard => (324, 204),
                _ => (0, 0)
            };
            DefinedName = name;
            Width = width;
            Height = height;
        }

        public override readonly int GetHashCode()
        {
            return (DefinedName, GetRoundedSize()).GetHashCode();
        }

        public override readonly bool Equals(object? value)
        {
            return value is Size size && Equals(size.DefinedName, size.Width, size.Height);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="name">The defined size of the specified object.</param>
        /// <param name="width">The width of the specified object.</param>
        /// <param name="height">The height of the specified object.</param>
        /// <param name="digits">The number of decimal places to be used for rounding.</param>
        /// <returns><see langword="true"/> if the specified object and this instance are the same type and represent the same value; otherwise, <see langword="false"/>.</returns>
        public readonly bool Equals(DefinedSize? name, double? width, double? height, int digits = 1)
        {
            return (name != null && name == DefinedName) || (width != null && height != null && (GetRoundedSize(width.Value, height.Value, digits) == GetRoundedSize(digits)));
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="size">The specified object.</param>
        /// <returns><see langword="true"/> if the specified object and this instance are the same type and represent the same value; otherwise, <see langword="false"/>.</returns>
        public readonly bool Equals(PageMediaSize? size)
        {
            return size != null && Equals(size.PageMediaSizeName != null ? ValueMappings.Map(size.PageMediaSizeName.Value, ValueMappings.SizeNameMapping) : null, size.Width, size.Height);
        }

        /// <summary>
        /// Gets the size in rounded width and height.
        /// </summary>
        /// <param name="digits">The number of decimal places in the return value.</param>
        /// <returns>The values of the rounded width and height.</returns>
        public readonly (decimal Width, decimal Height) GetRoundedSize(int digits = 1)
        {
            return GetRoundedSize(Width, Height, digits);
        }

        /// <summary>
        /// Gets the size in rounded width and height.
        /// </summary>
        /// <param name="width">The width of the size.</param>
        /// <param name="height">The height of the size.</param>
        /// <param name="digits">The number of decimal places in the return value.</param>
        /// <returns>The values of the rounded width and height.</returns>
        public static (decimal Width, decimal Height) GetRoundedSize(double width, double height, int digits = 1)
        {
            return (Math.Round((decimal)width, digits), Math.Round((decimal)height, digits));
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
        [StringResource(StringResource.EntryColor)]
        Color,

        /// <summary>
        /// Output that prints in a grayscale.
        /// </summary>
        [StringResource(StringResource.EntryGrayscale)]
        Grayscale,

        /// <summary>
        /// Output that prints in a single color and with the same degree of intensity.
        /// </summary>
        [StringResource(StringResource.EntryMonochrome)]
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
        [StringResource(StringResource.EntryAutomatic)]
        Automatic,

        /// <summary>
        /// Draft quality.
        /// </summary>
        [StringResource(StringResource.EntryDraft)]
        Draft,

        /// <summary>
        /// Fax quality.
        /// </summary>
        [StringResource(StringResource.EntryFax)]
        Fax,

        /// <summary>
        /// Higher than normal quality.
        /// </summary>
        [StringResource(StringResource.EntryHigh)]
        High,

        /// <summary>
        /// Normal quality.
        /// </summary>
        [StringResource(StringResource.EntryNormal)]
        Normal,

        /// <summary>
        /// Photographic quality.
        /// </summary>
        [StringResource(StringResource.EntryPhotographic)]
        Photographic,

        /// <summary>
        /// Text quality.
        /// </summary>
        [StringResource(StringResource.EntryText)]
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
        [StringResource(StringResource.EntryOne)]
        One,

        /// <summary>
        /// 2 pages per sheet.
        /// </summary>
        [StringResource(StringResource.EntryTwo)]
        Two,

        /// <summary>
        /// 4 pages per sheet.
        /// </summary>
        [StringResource(StringResource.EntryFour)]
        Four,

        /// <summary>
        /// 6 pages per sheet.
        /// </summary>
        [StringResource(StringResource.EntrySix)]
        Six,

        /// <summary>
        /// 9 pages per sheet.
        /// </summary>
        [StringResource(StringResource.EntryNine)]
        Nine,

        /// <summary>
        /// 16 pages per sheet.
        /// </summary>
        [StringResource(StringResource.EntrySixteen)]
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
        [StringResource(StringResource.EntryHorizontal)]
        Horizontal,

        /// <summary>
        /// Pages appear in rows, from right to left and top to bottom, relative to the page orientation.
        /// </summary>
        [StringResource(StringResource.EntryHorizontalReverse)]
        HorizontalReverse,

        /// <summary>
        /// Pages appear in columns, from top to bottom and left to right, relative to the page orientation.
        /// </summary>
        [StringResource(StringResource.EntryVertical)]
        Vertical,

        /// <summary>
        /// Pages appear in columns, from bottom to top and left to right, relative to the page orientation.
        /// </summary>
        [StringResource(StringResource.EntryVerticalReverse)]
        VerticalReverse
    }

    /// <summary>
    /// Specifies the percentage by which the printer enlarges or reduces the print content on a page.
    /// </summary>
    public enum Scale
    {
        [StringResource(StringResource.EntryAutoFit)]
        AutoFit,

        [StringResource(StringResource.EntryPercent25)]
        Percent25,

        [StringResource(StringResource.EntryPercent50)]
        Percent50,

        [StringResource(StringResource.EntryPercent75)]
        Percent75,

        [StringResource(StringResource.EntryPercent100)]
        Percent100,

        [StringResource(StringResource.EntryPercent150)]
        Percent150,

        [StringResource(StringResource.EntryPercent200)]
        Percent200,

        [StringResource(StringResource.EntryCustom)]
        Custom
    }

    /// <summary>
    /// Specifies the size of the unprinted margin around the edge of a page.
    /// </summary>
    public enum Margin
    {
        [StringResource(StringResource.EntryDefault)]
        Default,

        [StringResource(StringResource.EntryNone)]
        None,

        [StringResource(StringResource.EntryMinimum)]
        Minimum,

        [StringResource(StringResource.EntryCustom)]
        Custom
    }

    /// <summary>
    /// Specifies whether the printer uses one-sided printing or some type of two-sided (duplex) printing.
    /// </summary>
    public enum DoubleSided
    {
        /// <summary>
        /// Output prints on only one side of each sheet.
        /// </summary>
        OneSided,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="System.Printing.PrintDocumentImageableArea.MediaSizeWidth"/>.
        /// </summary>
        [StringResource(StringResource.EntryDoubleSidedShortEdge)]
        DoubleSidedShortEdge,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="System.Printing.PrintDocumentImageableArea.MediaSizeHeight"/>.
        /// </summary>
        [StringResource(StringResource.EntryDoubleSidedLongEdge)]
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
        [StringResource(StringResource.EntryAutoSelect)]
        AutoSelect,

        /// <summary>
        /// Archive-quality media.
        /// </summary>
        [StringResource(StringResource.EntryArchival)]
        Archival,

        /// <summary>
        /// Specialty back-printing film.
        /// </summary>
        [StringResource(StringResource.EntryBackPrintFilm)]
        BackPrintFilm,

        /// <summary>
        /// Standard bond media.
        /// </summary>
        [StringResource(StringResource.EntryBond)]
        Bond,

        /// <summary>
        /// Standard card stock.
        /// </summary>
        [StringResource(StringResource.EntryCardStock)]
        CardStock,

        /// <summary>
        /// Continuous-feed media.
        /// </summary>
        [StringResource(StringResource.EntryContinuous)]
        Continuous,

        /// <summary>
        /// Standard envelope.
        /// </summary>
        [StringResource(StringResource.EntryEnvelopePlain)]
        EnvelopePlain,

        /// <summary>
        /// Window envelope.
        /// </summary>
        [StringResource(StringResource.EntryEnvelopeWindow)]
        EnvelopeWindow,

        /// <summary>
        /// Fabric media.
        /// </summary>
        [StringResource(StringResource.EntryFabric)]
        Fabric,

        /// <summary>
        /// Specialty high-resolution media.
        /// </summary>
        [StringResource(StringResource.EntryHighResolution)]
        HighResolution,

        /// <summary>
        /// Label media.
        /// </summary>
        [StringResource(StringResource.EntryLabel)]
        Label,

        /// <summary>
        /// Attached multipart forms.
        /// </summary>
        [StringResource(StringResource.EntryMultiLayerForm)]
        MultiLayerForm,

        /// <summary>
        /// Individual multipart forms.
        /// </summary>
        [StringResource(StringResource.EntryMultiPartForm)]
        MultiPartForm,

        /// <summary>
        /// Standard photographic media.
        /// </summary>
        [StringResource(StringResource.EntryPhotographic)]
        Photographic,

        /// <summary>
        /// Film photographic media.
        /// </summary>
        [StringResource(StringResource.EntryPhotographicFilm)]
        PhotographicFilm,

        /// <summary>
        /// Glossy photographic media.
        /// </summary>
        [StringResource(StringResource.EntryPhotographicGlossy)]
        PhotographicGlossy,

        /// <summary>
        /// High-gloss photographic media.
        /// </summary>
        [StringResource(StringResource.EntryPhotographicHighGloss)]
        PhotographicHighGloss,

        /// <summary>
        /// Matte photographic media.
        /// </summary>
        [StringResource(StringResource.EntryPhotographicMatte)]
        PhotographicMatte,

        /// <summary>
        /// Satin photographic media.
        /// </summary>
        [StringResource(StringResource.EntryPhotographicSatin)]
        PhotographicSatin,

        /// <summary>
        /// Semi-gloss photographic media.
        /// </summary>
        [StringResource(StringResource.EntryPhotographicSemiGloss)]
        PhotographicSemiGloss,

        /// <summary>
        /// Plain paper.
        /// </summary>
        [StringResource(StringResource.EntryPlain)]
        Plain,

        /// <summary>
        /// Output to a display in continuous form.
        /// </summary>
        [StringResource(StringResource.EntryScreen)]
        Screen,

        /// <summary>
        /// Output to a display in paged form.
        /// </summary>
        [StringResource(StringResource.EntryScreenPaged)]
        ScreenPaged,

        /// <summary>
        /// Specialty stationary.
        /// </summary>
        [StringResource(StringResource.EntryStationery)]
        Stationery,

        /// <summary>
        /// Tab stock, not precut (single tabs).
        /// </summary>
        [StringResource(StringResource.EntryTabStockFull)]
        TabStockFull,

        /// <summary>
        /// Tab stock, precut (multiple tabs).
        /// </summary>
        [StringResource(StringResource.EntryTabStockPreCut)]
        TabStockPreCut,

        /// <summary>
        /// Transparent sheet.
        /// </summary>
        [StringResource(StringResource.EntryTransparency)]
        Transparency,

        /// <summary>
        /// Media that is used to transfer an image to a T-shirt.
        /// </summary>
        [StringResource(StringResource.EntryTShirtTransfer)]
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
        [StringResource(StringResource.EntryAutoSelect)]
        AutoSelect,

        /// <summary>
        /// A removable paper bin is used.
        /// </summary>
        [StringResource(StringResource.EntryCassette)]
        Cassette,

        /// <summary>
        /// A tractor feed (also called a pin feed) of continuous-feed paper is used.
        /// </summary>
        [StringResource(StringResource.EntryTractor)]
        Tractor,

        /// <summary>
        /// The automatic sheet feeder is used.
        /// </summary>
        [StringResource(StringResource.EntryAutoSheetFeeder)]
        AutoSheetFeeder,

        /// <summary>
        /// The manual input bin is used.
        /// </summary>
        [StringResource(StringResource.EntryManual)]
        Manual
    }
}
