using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Assets.SNEED.Unity3D.Editor.Maze;
using System.IO;

public class MazeEditorWindow : EditorWindow
{
    private EditorState state;

    private MazeBaker mazeBaker;
    
    public void Initialize(EditorState editorState)
    {
        titleContent = new GUIContent("Maze Editor");

        this.mazeBaker = new MazeBaker();

        this.state = editorState;

        editorState.EditorWindowVisible = true;
    }
     

    void OnGUI()
    {
        if (state == null) // the case that the Unity Editor remember the window
        {
            this.Close();
            return;
        }

        if (state.SelectedMaze == null)
        {
            EditorGUILayout.HelpBox("No maze Selected", MessageType.Error);
            return;
        }


        if (mazeBaker == null)
            mazeBaker = new MazeBaker();

        if (state.prefabOfSelectedMaze == null)
            state.CheckPrefabConnections();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("(1) Add a unit prefab!", EditorStyles.boldLabel);

        state.UnitPrefab = EditorGUILayout.ObjectField("Unit Prefab:", state.UnitPrefab, typeof(GameObject), false) as GameObject;

        if (state.UnitPrefab != null)
        {
            EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(state.UnitPrefab));
        }
        else
        {
            EditorGUILayout.HelpBox("First add an Unit prefab!", MessageType.Info);
        }
        

        EditorGUILayout.LabelField("(2) Define Maze Dimensions!", EditorStyles.boldLabel);

        state.MazeWidth = EditorGUILayout.FloatField("Widht", state.MazeWidth);

        state.MazeLength = EditorGUILayout.FloatField("Length", state.MazeLength);

        if (GUILayout.Button("Set Dimensions"))
        {
            state.SelectedMaze.MazeWidthInMeter = state.MazeWidth;
            state.SelectedMaze.MazeLengthInMeter = state.MazeLength;

            MazeEditorUtil.RebuildGrid(state.SelectedMaze);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("(3) Add units to the maze!", EditorStyles.boldLabel);

        #region Editing Mode UI

        if (state.UnitPrefab == null)
        {
            EditorGUILayout.HelpBox("You have to add a Unit prefab!", MessageType.Warning);
        }
        else if (!state.SelectedMaze.DoesNotContainPaths())
        {
            EditorGUILayout.HelpBox("Editing disabled, \n Maze contains paths!", MessageType.Warning);
        }
        else
        {
            RenderEditorModeOptions();
        }
        #endregion

        GUILayout.Space(10f);

        EditorGUILayout.LabelField("(4) Use selection to connect units!", EditorStyles.boldLabel);

        state.SelectionModeEnabled = GUILayout.Toggle(state.SelectionModeEnabled, "Selection Mode");

        #region Selection Mode UI

        if (state.SelectionModeEnabled)
        {

            if (state.ActiveMode != MazeEditorMode.SELECTION)
            {
                state.DisableModesExcept(MazeEditorMode.SELECTION);

                state.CurrentSelection = new List<GameObject>();

                state.EditorModeProcessEvent = null;
                state.EditorModeProcessEvent += state.SelectionMode;

                state.ActiveMode = MazeEditorMode.SELECTION;
            }

            if (GUILayout.Button("Connect", GUILayout.Width(100f)))
            {
                state.TryConnectingCurrentSelection();
            }

            if (GUILayout.Button("Disconnect", GUILayout.Width(100f)))
            {
                state.TryDisconnectingCurrentSelection();
            }
        }
        else
        {
            state.EditorModeProcessEvent -= state.SelectionMode;

            if (state.CurrentSelection != null)
                state.CurrentSelection.Clear();
        }
        #endregion


        GUILayout.Space(5f);

        if(state.prefabOfSelectedMaze != null && GUILayout.Button("Save prefab"))
        {
            PrefabUtility.ReplacePrefab(state.SelectedMaze.gameObject, state.prefabOfSelectedMaze, ReplacePrefabOptions.ConnectToPrefab);

            EditorUtility.SetDirty(state.prefabOfSelectedMaze);
        }

        if (GUILayout.Button("Save as new prefab"))
        {
            var filePath = EditorUtility.SaveFilePanel("Save as prefab", Application.dataPath, "prefabName", "prefab");

            if (filePath != null)
            {
                if (File.Exists(filePath))
                {
                    var relativeFileName = filePath.Replace(Application.dataPath, "Assets");

                    state.referenceToPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(relativeFileName);

                    state.prefabOfSelectedMaze = PrefabUtility.ReplacePrefab(state.SelectedMaze.gameObject, state.prefabOfSelectedMaze);
                }
                else
                {
                    var relativeFileName = filePath.Replace(Application.dataPath, "Assets");

                    state.prefabOfSelectedMaze = PrefabUtility.CreatePrefab(relativeFileName, state.SelectedMaze.gameObject);
                }
                
                EditorUtility.SetDirty(state.prefabOfSelectedMaze);
            }

        }

        GUILayout.Space(10f);

        EditorGUILayout.LabelField("(4) Bake the Bake to a single Mesh!", EditorStyles.boldLabel);

        mazeBaker.replaceOriginalMaze = EditorGUILayout.Toggle("Replace the original maze", mazeBaker.replaceOriginalMaze);

        mazeBaker.ignoreFloor = EditorGUILayout.Toggle("Remove floor on baking", mazeBaker.ignoreFloor);

        if (GUILayout.Button("Bake"))
        {
           state.finalizedMaze = mazeBaker.Bake(state.SelectedMaze);
        }

        if(GUILayout.Button("Safe baked maze as new prefab"))
        {
            var recommendedPath = EditorEnvironmentConstants.Get_PACKAGE_PREFAB_SUBFOLDER() + "/" + state.finalizedMaze.name + ".prefab";

            var chooseAnotherLocation = EditorUtility.DisplayDialog("Use default location?", "Using default location for these prefabs? \n " + recommendedPath, "No let me choose one", "Yes");

            if (chooseAnotherLocation)
            {
               recommendedPath = EditorUtility.SaveFilePanel("Save prefab", Application.dataPath, state.finalizedMaze.name, "prefab");
            }

            if(recommendedPath == null)
            {
                Debug.Log("You have to choose a path to save the prefab!");
                return;
            }


            PrefabUtility.CreatePrefab(recommendedPath, state.finalizedMaze.gameObject);
        }

    }

    private void RenderEditorModeOptions()
    {
        state.EditingModeEnabled = GUILayout.Toggle(state.EditingModeEnabled, "Editing Mode");

        if (state.EditingModeEnabled)
        {
            if (state.ActiveMode != MazeEditorMode.EDITING)
            {
                state.DisableModesExcept(MazeEditorMode.EDITING);
                state.EditorModeProcessEvent = null;
                state.EditorModeProcessEvent += state.EditingMode;
                state.ActiveMode = MazeEditorMode.EDITING;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(15f);
            GUILayout.BeginVertical();
            state.modeAddEnabled = GUILayout.Toggle(!state.modeRemoveEnabled, "Adding Cells");
            state.modeRemoveEnabled = GUILayout.Toggle(!state.modeAddEnabled, "Erasing Cells");
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        else
        {
            state.modeRemoveEnabled = false;
            state.modeAddEnabled = false;
            state.EditorModeProcessEvent -= state.EditingMode;
        }
    }

    void OnDestroy()
    {
        if(state != null)
            state.EditorWindowVisible = false;
    }
}
