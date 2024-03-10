using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintDialogX
{
    public class PrintDocument
    {
        public PrintDocument()
        {
            Pages = new List<PrintPage>();
        }

        /// <summary>
        /// The document name.
        /// </summary>
        public string DocumentName { get; set; } = "Untitled Document";

        /// <summary>
        /// The document size.
        /// </summary>
        public Size DocumentSize { get; set; } = new Size(96 * 8.25, 96 * 11.75);

        /// <summary>
        /// The default document margin.
        /// </summary>
        public double DocumentMargin { get; set; } = 60;

        /// <summary>
        /// The pages of the document.
        /// </summary>
        public ICollection<PrintPage> Pages { get; set; }

        /// <summary>
        /// Set the <see cref="DocumentSize"/> by inch values.
        /// <param name="widthInch">The width in inches.</param>
        /// <param name="heightInch">The height in inches.</param>
        /// <param name="dpi">The DPI, with the default value of 96.</param>
        /// </summary>
        public PrintDocument SetSizeByInch(double widthInch, double heightInch, double dpi = 96)
        {
            DocumentSize = new Size(widthInch * dpi, heightInch * dpi);
            return this;
        }

        /// <summary>
        /// Create <see cref="PrintDocument"/> from <see cref="FixedDocument"/>.
        /// <param name="document">The <see cref="FixedDocument"/> instance.</param>
        /// <param name="documentName">The document name.</param>
        /// <param name="documentMargin">The default document margin.</param>
        /// </summary>
        public static PrintDocument CreateFromFixedDocument(FixedDocument document, string documentName, double documentMargin)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.DocumentName = documentName;
            printDocument.DocumentMargin = documentMargin;
            printDocument.DocumentSize = document.DocumentPaginator.PageSize;

            foreach (PageContent pageContent in document.Pages)
            {
                FixedPage page = pageContent.Child;
                pageContent.Child = null;

                PrintPage printPage = new PrintPage();
                printPage.Content = page;
                printDocument.Pages.Add(printPage);
            }

            return printDocument;
        }
    }

    public class PrintPage
    {
        private UIElement _content;

        /// <summary>
        /// The content of the page.
        /// </summary>
        public UIElement Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (VisualTreeHelper.GetParent(value) != null)
                {
                    throw new Exception("Content is already the child of another element.");
                }
                else
                {
                    _content = value;
                }
            }
        }
    }
}

namespace PrintDialogX.Internal.PreviewHelper
{
    internal class MultiPagesPerSheetHelper
    {
        private readonly List<FixedPage> _document;
        private readonly Size _documentSize;
        private readonly PrintSettings.PagesPerSheet _pagesPerSheet;
        private readonly PrintSettings.PageOrder _pageOrder;
        private readonly PrintSettings.PageOrientation _pageOrientation;

        private readonly List<Size> _pagesPerSheetArrange = new List<Size> { new Size(1, 1), new Size(1, 2), new Size(2, 2), new Size(2, 3), new Size(3, 3), new Size(4, 4) };

        public MultiPagesPerSheetHelper(List<FixedPage> document, PrintSettings.PagesPerSheet pagesPerSheet, Size documentSize, PrintSettings.PageOrder pageOrder, PrintSettings.PageOrientation documentOrientation)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _documentSize = documentSize;
            _pageOrder = pageOrder;
            _pageOrientation = documentOrientation;
        }

