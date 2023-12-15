#if UNITY_EDITOR
using System;

using Bonkers.Utils;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

//using UnityEditor.Toolbars;

[InitializeOnLoad]
public static class StartFromFirstScene
{
    static StartFromFirstScene()
    {
        EditorToolbar.OnToolbarGui += OnDrawToolbar;
        
        // EditorSceneManager.sceneOpened -= SceneOpenedCallback;
        // EditorSceneManager.sceneOpened += SceneOpenedCallback;
        //
        // EditorApplication.update -= ValidateFirstScene;
        // EditorApplication.update += ValidateFirstScene;
    } 

    private static void OnDrawToolbar()
    {
        //GUILayout.FlexibleSpace();
        
        if (GUILayout.Button(new GUIContent(text: "1", tooltip: "Start Scene 1"), style: ToolbarButtonStyles.free_button_style))
        {
            EditorSceneHelpers.StartScene(sceneIndex: 0);
        }
    }

    private static class ToolbarButtonStyles
    {
        internal static readonly GUIStyle free_button_style = new(other: "Command")
        {
            fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, imagePosition = ImagePosition.ImageAbove,
        };
    }
    
    // /// <summary>
    // /// This method is used to validate first scene after Editor launch.
    // /// </summary>
    // private static void ValidateFirstScene()
    // {
    //     if (String.IsNullOrEmpty(SceneManager.GetActiveScene().name))
    //     {
    //         return;
    //     }
    //
    //     EditorApplication.update -= ValidateFirstScene;
    //     Scene activeScene = SceneManager.GetActiveScene();
    //     SceneOpenedCallback(activeScene, OpenSceneMode.Single);
    // }

    // /// <summary>
    // /// Handle <see cref="ToolbarButton"/>s addition for provided scene.
    // /// </summary>
    // /// <param name="scene"></param>
    // /// <param name="mode"></param>
    // private static void SceneOpenedCallback(Scene scene, OpenSceneMode mode)
    // {
    //     EditorToolbar.OnToolbarGui -= OnToolbarGui;
    //     if (scene.name != mySampleSceneName)
    //     {
    //         return;
    //     }
    //
    //     ToolboxEditorToolbar.OnToolbarGui += OnToolbarGui;
    // }
}
#endif