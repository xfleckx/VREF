using System;
using UnityEngine;

namespace Assets.VREF.Interfaces
{
	public interface ISubjectRelativePositionStream
	{
		Transform Head();
		Transform Body();
		void recalibrate();
	}
}

