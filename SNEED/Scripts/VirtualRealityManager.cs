using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class VirtualRealityManager : MonoBehaviour {

	//VR Real World Dimension for MoCap Systems
	public float HighQualityZoneWidth = 8f;
	public float HighQualityZoneLength = 12f;
	public float BorderZoneWidth = 1f;
	
	[SerializeField]
	public List<EnvironmentController> AvailableEnvironments = new List<EnvironmentController>();
	
    /// <summary>
    /// Change the whole world to exactly one environment
    /// </summary>
    /// <param name="worldName"></param>
	public EnvironmentController ChangeWorld(string worldName)
	{
        AvailableEnvironments.ForEach((e) => e.gameObject.SetActive(false)); 

		if (AvailableEnvironments.Any((i) => i.Title.Equals(worldName))) {
           
			var enabledEnvironment = AvailableEnvironments.First((i) => i.Title.Equals(worldName));

            if(enabledEnvironment != null)
                enabledEnvironment.gameObject.SetActive(true);

			return enabledEnvironment;
		}

		return null;
	}

	/// <summary>
	/// Combine multiple environments
	/// </summary>
	/// <param name="names"></param>
	public void CombineEnvironments(params string[] names)
	{
        AvailableEnvironments.ForEach((e) => e.gameObject.SetActive(false)); 

		foreach (var item in names)
		{
			var environment = this.AvailableEnvironments.First((i) => i.Title.Equals(item));
			if(environment != null)
				environment.gameObject.SetActive(true);
		}
	}

	void OnDrawGizmos()
	{
		DrawRealWorldBorder();
	}

	private void DrawRealWorldBorder()
	{
        var tempGizmoMatrix = Gizmos.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;

		float halfWidth = HighQualityZoneWidth / 2;
		float halfLengt = HighQualityZoneLength / 2;

		float x0 = 0 - halfWidth;
		float x1 = 0 + halfWidth;
		float z0 = 0 - halfLengt;
		float z1 = 0 + halfLengt;

		Vector3 env00 = new Vector3(x0, 0, z0);
		Vector3 env11 = new Vector3(x1, 0, z1);
		Vector3 env01 = new Vector3(x0, 0, z1);
		Vector3 env10 = new Vector3(x1, 0, z0);

        Gizmos.DrawLine(env00, env01);
        Gizmos.DrawLine(env00, env10);
        Gizmos.DrawLine(env10, env11);
        Gizmos.DrawLine(env01, env11);

		Vector3 bz_length_Size = new Vector3(BorderZoneWidth, 0, HighQualityZoneLength + 2 * BorderZoneWidth);
		Vector3 bz_Width_Size = new Vector3(HighQualityZoneWidth + 2 * BorderZoneWidth, 0, BorderZoneWidth);

		float halfBorderZone = BorderZoneWidth / 2;

		float bz_west = x0 - halfBorderZone;
		Vector3 bz_west_Origin = new Vector3(bz_west, 0, 0);

		float bz_east = x1 + halfBorderZone;
		Vector3 bz_east_Origin = new Vector3(bz_east, 0, 0);

		float bz_north = z1 + halfBorderZone;
		Vector3 bz_north_Origin = new Vector3(0, 0, bz_north);

		float bz_south = z0 - halfBorderZone;
		Vector3 bz_south_Origin = new Vector3(0, 0, bz_south);

		Gizmos.color = new Color(1, 0, 0, 0.1f);
		Gizmos.DrawCube(bz_west_Origin, bz_length_Size);
		Gizmos.DrawCube(bz_east_Origin, bz_length_Size);
		Gizmos.DrawCube(bz_north_Origin, bz_Width_Size);
		Gizmos.DrawCube(bz_south_Origin, bz_Width_Size);

        Gizmos.matrix = tempGizmoMatrix;
	}
}
