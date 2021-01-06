using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ab3d.Visuals;
using System.Linq;
using HelloWorldAmaro.Teste1;
using Ab3d.Controls;
using Ab3d.Common.Cameras;
using Ab3d.Utilities;
using System;
using Ab3d.Common.EventManager3D;

namespace HelloWorldAmaro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        private UtilsProject utils = new UtilsProject();
        private Ab3d.Utilities.EventManager3D _eventManager3D;

        private bool _isSelectedBoxClicked;

        private DiffuseMaterial _normalMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Silver);
        private DiffuseMaterial _selectedMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Orange);
        private DiffuseMaterial _clickedMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Red);


        public MeshGeometry3D mesh2;
        private ConeVisual3D test;
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

            test = new ConeVisual3D() 
            {
                BottomCenterPosition = new Point3D(0,0,0),
                BottomRadius = 10, TopRadius = 0, Height = 20 ,
                Material = material
            };
           
            ModelVisual.Children.Add(test);

            mesh2 = utils.MakeMeshTriangles(utils.GenarateRandomPoints(8, 0), new[]
                {
                    0, 2, 1,
                    3, 2, 0,
                    3, 5, 4,
                    5, 6, 4,
                    7, 3, 4,

                }
            );

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

            Setup3DObjects();
            Camera1.CameraChanged += delegate (object sender, CameraChangedRoutedEventArgs args)
                {
                    _eventManager3D.UpdateHitObjects();
                };
        }

        public void RecreateModelButtonOnClick(object sender, RoutedEventArgs e) {
            // TODO: Criar função GenararteTriangleIndices in utils
            mesh2 = utils.MakeMeshTriangles(utils.GenarateRandomPoints(8, 0), new[]
                {
                    0, 2, 1,
                    3, 2, 0,
                    3, 5, 4,
                    5, 6, 4,
                    7, 3, 4,

                }
            );
            test.SetValue(ConeVisual3D.BottomCenterPositionProperty, utils.GenarateRandomPoints(1, 0)[0]) ;

            var geometryModel2 = new GeometryModel3D()
            {
                Geometry = mesh2,
                Material = new DiffuseMaterial(Brushes.LightGreen),
                BackMaterial = new DiffuseMaterial(Brushes.Red),
            };
            ModelVisual.Content = geometryModel2;
            //MeshInspector2.MeshGeometry3D = mesh2;
            MeshInspector2.SetValue(MeshInspectorOverlay.MeshGeometry3DProperty, mesh2);
        }
        // EventManager3D checks the hit 3D object only on mouse events.
        // But when camera is changed, this can also change the 3D object that is behind the mouse.
        // To support that can subscribe to CameraChanged and in event handler call UpdateHitObjects method.
        private void Setup3DObjects()
        {
            _eventManager3D = new Ab3d.Utilities.EventManager3D(Viewport1);


            var wireGridVisual3D = new Ab3d.Visuals.WireGridVisual3D()
            {
                Size = new System.Windows.Size(1000, 1000),
                HeightCellsCount = 10,
                WidthCellsCount = 10,
                LineThickness = 3
            };

            Viewport1.Children.Add(wireGridVisual3D);


            // Create 7 x 7 boxes with different height
            for (int y = -3; y <= 3; y++)
            {
                for (int x = -3; x <= 3; x++)
                {
                    // Height is based on the distance from the center
                    double height = (5 - Math.Sqrt(x * x + y * y)) * 60;

                    // Create the 3D Box visual element

                    var boxVisual3D = new Ab3d.Visuals.BoxVisual3D()
                    {
                        CenterPosition = new Point3D(x * 100, height / 2, y * 100),
                        Size = new Size3D(80, height, 80),
                        Material = _normalMaterial
                    };

                    Viewport1.Children.Add(boxVisual3D);

                    var visualEventSource3D = new VisualEventSource3D(boxVisual3D);
                    visualEventSource3D.MouseEnter += BoxOnMouseEnter;
                    visualEventSource3D.MouseLeave += BoxOnMouseLeave;
                    visualEventSource3D.MouseClick += BoxOnMouseClick;

                    _eventManager3D.RegisterEventSource3D(visualEventSource3D);
                }
            }

            ToggleCameraAnimation(); // Start camer animation
        }

        private void BoxOnMouseEnter(object sender, Mouse3DEventArgs mouse3DEventArgs)
        {
            var boxVisual3D = mouse3DEventArgs.HitObject as Ab3d.Visuals.BoxVisual3D;
            if (boxVisual3D == null)
                return; // This should not happen

            // Set _isSelectedBoxClicked to true if the selected box is clicked (red) - this will be used on MouseLeave
            //_isSelectedBoxClicked = ReferenceEquals(boxVisual3D.Material, _clickedMaterial);

            boxVisual3D.Material = _selectedMaterial;
        }

        private void BoxOnMouseLeave(object sender, Mouse3DEventArgs mouse3DEventArgs)
        {
            var boxVisual3D = mouse3DEventArgs.HitObject as Ab3d.Visuals.BoxVisual3D;
            if (boxVisual3D == null)
                return; // This should not happen

            if (_isSelectedBoxClicked)
                boxVisual3D.Material = _clickedMaterial;
            else
                boxVisual3D.Material = _normalMaterial;
        }
        private void BoxOnMouseClick(object sender, MouseButton3DEventArgs mouseButton3DEventArgs)
        {
            var boxVisual3D = mouseButton3DEventArgs.HitObject as Ab3d.Visuals.BoxVisual3D;
            if (boxVisual3D == null)
                return; // This should not happen

            // Toggle clicked and normal material
            if (!_isSelectedBoxClicked)
            {
                boxVisual3D.Material = _clickedMaterial;
                _isSelectedBoxClicked = true;

                //_totalClickedHeight += boxVisual3D.Size.Y;
            }
            else
            {
                boxVisual3D.Material = _normalMaterial;
                _isSelectedBoxClicked = false;

                //_totalClickedHeight -= boxVisual3D.Size.Y;
            }

            //UpdateTotalClickedHeightText();
        }
        private void ToggleCameraAnimation()
        {
            if (Camera1.IsRotating)
            {
                Camera1.StopRotation();
                //AnimateButton.Content = "Start animation";
            }
            else
            {
                Camera1.StartRotation(10, 0); // animate with changing heading for 10 degrees in one second
                //AnimateButton.Content = "Stop animation";
            }
        }
    }

    public class Amaro
    {
        public string HelloText { get; set;}
    }
}
