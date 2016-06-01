using UnityEngine;
using System.Collections;

public class SpotDoor : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        SendMessageUpwards("DoorStepEntered", other, SendMessageOptions.RequireReceiver);
    }

}
