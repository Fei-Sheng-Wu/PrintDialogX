using System.Printing;

namespace PrintDialogX.PrintSettings
{
    public enum PageOrientation
    {
        /// <summary>
        /// Standard orientation.
        /// </summary>
        Portrait,

        /// <summary>
        /// Content of the imageable area is rotated on the page 90 degrees counterclockwise from standard (portrait) orientation.
        /// </summary>
        Landscape
    }

    public enum PageColor
    {
        /// <summary>
        /// Output that prints in color.
        /// </summary>
        Color,

        /// <summary>
        /// Output that prints in a grayscale.
        /// </summary>
        Grayscale,

        /// <summary>
        /// Output that prints in a single color and with the same degree of intensity.
        /// </summary>
        Monochrome
    }

    public enum PageQuality
    {
        /// <summary>
        /// Automatically selects a quality type that is based on the contents of a print job.
        /// </summary>
        Automatic,

        /// <summary>
        /// Draft quality.
        /// </summary>
        Draft,

        /// <summary>
        /// Fax quality.
        /// </summary>
        Fax,

        /// <summary>
        /// Higher than normal quality.
        /// </summary>
        High,

        /// <summary>
        /// Normal quality.
        /// </summary>
        Normal,

        /// <summary>
        /// Photographic quality.
        /// </summary>
        Photographic,

        /// <summary>
        /// Text quality.
        /// </summary>
        Text
    }

    public enum PageSize
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// A0.
        /// </summary>
        ISOA0,

        /// <summary>
        /// A1.
        /// </summary>
        ISOA1,

        /// <summary>
        /// A10.
        /// </summary>
        ISOA10,

        /// <summary>
        /// A2.
        /// </summary>
        ISOA2,

        /// <summary>
        /// A3.
        /// </summary>
        ISOA3,

        /// <summary>
        /// A3 Rotated.
        /// </summary>
        ISOA3Rotated,

        /// <summary>
        /// A3 Extra.
        /// </summary>
        ISOA3Extra,

        /// <summary>
        /// A4.
        /// </summary>
        ISOA4,

        /// <summary>
        /// A4 Rotated.
        /// </summary>
        ISOA4Rotated,

        /// <summary>
        /// A4 Extra.
        /// </summary>
        ISOA4Extra,

        /// <summary>
        /// A5.
        /// </summary>
        ISOA5,

        /// <summary>
        /// A5 Rotated.
        /// </summary>
        ISOA5Rotated,

        /// <summary>
        /// A5 Extra.
        /// </summary>
        ISOA5Extra,

        /// <summary>
        /// A6.
        /// </summary>
        ISOA6,

        /// <summary>
        /// A6 Rotated.
        /// </summary>
        ISOA6Rotated,

        /// <summary>
        /// A7.
        /// </summary>
        ISOA7,

        /// <summary>
        /// A8.
        /// </summary>
        ISOA8,

        /// <summary>
        /// A9.
        /// </summary>
        ISOA9,

        /// <summary>
        /// B0.
        /// </summary>
        ISOB0,

        /// <summary>
        /// B1.
        /// </summary>
        ISOB1,

        /// <summary>
        /// B10.
        /// </summary>
        ISOB10,

        /// <summary>
        /// B2.
        /// </summary>
        ISOB2,

        /// <summary>
        /// B3.
        /// </summary>
        ISOB3,

        /// <summary>
        /// B4.
        /// </summary>
        ISOB4,

        /// <summary>
        /// B4 Envelope.
        /// </summary>
        ISOB4Envelope,

        /// <summary>
        /// B5 Envelope.
        /// </summary>
        ISOB5Envelope,

        /// <summary>
        /// B5 Extra.
        /// </summary>
        ISOB5Extra,

        /// <summary>
        /// B7.
        /// </summary>
        ISOB7,

        /// <summary>
        /// B8.
        /// </summary>
        ISOB8,

        /// <summary>
        /// B9.
        /// </summary>
        ISOB9,

        /// <summary>
        /// C0.
        /// </summary>
        ISOC0,

        /// <summary>
        /// C1.
        /// </summary>
        ISOC1,

        /// <summary>
        /// C10.
        /// </summary>
        ISOC10,

        /// <summary>
        /// C2.
        /// </summary>
        ISOC2,

        /// <summary>
        /// C3.
        /// </summary>
        ISOC3,

        /// <summary>
        /// C3 Envelope.
        /// </summary>
        ISOC3Envelope,

        /// <summary>
        /// C4.
        /// </summary>
        ISOC4,

        /// <summary>
        /// C4 Envelope.
        /// </summary>
        ISOC4Envelope,

        /// <summary>
        /// C5.
        /// </summary>
        ISOC5,

        /// <summary>
        /// C5 Envelope.
        /// </summary>
        ISOC5Envelope,

        /// <summary>
        /// C6.
        /// </summary>
        ISOC6,

        /// <summary>
        /// C6 Envelope.
        /// </summary>
        ISOC6Envelope,

        /// <summary>
        /// C6C5 Envelope.
        /// </summary>
        ISOC6C5Envelope,

        /// <summary>
        /// C7.
        /// </summary>
        ISOC7,

        /// <summary>
        /// C8.
        /// </summary>
        ISOC8,

        /// <summary>
        /// C9.
        /// </summary>
        ISOC9,

        /// <summary>
        /// DL Envelope.
        /// </summary>
        ISODLEnvelope,

        /// <summary>
        /// DL Envelope Rotated.
        /// </summary>
        ISODLEnvelopeRotated,

        /// <summary>
        /// SRA 3.
        /// </summary>
        ISOSRA3,

        /// <summary>
        /// Quadruple Hagaki Postcard.
        /// </summary>
        JapanQuadrupleHagakiPostcard,

        /// <summary>
        /// Japanese Industrial Standard B0.
        /// </summary>
        JISB0,

        /// <summary>
        /// Japanese Industrial Standard B1.
        /// </summary>
        JISB1,

        /// <summary>
        /// Japanese Industrial Standard B10.
        /// </summary>
        JISB10,

        /// <summary>
        /// Japanese Industrial Standard B2.
        /// </summary>
        JISB2,

        /// <summary>
        /// Japanese Industrial Standard B3.
        /// </summary>
        JISB3,

        /// <summary>
        /// Japanese Industrial Standard B4.
        /// </summary>
        JISB4,

        /// <summary>
        /// Japanese Industrial Standard B4 Rotated.
        /// </summary>
        JISB4Rotated,

        /// <summary>
        /// Japanese Industrial Standard B5.
        /// </summary>
        JISB5,

        /// <summary>
        /// Japanese Industrial Standard B5 Rotated.
        /// </summary>
        JISB5Rotated,

        /// <summary>
        /// Japanese Industrial Standard B6.
        /// </summary>
        JISB6,

