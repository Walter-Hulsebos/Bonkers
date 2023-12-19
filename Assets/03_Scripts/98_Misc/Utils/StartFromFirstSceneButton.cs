// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine;
//
// namespace Bonkers
// {
// 	using System;
//
// 	using UnityToolbarExtender;
//
// 	internal static class ToolbarStyles
// 	{
// 		public static readonly GUIStyle commandButtonStyle;
//
// 		static ToolbarStyles()
// 		{
// 			commandButtonStyle = new GUIStyle(other: "Command")
// 			{
// 				fontSize      = 14,
// 				alignment     = TextAnchor.MiddleCenter,
// 				imagePosition = ImagePosition.ImageAbove,
// 				fontStyle     = FontStyle.Bold
// 			};
// 		}
// 	}
//
// 	[InitializeOnLoad]
// 	public class SceneSwitchLeftButton
// 	{
// 		static SceneSwitchLeftButton()
// 		{
// 			ToolbarExtender.LeftToolbarGUI.Add(item: OnToolbarGUI);
// 		}
//
// 		private static void OnToolbarGUI()
// 		{
// 			GUILayout.FlexibleSpace();
// 			if(GUILayout.Button(content: new GUIContent(text: "1", tooltip: "Start Scene 1"), style: ToolbarStyles.commandButtonStyle))
// 			{
// 				SceneHelper.StartScene(sceneIndex: 0);
// 			}
// 		}
// 	}
//
// 	internal static class SceneHelper
// 	{
// 		private static String _sceneNameToOpen  = null;
// 		private static Int32  _sceneIndexToOpen = -1;
//
// 		public static void StartScene(String sceneName)
// 		{
// 			if(EditorApplication.isPlaying)
// 			{
// 				EditorApplication.isPlaying = false;
// 			}
//
// 			_sceneNameToOpen          =  sceneName;
// 			EditorApplication.update += OnUpdateWithName;
// 		}
//
// 		public static void StartScene(Int32 sceneIndex)
// 		{
// 			if(EditorApplication.isPlaying)
// 			{
// 				EditorApplication.isPlaying = false;
// 			}
//
// 			_sceneIndexToOpen          = sceneIndex;
// 			EditorApplication.update += OnUpdateWithIndex;
// 		}
//
// 		private static void OnUpdateWithName()
// 		{
// 			if (_sceneNameToOpen == null || EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
// 			{
// 				return;
// 			}
//
// 			EditorApplication.update -= OnUpdateWithName;
//
// 			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
// 			{
// 				// need to get scene via search because the path to the scene
// 				// file contains the package version so it'll change over time
// 				String[] guids = AssetDatabase.FindAssets(filter: "t:scene " + _sceneNameToOpen, searchInFolders: null);
// 				if (guids.Length == 0)
// 				{
// 					Debug.LogWarning(message: "Couldn't find scene file");
// 				}
// 				else
// 				{
// 					String scenePath = AssetDatabase.GUIDToAssetPath(guid: guids[0]);
// 					EditorSceneManager.OpenScene(scenePath: scenePath);
// 					EditorApplication.isPlaying = true;
// 				}
// 			}
// 			_sceneNameToOpen = null;
// 		}
// 		
// 		private static void OnUpdateWithIndex()
// 		{
// 			if (_sceneIndexToOpen == -1 || EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
// 			{
// 				return;
// 			}
//
// 			EditorApplication.update -= OnUpdateWithIndex;
//
// 			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
// 			{
// 				EditorSceneManager.OpenScene(scenePath: EditorBuildSettings.scenes[_sceneIndexToOpen].path);
// 				EditorApplication.isPlaying = true;
// 			}
// 			_sceneIndexToOpen = -1;
// 		}
// 	}
// }
// #endif