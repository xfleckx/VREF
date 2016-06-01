using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Assets.SNEED.Unity3D.Editor.Maze;

public enum PathEditorMode { NONE, PATH_CREATION }

[CustomEditor(typeof(PathInMaze))]
public class PathEditor : AMazeEditor {

    PathInMaze instance;

    private LinkedList<MazeUnit> pathInSelection;

    private string pathElementPattern = "{0} {1} = {2} turn {3}";
    
    private bool PathCreationEnabled; 
    public PathEditorMode ActiveMode { get; set; }
    PathInMaze pathShouldBeRemoved;

    private bool showElements;
    private bool TilePositionIsValid = false;
    private Vector3 hoveringDistance;

    public void OnEnable()
    {
        instance = target as PathInMaze;

        editorState = EditorState.Instance;

        if (editorState.SelectedMaze == null)
            return;
        
        
        if (instance.PathAsLinkedList == null)
            instance.PathAsLinkedList = new LinkedList<PathElement>();

        instance.EditorGizmoCallbacks += RenderTileHighlighting;
        instance.EditorGizmoCallbacks += RenderEditorGizmos;
    }

    public void OnDisable()
    {
        if (instance == null)
            return;
        
    }

    public override void OnInspectorGUI()
    {
        instance = target as PathInMaze;

        if (instance != null) {
            editorState.SelectedMaze = instance.GetComponent<beMobileMaze>();
        }
        if(editorState.SelectedMaze == null) throw new MissingComponentException(string.Format("The Path Controller should be attached to a {0} instance", typeof(beMobileMaze).Name));

        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical();

        PathCreationEnabled = GUILayout.Toggle(PathCreationEnabled, "Path creation");
        
        showElements = EditorGUILayout.Foldout(showElements, "Show Elements");
        
        if(showElements)
            RenderElements();

        if (GUILayout.Button("Reverse Path"))
        {
            instance.InvertPath();
        }

        EditorGUILayout.Separator();
        
        EditorGUILayout.EndVertical();

        if (editorState.EditorModeProcessEvent != null)
            editorState.EditorModeProcessEvent(Event.current);
    }
    
    private void RenderElements()
    {
        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("Save Path"))
        {
            EditorUtility.SetDirty(instance);
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
        }