        /// <summary>
        /// Japanese Industrial Standard B6 Rotated.
        /// </summary>
        JISB6Rotated,

        /// <summary>
        /// Japanese Industrial Standard B7.
        /// </summary>
        JISB7,

        /// <summary>
        /// Japanese Industrial Standard B8.
        /// </summary>
        JISB8,

        /// <summary>
        /// Japanese Industrial Standard B9.
        /// </summary>
        JISB9,

        /// <summary>
        /// Chou 3 Envelope.
        /// </summary>
        JapanChou3Envelope,

        /// <summary>
        /// Chou 3 Envelope Rotated.
        /// </summary>
        JapanChou3EnvelopeRotated,

        /// <summary>
        /// Chou 4 Envelope.
        /// </summary>
        JapanChou4Envelope,

        /// <summary>
        /// Chou 4 Envelope Rotated.
        /// </summary>
        JapanChou4EnvelopeRotated,

        /// <summary>
        /// Hagaki Postcard.
        /// </summary>
        JapanHagakiPostcard,

        /// <summary>
        /// Hagaki Postcard Rotated.
        /// </summary>
        JapanHagakiPostcardRotated,

        /// <summary>
        /// Kaku 2 Envelope.
        /// </summary>
        JapanKaku2Envelope,

        /// <summary>
        /// Kaku 2 Envelope Rotated.
        /// </summary>
        JapanKaku2EnvelopeRotated,

        /// <summary>
        /// Kaku 3 Envelope.
        /// </summary>
        JapanKaku3Envelope,

        /// <summary>
        /// Kaku 3 Envelope Rotated.
        /// </summary>
        JapanKaku3EnvelopeRotated,

        /// <summary>
        /// You 4 Envelope.
        /// </summary>
        JapanYou4Envelope,

        /// <summary>
        /// 10 x 11.
        /// </summary>
        NorthAmerica10x11,

        /// <summary>
        /// 10 x 14.
        /// </summary>
        NorthAmerica10x14,

        /// <summary>
        /// 11 x 17.
        /// </summary>
        NorthAmerica11x17,

        /// <summary>
        /// 9 x 11.
        /// </summary>
        NorthAmerica9x11,

        /// <summary>
        /// Architecture A Sheet.
        /// </summary>
        NorthAmericaArchitectureASheet,

        /// <summary>
        /// Architecture B Sheet.
        /// </summary>
        NorthAmericaArchitectureBSheet,

        /// <summary>
        /// Architecture C Sheet.
        /// </summary>
        NorthAmericaArchitectureCSheet,

        /// <summary>
        /// Architecture D Sheet.
        /// </summary>
        NorthAmericaArchitectureDSheet,

        /// <summary>
        /// Architecture E Sheet.
        /// </summary>
        NorthAmericaArchitectureESheet,

        /// <summary>
        /// C Sheet.
        /// </summary>
        NorthAmericaCSheet,

        /// <summary>
        /// D Sheet.
        /// </summary>
        NorthAmericaDSheet,

        /// <summary>
        /// E Sheet.
        /// </summary>
        NorthAmericaESheet,

        /// <summary>
        /// Executive.
        /// </summary>
        NorthAmericaExecutive,

        /// <summary>
        /// German Legal Fanfold.
        /// </summary>
        NorthAmericaGermanLegalFanfold,

        /// <summary>
        /// German Standard Fanfold.
        /// </summary>
        NorthAmericaGermanStandardFanfold,

        /// <summary>
        /// Legal.
        /// </summary>
        NorthAmericaLegal,

        /// <summary>
        /// Legal Extra.
        /// </summary>
        NorthAmericaLegalExtra,

        /// <summary>
        /// Letter.
        /// </summary>
        NorthAmericaLetter,

        /// <summary>
        /// Letter Rotated.
        /// </summary>
        NorthAmericaLetterRotated,

        /// <summary>
        /// Letter Extra.
        /// </summary>
        NorthAmericaLetterExtra,

        /// <summary>
        /// Letter Plus.
        /// </summary>
        NorthAmericaLetterPlus,

        /// <summary>
        /// Monarch Envelope.
        /// </summary>
        NorthAmericaMonarchEnvelope,

        /// <summary>
        /// Note.
        /// </summary>
        NorthAmericaNote,

        /// <summary>
        /// #10 Envelope.
        /// </summary>
        NorthAmericaNumber10Envelope,

        /// <summary>
        /// #10 Envelope Rotated.
        /// </summary>
        NorthAmericaNumber10EnvelopeRotated,

        /// <summary>
        /// #9 Envelope.
        /// </summary>
        NorthAmericaNumber9Envelope,

        /// <summary>
        /// #11 Envelope.
        /// </summary>
        NorthAmericaNumber11Envelope,

        /// <summary>
        /// #12 Envelope.
        /// </summary>
        NorthAmericaNumber12Envelope,

        /// <summary>
        /// #14 Envelope.
        /// </summary>
        NorthAmericaNumber14Envelope,

        /// <summary>
        /// Personal Envelope.
        /// </summary>
        NorthAmericaPersonalEnvelope,

        /// <summary>
        /// Quarto.
        /// </summary>
        NorthAmericaQuarto,

        /// <summary>
        /// Statement.
        /// </summary>
        NorthAmericaStatement,

        /// <summary>
        /// Super A.
        /// </summary>
        NorthAmericaSuperA,

        /// <summary>
        /// Super B.
        /// </summary>
        NorthAmericaSuperB,

        /// <summary>
        /// Tabloid.
        /// </summary>
        NorthAmericaTabloid,

        /// <summary>
        /// Tabloid Extra.
        /// </summary>
        NorthAmericaTabloidExtra,

        /// <summary>
        /// A4 Plus.
        /// </summary>
        OtherMetricA4Plus,

        /// <summary>
        /// A3 Plus.
        /// </summary>
        OtherMetricA3Plus,

        /// <summary>
        /// Folio.
        /// </summary>
        OtherMetricFolio,

        /// <summary>
        /// Invite Envelope.
        /// </summary>
        OtherMetricInviteEnvelope,

        /// <summary>
        /// Italian Envelope.
        /// </summary>
        OtherMetricItalianEnvelope,

        /// <summary>
        /// People's Republic of China #1 Envelope.
        /// </summary>
        PRC1Envelope,

        /// <summary>
        /// People's Republic of China #1 Envelope Rotated.
        /// </summary>
        PRC1EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #10 Envelope.
        /// </summary>
        PRC10Envelope,

        /// <summary>
        /// People's Republic of China #10 Envelope Rotated.
        /// </summary>
        PRC10EnvelopeRotated,

        /// <summary>
        /// People's Republic of China 16K.
        /// </summary>
        PRC16K,

