using System;

namespace Assets.VREF.Interfaces
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

