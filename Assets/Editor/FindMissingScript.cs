using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindMissingScripts : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Scripts In Scene")]
    static void FindMissingScriptsInScene()
    {
        GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();
        int count = 0;
        foreach (GameObject go in gos)
        {
            Component[] components = go.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"Missing script found on: {go.name}", go);
                    count++;
                }
            }
        }

        Debug.Log($"Found {count} missing scripts.");
    }
}