        /// <summary>
        /// People's Republic of China 16K Rotated.
        /// </summary>
        PRC16KRotated,

        /// <summary>
        /// People's Republic of China #2 Envelope.
        /// </summary>
        PRC2Envelope,

        /// <summary>
        /// People's Republic of China #2 Envelope Rotated.
        /// </summary>
        PRC2EnvelopeRotated,

        /// <summary>
        /// People's Republic of China 32K.
        /// </summary>
        PRC32K,

        /// <summary>
        /// People's Republic of China 32K Rotated.
        /// </summary>
        PRC32KRotated,

        /// <summary>
        /// People's Republic of China 32K Big.
        /// </summary>
        PRC32KBig,

        /// <summary>
        /// People's Republic of China #3 Envelope.
        /// </summary>
        PRC3Envelope,

        /// <summary>
        /// People's Republic of China #3 Envelope Rotated.
        /// </summary>
        PRC3EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #4 Envelope.
        /// </summary>
        PRC4Envelope,

        /// <summary>
        /// People's Republic of China #4 Envelope Rotated.
        /// </summary>
        PRC4EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #5 Envelope.
        /// </summary>
        PRC5Envelope,

        /// <summary>
        /// People's Republic of China #5 Envelope Rotated.
        /// </summary>
        PRC5EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #6 Envelope.
        /// </summary>
        PRC6Envelope,

        /// <summary>
        /// People's Republic of China #6 Envelope Rotated.
        /// </summary>
        PRC6EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #7 Envelope.
        /// </summary>
        PRC7Envelope,

        /// <summary>
        /// People's Republic of China #7 Envelope Rotated.
        /// </summary>
        PRC7EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #8 Envelope.
        /// </summary>
        PRC8Envelope,

        /// <summary>
        /// People's Republic of China #8 Envelope Rotated.
        /// </summary>
        PRC8EnvelopeRotated,

        /// <summary>
        /// People's Republic of China #9 Envelope.
        /// </summary>
        PRC9Envelope,

        /// <summary>
        /// People's Republic of China #9 Envelope Rotated.
        /// </summary>
        PRC9EnvelopeRotated,

        /// <summary>
        /// 4-inch wide roll.
        /// </summary>
        Roll04Inch,

        /// <summary>
        /// 6-inch wide roll.
        /// </summary>
        Roll06Inch,

        /// <summary>
        /// 8-inch wide roll.
        /// </summary>
        Roll08Inch,

        /// <summary>
        /// 12-inch wide roll.
        /// </summary>
        Roll12Inch,

        /// <summary>
        /// 15-inch wide roll.
        /// </summary>
        Roll15Inch,

        /// <summary>
        /// 18-inch wide roll.
        /// </summary>
        Roll18Inch,

        /// <summary>
        /// 22-inch wide roll.
        /// </summary>
        Roll22Inch,

        /// <summary>
        /// 24-inch wide roll.
        /// </summary>
        Roll24Inch,

        /// <summary>
        /// 30-inch wide roll.
        /// </summary>
        Roll30Inch,

        /// <summary>
        /// 36-inch wide roll.
        /// </summary>
        Roll36Inch,

        /// <summary>
        /// 54-inch wide roll.
        /// </summary>
        Roll54Inch,

        /// <summary>
        /// Double Hagaki Postcard.
        /// </summary>
        JapanDoubleHagakiPostcard,

        /// <summary>
        /// Double Hagaki Postcard Rotated.
        /// </summary>
        JapanDoubleHagakiPostcardRotated,

        /// <summary>
        /// L Photo.
        /// </summary>
        JapanLPhoto,

        /// <summary>
        /// 2L Photo.
        /// </summary>
        Japan2LPhoto,

        /// <summary>
        /// You 1 Envelope.
        /// </summary>
        JapanYou1Envelope,

        /// <summary>
        /// You 2 Envelope.
        /// </summary>
        JapanYou2Envelope,

        /// <summary>
        /// You 3 Envelope.
        /// </summary>
        JapanYou3Envelope,

        /// <summary>
        /// You 4 Envelope Rotated.
        /// </summary>
        JapanYou4EnvelopeRotated,

        /// <summary>
        /// You 6 Envelope.
        /// </summary>
        JapanYou6Envelope,

        /// <summary>
        /// You 6 Envelope Rotated.
        /// </summary>
        JapanYou6EnvelopeRotated,

        /// <summary>
        /// 4 x 6.
        /// </summary>
        NorthAmerica4x6,

        /// <summary>
        /// 4 x 8.
        /// </summary>
        NorthAmerica4x8,

        /// <summary>
        /// 5 x 7.
        /// </summary>
        NorthAmerica5x7,

        /// <summary>
        /// 8 x 10.
        /// </summary>
        NorthAmerica8x10,

        /// <summary>
        /// 10 x 12.
        /// </summary>
        NorthAmerica10x12,

        /// <summary>
        /// 14 x 17.
        /// </summary>
        NorthAmerica14x17,

        /// <summary>
        /// Business card.
        /// </summary>
        BusinessCard,

