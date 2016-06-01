using UnityEngine;
using System.Collections;

public class MazeModel : ScriptableObject
{
    [SerializeField]
    public float MazeWidthInMeter = 6f;

    [SerializeField]
    public float MazeLengthInMeter = 10f;

    [SerializeField]
    public float RoomHigthInMeter = 2;

    [SerializeField]
    public float unitFloorOffset = 0f;

    [SerializeField]
    public Vector3 RoomDimension = new Vector3(1.3f, 2, 1.3f);

    [SerializeField]
    public Vector2 EdgeDimension = new Vector2(0.1f, 0.1f);

    [SerializeField]
    public float WallThicknessInMeter = 0.1f;

    [SerializeField]
    public int Rows;

    [SerializeField]
    public int Columns;

    [SerializeField]
    public string UnitNamePattern = "Unit_{0}_{1}";

}