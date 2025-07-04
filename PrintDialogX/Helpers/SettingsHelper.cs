using System.Printing;

namespace PrintDialogX.Internal
{
    internal class SettingsHelper
    {
        public static OutputColor ConvertPageColor(PrintSettings.PageColor color)
        {
            return (OutputColor)((int)color + 1);
        }

        public static OutputQuality ConvertPageQuality(PrintSettings.PageQuality quality)
        {
            return (OutputQuality)((int)quality + 1);
        }

        public static Duplexing ConvertDoubleSided(PrintSettings.DoubleSided doubleSided)
        {
            return (Duplexing)((int)doubleSided + 1);
        }

        public static PageMediaSize ConvertPageSize(PrintSettings.PageSize size)
        {
            return new PageMediaSize((PageMediaSizeName)((int)size + 1));
        }

        public static PageMediaType ConvertPageType(PrintSettings.PageType type)
        {
            return (PageMediaType)((int)type + 1);
        }

        public static string GetPageSizeName(PageMediaSizeName? size)
        {
            switch (size)
            {
                case PageMediaSizeName.BusinessCard: return "Business Card";
                case PageMediaSizeName.CreditCard: return "Credit Card";
                case PageMediaSizeName.ISOA0: return "A0";
                case PageMediaSizeName.ISOA1: return "A1";
                case PageMediaSizeName.ISOA10: return "A10";
                case PageMediaSizeName.ISOA2: return "A2";
                case PageMediaSizeName.ISOA3: return "A3";
                case PageMediaSizeName.ISOA3Extra: return "A3 Extra";
                case PageMediaSizeName.ISOA3Rotated: return "A3 Rotated";
                case PageMediaSizeName.ISOA4: return "A4";
                case PageMediaSizeName.ISOA4Extra: return "A4 Extra";
                case PageMediaSizeName.ISOA4Rotated: return "A4 Rotated";
                case PageMediaSizeName.ISOA5: return "A5";
                case PageMediaSizeName.ISOA5Extra: return "A5 Extra";
                case PageMediaSizeName.ISOA5Rotated: return "A5 Rotated";
                case PageMediaSizeName.ISOA6: return "A6";
                case PageMediaSizeName.ISOA6Rotated: return "A6 Rotated";
                case PageMediaSizeName.ISOA7: return "A7";
                case PageMediaSizeName.ISOA8: return "A8";
                case PageMediaSizeName.ISOA9: return "A9";
                case PageMediaSizeName.ISOB0: return "ISO B0";
                case PageMediaSizeName.ISOB1: return "ISO B1";
                case PageMediaSizeName.ISOB10: return "ISO B10";
                case PageMediaSizeName.ISOB2: return "ISO B2";
                case PageMediaSizeName.ISOB3: return "ISO B3";
                case PageMediaSizeName.ISOB4: return "ISO B4";
                case PageMediaSizeName.ISOB4Envelope: return "ISO B4 Envelope";
                case PageMediaSizeName.ISOB5Envelope: return "ISO B5 Envelope";
                case PageMediaSizeName.ISOB5Extra: return "ISO B5 Extra";
                case PageMediaSizeName.ISOB7: return "ISO B7";
                case PageMediaSizeName.ISOB8: return "ISO B8";
                case PageMediaSizeName.ISOB9: return "ISO B9";
                case PageMediaSizeName.ISOC0: return "C0";
                case PageMediaSizeName.ISOC1: return "C1";
                case PageMediaSizeName.ISOC2: return "C2";
                case PageMediaSizeName.ISOC3: return "C3";
                case PageMediaSizeName.ISOC3Envelope: return "C3 Envelope";
                case PageMediaSizeName.ISOC4: return "C4";
                case PageMediaSizeName.ISOC4Envelope: return "C4 Envelope";
                case PageMediaSizeName.ISOC5: return "C5";
                case PageMediaSizeName.ISOC5Envelope: return "C5 Envelope";
                case PageMediaSizeName.ISOC6: return "C6";
                case PageMediaSizeName.ISOC6C5Envelope: return "C6C5 Envelope";
                case PageMediaSizeName.ISOC6Envelope: return "C6 Envelope";
                case PageMediaSizeName.ISOC7: return "C7";
                case PageMediaSizeName.ISOC8: return "C8";
                case PageMediaSizeName.ISOC9: return "C9";
                case PageMediaSizeName.ISOC10: return "C10";
                case PageMediaSizeName.ISODLEnvelope: return "DL Envelope";
                case PageMediaSizeName.ISODLEnvelopeRotated: return "DL Envelope Rotated";
                case PageMediaSizeName.ISOSRA3: return "SRA3";
                case PageMediaSizeName.Japan2LPhoto: return "2L Photo";
                case PageMediaSizeName.JapanChou3Envelope: return "Chou 3 Envelope";
                case PageMediaSizeName.JapanChou3EnvelopeRotated: return "Chou 3 Envelope Rotated";
                case PageMediaSizeName.JapanChou4Envelope: return "Chou 4 Envelope";
                case PageMediaSizeName.JapanChou4EnvelopeRotated: return "Chou 4 Envelope Rotated";
                case PageMediaSizeName.JapanDoubleHagakiPostcard: return "Double Hagaki Postcard";
                case PageMediaSizeName.JapanDoubleHagakiPostcardRotated: return "Double Hagaki Postcard Rotated";
                case PageMediaSizeName.JapanHagakiPostcard: return "Hagaki Postcard";
                case PageMediaSizeName.JapanHagakiPostcardRotated: return "Hagaki Postcard Rotated";
                case PageMediaSizeName.JapanKaku2Envelope: return "Kaku 2 Envelope";
                case PageMediaSizeName.JapanKaku2EnvelopeRotated: return "Kaku 2 Envelope Rotated";
                case PageMediaSizeName.JapanKaku3Envelope: return "Kaku 3 Envelope";
                case PageMediaSizeName.JapanKaku3EnvelopeRotated: return "Kaku 3 Envelope Rotated";
                case PageMediaSizeName.JapanLPhoto: return "L Photo";
                case PageMediaSizeName.JapanQuadrupleHagakiPostcard: return "Quadruple Hagaki Postcard";
                case PageMediaSizeName.JapanYou1Envelope: return "You 1 Envelope";
                case PageMediaSizeName.JapanYou2Envelope: return "You 2 Envelope";
                case PageMediaSizeName.JapanYou3Envelope: return "You 3 Envelope";
                case PageMediaSizeName.JapanYou4Envelope: return "You 4 Envelope";
                case PageMediaSizeName.JapanYou4EnvelopeRotated: return "You 4 Envelope Rotated";
                case PageMediaSizeName.JapanYou6Envelope: return "You 6 Envelope";
                case PageMediaSizeName.JapanYou6EnvelopeRotated: return "You 6 Envelope Rotated";
                case PageMediaSizeName.JISB0: return "JIS B0";
                case PageMediaSizeName.JISB1: return "JIS B1";
                case PageMediaSizeName.JISB10: return "JIS B10";
                case PageMediaSizeName.JISB2: return "JIS B2";
                case PageMediaSizeName.JISB3: return "JIS B3";
                case PageMediaSizeName.JISB4: return "JIS B4";
                case PageMediaSizeName.JISB4Rotated: return "JIS B4 Rotated";
                case PageMediaSizeName.JISB5: return "JIS B5";
                case PageMediaSizeName.JISB5Rotated: return "JIS B5 Rotated";
                case PageMediaSizeName.JISB6: return "JIS B6";
                case PageMediaSizeName.JISB6Rotated: return "JIS B6 Rotated";
                case PageMediaSizeName.JISB7: return "JIS B7";
                case PageMediaSizeName.JISB8: return "JIS B8";
                case PageMediaSizeName.JISB9: return "JIS B9";
                case PageMediaSizeName.NorthAmerica10x11: return "10 × 11 in";
                case PageMediaSizeName.NorthAmerica10x12: return "10 × 12 in";
                case PageMediaSizeName.NorthAmerica10x14: return "10 × 14 in";
                case PageMediaSizeName.NorthAmerica11x17: return "11 × 17 in";
                case PageMediaSizeName.NorthAmerica14x17: return "14 × 17 in";
                case PageMediaSizeName.NorthAmerica4x6: return "4 × 6 in";
                case PageMediaSizeName.NorthAmerica4x8: return "4 × 8 in";
                case PageMediaSizeName.NorthAmerica5x7: return "5 × 7 in";
                case PageMediaSizeName.NorthAmerica8x10: return "8 × 10 in";
                case PageMediaSizeName.NorthAmerica9x11: return "9 × 11 in";
                case PageMediaSizeName.NorthAmericaArchitectureASheet: return "Architecture A Sheet";
                case PageMediaSizeName.NorthAmericaArchitectureBSheet: return "Architecture B Sheet";
                case PageMediaSizeName.NorthAmericaArchitectureCSheet: return "Architecture C Sheet";
                case PageMediaSizeName.NorthAmericaArchitectureDSheet: return "Architecture D Sheet";
                case PageMediaSizeName.NorthAmericaArchitectureESheet: return "Architecture E Sheet";
                case PageMediaSizeName.NorthAmericaCSheet: return "C Sheet";
                case PageMediaSizeName.NorthAmericaDSheet: return "D Sheet";
                case PageMediaSizeName.NorthAmericaESheet: return "E Sheet";
                case PageMediaSizeName.NorthAmericaExecutive: return "Executive";
                case PageMediaSizeName.NorthAmericaGermanLegalFanfold: return "German Legal Fanfold";
                case PageMediaSizeName.NorthAmericaGermanStandardFanfold: return "German Standard Fanfold";
                case PageMediaSizeName.NorthAmericaLegal: return "Legal";
                case PageMediaSizeName.NorthAmericaLegalExtra: return "Legal Extra";
                case PageMediaSizeName.NorthAmericaLetter: return "Letter";
                case PageMediaSizeName.NorthAmericaLetterExtra: return "Letter Extra";
                case PageMediaSizeName.NorthAmericaLetterPlus: return "Letter Plus";
                case PageMediaSizeName.NorthAmericaLetterRotated: return "Letter Rotated";
                case PageMediaSizeName.NorthAmericaMonarchEnvelope: return "Monarch Envelope";
                case PageMediaSizeName.NorthAmericaNote: return "Note";
                case PageMediaSizeName.NorthAmericaNumber10Envelope: return "Number 10 Envelope";
                case PageMediaSizeName.NorthAmericaNumber10EnvelopeRotated: return "Number 10 Envelope Rotated";
                case PageMediaSizeName.NorthAmericaNumber11Envelope: return "Number 11 Envelope";
                case PageMediaSizeName.NorthAmericaNumber12Envelope: return "Number 12 Envelope";
                case PageMediaSizeName.NorthAmericaNumber14Envelope: return "Number 14 Envelope";
                case PageMediaSizeName.NorthAmericaNumber9Envelope: return "Number 9 Envelope";
                case PageMediaSizeName.NorthAmericaPersonalEnvelope: return "Personal Envelope";
                case PageMediaSizeName.NorthAmericaQuarto: return "Quarto";
                case PageMediaSizeName.NorthAmericaStatement: return "Statement";
                case PageMediaSizeName.NorthAmericaSuperA: return "Super A";
                case PageMediaSizeName.NorthAmericaSuperB: return "Super B";
                case PageMediaSizeName.NorthAmericaTabloid: return "Tabloid";
                case PageMediaSizeName.NorthAmericaTabloidExtra: return "Tabloid Extra";
                case PageMediaSizeName.OtherMetricA3Plus: return "A3 Plus";
                case PageMediaSizeName.OtherMetricA4Plus: return "A4 Plus";
                case PageMediaSizeName.OtherMetricFolio: return "Folio";
                case PageMediaSizeName.OtherMetricInviteEnvelope: return "Invite Envelope";
                case PageMediaSizeName.OtherMetricItalianEnvelope: return "Italian Envelope";
                case PageMediaSizeName.PRC10Envelope: return "#10 Envelope";
                case PageMediaSizeName.PRC10EnvelopeRotated: return "#10 Envelope Rotated";
                case PageMediaSizeName.PRC16K: return "16K";
                case PageMediaSizeName.PRC16KRotated: return "16K Rotated";
                case PageMediaSizeName.PRC1Envelope: return "#1 Envelope";
                case PageMediaSizeName.PRC1EnvelopeRotated: return "#1 Envelope Rotated";
                case PageMediaSizeName.PRC2Envelope: return "#2 Envelope";
                case PageMediaSizeName.PRC2EnvelopeRotated: return "#2 Envelope Rotated";
                case PageMediaSizeName.PRC32K: return "32K";
                case PageMediaSizeName.PRC32KBig: return "32K Big";
                case PageMediaSizeName.PRC32KRotated: return "32K Rotated";
                case PageMediaSizeName.PRC3Envelope: return "#3 Envelope";
                case PageMediaSizeName.PRC3EnvelopeRotated: return "#3 Envelope Rotated";
                case PageMediaSizeName.PRC4Envelope: return "#4 Envelope";
                case PageMediaSizeName.PRC4EnvelopeRotated: return "#4 Envelope Rotated";
                case PageMediaSizeName.PRC5Envelope: return "#5 Envelope";
                case PageMediaSizeName.PRC5EnvelopeRotated: return "#5 Envelope Rotated";
                case PageMediaSizeName.PRC6Envelope: return "#6 Envelope";
                case PageMediaSizeName.PRC6EnvelopeRotated: return "#6 Envelope Rotated";
                case PageMediaSizeName.PRC7Envelope: return "#7 Envelope";
                case PageMediaSizeName.PRC7EnvelopeRotated: return "#7 Envelope Rotated";
                case PageMediaSizeName.PRC8Envelope: return "#8 Envelope";
                case PageMediaSizeName.PRC8EnvelopeRotated: return "#8 Envelope Rotated";
                case PageMediaSizeName.PRC9Envelope: return "#9 Envelope";
                case PageMediaSizeName.PRC9EnvelopeRotated: return "#9 Envelope Rotated";
                case PageMediaSizeName.Roll04Inch: return "4-inch Wide Roll";
                case PageMediaSizeName.Roll06Inch: return "6-inch Wide Roll";
                case PageMediaSizeName.Roll08Inch: return "8-inch Wide Roll";
                case PageMediaSizeName.Roll12Inch: return "12-inch Wide Roll";
                case PageMediaSizeName.Roll15Inch: return "15-inch Wide Roll";
                case PageMediaSizeName.Roll18Inch: return "18-inch Wide Roll";
                case PageMediaSizeName.Roll22Inch: return "22-inch Wide Roll";
                case PageMediaSizeName.Roll24Inch: return "24-inch Wide Roll";
                case PageMediaSizeName.Roll30Inch: return "30-inch Wide Roll";
                case PageMediaSizeName.Roll36Inch: return "36-inch Wide Roll";
                case PageMediaSizeName.Roll54Inch: return "54-inch Wide Roll";
                default: return "Custom Size";
            };
        }

