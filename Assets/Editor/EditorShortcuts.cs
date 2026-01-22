using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[InitializeOnLoad]
public static class EditorShortcuts
{
    private static List<GameObject> selectionHistory = new List<GameObject>();
    private const int MAX_HISTORY = 100;

    static EditorShortcuts()
    {
        // Initialize selection history tracking
        Selection.selectionChanged += () =>
        {
            if (Selection.activeGameObject != null)
            {
                selectionHistory.Add(Selection.activeGameObject);
                if (selectionHistory.Count > MAX_HISTORY)
                {
                    selectionHistory.RemoveAt(0);
                }
            }
        };
    }

    // Toggle current selection active/inactive
    [MenuItem("Custom Tools/Toggle Active State &z")] // Alt+Z
    private static void ToggleActiveState()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj, "Toggle Active State");
            obj.SetActive(!obj.activeSelf);
        }
    }

    // Create a new folder in the selected path or in Assets
    [MenuItem("Custom Tools/Create New Folder %#n")] // Ctrl+Shift+N
    private static void CreateNewFolder()
    {
        string folderPath = "Assets";

        if (Selection.activeObject != null)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path) && AssetDatabase.IsValidFolder(path))
            {
                folderPath = path;
            }
            else
            {
                string maybeFolder = Path.GetDirectoryName(path);
                if (AssetDatabase.IsValidFolder(maybeFolder))
                {
                    folderPath = maybeFolder;
                }
            }
        }

        string newFolderPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, "New Folder"));
        AssetDatabase.CreateFolder(folderPath, Path.GetFileName(newFolderPath));
        AssetDatabase.Refresh();

        Debug.Log($"Created folder: {newFolderPath}");
    }

    [MenuItem("Custom Tools/Toggle Inspector Lock &x")] // Alt+X
    private static void ToggleInspectorLock()
    {
        var tracker = ActiveEditorTracker.sharedTracker;
        if (tracker != null)
        {
            tracker.isLocked = !tracker.isLocked;
            tracker.ForceRebuild();

            // Refresh all inspector windows
            var inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            var inspectors = Resources.FindObjectsOfTypeAll(inspectorType);
            foreach (EditorWindow inspector in inspectors)
            {
                inspector.Repaint();
            }
        }
    }

    // Go to last selected object in hierarchy
    [MenuItem("Custom Tools/Go To Last Selected &s")] // Alt+s
    private static void GoToLastSelected()
    {
        if (selectionHistory.Count > 1)
        {
            GameObject lastSelected = selectionHistory[selectionHistory.Count - 2];
            if (lastSelected != null)
            {
                Selection.activeGameObject = lastSelected;
                EditorGUIUtility.PingObject(lastSelected);
            }
        }
    }
}