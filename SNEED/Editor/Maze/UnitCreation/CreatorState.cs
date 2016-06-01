using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.SNEED.Unity3D.Editor.Maze.UnitCreation
{
    public abstract class CreatorState<T> : ScriptableObject, ICreatorState where T : MonoBehaviour
    {
        public virtual string CreatorName { get { return "CreatorName"; } }

        public virtual string AssetSubPath { get { return "MazeUnitElements"; } }

        public virtual string AssetModelsPath { 
            get { 
                return EditorEnvironmentConstants.Get_PACKAGE_MODEL_SUBFOLDER() + Path.AltDirectorySeparatorChar + AssetSubPath;
            } 
        }

        public virtual string AssetPrefabPath
        {
            get
            {
                return EditorEnvironmentConstants.Get_PACKAGE_PREFAB_SUBFOLDER() + Path.AltDirectorySeparatorChar + AssetSubPath;
            }
        }

        protected Vector3 roomDimension = Vector3.one;
        /// <summary>
        /// Orientation for creating Maze Elements which are intented to be added as children of Maze Units
        /// </summary>
        public Vector3 RoomDimension
        {
            get { return roomDimension; }
            set {
                if (value != roomDimension) { 
                    roomDimension = value;
                    OnRoomDimensionUpdate();
                }
            }
        }

        protected abstract void OnRoomDimensionUpdate();

        protected Vector3 pivotOrigin = Vector3.zero;
        public Vector3 PivotOrigin
        {
            get { return pivotOrigin; }
            set { pivotOrigin = value; }
        }

        protected string prefabName = String.Empty;
        public string PrefabName
        {
            get { return prefabName; }
            set { prefabName = value; }
        }

        protected T constructedUnit = null;
        public T ConstructedUnit
        {
            get { return constructedUnit; }
            set { constructedUnit = value; }
        }

        protected GameObject prefabReference = null;
        public GameObject PrefabReference
        {
            get { return prefabReference; }
            set { prefabReference = value; }
        }

        public abstract void Initialize();

        public abstract Rect OnGUI();

        protected void Render_SaveAsPrefab_Option<O>(O instance) where O : MonoBehaviour
        {
            prefabName = EditorGUILayout.TextField("Prefab Name:", prefabName);

            if (constructedUnit != null && prefabReference == null)
            {
                if ((prefabName != null && prefabName != String.Empty) && GUILayout.Button("Save as Prefab")) { 

                    var targetPath = EditorEnvironmentConstants.Get_PROJECT_PREFAB_DIR_PATH();

                    Debug.Assert(AssetDatabase.IsValidFolder(targetPath), string.Format("Expected prefab folder at \"{0}\"", targetPath));

                    var targetFilePath = targetPath + Path.AltDirectorySeparatorChar +
                        string.Format("{0}.{1}", prefabName + roomDimension.AsPartFileName(), EditorEnvironmentConstants.PREFAB_EXTENSION);

                    EditorEnvironmentConstants.CreateWhenNotExist(targetPath);

                    OnBeforeCreatePrefab();

                    prefabReference = PrefabUtility.CreatePrefab(targetFilePath, instance.gameObject);

                    AssetDatabase.SaveAssets();

                    DestroyImmediate(instance.gameObject);
                }
                else
                {
                    EditorGUILayout.HelpBox("Please enter a name to enable\n\"Save as prefab\"", MessageType.Info);
                }

            }

        }

        protected abstract void OnBeforeCreatePrefab();

        #region Helper

        protected void SaveAsAsset(Mesh mesh, string targetPath, bool refreshDatabase = false)
        {
            Debug.Log(string.Format("create Mesh asset at: {0}", targetPath));
            
            var targetDirectory = Path.GetDirectoryName(targetPath);

            EditorEnvironmentConstants.CreateWhenNotExist(targetDirectory);

            AssetDatabase.CreateAsset(mesh, targetPath);

            if (refreshDatabase) { 
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        protected Material GetPrototypeMaterial()
        {
            var prototype = GameObject.CreatePrimitive(PrimitiveType.Plane);
            prototype.hideFlags = HideFlags.HideAndDontSave;
            var prototypeMeshRenderer = prototype.GetComponent<MeshRenderer>();
            var material = prototypeMeshRenderer.sharedMaterial;
            
            GameObject.DestroyImmediate(prototype);

            return material;
        }

        protected Vector3 V(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }

        protected Vector2 V(float x, float y)
        {
            return new Vector2(x, y);
        }

        #endregion

    }
}
