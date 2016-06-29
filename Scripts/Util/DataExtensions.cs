using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.BeMoBI.Scripts
{
    public static class DataExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            return collection.OrderBy((i) => Guid.NewGuid());
        }
        
        public static IntVector2 AsIntVector(this Vector2 vector)
        {
            return new IntVector2((int)vector.x, (int)vector.y);
        }

        public static IntVector3 AsIntVector(this Vector3 vector)
        {
            return new IntVector3((int)vector.x, (int)vector.y, (int) vector.z);
        }
    }

    public struct IntVector2
    {
        int x;
        int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format("({0} {1})", x, y);
        }
    }


    public struct IntVector3
    {
        int x;
        int y;
        int z;
        
        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", x, y, z);
        }
    }
}
