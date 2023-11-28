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
        private sealed class TeamProjectWindowIcons
        {
            private static Color32
                _proColor   = new(r: 51,  g: 51,  b: 51,  a: 255),
                _scrubColor = new(r: 190, g: 190, b: 190, a: 255);
            
            [PublicAPI]
            public static ref Color32 BackgroundColor => ref EditorGUIUtility.isProSkin ? ref _proColor : ref _scrubColor;
            
            private static Texture2D _teamIcon = null;
            
            static TeamProjectWindowIcons()
            {               
                _teamIcon = EditorGUIUtility.FindTexture(name: "Assets/03_Scripts/99_Shared/TeamIcon.png");
                Debug.Assert(condition: _teamIcon != null, message: nameof(_teamIcon) + " is NULL!!");
                
                EditorApplication.projectWindowItemOnGUI += DrawProjectWindowItem;
            }

            private static void DrawProjectWindowItem(String guid, Rect selectionRect)
            {
                String __assetPath = AssetDatabase.GUIDToAssetPath(guid: guid);
                
                if(!IsTeamAsset(assetPath: __assetPath, team: out Team __team)) return;
                
                DrawBackgroundRect(rect: selectionRect);
                
                DrawTeamIcon(rect: selectionRect, team: __team);
            }
            
            private static Bool IsTeamAsset(String assetPath, out Team team)
            {
                if (!assetPath.EndsWith(value: ".asset", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    team = null;
                    return false;
                }
                
                team = AssetDatabase.LoadAssetAtPath<Team>(assetPath: assetPath);
                return team != null;
            }
            
            private static void DrawBackgroundRect(Rect rect) => EditorGUI.DrawRect(rect: rect, color: BackgroundColor);

            [PublicAPI]
            private static void DrawTeamIcon(Rect rect, Team team)
            {
                //EditorGUI.DrawPreviewTexture();
                
                Color __oldColor = GUI.color;
                GUI.color = team.Color;

                Vector2 __pos  = rect.position;
                Vector2 __size = new(x: rect.height, y: rect.height);
			
                GUI.DrawTexture(position: new Rect(position: __pos, size: __size), image: _teamIcon);
                //GUI.DrawTexture(position: rect, image: _teamIcon);
                
                GUI.color = __oldColor;
            }
        }
        #endif

        #endregion
    }
}