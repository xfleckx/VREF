using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Assets.SNEED.Unity3D.Editor.Maze.UnitCreation;

public class UnitCreator : EditorWindow
{ 
    [MenuItem("SNEED/Maze/Unit Creator")]
    static void OpenUnitCreator()
    {
        var window = EditorWindow.GetWindow<UnitCreator>();

        window.titleContent = new GUIContent("Unit Creator");
        
        window.Show();
    }
    
    void Initialize()
    {
        if(availableCreators == null)
            availableCreators = new List<ICreatorState>();
       
        var baseUnitCreator = CreateAndInitialize<BasicUnitCreator, MazeUnit>();
        
        if (!availableCreators.Contains(baseUnitCreator))
            availableCreators.Add(baseUnitCreator);

        var hidingSpotCreator = CreateAndInitialize<HidingSpotCreator, HidingSpot>();
        
        if(!availableCreators.Contains(hidingSpotCreator))
             availableCreators.Add(hidingSpotCreator); 

        var topLightCreator = CreateAndInitialize<TopLightCreator, TopLighting>();

        if (!availableCreators.Contains(topLightCreator))
            availableCreators.Add(topLightCreator);

        if (currentVisibleCreator == null)
            currentVisibleCreator = baseUnitCreator;
    }

    List<ICreatorState> availableCreators;

    private CS CreateAndInitialize<CS, T>() where CS : CreatorState<T> where T : MonoBehaviour
    {
        var t = CreateInstance<CS>();
        t.hideFlags = HideFlags.HideAndDontSave;

        t.Initialize();

        return t;
    }

    private UnityEngine.Object lastSelection;
    
    private TopLightCreator topLightCreator; 

    [SerializeField]
    private ICreatorState currentVisibleCreator;

    #region Toogle Button
    public bool ToggleButton(bool state, string label)
    {
        BuildStyle();

        bool out_bool = false;

        if (state)
            out_bool = GUILayout.Button(label, toggled_style);
        else
            out_bool = GUILayout.Button(label);

        if (out_bool)
            return !state;
        else
            return state;
    }

    static GUIStyle toggled_style;
    public static GUIStyle StyleButtonToggled
    {
        get
        {
            return toggled_style;
        }
    }

    static GUIStyle labelText_style;
  
    private void BuildStyle()
    {
        if (toggled_style == null)
        {
            toggled_style = new GUIStyle(GUI.skin.button);
            toggled_style.normal.background = toggled_style.onActive.background;
            toggled_style.normal.textColor = toggled_style.onActive.textColor;
        }
        if (labelText_style == null)
        {
            labelText_style = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textField);
            labelText_style.normal = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button.onNormal;
        }
    }

    #endregion
    
    void OnGUI()
    {
        BuildStyle();

        var currentSelection = Selection.activeGameObject;

        if (currentSelection != null && (lastSelection != null || lastSelection != currentSelection))
            OnUpdateSelection(currentSelection);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(40));

        GUILayout.Space(4);

        foreach (var item in availableCreators)
        {
            GUIStyle style;

            if(item.Equals(currentVisibleCreator))
                style = toggled_style;
            else
                style = GUI.skin.button;

            if (GUILayout.Button(item.CreatorName, style))
            {
                currentVisibleCreator = item;
            }
        }
         
        EditorGUILayout.EndVertical();
        
        if(currentVisibleCreator != null)
            currentVisibleCreator.OnGUI();

        EditorGUILayout.EndHorizontal();
    }

    private void OnUpdateSelection(GameObject selection)
    { 
        var mazeUnit = selection.GetComponent<MazeUnit>();
        
        if (mazeUnit != null)
        {
            var dimension = mazeUnit.Dimension;

            foreach (var item in availableCreators)
            {
                item.RoomDimension = dimension;
            }
        }

        var maze = selection.GetComponent<beMobileMaze>();
        
        if (maze != null)
        {
            var dimension = maze.RoomDimension;

            foreach (var item in availableCreators)
            {
                item.RoomDimension = dimension;
            }
        }

        lastSelection = selection;
    }
      
    public void OnEnable()
    {
        Initialize();

        Selection.selectionChanged += Repaint;

        if (SceneView.onSceneGUIDelegate == null)
            SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    public void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeObject != null && Selection.activeObject is GameObject)
        {
            var go = Selection.activeObject as GameObject;

            var expectedMeshFilter = go.GetComponent<MeshFilter>();

            if (expectedMeshFilter == null)
                return;

            if (expectedMeshFilter.sharedMesh == null)
                return;
            
            var temp = Handles.matrix;

            Handles.matrix = go.transform.localToWorldMatrix;

            var vertices = expectedMeshFilter.sharedMesh.vertices;
            var vertexCount = vertices.Length;

            for (int i = 0; i < vertexCount; i++)
            {
                //var index = indices[i];
                var vertex = vertices[i];

                Handles.CubeCap(0, vertex, Quaternion.identity, 0.01f);
                
                var info = string.Format( "{0} {1}", i, vertex.ToString());
                
                Handles.Label(vertex, info);
            }
            
            Handles.matrix = temp;

        }
    } 

}

