using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.SNEED.Unity3D.Editor.Maze.UnitCreation
{
    public static class MeshUtilities
    {
        public static List<Vector2> GetPlaneUVs()
        {
           var uvs   = new List<Vector2>()
            {
                V(0, 0),
                V(1, 0),
                V(0, 1),
                V(1, 1)
            };

           return uvs;
        }

        public static List<Vector2> GetInvertPlaneUVs()
        {
            var uvs = new List<Vector2>()
            {
                V(0, 0),
                V(0, 1),
                V(1, 0),
                V(1, 1)
            };

            return uvs;
        }

        #region Helper - type less... :)

        private static Vector3 V(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }
        private static Vector2 V(float x, float y)
        {
            return new Vector2(x, y);
        }

        #endregion
    }
}
