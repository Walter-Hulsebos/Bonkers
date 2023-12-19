using System;

using UnityEngine;

using UnityEditor;

using System.Reflection;

using UnityEngine.UIElements;

namespace UnityToolbarExtender
{
    using Object = UnityEngine.Object;

    public static class ToolbarCallback
    {
        private static Type m_toolbarType = typeof(Editor).Assembly.GetType(name: "UnityEditor.Toolbar");
        private static Type m_guiViewType = typeof(Editor).Assembly.GetType(name: "UnityEditor.GUIView");
        #if UNITY_2020_1_OR_NEWER
        private static Type m_iWindowBackendType = typeof(Editor).Assembly.GetType(name: "UnityEditor.IWindowBackend");

        private static PropertyInfo m_windowBackend = m_guiViewType.GetProperty(name: "windowBackend", bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static PropertyInfo m_viewVisualTree = m_iWindowBackendType.GetProperty(name: "visualTree", bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        #endif
        private static FieldInfo m_imguiContainerOnGui = typeof(IMGUIContainer).GetField(name: "m_OnGUIHandler", bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static ScriptableObject m_currentToolbar;

        /// <summary>
        /// Callback for toolbar OnGUI method.
        /// </summary>
        public static Action OnToolbarGUI;

        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (m_currentToolbar == null)
            {
                // Find toolbar
                Object[] toolbars = Resources.FindObjectsOfTypeAll(type: m_toolbarType);
                m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject) toolbars[0] : null;

                if (m_currentToolbar != null)
                {
                    FieldInfo     root    = m_currentToolbar.GetType().GetField(name: "m_Root", bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance);
                    System.Object rawRoot = root.GetValue(obj: m_currentToolbar);
                    VisualElement mRoot   = rawRoot as VisualElement;
                    RegisterCallback(root: "ToolbarZoneLeftAlign",  cb: OnToolbarGUILeft);
                    RegisterCallback(root: "ToolbarZoneRightAlign", cb: OnToolbarGUIRight);

                    void RegisterCallback(String root, Action cb)
                    {
                        VisualElement toolbarZone = mRoot.Q(name: root);

                        VisualElement  parent    = new () { style = { flexGrow = 1, flexDirection = FlexDirection.Row, }, };
                        IMGUIContainer container = new ();
                        container.style.flexGrow =  1;
                        container.onGUIHandler   += () => { cb?.Invoke(); };
                        parent.Add(child: container);
                        toolbarZone.Add(child: parent);
                    }
                }
            }
        }

        private static void OnGUI()
        {
            Action handler = OnToolbarGUI;

            if (handler != null) { handler(); }
        }
    }
}