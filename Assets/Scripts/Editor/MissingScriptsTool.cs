#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
/// <summary>
/// Editor tool for scanning selected prefabs for missing scripts,
/// displaying affected objects, and removing missing components.
/// Supports drag & drop for quick prefab selection.
/// </summary>
public class MissingScriptsTool : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private Vector2 scroll;

    private Dictionary<GameObject, List<GameObject>> results = new Dictionary<GameObject, List<GameObject>>();

    [MenuItem("Tools/Missing Scripts Tool")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptsTool>("Missing Scripts Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefabs List", EditorStyles.boldLabel);

        // список префабов
        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            prefabs[i] = (GameObject)EditorGUILayout.ObjectField(prefabs[i], typeof(GameObject), false);

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                prefabs.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Add Empty Slot"))
        {
            prefabs.Add(null);
        }

        if (GUILayout.Button("Clear List"))
        {
            prefabs.Clear();
            results.Clear();
        }

        GUILayout.Space(10);

        // 👇 DRAG & DROP ЗОНА
        DrawDropArea();

        GUILayout.Space(10);

        if (GUILayout.Button("Scan All"))
        {
            ScanAll();
        }

        if (results.Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("Scan Results:", EditorStyles.boldLabel);

            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(250));

            foreach (var kvp in results)
            {
                GUILayout.Label("Prefab: " + kvp.Key.name, EditorStyles.boldLabel);

                foreach (var obj in kvp.Value)
                {
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                }

                GUILayout.Space(5);
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            if (GUILayout.Button("Remove Missing Scripts From All"))
            {
                RemoveAll();
            }
        }
    }

    private void DrawDropArea()
    {
        Rect dropArea = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag & Drop Prefabs Here");

        Event evt = Event.current;

        if (!dropArea.Contains(evt.mousePosition))
            return;

        if (evt.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            evt.Use();
        }
        else if (evt.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();

            foreach (Object draggedObject in DragAndDrop.objectReferences)
            {
                if (draggedObject is GameObject go)
                {
                    // проверяем что это prefab
                    if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab)
                    {
                        if (!prefabs.Contains(go))
                        {
                            prefabs.Add(go);
                        }
                    }
                }
            }

            evt.Use();
        }
    }

    private void ScanAll()
    {
        results.Clear();

        foreach (var prefab in prefabs)
        {
            if (prefab == null) continue;

            List<GameObject> missingObjects = new List<GameObject>();

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            var allObjects = instance.GetComponentsInChildren<Transform>(true);

            foreach (var t in allObjects)
            {
                Component[] components = t.GetComponents<Component>();

                foreach (var comp in components)
                {
                    if (comp == null)
                    {
                        missingObjects.Add(t.gameObject);
                        break;
                    }
                }
            }

            DestroyImmediate(instance);

            if (missingObjects.Count > 0)
            {
                results[prefab] = missingObjects;
            }
        }
    }

    private void RemoveAll()
    {
        foreach (var prefab in prefabs)
        {
            if (prefab == null) continue;

            string path = AssetDatabase.GetAssetPath(prefab);
            GameObject root = PrefabUtility.LoadPrefabContents(path);

            var allObjects = root.GetComponentsInChildren<Transform>(true);

            foreach (var t in allObjects)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
            }

            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }

        Debug.Log("Missing scripts removed from all prefabs.");

        results.Clear();
    }
}
#endif