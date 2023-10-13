// Copyright 2021 by Hextant Studios. https://HextantStudios.com
// This work is licensed under CC BY 4.0. http://creativecommons.org/licenses/by/4.0/

namespace CGTK.Utils.Settings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using UnityEditor;

    using UnityEngine;


    /// <summary>
    /// Base class for project/users settings. Use the [Settings] attribute to
    /// specify its usage, display path, and filename.
    /// - Derived classes *must* be placed in a file with the same name as the class.
    /// _ Settings are stored in `Assets/99_Settings/` folder.
    /// - The user settings folder `Assets/99_Settings/Editor/User/` *must* be
    ///   excluded from source control.
    /// - User settings will be placed in a subdirectory named the same as
    ///   the current project folder so that shallow cloning (symbolic links to
    ///   the Assets/ folder) can be used when testing multiplayer games.
    /// </summary>
    /// <typeparam name="T">CRTP</typeparam>
    public abstract class Settings<T> : ScriptableObject where T : Settings<T>
    {
        // The singleton instance. (Not thread safe but fine for ScriptableObjects.)
        public static  T Instance => _instance != null ? _instance : Initialize();
        private static T _instance;

        // Loads or creates the settings instance and stores it in _instance.
        protected static T Initialize()
        {
            // If the instance is already valid, return it. Needed if called from a 
            // derived class that wishes to ensure the settings are initialized.
            if ( _instance != null ) return _instance;

            // Verify there was a [Settings] attribute.
            if ( Attribute == null ) throw new InvalidOperationException(message: "[Settings] attribute missing for type: " + typeof( T ).Name);

            // Attempt to load the settings asset.
            String __filename = Attribute.filename ?? typeof( T ).Name;
            String __path     = GetSettingsPath() + __filename + ".asset";

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (Attribute.usage == SettingsUsage.RuntimeProject)
            {
                _instance = Resources.Load<T>( path: __filename );
            }
            #if UNITY_EDITOR
            else
            {
                _instance = AssetDatabase.LoadAssetAtPath<T>(assetPath: __path);
            }

            // Return the instance if it was the load was successful.
            if ( _instance != null ) { return _instance; }

            // Move settings if its path changed (type renamed or attribute changed)
            // while the editor was running. This must be done manually if the
            // change was made outside the editor.
            T[] __instances = Resources.FindObjectsOfTypeAll<T>();

            if ( __instances.Length > 0 )
            {
                String __oldPath = AssetDatabase.GetAssetPath( assetObject: __instances[ 0 ] );
                String __result  = AssetDatabase.MoveAsset( oldPath: __oldPath, newPath: __path );

                if (String.IsNullOrEmpty(value: __result)) { return _instance = __instances[ 0 ]; }

                Debug.LogWarning
                (
                    message: $"Failed to move previous settings asset " + $"'{__oldPath}' to '{__path}'. " +
                             $"A new settings asset will be created.",
                    context: _instance
                );
            }
            #endif
            
            // Create the settings instance if it was not loaded or found.
            if (_instance != null) { return _instance; }

            _instance = CreateInstance<T>();

            #if UNITY_EDITOR
            // Verify the derived class is in a file with the same name.
            MonoScript __script = MonoScript.FromScriptableObject( scriptableObject: _instance );

            if ( __script == null || __script.name != typeof( T ).Name )
            {
                DestroyImmediate( obj: _instance );
                _instance = null;
                throw new InvalidOperationException(message: "Settings-derived class and filename must match: " + typeof( T ).Name );
            }

            // Create a new settings instance if it was not found.
            // Create the directory as Unity does not do this itself.
            Directory.CreateDirectory( path: Path.Combine(path1: Directory.GetCurrentDirectory(), path2: Path.GetDirectoryName( path: __path ) ) );

            // Create the asset only in the editor.
            AssetDatabase.CreateAsset( asset: _instance, path: __path );
            #endif
            return _instance;
        }

        // Returns the full asset path to the settings file.
        private static String GetSettingsPath()
        {
            String __path = "Assets/99_Settings/";

            switch ( Attribute.usage )
            {
                case SettingsUsage.RuntimeProject:
                    __path += "Resources/";
                    break;
                #if UNITY_EDITOR
                case SettingsUsage.EditorProject:
                    __path += "Editor/";
                    break;

                case SettingsUsage.EditorUser:
                    __path += "Editor/User/" + GetProjectFolderName() + '/';
                    break;
                #endif
                default: throw new InvalidOperationException();
            }

            return __path;
        }

        // The derived type's [Settings] attribute.
        public  static SettingsAttribute Attribute => _attribute ??= typeof( T ).GetCustomAttribute<SettingsAttribute>( inherit: true );
        private static SettingsAttribute _attribute;

        // Called to validate settings changes.
        protected virtual void OnValidate() { }

        #if UNITY_EDITOR
        // Sets the specified setting to the desired value and marks the settings
        // so that it will be saved.
        protected void Set<TS>( ref TS setting, TS value )
        {
            if ( EqualityComparer<TS>.Default.Equals( x: setting, y: value ) ) return;
            setting = value;
            OnValidate();
            SetDirty();
        }

        // Marks the settings dirty so that it will be saved.
        protected new void SetDirty() => EditorUtility.SetDirty( target: this );

        // The directory name of the current project folder.
        private static String GetProjectFolderName()
        {
            String[] __path = Application.dataPath.Split( separator: '/' );
            return __path[^2];
        }
        #endif

        // Base class for settings contained by a Settings<T> instance.
        [Serializable]
        public abstract class SubSettings
        {
            // Called when a setting is modified.
            protected virtual void OnValidate() { }

            #if UNITY_EDITOR
            // Sets the specified setting to the desired value and marks the settings
            // instance so that it will be saved.
            protected void Set<TS>( ref TS setting, TS value )
            {
                if ( EqualityComparer<TS>.Default.Equals( setting, value ) ) { return; }

                setting = value;
                OnValidate();
                Instance.SetDirty();
            }
            #endif
        }
    }
}