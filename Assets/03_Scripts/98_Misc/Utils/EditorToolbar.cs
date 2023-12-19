//Custom reimplementation of an idea orginally provided here - https://github.com/marijnz/unity-toolbar-extender, 2019

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Reflection;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

using Unity.EditorCoroutines.Editor;

using UnityEngine.UIElements;

using static UnityEngine.UIElements.FlexDirection;

//NOTE: since everything in this class is reflection-based it is a little bit "hacky"

/// <summary>
/// Toolbar extension which provides new funtionalites into classic Unity's scene toolbar.
/// </summary>
[InitializeOnLoad]
public static class EditorToolbar
{
    static EditorToolbar() { EditorCoroutineUtility.StartCoroutineOwnerless(routine: Initialize()); }

    private static readonly Type container_type = typeof(IMGUIContainer);
    private static readonly Type toolbar_type   = typeof(Editor).Assembly.GetType(name: "UnityEditor.Toolbar");
    private static readonly Type gui_view_type  = typeof(Editor).Assembly.GetType(name: "UnityEditor.GUIView");
    private static readonly Type backend_type   = typeof(Editor).Assembly.GetType(name: "UnityEditor.IWindowBackend");

    // private static readonly PropertyInfo gui_backend = gui_view_type.GetProperty
    //     (name: "windowBackend", bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    //
    // private static readonly PropertyInfo visual_tree = backend_type.GetProperty
    //     (name: "visualTree", bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    //
    // private static readonly FieldInfo on_gui_handler = container_type.GetField
    //     (name: "m_OnGUIHandler", bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    private static Object _toolbar;

    private static IEnumerator Initialize()
    {
        while (_toolbar == null)
        {
            Object[] __toolbars = Resources.FindObjectsOfTypeAll(type: toolbar_type);

            if (__toolbars == null || __toolbars.Length == 0)
            {
                yield return null;
                continue;
            }

            _toolbar = __toolbars[0];
        }

        FieldInfo     __rootField = _toolbar.GetType().GetField(name: "m_Root", bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);
        VisualElement __root      = __rootField.GetValue(obj: _toolbar) as VisualElement;
        VisualElement __toolbar   = __root.Q(name: "ToolbarZoneLeftAlign");

        VisualElement __element = new () { style = { flexGrow = 1, flexDirection = Row, }, };

        IMGUIContainer __container = new ();
        __container.style.flexGrow =  1;
        __container.onGUIHandler   += OnGui;

        __element.Add(child: __container);
        __toolbar.Add(child: __element);
    }

    private static void OnGui()
    {
        if (!IsToolbarAllowed || OnToolbarGui == null) { return; }

        using (new GUILayout.HorizontalScope()) { OnToolbarGui.Invoke(); }
    }


    public static Boolean IsToolbarAllowed { get; set; } = true;

    public static Single FromToolsOffset { get; set; } = 400.0f;
    public static Single FromStripOffset { get; set; } = 150.0f;

    public static event Action OnToolbarGui;

    private static class Style
    {
        internal static readonly Single row_height  = 30.0f;
        internal static readonly Single spacing     = 15.0f;
        internal static readonly Single top_padding = 5.0f;
        internal static readonly Single bot_padding = 3.0f;
    }
}
#endif