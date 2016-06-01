using UnityEngine;
using System.Collections;
using System;

public class TopLighting : MonoBehaviour
{

    private Light centerLight;

    void Awake()
    {
        centerLight = GetComponentInChildren<Light>();
    }

    public void SwitchOn()
    {
        centerLight.gameObject.SetActive(true);

        if (centerLight != null)
            centerLight.enabled = true;
    }

    public void SwitchOff()
    {
        centerLight.gameObject.SetActive(false);

        if (centerLight != null)
            centerLight.enabled = false;
    }

    public void ToggleLightDirection(OpenDirections direction)
    {
        string directionName = Enum.GetName(typeof(OpenDirections), direction);

        var target = this.transform.FindChild(directionName);

        if (target == null)
        {
            Debug.Assert(target != null, string.Format("This component should have an child with the name: {0}", target));
            return;
        }

        bool state = target.gameObject.activeSelf;
        target.gameObject.SetActive(!state);
    }


    public void OnDrawGizmos()
    {
        var temp = Gizmos.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawLine(Vector3.zero, Vector3.down / 2);

        Gizmos.matrix = temp;
    }

}