        foreach (var e in instance.PathAsLinkedList.ToList())
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                string.Format(pathElementPattern, e.Unit.GridID.x, e.Unit.GridID.y, 
                Enum.GetName(typeof(UnitType), e.Type), 
                Enum.GetName(typeof(TurnType), e.Turn)), 
                GUILayout.Width(150f));
            
            EditorGUILayout.ObjectField(e.Unit, typeof(MazeUnit), false);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    protected void RenderTileHighlighting()
    {
        if (editorState.SelectedMaze == null || !editorState.EditorWindowVisible)
            return;

        TilePositionIsValid = CheckIfTileIsValidPathElement(editorState.currentTilePosition);

        var maze = editorState.SelectedMaze;

        var temp = Gizmos.matrix;

        Gizmos.matrix = editorState.SelectedMaze.transform.localToWorldMatrix;

        if (!TilePositionIsValid)
        {
            Gizmos.color = Color.red;

        }
        else
        {
            // todo tilePosition from Unit
            if (instance.PathAsLinkedList.Last != null && instance.PathAsLinkedList.Last.Previous != null)
            {
                RenderTurningDegree(
                    instance.PathAsLinkedList.Last.Previous.Value.Unit.transform.position,
                    instance.PathAsLinkedList.Last.Value.Unit.transform.position,
                    editorState.MarkerPosition);
            }
        }
        
        Gizmos.DrawWireCube(editorState.MarkerPosition + new Vector3(0, maze.RoomDimension.y / 2, 0), new Vector3(maze.RoomDimension.x, maze.RoomDimension.y, maze.RoomDimension.z) * 1.1f);

        Gizmos.matrix = temp;
        
        Gizmos.color = Color.blue;
    }

    private void RenderTurningDegree(Vector3 lastUnitPosition, Vector3 currentUnitPosition, Vector3 nextUnitPosition)
    {
        var a = lastUnitPosition - currentUnitPosition;
        var b = currentUnitPosition - nextUnitPosition;

        var turningAngle = a.SignedAngle(b, Vector3.up); 
        var arcPosition = currentUnitPosition + hoveringDistance;

        Handles.color = Color.cyan;

        Handles.DrawSolidArc(arcPosition, Vector3.up, Vector3.forward, turningAngle, 0.3f);
        Handles.Label(arcPosition, turningAngle.ToString());

    }
    
    private bool CheckIfTileIsValidPathElement(Vector2 tilePosition)
    {
        if (!editorState.SelectedMaze.Units.Any((u) => u.GridID.Equals(tilePosition)))
            return false;

        if (instance.PathAsLinkedList.Count == 0)
            return true;

        var lastElement = instance.PathAsLinkedList.Last;

        var gridIdOfLastElement = lastElement.Value.Unit.GridID;

        if (tilePosition == gridIdOfLastElement)
            return false;

        if (lastElement.Previous != null) { 

            var gridIdOfPreviousElement = lastElement.Previous.Value.Unit.GridID;

            if (gridIdOfPreviousElement == tilePosition)
                return false;
        }

        var deltaBetweenLastAndCurrent = tilePosition - gridIdOfLastElement;
        var absDelta = deltaBetweenLastAndCurrent.SqrMagnitude();

        if (absDelta > 1)
            return false;
        
        return true;
    }

    public override void RenderSceneViewUI()
    {
        Handles.BeginGUI();

        #region Path creation mode

        EditorGUILayout.BeginVertical(GUILayout.Width(100f));
        
        if (PathCreationEnabled)
        { 
            if (ActiveMode != PathEditorMode.PATH_CREATION)
            { 
                pathInSelection = new LinkedList<MazeUnit>();

                editorState.EditorModeProcessEvent += PathCreationMode;
                ActiveMode = PathEditorMode.PATH_CREATION;
            }

            GUILayout.Space(4f);
            
        }
        else
        {
            editorState.EditorModeProcessEvent -= PathCreationMode;

            if (pathInSelection != null)
                pathInSelection.Clear();

        }


        EditorGUILayout.EndVertical();

        #endregion

        Handles.EndGUI();
    }
     
    #region path creation logic
    private void PathCreationMode(Event _ce)
    {
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        if (_ce.type == EventType.MouseDown || _ce.type == EventType.MouseDrag)
        {
            var unit = editorState.SelectedMaze.Grid[Mathf.FloorToInt(editorState.currentTilePosition.x), Mathf.FloorToInt(editorState.currentTilePosition.y)];

            if (unit == null)
            {
                Debug.Log("no element added");
                
                GUIUtility.hotControl = controlId;
                _ce.Use();

                return;
            }

            if (_ce.button == 0 && TilePositionIsValid)
            {
                Add(unit);

                EditorUtility.SetDirty(instance);
            }

            if (_ce.button == 1)
            {
                if(instance.PathAsLinkedList.Any() &&
                    instance.PathAsLinkedList.Last.Value.Unit.Equals(unit)) { 
                    Remove(unit);
                }

                EditorUtility.SetDirty(instance);
            } 

            GUIUtility.hotControl = controlId;
            _ce.Use();
        }
    }

    private void Add(MazeUnit newUnit)
    {
        var newElement = new PathElement(newUnit);
        
        newElement = GetElementType(newElement);
         
        var nr_el = instance.PathAsLinkedList.Count; // count all elements in the path to get the second last for turning calculation

        if (nr_el >= 1)
        {
            var previousElement = instance.PathAsLinkedList.Last.Value;

            if (nr_el >= 2)
            {
                var secpreviousElement = instance.PathAsLinkedList.Last.Previous.Value;
                newElement = GetTurnType(newElement, previousElement, secpreviousElement);
            }
            else
            {
                newElement.Turn = TurnType.STRAIGHT;
            } 
        }

        instance.PathAsLinkedList.AddLast(newElement);
    }
    
    public static PathElement GetElementType(PathElement element)
    {
        var u = element.Unit;

        if (u.WaysOpen == (OpenDirections.East | OpenDirections.West) ||
            u.WaysOpen == (OpenDirections.North | OpenDirections.South) || 
            u.WaysOpen == OpenDirections.East ||
            u.WaysOpen == OpenDirections.West ||
            u.WaysOpen == OpenDirections.North ||
            u.WaysOpen == OpenDirections.South )
        {
           element.Type = UnitType.I;
        }
        
        if(u.WaysOpen == OpenDirections.All)
            element.Type = UnitType.X;

        if(u.WaysOpen == (OpenDirections.West | OpenDirections.North | OpenDirections.East) ||
           u.WaysOpen ==  (OpenDirections.West | OpenDirections.South | OpenDirections.East) ||
           u.WaysOpen == (OpenDirections.West | OpenDirections.South | OpenDirections.North) ||
            u.WaysOpen ==  (OpenDirections.East | OpenDirections.South | OpenDirections.North) )
        {
            element.Type = UnitType.T;
        }

        if(u.WaysOpen == (OpenDirections.West | OpenDirections.North ) ||
           u.WaysOpen ==  (OpenDirections.West | OpenDirections.South ) ||
           u.WaysOpen == (OpenDirections.East | OpenDirections.South ) ||
            u.WaysOpen ==  (OpenDirections.East | OpenDirections.North) )
        {
            element.Type = UnitType.L;
        }

        return element;
    }

    public static PathElement GetTurnType(PathElement current, PathElement last, PathElement sec2last)
    {
        var x0 = sec2last.Unit.GridID.x;
        var y0 = sec2last.Unit.GridID.y;
        //  var x1 = last.Unit.GridID.x; // why unused?
        var y1 = last.Unit.GridID.y;
        var x2 = current.Unit.GridID.x;
        var y2 = current.Unit.GridID.y;

        if ((x0 - x2) - (y0 - y2) == 0) // same sign
        {
            if (y0 != y1) // first change in y
            {
                last.Turn = TurnType.RIGHT;
            }
            else
            {
                last.Turn = TurnType.LEFT;
            }
        }
        else // different sign
        {
            if (y0 != y1) // first change in y
            {
                last.Turn = TurnType.LEFT;
            }
            else
            {
                last.Turn = TurnType.RIGHT;
            }
        }

        if (Math.Abs(x0 - x2) == 2 || Math.Abs(y0 - y2) == 2)
        {
            last.Turn = TurnType.STRAIGHT;
        }

        return current;
    }

    private void Remove(MazeUnit unit)
    {
        var elementToRemove = instance.PathAsLinkedList.First(e => e.Unit.Equals(unit));

        instance.PathAsLinkedList.Remove(elementToRemove);
    }
    
    #endregion  

    protected void RenderEditorGizmos()
    {
        var maze = editorState.SelectedMaze;

        var temp_handles_color = Handles.color;

        if (!instance.enabled)
            return;

        if (instance.PathAsLinkedList.Count > 0)
        {
            hoveringDistance = new Vector3(0f, maze.RoomDimension.y, 0f);

            var start = instance.PathAsLinkedList.First.Value.Unit.transform;

            Handles.color = Color.blue;

            Handles.CubeCap(this.GetInstanceID(), start.position + hoveringDistance, start.rotation, 0.3f);


            var iterator = instance.PathAsLinkedList.GetEnumerator();
            MazeUnit last = null;

            while (iterator.MoveNext())
            {
                if (last == null)
                {
                    last = iterator.Current.Unit;
                    continue;
                }

                if (last == null || iterator.Current.Unit == null) { 
                    last = iterator.Current.Unit;
                    continue;
                }

                Gizmos.DrawLine(last.transform.position + hoveringDistance, iterator.Current.Unit.transform.position + hoveringDistance);

                last = iterator.Current.Unit;
            }
            
            var lastElement = instance.PathAsLinkedList.Last.Value.Unit;
            var endTransform = lastElement.transform;

            var coneRotation =  start.rotation;

            switch (lastElement.WaysOpen)
            {
                case OpenDirections.None:
                    break;
                case OpenDirections.North:
                    coneRotation.SetLookRotation(-endTransform.forward);
                    break;
                case OpenDirections.South:
                    coneRotation.SetLookRotation(endTransform.forward);
                    break;
                case OpenDirections.East:
                    coneRotation.SetLookRotation(-endTransform.right);
                    break;
                case OpenDirections.West:
                    coneRotation.SetLookRotation(endTransform.right);
                    break;
                case OpenDirections.All:
                    break;
                default:
                    break;
            }    
           
            Handles.ConeCap(this.GetInstanceID(), endTransform.position + hoveringDistance, coneRotation, 0.3f);
            Handles.color = temp_handles_color;
        }
    } 
}
