namespace PrintDialogX.PrintSettings
{
    /// <summary>
    /// Specifies how pages of content are oriented on print media.
    /// </summary>
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

    /// <summary>
    /// Specifies how to print content that contains color or shades of gray.
    /// </summary>
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

    /// <summary>
    /// Specifies the type of output quality for a print device.
    /// </summary>
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

    /// <summary>
    /// Specifies the page size or roll width of the paper or other print media.
    /// </summary>
    public enum PageSize
    {
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

    /// <summary>
    /// Specifies the type of printing paper or other media.
    /// </summary>
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

    /// <summary>
    /// Specifies the number of pages that print on each printed side of a sheet of paper.
    /// </summary>
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

    /// <summary>
    /// Specifies whether a printer uses one-sided printing or some type of two-sided (duplex) printing.
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
        DoubleSidedShortEdge,

        /// <summary>
        /// Output prints on both sides of each sheet, which flips along the edge parallel to <see cref="System.Printing.PrintDocumentImageableArea.MediaSizeHeight"/>.
        /// </summary>
        DoubleSidedLongEdge
    }

    /// <summary>
    /// Specifies the arrangement of pages when more than one page of content appears on a single side of print media.
    /// </summary>
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
