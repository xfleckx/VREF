using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.SNEED.Unity3D.Editor.Util
{
    class EditHelper
    {
        /// <summary>
        /// Calculates the position of the mouse over the tile maze in local space coordinates.
        /// </summary>
        /// <returns>Returns true if the mouse is over the tile maze.</returns>
        protected bool UpdateHitPosition(GameObject go, ref Vector3 mouseHit)
        {
            // build a plane object that 
            var p = new Plane(go.transform.TransformDirection(go.transform.up), go.transform.position);

            // build a ray type from the current mouse position
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            // stores the hit location
            var hit = new Vector3();

            // stores the distance to the hit location
            float dist;

            // cast a ray to determine what location it intersects with the plane
            if (p.Raycast(ray, out dist))
            {
                // the ray hits the plane so we calculate the hit location in world space
                hit = ray.origin + (ray.direction.normalized * dist);
            }

            // convert the hit location from world space to local space
            var value = go.transform.InverseTransformPoint(hit);

            // if the value is different then the current mouse hit location set the 
            // new mouse hit location and return true indicating a successful hit test
            if (value != mouseHit)
            {
                mouseHit = value;
                return true;
            }

            // return false if the hit test failed
            return false;
        }

    }
}
