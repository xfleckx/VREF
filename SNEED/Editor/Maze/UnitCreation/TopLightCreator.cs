using UnityEngine;
using System.Collections;
using Assets.SNEED.Unity3D.Editor.Maze.UnitCreation;
using UnityEditor;
using System.Collections.Generic;
using System.IO; 

public class TopLightCreator : CreatorState<TopLighting>
{
    [SerializeField]
    private Vector2 lightingDimension;

    private float spotPlaneWidth = 0.3f;

    [SerializeField]
    private Material illuminationMaterial;

    public override string CreatorName
    {
        get
        {
            return "Top Light";
        }
    }

    public override string AssetModelsPath
    {
        get
        {
            return base.AssetModelsPath;
        }
    }

    public override string AssetPrefabPath
    {
        get
        {
            return base.AssetPrefabPath;
        }
    }

    protected override void OnRoomDimensionUpdate()
    {
        lightingDimension = new Vector2(RoomDimension.x, RoomDimension.z);
    }

    public override void Initialize()
    {
        // make sure that lightingDimension equals RoomDimension on first window creation
        OnRoomDimensionUpdate();
    }

    public override Rect OnGUI()
    {
        var rect = EditorGUILayout.BeginVertical();

        roomDimension = EditorGUILayout.Vector3Field("Room Dimension", roomDimension);
        
        spotPlaneWidth = EditorGUILayout.FloatField("Light plane width (m)", spotPlaneWidth);
        
        illuminationMaterial = EditorGUILayout.ObjectField("Illumination Material", illuminationMaterial, typeof(Material), false) as Material;

        EditorGUILayout.Space();

        if (illuminationMaterial != null)
        {
            if (GUILayout.Button("Create Top Light"))
            {
                Create();
            }

            constructedUnit = EditorGUILayout.ObjectField("Created Unit", constructedUnit, typeof(TopLighting), true) as TopLighting;

            Render_SaveAsPrefab_Option<TopLighting>(constructedUnit);

        }
        else
        {
            EditorGUILayout.HelpBox("To create a top light, please add a illumniating material!", MessageType.Info);
        }

        EditorGUILayout.EndVertical();

        return rect;
    }

    private void Create()
    {
        var centerMesh = CreateCenterMesh();

        var topLightHost = new GameObject("TopLightning");
        var topLightInstance = topLightHost.AddComponent<TopLighting>();
        
        var center = new GameObject("Center");
        center.transform.parent = topLightHost.transform;
        var centerMeshFilter = center.AddComponent<MeshFilter>();
        centerMeshFilter.mesh = centerMesh;
        center.AddComponent<MeshRenderer>().material = illuminationMaterial;

        var result = string.Format("{0}{1}{2}_Mesh_{3}_{4}.asset", AssetModelsPath, Path.AltDirectorySeparatorChar, center.name, lightingDimension.AsPartFileName(), spotPlaneWidth.ToString().Replace('.', '_'));

        SaveAsAsset(centerMeshFilter.sharedMesh, result);

        var lightHost = new GameObject("Light");
        lightHost.transform.parent = center.transform;
        lightHost.transform.Rotate(new Vector3(90, 0, 0));
        
        var light = lightHost.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = roomDimension.y;
        
        var panelMesh_Z_Length = CreateLightPanelMeshForNorthAndSouth();
        
        var NorthSouthMeshPath = string.Format("{0}{1}{2}_Mesh_{3}_{4}.asset", AssetModelsPath, Path.AltDirectorySeparatorChar, "NorthSouth", lightingDimension.AsPartFileName(), spotPlaneWidth.ToString().Replace('.', '_'));

        SaveAsAsset(panelMesh_Z_Length, NorthSouthMeshPath);


        var northSouthOffset = (spotPlaneWidth / 2 + (lightingDimension.y / 2)) / 2;

        var north = new GameObject("North");
        north.transform.parent = topLightHost.transform;
        north.AddComponent<MeshFilter>().mesh = panelMesh_Z_Length;
        north.AddComponent<MeshRenderer>().material = illuminationMaterial;
        north.transform.localPosition = new Vector3(0, 0, northSouthOffset);

        var south = new GameObject("South");
        south.transform.parent = topLightHost.transform;
        south.AddComponent<MeshFilter>().mesh = panelMesh_Z_Length;
        south.AddComponent<MeshRenderer>().material = illuminationMaterial;
        south.transform.localPosition = new Vector3(0, 0, -northSouthOffset);

        var panelMesh_X_Length = CreateLightPanelMeshForEastAndWest();

        var EastWestMeshPath = string.Format("{0}{1}{2}_Mesh_{3}_{4}.asset", AssetModelsPath, Path.AltDirectorySeparatorChar, "EastWest", lightingDimension.AsPartFileName(), spotPlaneWidth.ToString().Replace('.', '_'));

        SaveAsAsset(panelMesh_X_Length, EastWestMeshPath);

        var eastWestOffset = (spotPlaneWidth / 2 + (lightingDimension.x / 2)) / 2;

        var west = new GameObject("West");
        west.transform.parent = topLightHost.transform;
        west.AddComponent<MeshFilter>().mesh = panelMesh_X_Length;
        west.transform.localPosition = new Vector3(-eastWestOffset, 0, 0);
        west.AddComponent<MeshRenderer>().material = illuminationMaterial;
        west.transform.Rotate(new Vector3(0, 90, 0));

        var east = new GameObject("East");
        east.transform.parent = topLightHost.transform;
        east.transform.localPosition = new Vector3(eastWestOffset, 0, 0);
        east.transform.Rotate(new Vector3(0, 90, 0));
        east.AddComponent<MeshFilter>().mesh = panelMesh_X_Length;
        east.AddComponent<MeshRenderer>().material = illuminationMaterial;

        constructedUnit = topLightInstance;

        AssetDatabase.SaveAssets();
    }

    private Mesh CreateCenterMesh()
    {
        return CreatePlane(spotPlaneWidth, spotPlaneWidth);
    } 

    private Mesh CreateLightPanelMeshForNorthAndSouth()
    {
        var width = spotPlaneWidth;

        var length = (lightingDimension.y / 2) - (spotPlaneWidth / 2);

        return CreatePlane(width, length);
    }

    private Mesh CreateLightPanelMeshForEastAndWest()
    {
        var width = spotPlaneWidth;
        var length = (lightingDimension.x / 2) - (spotPlaneWidth / 2);

        return CreatePlane(width, length);
    }

    private Mesh CreatePlane(float width, float length)
    {
        var mesh = new Mesh();

        var x1 = -width / 2;
        var x2 = width / 2;
        var y1 = -length / 2;
        var y2 = length / 2;

        var vertices = new List<Vector3>()
        {
            V(x1, 0, y1),
            V(x1, 0, y2),
            V(x2, 0, y2),
            V(x2, 0, y1)
        };

        mesh.SetVertices(vertices);

        var triangles = new List<int>()
        {
            2,1,0,
            0,3,2
        };

        mesh.SetTriangles(triangles, 0);

        var normals = new List<Vector3>()
        {
            Vector3.down,
            Vector3.down,
            Vector3.down,
            Vector3.down
        };

        mesh.SetNormals(normals);

        var uvs = new List<Vector2>()
        {
            Vector2.zero,
            V(0, 1),
            Vector2.one,
            V(1, 0)
        };

        mesh.SetUVs(0, uvs);

        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }


    protected override void OnBeforeCreatePrefab()
    {
        AssetDatabase.SaveAssets(); // Make sure that the mesh assets are saved!
    }


}