        public static string GetPageSizeName(PrintSettings.PageSize size)
        {
            return GetPageSizeName(ConvertPageSize(size).PageMediaSizeName);
        }

        public static string GetPageTypeName(PageMediaType type)
        {
            switch (type)
            {
                case PageMediaType.Archival: return "Archival";
                case PageMediaType.AutoSelect: return "Auto Select";
                case PageMediaType.BackPrintFilm: return "Back Print Film";
                case PageMediaType.Bond: return "Bond";
                case PageMediaType.CardStock: return "Card Stock";
                case PageMediaType.Continuous: return "Continuous";
                case PageMediaType.EnvelopePlain: return "Envelope Plain";
                case PageMediaType.EnvelopeWindow: return "Envelope Window";
                case PageMediaType.Fabric: return "Fabric";
                case PageMediaType.HighResolution: return "High Resolution";
                case PageMediaType.Label: return "Label";
                case PageMediaType.MultiLayerForm: return "Multi Layer Form";
                case PageMediaType.MultiPartForm: return "Multi Part Form";
                case PageMediaType.Photographic: return "Photographic";
                case PageMediaType.PhotographicFilm: return "Photographic Film";
                case PageMediaType.PhotographicGlossy: return "Photographic Glossy";
                case PageMediaType.PhotographicHighGloss: return "Photographic High Gloss";
                case PageMediaType.PhotographicMatte: return "Photographic Matte";
                case PageMediaType.PhotographicSatin: return "Photographic Satin";
                case PageMediaType.PhotographicSemiGloss: return "Photographic Semi Gloss";
                case PageMediaType.Plain: return "Plain";
                case PageMediaType.Screen: return "Screen";
                case PageMediaType.ScreenPaged: return "Screen Paged";
                case PageMediaType.Stationery: return "Stationery";
                case PageMediaType.TabStockFull: return "Tab Stock Full";
                case PageMediaType.TabStockPreCut: return "Tab Stock Pre Cut";
                case PageMediaType.Transparency: return "Transparency";
                case PageMediaType.TShirtTransfer: return "T-shirt Transfer";
                default: return "Unknown Type";
            };
        }

