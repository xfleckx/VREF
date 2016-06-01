using UnityEngine;
using System.Collections;

/// <summary>
/// Definition of anything which should be able to provide the functionality of sampling or sampling without replacement.
/// Sampling without replacement is a stateful operation so be aware of reseting a sample process when it is finished!
/// </summary>
/// <typeparam name="T"> Result Type of sampling methods</typeparam>
public interface IProvideSampling<T> {

    bool AutoResetSequence { get; set; }

    T Sample();

    T SampleWithoutReplacement();

    void ResetSamplingSequence();
}
