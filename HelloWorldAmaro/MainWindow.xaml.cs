using System.Windows;

namespace HelloWorldAmaro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Amaro amaro = new Amaro { HelloText = "Eu sou crazy" };
            this.MyGrid.DataContext = amaro;
        }
    }

    public class Amaro
    {
        public string HelloText { get; set;}
    }
}
