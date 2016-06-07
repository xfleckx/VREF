using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.VREF.Interfaces
{
    public interface ICombinedControl : IHeadMovementController, IBodyMovementController
    {
    }

    public interface IInputController
    {
        string Identifier { get; }

        void Enable();

        void Disable();
    }

    public interface IInputCanCalibrate
    {
        void Calibrate();
    }
}
