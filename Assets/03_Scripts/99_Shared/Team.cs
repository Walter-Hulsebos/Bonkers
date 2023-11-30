namespace Bonkers.Shared
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
        
        [ContextMenu(itemName: "Populate Enemies Automatically")]
        private void PopulateEnemiesAutomatically()
        {
            Team[] __allTeams = Resources.LoadAll<Team>(path: "");
            //Ignore self.
            Enemies = Array.FindAll(array: __allTeams, match: team => team != this);
        }
        
        #region Custom Editor

        #if UNITY_EDITOR
        [InitializeOnLoad]
        private sealed class TeamProjectWindowIcons
        {
            private static Color32
                _proColor   = new(r: 51,  g: 51,  b: 51,  a: 255),
                _scrubColor = new(r: 190, g: 190, b: 190, a: 255);

            private const float TEXT_PADDING_PRIMANTISSA = 0.08f;
            
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
            
            private static void DrawBackgroundRect(Rect rect) => EditorGUI.DrawRect(rect: CalculateIconRect(rect), color: BackgroundColor);

            [PublicAPI]
            private static void DrawTeamIcon(Rect rect, Team team)
            {
                Color __oldColor = GUI.color;
                GUI.color = team.Color;
                GUI.DrawTexture(position: CalculateIconRect(rect), image: _teamIcon);
                GUI.color = __oldColor;
            }
            
            private static Rect CalculateIconRect(Rect rect)
            {
                float __textPaddingPixels = rect.height * TEXT_PADDING_PRIMANTISSA;
                
                rect.x     =  rect.x;
                rect.y     -= __textPaddingPixels;
                rect.width =  rect.height -= __textPaddingPixels;
                
                return rect;
            }
        }
        #endif

        #endregion
    }
}