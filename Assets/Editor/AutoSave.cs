using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoSave
{
    static AutoSave()
    {
        EditorApplication.playModeStateChanged += SaveOnPlay;
    }

    private static void SaveOnPlay(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
            return;

        Debug.Log("Auto-saving scenes...");
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("Auto-saving assets...");
        AssetDatabase.SaveAssets();

        Debug.LogWarning("Auto-save completed");
    }
    private static void OnDestroy() => 
        EditorApplication.playModeStateChanged -= SaveOnPlay;
}

