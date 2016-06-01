using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Assets.SNEED.Unity3D.Editor.Maze;
using Assets.BeMoBI.Unity3D.Editor.Maze;

public enum MazeEditorMode { NONE, EDITING, SELECTION }

[CustomEditor(typeof(beMobileMaze))]
public class MazeInspector : AMazeEditor
{
    public const string STD_UNIT_PREFAB_NAME = "MazeUnit";
    
    private MazeUnit lastAddedUnit;
    
    public void OnEnable()
    {
        editorState = EditorState.Instance;

        var currentTarget = target as beMobileMaze;
        
        editorState.Initialize(currentTarget);

        if (editorState.SelectedMaze.Grid == null)
        {
            MazeEditorUtil.RebuildGrid(editorState.SelectedMaze);
        }

        editorState.referenceToPrefab = PrefabUtility.GetPrefabParent(editorState.SelectedMaze.gameObject);

        if (editorState.referenceToPrefab != null) {
            editorState.PathToMazePrefab = AssetDatabase.GetAssetPath(editorState.referenceToPrefab);
        }

        SetupGUIStyle();

        if (editorState.SelectedMaze) {
            editorState.SelectedMaze.EditorGizmoCallbacks += RenderTileHighlighting;
            editorState.SelectedMaze.EditorGizmoCallbacks += RenderEditorGizmos;
        }
         
    }

