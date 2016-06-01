using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class NavigationAssistent : beMoBIBase {

	public  List<GameObject> Waypoints = new  List<GameObject>();
	private LineRenderer lineRenderer;

    public VirtualRealityManager worldManager;

	void Awake() {
		base.Initialize();

		lineRenderer = GetComponent<LineRenderer>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		 
		UpdateLineRenderer(); 
	}

    void RecieveWaypointEvent(string waypointName)
    {
        Debug.Log(string.Format("Receive event from {0}", waypointName));

        var wp = Waypoints.First((w) => w.name.Equals(waypointName));

        if (wp != null)
        {
            wp.SetActive(false); 
        }

        
    }

	private void UpdateLineRenderer()
	{
		var wps = GetComponentsInChildren<Waypoint>();
		
		Waypoints.Clear();
		
		Waypoints.AddRange(wps.Where((w) => w.enabled).Select((w) => w.gameObject));

		lineRenderer.SetVertexCount(Waypoints.Count);

		for (int i = 0; i < Waypoints.Count; i++)
		{
			lineRenderer.SetPosition(i, Waypoints[i].transform.position);
		}
	}
}
