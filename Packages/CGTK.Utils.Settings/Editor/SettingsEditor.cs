// Copyright 2021 by Hextant Studios. https://HextantStudios.com
// This work is licensed under CC BY 4.0. http://creativecommons.org/licenses/by/4.0/

namespace CGTK.Utils.Settings.Editor
{
    using System;

    using UnityEditor;

    // A custom inspector for Settings that does not draw the "Script" field.
    [CustomEditor(inspectedType: typeof(Settings<>), editorForChildClasses: true)]
    public class SettingsEditor : Editor
    {
        public override void OnInspectorGUI() => DrawDefaultInspector();

        // Draws the UI for exposed properties *without* the "Script" field.
        protected new Boolean DrawDefaultInspector()
        {
            if( serializedObject.targetObject == null ) return false;

            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            DrawPropertiesExcluding( obj: serializedObject, propertyToExclude: _excludedFields );

            serializedObject.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        private static readonly String[] _excludedFields = { "m_Script" };
    }
}
