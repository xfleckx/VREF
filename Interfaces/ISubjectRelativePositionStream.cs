using System;

namespace Assets.VREF.Interfaces
{
	public interface ISubjectRelativePositionStream
	{
		Transform Head();
		Transform Body();
		void recalibrate();
	}
}

