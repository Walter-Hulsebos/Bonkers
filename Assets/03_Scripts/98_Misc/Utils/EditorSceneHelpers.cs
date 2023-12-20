#if UNITY_EDITOR
namespace Bonkers.Utils
{
	using System;

	using JetBrains.Annotations;

	using UnityEditor;
	using UnityEditor.SceneManagement;

	using UnityEngine;

	internal static class EditorSceneHelpers
	{
		private static String _sceneNameToOpen  = null;
		private static Int32  _sceneIndexToOpen = -1;

		[PublicAPI]
		public static void StartScene(String sceneName)
		{
			if (EditorApplication.isPlaying) { EditorApplication.isPlaying = false; }

			_sceneNameToOpen         =  sceneName;
			EditorApplication.update += OnUpdateWithName;
		}

		[PublicAPI]
		public static void StartScene(Int32 sceneIndex)
		{
			if (EditorApplication.isPlaying) { EditorApplication.isPlaying = false; }

			_sceneIndexToOpen        =  sceneIndex;
			EditorApplication.update += OnUpdateWithIndex;
		}

		private static void OnUpdateWithName()
		{
			if (_sceneNameToOpen == null || EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling ||
			    EditorApplication.isPlayingOrWillChangePlaymode) { return; }

			EditorApplication.update -= OnUpdateWithName;

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				// need to get scene via search because the path to the scene
				// file contains the package version so it'll change over time
				String[] guids = AssetDatabase.FindAssets(filter: "t:scene " + _sceneNameToOpen, searchInFolders: null);

				if (guids.Length == 0) { Debug.LogWarning(message: "Couldn't find scene file"); }
				else
				{
					String scenePath = AssetDatabase.GUIDToAssetPath(guid: guids[0]);
					EditorSceneManager.OpenScene(scenePath: scenePath);
					EditorApplication.isPlaying = true;
				}
			}

			_sceneNameToOpen = null;
		}

		private static void OnUpdateWithIndex()
		{
			if (_sceneIndexToOpen == -1 || EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling ||
			    EditorApplication.isPlayingOrWillChangePlaymode) { return; }

			EditorApplication.update -= OnUpdateWithIndex;

			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(scenePath: EditorBuildSettings.scenes[_sceneIndexToOpen].path);
				EditorApplication.isPlaying = true;
			}

			_sceneIndexToOpen = -1;
		}
	}
}
#endif