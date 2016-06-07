using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.VREF.Interfaces
{
    public interface IHeadMovementController : IInputController
    {
        Transform Head { get; set; }
    }

   /* public interface IInputCanCalibrate
    {
        void Calibrate();
    }*/
}