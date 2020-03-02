using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class test : EditorWindow
{

    [MenuItem("Tool/removeCollider")]
    static void showWindow()
    {
        var go = GameObject.Find("PV array");
        foreach (Transform child in go.GetComponentsInChildren<Transform>(true))
        {
            var c = child.GetComponent<Collider>();
            if (c != null)
            {
                DestroyImmediate(c);
            }
        }
        AssetDatabase.SaveAssets();

    }
}
