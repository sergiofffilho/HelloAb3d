using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ab3d.Visuals;
using System.Linq;
using HelloWorldAmaro.ManagerEvents;
using Ab3d.Controls;
using Ab3d.Common.Cameras;
using Ab3d.Utilities;
using System;
using Ab3d.Common.EventManager3D;
using System.Collections.Generic;
using Ab3d.Animation;
using System.Threading.Tasks;
using Ab3d.Cameras;

namespace HelloWorldAmaro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        public bool _isAdjustingDistance = false;

        public UtilsProject utils;
        
        public EventManagerButtons EventsButtons = EventManagerButtons.GetInstance();

        public bool IsSelectedBoxClicked { get; set; }
        public Controle controle { get; set; }
        public bool CheckShowButtons { get; set; }

        public DiffuseMaterial _normalMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Silver);
        //private SolidColorBrush _normalColorSolid = new SolidColorBrush(System.Windows.Media.Brushes.Silver.Color);

        public DiffuseMaterial _selectedMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Orange);
        public  DiffuseMaterial _clickedMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Red);


        public MeshGeometry3D mesh2 { get; set; }
        public ConeVisual3D test { get; set; }

        //private GeometryModel3D _geometryModel3;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += StartWindow;
          
           
            //Camera1.CameraChanged += Camera1_CameraChanged;
            //Camera1.PreviewCameraChanged += Camera1_PreviewCameraChanged;
            //StartGeniusPlay();
        }
        private void StartWindow (object sender, RoutedEventArgs args)
        {
            EventsButtons._eventManager3D = new Ab3d.Utilities.EventManager3D(Viewport1);
            utils = new UtilsProject(this);
            controle = new Controle(this);

            utils.CriarMesh();
            utils.Setup3DObjectsGenius();
            utils.StartPlane3D();
            Camera1.CameraChanged += delegate (object s, CameraChangedRoutedEventArgs a)
            {
                if (CheckShowButtons)
                {
                    return;
                }
              };
            FitIntoView();

        }

        //private void Camera1_PreviewCameraChanged(object sender, PreviewCameraChangedRoutedEventArgs e)
        //{
        //}

        //private void Camera1_CameraChanged(object sender, CameraChangedRoutedEventArgs e)
        //{
        //}

        public async void StartGeniusPlay()
        {
            controle.ClearOrdem();
            controle.AddOrdemGame();
            controle.ClearAllBox();
            try { 
                await ShowOrdem();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public void EndGeniusPlay()
        {
            controle.ClearOrdem();
        }


        public async Task  ShowOrdem() {
            if (controle.ordemGame.Count == 0) {
                return;
            }
            CheckShowButtons = true;

            foreach (Box show in controle.ordemGame) {
                BoxVisual3D test = show.box;
                Console.WriteLine(show.box.CenterPosition);
                test.Material = _clickedMaterial;
                await Task.Delay(2000);
                test.Material = _normalMaterial;

            }
            controle.ClearAllBox();
            CheckShowButtons = false;
            return;
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
      


    
     
    
        //private async void esperarShowOrdem() {
        //    ShowOrdem
        //}

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

        

        private void FitIntoView()
        {
            double selectedAdjustmentFactor = 1.0;

            bool adjustTargetPosition = true;// CenterCameraCheckBox.IsChecked ?? false;

            _isAdjustingDistance = true; // Prevent infinite recursion


            Camera1.FitIntoView(visuals: ModelVisual.Children,               // Do not use WireGrid for FitIntoView calculations (this parameter can be skipped if all shown objects should be checked)
                                fitIntoViewType: Ab3d.Common.FitIntoViewType.CheckAllPositions, // CheckAllPositions can take some time bigger scenes. In this case you can use the CheckBounds
                                adjustTargetPosition: adjustTargetPosition,                     // Adjust TargetPosition to better fit into view; set to false to preserve the current TargetPosition
                                adjustmentFactor: selectedAdjustmentFactor);                    // adjustmentFactor can be used to specify the margin

       
            _isAdjustingDistance = false;
        }

   
    }

    public class Box
    {
        public BoxVisual3D box { get; set; }
        public bool selcted { get; set; }

    }

   
    //public class Object3DGenerator  {
    //    Camera1
    //}
}
