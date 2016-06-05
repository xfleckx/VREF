using UnityEngine;
using System.Collections;
using System.IO;


namespace Assets.VREF.PhaseSpaceExtensions
{
    public interface IProvideRigidBodyFile {

        FileInfo GetRigidBodyDefinition();

    }
}

