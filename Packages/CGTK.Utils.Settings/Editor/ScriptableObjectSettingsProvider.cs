// Copyright 2021 by Hextant Studios. https://HextantStudios.com
// This work is licensed under CC BY 4.0. http://creativecommons.org/licenses/by/4.0/

namespace CGTK.Utils.Settings.Editor
{
    using System;

    using UnityEditor;

    using UnityEngine;

    using Editor = UnityEditor.Editor;

    // SettingsProvider helper used to display settings for a ScriptableObject
    // derived class.
    public class ScriptableObjectSettingsProvider : SettingsProvider
    {
        public ScriptableObjectSettingsProvider( ScriptableObject settings, SettingsScope scope, String displayPath ) : base
            ( path: displayPath, scopes: scope ) =>
            this.settings = settings;

        // The settings instance being edited.
        public readonly ScriptableObject settings;

        // The SerializedObject settings instance.
        public  SerializedObject serializedSettings => _serializedSettings ??= new SerializedObject( obj: settings );
        private SerializedObject _serializedSettings;

        // Called when the settings are displayed in the UI.
        public override void OnActivate( String searchContext, UnityEngine.UIElements.VisualElement rootElement )
        {
            _editor = Editor.CreateEditor( targetObject: settings );
            base.OnActivate( searchContext: searchContext, rootElement: rootElement );
        }

        // Called when the settings are no longer displayed in the UI.
        public override void OnDeactivate()
        {
            Editor.DestroyImmediate( obj: _editor );
            _editor = null;
            base.OnDeactivate();
        }

        // Displays the settings.
        public override void OnGUI( String searchContext )
        {
            if ( settings == null || _editor == null ) return;

            // Set label width and indentation to match other settings.
            EditorGUIUtility.labelWidth = 250;
            GUILayout.BeginHorizontal();
            GUILayout.Space( pixels: 10 );
            GUILayout.BeginVertical();
            GUILayout.Space( pixels: 10 );

            // Draw the editor's GUI.
            _editor.OnInspectorGUI();

            // Reset label width and indent.
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;
        }

        // Build the set of keywords on demand from the settings fields.
        public override Boolean HasSearchInterest( String searchContext )
        {
            if ( !_keywordsBuilt )
            {
                keywords       = GetSearchKeywordsFromSerializedObject(serializedObject: serializedSettings );
                _keywordsBuilt = true;
            }

            return base.HasSearchInterest( searchContext: searchContext );
        }

        // True if the keywords set has been built.
        private Boolean _keywordsBuilt;

        // Cached editor used to render inspector GUI.
        private Editor _editor;
    }
}