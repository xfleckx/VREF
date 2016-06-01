using UnityEngine;
using System.Collections;
using System;

public class ObjectHideOut : MazeUnit {

    private Transform targetTransform;
    private Quaternion rotation;
    private Vector3 position;
    private Vector3 scaling = Vector3.one;

    public GameObject TargetObject;

    private bool fading;
    private float targetState = 0;
    public float FadingStep = 0.1f;

    private Material FadingTarget;


    public GameObject GrabObject()
    {
        rotation = TargetObject.transform.localRotation;
        position = TargetObject.transform.localPosition;
        TargetObject.transform.parent = null;
        return TargetObject;
    }

    public void ReturnObject(GameObject o)
    {
        o.transform.parent = gameObject.transform;
        o.transform.localPosition = position;
        o.transform.localRotation = rotation;
        o.transform.localScale = scaling;
    }

    public void Open()
    {
        Open(WaysOpen);
    }

    public void Close()
    {
        Close(WaysOpen);
    }

    public override void Open(OpenDirections direction)
    {
       var directionName = Enum.GetName(typeof(OpenDirections), direction);

       FadingTarget = GetTargetMaterial(directionName);
       targetState = 0;
       FadingStep *= -1;
       StartCoroutine(Fade());
    }

    public override void Close(OpenDirections direction)
    {
        var directionName = Enum.GetName(typeof(OpenDirections), direction);

        FadingTarget = GetTargetMaterial(directionName);
        targetState = 1;
        FadingStep *= -1;
        StartCoroutine(Fade());
    }

    private Material GetTargetMaterial(string directionName)
    {
        var childTransform = transform.FindChild(directionName);

        if (childTransform == null)
        {
            Debug.LogWarning(string.Format("Fading target {0} not found!", directionName), this);
        }

        var door = childTransform.gameObject;
        var doorMaterial = door.GetComponent<Renderer>();

        return doorMaterial.material;
    }

    IEnumerator Fade()
    {
        do
        {
            if (FadingTarget.color.a != targetState)
            {
                yield return new WaitForSeconds(Mathf.Abs(FadingStep));
                fading = true;
                FadingTarget.color = new Color(FadingTarget.color.r, FadingTarget.color.g, FadingTarget.color.b, FadingTarget.color.a + FadingStep); 
            }
            else
            {
                fading = false;
                yield return null;
            }

        } while (fading);
    }
       
    public bool IsInTransition(){
        return fading;
    }

    public bool IsOpen(string directionName)
    {
        var material = GetTargetMaterial(directionName);

        return material.color.a == 0;
    }
}
