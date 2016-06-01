using Assets.SNEED.Unity3D.Editor.Maze.UnitCreation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.SNEED.Unity3D.Editor.Maze.UnitCreation
{
    public class HidingSpotCreator : CreatorState<HidingSpot>
    {

        Vector3 socketOffset;
        float socketHeight = 0.55f;
        GameObject socketPrefab;
        GameObject socketInstance;
        bool useCustomSocket = false;

        Vector2 doorDimension;
        float doorOffset = -0.3f;
        Vector3 doorColliderDimension;
        private float doorStepWidth = 0.05f;

        Mesh leftDoor;
        Mesh rightDoor;

        public override string CreatorName
        {
            get
            {
                return "Hiding Spot";
            }
        }

        public override void Initialize()
        { 
            // for this unit it does nothing...
        }

        protected override void OnRoomDimensionUpdate()
        {
            doorDimension = new Vector2(RoomDimension.x, RoomDimension.y);
            doorColliderDimension = new Vector3(doorDimension.x, doorDimension.y, doorStepWidth);
        }

        public override Rect OnGUI()
        {
            var rect = EditorGUILayout.BeginVertical();
            {
                roomDimension = EditorGUILayout.Vector3Field("Room Dimension", roomDimension);

                EditorGUILayout.Space();

                doorDimension = EditorGUILayout.Vector2Field("Spot Door Dimension (width x height)", doorDimension);

                doorStepWidth = EditorGUILayout.FloatField("Door Step Width", doorStepWidth);

                doorOffset = EditorGUILayout.FloatField("Door position", doorOffset);

                doorColliderDimension = EditorGUILayout.Vector3Field("Door Collider", doorColliderDimension);
            
                EditorGUILayout.Space();

                socketOffset = EditorGUILayout.Vector3Field("Socket position", socketOffset);

                socketHeight = EditorGUILayout.FloatField("Socket height", socketHeight);

                socketPrefab = EditorGUILayout.ObjectField("Custom socket:", socketPrefab, typeof(GameObject), false, null) as GameObject;

                useCustomSocket = EditorGUILayout.Toggle("Use custom socket", useCustomSocket) && (socketPrefab != null);

                EditorGUILayout.Space();

                if (GUILayout.Button("Create Hiding Spot", GUILayout.Height(25f)))
                {
                   constructedUnit = Construct();
                }

                EditorGUILayout.Space();

                if (constructedUnit != null)
                {
                    Render_SaveAsPrefab_Option(constructedUnit);
                }
            }
            EditorGUILayout.EndVertical();

            return rect;
        }

        protected override void OnBeforeCreatePrefab()
        {
            EditorEnvironmentConstants.CreateWhenNotExist(AssetModelsPath);

            var namePattern = "{0}{1}{2}_Mesh_{3}.asset";

            var leftDoorTargetPath = string.Format(namePattern, AssetModelsPath, Path.AltDirectorySeparatorChar ,"LeftDoorPanel", roomDimension.AsPartFileName());
            
            SaveAsAsset(leftDoor, leftDoorTargetPath);

            var rightDoorTargetPath = string.Format(namePattern, AssetModelsPath, Path.AltDirectorySeparatorChar, "RightDoorPanel", roomDimension.AsPartFileName());
            
            SaveAsAsset(rightDoor, rightDoorTargetPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private HidingSpot Construct()
        {
            var spotHost = new GameObject("HidingSpot");
            
            var hidingSpotController = spotHost.AddComponent<HidingSpot>();
            
            var prototypeMaterial = GetPrototypeMaterial();
            
            float doorPanelWidth = roomDimension.x / 2f;
            float doorPanelHeight = roomDimension.y;

            leftDoor = CreateDoorPlane(doorPanelWidth, doorPanelHeight, Vector3.zero, Vector3.left);
            rightDoor = CreateDoorPlane(doorPanelWidth, doorPanelHeight, Vector3.zero, Vector3.right);

            var door = new GameObject("Door");
            door.transform.parent = spotHost.transform;
            door.transform.localPosition = new Vector3(0, 0, doorOffset);
            door.AddComponent<SpotDoor>();

            var doorCollider = door.AddComponent<BoxCollider>();
            doorCollider.isTrigger = true;
            doorCollider.center = new Vector3(0, roomDimension.y / 2, 0);
            doorCollider.size = doorColliderDimension;

            var panelLeft = new GameObject("Left_Panel");
            panelLeft.transform.parent = door.transform;
            AddMesh(panelLeft, leftDoor, prototypeMaterial);
            panelLeft.transform.localPosition = new Vector3(roomDimension.x / 2, 0, 0);

            var panelRight = new GameObject("Right_Panel");
            panelRight.transform.parent = door.transform;
            AddMesh(panelRight, rightDoor, prototypeMaterial);
            panelRight.transform.localPosition = new Vector3(-roomDimension.x / 2, 0, 0);

            hidingSpotController.roomSize = RoomDimension;
            hidingSpotController.DoorA = panelLeft;
            hidingSpotController.DoorB = panelRight;
            hidingSpotController.DoorMovingDirection = HidingSpot.Direction.Horizontal;

            if (!useCustomSocket) {
                socketInstance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                socketInstance.transform.localPosition = new Vector3(socketOffset.x, socketHeight / 2, socketOffset.z);
                socketInstance.transform.localScale = new Vector3(0.2f, socketHeight, 0.2f);

                hidingSpotController.Socket = new GameObject("Socket");
                hidingSpotController.Socket.transform.SetParent(socketInstance.transform);
                hidingSpotController.Socket.transform.localPosition = new Vector3(socketOffset.x, socketHeight, socketOffset.z);
                hidingSpotController.Socket.transform.localScale = Vector3.one;
            }
            else
            {
                socketInstance = PrefabUtility.InstantiatePrefab(socketPrefab) as GameObject;
                socketInstance.transform.localPosition = socketOffset;
            }

            socketInstance.transform.parent = spotHost.transform;

            return hidingSpotController;
        }

        private void AddMesh(GameObject host, Mesh mesh, Material material)
        {
            var filter = host.AddComponent<MeshFilter>();

            var renderer = host.AddComponent<MeshRenderer>();

            filter.mesh = mesh;

            renderer.material = material;
        }

        private Mesh CreateDoorPlane(float width, float height, Vector3 origin, Vector3 orientationFormOrigin)
        {
            var frontSize = V(width * orientationFormOrigin.x, height, 0);

            var mesh = new Mesh();

            if (orientationFormOrigin == Vector3.right) { 

                var vertices = new List<Vector3>()
                {
                    V(origin.x,     origin.y,       0),
                    V(origin.x,     frontSize.y,    0),
                    V(frontSize.x,  frontSize.y,    0),
                    V(frontSize.x,  origin.y,       0)
                };

                mesh.SetVertices(vertices);

                var triangles = new List<int>()
                {
                    2,3,0,
                    0,1,2
                };

                mesh.SetTriangles(triangles, 0);

            }
            else
            {
                var vertices = new List<Vector3>()
                {
                    V(frontSize.x,  frontSize.y,    0),
                    V(frontSize.x,  origin.y,       0),
                    V(origin.x,     origin.y,       0),
                    V(origin.x,     frontSize.y,    0)
                };

                mesh.SetVertices(vertices);

                var triangles = new List<int>()
                {
                    2,1,0,
                    0,3,2
                };

                mesh.SetTriangles(triangles, 0); 
            }

            var normals = new List<Vector3>()
            {
                Vector3.back,
                Vector3.back,
                Vector3.back,
                Vector3.back
            
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
    }

}
