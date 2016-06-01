using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.BeMoBI.Scripts.Controls
{
    public interface IBodyMovementController : IInputController
    {
        CharacterController Body
        {
            get; set;
        }
        
    }
}