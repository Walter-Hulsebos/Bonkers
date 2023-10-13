// Copyright 2021 by Hextant Studios. https://HextantStudios.com
// This work is licensed under CC BY 4.0. http://creativecommons.org/licenses/by/4.0/

namespace CGTK.Utils.Settings.Editor
{
    using UnityEditor;

    using UnityEngine;

    public static class SettingsExtensions
    {
        // The SettingsProvider instance used to display settings in Edit/Preferences
        // and Edit/Project Settings.
        public static SettingsProvider GetSettingsProvider<T>(this Settings<T> settings ) where T : Settings<T>
        {
            Debug.Assert( condition: Settings<T>.Attribute.displayPath != null );

            return new ScriptableObjectSettingsProvider(
                settings:    settings,
                scope:       Settings<T>.Attribute.usage == SettingsUsage.EditorUser 
                                 ? SettingsScope.User 
                                 : SettingsScope.Project,
                displayPath: Settings<T>.Attribute.displayPath
            );
        }
    }
}