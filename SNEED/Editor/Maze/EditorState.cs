using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System;

namespace Assets.SNEED.Unity3D.Editor.Maze
{
    public class EditorState : ScriptableObject
    {
        private static EditorState instance;
        public static EditorState Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                else {
                    
                    instance = CreateInstance<EditorState>();
                    return instance;
                }
            }
        }

        public bool EditorWindowVisible = false;
        
        public beMobileMaze SelectedMaze;

        public float MazeWidth;

        public float MazeLength;

        public float unitFloorOffset = 0f;

        public string PathToMazePrefab = string.Empty;

        public bool SelectionModeEnabled = false;

        public List<GameObject> CurrentSelection;

        internal void CheckPrefabConnections()
        {
            prefabOfSelectedMaze = PrefabUtility.GetPrefabObject(SelectedMaze);
        }

        public bool EditingModeEnabled = false;
        public bool modeAddEnabled = false;
        public bool modeRemoveEnabled = false;

        public UnityEngine.Object referenceToPrefab;
        public UnityEngine.Object prefabOfSelectedMaze;

        public beMobileMaze finalizedMaze;

        public MazeEditorMode ActiveMode = MazeEditorMode.NONE;

        private MazeUnit lastAddedUnit;

        private GameObject unitPrefab;

        public string PathToUnitPrefab;

        public GameObject UnitPrefab
        {
            get
            {
                return unitPrefab;
            }

            set
            {
                if (unitPrefab != value)
                {
                    unitPrefab = value;
                    OnUnitPrefabChanged();
                }
            }
        }

        private void OnUnitPrefabChanged()
        {
            var unit = unitPrefab.GetComponent<MazeUnit>();

            if (SelectedMaze.RoomDimension != unit.Dimension)
                SelectedMaze.RoomDimension = unit.Dimension;

        }

        public void Initialize(beMobileMaze currentMaze)
        {
            SelectedMaze = currentMaze;

            MazeWidth = SelectedMaze.MazeWidthInMeter;

            MazeLength = SelectedMaze.MazeLengthInMeter;

            MazeEditorUtil.RebuildGrid(SelectedMaze);
            
            prefabOfSelectedMaze = PrefabUtility.GetPrefabParent(currentMaze);
        }
        
        public Vector3 MarkerPosition;
        public Color MarkerColor = Color.blue;
        public Vector3 draggingStart;
        public Vector2 currentTilePosition;
        public Vector3 mouseHitPos;

        public void DisableModesExcept(MazeEditorMode mode)
        {
            switch (mode)
            {
                case MazeEditorMode.NONE:
                    EditingModeEnabled = false;
                    SelectionModeEnabled = false;
                    break;
                case MazeEditorMode.EDITING:
                    SelectionModeEnabled = false;
                    break;
                case MazeEditorMode.SELECTION:
                    EditingModeEnabled = false;
                    break;
                default:
                    break;
            }
        }
        
        public Action<Event> EditorModeProcessEvent;
        
        #region Editor Modes

        public void EditingMode(Event _ce)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            // Before repaint
            if (_ce.type == EventType.Layout || _ce.type == EventType.layout)
            {

            }


            if (_ce.type == EventType.Repaint || _ce.type == EventType.repaint)
            {
            }



            if (_ce.type == EventType.MouseDown || _ce.type == EventType.MouseDrag)
            {
                if (EditingModeEnabled)
                {
                    if (modeAddEnabled)
                        Draw();
                    else if (modeRemoveEnabled)
                        Erase();
                }
                GUIUtility.hotControl = controlId;
                _ce.Use();
            }
        }

        private void Draw()
        {
            bool hasSuchAUnit = SelectedMaze.Units.Any((u) => u.GridID.x == currentTilePosition.x && u.GridID.y == currentTilePosition.y);
            // if there is already a tile present and it is not a child of the game object we can just exit.
            if (hasSuchAUnit)
            {
                return;
            }

            var unitHost = PrefabUtility.InstantiatePrefab(UnitPrefab) as GameObject;

            PrefabUtility.DisconnectPrefabInstance(unitHost);

            var unit = MazeEditorUtil.InitializeUnit(SelectedMaze, currentTilePosition, unitFloorOffset, unitHost);

            SelectedMaze.Grid[(int)currentTilePosition.x, (int)currentTilePosition.y] = unit;

            SelectedMaze.Units.Add(unit);

        }

        /// <summary>
        /// Erases a block at the pre-calculated mouse hit position
        /// </summary>
        private void Erase()
        {
            var unitHost = GameObject.Find(string.Format(SelectedMaze.UnitNamePattern, currentTilePosition.x, currentTilePosition.y));

            if (!unitHost)
            {
                Debug.Log("Nothing to erase!");
                return;
            }

            var unit = unitHost.GetComponent<MazeUnit>();

            // if a game object was found with the same assetName and it is a child we just destroy it immediately
            if (unit != null && unit.transform.parent == SelectedMaze.transform)
            {
                SelectedMaze.Units.Remove(unit);
                SelectedMaze.Grid[(int) currentTilePosition.x, (int)currentTilePosition.y] = null;
                DestroyImmediate(unit.gameObject);
            }
        }

        public void SelectionMode(Event _ce)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            if (_ce.type == EventType.MouseDown || _ce.type == EventType.MouseDrag)
            {

                var unitHost = GameObject.Find(string.Format(SelectedMaze.UnitNamePattern, currentTilePosition.x, currentTilePosition.y));

                if (unitHost != null)
                {
                    if (CurrentSelection.Contains(unitHost))
                    {
                        if (_ce.button == 1)
                            CurrentSelection.Remove(unitHost);
                    }
                    else {
                        if (_ce.button == 0)
                            CurrentSelection.Add(unitHost);
                    }
                }

                GUIUtility.hotControl = controlId;
                _ce.Use();
            }

        }

        public void TryConnectingCurrentSelection()
        {
            if (CurrentSelection == null)
                return;

            if (!CurrentSelection.Any())
                return;

            var iterator = CurrentSelection.GetEnumerator();

            MazeUnit last = null;

            while (iterator.MoveNext())
            {
                var current = iterator.Current.GetComponent<MazeUnit>();

                if (!last)
                {
                    last = current;
                    continue;
                }
                Debug.Log(current.GridID.ToString());
                // check if current and last are really neighbors:
                if (Math.Abs(current.GridID.x - last.GridID.x) + Math.Abs(current.GridID.y - last.GridID.y) == 1)
                {
                    // check which direction we go, possibilities:
                    if (current.GridID.x - last.GridID.x == 1) // going east
                    {
                        last.Open(OpenDirections.East);
                        current.Open(OpenDirections.West);
                    }
                    else if (current.GridID.x - last.GridID.x == -1) // going west
                    {
                        last.Open(OpenDirections.West);
                        current.Open(OpenDirections.East);
                    }

                    if (current.GridID.y - last.GridID.y == 1) // going north
                    {
                        last.Open(OpenDirections.North);
                        current.Open(OpenDirections.South);
                    }
                    else if (current.GridID.y - last.GridID.y == -1) // going south
                    {
                        last.Open(OpenDirections.South);
                        current.Open(OpenDirections.North);
                    }
                }


                last = current;
            }
        }

        public void TryDisconnectingCurrentSelection()
        {
            if (CurrentSelection == null)
                return;

            if (!CurrentSelection.Any())
                return;


            if (CurrentSelection.Count == 1)
            {
                var unit = CurrentSelection.First().GetComponent<MazeUnit>();
                unit.Close(OpenDirections.North);
                unit.Close(OpenDirections.South);
                unit.Close(OpenDirections.West);
                unit.Close(OpenDirections.East);
            }

            var iterator = CurrentSelection.GetEnumerator();

            MazeUnit last = null;

            while (iterator.MoveNext())
            {
                var current = iterator.Current.GetComponent<MazeUnit>();

                if (!last)
                {
                    last = current;
                    continue;
                }

                if (current.GridID.x - 1 == last.GridID.x)
                {
                    last.Close(OpenDirections.East);
                    current.Close(OpenDirections.West);
                }
                else if (current.GridID.x + 1 == last.GridID.x)
                {
                    last.Close(OpenDirections.West);
                    current.Close(OpenDirections.East);
                }

                if (current.GridID.y - 1 == last.GridID.y)
                {
                    last.Close(OpenDirections.North);
                    current.Close(OpenDirections.South);
                }
                else if (current.GridID.y + 1 == last.GridID.y)
                {
                    last.Close(OpenDirections.South);
                    current.Close(OpenDirections.North);
                }

                last = current;
            }
        }

        #endregion

    }

}