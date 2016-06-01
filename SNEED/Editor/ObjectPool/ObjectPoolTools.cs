using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class ObjectPoolTools : EditorWindow
{ 
    [MenuItem("SNEED/Object Pool/Open Pool Editor")]
    public static void OpenPoolEditor()
    {
        var w = EditorWindow.CreateInstance<ObjectPoolTools>();

        w.titleContent = new GUIContent( "Object Pool Tools" );

        w.Initialize();

        w.Show();
    }

    private GameObject prefabReference;
    private ObjectPool currentPool;
    private GameObject lastPreviewObject;
    private Transform targetTransform;

    private void Initialize()
    {
        CheckIfAObjectPoolOrCategoryIsSelected();
    }

    private Type CheckIfAObjectPoolOrCategoryIsSelected()
    {
        var go = Selection.activeGameObject;

        if (go == null)
            return null;

        currentPool = go.GetComponent<ObjectPool>();

        // TODO get prefab connection if available!
        //prefabReference = PrefabUtility.G

        if (currentPool != null)
            return typeof(ObjectPool);

        selectedCategory = go.GetComponent<Category>();

        if (selectedCategory != null)
            return typeof(Category);

        return typeof(GameObject);
    }

    void OnGUI()
    {
        var selected = CheckIfAObjectPoolOrCategoryIsSelected();

        if(selected == null){ 
            renderUiForPoolSelectionOrCreation();
            return; 
        }

        if (selected == typeof(ObjectPool)) {
            CheckCategoryListConsistency(currentPool);
            renderUiForPoolEditing();
            return;

        }
        else if (selected == typeof(Category))
        {  
            renderUiForCategoryEditing();
            return;
        }

        if (selected == typeof(GameObject))
        {
            renderUiForTransformSelectionToObjectPool();
            return;
        }
    }

    #region Object pool

    private string newPoolName = "ObjectPool";

    private void renderUiForPoolSelectionOrCreation()
    {
        newPoolName = GUILayout.TextField(newPoolName);

        if (GUILayout.Button("Create new Pool")) { 
            CreateNewPoolInScene(newPoolName);
            Repaint();
        }

        var availablePools = GameObject.FindObjectsOfType<ObjectPool>();

        if (availablePools != null && availablePools.Any()) { 

        GUILayout.Label("Select one from available:");

            foreach (var item in availablePools)
            {
                if (GUILayout.Button(item.name)) {
                    Selection.activeObject = item;
                    Repaint();
                }
            }
        }
        
    }

    private void renderUiForTransformSelectionToObjectPool()
    {
        if(GUILayout.Button("Create Object Pool \n from current selection")){

            var go = Selection.activeGameObject;

            currentPool = go.AddComponent<ObjectPool>();

            int childCount = go.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var newCategoryHost = go.transform.GetChild(i);

                var newCategory = newCategoryHost.gameObject.AddComponent<Category>();
                
                currentPool.Categories.Add(newCategory);

                var objectCount = newCategory.transform.childCount;

                for (int j = 0; j < objectCount; j++)
                {
                    var potentialObject = newCategory.transform.GetChild(j);

                    var meshRenderer = potentialObject.GetComponentInChildren<MeshRenderer>();
                    var skinnedMeshRenderer = potentialObject.GetComponentInChildren<SkinnedMeshRenderer>();

                    if (meshRenderer != null || skinnedMeshRenderer != null) { 
                        newCategory.AssociatedObjects.Add(potentialObject.gameObject);
                        potentialObject.gameObject.SetActive(false);
                    }
                }
            }
        } 
    }

    private void renderUiForPoolEditing()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Edit Object Pool", EditorStyles.boldLabel);

        EditorGUILayout.Separator();

        newCategoryName = GUILayout.TextField(newCategoryName);

        if (!currentPool.Categories.Any(c => c.name == null || c.name == String.Empty || c.name.Equals(newCategoryName)))
        {
            if (GUILayout.Button("Add Category"))
            {
                CreateNewCategory(currentPool, newCategoryName);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Category name empty or already existing!", MessageType.Error);
        }

        if (prefabReference == null) {
            if (GUILayout.Button("Save as object pool as prefab"))
            {
                var filePath = EditorUtility.SaveFilePanelInProject("Save object pool as prefab", "objectPool", "prefab", "");
                
                prefabReference = PrefabUtility.CreatePrefab(filePath, currentPool.gameObject);
           
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            // TODO prefab updates!
        }

        EditorGUILayout.Space();

        GUILayout.Label("Get Random Object from Category", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("Use this only for testing or preview purposes!", MessageType.Info);

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();

        useTargetTransform = GUILayout.Toggle(useTargetTransform & targetTransform != null, "Use ");

        targetTransform = EditorGUILayout.ObjectField(targetTransform, typeof(Transform), true) as Transform;

        GUILayout.Label("as target");

        EditorGUILayout.EndHorizontal();

        foreach (var item in currentPool.Categories)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(item.name))
            {
                if (lastPreviewObject != null)
                    DestroyImmediate(lastPreviewObject);

                var original = item.Sample();

                var clone = GameObject.Instantiate(original);

                if (useTargetTransform) {
                    
                    clone.transform.ApplyTarget(targetTransform);
                }

                clone.SetActive(true);

                lastPreviewObject = clone;
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();

        specialLayer = EditorGUILayout.IntField("Layer", specialLayer);

        if(GUILayout.Button("Set Layer to all Objects"))
        {
            foreach (var category in currentPool.Categories)
            {
                foreach (var obj in category.AssociatedObjects)
                {
                    obj.layer = specialLayer;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add Trigger collider to all objects"))
        {
            foreach (var category in currentPool.Categories)
            {
                foreach (var obj in category.AssociatedObjects)
                {
                    var collider = obj.AddComponent<SphereCollider>();
                    collider.isTrigger = true;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        tag = EditorGUILayout.TextField("Tag", tag);

        if (GUILayout.Button("Tag all Objects"))
        {
            foreach (var category in currentPool.Categories)
            {
                foreach (var obj in category.AssociatedObjects)
                {
                    obj.tag = tag;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private int specialLayer = 0;
    private string tag = string.Empty;

    private bool useTargetTransform = false;

    private void CreateNewPoolInScene(string name)
    {
        var host = new GameObject(name);

        var pool = host.AddComponent<ObjectPool>();

        currentPool = pool; 
    }
    
    #endregion

    #region Category

    private void renderUiForCategorySelectionOrCreation()
    {

    }

    private void renderUiForCategoryEditing()
    {
        if(GUILayout.Button("Add Object from prefab")){

            string pathToPrefab = EditorUtility.OpenFilePanel("Prefab selection", Application.dataPath, "prefab");

            pathToPrefab = pathToPrefab.Replace(Application.dataPath,"Assets");

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathToPrefab);

            Debug.Assert(prefab != null, "Loading the prefab failed!");

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            prefabInstance.transform.parent = selectedCategory.transform;
            prefabInstance.SetActive(false);
            selectedCategory.AssociatedObjects.Add(prefabInstance);
        }


        tag = EditorGUILayout.TextField("Tag", tag);

        if (GUILayout.Button("Tag Objects"))
        {
            foreach (var obj in selectedCategory.AssociatedObjects)
            {
                obj.tag = tag;
            }
                 
        }
        
    }

    private void CheckCategoryListConsistency(ObjectPool targetPool)
    {
        var isNull = new Func<Category,bool>(c => c == null);
         
        if(currentPool.Categories.Any(isNull)){
            currentPool.Categories.RemoveAll(c => c == null);
        }
    }

    private void CreateNewCategory(ObjectPool targetPool, string categoryName)
    {
        var host = new GameObject(categoryName);

        host.transform.parent = targetPool.transform;

        var newCategory = host.AddComponent<Category>();

        targetPool.Categories.Add(newCategory);

        CheckCategoryListConsistency(targetPool);
    }

    private string newCategoryName = "newCategory";
    private Category selectedCategory;

    #endregion
}