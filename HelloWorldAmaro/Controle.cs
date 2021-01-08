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
using Ab3d.Cameras;


namespace HelloWorldAmaro
{
    public class Controle
    {
        private DiffuseMaterial _normalMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Silver);
        private DiffuseMaterial _selectedMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Orange);
        private DiffuseMaterial _clickedMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.Red);

        private List<Task> managerTaks = new List<Task>();

        private Random rnd = new Random();
        public List<Box> ordemGame = new List<Box>();
        public List<Box> OrdemCurrentGame = new List<Box>();
        public Box[,] MatrizPosisoes;
        public MainWindow MainWindowCurrent;

        public Controle(MainWindow main)
        {
            MainWindowCurrent = main;
        }

     
        public async void BoxOnMouseClick(object sender, MouseButton3DEventArgs mouseButton3DEventArgs)
        {

            if (MainWindowCurrent.CheckShowButtons)
            {
                return;
            }
            var boxVisual3D = mouseButton3DEventArgs.HitObject as BoxVisual3D;

            if (boxVisual3D == null)
                return; // This should not happen

            Box boxClicado = new Box()
            {
                box = boxVisual3D,
                selcted = true,
            };

            Task<bool> movimentoCorretoTask = CheckNextMove(boxClicado);
            bool movimentoCorreto = movimentoCorretoTask.Result;
            movimentoCorretoTask.Wait();
            // Toggle clicked and normal material


            if (movimentoCorreto)
            {
                //managerTaks.Add(MainWindowCurrent.ShowOrdem());
              
                if (!MainWindowCurrent.IsSelectedBoxClicked)
                {
                    boxVisual3D.Material = _clickedMaterial;
                    MainWindowCurrent.IsSelectedBoxClicked = true;
                }
                else
                {
                    boxVisual3D.Material = _normalMaterial;
                    MainWindowCurrent.IsSelectedBoxClicked = false;

                }
            }

           if (OrdemCurrentGame.Count != 0)
                {
                    AddOrdemGame();
                    await MainWindowCurrent.ShowOrdem();
                }
                else 
                {
                    RemoverJogadaAtual();
                    await MainWindowCurrent.ShowOrdem();
                }
            //UpdateTotalClickedHeightText();
        }

        public void BoxOnMouseLeave(object sender, Mouse3DEventArgs mouse3DEventArgs)
        {
            if (MainWindowCurrent.CheckShowButtons)
            {
                return;
            }
            if (MainWindowCurrent.IsSelectedBoxClicked)
            {
                return;
            }

            var boxVisual3D = mouse3DEventArgs.HitObject as Ab3d.Visuals.BoxVisual3D;
            if (boxVisual3D == null)
                return; // This should not happen

            if (MainWindowCurrent.IsSelectedBoxClicked)
                boxVisual3D.Material = _clickedMaterial;
            else
                boxVisual3D.Material = _normalMaterial;
        }

        public void BoxOnMouseEnter(object sender, Mouse3DEventArgs mouse3DEventArgs)
        {
            if (MainWindowCurrent.CheckShowButtons)
            {
                return;
            }
            if (MainWindowCurrent.IsSelectedBoxClicked) {
                return ;
            }

            var boxVisual3D = mouse3DEventArgs.HitObject as Ab3d.Visuals.BoxVisual3D;
            if (boxVisual3D == null)
                return; // This should not happen

            // Set _isSelectedBoxClicked to true if the selected box is clicked (red) - this will be used on MouseLeave
            MainWindowCurrent.IsSelectedBoxClicked = ReferenceEquals(boxVisual3D.Material, MainWindowCurrent.CheckShowButtons);

            boxVisual3D.Material = _selectedMaterial;
        }

        public void movableEventSource3D_MouseDrag(object sender, MouseDrag3DEventArgs e)
        {

            //TODO: Verificar eventos de movimento ao drag
            if (e.HitSurface != null)
            {
                var boxVisual3D = e.HitObject as BoxVisual3D;
                if (boxVisual3D == null)
                    return;


                // Change the position of the box and arrow line

                // This can be done in a couple of ways:
                // The most easy is to change the CenterPosition of the box and StartPosition and EndPostition of the arrow line.
                // But this would recreate the geometry

                // Therfore it is better to add a transformation to the box and line and change that
                // Even better is to create a parent ModelVisual3D that holds box and arrow line and transform the this ModelVisual3D

                // Commented:
                //MovableBoxVisual3D.CenterPosition = new Point3D(e.CurrentSurfaceHitPoint.X,
                //                                                e.CurrentSurfaceHitPoint.Y + MovableBoxVisual3D.Size.Y / 2,
                //                                                e.CurrentSurfaceHitPoint.Z);

                //MovableBoxTranslate.OffsetX = e.CurrentSurfaceHitPoint.X;
                //MovableBoxTranslate.OffsetY = e.CurrentSurfaceHitPoint.Y + MovableBoxVisual3D.Size.Y / 2;
                //MovableBoxTranslate.OffsetZ = e.CurrentSurfaceHitPoint.Z;

                //ArrowLineTranslate.OffsetX = e.CurrentSurfaceHitPoint.X;
                //ArrowLineTranslate.OffsetY = e.CurrentSurfaceHitPoint.Y + MovableBoxVisual3D.Size.Y / 2;
                //ArrowLineTranslate.OffsetZ = e.CurrentSurfaceHitPoint.Z;

                //MovableVisualTranslate.OffsetX = e.CurrentSurfaceHitPoint.X;
                //MovableVisualTranslate.OffsetY = e.CurrentSurfaceHitPoint.Y + MovableBoxVisual3D.Size.Y / 2;
                //MovableVisualTranslate.OffsetZ = e.CurrentSurfaceHitPoint.Z;
            }
        }

        public void SetOrdemCurrentGame()
        {
            OrdemCurrentGame = ordemGame;
        }

        public void AddOrdemGame()
        {
            ordemGame.Add(MatrizPosisoes[rnd.Next(0, 6), rnd.Next(0, 6)]);

            OrdemCurrentGame = ordemGame;
        }

        public void ClearOrdem()
        {
            OrdemCurrentGame.Clear();
            ordemGame.Clear();
        }

        public Box FindBoxInMatriz(int x = 0, int y = 0)
        {
            return MatrizPosisoes[x, y];
        }

        public async Task<bool> CheckNextMove(Box box)
        {
            if (OrdemCurrentGame == null || !OrdemCurrentGame.Any())
            {
                return false;
            }
            else
            {
                if (OrdemCurrentGame.Last().box.Equals(box.box))
                {

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void RemoverJogadaAtual()
        {
            OrdemCurrentGame.RemoveAt(OrdemCurrentGame.Count - 1);
        }


    }
    public class EventManagerButtons
    {
        public static EventManagerButtons _eventManager;
        public EventManager3D _eventManager3D;

        public static EventManagerButtons GetInstance()
        {
            if (_eventManager == null)
            {
                _eventManager = new EventManagerButtons();
            }
            return _eventManager;
        }

    }
}
