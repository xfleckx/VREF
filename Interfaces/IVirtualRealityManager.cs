using System;
using System.Collections;
using UnityEngine;

namespace Assets.VREF.Interfaces
{
    public interface IVirtualRealityManager 
    {
        EnvironmentController ChangeWorld(string worldName);
        
        void CombineEnvironments(params string [] names);
        
        void DrawBoundary();   
        
    }
    public class EnvironmentController : MonoBehaviour {

        public string Title = "Name of the Environment";

        void Awake()
        {
            Title = gameObject.name;
        }

        /// <summary>
        /// Just an idea of how this controller Script could make sense...
        /// </summary>
        public void DisableIllumination()
        {

        }
    }

}