        /// <summary>
        /// Credit card.
        /// </summary>
        CreditCard
    }

    public enum PageType
    {
        /// <summary>
        /// The print device selects the media.
        /// </summary>
        AutoSelect,

        /// <summary>
        /// Archive-quality media.
        /// </summary>
        Archival,

        /// <summary>
        /// Specialty back-printing film.
        /// </summary>
        BackPrintFilm,

        /// <summary>
        /// Standard bond media.
        /// </summary>
        Bond,

        /// <summary>
        /// Standard card stock.
        /// </summary>
        CardStock,

        /// <summary>
        /// Continuous-feed media.
        /// </summary>
        Continuous,

        /// <summary>
        /// Standard envelope.
        /// </summary>
        EnvelopePlain,

        /// <summary>
        /// Window envelope.
        /// </summary>
        EnvelopeWindow,

        /// <summary>
        /// Fabric media.
        /// </summary>
        Fabric,

        /// <summary>
        /// Specialty high-resolution media.
        /// </summary>
        HighResolution,

        /// <summary>
        /// Label media.
        /// </summary>
        Label,

        /// <summary>
        /// Attached multipart forms.
        /// </summary>
        MultiLayerForm,

        /// <summary>
        /// Individual multipart forms.
        /// </summary>
        MultiPartForm,

        /// <summary>
        /// Standard photographic media.
        /// </summary>
        Photographic,

        /// <summary>
        /// Film photographic media.
        /// </summary>
        PhotographicFilm,

        /// <summary>
        /// Glossy photographic media.
        /// </summary>
        PhotographicGlossy,

        /// <summary>
        /// High-gloss photographic media.
        /// </summary>
        PhotographicHighGloss,

        /// <summary>
        /// Matte photographic media.
        /// </summary>
        PhotographicMatte,

        /// <summary>
        /// Satin photographic media.
        /// </summary>
        PhotographicSatin,

        /// <summary>
        /// Semi-gloss photographic media.
        /// </summary>
        PhotographicSemiGloss,

        /// <summary>
        /// Plain paper.
        /// </summary>
        Plain,

        /// <summary>
        /// Output to a display in continuous form.
        /// </summary>
        Screen,

        /// <summary>
        /// Output to a display in paged form.
        /// </summary>
        ScreenPaged,

        /// <summary>
        /// Specialty stationary.
        /// </summary>
        Stationery,

        /// <summary>
        /// Tab stock, not precut (single tabs).
        /// </summary>
        TabStockFull,

        /// <summary>
        /// Tab stock, precut (multiple tabs).
        /// </summary>
        TabStockPreCut,

        /// <summary>
        /// Transparent sheet.
        /// </summary>
        Transparency,

        /// <summary>
        /// Media that is used to transfer an image to a T-shirt.
        /// </summary>
        TShirtTransfer
    }

    public enum PagesPerSheet
    {
        /// <summary>
        /// 1 page per sheet.
        /// </summary>
        One,

        /// <summary>
        /// 2 pages per sheet.
        /// </summary>
        Two,

        /// <summary>
        /// 4 pages per sheet.
        /// </summary>
        Four,

        /// <summary>
        /// 6 pages per sheet.
        /// </summary>
        Six,

        /// <summary>
        /// 9 pages per sheet.
        /// </summary>
        Nine,

        /// <summary>
        /// 16 pages per sheet.
        /// </summary>
        Sixteen
    }

    public enum DoubleSided
    {
        /// <summary>
        /// Output prints on only one side of each sheet.
        /// </summary>
        OneSided,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="PrintDocumentImageableArea.MediaSizeWidth"/>.
        /// </summary>
        DoubleSidedShortEdge,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="PrintDocumentImageableArea.MediaSizeHeight"/>.
        /// </summary>
        DoubleSidedLongEdge
    }

    public enum PageOrder
    {
        /// <summary>
        /// Pages appear in rows, from left to right and top to bottom relative to page orientation.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Pages appear in rows, from right to left and top to bottom relative to page orientation.
        /// </summary>
        HorizontalReverse,

        /// <summary>
        /// Pages appear in columns, from top to bottom and left to right relative to page orientation.
        /// </summary>
        Vertical,

        /// <summary>
        /// Pages appear in columns, from bottom to top and left to right relative to page orientation.
        /// </summary>
        VerticalReverse
    }
}

namespace PrintDialogX.PrintSettings.SettingsHepler
{
    public class NameInfoHepler
    {
        protected NameInfoHepler()
        {
            return;
        }