        public FixedDocument GetMultiPagesPerSheetDocument(double scale)
        {
            if (_document == null || _document.Count == 0)
            {
                return new FixedDocument();
            }

            FixedDocument newDocument = new FixedDocument();
            newDocument.DocumentPaginator.PageSize = _documentSize;

            int currentPageCount = 0;
            int arrangeRows;
            int arrangeColumns;
            if (_pageOrientation == PrintSettings.PageOrientation.Portrait)
            {
                arrangeRows = (int)_pagesPerSheetArrange[(int)_pagesPerSheet].Height;
                arrangeColumns = (int)_pagesPerSheetArrange[(int)_pagesPerSheet].Width;
            }
            else
            {
                arrangeRows = (int)_pagesPerSheetArrange[(int)_pagesPerSheet].Width;
                arrangeColumns = (int)_pagesPerSheetArrange[(int)_pagesPerSheet].Height;
            }
            double innerPageHeight = newDocument.DocumentPaginator.PageSize.Height / arrangeRows;
            double innerPageWidth = newDocument.DocumentPaginator.PageSize.Width / arrangeColumns;

            while (currentPageCount < _document.Count)
            {
                FixedPage outerPage = new FixedPage
                {
                    Width = newDocument.DocumentPaginator.PageSize.Width,
                    Height = newDocument.DocumentPaginator.PageSize.Height
                };

                Grid outerGrid = new Grid()
                {
                    Width = outerPage.Width,
                    Height = outerPage.Height
                };

                for (int i = 0; i < arrangeRows; i++)
                {
                    outerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                }
                for (int i = 0; i < arrangeColumns; i++)
                {
                    outerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }

                if (_pageOrder == PrintSettings.PageOrder.Horizontal)
                {
                    for (int i = 0; i < arrangeRows; i++)
                    {
                        for (int j = 0; j < arrangeColumns && currentPageCount < _document.Count; j++)
                        {
                            outerGrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PrintSettings.PageOrder.HorizontalReverse)
                {
                    for (int i = 0; i < arrangeRows; i++)
                    {
                        for (int j = arrangeColumns - 1; j >= 0 && currentPageCount < _document.Count; j--)
                        {
                            outerGrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PrintSettings.PageOrder.Vertical)
                {
                    for (int j = 0; j < arrangeColumns; j++)
                    {
                        for (int i = 0; i < arrangeRows && currentPageCount < _document.Count; i++)
                        {
                            outerGrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PrintSettings.PageOrder.VerticalReverse)
                {
                    for (int j = 0; j < arrangeColumns; j++)
                    {
                        for (int i = arrangeRows - 1; i >= 0 && currentPageCount < _document.Count; i--)
                        {
                            outerGrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else
                {
                    return new FixedDocument();
                }

                outerPage.Children.Add(outerGrid);

                if (scale != double.NaN)
                {
                    outerPage.RenderTransformOrigin = new Point(0, 0);
                    outerPage.RenderTransform = new ScaleTransform(scale / 100.0, scale / 100.0);
                }

                newDocument.Pages.Add(new PageContent { Child = outerPage });
            }

            return newDocument;
        }

        private Grid GetMultiPagesPerSheetGrid(int column, int row, int currentPage, double innerPageWidth, double innerPageHeight, double outerPageWidth, double outerPageHeight)
        {
            Grid innerGrid = new Grid()
            {
                Width = innerPageWidth,
                Height = innerPageHeight,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            innerGrid.SetValue(Grid.RowProperty, column);
            innerGrid.SetValue(Grid.ColumnProperty, row);

            FixedPage innerPage = _document[currentPage];

            if (_documentSize.Width == 0 || _documentSize.Height == 0 || _documentSize.Width == double.NaN || _documentSize.Height == double.NaN)
            {
                innerPage.Width = outerPageWidth;
                innerPage.Height = outerPageHeight;
            }
            else
            {
                innerPage.Width = _documentSize.Width;
                innerPage.Height = _documentSize.Height;
            }

            innerPage.VerticalAlignment = VerticalAlignment.Center;
            innerPage.HorizontalAlignment = HorizontalAlignment.Center;

            double widthRatio;
            double heightRatio;

            widthRatio = innerPageWidth / innerPage.Width;
            heightRatio = innerPageHeight / innerPage.Height;

            if (innerPage.Height * widthRatio <= innerPageHeight)
            {
                innerPage.Width *= widthRatio;
                innerPage.Height *= widthRatio;

                innerPage.RenderTransform = new ScaleTransform(widthRatio, widthRatio);
            }
            else
            {
                innerPage.Width *= heightRatio;
                innerPage.Height *= heightRatio;

                innerPage.RenderTransform = new ScaleTransform(heightRatio, heightRatio);
            }

            innerGrid.Children.Add(innerPage);
            return innerGrid;
        }
    }
}
