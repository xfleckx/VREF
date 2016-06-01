using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Category : MonoBehaviour, IProvideSampling<GameObject> {
    
    public List<GameObject> AssociatedObjects = new List<GameObject>();

    public bool HasObjects { get { return AssociatedObjects.Count > 0; } }

    private bool autoResetSequence = true;
    public bool AutoResetSequence
    {
        get
        {
            return autoResetSequence;
        }

        set
        {
            autoResetSequence = value;
        }
    }

    private Stack<GameObject> tempSamplingSet;

    public GameObject Sample()
    {
        int max =  AssociatedObjects.Count;
        
        var randomIndex = UnityEngine.Random.Range(0,max);

        return AssociatedObjects[randomIndex];
    }

    /// <summary>
    /// Set will be sampled until its empty than it resets and starts again.
    /// </summary>
    /// <remarks>Don't forget to reset the sampling secquence!</remarks>
    /// <returns>An Object from this category</returns>
    public GameObject SampleWithoutReplacement()
    {

        if (tempSamplingSet == null)
        {
            var shuffled = AssociatedObjects.OrderBy(obj => Guid.NewGuid()).ToList();
            tempSamplingSet = new Stack<GameObject>(shuffled);
        }

        if (tempSamplingSet.Count == 0 && autoResetSequence)
        {

            var shuffled = AssociatedObjects.OrderBy(obj => Guid.NewGuid()).ToList();
            tempSamplingSet = new Stack<GameObject>(shuffled);

        }
        else if (tempSamplingSet.Count == 0 && !autoResetSequence)
        {

            throw new InvalidOperationException(
                "Warning! You try to use a sampling sequence without reseting it. \n" +
                "Set AutoResetSequence to \"true\" or call ResetSamplingSequence manually.");
        }


        return tempSamplingSet.Pop();
    }

    /// <summary>
    /// Cleanup a sampling sequence
    /// </summary>
    public void ResetSamplingSequence()
    {
        tempSamplingSet.Clear();
        tempSamplingSet = null;
    }

    public GameObject GetObjectBy(string requestedName)
    {
        var target = AssociatedObjects.Single((o) => o.name.Equals(requestedName));

        if(target == null) { 

           var targetTransform = this.transform.Find(name);

            if (targetTransform == null)
                throw new ArgumentException(string.Format("The request object with the \"{0}\" doesn't exist!", requestedName));

            var hasMesh = targetTransform.GetComponentInChildren<MeshRenderer>() != null;
            var hasSkinnedMesh = targetTransform.GetComponentInChildren<SkinnedMeshRenderer>() != null;


            if (hasMesh || hasSkinnedMesh) {
                AssociatedObjects.Add(targetTransform.gameObject);
                return targetTransform.gameObject;
            }
            else
                throw new ArgumentException(string.Format("The request object with the \"{0}\" exists but doesn't appear to be a visible object!"));
        }

        return target;
    }
}
