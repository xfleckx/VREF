using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System;
using System.Linq;

public class ObjectPool : MonoBehaviour, IProvideSampling<Category>
{
    public List<Category> Categories = new List<Category>();

    private Stack<Category> tempSamplingSet;

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

    public void ResetSamplingSequence()
    {
        tempSamplingSet.Clear();
        tempSamplingSet = null;
    }

    public Category Sample()
    {
        int max = Categories.Count;

        var randomIndex = UnityEngine.Random.Range(0, max);

        return Categories[randomIndex];
    }

    public Category SampleWithoutReplacement()
    {
        if (tempSamplingSet == null)
        {
            var shuffled = Categories.OrderBy( category => Guid.NewGuid()).ToList();
            tempSamplingSet = new Stack<Category>(shuffled);
        }

        if (tempSamplingSet.Count == 0 && autoResetSequence)
        {

            var shuffled = Categories.OrderBy(category => Guid.NewGuid()).ToList();
            tempSamplingSet = new Stack<Category>(shuffled);

        }
        else if (tempSamplingSet.Count == 0 && !autoResetSequence)
        {

            throw new InvalidOperationException(
                "Warning! You try to use a sampling sequence without reseting it. \n" +
                "Set AutoResetSequence to \"true\" or call ResetSamplingSequence manually.");
        }


        return tempSamplingSet.Pop();
    }
}    
