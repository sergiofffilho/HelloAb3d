using Ab3d.Common.EventManager3D;
using Ab3d.Utilities;
using Ab3d.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelloWorldAmaro
{
    public class UtilsProject
    {
        private Random rnd = new Random();
        private MainWindow _mainWindow;

        public UtilsProject(MainWindow window){
            _mainWindow = window;
        }

        public MeshGeometry3D MakeMeshTriangles(Point3D[] positions, int[] triangles)
        {
            return new MeshGeometry3D()
            {
                Positions = new Point3DCollection(positions),

                TriangleIndices = new Int32Collection(triangles)
            };
        }

        public Point3D[] GenarateRandomPoints(int length, int start) {
            Point3D[] newArray = new Point3D[length];
            for (int i = 0; i < length; i++) {
                newArray[i] = new Point3D(
                    start + rnd.Next(start - 100, start + 100),
                    start + rnd.Next(start - 100, start + 100),
                    start + rnd.Next(start - 100, start + 100)
                    );
                start = start + rnd.Next(start - 50, start + 50);
            }

            return newArray;
        }

        public void CriarMesh()
        {

            DiffuseMaterial material = new DiffuseMaterial()
            {
                Brush = new SolidColorBrush(Colors.Red)
            };

            _mainWindow.test = new ConeVisual3D()
            {
                BottomCenterPosition = new Point3D(0, 0, 0),
                BottomRadius = 10,
                TopRadius = 0,
                Height = 20,
                Material = material
            };

            _mainWindow.ModelVisual.Children.Add(_mainWindow.test);

            _mainWindow.mesh2 = MakeMeshTriangles(GenarateRandomPoints(8, 0), new[]
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
                Geometry = _mainWindow.mesh2,
                Material = new DiffuseMaterial(Brushes.LightGreen),
                BackMaterial = new DiffuseMaterial(Brushes.Red),
            };

            _mainWindow.ModelVisual.Content = geometryModel2;


            _mainWindow.MeshInspector2.MeshGeometry3D = _mainWindow.mesh2;
        }

        public void WaitTime(int miliseconds) 
        {
            System.Threading.Thread.Sleep(miliseconds);
        }

        public void Setup3DObjects()
        {
            var wireGridVisual3D = new Ab3d.Visuals.WireGridVisual3D()
            {
                Size = new System.Windows.Size(1000, 1000),
                HeightCellsCount = 10,
                WidthCellsCount = 10,
                LineThickness = 3,
                CenterPosition = new Point3D(1000 + 50, 0, 1000 + 50)
            };

            _mainWindow.Viewport1.Children.Add(wireGridVisual3D);

            _mainWindow.controle.MatrizPosisoes = new Box[7, 7];
            int positionx, positiony = positionx = 0;
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
                        CenterPosition = new Point3D(1000 + x * 100, height / 2, 1000 + y * 100),
                        Size = new Size3D(80, height, 80),
                        Material = _mainWindow._normalMaterial
                    };

                    _mainWindow.ModelVisual.Children.Add(boxVisual3D);

                    Box currentBox = new Box() { box = boxVisual3D };

                    _mainWindow.controle.MatrizPosisoes[positionx, positiony] = currentBox;

                    var visualEventSource3D = new VisualEventSource3D(boxVisual3D);
                    visualEventSource3D.MouseEnter += _mainWindow.controle.BoxOnMouseEnter;
                    visualEventSource3D.MouseLeave += _mainWindow.controle.BoxOnMouseLeave;
                    visualEventSource3D.MouseClick += _mainWindow.controle.BoxOnMouseClick;
                    visualEventSource3D.MouseDrag += new MouseDrag3DEventHandler(_mainWindow.controle.movableEventSource3D_MouseDrag);


                    _mainWindow.EventsButtons._eventManager3D.RegisterEventSource3D(visualEventSource3D);
                    positionx += 1;

                }
                positionx = 0;
                positiony += 1;
            }

            //ToggleCameraAnimation(); // Start camer animation
        }
    }

}