        /// <summary>
        /// Get the name info of the <see cref="PageMediaSizeName"/> object.
        /// </summary>
        public static string GetPageMediaSizeNameInfo(PageMediaSizeName sizeName)
        {
            return sizeName switch
            {
                PageMediaSizeName.BusinessCard => "Business Card",
                PageMediaSizeName.CreditCard => "Credit Card",
                PageMediaSizeName.ISOA0 => "ISO A0",
                PageMediaSizeName.ISOA1 => "ISO A1",
                PageMediaSizeName.ISOA10 => "ISO A10",
                PageMediaSizeName.ISOA2 => "ISO A2",
                PageMediaSizeName.ISOA3 => "ISO A3",
                PageMediaSizeName.ISOA3Extra => "ISO A3 Extra",
                PageMediaSizeName.ISOA3Rotated => "ISO A3 Rotated",
                PageMediaSizeName.ISOA4 => "ISO A4",
                PageMediaSizeName.ISOA4Extra => "ISO A4 Extra",
                PageMediaSizeName.ISOA4Rotated => "ISO A4 Rotated",
                PageMediaSizeName.ISOA5 => "ISO A5",
                PageMediaSizeName.ISOA5Extra => "ISO A5 Extra",
                PageMediaSizeName.ISOA5Rotated => "ISO A5 Rotated",
                PageMediaSizeName.ISOA6 => "ISO A6",
                PageMediaSizeName.ISOA6Rotated => "ISO A6 Rotated",
                PageMediaSizeName.ISOA7 => "ISO A7",
                PageMediaSizeName.ISOA8 => "ISO A8",
                PageMediaSizeName.ISOA9 => "ISO A9",
                PageMediaSizeName.ISOB0 => "ISO B0",
                PageMediaSizeName.ISOB1 => "ISO B1",
                PageMediaSizeName.ISOB10 => "ISO B10",
                PageMediaSizeName.ISOB2 => "ISO B2",
                PageMediaSizeName.ISOB3 => "ISO B3",
                PageMediaSizeName.ISOB4 => "ISO B4",
                PageMediaSizeName.ISOB4Envelope => "ISO B4 Envelope",
                PageMediaSizeName.ISOB5Envelope => "ISO B5 Envelope",
                PageMediaSizeName.ISOB5Extra => "ISO B5 Extra",
                PageMediaSizeName.ISOB7 => "ISO B7",
                PageMediaSizeName.ISOB8 => "ISO B8",
                PageMediaSizeName.ISOB9 => "ISO B9",
                PageMediaSizeName.ISOC0 => "ISO C0",
                PageMediaSizeName.ISOC1 => "ISO C1",
                PageMediaSizeName.ISOC2 => "ISO C2",
                PageMediaSizeName.ISOC3 => "ISO C3",
                PageMediaSizeName.ISOC3Envelope => "ISO C3 Envelope",
                PageMediaSizeName.ISOC4 => "ISO C4",
                PageMediaSizeName.ISOC4Envelope => "ISO C4 Envelope",
                PageMediaSizeName.ISOC5 => "ISO C5",
                PageMediaSizeName.ISOC5Envelope => "ISO C5 Envelope",
                PageMediaSizeName.ISOC6 => "ISO C6",
                PageMediaSizeName.ISOC6C5Envelope => "ISO C6C5 Envelope",
                PageMediaSizeName.ISOC6Envelope => "ISO C6 Envelope",
                PageMediaSizeName.ISOC7 => "ISO C7",
                PageMediaSizeName.ISOC8 => "ISO C8",
                PageMediaSizeName.ISOC9 => "ISO C9",
                PageMediaSizeName.ISOC10 => "ISO C10",
                PageMediaSizeName.ISODLEnvelope => "ISO DL Envelope",
                PageMediaSizeName.ISODLEnvelopeRotated => "ISO DL Envelope Rotated",
                PageMediaSizeName.ISOSRA3 => "ISO SRA3",
                PageMediaSizeName.Japan2LPhoto => "Japan 2L Photo",
                PageMediaSizeName.JapanChou3Envelope => "Japan Chou 3 Envelope",
                PageMediaSizeName.JapanChou3EnvelopeRotated => "Japan Chou 3 Envelope Rotated",
                PageMediaSizeName.JapanChou4Envelope => "Japan Chou 4 Envelope",
                PageMediaSizeName.JapanChou4EnvelopeRotated => "Japan Chou 4 Envelope Rotated",
                PageMediaSizeName.JapanDoubleHagakiPostcard => "Japan Double Hagaki Postcard",
                PageMediaSizeName.JapanDoubleHagakiPostcardRotated => "Japan Double Hagaki Postcard Rotated",
                PageMediaSizeName.JapanHagakiPostcard => "Japan Hagaki Postcard",
                PageMediaSizeName.JapanHagakiPostcardRotated => "Japan Hagaki Postcard Rotated",
                PageMediaSizeName.JapanKaku2Envelope => "Japan Kaku 2 Envelope",
                PageMediaSizeName.JapanKaku2EnvelopeRotated => "Japan Kaku 2 Envelope Rotated",
                PageMediaSizeName.JapanKaku3Envelope => "Japan Kaku 3 Envelope",
                PageMediaSizeName.JapanKaku3EnvelopeRotated => "Japan Kaku 3 Envelope Rotated",
                PageMediaSizeName.JapanLPhoto => "Japan L Photo",
                PageMediaSizeName.JapanQuadrupleHagakiPostcard => "Japan Quadruple Hagaki Postcard",
                PageMediaSizeName.JapanYou1Envelope => "Japan You 1 Envelope",
                PageMediaSizeName.JapanYou2Envelope => "Japan You 2 Envelope",
                PageMediaSizeName.JapanYou3Envelope => "Japan You 3 Envelope",
                PageMediaSizeName.JapanYou4Envelope => "Japan You 4 Envelope",
                PageMediaSizeName.JapanYou4EnvelopeRotated => "Japan You 4 Envelope Rotated",
                PageMediaSizeName.JapanYou6Envelope => "Japan You 6 Envelope",
                PageMediaSizeName.JapanYou6EnvelopeRotated => "Japan You 6 Envelope Rotated",
                PageMediaSizeName.JISB0 => "JIS B0",
                PageMediaSizeName.JISB1 => "JIS B1",
                PageMediaSizeName.JISB10 => "JIS B10",
                PageMediaSizeName.JISB2 => "JIS B2",
                PageMediaSizeName.JISB3 => "JIS B3",
                PageMediaSizeName.JISB4 => "JIS B4",
                PageMediaSizeName.JISB4Rotated => "JIS B4 Rotated",
                PageMediaSizeName.JISB5 => "JIS B5",
                PageMediaSizeName.JISB5Rotated => "JIS B5 Rotated",
                PageMediaSizeName.JISB6 => "JIS B6",
                PageMediaSizeName.JISB6Rotated => "JIS B6 Rotated",
                PageMediaSizeName.JISB7 => "JIS B7",
                PageMediaSizeName.JISB8 => "JIS B8",
                PageMediaSizeName.JISB9 => "JIS B9",
                PageMediaSizeName.NorthAmerica10x11 => "North America 10 x 11",
                PageMediaSizeName.NorthAmerica10x12 => "North America 10 x 12",
                PageMediaSizeName.NorthAmerica10x14 => "North America 10 x 14",
                PageMediaSizeName.NorthAmerica11x17 => "North America 11 x 17",
                PageMediaSizeName.NorthAmerica14x17 => "North America 14 x 17",
                PageMediaSizeName.NorthAmerica4x6 => "North America 4 x 6",
                PageMediaSizeName.NorthAmerica4x8 => "North America 4 x 8",
                PageMediaSizeName.NorthAmerica5x7 => "North America 5 x 7",
                PageMediaSizeName.NorthAmerica8x10 => "North America 8 x 10",
                PageMediaSizeName.NorthAmerica9x11 => "North America 9 x 11",
                PageMediaSizeName.NorthAmericaArchitectureASheet => "North America Architecture A Sheet",
                PageMediaSizeName.NorthAmericaArchitectureBSheet => "North America Architecture B Sheet",
                PageMediaSizeName.NorthAmericaArchitectureCSheet => "North America Architecture C Sheet",
                PageMediaSizeName.NorthAmericaArchitectureDSheet => "North America Architecture D Sheet",
                PageMediaSizeName.NorthAmericaArchitectureESheet => "North America Architecture E Sheet",
                PageMediaSizeName.NorthAmericaCSheet => "North America C Sheet",
                PageMediaSizeName.NorthAmericaDSheet => "North America D Sheet",
                PageMediaSizeName.NorthAmericaESheet => "North America E Sheet",
                PageMediaSizeName.NorthAmericaExecutive => "North America Executive",
                PageMediaSizeName.NorthAmericaGermanLegalFanfold => "North America German Legal Fanfold",
                PageMediaSizeName.NorthAmericaGermanStandardFanfold => "North America German Standard Fanfold",
                PageMediaSizeName.NorthAmericaLegal => "North America Legal",
                PageMediaSizeName.NorthAmericaLegalExtra => "North America Legal Extra",
                PageMediaSizeName.NorthAmericaLetter => "North America Letter",
                PageMediaSizeName.NorthAmericaLetterExtra => "North America Letter Extra",
                PageMediaSizeName.NorthAmericaLetterPlus => "North America Letter Plus",
                PageMediaSizeName.NorthAmericaLetterRotated => "North America Letter Rotated",
                PageMediaSizeName.NorthAmericaMonarchEnvelope => "North America Monarch Envelope",
                PageMediaSizeName.NorthAmericaNote => "North America Note",
                PageMediaSizeName.NorthAmericaNumber10Envelope => "North America Number 10 Envelope",
                PageMediaSizeName.NorthAmericaNumber10EnvelopeRotated => "North America Number 10 Envelope Rotated",
                PageMediaSizeName.NorthAmericaNumber11Envelope => "North America Number 11 Envelope",
                PageMediaSizeName.NorthAmericaNumber12Envelope => "North America Number 12 Envelope",
                PageMediaSizeName.NorthAmericaNumber14Envelope => "North America Number 14 Envelope",
                PageMediaSizeName.NorthAmericaNumber9Envelope => "North America Number 9 Envelope",
                PageMediaSizeName.NorthAmericaPersonalEnvelope => "North America Personal Envelope",
                PageMediaSizeName.NorthAmericaQuarto => "North America Quarto",
                PageMediaSizeName.NorthAmericaStatement => "North America Statement",
                PageMediaSizeName.NorthAmericaSuperA => "North America Super A",
                PageMediaSizeName.NorthAmericaSuperB => "North America Super B",
                PageMediaSizeName.NorthAmericaTabloid => "North America Tabloid",
                PageMediaSizeName.NorthAmericaTabloidExtra => "North America Tabloid Extra",
                PageMediaSizeName.OtherMetricA3Plus => "A3 Plus",
                PageMediaSizeName.OtherMetricA4Plus => "A4 Plus",
                PageMediaSizeName.OtherMetricFolio => "Folio",
                PageMediaSizeName.OtherMetricInviteEnvelope => "Invite Envelope",
                PageMediaSizeName.OtherMetricItalianEnvelope => "Italian Envelope",
                PageMediaSizeName.PRC10Envelope => "PRC #10 Envelope",
                PageMediaSizeName.PRC10EnvelopeRotated => "PRC #10 Envelope Rotated",
                PageMediaSizeName.PRC16K => "PRC 16K",
                PageMediaSizeName.PRC16KRotated => "PRC 16K Rotated",
                PageMediaSizeName.PRC1Envelope => "PRC #1 Envelope",
                PageMediaSizeName.PRC1EnvelopeRotated => "PRC #1 Envelope Rotated",
                PageMediaSizeName.PRC2Envelope => "PRC #2 Envelope",
                PageMediaSizeName.PRC2EnvelopeRotated => "PRC #2 Envelope Rotated",
                PageMediaSizeName.PRC32K => "PRC 32K",
                PageMediaSizeName.PRC32KBig => "PRC 32K Big",
                PageMediaSizeName.PRC32KRotated => "PRC 32K Rotated",
                PageMediaSizeName.PRC3Envelope => "PRC #3 Envelope",
                PageMediaSizeName.PRC3EnvelopeRotated => "PRC #3 Envelope Rotated",
                PageMediaSizeName.PRC4Envelope => "PRC #4 Envelope",
                PageMediaSizeName.PRC4EnvelopeRotated => "PRC #4 Envelope Rotated",
                PageMediaSizeName.PRC5Envelope => "PRC #5 Envelope",
                PageMediaSizeName.PRC5EnvelopeRotated => "PRC #5 Envelope Rotated",
                PageMediaSizeName.PRC6Envelope => "PRC #6 Envelope",
                PageMediaSizeName.PRC6EnvelopeRotated => "PRC #6 Envelope Rotated",
                PageMediaSizeName.PRC7Envelope => "PRC #7 Envelope",
                PageMediaSizeName.PRC7EnvelopeRotated => "PRC #7 Envelope Rotated",
                PageMediaSizeName.PRC8Envelope => "PRC #8 Envelope",
                PageMediaSizeName.PRC8EnvelopeRotated => "PRC #8 Envelope Rotated",
                PageMediaSizeName.PRC9Envelope => "PRC #9 Envelope",
                PageMediaSizeName.PRC9EnvelopeRotated => "PRC #9 Envelope Rotated",
                PageMediaSizeName.Roll04Inch => "4-inch Wide Roll",
                PageMediaSizeName.Roll06Inch => "6-inch Wide Roll",
                PageMediaSizeName.Roll08Inch => "8-inch Wide Roll",
                PageMediaSizeName.Roll12Inch => "12-inch Wide Roll",
                PageMediaSizeName.Roll15Inch => "15-inch Wide Roll",
                PageMediaSizeName.Roll18Inch => "18-inch Wide Roll",
                PageMediaSizeName.Roll22Inch => "22-inch Wide Roll",
                PageMediaSizeName.Roll24Inch => "24-inch Wide Roll",
                PageMediaSizeName.Roll30Inch => "30-inch Wide Roll",
                PageMediaSizeName.Roll36Inch => "36-inch Wide Roll",
                PageMediaSizeName.Roll54Inch => "54-inch Wide Roll",

                _ => "Unknown Size",
            };
        }