    public override void OnInspectorGUI()
    {
        if (editorState.SelectedMaze == null) {
            renderEmptyMazeGUI();
        }
        
        GUILayout.BeginVertical();

        GUILayout.Label("Properties", EditorStyles.boldLabel);
        
        EditorGUILayout.LabelField("Length of Maze (m)", editorState.SelectedMaze.MazeLengthInMeter.ToString());
 
        EditorGUILayout.LabelField("Width of Maze (m)", editorState.SelectedMaze.MazeWidthInMeter.ToString());

        EditorGUILayout.LabelField("Units:", editorState.SelectedMaze.Units.Count.ToString());

        EditorGUILayout.LabelField("Room size (m):", editorState.SelectedMaze.RoomDimension.ToString());

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open Customizer", GUILayout.MinWidth(200), GUILayout.Height(40)))
        {
            var window = CreateInstance<MazeCustomizer>();

            window.Initialize(editorState.SelectedMaze);

            window.Show();
        }

        if(GUILayout.Button("Open Editor", GUILayout.MinWidth(120), GUILayout.Height(40)))
        {
            var window = EditorWindow.GetWindow<MazeEditorWindow>();
            
            window.Initialize(editorState);

            window.Show();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Close Maze Roof"))
        {
            foreach (var unit in editorState.SelectedMaze.Units)
            {
                var topTransform = unit.transform.FindChild("Top");
                if (topTransform != null)
                    topTransform.gameObject.SetActive(true);
            }
        }

        if (GUILayout.Button("Open Maze Roof"))
        {
            foreach (var unit in editorState.SelectedMaze.Units)
            {
                var topTransform = unit.transform.FindChild("Top");
                if (topTransform != null)
                    topTransform.gameObject.SetActive(false);
            }
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Search for Units")) 
        {
            MazeEditorUtil.CacheUnitsIn(editorState.SelectedMaze);
        }

        if (GUILayout.Button("Configure Grid"))
        {
            MazeEditorUtil.RebuildGrid(editorState.SelectedMaze);
        }

        if (editorState.referenceToPrefab && GUILayout.Button("Update Prefab"))
        {
            UpdatePrefabOfCurrentMaze();
        }

        if (GUILayout.Button("Repair Unit List"))
        {
            MazeEditorUtil.LookUpAllUnits(editorState.SelectedMaze);

        }

        GUILayout.EndVertical();

    }

    public void OnDisable()
    {
        if (editorState.SelectedMaze) {
            editorState.SelectedMaze.ClearCallbacks();
        }
    }
    
    
    private void UpdatePrefabOfCurrentMaze()
    {
       editorState.referenceToPrefab = PrefabUtility.ReplacePrefab(editorState.SelectedMaze.gameObject, editorState.referenceToPrefab, ReplacePrefabOptions.ConnectToPrefab);

       EditorUtility.SetDirty(editorState.referenceToPrefab);
       EditorApplication.delayCall += AssetDatabase.SaveAssets;
    }

    private void SavePrefabAndCreateCompanionFolder()
    {
        editorState.PathToMazePrefab = EditorUtility.SaveFilePanelInProject("Save maze", "maze.prefab", "prefab", "Save maze as Prefab");
        Debug.Log("Saved to " + editorState.PathToMazePrefab);
        editorState.referenceToPrefab = PrefabUtility.CreatePrefab(editorState.PathToMazePrefab, editorState.SelectedMaze.gameObject, ReplacePrefabOptions.ConnectToPrefab);
        
        Debug.Log("Create companion folder " + editorState.PathToMazePrefab);
    }

    private void RenderEditorGizmos(beMobileMaze maze)
    {
        var tempMatrix = Gizmos.matrix;

        Gizmos.matrix = maze.transform.localToWorldMatrix;

        var temp = Handles.matrix;
        Handles.matrix = Gizmos.matrix;

        if(editorState.EditorWindowVisible || !maze.Units.Any())
            EditorVisualUtils.DrawFloorGrid(editorState.SelectedMaze);

        Gizmos.color = Color.blue;

        if (editorState.EditorWindowVisible && editorState.CurrentSelection != null) { 
            foreach (var item in editorState.CurrentSelection)
            {
                var pos = item.transform.localPosition + new Vector3(0, maze.RoomDimension.y / 2, 0);
                Gizmos.DrawCube(pos, new Vector3(maze.RoomDimension.x, maze.RoomDimension.y, maze.RoomDimension.z));    
            }
        }

        Handles.matrix = temp;
        Gizmos.matrix = tempMatrix;
    }

    public override void RenderSceneViewUI()
    {
        if (EditorApplication.isPlaying)
            return;
        
        Handles.BeginGUI();

        GUILayout.BeginVertical(GUILayout.Width(200f));

        GUILayout.Label("Position in local Space of the maze");
        GUILayout.Label(string.Format("{0} {1} {2}", this.editorState.mouseHitPos.x, this.editorState.mouseHitPos.y, this.editorState.mouseHitPos.z));
        GUILayout.Label(string.Format("Marker: {0} {1} {2}", editorState.MarkerPosition.x, editorState.MarkerPosition.y, editorState.MarkerPosition.z));

        GUILayout.Space(10f);
        
        GUILayout.Space(10f);

        GUILayout.Label("Grid:");
        GUILayout.Space(3f);

        RenderMazeGrid(editorState.SelectedMaze);

        GUILayout.EndVertical();
        
        Handles.EndGUI();
    }
    
    void RenderMazeGrid(beMobileMaze maze) {

        if (maze.Grid == null)
            return;

        StringBuilder gridCode = new StringBuilder();
        StringBuilder line = new StringBuilder();

        int cols = maze.Grid.GetLength(0);
        int rows = maze.Grid.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (maze.Grid[c, r] != null)
                    line.AppendFormat(" {0}", 1);
                else
                    line.AppendFormat(" {0}", 0);
            }

            gridCode.AppendLine(line.ToString());
            line.Remove(0, line.Length);
        }

        GUILayout.Label(gridCode.ToString());
    }
    
    private void renderEmptyMazeGUI()
    {
        GUILayout.BeginVertical();

        if (GUILayout.Button("Edit selected maze", GUILayout.Width(255)))
        {
             
        } 

        GUILayout.EndVertical();
    }

}


public abstract class AMazeEditor : Editor {
    
    protected EditorState editorState;
    
    protected GUIStyle sceneViewUIStyle;
    
    public void OnSceneGUI()
    {
        if (!editorState.EditorWindowVisible)
            return;
        
        TileHighlightingOnMouseCursor();

        RenderSceneViewUI();

        if (editorState.EditorWindowVisible && editorState.EditorModeProcessEvent != null)
            editorState.EditorModeProcessEvent(Event.current);
    }

    protected void TileHighlightingOnMouseCursor()
    {
        // if UpdateHitPosition return true we should update the scene views so that the marker will update in real time
        if (this.UpdateHitPosition(editorState.SelectedMaze.transform))
        {
            this.RecalculateMarkerPosition();

            editorState.currentTilePosition = this.GetTilePositionFromMouseLocation(editorState.SelectedMaze, editorState.mouseHitPos);

            SceneView.currentDrawingSceneView.Repaint();
        }

    }

    public abstract void RenderSceneViewUI();

