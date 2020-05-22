using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pricimage.Helpers;

namespace Pricimage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            cboCode.ItemsSource = ReadFileList();
        }

        private HistoryItem currentItem;
        private List<HistoryItem> history = new List<HistoryItem>();
        private string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

        private List<string> ReadFileList()
        {
            List<string> result = new List<string>();

            var imageFiles = Directory.GetFiles(imagePath);
            if (imageFiles != null && imageFiles.Any())
            {
                foreach (var item in imageFiles)
                {
                    var code = System.IO.Path.GetFileNameWithoutExtension(item);
                    result.Add(code);
                }
            }
            else
            {
                var messageBoxResult = MessageBox.Show("Chưa có thư mục Images hoặc thư mục Images trống!", "Lỗi cài đặt");
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    this.Close();
                }
            }

            return result;
        }

        private void HandleEnterKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleDraw();
            }
        }

        private void HandleDraw()
        {
            // Load data
            currentItem = new HistoryItem
            {
                Code = cboCode.SelectedItem.ToString()
            };
            currentItem.OldPrice = txtOldPrice.Text;
            currentItem.NewPrice = txtNewPrice.Text;
            currentItem.Note = txtNote.Text;
            currentItem.IsSavedToHistory = false;

            // Draw text on image and temporary save output in temp folder
            var image = DrawPreview(currentItem);

            // Show preview
            imgPreview.Source = image;
        }

        private bool IsValidItem(HistoryItem item)
        {
            return item != null && !string.IsNullOrEmpty(item.Code) && !string.IsNullOrEmpty(item.NewPrice);
        }

        private void AddToHistory(HistoryItem item)
        {
            if (IsValidItem(item) && !item.IsSavedToHistory)
            {
                history = history.Prepend(item).ToList();
                lstHistory.ItemsSource = history;

                item.IsSavedToHistory = true;
            }
        }

        private DrawingImage DrawPreview(HistoryItem item)
        {
            DrawingGroup drawingGroup = new DrawingGroup();

            // Background: Product image
            BitmapImage image = new BitmapImage(new Uri(currentItem.ImageSource, UriKind.Relative));
            ImageDrawing imageDrawing = new ImageDrawing(image, new Rect(0, 0, image.Width, image.Height));
            imageDrawing.Freeze();
            drawingGroup.Children.Add(imageDrawing);

            // Rectangle in the center
            GeometryDrawing rectangle = new GeometryDrawing();
            double rectRatio = 0.5;
            double rectWidth = image.Width * rectRatio;
            double rectHeight = image.Height * rectRatio;
            double rectX = (image.Width - rectWidth) / 2;
            double rectY = (image.Height - rectHeight) / 2;
            var rectangleBrush = new SolidColorBrush(Colors.Gray);
            rectangleBrush.Opacity = 0.7;
            rectangle.Brush = rectangleBrush;
            rectangle.Geometry = new RectangleGeometry(new Rect(rectX, rectY, rectWidth, rectHeight));
            drawingGroup.Children.Add(rectangle);

            double fontSize = image.Width * 0.08;
            double lineHeight = fontSize * 1.2;
            double textX = rectX + fontSize / 10;
            double textY = rectY + lineHeight / 10;

            // Text inside rectangle
            DrawingVisual visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            { 
                context.DrawText(new FormattedText(item.TimeStamp.ToString("dd/MM/yyyy")
                    , CultureInfo.InvariantCulture
                    , FlowDirection.LeftToRight
                    , new Typeface("Arial")
                    , fontSize
                    , Brushes.White
                    , VisualTreeHelper.GetDpi(this).PixelsPerDip), new Point(textX, textY));

                context.DrawText(new FormattedText((!string.IsNullOrEmpty(item.OldPrice)? item.OldPrice + " => " : "") + item.NewPrice
                    , CultureInfo.InvariantCulture
                    , FlowDirection.LeftToRight
                    , new Typeface("Arial")
                    , fontSize
                    , Brushes.White
                    , VisualTreeHelper.GetDpi(this).PixelsPerDip), new Point(textX, textY + lineHeight));

                if (!string.IsNullOrEmpty(item.Note))
                {
                    context.DrawText(new FormattedText(item.Note
                    , CultureInfo.InvariantCulture
                    , FlowDirection.LeftToRight
                    , new Typeface("Arial")
                    , fontSize
                    , Brushes.White
                    , VisualTreeHelper.GetDpi(this).PixelsPerDip), new Point(textX, textY + 2 * lineHeight));
                }
            }
            drawingGroup.Children.Add(visual.Drawing);

            DrawingImage drawingImage = new DrawingImage(drawingGroup);
            return drawingImage;
        }

        private void cboCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                HandleDraw();
            }
        }

        private void txtOldPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleDraw();
        }

        private void txtNewPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleDraw();
        }

        private void txtNote_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleDraw();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            SaveImageToClipboard(currentItem);
            e.Handled = true;
        }

        private void SaveImageToClipboard(HistoryItem item)
        {
            Clipboard.SetImage(ImageHelper.ConvertToBitmapSource(imgPreview));
            AddToHistory(item);
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.C)
                {
                    SaveImageToClipboard(currentItem);
                    e.Handled = true;
                }
                else if (e.Key == Key.S)
                {
                    SaveImage(currentItem);
                    e.Handled = true;
                }
            }
        }

        private void txtOldPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            txtOldPrice.SelectAll();
        }

        private void txtNewPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            txtNewPrice.SelectAll();
        }

        private void txtNote_GotFocus(object sender, RoutedEventArgs e)
        {
            txtNote.SelectAll();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveImage(currentItem);
            e.Handled = true;
        }

        private void SaveImage(HistoryItem item)
        {
            ImageHelper.SaveToPng(imgPreview, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");
            AddToHistory(item);
        }

        private void lstHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HistoryItem item = (HistoryItem)lstHistory.SelectedItem;
            cboCode.SelectedItem = item.Code;
            txtOldPrice.Text = item.OldPrice;
            txtNewPrice.Text = item.NewPrice;
            txtNote.Text = item.Note;

            HandleDraw();
        }
    }
}