        /// <summary>
        /// Get the name info of the <see cref="PageSize"/> object.
        /// </summary>
        public static string GetPageMediaSizeNameInfo(PageSize sizeName)
        {
            return sizeName switch
            {
                PageSize.BusinessCard => "Business Card",
                PageSize.CreditCard => "Credit Card",
                PageSize.ISOA0 => "ISO A0",
                PageSize.ISOA1 => "ISO A1",
                PageSize.ISOA10 => "ISO A10",
                PageSize.ISOA2 => "ISO A2",
                PageSize.ISOA3 => "ISO A3",
                PageSize.ISOA3Extra => "ISO A3 Extra",
                PageSize.ISOA3Rotated => "ISO A3 Rotated",
                PageSize.ISOA4 => "ISO A4",
                PageSize.ISOA4Extra => "ISO A4 Extra",
                PageSize.ISOA4Rotated => "ISO A4 Rotated",
                PageSize.ISOA5 => "ISO A5",
                PageSize.ISOA5Extra => "ISO A5 Extra",
                PageSize.ISOA5Rotated => "ISO A5 Rotated",
                PageSize.ISOA6 => "ISO A6",
                PageSize.ISOA6Rotated => "ISO A6 Rotated",
                PageSize.ISOA7 => "ISO A7",
                PageSize.ISOA8 => "ISO A8",
                PageSize.ISOA9 => "ISO A9",
                PageSize.ISOB0 => "ISO B0",
                PageSize.ISOB1 => "ISO B1",
                PageSize.ISOB10 => "ISO B10",
                PageSize.ISOB2 => "ISO B2",
                PageSize.ISOB3 => "ISO B3",
                PageSize.ISOB4 => "ISO B4",
                PageSize.ISOB4Envelope => "ISO B4 Envelope",
                PageSize.ISOB5Envelope => "ISO B5 Envelope",
                PageSize.ISOB5Extra => "ISO B5 Extra",
                PageSize.ISOB7 => "ISO B7",
                PageSize.ISOB8 => "ISO B8",
                PageSize.ISOB9 => "ISO B9",
                PageSize.ISOC0 => "ISO C0",
                PageSize.ISOC1 => "ISO C1",
                PageSize.ISOC2 => "ISO C2",
                PageSize.ISOC3 => "ISO C3",
                PageSize.ISOC3Envelope => "ISO C3 Envelope",
                PageSize.ISOC4 => "ISO C4",
                PageSize.ISOC4Envelope => "ISO C4 Envelope",
                PageSize.ISOC5 => "ISO C5",
                PageSize.ISOC5Envelope => "ISO C5 Envelope",
                PageSize.ISOC6 => "ISO C6",
                PageSize.ISOC6C5Envelope => "ISO C6C5 Envelope",
                PageSize.ISOC6Envelope => "ISO C6 Envelope",
                PageSize.ISOC7 => "ISO C7",
                PageSize.ISOC8 => "ISO C8",
                PageSize.ISOC9 => "ISO C9",
                PageSize.ISOC10 => "ISO C10",
                PageSize.ISODLEnvelope => "ISO DL Envelope",
                PageSize.ISODLEnvelopeRotated => "ISO DL Envelope Rotated",
                PageSize.ISOSRA3 => "ISO SRA3",
                PageSize.Japan2LPhoto => "Japan 2L Photo",
                PageSize.JapanChou3Envelope => "Japan Chou 3 Envelope",
                PageSize.JapanChou3EnvelopeRotated => "Japan Chou 3 Envelope Rotated",
                PageSize.JapanChou4Envelope => "Japan Chou 4 Envelope",
                PageSize.JapanChou4EnvelopeRotated => "Japan Chou 4 Envelope Rotated",
                PageSize.JapanDoubleHagakiPostcard => "Japan Double Hagaki Postcard",
                PageSize.JapanDoubleHagakiPostcardRotated => "Japan Double Hagaki Postcard Rotated",
                PageSize.JapanHagakiPostcard => "Japan Hagaki Postcard",
                PageSize.JapanHagakiPostcardRotated => "Japan Hagaki Postcard Rotated",
                PageSize.JapanKaku2Envelope => "Japan Kaku 2 Envelope",
                PageSize.JapanKaku2EnvelopeRotated => "Japan Kaku 2 Envelope Rotated",
                PageSize.JapanKaku3Envelope => "Japan Kaku 3 Envelope",
                PageSize.JapanKaku3EnvelopeRotated => "Japan Kaku 3 Envelope Rotated",
                PageSize.JapanLPhoto => "Japan L Photo",
                PageSize.JapanQuadrupleHagakiPostcard => "Japan Quadruple Hagaki Postcard",
                PageSize.JapanYou1Envelope => "Japan You 1 Envelope",
                PageSize.JapanYou2Envelope => "Japan You 2 Envelope",
                PageSize.JapanYou3Envelope => "Japan You 3 Envelope",
                PageSize.JapanYou4Envelope => "Japan You 4 Envelope",
                PageSize.JapanYou4EnvelopeRotated => "Japan You 4 Envelope Rotated",
                PageSize.JapanYou6Envelope => "Japan You 6 Envelope",
                PageSize.JapanYou6EnvelopeRotated => "Japan You 6 Envelope Rotated",
                PageSize.JISB0 => "JIS B0",
                PageSize.JISB1 => "JIS B1",
                PageSize.JISB10 => "JIS B10",
                PageSize.JISB2 => "JIS B2",
                PageSize.JISB3 => "JIS B3",
                PageSize.JISB4 => "JIS B4",
                PageSize.JISB4Rotated => "JIS B4 Rotated",
                PageSize.JISB5 => "JIS B5",
                PageSize.JISB5Rotated => "JIS B5 Rotated",
                PageSize.JISB6 => "JIS B6",
                PageSize.JISB6Rotated => "JIS B6 Rotated",
                PageSize.JISB7 => "JIS B7",
                PageSize.JISB8 => "JIS B8",
                PageSize.JISB9 => "JIS B9",
                PageSize.NorthAmerica10x11 => "North America 10 x 11",
                PageSize.NorthAmerica10x12 => "North America 10 x 12",
                PageSize.NorthAmerica10x14 => "North America 10 x 14",
                PageSize.NorthAmerica11x17 => "North America 11 x 17",
                PageSize.NorthAmerica14x17 => "North America 14 x 17",
                PageSize.NorthAmerica4x6 => "North America 4 x 6",
                PageSize.NorthAmerica4x8 => "North America 4 x 8",
                PageSize.NorthAmerica5x7 => "North America 5 x 7",
                PageSize.NorthAmerica8x10 => "North America 8 x 10",
                PageSize.NorthAmerica9x11 => "North America 9 x 11",
                PageSize.NorthAmericaArchitectureASheet => "North America Architecture A Sheet",
                PageSize.NorthAmericaArchitectureBSheet => "North America Architecture B Sheet",
                PageSize.NorthAmericaArchitectureCSheet => "North America Architecture C Sheet",
                PageSize.NorthAmericaArchitectureDSheet => "North America Architecture D Sheet",
                PageSize.NorthAmericaArchitectureESheet => "North America Architecture E Sheet",
                PageSize.NorthAmericaCSheet => "North America C Sheet",
                PageSize.NorthAmericaDSheet => "North America D Sheet",
                PageSize.NorthAmericaESheet => "North America E Sheet",
                PageSize.NorthAmericaExecutive => "North America Executive",
                PageSize.NorthAmericaGermanLegalFanfold => "North America German Legal Fanfold",
                PageSize.NorthAmericaGermanStandardFanfold => "North America German Standard Fanfold",
                PageSize.NorthAmericaLegal => "North America Legal",
                PageSize.NorthAmericaLegalExtra => "North America Legal Extra",
                PageSize.NorthAmericaLetter => "North America Letter",
                PageSize.NorthAmericaLetterExtra => "North America Letter Extra",
                PageSize.NorthAmericaLetterPlus => "North America Letter Plus",
                PageSize.NorthAmericaLetterRotated => "North America Letter Rotated",
                PageSize.NorthAmericaMonarchEnvelope => "North America Monarch Envelope",
                PageSize.NorthAmericaNote => "North America Note",
                PageSize.NorthAmericaNumber10Envelope => "North America Number 10 Envelope",
                PageSize.NorthAmericaNumber10EnvelopeRotated => "North America Number 10 Envelope Rotated",
                PageSize.NorthAmericaNumber11Envelope => "North America Number 11 Envelope",
                PageSize.NorthAmericaNumber12Envelope => "North America Number 12 Envelope",
                PageSize.NorthAmericaNumber14Envelope => "North America Number 14 Envelope",
                PageSize.NorthAmericaNumber9Envelope => "North America Number 9 Envelope",
                PageSize.NorthAmericaPersonalEnvelope => "North America Personal Envelope",
                PageSize.NorthAmericaQuarto => "North America Quarto",
                PageSize.NorthAmericaStatement => "North America Statement",
                PageSize.NorthAmericaSuperA => "North America Super A",
                PageSize.NorthAmericaSuperB => "North America Super B",
                PageSize.NorthAmericaTabloid => "North America Tabloid",
                PageSize.NorthAmericaTabloidExtra => "North America Tabloid Extra",
                PageSize.OtherMetricA3Plus => "A3 Plus",
                PageSize.OtherMetricA4Plus => "A4 Plus",
                PageSize.OtherMetricFolio => "Folio",
                PageSize.OtherMetricInviteEnvelope => "Invite Envelope",
                PageSize.OtherMetricItalianEnvelope => "Italian Envelope",
                PageSize.PRC10Envelope => "PRC #10 Envelope",
                PageSize.PRC10EnvelopeRotated => "PRC #10 Envelope Rotated",
                PageSize.PRC16K => "PRC 16K",
                PageSize.PRC16KRotated => "PRC 16K Rotated",
                PageSize.PRC1Envelope => "PRC #1 Envelope",
                PageSize.PRC1EnvelopeRotated => "PRC #1 Envelope Rotated",
                PageSize.PRC2Envelope => "PRC #2 Envelope",
                PageSize.PRC2EnvelopeRotated => "PRC #2 Envelope Rotated",
                PageSize.PRC32K => "PRC 32K",
                PageSize.PRC32KBig => "PRC 32K Big",
                PageSize.PRC32KRotated => "PRC 32K Rotated",
                PageSize.PRC3Envelope => "PRC #3 Envelope",
                PageSize.PRC3EnvelopeRotated => "PRC #3 Envelope Rotated",
                PageSize.PRC4Envelope => "PRC #4 Envelope",
                PageSize.PRC4EnvelopeRotated => "PRC #4 Envelope Rotated",
                PageSize.PRC5Envelope => "PRC #5 Envelope",
                PageSize.PRC5EnvelopeRotated => "PRC #5 Envelope Rotated",
                PageSize.PRC6Envelope => "PRC #6 Envelope",
                PageSize.PRC6EnvelopeRotated => "PRC #6 Envelope Rotated",
                PageSize.PRC7Envelope => "PRC #7 Envelope",
                PageSize.PRC7EnvelopeRotated => "PRC #7 Envelope Rotated",
                PageSize.PRC8Envelope => "PRC #8 Envelope",
                PageSize.PRC8EnvelopeRotated => "PRC #8 Envelope Rotated",
                PageSize.PRC9Envelope => "PRC #9 Envelope",
                PageSize.PRC9EnvelopeRotated => "PRC #9 Envelope Rotated",
                PageSize.Roll04Inch => "4-inch Wide Roll",
                PageSize.Roll06Inch => "6-inch Wide Roll",
                PageSize.Roll08Inch => "8-inch Wide Roll",
                PageSize.Roll12Inch => "12-inch Wide Roll",
                PageSize.Roll15Inch => "15-inch Wide Roll",
                PageSize.Roll18Inch => "18-inch Wide Roll",
                PageSize.Roll22Inch => "22-inch Wide Roll",
                PageSize.Roll24Inch => "24-inch Wide Roll",
                PageSize.Roll30Inch => "30-inch Wide Roll",
                PageSize.Roll36Inch => "36-inch Wide Roll",
                PageSize.Roll54Inch => "54-inch Wide Roll",

                _ => "Unknown Size",
            };
        }

