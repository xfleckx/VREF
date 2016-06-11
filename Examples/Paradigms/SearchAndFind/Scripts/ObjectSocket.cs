using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class ObjectSocket : MonoBehaviour {

    private BoxCollider boxToFit;

    public bool AutoRescaleToFitTheBox = true;

    public Transform objectLookAt;


    void Awake()
    {
        boxToFit = GetComponent<BoxCollider>();
    }

    public void PutIn(GameObject objectToPresent)
    {
        objectToPresent.transform.SetParent(this.transform, false);
        objectToPresent.transform.localPosition = Vector3.zero;
        objectToPresent.transform.localRotation =Quaternion.identity;

        // Not working yet!
        //var targetPosition = objectLookAt.position;
        //targetPosition.y = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        //transform.LookAt(targetPosition);


        if (AutoRescaleToFitTheBox) { 

            var meshRenderers = objectToPresent.GetComponentsInChildren<MeshRenderer>();

            var objectsOriginalBounds = new Bounds();

            foreach (var meshRenderer in meshRenderers)
            {
                objectsOriginalBounds.Encapsulate(meshRenderer.bounds);
            }

            float x_fac = boxToFit.bounds.size.x / objectsOriginalBounds.size.x ;
            float y_fac = boxToFit.bounds.size.y / objectsOriginalBounds.size.y ;
            float z_fac = boxToFit.bounds.size.z / objectsOriginalBounds.size.z ;

            objectToPresent.transform.localScale = new Vector3(1-x_fac, 1-y_fac, 1-z_fac);
        }
    }
}
