using UnityEngine;
using System.Collections;

public class Door : MazeUnit {

    enum DoorState { Open, Closed, Opening, Closing }

    private DoorState state = DoorState.Closed;
    public bool IsOpen { get { return state == DoorState.Open; } }

    public bool IsClosed { get { return state == DoorState.Closed; } }

    public bool IsClosing { get { return state == DoorState.Closing; } }

    public bool IsOpening { get { return state == DoorState.Opening; } }
    public GameObject DoorElement;

    public override void Open(OpenDirections direction)
    {
        base.Open(direction);

        ConfigureDoorElementRotation();
    }

    public override void Close(OpenDirections direction)
    {
        base.Close(direction);

        ConfigureDoorElementRotation();
    }

    private void ConfigureDoorElementRotation()
    {
        if (WaysOpen == (OpenDirections.East | OpenDirections.West))
        {
            DoorElement.transform.localRotation = Quaternion.identity;
        }

        if (WaysOpen == (OpenDirections.North | OpenDirections.South))
        {
            DoorElement.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
    }

    public void Open()
    {
        if (IsOpening || IsOpen)
            return;

    }

    public void Close()
    {
        if (IsClosing || IsClosed)
            return;

    }

    IEnumerator MoveDoor()
    {


        return null;
    }

}
