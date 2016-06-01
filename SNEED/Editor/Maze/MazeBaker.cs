using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

namespace Assets.SNEED.Unity3D.Editor.Maze
{
    public class MazeBaker
    {
        public bool replaceOriginalMaze = false;

        public bool ignoreFloor = false;

        private const bool MERGE_SUBMESHES = true;
        private const bool USE_MATRICES = true;

        public beMobileMaze Bake(beMobileMaze originalMaze)
        {
            beMobileMaze mazeToUse = null;

            if (!replaceOriginalMaze)
            {
                mazeToUse = GameObject.Instantiate(originalMaze);
                originalMaze.gameObject.SetActive(false);
            }

            Debug.Assert(mazeToUse.GetComponent<MeshFilter>() == null, "Component has already a MeshFilter");
             
            var meshFilter = mazeToUse.gameObject.AddComponent<MeshFilter>();

            Debug.Assert(mazeToUse.GetComponent<MeshRenderer>() == null, "Component has already a MeshRenderer");
            
            meshFilter.mesh = new Mesh();
            meshFilter.sharedMesh.name = mazeToUse.name;

            var meshRenderer = mazeToUse.gameObject.AddComponent<MeshRenderer>();

            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            meshRenderer.material = plane.GetComponent<MeshRenderer>().sharedMaterial;

            GameObject.DestroyImmediate(plane);

            var combineInstances = new List<CombineInstance>();

            var allMeshFilter = new List<MeshFilter>();
            var allRenderer = new List<MeshRenderer>();

            traverseAndCollectAll(new Stack<MazeUnit>(mazeToUse.Units), allMeshFilter, allRenderer);

            List<MeshFilter> selectedMeshFilter = allMeshFilter;

            selectedMeshFilter.RemoveAll(filter => !filter.gameObject.activeInHierarchy);
            
            if(ignoreFloor)
                selectedMeshFilter.RemoveAll(filter => filter.name.Equals("Floor") || filter.transform.parent.name.Equals("Floor"));

            foreach (var filter in selectedMeshFilter)
            {
                var combined = new CombineInstance();
                combined.mesh = filter.sharedMesh;
                combined.transform = filter.transform.localToWorldMatrix;
                combineInstances.Add(combined);

                Component.DestroyImmediate(filter);
            }
                  
            foreach (var renderer in allRenderer)
            {
                Component.DestroyImmediate(renderer);
            }
                 

            meshFilter.sharedMesh.CombineMeshes(combineInstances.ToArray(), MERGE_SUBMESHES, USE_MATRICES);

            meshFilter.sharedMesh.Optimize();

            meshFilter.sharedMesh.RecalculateNormals();

            AssetDatabase.CreateAsset(meshFilter.sharedMesh, EditorEnvironmentConstants.Get_PACKAGE_MODEL_SUBFOLDER() + "/" + mazeToUse.name + "_combinedMesh.asset");

            return mazeToUse;
        }

        private void traverseAndCollectAll(Stack<MazeUnit> units, List<MeshFilter> existingFilter, List<MeshRenderer> existingRenderer)
        { 
            if(units == null || units.Count == 0)
            {
                return;
            }

            var unit = units.Pop();

            extract(new Stack<GameObject>(unit.transform.AllChildren()), existingFilter, existingRenderer);
            
            traverseAndCollectAll(units, existingFilter, existingRenderer);
        }

        private void extract(Stack<GameObject> list, List<MeshFilter> filter, List<MeshRenderer> renderer)
        {
            if (list.Count == 0)
                return;

            var current = list.Pop();
            var expectedFilter = current.GetComponent<MeshFilter>();

            if ( expectedFilter != null && (current.name.Equals("North") ||
                                            current.name.Equals("West") ||
                                            current.name.Equals("East") ||
                                            current.name.Equals("South") ||
                                            current.name.Equals("Top") ||
                                            current.name.Equals("Floor") ))
            {
                extract(current, filter, renderer);
            }
            else
            {
                extractFromAllChildren(new Stack<GameObject>(current.transform.AllChildren()), filter, renderer);
            }

            extract(list, filter, renderer);
        }

        private void extractFromAllChildren(Stack<GameObject> list, List<MeshFilter> filter, List<MeshRenderer> renderer)
        {
            if(list.Count == 0)
            {
                return;
            }

            var current = list.Pop();
             
            extract(current, filter, renderer);

            var itsChildren = current.transform.AllChildren();

            if (itsChildren.Any()) { 
                extractFromAllChildren(new Stack<GameObject>(itsChildren), filter, renderer);
            }
            else
            {
                extractFromAllChildren(list, filter, renderer); 
            }
        }

        private void extract(GameObject go, List<MeshFilter> filter, List<MeshRenderer> renderer)
        { 
            var expectedFilter = go.GetComponent<MeshFilter>();

            if (expectedFilter != null)
                filter.Add(expectedFilter);

            var expectedRenderer = go.GetComponent<MeshRenderer>();

            if (expectedRenderer != null)
                renderer.Add(expectedRenderer);
        }
    }

}
