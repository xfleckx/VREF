using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class Waypoint : beMoBIBase, IEventSystemHandler {

	[SerializeField]
	public UnityEvent m_OnWaypointReached = new UnityEvent();

	public bool Active = false;

	void Awake()
	{

	}

	// Use this for initialization
	void Start () {
		base.Initialize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!Active)
			return;

		WriteMarker("Way point entered");

		m_OnWaypointReached.Invoke();
	}

	public void OnTriggerExit(Collider other)
	{
		if (!Active)
			return;

		WriteMarker("Way point exit");

		SendMessageUpwards("RecieveWaypointEvent", name);
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, 0.1f); 
	}
}
 