using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PrintDialogX.PrintControl.PreviewHelper
{
    internal enum DocumentOrientation
    {
        Portrait,
        Landscape
    }

    internal enum PageOrder
    {
        Horizontal,
        HorizontalReverse,
        Vertical,
        VerticalReverse
    }

    internal class MultiPagesPerSheetHelper
    {
        private readonly int _pagesPerSheet;
        private readonly Size _innerPageSize;
        private readonly PageOrder _pageOrder;
        private readonly FixedDocument _document;
        private readonly DocumentOrientation _documentOrientation;

        private readonly List<int> _allowedPagesPerSheetCount = new List<int> { 1, 2, 4, 6, 9, 16 };
        private readonly List<Size> _allowedPagesPerSheetArrange = new List<Size> { new Size(1, 1), new Size(1, 2), new Size(2, 2), new Size(2, 3), new Size(3, 3), new Size(4, 4) };

        public MultiPagesPerSheetHelper()
        {
            _document = null;
            _pagesPerSheet = 0;
            _innerPageSize = new Size();
            _pageOrder = PageOrder.Horizontal;
            _documentOrientation = DocumentOrientation.Portrait;
        }

        public MultiPagesPerSheetHelper(int pagesPerSheet, FixedDocument document)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _innerPageSize = new Size();
            _pageOrder = PageOrder.Horizontal;
            _documentOrientation = DocumentOrientation.Portrait;
        }

        public MultiPagesPerSheetHelper(int pagesPerSheet, FixedDocument document, Size innerPageSize)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _innerPageSize = innerPageSize;
            _pageOrder = PageOrder.Horizontal;
            _documentOrientation = DocumentOrientation.Portrait;
        }

        public MultiPagesPerSheetHelper(int pagesPerSheet, FixedDocument document, Size innerPageSize, DocumentOrientation documentOrientation)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _innerPageSize = innerPageSize;
            _pageOrder = PageOrder.Horizontal;
            _documentOrientation = documentOrientation;
        }

        public MultiPagesPerSheetHelper(int pagesPerSheet, FixedDocument document, Size innerPageSize, DocumentOrientation documentOrientation, PageOrder pageOrder)
        {
            _document = document;
            _pagesPerSheet = pagesPerSheet;
            _innerPageSize = innerPageSize;
            _pageOrder = pageOrder;
            _documentOrientation = documentOrientation;
        }

        public static int GetPagePerSheetCountIndex(int pagesPerSheetCount)
        {
            List<int> _allowedPagesPerSheetCount = new List<int> { 1, 2, 4, 6, 9, 16 };

            if (_allowedPagesPerSheetCount.Contains(pagesPerSheetCount))
            {
                return _allowedPagesPerSheetCount.IndexOf(pagesPerSheetCount);
            }
            else
            {
                return -1;
            }
        }

        public FixedDocument GetMultiPagesPerSheetDocument(double scale)
        {
            if (_document == null && _document.Pages.Count == 0 && _pagesPerSheet == 0)
            {
                return new FixedDocument();
            }
            if (_allowedPagesPerSheetCount.Contains(_pagesPerSheet) == false)
            {
                return new FixedDocument();
            }

            FixedDocument doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = _document.DocumentPaginator.PageSize;

            int currentPageCount = 0;
            int pagesPerSheetIndex = _allowedPagesPerSheetCount.IndexOf(_pagesPerSheet);
            int arrangeRows;
            int arrangeColumns;
            if (_documentOrientation == DocumentOrientation.Portrait)
            {
                arrangeRows = (int)_allowedPagesPerSheetArrange[pagesPerSheetIndex].Height;
                arrangeColumns = (int)_allowedPagesPerSheetArrange[pagesPerSheetIndex].Width;
            }
            else
            {
                arrangeRows = (int)_allowedPagesPerSheetArrange[pagesPerSheetIndex].Width;
                arrangeColumns = (int)_allowedPagesPerSheetArrange[pagesPerSheetIndex].Height;
            }
            double innerPageHeight = doc.DocumentPaginator.PageSize.Height / arrangeRows;
            double innerPageWidth = doc.DocumentPaginator.PageSize.Width / arrangeColumns;

            while (currentPageCount < _document.Pages.Count)
            {
                FixedPage outerPage = new FixedPage
                {
                    Width = doc.DocumentPaginator.PageSize.Width,
                    Height = doc.DocumentPaginator.PageSize.Height
                };

                Grid outergrid = new Grid()
                {
                    Width = outerPage.Width,
                    Height = outerPage.Height
                };

                for (int i = 0; i < arrangeRows; i++)
                {
                    outergrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                }
                for (int i = 0; i < arrangeColumns; i++)
                {
                    outergrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }

                if (_pageOrder == PageOrder.Horizontal)
                {
                    for (int i = 0; i < arrangeRows; i++)
                    {
                        for (int j = 0; j < arrangeColumns && currentPageCount < _document.Pages.Count; j++)
                        {
                            outergrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PageOrder.HorizontalReverse)
                {
                    for (int i = 0; i < arrangeRows; i++)
                    {
                        for (int j = arrangeColumns - 1; j >= 0 && currentPageCount < _document.Pages.Count; j--)
                        {
                            outergrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PageOrder.Vertical)
                {
                    for (int j = 0; j < arrangeColumns; j++)
                    {
                        for (int i = 0; i < arrangeRows && currentPageCount < _document.Pages.Count; i++)
                        {
                            outergrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else if (_pageOrder == PageOrder.VerticalReverse)
                {
                    for (int j = 0; j < arrangeColumns; j++)
                    {
                        for (int i = arrangeRows - 1; i >= 0 && currentPageCount < _document.Pages.Count; i--)
                        {
                            outergrid.Children.Add(GetMultiPagesPerSheetGrid(i, j, currentPageCount, innerPageWidth, innerPageHeight, outerPage.Width, outerPage.Height));
                            currentPageCount++;
                        }
                    }
                }
                else
                {
                    return new FixedDocument();
                }

                outerPage.Children.Add(outergrid);

                if (scale != double.NaN)
                {
                    outerPage.RenderTransformOrigin = new Point(0, 0);
                    outerPage.RenderTransform = new ScaleTransform(scale / 100.0, scale / 100.0);
                }

                doc.Pages.Add(new PageContent { Child = outerPage });
            }

            return doc;
        }

        private Grid GetMultiPagesPerSheetGrid(int column, int row, int currentPageCount, double innerPageWidth, double innerPageHeight, double outerPageWidth, double outerPageHeight)
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

            string xaml = XamlWriter.Save(_document.Pages[currentPageCount].Child);
            FixedPage innerPage = XamlReader.Parse(xaml) as FixedPage;

            if (_innerPageSize.Width == 0 || _innerPageSize.Height == 0 || _innerPageSize.Width == double.NaN || _innerPageSize.Height == double.NaN)
            {
                innerPage.Width = outerPageWidth;
                innerPage.Height = outerPageHeight;
            }
            else
            {
                innerPage.Width = _innerPageSize.Width;
                innerPage.Height = _innerPageSize.Height;
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
