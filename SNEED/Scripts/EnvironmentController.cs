using UnityEngine;
using System.Collections;

public class EnvironmentController : MonoBehaviour {

	public string Title = "Name of the Environment";

	void Awake()
	{
		Title = gameObject.name;
	}

    /// <summary>
    /// Just an idea of how this controller Script could make sense...
    /// </summary>
    public void DisableIllumination()
    {

    }
}
