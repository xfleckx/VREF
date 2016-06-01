using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class MazeCustomizer : EditorWindow
{
    [MenuItem("SNEED/Maze/Customizer")]
    static void Init()
    {
        var window = EditorWindow.GetWindow<MazeCustomizer>();

        window.titleContent = new GUIContent( "Maze Customizer" );

        window.Show();
    }
    
    public void Initialize(beMobileMaze maze)
    {
        selectedMaze = maze;
    }

    void OnEnable()
    {
        Selection.selectionChanged += Repaint;
    }

    void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        if (Selection.activeGameObject != null)
            selectedMaze = Selection.activeGameObject.GetComponent<beMobileMaze>();

        if(selectedMaze == null) {
            renderUiForNoMazeSelected();
            return;
        }

        if(Selection.gameObjects.Length > 1 && Selection.gameObjects.All((o) => o.GetComponent<beMobileMaze>() != null))
        {
            renderUiForMultipleMazesSelected();
            return;
        }


        if (selectedMaze != null)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Selected Maze:");

            selectedMaze = EditorGUILayout.ObjectField(selectedMaze, typeof(beMobileMaze), true) as beMobileMaze;
            prefabOfSelectedMaze = PrefabUtility.GetPrefabParent(selectedMaze);


            if (GUILayout.Button("Other..."))
            {
                selectedMaze = null;
                Repaint();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(200));

            GUILayout.Label("Modify Maze Structure", EditorStyles.boldLabel);

            renderUiForSingleMazeSelected();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            GUILayout.Label("Modify Unit Appeareance", EditorStyles.boldLabel);

            renderUiForUnitSetCustomization();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Compatibility options - (these might be removed in later versions)");
        
        EditorGUILayout.Separator();

        if (prefabOfSelectedMaze != null)
        {
            if (GUILayout.Button("Update Prefab", GUILayout.Height(35)))
            {
                PrefabUtility.ReplacePrefab(selectedMaze.gameObject, prefabOfSelectedMaze, ReplacePrefabOptions.ConnectToPrefab);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("The Maze has no prefab reference! \n Create one!", MessageType.Warning);
        }


        EditorGUILayout.EndVertical();
    }
    

    GameObject lightPrefab;
    Material FloorMaterial;
    Material WallMaterial;
    Material TopMaterial;

    private void renderUiForUnitSetCustomization()
    {
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();

        GUILayout.Label("Change Materials for each unit");

        #region Materials

        EditorGUILayout.BeginHorizontal();
        WallMaterial = EditorGUILayout.ObjectField("Wall: ", WallMaterial, typeof(Material), false) as Material;
        if (WallMaterial != null && GUILayout.Button("Apply"))
        {
            selectedMaze.ApplyToAllMazeUnits( (u) =>
            {

                int c = u.transform.childCount;
                for (int i = 0; i < c; i++)
                {
                    var child = u.transform.GetChild(i);

                    if (child.name.Equals("East") ||
                        child.name.Equals("West") ||
                        child.name.Equals("North") ||
                        child.name.Equals("South"))
                    {
                        var renderer = child.gameObject.GetComponent<Renderer>();
                        renderer.material = WallMaterial;
                    }
                }
            });
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        FloorMaterial = EditorGUILayout.ObjectField("Floor: ", FloorMaterial, typeof(Material), false) as Material;
        if (FloorMaterial != null && GUILayout.Button("Apply"))
        {
            selectedMaze.ApplyToAllMazeUnits( (u) =>
            {
                int c = u.transform.childCount;
                for (int i = 0; i < c; i++)
                {
                    var child = u.transform.GetChild(i);

                    if (child.name.Equals("Floor"))
                    {
                        var renderer = child.gameObject.GetComponent<Renderer>();
                        renderer.material = FloorMaterial;
                    }
                }
            });
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        TopMaterial = EditorGUILayout.ObjectField("Top: ", TopMaterial, typeof(Material), false) as Material;
        if (TopMaterial && GUILayout.Button("Apply"))
        {
            selectedMaze.ApplyToAllMazeUnits( (u) =>
            {
                int c = u.transform.childCount;
                for (int i = 0; i < c; i++)
                {
                    var child = u.transform.GetChild(i);

                    if (child.name.Equals("Top"))
                    {
                        var renderer = child.gameObject.GetComponent<Renderer>();
                        renderer.material = TopMaterial;
                    }
                }
            });
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Disable Floor"))
        {
            selectedMaze.ApplyToAllMazeUnits((u) =>
            {
               var floor = u.transform.AllChildren().Where(c => c.name.Equals("Floor"));
                foreach (var item in floor)
                {
                    item.SetActive(false);
                }
            });
        }

        EditorGUILayout.EndHorizontal();

        #endregion

        EditorGUILayout.LabelField("Lighting", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();

        RenderUIForRequestedPrefab<GameObject, TopLighting>(ref lightPrefab, typeof(TopLighting));

        if (lightPrefab != null && GUILayout.Button("Add Lighting to each unit.", GUILayout.Height(50f)))
        {
            AddLightingToMaze();
        }
        
        if (GUILayout.Button("Remove Lighting to each unit.", GUILayout.Height(20f)))
        {
            RemoveLightingFromMaze();
        }

        if (GUILayout.Button("Remove only the Light from Top lightning.", GUILayout.Height(20f)))
        {
            RemoveTheLightFromLighting();
        }

        EditorGUILayout.EndHorizontal();

        if (lightPrefab)
        {
            var previewTexture = AssetPreview.GetAssetPreview(lightPrefab);

            GUILayout.Box(previewTexture);
        }

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();
    }

    private void RemoveTheLightFromLighting()
    {
        foreach (var unit in selectedMaze.Units)
        {
            var lightning = unit.GetComponentInChildren<TopLighting>();

            var light = lightning.GetComponentInChildren<Light>();

            if (light != null)
                DestroyImmediate(light);
        }
    }

    GameObject replacementPrefab;

    private beMobileMaze selectedMaze;
    private UnityEngine.Object prefabOfSelectedMaze;

    private void renderUiForSingleMazeSelected()
    { 
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Replace Units");

        replacementPrefab = (GameObject) EditorGUILayout.ObjectField(replacementPrefab, typeof(UnityEngine.GameObject), false);

        MazeUnit mazeUnit = null;

        string errorMessage = "No prefab selected!";

        if (replacementPrefab != null) { 

           mazeUnit = replacementPrefab.GetComponent<MazeUnit>();

        }
        else
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }

        if (mazeUnit == null) {

            if (replacementPrefab != null) { 
                errorMessage = "Selected Asset is no MazeUnit!";

                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
            }
        }
        else
        {
            if (GUILayout.Button(new GUIContent("Replace Maze Units \n with this prefab.", ToolTip_Replace_Units), GUILayout.Height(50f)))
            {
                CallReplaceAlgorithm();
            }

            EditorGUILayout.HelpBox(MsgBox_Replace_Action, MessageType.Warning);
        }

        EditorGUILayout.EndVertical();

        if (replacementPrefab) { 

            var previewTexture = AssetPreview.GetAssetPreview(replacementPrefab);

            GUILayout.Box(previewTexture);
        }

        if (GUILayout.Button(new GUIContent("Save as new prefab?", "Creates a new prefab instead of overwriting the old one!")))
        {
            var targetPath = EditorUtility.SaveFilePanelInProject("Save new maze prefab", "ResizedMaze", EditorEnvironmentConstants.PREFAB_EXTENSION, "sdfsfd");

            if(targetPath != null)
                PrefabUtility.CreatePrefab(targetPath, selectedMaze.gameObject);
        }

        // Resize currently through replacement of Units!
        //GUILayout.Label("Change:", EditorStyles.boldLabel);

        //selectedMaze.RoomDimension = EditorGUILayout.Vector3Field("Room dimension", selectedMaze.RoomDimension );

        //if (GUILayout.Button(new GUIContent("Resize (recommended)", "Resize the geometry"), GUILayout.Height(30)))
        //{
        //    var modificationModel = new UnitMeshModificationModel(selectedMaze.RoomDimension, Vector3.zero ,true); 
        //    ApplyToAllMazeUnits(selectedMaze, (u) => {

        //        MazeEditorUtil.ResizeUnitByMeshModification(u.gameObject, modificationModel);
        //    });
        //}

        //if (GUILayout.Button(new GUIContent("Rescale", "Rescale over the localScale value")))
        //{
        //    MazeEditorUtil.Rescale(selectedMaze, selectedMaze.RoomDimension, unitFloorOffset);
        //    MazeEditorUtil.RebuildGrid(selectedMaze);
        //}

    }

    private void RenderUIForRequestedPrefab<T, TC>(ref T targetPrefab, Type expectedComponent ) where T : UnityEngine.Object where TC : UnityEngine.MonoBehaviour
    { 
        EditorGUILayout.BeginVertical();

        targetPrefab = (T) EditorGUILayout.ObjectField(targetPrefab, typeof(T), false);

        TC targetComponent = null;

        string errorMessage = "No prefab selected!";

        if (targetPrefab != null && targetPrefab is GameObject)
        {
            targetComponent = (targetPrefab as GameObject).GetComponent<TC>();
        }
        else
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
        }

        if (targetComponent == null)
        {
            if (targetPrefab != null)
            {
                errorMessage = string.Format( "Selected Asset is no {0}", typeof(TC).Name);

                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void CallReplaceAlgorithm()
    {
        Debug.Assert(replacementPrefab != null && selectedMaze != null);

        if (selectedMaze.Units.Count == 0 || selectedMaze.Units.Any((u) => u == null))
            MazeEditorUtil.CacheUnitsIn(selectedMaze);

        var mazeUnitComponentOfPrefab = replacementPrefab.GetComponent<MazeUnit>();

        // make sure that the old values are available!
        MazeEditorUtil.ReconfigureGrid(selectedMaze, selectedMaze.MazeWidthInMeter, selectedMaze.MazeLengthInMeter);

        MazeEditorUtil.CheckAndUpdateMazeDefinitionOn(selectedMaze, mazeUnitComponentOfPrefab.Dimension);

        MazeEditorUtil.ReconfigureGrid(selectedMaze, selectedMaze.MazeWidthInMeter, selectedMaze.MazeLengthInMeter);

        MazeEditorUtil.ReplaceUnits(selectedMaze, replacementPrefab);

        var newUnitFromPrefab = PrefabUtility.InstantiatePrefab(replacementPrefab) as GameObject;

        var combinedBounds = new Bounds();
        var renderers = newUnitFromPrefab.GetComponentsInChildren<MeshRenderer>();

        foreach (var render in renderers)
        {
            combinedBounds.Encapsulate(render.bounds);
        }

        selectedMaze.RoomDimension = combinedBounds.size;

        var gridDim = MazeEditorUtil.CalcGridSize(selectedMaze);
        
        MazeEditorUtil.ReconfigureGrid(selectedMaze, gridDim.x, gridDim.y);

        foreach (var unit in selectedMaze.Units)
        {
            MazeEditorUtil.InitializeUnit(selectedMaze, unit.GridID, 0f, unit.gameObject);
        }

        GameObject.DestroyImmediate(newUnitFromPrefab.gameObject);

    }

    private void AddLightingToMaze()
    {
        foreach (var unit in selectedMaze.Units)
        {
            var top = unit.gameObject.transform.FindChild("Top"); 

            var newLighthost = PrefabUtility.InstantiatePrefab(lightPrefab) as GameObject;
            var newLightInstance = newLighthost.GetComponent<TopLighting>();

            PrefabUtility.DisconnectPrefabInstance(newLighthost);

            newLightInstance.transform.SetParent( top.transform, false );
            newLightInstance.transform.localPosition = new Vector3(0, -0.001f, 0);
            
            ConfigureTopLightBasedOnUnitConfiguration(unit, newLightInstance);
        }
    }
    
    public void ConfigureTopLightBasedOnUnitConfiguration(MazeUnit unit, TopLighting topLight)
    {
        var unitChildren = unit.transform.AllChildren();
        var lightChildren = topLight.transform.AllChildren();

        foreach (var lightChild in lightChildren)
        {
            var corresponding = unitChildren.Where((u) => u.name.Equals(lightChild.name)).FirstOrDefault();
           
            if (corresponding != null) 
                lightChild.SetActive(!corresponding.activeSelf);
        }
    }

    private void RemoveLightingFromMaze()
    {
        foreach (var unit in selectedMaze.Units)
        {
            var lightning = unit.GetComponentInChildren<TopLighting>(); 
            
            if(lightning != null)
                DestroyImmediate(lightning.gameObject);
        }
    }

    private void renderUiForNoMazeSelected()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.HelpBox("No Maze selected! \n Please create one or select one from scene!", MessageType.Info);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();

        var mazesInScene = Resources.FindObjectsOfTypeAll<beMobileMaze>();

        GUILayout.Space(5);

        GUILayout.Label(string.Format("Mazes found at the current scene:"));

        foreach (var item in mazesInScene)
        {
            if (!item.gameObject.IsPrefab() && GUILayout.Button(item.name))
            {
                Selection.activeGameObject = item.gameObject;
            }
        }

        GUILayout.Space(5);

        GUILayout.Label(string.Format("Mazes found as prefabs:"));

        foreach (var item in mazesInScene)
        {
            if (item.gameObject.IsPrefab() && GUILayout.Button(item.name))
            {
                var instance = PrefabUtility.InstantiatePrefab(item) as beMobileMaze;

                Selection.activeGameObject = instance.gameObject;
            }
        }
        EditorGUILayout.EndVertical();

    }

    #region TODO

    private void renderUiForMultipleMazesSelected()
    {
        EditorGUILayout.HelpBox("Editing of multiple mazes is currently not supported", MessageType.Info);
    }

    #endregion

    #region constants

    const string ToolTip_Replace_Units = "The the configuration of each Unit will be obtained";
    const string MsgBox_Replace_Action = "This action will replace all existing Units of the selected maze but obtains the structure of the maze and the configuration of each unit.";
 
    #endregion

}