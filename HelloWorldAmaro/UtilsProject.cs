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
                    start + rnd.Next(start-100, start+100),
                    start + rnd.Next(start - 100, start + 100),
                    start + rnd.Next(start - 100, start + 100)
                    );
                start = start + rnd.Next(start - 50, start + 50);
            }
            
            return newArray;
        }


        public void WaitTime(int miliseconds) 
        {
            System.Threading.Thread.Sleep(miliseconds);
        }
    }
}




