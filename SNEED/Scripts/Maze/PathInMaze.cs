using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum UnitType { I, L, T, X }
public enum TurnType { STRAIGHT, LEFT, RIGHT }

[RequireComponent(typeof(PathController))]
public class PathInMaze : MonoBehaviour, ISerializationCallbackReceiver
{
	public bool Available = true;
    
    public LinkedList<PathElement> PathAsLinkedList;
    
	private bool inverse = false;

	public bool Inverse
	{
		get { return inverse; } 
	}

	public int ID = -1;

	public void OnEnable()
	{
		InitEmptys();
	}
    
	void Awake()
	{
		InitEmptys();
	}

	void InitEmptys()
	{
        if (PathAsLinkedList == null)
        {
            PathAsLinkedList = new LinkedList<PathElement>();
            Debug.Log("Creating empty Linked List");
        }
    }

    public void InvertPath()
	{
        PathAsLinkedList = new LinkedList<PathElement>( PathAsLinkedList.Reverse() );
		inverse = !inverse;
	}

	#region Serialization
     
	[HideInInspector]
	[SerializeField]
	private List<PathElement> Elements;
    
	public void OnBeforeSerialize()
	{
        if (Elements != null)
            Elements.Clear();
        else
            Elements = new List<PathElement>();

		foreach (var pathElement in PathAsLinkedList)
		{
			Elements.Add(pathElement);
		}
	}
    
	public void OnAfterDeserialize()
	{
        PathAsLinkedList = new LinkedList<PathElement>(Elements);
	}

	#endregion 

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

[Serializable]
public class PathElement
{
	[SerializeField]
	public MazeUnit Unit;

	[SerializeField]
	public UnitType Type;

	[SerializeField]
	public TurnType Turn;

	public PathElement()
	{

	}

	public PathElement(MazeUnit unit, UnitType type = UnitType.I, TurnType turn = TurnType.STRAIGHT) 
	{ 
		Type = type;
		Turn = turn;
		Unit = unit;
	}
}
