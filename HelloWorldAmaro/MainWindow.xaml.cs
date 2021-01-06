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
using System.Collections.Generic;
using Ab3d.Animation;
using System.Threading.Tasks;

namespace HelloWorldAmaro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        private UtilsProject utils = new UtilsProject();
        private EventManager3D _eventManager3D;

        private bool _isSelectedBoxClicked;
        private Controle controle;

        private DiffuseMaterial _normalMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Silver);
        //private SolidColorBrush _normalColorSolid = new SolidColorBrush(System.Windows.Media.Brushes.Silver.Color);
        
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

            controle = new Controle(); 

            DiffuseMaterial material = new DiffuseMaterial()
            {
                Brush = new SolidColorBrush(Colors.Red)
            };

            test = new ConeVisual3D()
            {
                BottomCenterPosition = new Point3D(0, 0, 0),
                BottomRadius = 10, TopRadius = 0, Height = 20,
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
           

            MeshInspector2.MeshGeometry3D = mesh2;

            Setup3DObjects();
            Camera1.CameraChanged += delegate (object sender, CameraChangedRoutedEventArgs args)
                {
                    _eventManager3D.UpdateHitObjects();
                };
            //StartGeniusPlay();
        }

        public void StartGeniusPlay() 
        {
            controle.ClearOrdem();
            controle.AddOrdemGame();
            ClearAllBox();
            ShowOrdem();
        }


        public void EndGeniusPlay()
        {
            controle.ClearOrdem();
        }

        public async Task ShowOrdem() {
            if (controle.ordemGame.Count == 0) {
                return ;
            }

            foreach (Box show in controle.ordemGame) {
                BoxVisual3D test = show.box;
                test.Material = _clickedMaterial;
                await Task.Delay(3000);
                test.Material = _normalMaterial;
                
            }

        }

        //Recrear objeto mesh2 com triangulos em posições aleatórias.
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
            test.SetValue(ConeVisual3D.BottomCenterPositionProperty, utils.GenarateRandomPoints(1, 0)[0]);

            var geometryModel2 = new GeometryModel3D()
            {
                Geometry = mesh2,
                Material = new DiffuseMaterial(Brushes.LightGreen),
                BackMaterial = new DiffuseMaterial(Brushes.Red),
            };
            ModelVisual.Content = geometryModel2;
            //MeshInspector2.MeshGeometry3D = mesh2;
            MeshInspector2.SetValue(MeshInspectorOverlay.MeshGeometry3DProperty, mesh2);


            StartGeniusPlay();
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

            controle.matrizPosisoes = new Box[7, 7];
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
                        CenterPosition = new Point3D(x * 100, height / 2, y * 100),
                        Size = new Size3D(80, height, 80),
                        Material = _normalMaterial
                    };

                    Viewport1.Children.Add(boxVisual3D);

                    Box currentBox = new Box() { box= boxVisual3D };

                    controle.matrizPosisoes[positionx, positiony] = currentBox;

                    var visualEventSource3D = new VisualEventSource3D(boxVisual3D);
                    visualEventSource3D.MouseEnter += BoxOnMouseEnter;
                    visualEventSource3D.MouseLeave += BoxOnMouseLeave;
                    visualEventSource3D.MouseClick += BoxOnMouseClick;


                    _eventManager3D.RegisterEventSource3D(visualEventSource3D);
                    positionx += 1;

                }
                positionx = 0;
                positiony += 1;
            }

            //ToggleCameraAnimation(); // Start camer animation
        }

        private void BoxOnMouseEnter(object sender, Mouse3DEventArgs mouse3DEventArgs)
        {
            var boxVisual3D = mouse3DEventArgs.HitObject as Ab3d.Visuals.BoxVisual3D;
            if (boxVisual3D == null)
                return; // This should not happen

            // Set _isSelectedBoxClicked to true if the selected box is clicked (red) - this will be used on MouseLeave
            _isSelectedBoxClicked = ReferenceEquals(boxVisual3D.Material, _clickedMaterial);

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
            var boxVisual3D = mouseButton3DEventArgs.HitObject as BoxVisual3D;

            if (boxVisual3D == null)
                return; // This should not happen

            Box boxClicado = new Box()
            {
                box = boxVisual3D,
                selcted = true,
            };

            Task<bool> movimentoCorretoTask = controle.CheckNextMove(boxClicado);
            bool movimentoCorreto = movimentoCorretoTask.Result;
         
            if (movimentoCorreto) {
                controle.RemoverJogadaAtual();
                controle.AddOrdemGame();
                ShowOrdem();
            }

            // Toggle clicked and normal material
            if (!_isSelectedBoxClicked)
            {
                boxVisual3D.Material = _clickedMaterial;
                _isSelectedBoxClicked = true;
            }
            else
            {
                boxVisual3D.Material = _normalMaterial;
                _isSelectedBoxClicked = false;

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

        private void ClearAllBox() 
        {
            foreach (var boxVisual3D in Viewport1.Children.OfType<BoxVisual3D>())
                boxVisual3D.Material = _normalMaterial;
        }
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearAllBox();
            //UpdateTotalClickedHeightText();
        }
    }

    public class Box
    {
        public BoxVisual3D box { get; set; }
        public bool selcted { get; set; }

    }

    public class Controle
    {
        private Random rnd = new Random();
        public List<Box> ordemGame = new List<Box>();
        public List<Box> ordemCurrentGame = new List<Box>();
        public Box[,] matrizPosisoes;

        public void SetOrdemCurrentGame()
        {
            ordemCurrentGame = ordemGame;
        }

        public void AddOrdemGame() 
        {
            ordemGame.Add(matrizPosisoes[rnd.Next(0, 6), rnd.Next(0, 6)]);
            ordemCurrentGame = ordemGame;
        }

        public void ClearOrdem()
        {
            ordemCurrentGame.Clear();
            ordemGame.Clear();
        }

        public Box FindBoxInMatriz(int x=0, int y=0) {
            return matrizPosisoes[x, y];   
        }
        public async  Task<bool> CheckNextMove(Box box)
        {
            if (ordemCurrentGame == null || !ordemCurrentGame.Any())
            {
                return false;
            }
            else {
                if (ordemCurrentGame.Last().box.Equals(box.box))
                {
                    
                    return true;
                }
                else
                {
                    return false;
                }          
            }
        }
        public void RemoverJogadaAtual() {
            ordemCurrentGame.RemoveAt(ordemCurrentGame.Count - 1);
        }
    }
    //public class SolidColorBrushAnimationNode : AnimationNodeBase
    //{
    //    /// <summary>
    //    /// Gets the first defined frame number for this AnimationNode.
    //    /// </summary>
    //    public override double FirstFrameNumber
    //    {
    //        // Return smallest frame number. If there are multiple Tracks defined then use code similar to: Math.Min(Math.Min(RotationTrack.FirstFrame, PositionTrack.FirstFrame), DistanceTrack.FirstFrame);
    //        get { return ColorAmountTrack.FirstFrame; }
    //    }

    //    /// <summary>
    //    /// Gets the last defined frame number for this AnimationNode.
    //    /// </summary>
    //    public override double LastFrameNumber
    //    {
    //        get { return ColorAmountTrack.LastFrame; }
    //    }

    //    /// <summary>
    //    /// SolidColorBrush that is animated. This value is set in the GeometryModel3DColorAnimationNode's constructor.
    //    /// </summary>
    //    public SolidColorBrush SolidColorBrush { get; private set; }

    //    /// <summary>
    //    /// Gets a DoubleTrack that defines double value key frames for color amount.
    //    /// </summary>
    //    public DoubleTrack ColorAmountTrack { get; private set; }

    //    // It is also possible to use:
    //    // - Position3DTrack
    //    // - Vector3DTrack
    //    // - RotationTrack
    //    // - CameraRotationTrack

    //    public bool AnimateAlpha { get; set; }
    //    public bool AnimateRed { get; set; }
    //    public bool AnimateGreen { get; set; }
    //    public bool AnimateBlue { get; set; }

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="solidColorBrush">SolidColorBrush that is animated</param>
    //    public SolidColorBrushAnimationNode(SolidColorBrush solidColorBrush)
    //    {
    //        if (solidColorBrush == null) throw new ArgumentNullException(nameof(solidColorBrush));

    //        AnimateAlpha = false;
    //        AnimateRed = true;
    //        AnimateGreen = true;
    //        AnimateBlue = true;

    //        SolidColorBrush = solidColorBrush;
    //        ColorAmountTrack = new DoubleTrack();
    //    }

    //    /// <inheritdoc />
    //    public override void GoToFrame(double frameNumber)
    //    {
    //        if (ColorAmountTrack.KeysCount == 0)
    //            return;

    //        var newDoubleValue = ColorAmountTrack.GetDoubleValueForFrame(frameNumber);

    //        newDoubleValue = Math.Min(1.0, Math.Max(0.0, newDoubleValue)); // Clip to range from 0 to 1

    //        byte newColorValue = (byte)(newDoubleValue * 255.0);

    //        var existingColor = SolidColorBrush.Color;
    //        SolidColorBrush.Color = Color.FromArgb(AnimateAlpha ? newColorValue : existingColor.A,
    //                                               AnimateRed ? newColorValue : existingColor.R,
    //                                               AnimateGreen ? newColorValue : existingColor.G,
    //                                               AnimateBlue ? newColorValue : existingColor.B);
    //    }


    //    /// <summary>
    //    /// GetDumpString virtual method can be overridden to provide detailed description of this object.
    //    /// </summary>
    //    /// <returns>details about this object</returns>
    //    public override string GetDumpString()
    //    {
    //        var sb = new StringBuilder();

    //        var thisType = this.GetType();

    //        sb.Append(thisType.Name).AppendLine(":\r\n");
    //        sb.Append("DoubleTrack as ").AppendLine(this.ColorAmountTrack.GetDumpString());

    //        return sb.ToString();
    //    }
    //}
}
