namespace Bonkers
{
    using System;
    
    using UnityEngine;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    
    using JetBrains.Annotations;

    using Bool = System.Boolean;
    
    
    [PublicAPI]
    [CreateAssetMenu(menuName = "Bonkers/Team")]
    public sealed class Team : ScriptableObject
    {
        [field:SerializeField] 
        public Color  Color   { get; [UsedImplicitly] private set; } = Color.white;
        [field:SerializeField]
        public Team[] Enemies { get; [UsedImplicitly] private set; } = Array.Empty<Team>();

        #region Custom Editor

        #if UNITY_EDITOR
        // [UnityEditor.CustomEditor(typeof(Team))]
        // private sealed class TeamEditor : UnityEditor.Editor
        // {
        //     public override void OnInspectorGUI()
        //     {
        //         base.OnInspectorGUI();
        //         
        //         //Draw an overlay of the team instance icons in the Project Window, draw an icon tinted in the color chosen for this team.
        //         //Use EditorApplication.projectWindowItemOnGUI to draw the overlayed icons.
        //         //Use EditorGUIUtility.FindTexture to find the icons for the team instances.
        //
        //         Texture2D __teamIcon = EditorGUIUtility.FindTexture(name: "TeamIcon");
        //         
        //         //EditorApplication.projectWindowItemOnGUI
        //         //EditorApplication.projectWindowItemInstanceOnGUI
        //         //EditorApplication.ProjectWindowItemCallback
        //         //EditorApplication.ProjectWindowItemInstanceCallback
        //
        //     }
        // }
        [InitializeOnLoad]
        private class TeamProjectWindowIcons
        {
            static TeamProjectWindowIcons()
            {
                EditorApplication.projectWindowItemOnGUI += DrawProjectWindowItem;
            }

            private static void DrawProjectWindowItem(String guid, Rect selectionRect)
            {
                String __assetPath = AssetDatabase.GUIDToAssetPath(guid: guid);
                
                if(!IsTeamAsset(assetPath: __assetPath)) return;
                
                
            }
            
            private static Bool IsTeamAsset(String assetPath)
            {
                if(!assetPath.EndsWith(value: ".asset", comparisonType: StringComparison.OrdinalIgnoreCase)) return false;
                
                Team __team = AssetDatabase.LoadAssetAtPath<Team>(assetPath: assetPath);
                return __team != null;
            }
        }
        #endif

        #endregion
    }
}