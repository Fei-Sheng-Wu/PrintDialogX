using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace PrintDialogX.Internal
{
    internal sealed class PreviewPage(int index, IEnumerable<PrintPage> subpages, PreviewPage.Construction construction)
    {
        internal sealed class Construction(Size document, Size extent, double horizontal, double vertical, double margin, int columns, int rows, Enums.PageOrder order, double scale)
        {
            public Size DocumentSize { get; } = document;

            public double ExtentWidth { get; } = extent.Width;
            public double ExtentHeight { get; } = extent.Height;
            public double CellWidth { get; } = horizontal;
            public double CellHeight { get; } = vertical;
            public double Margin { get; } = margin;

            public int ColumnCount { get; } = columns;
            public int RowCount { get; } = rows;
            public Enums.PageOrder Order { get; } = order;

            public ScaleTransform Scaling { get; } = Common.Freeze(new ScaleTransform(scale, scale));
            public RectangleGeometry Clip { get; } = Common.Freeze(new RectangleGeometry(new(0, 0, horizontal / scale, vertical / scale)));
        }

        public int Index { get; } = index;

        private Canvas? content = null;

        public Canvas ConstructContent()
        {
            if (content is Canvas cache)
            {
                return cache;
            }

            content = new();

            int step = 0;
            foreach (PrintPage page in subpages)
            {
                (int column, int row) = construction.Order switch
                {
                    Enums.PageOrder.Horizontal => (step % construction.ColumnCount, step / construction.ColumnCount),
                    Enums.PageOrder.HorizontalReverse => (construction.ColumnCount - step % construction.ColumnCount - 1, step / construction.ColumnCount),
                    Enums.PageOrder.Vertical => (step / construction.RowCount, step % construction.RowCount),
                    Enums.PageOrder.VerticalReverse => (step / construction.RowCount, construction.RowCount - step % construction.RowCount - 1),
                    _ => (0, 0)
                };
                step++;

                if (page.Content is not FrameworkElement element)
                {
                    continue;
                }

                if (element.Parent is ContentPresenter former)
                {
                    former.Content = null;
                }
                else if (element.Parent is DependencyObject parent)
                {
                    throw new PrintDocumentException(element, "The content is already the child of another element.", parent);
                }

                ContentPresenter cell = new()
                {
                    Width = construction.ExtentWidth,
                    Height = construction.ExtentHeight,
                    Content = element,
                    RenderTransform = construction.Scaling,
                    Clip = construction.Clip
                };
                Canvas.SetLeft(cell, construction.Margin + construction.CellWidth * column);
                Canvas.SetTop(cell, construction.Margin + construction.CellHeight * row);
                content.Children.Add(cell);
            }

            content.Measure(construction.DocumentSize);
            content.Arrange(new(construction.DocumentSize));

            return content;
        }

        public DocumentPage Paginate()
        {
            return new(ConstructContent(), construction.DocumentSize, new(construction.DocumentSize), new(construction.DocumentSize));
        }
    }

    internal sealed class PreviewDocument(PrintDialogViewModel.ModelLocker locker) : DocumentPaginator()
    {
        public List<PreviewPage> Pages { get; } = [];
        public PrintDialogViewModel.ModelLocker Locker { get; } = locker;

        public override bool IsPageCountValid { get; } = true;
        public override int PageCount { get => Pages.Count; }
        public override Size PageSize { get; set; } = new(0, 0);
        public override IDocumentPaginatorSource? Source { get; } = null;

        public override DocumentPage GetPage(int index)
        {
            return index >= 0 && index < Pages.Count ? Pages[index].Paginate() : DocumentPage.Missing;
        }
    }

    internal sealed class DocumentToContentConverter() : LanguageHostConverter(), IValueConverter
    {
        internal sealed class Content(PreviewPage page, string name)
        {
            public PreviewPage Page { get; } = page;
            public string Name { get; } = name;
        }

        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value is not PreviewDocument document)
            {
                return Binding.DoNothing;
            }

            using (document.Locker.Lock())
            {
                string construction = GetText(TextResource.ConstructionPage);
                Content[] contents = new Content[document.Pages.Count];
                for (int i = 0; i < contents.Length; i++)
                {
                    contents[i] = new(document.Pages[i], string.Format(culture, construction, i + 1, document.Pages.Count));
                }

                return contents;
            }
        }

        public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    internal sealed class DocumentToDescriptionConverter() : LanguageHostConverter(), IMultiValueConverter
    {
        public object Convert(object[] values, Type type, object parameter, CultureInfo culture)
        {
            return (values[0], values[1]) is (int current, int total) ? string.Format(culture, GetText(TextResource.ConstructionPage), current, total) : Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] types, object parameter, CultureInfo culture)
        {
            return [.. Enumerable.Repeat(Binding.DoNothing, types.Length)];
        }
    }

    internal sealed class PreviewPageControl : Border
    {
        internal sealed class ColorEffect : ShaderEffect
        {
            public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(nameof(Input), typeof(ColorEffect), 0);
            public static readonly DependencyProperty ViewportLeftProperty = DependencyProperty.Register(nameof(ViewportLeft), typeof(float), typeof(ColorEffect), new(0.0f, PixelShaderConstantCallback(0)));
            public static readonly DependencyProperty ViewportTopProperty = DependencyProperty.Register(nameof(ViewportTop), typeof(float), typeof(ColorEffect), new(0.0f, PixelShaderConstantCallback(1)));
            public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register(nameof(ViewportWidth), typeof(float), typeof(ColorEffect), new(0.0f, PixelShaderConstantCallback(2)));
            public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register(nameof(ViewportHeight), typeof(float), typeof(ColorEffect), new(0.0f, PixelShaderConstantCallback(3)));

            public Brush Input
            {
                get => (Brush)GetValue(InputProperty);
                set => SetValue(InputProperty, value);
            }
            public float ViewportLeft
            {
                get => (float)GetValue(ViewportLeftProperty);
                set => SetValue(ViewportLeftProperty, value);
            }
            public float ViewportTop
            {
                get => (float)GetValue(ViewportTopProperty);
                set => SetValue(ViewportTopProperty, value);
            }
            public float ViewportWidth
            {
                get => (float)GetValue(ViewportWidthProperty);
                set => SetValue(ViewportWidthProperty, value);
            }
            public float ViewportHeight
            {
                get => (float)GetValue(ViewportHeightProperty);
                set => SetValue(ViewportHeightProperty, value);
            }

            public ColorEffect(string shader) : base()
            {
                PixelShader = new()
                {
                    UriSource = new(Common.Format("/PrintDialogX;component/Resources/Effects/{0}", [shader]), UriKind.Relative)
                };
                UpdateShaderValue(InputProperty);
                UpdateShaderValue(ViewportLeftProperty);
                UpdateShaderValue(ViewportTopProperty);
                UpdateShaderValue(ViewportWidthProperty);
                UpdateShaderValue(ViewportHeightProperty);
            }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(PreviewPage), typeof(PreviewPageControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, (x, e) =>
        {
            PreviewPageControl element = (PreviewPageControl)x;
            if (e is not { NewValue: PreviewPage page })
            {
                element.ClearContent();
                return;
            }

            VisualBrush visual = new(page.ConstructContent())
            {
                ViewboxUnits = BrushMappingMode.Absolute,
                AutoLayoutContent = false
            };
            Rectangle bound = new()
            {
                Fill = visual
            };
            element.ContentBrush = (new(bound), bound, visual);
        }));
        public static readonly DependencyProperty ViewerProperty = DependencyProperty.Register(nameof(Viewer), typeof(PreviewDocumentControl), typeof(PreviewPageControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Enums.Color), typeof(PreviewPageControl), new FrameworkPropertyMetadata(Enums.Color.Color, FrameworkPropertyMetadataOptions.AffectsRender, (x, e) =>
        {
            PreviewPageControl element = (PreviewPageControl)x;
            if (e is not { NewValue: Enums.Color color })
            {
                return;
            }

            element.UpdateColor(color);
        }));
        public static readonly DependencyProperty ColorEmulationLevelProperty = DependencyProperty.Register(nameof(ColorEmulationLevel), typeof(ColorEmulationLevel), typeof(PreviewPageControl), new FrameworkPropertyMetadata(ColorEmulationLevel.Simple, FrameworkPropertyMetadataOptions.AffectsRender));

        public PreviewPage? Content
        {
            get => (PreviewPage?)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
        public PreviewDocumentControl? Viewer
        {
            get => (PreviewDocumentControl?)GetValue(ViewerProperty);
            set => SetValue(ViewerProperty, value);
        }
        public Enums.Color Color
        {
            get => (Enums.Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public ColorEmulationLevel ColorEmulationLevel
        {
            get => (ColorEmulationLevel)GetValue(ColorEmulationLevelProperty);
            set => SetValue(ColorEmulationLevelProperty, value);
        }

        public (VisualBrush Brush, Rectangle Bound, VisualBrush Visual)? ContentBrush { get; set; } = null;

        private Rect viewport = new(0, 0, 0, 0);
        private (Enums.Color Color, ColorEffect? Effect) color = (Enums.Color.Color, null);

        public PreviewPageControl() : base()
        {
            Loaded += (x, e) => CompositionTarget.Rendering += UpdateViewport;
            Unloaded += (x, e) =>
            {
                CompositionTarget.Rendering -= UpdateViewport;
                ClearContent();
            };
        }

        private void ClearContent()
        {
            ContentBrush?.Visual.Visual = null;
            ContentBrush?.Bound.Fill = null;
            ContentBrush = null;
        }

        private void UpdateViewport(object? sender, EventArgs e)
        {
            if (Viewer is not PreviewDocumentControl viewer)
            {
                return;
            }

            Point origin = viewer.TranslatePoint(new(0, 0), this);
            Point extent = viewer.TranslatePoint(new(viewer.ViewportWidth, viewer.ViewportHeight), this);
            if ((extent - origin).Length <= 0)
            {
                return;
            }

            Rect intersection = Common.Validate(Rect.Intersect(new(RenderSize), new(origin, extent)), x => !x.IsEmpty, new(0, 0, 0, 0));
            if (intersection != viewport)
            {
                viewport = intersection;
                InvalidateVisual();
            }
        }

        private void UpdateColor(Enums.Color value)
        {
            color = (value, (ColorEmulationLevel, value) switch
            {
                (ColorEmulationLevel.Simple, Enums.Color.Grayscale or Enums.Color.Monochrome) => new("Grayscale.ps"),
                (ColorEmulationLevel.Full, Enums.Color.Grayscale) => new("Grayscale.ps"),
                (ColorEmulationLevel.Full, Enums.Color.Monochrome) => new("Monochrome.ps"),
                _ => null
            });
        }

        protected override void OnRender(DrawingContext context)
        {
            if ((ContentBrush, Viewer) is not ((VisualBrush brush, Rectangle bound, VisualBrush visual), PreviewDocumentControl viewer))
            {
                return;
            }

            visual.Viewbox = viewer.TranslateViewport(viewport);
            bound.Width = viewport.Width;
            bound.Height = viewport.Height;
            bound.Effect = color.Effect;
            color.Effect?.ViewportLeft = (float)visual.Viewbox.X;
            color.Effect?.ViewportTop = (float)visual.Viewbox.Y;
            color.Effect?.ViewportWidth = (float)visual.Viewbox.Width;
            color.Effect?.ViewportHeight = (float)visual.Viewbox.Height;

            context.DrawRectangle(Brushes.White, null, viewport);
            context.DrawRectangle(brush, null, viewport);

            base.OnRender(context);
        }
    }

    internal sealed class PreviewDocumentControl : VirtualizingPanel, IScrollInfo
    {
        internal enum Zoom
        {
            Custom,
            FitToWidth,
            FitToHeight,
            FitToPage
        }

        public const int COUNT_CACHE = 1;
        public const double EPSILON_ZOOM = 0.35;
        public const double EPSILON_NAVIGATION = 0.1;
        public const double LENGTH_SCROLL_DELTA_LINE = 16;
        public const double LENGTH_SCROLL_DELTA_WHEEL = 48;
        public const double PERCENTAGE_ZOOM_MINIMUM = 0.05;
        public const double PERCENTAGE_ZOOM_MAXIMUM = 10000;
        public const double PERCENTAGE_ZOOM_DELTA_BUTTON = 0.25;
        public const double PERCENTAGE_ZOOM_DELTA_WHEEL = 0.15;

        public static readonly DependencyProperty DocumentSizeProperty = DependencyProperty.Register(nameof(DocumentSize), typeof(Size), typeof(PreviewDocumentControl), new FrameworkPropertyMetadata(new Size(0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty DocumentIndexProperty = DependencyProperty.Register(nameof(DocumentIndex), typeof(int), typeof(PreviewDocumentControl), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.None));

        public Size DocumentSize
        {
            get => (Size)GetValue(DocumentSizeProperty);
            set => SetValue(DocumentSizeProperty, value);
        }
        public int DocumentIndex
        {
            get => (int)GetValue(DocumentIndexProperty);
            set => SetValue(DocumentIndexProperty, value);
        }

        public double Spacing { get; set; } = 0;

        public bool CanHorizontallyScroll { get; set; } = true;
        public bool CanVerticallyScroll { get; set; } = true;
        public double ViewportWidth { get; set; } = 0;
        public double ViewportHeight { get; set; } = 0;
        public double ExtentWidth { get; set; } = 0;
        public double ExtentHeight { get; set; } = 0;
        public double HorizontalOffset { get; set; } = 0;
        public double VerticalOffset { get; set; } = 0;
        public ScrollViewer? ScrollOwner { get; set; } = null;

        private (Zoom Mode, double Percentage) zoom = (Zoom.FitToWidth, 1);
        private int columns = 1;

        public PreviewDocumentControl() : base()
        {
            PreviewMouseWheel += (x, e) =>
            {
                switch (Keyboard.Modifiers)
                {
                    case ModifierKeys.Shift:
                        SetHorizontalOffset(HorizontalOffset - e.Delta);
                        e.Handled = true;
                        break;
                    case ModifierKeys.Control:
                        UpdateZoom(Zoom.Custom, zoom.Percentage * (1 + Math.Sign(e.Delta) * PERCENTAGE_ZOOM_DELTA_WHEEL), e.GetPosition(this));
                        InvalidateMeasure();
                        e.Handled = true;
                        break;
                }
            };
        }

        private int GetItemCount()
        {
            return ItemsControl.GetItemsOwner(this)?.ItemContainerGenerator?.Items.Count ?? 0;
        }

        private double ComputeLayoutOffset(double unit, int repetition)
        {
            return unit * repetition + Spacing * (repetition + 1);
        }

        private double ComputeLayoutStep(double offset, double unit)
        {
            return Math.Max(0, offset - Spacing) / (unit + Spacing);
        }

        private double ComputeZoomPercentage(Zoom mode)
        {
            return mode switch
            {
                Zoom.FitToWidth => (ViewportWidth - Spacing * (columns + 1)) / DocumentSize.Width / columns,
                Zoom.FitToHeight => (ViewportHeight - Spacing * 2) / DocumentSize.Height,
                Zoom.FitToPage => Math.Min(ComputeZoomPercentage(Zoom.FitToWidth), ComputeZoomPercentage(Zoom.FitToHeight)),
                _ => zoom.Percentage
            };
        }

        private void UpdateZoom(Zoom mode, double percentage, Point? origin)
        {
            double original = zoom.Percentage;
            zoom = (mode, Common.Clamp(PERCENTAGE_ZOOM_MINIMUM, PERCENTAGE_ZOOM_MAXIMUM, percentage));

            double x = origin?.X ?? 0;
            double y = origin?.Y ?? 0;
            int columns = Common.Floor(ComputeLayoutStep(HorizontalOffset + x, original * DocumentSize.Width), null) + 1;
            int rows = Common.Floor(ComputeLayoutStep(VerticalOffset + y, original * DocumentSize.Height), null) + 1;
            SetHorizontalOffset(zoom.Percentage * (HorizontalOffset + x - Spacing * columns) / original - x + Spacing * columns);
            SetVerticalOffset(zoom.Percentage * (VerticalOffset + y - Spacing * rows) / original - y + Spacing * rows);
        }

        private void UpdateNavigation(int index)
        {
            SetHorizontalOffset(ComputeLayoutOffset(zoom.Percentage * DocumentSize.Width, index % columns) - Spacing);
            SetVerticalOffset(ComputeLayoutOffset(zoom.Percentage * DocumentSize.Height, index / columns) - Spacing);
        }

        public void ZoomMode(Zoom mode)
        {
            UpdateZoom(mode, ComputeZoomPercentage(mode), null);
            InvalidateMeasure();
        }

        public void ZoomIn()
        {
            UpdateZoom(Zoom.Custom, PERCENTAGE_ZOOM_DELTA_BUTTON * (Common.Floor(zoom.Percentage / PERCENTAGE_ZOOM_DELTA_BUTTON, EPSILON_ZOOM) + 1), null);
            InvalidateMeasure();
        }

        public void ZoomOut()
        {
            UpdateZoom(Zoom.Custom, PERCENTAGE_ZOOM_DELTA_BUTTON * (Common.Ceiling(zoom.Percentage / PERCENTAGE_ZOOM_DELTA_BUTTON, EPSILON_ZOOM) - 1), null);
            InvalidateMeasure();
        }

        public void ZoomActual()
        {
            UpdateZoom(Zoom.Custom, 1, null);
            InvalidateMeasure();
        }

        public void ZoomColumns(int count)
        {
            int current = DocumentIndex - 1;
            columns = count;
            UpdateZoom(Zoom.FitToPage, ComputeZoomPercentage(Zoom.FitToPage), null);
            UpdateNavigation(current);
            InvalidateMeasure();
        }

        public void NavigateIndex(int index)
        {
            UpdateNavigation(Common.Clamp(0, GetItemCount() - 1, index - 1));
        }

        public void NavigateFirst()
        {
            UpdateNavigation(0);
        }

        public void NavigatePrevious()
        {
            UpdateNavigation(Common.Clamp(0, GetItemCount() - 1, DocumentIndex - (zoom.Percentage * DocumentSize.Width + Spacing > ViewportWidth ? 1 : columns) - 1));
        }

        public void NavigateNext()
        {
            UpdateNavigation(Common.Clamp(0, GetItemCount() - 1, DocumentIndex + (zoom.Percentage * DocumentSize.Width + Spacing > ViewportWidth ? 1 : columns) - 1));
        }

        public void NavigateLast()
        {
            UpdateNavigation(GetItemCount() - 1);
        }

        public Rect TranslateViewport(Rect viewport)
        {
            return new(viewport.X / zoom.Percentage, viewport.Y / zoom.Percentage, viewport.Width / zoom.Percentage, viewport.Height / zoom.Percentage);
        }

        public Rect MakeVisible(Visual visual, Rect bound)
        {
            Point position = visual.TransformToAncestor(this).Transform(new(HorizontalOffset, VerticalOffset));

            double x = 0;
            if (position.X < HorizontalOffset)
            {
                x = position.X - HorizontalOffset;
            }
            else if (position.X + bound.Width > HorizontalOffset + ViewportWidth)
            {
                x = Math.Min(position.X - HorizontalOffset, position.X + bound.Width - HorizontalOffset - ViewportWidth);
            }
            SetHorizontalOffset(HorizontalOffset + x);

            double y = 0;
            if (position.Y < VerticalOffset)
            {
                y = position.Y - VerticalOffset;
            }
            else if (position.Y + bound.Height > VerticalOffset + ViewportHeight)
            {
                y = Math.Min(position.Y - VerticalOffset, position.Y + bound.Height - VerticalOffset - ViewportHeight);
            }
            SetVerticalOffset(VerticalOffset + y);

            return new(x, y, Math.Min(ViewportWidth, bound.Width), Math.Min(ViewportHeight, bound.Height));
        }

        public void SetHorizontalOffset(double offset)
        {
            HorizontalOffset = offset;
            InvalidateMeasure();
        }

        public void SetVerticalOffset(double offset)
        {
            VerticalOffset = offset;
            InvalidateMeasure();
        }

        public void LineLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LENGTH_SCROLL_DELTA_LINE);
        }

        public void LineRight()
        {
            SetHorizontalOffset(HorizontalOffset + LENGTH_SCROLL_DELTA_LINE);
        }

        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - LENGTH_SCROLL_DELTA_LINE);
        }

        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + LENGTH_SCROLL_DELTA_LINE);
        }

        public void MouseWheelLeft()
        {
            SetHorizontalOffset(HorizontalOffset - LENGTH_SCROLL_DELTA_WHEEL);
        }

        public void MouseWheelRight()
        {
            SetHorizontalOffset(HorizontalOffset + LENGTH_SCROLL_DELTA_WHEEL);
        }

        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - LENGTH_SCROLL_DELTA_WHEEL);
        }

        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + LENGTH_SCROLL_DELTA_WHEEL);
        }

        public void PageLeft()
        {
            SetHorizontalOffset(HorizontalOffset - ViewportWidth);
        }

        public void PageRight()
        {
            SetHorizontalOffset(HorizontalOffset + ViewportWidth);
        }

        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - ViewportHeight);
        }

        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + ViewportHeight);
        }

        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            RemoveInternalChildRange(0, Children.Count);
        }

        protected override Size MeasureOverride(Size available)
        {
            ViewportWidth = double.IsInfinity(available.Width) ? 0 : available.Width;
            ViewportHeight = double.IsInfinity(available.Height) ? 0 : available.Height;
            if (zoom.Mode is not Zoom.Custom)
            {
                UpdateZoom(zoom.Mode, ComputeZoomPercentage(zoom.Mode), null);
            }

            int count = GetItemCount();
            double width = zoom.Percentage * DocumentSize.Width;
            double height = zoom.Percentage * DocumentSize.Height;
            ExtentWidth = Math.Max(ViewportWidth, ComputeLayoutOffset(width, columns));
            ExtentHeight = ComputeLayoutOffset(height, Common.Partition(count, columns));

            HorizontalOffset = Common.Clamp(0, ExtentWidth - ViewportWidth, HorizontalOffset);
            VerticalOffset = Common.Clamp(0, ExtentHeight - ViewportHeight, VerticalOffset);
            DocumentIndex = Common.Clamp(1, count, Common.Floor(ComputeLayoutStep(VerticalOffset, zoom.Percentage * DocumentSize.Height), EPSILON_NAVIGATION) * columns + Common.Floor(ComputeLayoutStep(HorizontalOffset, zoom.Percentage * DocumentSize.Width), EPSILON_NAVIGATION) + 1);
            ScrollOwner?.InvalidateScrollInfo();

            if (ItemsControl.GetItemsOwner(this)?.ItemContainerGenerator is IRecyclingItemContainerGenerator generator)
            {
                int start = Common.Clamp(0, count, (Common.Floor(ComputeLayoutStep(VerticalOffset, height), null) - COUNT_CACHE) * columns);
                int end = Common.Clamp(0, count, (Common.Ceiling(ComputeLayoutStep(VerticalOffset + ViewportHeight, height), null) + COUNT_CACHE) * columns);

                GeneratorPosition position = generator.GeneratorPositionFromIndex(start);
                using (generator.StartAt(position, GeneratorDirection.Forward, true))
                {
                    for (int i = start, j = position.Offset != 0 ? position.Index + 1 : position.Index; i < end; i++, j++)
                    {
                        ContentPresenter element = (ContentPresenter)generator.GenerateNext(out bool isRealized);
                        element.Measure(new(width, height));
                        if (!isRealized && Children.Contains(element))
                        {
                            continue;
                        }

                        if (j >= Children.Count)
                        {
                            AddInternalChild(element);
                        }
                        else
                        {
                            InsertInternalChild(j, element);
                        }
                        generator.PrepareItemContainer(element);
                    }
                }

                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    int index = generator.IndexFromGeneratorPosition(new(i, 0));
                    if (index >= start && index < end)
                    {
                        continue;
                    }

                    if (index >= 0)
                    {
                        generator.Recycle(new(i, 0), 1);
                    }
                    RemoveInternalChildRange(i, 1);
                }
            }

            return new(ExtentWidth, ExtentHeight);
        }

        protected override Size ArrangeOverride(Size available)
        {
            if (ItemsControl.GetItemsOwner(this)?.ItemContainerGenerator is not IRecyclingItemContainerGenerator generator)
            {
                return available;
            }

            double width = zoom.Percentage * DocumentSize.Width;
            double height = zoom.Percentage * DocumentSize.Height;
            double margin = Math.Max(0, (ViewportWidth - ComputeLayoutOffset(width, columns)) / 2);
            for (int i = 0; i < Children.Count; i++)
            {
                int index = generator.IndexFromGeneratorPosition(new(i, 0));
                Children[i].Arrange(new(margin + ComputeLayoutOffset(width, index % columns) - HorizontalOffset, ComputeLayoutOffset(height, index / columns) - VerticalOffset, width, height));
            }

            return available;
        }
    }
}
