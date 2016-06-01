using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class EditorExtensions
{

    public static void ApplyToAll<T>(this IEnumerable<T> collection, Action<T> function)
    {
        foreach (var item in collection)
        {
            function(item);
        }
    }  

    public static bool IsPrefab(this UnityEngine.Object o){
        
        var prefabType = PrefabUtility.GetPrefabType(o);

        return prefabType == PrefabType.Prefab;
    }

    public static int RenderAsSelectionBox<T>(this IEnumerable<T> list, int selectionIndex)
    {
        int optionCount = list.Count();
        string[] options = new string[optionCount];
        list.Select(i => i.ToString()).ToArray().CopyTo(options, 0);
        selectionIndex = EditorGUILayout.Popup(selectionIndex, options);
        return selectionIndex;
    } 
} 