        public static string GetPageTypeName(PrintSettings.PageType type)
        {
            return GetPageTypeName(ConvertPageType(type));
        }

        public static string GetInputBinName(InputBin inputBin)
        {
            switch (inputBin)
            {
                case InputBin.AutoSelect: return "Auto Select";
                case InputBin.AutoSheetFeeder: return "Auto Sheet Feeder";
                case InputBin.Cassette: return "Cassette";
                case InputBin.Manual: return "Manual";
                case InputBin.Tractor: return "Tractor";
                default: return "Unknown Input Bin";
            };
        }

        public static PageMediaSizeName GetPageSizeName(string size)
        {
            string sizeAdjusted = (size.StartsWith("psk:") ? size.Remove(0, 4) : size).ToLower();
            switch (sizeAdjusted)
            {
                case "businesscard": return PageMediaSizeName.BusinessCard;
                case "creditcard": return PageMediaSizeName.CreditCard;
                case "isoa0": return PageMediaSizeName.ISOA0;
                case "isoa1": return PageMediaSizeName.ISOA1;
                case "isoa10": return PageMediaSizeName.ISOA10;
                case "isoa2": return PageMediaSizeName.ISOA2;
                case "isoa3": return PageMediaSizeName.ISOA3;
                case "isoa3extra": return PageMediaSizeName.ISOA3Extra;
                case "isoa3rotated": return PageMediaSizeName.ISOA3Rotated;
                case "isoa4": return PageMediaSizeName.ISOA4;
                case "isoa4extra": return PageMediaSizeName.ISOA4Extra;
                case "isoa4rotated": return PageMediaSizeName.ISOA4Rotated;
                case "isoa5": return PageMediaSizeName.ISOA5;
                case "isoa5extra": return PageMediaSizeName.ISOA5Extra;
                case "isoa5rotated": return PageMediaSizeName.ISOA5Rotated;
                case "isoa6": return PageMediaSizeName.ISOA6;
                case "isoa6rotated": return PageMediaSizeName.ISOA6Rotated;
                case "isoa7": return PageMediaSizeName.ISOA7;
                case "isoa8": return PageMediaSizeName.ISOA8;
                case "isoa9": return PageMediaSizeName.ISOA9;
                case "isob0": return PageMediaSizeName.ISOB0;
                case "isob1": return PageMediaSizeName.ISOB1;
                case "isob10": return PageMediaSizeName.ISOB10;
                case "isob2": return PageMediaSizeName.ISOB2;
                case "isob3": return PageMediaSizeName.ISOB3;
                case "isob4": return PageMediaSizeName.ISOB4;
                case "isob4envelope": return PageMediaSizeName.ISOB4Envelope;
                case "isob5envelope": return PageMediaSizeName.ISOB5Envelope;
                case "isob5extra": return PageMediaSizeName.ISOB5Extra;
                case "isob7": return PageMediaSizeName.ISOB7;
                case "isob8": return PageMediaSizeName.ISOB8;
                case "isob9": return PageMediaSizeName.ISOB9;
                case "isoc0": return PageMediaSizeName.ISOC0;
                case "isoc1": return PageMediaSizeName.ISOC1;
                case "isoc2": return PageMediaSizeName.ISOC2;
                case "isoc3": return PageMediaSizeName.ISOC3;
                case "isoc3envelope": return PageMediaSizeName.ISOC3Envelope;
                case "isoc4": return PageMediaSizeName.ISOC4;
                case "isoc4envelope": return PageMediaSizeName.ISOC4Envelope;
                case "isoc5": return PageMediaSizeName.ISOC5;
                case "isoc5envelope": return PageMediaSizeName.ISOC5Envelope;
                case "isoc6": return PageMediaSizeName.ISOC6;
                case "isoc6c5envelope": return PageMediaSizeName.ISOC6C5Envelope;
                case "isoc6envelope": return PageMediaSizeName.ISOC6Envelope;
                case "isoc7": return PageMediaSizeName.ISOC7;
                case "isoc8": return PageMediaSizeName.ISOC8;
                case "isoc9": return PageMediaSizeName.ISOC9;
                case "isoc10": return PageMediaSizeName.ISOC10;
                case "isodlenvelope": return PageMediaSizeName.ISODLEnvelope;
                case "isodlenveloperotated": return PageMediaSizeName.ISODLEnvelopeRotated;
                case "isosra3": return PageMediaSizeName.ISOSRA3;
                case "japan2lphoto": return PageMediaSizeName.Japan2LPhoto;
                case "japanchou3envelope": return PageMediaSizeName.JapanChou3Envelope;
                case "japanchou3enveloperotated": return PageMediaSizeName.JapanChou3EnvelopeRotated;
                case "japanchou4envelope": return PageMediaSizeName.JapanChou4Envelope;
                case "japanchou4enveloperotated": return PageMediaSizeName.JapanChou4EnvelopeRotated;
                case "japandoublehagakipostcard": return PageMediaSizeName.JapanDoubleHagakiPostcard;
                case "japandoublehagakipostcardrotated": return PageMediaSizeName.JapanDoubleHagakiPostcardRotated;
                case "japanhagakipostcard": return PageMediaSizeName.JapanHagakiPostcard;
                case "japanhagakipostcardrotated": return PageMediaSizeName.JapanHagakiPostcardRotated;
                case "japankaku2envelope": return PageMediaSizeName.JapanKaku2Envelope;
                case "japankaku2enveloperotated": return PageMediaSizeName.JapanKaku2EnvelopeRotated;
                case "japankaku3envelope": return PageMediaSizeName.JapanKaku3Envelope;
                case "japankaku3enveloperotated": return PageMediaSizeName.JapanKaku3EnvelopeRotated;
                case "japanlphoto": return PageMediaSizeName.JapanLPhoto;
                case "japanquadruplehagakipostcard": return PageMediaSizeName.JapanQuadrupleHagakiPostcard;
                case "japanyou1envelope": return PageMediaSizeName.JapanYou1Envelope;
                case "japanyou2envelope": return PageMediaSizeName.JapanYou2Envelope;
                case "japanyou3envelope": return PageMediaSizeName.JapanYou3Envelope;
                case "japanyou4envelope": return PageMediaSizeName.JapanYou4Envelope;
                case "japanyou4enveloperotated": return PageMediaSizeName.JapanYou4EnvelopeRotated;
                case "japanyou6envelope": return PageMediaSizeName.JapanYou6Envelope;
                case "japanyou6enveloperotated": return PageMediaSizeName.JapanYou6EnvelopeRotated;
                case "jisb0": return PageMediaSizeName.JISB0;
                case "jisb1": return PageMediaSizeName.JISB1;
                case "jisb10": return PageMediaSizeName.JISB10;
                case "jisb2": return PageMediaSizeName.JISB2;
                case "jisb3": return PageMediaSizeName.JISB3;
                case "jisb4": return PageMediaSizeName.JISB4;
                case "jisb4rotated": return PageMediaSizeName.JISB4Rotated;
                case "jisb5": return PageMediaSizeName.JISB5;
                case "jisb5rotated": return PageMediaSizeName.JISB5Rotated;
                case "jisb6": return PageMediaSizeName.JISB6;
                case "jisb6rotated": return PageMediaSizeName.JISB6Rotated;
                case "jisb7": return PageMediaSizeName.JISB7;
                case "jisb8": return PageMediaSizeName.JISB8;
                case "jisb9": return PageMediaSizeName.JISB9;
                case "northamerica10x11": return PageMediaSizeName.NorthAmerica10x11;
                case "northamerica10x12": return PageMediaSizeName.NorthAmerica10x12;
                case "northamerica10x14": return PageMediaSizeName.NorthAmerica10x14;
                case "northamerica11x17": return PageMediaSizeName.NorthAmerica11x17;
                case "northamerica14x17": return PageMediaSizeName.NorthAmerica14x17;
                case "northamerica4x6": return PageMediaSizeName.NorthAmerica4x6;
                case "northamerica4x8": return PageMediaSizeName.NorthAmerica4x8;
                case "northamerica5x7": return PageMediaSizeName.NorthAmerica5x7;
                case "northamerica8x10": return PageMediaSizeName.NorthAmerica8x10;
                case "northamerica9x11": return PageMediaSizeName.NorthAmerica9x11;
                case "northamericaarchitectureasheet": return PageMediaSizeName.NorthAmericaArchitectureASheet;
                case "northamericaarchitecturebsheet": return PageMediaSizeName.NorthAmericaArchitectureBSheet;
                case "northamericaarchitecturecsheet": return PageMediaSizeName.NorthAmericaArchitectureCSheet;
                case "northamericaarchitecturedsheet": return PageMediaSizeName.NorthAmericaArchitectureDSheet;
                case "northamericaarchitectureesheet": return PageMediaSizeName.NorthAmericaArchitectureESheet;
                case "northamericacsheet": return PageMediaSizeName.NorthAmericaCSheet;
                case "northamericadsheet": return PageMediaSizeName.NorthAmericaDSheet;
                case "northamericaesheet": return PageMediaSizeName.NorthAmericaESheet;
                case "northamericaexecutive": return PageMediaSizeName.NorthAmericaExecutive;
                case "northamericagermanlegalfanfold": return PageMediaSizeName.NorthAmericaGermanLegalFanfold;
                case "northamericagermanstandardfanfold": return PageMediaSizeName.NorthAmericaGermanStandardFanfold;
                case "northamericalegal": return PageMediaSizeName.NorthAmericaLegal;
                case "northamericalegalextra": return PageMediaSizeName.NorthAmericaLegalExtra;
                case "northamericaletter": return PageMediaSizeName.NorthAmericaLetter;
                case "northamericaletterextra": return PageMediaSizeName.NorthAmericaLetterExtra;
                case "northamericaletterplus": return PageMediaSizeName.NorthAmericaLetterPlus;
                case "northamericaletterrotated": return PageMediaSizeName.NorthAmericaLetterRotated;
                case "northamericamonarchenvelope": return PageMediaSizeName.NorthAmericaMonarchEnvelope;
                case "northamericanote": return PageMediaSizeName.NorthAmericaNote;
                case "northamericanumber10envelope": return PageMediaSizeName.NorthAmericaNumber10Envelope;
                case "northamericanumber10enveloperotated": return PageMediaSizeName.NorthAmericaNumber10EnvelopeRotated;
                case "northamericanumber11envelope": return PageMediaSizeName.NorthAmericaNumber11Envelope;
                case "northamericanumber12envelope": return PageMediaSizeName.NorthAmericaNumber12Envelope;
                case "northamericanumber14envelope": return PageMediaSizeName.NorthAmericaNumber14Envelope;
                case "northamericanumber9envelope": return PageMediaSizeName.NorthAmericaNumber9Envelope;
                case "northamericapersonalenvelope": return PageMediaSizeName.NorthAmericaPersonalEnvelope;
                case "northamericaquarto": return PageMediaSizeName.NorthAmericaQuarto;
                case "northamericastatement": return PageMediaSizeName.NorthAmericaStatement;
                case "northamericasupera": return PageMediaSizeName.NorthAmericaSuperA;
                case "northamericasuperb": return PageMediaSizeName.NorthAmericaSuperB;
                case "northamericatabloid": return PageMediaSizeName.NorthAmericaTabloid;
                case "northamericatabloidextra": return PageMediaSizeName.NorthAmericaTabloidExtra;
                case "othermetrica3plus": return PageMediaSizeName.OtherMetricA3Plus;
                case "othermetrica4plus": return PageMediaSizeName.OtherMetricA4Plus;
                case "othermetricfolio": return PageMediaSizeName.OtherMetricFolio;
                case "othermetricinviteenvelope": return PageMediaSizeName.OtherMetricInviteEnvelope;
                case "othermetricitalianenvelope": return PageMediaSizeName.OtherMetricItalianEnvelope;
                case "prc10envelope": return PageMediaSizeName.PRC10Envelope;
                case "prc10enveloperotated": return PageMediaSizeName.PRC10EnvelopeRotated;
                case "prc16k": return PageMediaSizeName.PRC16K;
                case "prc16krotated": return PageMediaSizeName.PRC16KRotated;
                case "prc1envelope": return PageMediaSizeName.PRC1Envelope;
                case "prc1enveloperotated": return PageMediaSizeName.PRC1EnvelopeRotated;
                case "prc2envelope": return PageMediaSizeName.PRC2Envelope;
                case "prc2enveloperotated": return PageMediaSizeName.PRC2EnvelopeRotated;
                case "prc32k": return PageMediaSizeName.PRC32K;
                case "prc32kbig": return PageMediaSizeName.PRC32KBig;
                case "prc32krotated": return PageMediaSizeName.PRC32KRotated;
                case "prc3envelope": return PageMediaSizeName.PRC3Envelope;
                case "prc3enveloperotated": return PageMediaSizeName.PRC3EnvelopeRotated;
                case "prc4envelope": return PageMediaSizeName.PRC4Envelope;
                case "prc4enveloperotated": return PageMediaSizeName.PRC4EnvelopeRotated;
                case "prc5envelope": return PageMediaSizeName.PRC5Envelope;
                case "prc5enveloperotated": return PageMediaSizeName.PRC5EnvelopeRotated;
                case "prc6envelope": return PageMediaSizeName.PRC6Envelope;
                case "prc6enveloperotated": return PageMediaSizeName.PRC6EnvelopeRotated;
                case "prc7envelope": return PageMediaSizeName.PRC7Envelope;
                case "prc7enveloperotated": return PageMediaSizeName.PRC7EnvelopeRotated;
                case "prc8envelope": return PageMediaSizeName.PRC8Envelope;
                case "prc8enveloperotated": return PageMediaSizeName.PRC8EnvelopeRotated;
                case "prc9envelope": return PageMediaSizeName.PRC9Envelope;
                case "prc9enveloperotated": return PageMediaSizeName.PRC9EnvelopeRotated;
                case "roll04inch": return PageMediaSizeName.Roll04Inch;
                case "roll06inch": return PageMediaSizeName.Roll06Inch;
                case "roll08inch": return PageMediaSizeName.Roll08Inch;
                case "roll12inch": return PageMediaSizeName.Roll12Inch;
                case "roll15inch": return PageMediaSizeName.Roll15Inch;
                case "roll18inch": return PageMediaSizeName.Roll18Inch;
                case "roll22inch": return PageMediaSizeName.Roll22Inch;
                case "roll24inch": return PageMediaSizeName.Roll24Inch;
                case "roll30inch": return PageMediaSizeName.Roll30Inch;
                case "roll36inch": return PageMediaSizeName.Roll36Inch;
                case "roll54inch": return PageMediaSizeName.Roll54Inch;
                default: return PageMediaSizeName.Unknown;
            };
        }
    }
}