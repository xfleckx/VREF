using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.BeMoBI.Scripts.SubjectRepresentation
{
    interface ISubject
    {
        Transform Head { get; }

        Transform Body { get; }

        void Recalibrate();
    }
}