        /// <summary>
        /// Get the name info of the <see cref="PageMediaType"/> object.
        /// </summary>
        public static string GetPageMediaTypeNameInfo(PageMediaType type)
        {
            return type switch
            {
                PageMediaType.Archival => "Archival",
                PageMediaType.AutoSelect => "Auto Select",
                PageMediaType.BackPrintFilm => "Back Print Film",
                PageMediaType.Bond => "Bond",
                PageMediaType.CardStock => "Card Stock",
                PageMediaType.Continuous => "Continuous",
                PageMediaType.EnvelopePlain => "Envelope Plain",
                PageMediaType.EnvelopeWindow => "Envelope Window",
                PageMediaType.Fabric => "Fabric",
                PageMediaType.HighResolution => "High Resolution",
                PageMediaType.Label => "Label",
                PageMediaType.MultiLayerForm => "Multi Layer Form",
                PageMediaType.MultiPartForm => "Multi Part Form",
                PageMediaType.Photographic => "Photographic",
                PageMediaType.PhotographicFilm => "Photographic Film",
                PageMediaType.PhotographicGlossy => "Photographic Glossy",
                PageMediaType.PhotographicHighGloss => "Photographic High Gloss",
                PageMediaType.PhotographicMatte => "Photographic Matte",
                PageMediaType.PhotographicSatin => "Photographic Satin",
                PageMediaType.PhotographicSemiGloss => "Photographic Semi Gloss",
                PageMediaType.Plain => "Plain",
                PageMediaType.Screen => "Screen",
                PageMediaType.ScreenPaged => "Screen Paged",
                PageMediaType.Stationery => "Stationery",
                PageMediaType.TabStockFull => "Tab Stock Full",
                PageMediaType.TabStockPreCut => "Tab Stock Pre Cut",
                PageMediaType.Transparency => "Transparency",
                PageMediaType.TShirtTransfer => "T-shirt Transfer",

                _ => "Unknown Type",
            };
        }

