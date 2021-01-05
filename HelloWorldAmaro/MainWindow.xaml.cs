using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ab3d.Visuals;
using System.Linq;

namespace HelloWorldAmaro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {

        public MeshGeometry3D mesh2;
        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //    Amaro amaro = new Amaro { HelloText = "Eu sou crazy" };
            //    this.MyGrid.DataContext = amaro;
        }
        //private GeometryModel3D _geometryModel3;

        public MainWindow()
        {
            InitializeComponent();

            DiffuseMaterial material = new DiffuseMaterial()
            {
                Brush = new SolidColorBrush(Colors.Red)
            };

            ConeVisual3D test = new ConeVisual3D() 
            {
                   BottomCenterPosition = new Point3D(0,0,0),
                BottomRadius = 10, TopRadius = 0, Height = 20 ,
                Material = material
            };
           
            ModelVisual.Children.Add(test);

            mesh2 = new MeshGeometry3D()
            {
                Positions = new Point3DCollection(new[]
                {
                    new Point3D(5,   0, 5),
                    new Point3D(100, 0, 5),
                    new Point3D(100, 0, 50),
                    new Point3D(5,   0, 50)
                }),

                TriangleIndices = new Int32Collection(new[]
                {
                    0, 2, 1,
                    3, 2, 0
                })
            };

            var geometryModel2 = new GeometryModel3D()
            {
                Geometry = mesh2,
                Material = new DiffuseMaterial(Brushes.LightGreen),
                BackMaterial = new DiffuseMaterial(Brushes.Red),
            };

            ModelVisual.Content = geometryModel2;
            //var modelVisual2 = new ModelVisual3D()
            //{
            //    Content = geometryModel2
            //};

            //// Using CreateModelVisual3D extensiton method from Ab3d.PowerToys
            //var modelVisual2 = geometryModel2.CreateModelVisual3D();
            //Viewport2.Children.Add(modelVisual2);

            //// in one line:
            //Viewport2.Children.Add(geometryModel2.CreateModelVisual3D());

            //Viewport2.Children.Add(modelVisual2);

            MeshInspector2.MeshGeometry3D = mesh2;
        }

        public void RecreateModelButtonOnClick(object sender, RoutedEventArgs e) {
            mesh2.Positions = new Point3DCollection(new[]
                {
                    new Point3D(5,   0, 5),
                    new Point3D(500, 0, 5),
                    new Point3D(100, 0, 50),
                    new Point3D(5,   0, 50)
                });

        }

    }

    public class Amaro
    {
        public string HelloText { get; set;}
    }
}
