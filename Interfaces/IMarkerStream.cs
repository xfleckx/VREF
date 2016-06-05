using UnityEngine;
using System;
using System.Collections;


namespace Assets.VREF.Interfaces
{
	
	public interface IMarkerStream
	{
		string StreamName { get; } 

		void Write(string name, float customTimeStamp);

		void Write(string name);

		void WriteAtTheEndOfThisFrame(string marker);

	}

	public class MarkerStream : MonoBehaviour, IMarkerStream
	{

		public void Write(string marker, double customTimeStamp)
		{
			Debug.Log (string.Format ("Write Marker:\t{0}\t{1}", customTimeStamp, marker));
		}

		public void Write(string marker)
		{
			Debug.Log (string.Format ("Write Marker:\t{0}\t{1}", timestamp, marker));
		}


		#region Write Marker at the end of a frame

		private string pendingMarker;

		IEnumerator WriteAfterPostPresent()
		{
			yield return new WaitForEndOfFrame();

			Write(pendingMarker);

			yield return null;
		}

		public void WriteAtTheEndOfThisFrame(string marker)
		{
			pendingMarker = marker;
			StartCoroutine(WriteAfterPostPresent());
		}

		#endregion
	}




	/// <summary>
	/// Example implementation of an marker stream
	/// </summary>
	public class DebugMarkerStream : IMarkerStream
	{
		private const string streamName = "DebugMarkerStream";
		private const string logWithTimeStampPattern = "Marker {0} at {1}";

		public string StreamName
		{
			get
			{
				return streamName;
			}
		} 

		public void Write(string name, float customTimeStamp)
		{
			Debug.Log(string.Format(logWithTimeStampPattern, name, customTimeStamp));
		}

		public void Write(string name)
		{
			Debug.Log(string.Format(logWithTimeStampPattern, name, Time.realtimeSinceStartup));
		}
	}

}




