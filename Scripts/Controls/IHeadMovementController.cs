using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.BeMoBI.Scripts.Controls
{
    public interface IHeadMovementController : IInputController
    {
        Transform Head { get; set; }
    }
}