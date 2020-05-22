using System;
using System.Collections.Generic;
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

            List<HistoryItem> historyItems = new List<HistoryItem>
            {
                new HistoryItem{ Code = "02324590", NewPrice = 68},
                new HistoryItem{ Code = "05424654", OldPrice = 256.8, NewPrice = 245},
                new HistoryItem{ Code = "02324590", NewPrice = 68},
                new HistoryItem{ Code = "05424654", OldPrice = 256.8, NewPrice = 245},
                new HistoryItem{ Code = "02324590", NewPrice = 68},
                new HistoryItem{ Code = "05424654", OldPrice = 256.8, NewPrice = 245},
                new HistoryItem{ Code = "02324590", NewPrice = 68},
                new HistoryItem{ Code = "05424654", OldPrice = 256.8, NewPrice = 245},
            };
            lstHistory.ItemsSource = historyItems;

            cboCode.ItemsSource = ReadFileList();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private List<string> ReadFileList()
        {
            List<string> result = new List<string>();

            var imageFiles = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"));
            if (imageFiles != null && imageFiles.Any())
            {
                foreach (var item in imageFiles)
                {
                    var code = System.IO.Path.GetFileNameWithoutExtension(item);
                    result.Add(code);
                }
            }

            return result;
        }
    }
}
