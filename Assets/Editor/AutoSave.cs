using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

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

        try
        {
            Debug.Log("Auto-saving scenes...");
            EditorSceneManager.SaveOpenScenes();

            Debug.Log("Auto-saving assets...");
            AssetDatabase.SaveAssets();

            Debug.Log("Auto-save completed");
        }
        catch (Exception ex)
        {
            Debug.Log($"Auto-save is incomplete! {ex.Message}");
        }
    }
    private static void OnDestroy() => 
        EditorApplication.playModeStateChanged -= SaveOnPlay;
}

