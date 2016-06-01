using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(beMobileMaze))]
public class PathController : MonoBehaviour {

	public List<PathInMaze> Paths = new List<PathInMaze>();

	void Start()
	{
		ForcePathLookup();
	}
	
	public void ForcePathLookup()
	{
		Paths.Clear();

		var existingPaths = GetComponents<PathInMaze>();

		Paths.AddRange(existingPaths);
	}

	public PathInMaze EnablePathContaining(int id)
	{
		PathInMaze result = null;

		foreach (var item in Paths) 
		{
			if (item.ID == id){
				item.enabled = true;
				result = item;
			}
			else { 
				item.enabled = false;
			}
		} 

		return result;
	}

	public int[] GetAvailablePathIDs()
	{
		var ids = Paths.Where(p => p.Available).Select(p => p.ID);

		return ids.ToArray();
	}

#if UNITY_EDITOR
	public Action EditorGizmoCallbacks;
#endif

	public void OnDrawGizmos()
	{ 
#if UNITY_EDITOR
		if (EditorGizmoCallbacks != null)
			EditorGizmoCallbacks();
#endif
	}
}
