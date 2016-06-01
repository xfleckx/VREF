using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Flags]
public enum OpenDirections { 
	None = 0x00, 
	North = 0x01, 
	South = 0x02, 
	East = 0x04,
	West = 0x08,
	All = North | West | South | East
}

[Serializable]
public class MazeUnit : MonoBehaviour {

    public const string TOP = "Top";
    public const string FLOOR = "Floor";
    public const string NORTH = "North";
    public const string SOUTH = "South";
    public const string WEST = "West";
    public const string EAST = "East";

	public OpenDirections WaysOpen = OpenDirections.None;

	[SerializeField]
	public Vector2 GridID;

    [SerializeField]
    protected Vector3 dimension;
    public Vector3 Dimension { get { return dimension; } }

	public void Initialize(Vector2 tilePos, Vector3 newDimension)
	{
		GridID = tilePos;
        dimension = newDimension;
	}

    public void Initialize(Vector3 newDimension)
    {
        GridID = Vector2.zero;
        dimension = newDimension;
    }
 
	public virtual void Open(OpenDirections direction)
	{
		var directionName = Enum.GetName(typeof(OpenDirections), direction);

        var expectedChild = transform.FindChild(directionName);

        if (expectedChild != null) { 
            expectedChild.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError(string.Format("Children with name {0} expected but not found!", directionName));
        }

		WaysOpen |= direction;
	}

	public virtual void Close(OpenDirections direction)
	{
		var directionName = Enum.GetName(typeof(OpenDirections), direction);
		transform.FindChild(directionName).gameObject.SetActive(true);

		WaysOpen &= ~direction;
	}

	void OnTriggerEnter(Collider c)
	{
		var evt = new MazeUnitEvent(MazeUnitEventType.Entering, c, this); 

		SendMessageUpwards("RecieveUnitEvent", evt, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnTriggerExit(Collider c)
	{
		var evt = new MazeUnitEvent(MazeUnitEventType.Exiting, c, this); 
		
		SendMessageUpwards("RecieveUnitEvent", evt, SendMessageOptions.DontRequireReceiver);
	}

    private Vector3 cubeGizmoSize = new Vector3(0.001f, 0.001f, 0.001f);
    void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, cubeGizmoSize);
    }

}

public enum MazeUnitEventType { Entering, Exiting }

public class MazeUnitEvent
{
	private MazeUnitEventType mazeUnitEventType;
	public MazeUnitEventType MazeUnitEventType
	{
		get { return mazeUnitEventType; } 
	}

	private Collider c;
	private MazeUnit mazeUnit;

	public MazeUnit MazeUnit
	{
		get { return mazeUnit; }
	}

	public Collider Collider
	{
		get { return c; } 
	}
	 
	public MazeUnitEvent(MazeUnitEventType mazeUnitEventType, UnityEngine.Collider c, MazeUnit mazeUnit)
	{
		this.mazeUnitEventType = mazeUnitEventType;
		this.c = c;
		this.mazeUnit = mazeUnit;
	}
	
}