using System;

namespace Assets.BeMoBI.Scripts.HWinterfaces
{
	public class APhaseSpaceRigid
	{
		public abstract class PhaseSpaceRigid ()
		{
			public abstract Rigid () : this(-1, -1, Vector3.zero, Quaternion.identity)
			{

			}

			public abstract Rigid (int id, float cond, Vector3 pos, Quaternion rot) :  base(id, cond, pos)
			{
				rotation = rot;
			}
		}
	}
}

