using Homework3.Classes;
using Homework3.Interfaces;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Homework3
{
    // extracts name from abstract animal, used in xaml for animal ListBox
    public class TypeNameConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.GetType().Name;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ILogger logger = new JsonLogger(); 
            // var logger = new XmlLogger();

            DataContext = new ViewModel(logger);
        }
    }
}