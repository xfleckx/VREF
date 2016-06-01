using System;

namespace Assets.BeMoBI.Scripts.HWinterfaces
{
	public interface IOWLinterface
	{
		void Initialize();
		void ConnectToOWLInstance ();
		void DisconnectFromOWLInstance ();
		bool HasConfigurationAvaiable ();
		void PerformOwlUpdate ();
	
	}
}

