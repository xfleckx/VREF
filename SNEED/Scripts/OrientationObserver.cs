using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class OrientationObserver : MonoBehaviour, IEventSystemHandler {


	[SerializeField]
	public UnityEvent m_OnTargetOrientation;

	public Transform OrientationTarget;

	public float AngleBetween = float.PositiveInfinity;

	public float FocusRange = 6.5f; 

	public Transform ExpectedOrientationSource;

	private Transform OrientationSource;

	private bool EventInvoked;

	// Use this for initialization
	void Start () {
		
	}

	private void SetOrientationSource()
	{
		if (!OrientationSource)
			OrientationSource = transform;
	}

	// Update is called once per frame
	void Update () {

		SetOrientationSource();

		
	}

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("Entering Trigger");

		// Schwierig.. hier muss ja eigentlich das CameraRig hin? 
		OrientationSource = ExpectedOrientationSource;
	}

	public void OnTriggerStay(Collider other)
	{ 
		var targetDir = OrientationTarget.position - ExpectedOrientationSource.position;
		AngleBetween = Vector3.Angle(targetDir, ExpectedOrientationSource.forward);

		if (AngleBetween <= FocusRange && !EventInvoked)
		{
            if(m_OnTargetOrientation.GetPersistentEventCount() > 0)
			    m_OnTargetOrientation.Invoke();
		}
		else
		{
			EventInvoked = false;
		}
	}


	public void OnTriggerExit(Collider other)
	{
		Debug.Log("Exit Trigger");
		OrientationSource = null;
		AngleBetween = float.PositiveInfinity;
	}

	void OnDrawGizmos()
	{
		SetOrientationSource();

		Gizmos.DrawSphere(OrientationSource.position, 0.1f);   

		if (OrientationTarget != null)
		{
			RenderOrientationHelper();
		}
	}

	void RenderOrientationHelper()
	{
		if (!this.enabled)
			return;

		Gizmos.DrawLine(OrientationSource.position, OrientationTarget.position);
#if UNITY_EDITOR
		if (OrientationSource.Equals(ExpectedOrientationSource)){

			var targetDir = OrientationTarget.position - ExpectedOrientationSource.position;

			Handles.color = new Color(1, 0.2f, 0, 0.2f);
			Handles.DrawSolidArc(
				ExpectedOrientationSource.position,
				ExpectedOrientationSource.up,
				targetDir,
				-AngleBetween,
				1f);
		}
#endif
	}
}
