using UnityEngine;
using System.Collections;
using System.IO;

namespace Assets.BeMoBI.Scripts.PhaseSpaceExtensions
{
    public interface IProvideRigidBodyFile {

        FileInfo GetRigidBodyDefinition();

    }
}

