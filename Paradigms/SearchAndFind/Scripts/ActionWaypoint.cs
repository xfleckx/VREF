using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class ActionWaypoint : MonoBehaviour
{
    public int WaypointId = 0;

    public string TagOfInterest;

    public Text info;

    public Transform InfoText;

    public Transform InfoTextLookAtTarget;
    
    public WaypointEvent EnteredAt;

    public WaypointEvent LeaveFrom;


    void Update() {

        if (InfoText != null && InfoTextLookAtTarget != null)
            InfoText.LookAt(InfoTextLookAtTarget);
    }

    public void HideInfoText() {

        InfoText.gameObject.SetActive(false);
    }

    public void RevealInfoText()
    {
        InfoText.gameObject.SetActive(true);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == TagOfInterest && EnteredAt.GetPersistentEventCount() > 0)
            EnteredAt.Invoke(this);
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == TagOfInterest && EnteredAt.GetPersistentEventCount() > 0)
            LeaveFrom.Invoke(this);
    }
}

[Serializable]
public class WaypointEvent : UnityEvent<ActionWaypoint> { }