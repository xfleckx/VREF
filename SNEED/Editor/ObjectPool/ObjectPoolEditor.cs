using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using System.Linq;

[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolEditor : Editor
{

    ObjectPool instance;
    private ReorderableList list;

    void OnEnable()
    {
        if (instance != null && instance.Categories.Any(i => i == null))
        {
            var newListWithoutNullElements = new List<Category>();

            foreach (var item in instance.Categories)
            {
                if (item != null)
                    newListWithoutNullElements.Add(item);
            }
            instance.Categories = newListWithoutNullElements;
        }

        list = new ReorderableList(serializedObject,
             serializedObject.FindProperty("Categories"),
             true, true, false, false);

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Categories");
        };

        list.onAddCallback = (l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            var newItem = new GameObject().AddComponent<Category>();
            newItem.transform.parent = instance.transform;
            element.objectReferenceValue = newItem;
        };

        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {

                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                var displayedProp = element.objectReferenceValue.name;

                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    displayedProp);
            };
    }

    private void lookUpCategoriesOn(ObjectPool instance)
    {
        var allChildren = instance.transform.AllChildren();

        foreach (var child in allChildren)
        {
            var category = child.GetComponent<Category>();

            if (!instance.Categories.Contains(category))
            {
                instance.Categories.Add(category);
                EditorUtility.SetDirty(instance);
            }
        }
    }


    public override void OnInspectorGUI()
    {
        instance = target as ObjectPool;

        lookUpCategoriesOn(instance);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();

        serializedObject.Update();

        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Open Pool Editor", GUILayout.Height(30)))
        {
            ObjectPoolTools.OpenPoolEditor();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

}
