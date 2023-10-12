#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CGTK.Utils.UnityFunc;
using CGTK.Utils.UnityFunc.Attributes;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(TargetConstraintAttribute))]
[CustomPropertyDrawer(typeof(UnityFuncBase), true)]
public class UnityFuncDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Without this, you can't edit fields above the SerializedProperty
        property.serializedObject.ApplyModifiedProperties();

        // Indent label
        label = new GUIContent(label) { text = " " + label.text, };

        GUI.Box(position, "");

        position.y += 4;
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        property.serializedObject.Update();
        EditorGUI.BeginProperty(position, label, property);
        // Draw label
        Rect __pos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        Rect __targetRect = new (__pos.x, __pos.y, __pos.width, EditorGUIUtility.singleLineHeight);

        // Get target
        SerializedProperty __targetProp = property.FindPropertyRelative("_target");
        System.Object      __target     = __targetProp.objectReferenceValue;

        if (attribute is TargetConstraintAttribute)
        {
            Type __targetType = (attribute as TargetConstraintAttribute).targetType;
            EditorGUI.ObjectField(__targetRect, __targetProp, __targetType, GUIContent.none);
        }
        else { EditorGUI.PropertyField(__targetRect, __targetProp, GUIContent.none); }

        if (__target == null)
        {
            Rect __helpBoxRect = new
            (
                position.x         + 8,
                __targetRect.max.y + EditorGUIUtility.standardVerticalSpacing,
                position.width     - 16,
                EditorGUIUtility.singleLineHeight
            );

            String __msg = "Call not set. Execution will be slower.";
            EditorGUI.HelpBox(__helpBoxRect, __msg, MessageType.Warning);
        }
        else if (__target is MonoScript)
        {
            Rect __helpBoxRect = new
            (
                position.x                        + 8,
                __targetRect.max.y                + EditorGUIUtility.standardVerticalSpacing,
                position.width                    - 16,
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
            );

            String __msg = "Assign a GameObject, Component or a ScriptableObject, not a script.";
            EditorGUI.HelpBox(__helpBoxRect, __msg, MessageType.Warning);
        }
        else
        {
            Int32 __indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            // Get method name
            SerializedProperty __methodProp = property.FindPropertyRelative("_methodName");
            String             __methodName = __methodProp.stringValue;

            // Get args
            SerializedProperty __argProps = property.FindPropertyRelative("_args");
            Type[]             __argTypes = GetArgTypes(__argProps);

            // Get dynamic
            SerializedProperty __dynamicProp = property.FindPropertyRelative("_dynamic");
            Boolean            __dynamic     = __dynamicProp.boolValue;

            // Get active method
            MethodInfo __activeMethod = GetMethod(__target, __methodName, __argTypes);

            GUIContent __methodlabel = new ("n/a");

            if (__activeMethod != null) { __methodlabel = new GUIContent(PrettifyMethod(__activeMethod)); }
            else if (!String.IsNullOrEmpty(__methodName))
            {
                __methodlabel = new GUIContent("Missing (" + PrettifyMethod(__methodName, __argTypes) + ")");
            }

            Rect __methodRect = new
                (position.x, __targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

            // Method select button
            __pos = EditorGUI.PrefixLabel
                (__methodRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(__dynamic ? "Method (dynamic)" : "Method"));

            if (EditorGUI.DropdownButton(__pos, __methodlabel, FocusType.Keyboard)) { MethodSelector(property); }

            if (__activeMethod != null && !__dynamic)
            {
                // Args
                ParameterInfo[] __activeParameters = __activeMethod.GetParameters();

                Rect __argRect = new
                    (position.x, __methodRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

                String[] __types = new String[__argProps.arraySize];

                for (Int32 __i = 0; __i < __types.Length; __i++)
                {
                    SerializedProperty __argProp  = __argProps.FindPropertyRelative("Array.data[" + __i + "]");
                    GUIContent         __argLabel = new (ObjectNames.NicifyVariableName(__activeParameters[__i].Name));

                    EditorGUI.BeginChangeCheck();

                    switch ((Arg.ArgType) __argProp.FindPropertyRelative("argType").enumValueIndex)
                    {
                        case Arg.ArgType.Bool:
                            EditorGUI.PropertyField(__argRect, __argProp.FindPropertyRelative("boolValue"), __argLabel);
                            break;

                        case Arg.ArgType.Int:
                            EditorGUI.PropertyField(__argRect, __argProp.FindPropertyRelative("intValue"), __argLabel);
                            break;

                        case Arg.ArgType.Float:
                            EditorGUI.PropertyField(__argRect, __argProp.FindPropertyRelative("floatValue"), __argLabel);
                            break;

                        case Arg.ArgType.String:
                            EditorGUI.PropertyField(__argRect, __argProp.FindPropertyRelative("stringValue"), __argLabel);
                            break;

                        case Arg.ArgType.Object:
                            SerializedProperty __typeProp = __argProp.FindPropertyRelative("_typeName");
                            SerializedProperty __objProp  = __argProp.FindPropertyRelative("objectValue");

                            if (__typeProp != null)
                            {
                                Type __objType = Type.GetType(__typeProp.stringValue, false);
                                EditorGUI.BeginChangeCheck();
                                Object __obj = __objProp.objectReferenceValue;
                                __obj = EditorGUI.ObjectField(__argRect, __argLabel, __obj, __objType, true);

                                if (EditorGUI.EndChangeCheck()) { __objProp.objectReferenceValue = __obj; }
                            }
                            else { EditorGUI.PropertyField(__argRect, __objProp, __argLabel); }

                            break;
                    }

                    if (EditorGUI.EndChangeCheck()) { property.FindPropertyRelative("dirty").boolValue = true; }

                    __argRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            EditorGUI.indentLevel = __indent;
        }

        // Set indent back to what it was
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }

    private class MenuItem
    {
        public GenericMenu.MenuFunction action;
        public String                   path;
        public GUIContent               label;

        public MenuItem(String path, String name, GenericMenu.MenuFunction action)
        {
            this.action = action;
            label       = new GUIContent(path + '/' + name);
            this.path   = path;
        }
    }

    private void MethodSelector(SerializedProperty property)
    {
        // Return type constraint
        Type __returnType = null;
        // Arg type constraint
        Type[] __argTypes = Type.EmptyTypes;

        // Get return type and argument constraints
        UnityFuncBase __dummy        = GetDummyFunction(property);
        Type[]        __genericTypes = __dummy.GetType().BaseType.GetGenericArguments();

        // SerializableEventBase is always void return type
        if (__dummy is SerializableEventBase)
        {
            __returnType = typeof(void);

            if (__genericTypes.Length > 0)
            {
                __argTypes = new Type[__genericTypes.Length];
                Array.Copy(__genericTypes, __argTypes, __genericTypes.Length);
            }
        }
        else
        {
            if (__genericTypes is { Length: > 0, })
            {
                // The last generic argument is the return type
                __returnType = __genericTypes[^1];

                if (__genericTypes.Length > 1)
                {
                    __argTypes = new Type[__genericTypes.Length - 1];
                    Array.Copy(__genericTypes, __argTypes, __genericTypes.Length - 1);
                }
            }
        }

        SerializedProperty __targetProp = property.FindPropertyRelative("_target");

        List<MenuItem> __dynamicItems = new ();
        List<MenuItem> __staticItems  = new ();

        List<Object> __targets = new () { __targetProp.objectReferenceValue, };

        if (__targets[0] is Component)
        {
            __targets = (__targets[0] as Component).gameObject.GetComponents<Component>().ToList<Object>();
            __targets.Add((__targetProp.objectReferenceValue as Component).gameObject);
        }
        else if (__targets[0] is GameObject)
        {
            __targets = (__targets[0] as GameObject).GetComponents<Component>().ToList<Object>();
            __targets.Add(__targetProp.objectReferenceValue as GameObject);
        }

        for (Int32 __c = 0; __c < __targets.Count; __c++)
        {
            Object       __t       = __targets[__c];
            MethodInfo[] __methods = __targets[__c].GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            for (Int32 __i = 0; __i < __methods.Length; __i++)
            {
                MethodInfo __method = __methods[__i];

                // Skip methods with wrong return type
                if (__returnType != null && __method.ReturnType != __returnType) { continue; }

                // Skip methods with null return type
                // if (method.ReturnType == typeof(void)) continue;
                // Skip generic methods
                if (__method.IsGenericMethod) { continue; }

                Type[] __parms = __method.GetParameters().Select(x => x.ParameterType).ToArray();

                // Skip methods with more than 4 args
                if (__parms.Length > 4) { continue; }

                // Skip methods with unsupported args
                if (__parms.Any(x => !Arg.IsSupported(x))) { continue; }

                String __methodPrettyName = PrettifyMethod(__methods[__i]);

                __staticItems.Add
                (
                    new MenuItem
                    (
                        __targets[__c].GetType().Name + "/" + __methods[__i].DeclaringType.Name,
                        __methodPrettyName,
                        () => SetMethod(property, __t, __method, false)
                    )
                );

                // Skip methods with wrong constrained args
                if (__argTypes.Length == 0 || !__argTypes.SequenceEqual( __parms)) { continue; }

                __dynamicItems.Add
                (
                    new MenuItem
                    (
                        __targets[__c].GetType().Name + "/" + __methods[__i].DeclaringType.Name,
                        __methods[__i].Name,
                        () => SetMethod(property, __t, __method, true)
                    )
                );
            }
        }

        // Construct and display context menu
        GenericMenu __menu = new ();

        if (__dynamicItems.Count > 0)
        {
            String[] __paths = __dynamicItems.GroupBy(x => x.path).Select(x => x.First().path).ToArray();

            foreach (String __path in __paths) { __menu.AddItem(new GUIContent(__path + "/Dynamic " + PrettifyTypes(__argTypes)), false, null); }

            for (Int32 __i = 0; __i < __dynamicItems.Count; __i++) { __menu.AddItem(__dynamicItems[__i].label, false, __dynamicItems[__i].action); }

            foreach (String __path in __paths)
            {
                __menu.AddItem(new GUIContent(__path + "/  "),                false, null);
                __menu.AddItem(new GUIContent(__path + "/Static parameters"), false, null);
            }
        }

        for (Int32 __i = 0; __i < __staticItems.Count; __i++) { __menu.AddItem(__staticItems[__i].label, false, __staticItems[__i].action); }

        if (__menu.GetItemCount() == 0) { __menu.AddDisabledItem(new GUIContent("No methods with return type '" + GetTypeName(__returnType) + "'")); }

        __menu.ShowAsContext();
    }

    private String PrettifyMethod(String methodName, Type[] parmTypes)
    {
        String __parmnames = PrettifyTypes(parmTypes);
        return methodName + "(" + __parmnames + ")";
    }

    private String PrettifyMethod(MethodInfo methodInfo)
    {
        if (methodInfo == null) { throw new ArgumentNullException(nameof(methodInfo)); }

        ParameterInfo[] __parms     = methodInfo.GetParameters();
        String          __parmnames = PrettifyTypes(__parms.Select(x => x.ParameterType).ToArray());
        return GetTypeName(methodInfo.ReturnParameter.ParameterType) + " " + methodInfo.Name + "(" + __parmnames + ")";
    }

    private String PrettifyTypes(Type[] types)
    {
        if (types == null) { throw new ArgumentNullException(nameof(types)); }

        return String.Join(", ", types.Select(GetTypeName).ToArray());
    }

    private MethodInfo GetMethod(System.Object target, String methodName, Type[] types)
    {
        MethodInfo __activeMethod = target.GetType()
                                          .GetMethod
                                           (
                                               methodName,
                                               BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                                               null,
                                               CallingConventions.Any,
                                               types,
                                               null
                                           );

        return __activeMethod;
    }

    private Type[] GetArgTypes(SerializedProperty argsProp)
    {
        Type[] __types = new Type[argsProp.arraySize];

        for (Int32 __i = 0; __i < argsProp.arraySize; __i++)
        {
            SerializedProperty __argProp      = argsProp.GetArrayElementAtIndex(__i);
            SerializedProperty __typeNameProp = __argProp.FindPropertyRelative("_typeName");

            if (__typeNameProp != null) { __types[__i] = Type.GetType(__typeNameProp.stringValue, false); }

            if (__types[__i] == null) { __types[__i] = Arg.RealType((Arg.ArgType) __argProp.FindPropertyRelative("argType").enumValueIndex); }
        }

        return __types;
    }

    private void SetMethod(SerializedProperty property, Object target, MethodInfo methodInfo, Boolean dynamic)
    {
        SerializedProperty __targetProp = property.FindPropertyRelative("_target");
        __targetProp.objectReferenceValue = target;
        SerializedProperty __methodProp = property.FindPropertyRelative("_methodName");
        __methodProp.stringValue = methodInfo.Name;
        SerializedProperty __dynamicProp = property.FindPropertyRelative("_dynamic");
        __dynamicProp.boolValue = dynamic;
        SerializedProperty __argProp    = property.FindPropertyRelative("_args");
        ParameterInfo[]    __parameters = methodInfo.GetParameters();
        __argProp.arraySize = __parameters.Length;

        for (Int32 __i = 0; __i < __parameters.Length; __i++)
        {
            __argProp.FindPropertyRelative("Array.data[" + __i + "].argType").enumValueIndex =
                (Int32) Arg.FromRealType(__parameters[__i].ParameterType);

            __argProp.FindPropertyRelative("Array.data[" + __i + "]._typeName").stringValue = __parameters[__i].ParameterType.AssemblyQualifiedName;
        }

        property.FindPropertyRelative("dirty").boolValue = true;
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();
    }

    private static String GetTypeName(Type type)
    {
        if (type == typeof(Int32)) { return "int"; }

        if (type == typeof(Single)) { return "float"; }

        if (type == typeof(String)) { return "string"; }

        if (type == typeof(Boolean)) { return "bool"; }

        if (type == typeof(void)) { return "void"; }

        return type.Name;
    }

    public override Single GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Single             __lineheight  = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
        SerializedProperty __targetProp  = property.FindPropertyRelative("_target");
        SerializedProperty __argProps    = property.FindPropertyRelative("_args");
        SerializedProperty __dynamicProp = property.FindPropertyRelative("_dynamic");
        Single             __height      = __lineheight + __lineheight;

        if (__targetProp.objectReferenceValue      != null && __targetProp.objectReferenceValue is MonoScript) { __height += __lineheight; }
        else if (__targetProp.objectReferenceValue != null && !__dynamicProp.boolValue) { __height += __argProps.arraySize * __lineheight; }

        __height += 8;
        return __height;
    }

    private static UnityFuncBase GetDummyFunction(SerializedProperty prop)
    {
        String        __stringValue = prop.FindPropertyRelative("_typeName").stringValue;
        Type          __type        = Type.GetType(__stringValue, false);
        UnityFuncBase __result;

        if (__type == null) { return null; }
        else { __result = Activator.CreateInstance(__type) as UnityFuncBase; }

        return __result;
    }
}
#endif