    protected virtual void SetupGUIStyle()
    {
        sceneViewUIStyle = new GUIStyle();
        sceneViewUIStyle.normal.textColor = Color.blue;
    }

    protected void RenderTileHighlighting(beMobileMaze maze)
    {
        if (!editorState.EditorWindowVisible)
            return;

        var tempMatrix = Gizmos.matrix;

        Gizmos.matrix = maze.transform.localToWorldMatrix;

        Gizmos.color = editorState.MarkerColor;

        var pos = editorState.MarkerPosition + new Vector3(0, maze.RoomDimension.y / 2, 0);

        Gizmos.DrawWireCube(pos, new Vector3(maze.RoomDimension.x, maze.RoomDimension.y, maze.RoomDimension.z) * 1.1f);

        var temp = Handles.matrix;

        Handles.matrix = Gizmos.matrix;

        Handles.Label(pos, string.Format("{0}.{1}", (int)editorState.MarkerPosition.x, (int)editorState.MarkerPosition.z), sceneViewUIStyle);

        Handles.matrix = temp;

        Gizmos.matrix = tempMatrix;
    }

    #region General calculations based on tile editor

    /// <summary>
    /// Calculates the location in tile coordinates (Column/Row) of the mouse position
    /// </summary>
    /// <returns>Returns a <see cref="Vector2"/> type representing the Column and Row where the mouse of positioned over.</returns>
    protected Vector2 GetTilePositionFromMouseLocation(beMobileMaze maze, Vector3 mouseHit)
    {
        // calculate column and row location from mouse hit location
        var pos = new Vector3(mouseHit.x / maze.RoomDimension.x, mouseHit.y / maze.transform.position.y, mouseHit.z / maze.RoomDimension.z);

        // round the numbers to the nearest whole number using 5 decimal place precision
        pos = new Vector3((int)Math.Round(pos.x, 5, MidpointRounding.ToEven), (int)Math.Round(pos.y, 5, MidpointRounding.ToEven), (int)Math.Round(pos.z, 5, MidpointRounding.ToEven));
        // do a check to ensure that the row and column are with the bounds of the tile maze
        var col = (int)pos.x;
        var row = (int)pos.z;
        if (row < 0)
        {
            row = 0;
        }

        if (row > maze.Rows - 1)
        {
            row = maze.Rows - 1;
        }

        if (col < 0)
        {
            col = 0;
        }

        if (col > maze.Columns - 1)
        {
            col = maze.Columns - 1;
        }

        // return the column and row values
        return new Vector2(col, row);
    }

    /// <summary>
    /// Recalculates the position of the marker based on the location of the mouse pointer.
    /// </summary>
    protected void RecalculateMarkerPosition()
    {
        // store the tile position in world space
        var pos = new Vector3(editorState.currentTilePosition.x * editorState.SelectedMaze.RoomDimension.x, 0, editorState.currentTilePosition.y * editorState.SelectedMaze.RoomDimension.z);

        // set the TileMap.MarkerPosition value
        editorState.MarkerPosition = new Vector3(pos.x + (editorState.SelectedMaze.RoomDimension.x / 2), pos.y, pos.z + (editorState.SelectedMaze.RoomDimension.z / 2));
    }

    /// <summary>
    /// Calculates the position of the mouse over the tile maze in local space coordinates.
    /// </summary>
    /// <returns>Returns true if the mouse is over the tile maze.</returns>
    protected bool UpdateHitPosition(Transform targetLocalSpace)
    {
        // build a plane object that 
        var p = new Plane(targetLocalSpace.TransformDirection(targetLocalSpace.up), targetLocalSpace.position);

        // build a ray type from the current mouse position
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // stores the hit location
        var hit = new Vector3();

        // stores the distance to the hit location
        float dist;

        // cast a ray to determine what location it intersects with the plane
        if (p.Raycast(ray, out dist))
        {
            // the ray hits the plane so we calculate the hit location in world space
            hit = ray.origin + (ray.direction.normalized * dist);
        }

        // convert the hit location from world space to local space
        var value = targetLocalSpace.InverseTransformPoint(hit);

        // if the value is different then the current mouse hit location set the 
        // new mouse hit location and return true indicating a successful hit test
        if (value != this.editorState.mouseHitPos)
        {
            this.editorState.mouseHitPos = value;
            return true;
        }

        // return false if the hit test failed
        return false;
    }

    #endregion

}