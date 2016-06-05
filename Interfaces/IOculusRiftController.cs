using System;
using System.Collections;
namespace Assets.VREF.Interfaces
{
	public interface IOculusRiftController
	{
		void Recenter();
		void SetIPDValue (float newIpdValue);
		void ChangeIPDValue (float value);
		void RestoreOriginalIpd();
		void RequestConfigValues ();
		void OnMonoscopicRenderingChanged ();
		IEnumerator LookUpRift();

	}
}