        /// <summary>
        /// Get the name info of the <see cref="PageType"/> object.
        /// </summary>
        public static string GetPageMediaTypeNameInfo(PageType type)
        {
            return type switch
            {
                PageType.Archival => "Archival",
                PageType.AutoSelect => "Auto Select",
                PageType.BackPrintFilm => "Back Print Film",
                PageType.Bond => "Bond",
                PageType.CardStock => "Card Stock",
                PageType.Continuous => "Continuous",
                PageType.EnvelopePlain => "Envelope Plain",
                PageType.EnvelopeWindow => "Envelope Window",
                PageType.Fabric => "Fabric",
                PageType.HighResolution => "High Resolution",
                PageType.Label => "Label",
                PageType.MultiLayerForm => "Multi Layer Form",
                PageType.MultiPartForm => "Multi Part Form",
                PageType.Photographic => "Photographic",
                PageType.PhotographicFilm => "Photographic Film",
                PageType.PhotographicGlossy => "Photographic Glossy",
                PageType.PhotographicHighGloss => "Photographic High Gloss",
                PageType.PhotographicMatte => "Photographic Matte",
                PageType.PhotographicSatin => "Photographic Satin",
                PageType.PhotographicSemiGloss => "Photographic Semi Gloss",
                PageType.Plain => "Plain",
                PageType.Screen => "Screen",
                PageType.ScreenPaged => "Screen Paged",
                PageType.Stationery => "Stationery",
                PageType.TabStockFull => "Tab Stock Full",
                PageType.TabStockPreCut => "Tab Stock Pre Cut",
                PageType.Transparency => "Transparency",
                PageType.TShirtTransfer => "T-shirt Transfer",

                _ => "Unknown Type",
            };
        }

        /// <summary>
        /// Get the name info of <see cref="InputBin"/> object.
        /// </summary>
        /// <param name="inputBin">The <see cref="InputBin"/> object of page source.</param>
        /// <returns>The name info.</returns>
        public static string GetInputBinNameInfo(InputBin inputBin)
        {
            return inputBin switch
            {
                InputBin.AutoSelect => "Auto Select",
                InputBin.AutoSheetFeeder => "Auto Sheet Feeder",
                InputBin.Cassette => "Cassette",
                InputBin.Manual => "Manual",
                InputBin.Tractor => "Tractor",

                _ => "Unknown Input Bin",
            };
        }
    }
}