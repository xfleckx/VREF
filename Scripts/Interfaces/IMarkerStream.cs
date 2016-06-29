
namespace Assets.VREF.Scripts.Interfaces
{
	public interface IMarkerStream
	{
		string StreamName { get; }

		void Write(string name, float customTimeStamp);

		void Write(string name);

		void WriteAtTheEndOfThisFrame(string marker);

	}
}




