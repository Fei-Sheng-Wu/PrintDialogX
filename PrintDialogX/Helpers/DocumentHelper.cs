using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PrintDialogX.Internal
{
    internal class DocumentHelper
    {
        private static readonly (int, int)[] _pagesPerSheetArrangements = new (int, int)[] { (1, 1), (1, 2), (2, 2), (2, 3), (3, 3), (4, 4) };

        public static System.Windows.Documents.FixedDocument GetPagesPerSheetDocument(List<System.Windows.Documents.FixedPage> document, Size size, PrintDialog.DocumentInfo info)
        {
            System.Windows.Documents.FixedDocument result = new System.Windows.Documents.FixedDocument();
            result.DocumentPaginator.PageSize = size;

            int columns = info.Orientation == PrintSettings.PageOrientation.Portrait ? _pagesPerSheetArrangements[(int)info.PagesPerSheet].Item1 : _pagesPerSheetArrangements[(int)info.PagesPerSheet].Item2;
            int rows = info.Orientation == PrintSettings.PageOrientation.Portrait ? _pagesPerSheetArrangements[(int)info.PagesPerSheet].Item2 : _pagesPerSheetArrangements[(int)info.PagesPerSheet].Item1;

            int index = 0;
            double pageWidth = result.DocumentPaginator.PageSize.Width;
            double pageHeight = result.DocumentPaginator.PageSize.Height;
            double elementWidth = pageWidth / columns;
            double elementHeight = pageHeight / rows;
            FrameworkElement actionAddElement(int column, int row)
            {
                System.Windows.Documents.FixedPage element = document[index];
                index++;

                double ratio = pageHeight * (elementWidth / pageWidth) <= elementHeight ? elementWidth / pageWidth : elementHeight / pageHeight;
                element.Width = pageWidth * ratio;
                element.Height = pageHeight * ratio;
                element.RenderTransform = new System.Windows.Media.ScaleTransform(ratio, ratio);
                element.RenderTransformOrigin = new Point(0, 0);
                Decorator elementContainer = new Decorator() { Child = element, Width = elementWidth, Height = elementHeight, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                elementContainer.SetValue(Grid.RowProperty, column);
                elementContainer.SetValue(Grid.ColumnProperty, row);
                return elementContainer;
            }

            while (index < document.Count)
            {
                System.Windows.Documents.FixedPage page = new System.Windows.Documents.FixedPage() { Width = pageWidth, Height = pageHeight };
                Grid pageGrid = new Grid() { Width = page.Width, Height = page.Height };
                for (int i = 0; i < rows; i++)
                {
                    pageGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                }
                for (int i = 0; i < columns; i++)
                {
                    pageGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }
                switch (info.PageOrder)
                {
                    case PrintSettings.PageOrder.Horizontal:
                        for (int i = 0; i < rows; i++)
                        {
                            for (int j = 0; j < columns && index < document.Count; j++)
                            {
                                pageGrid.Children.Add(actionAddElement(i, j));
                            }
                        }
                        break;
                    case PrintSettings.PageOrder.HorizontalReverse:
                        for (int i = 0; i < rows; i++)
                        {
                            for (int j = columns - 1; j >= 0 && index < document.Count; j--)
                            {
                                pageGrid.Children.Add(actionAddElement(i, j));
                            }
                        }
                        break;
                    case PrintSettings.PageOrder.Vertical:
                        for (int j = 0; j < columns; j++)
                        {
                            for (int i = 0; i < rows && index < document.Count; i++)
                            {
                                pageGrid.Children.Add(actionAddElement(i, j));
                            }
                        }
                        break;
                    case PrintSettings.PageOrder.VerticalReverse:
                        for (int j = 0; j < columns; j++)
                        {
                            for (int i = rows - 1; i >= 0 && index < document.Count; i--)
                            {
                                pageGrid.Children.Add(actionAddElement(i, j));
                            }
                        }
                        break;
                    default:
                        return result;
                }
                page.Children.Add(pageGrid);
                if (info.Scale != double.NaN)
                {
                    page.RenderTransformOrigin = new Point(0, 0);
                    page.RenderTransform = new System.Windows.Media.ScaleTransform(info.Scale / 100.0, info.Scale / 100.0);
                }
                result.Pages.Add(new System.Windows.Documents.PageContent() { Child = page });
            }
            return result;
        }
    }
}
