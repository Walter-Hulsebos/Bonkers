using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions
{
	[PublicAPI]
    public static class TypeChecks
    {
        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        public static Boolean HasUnityEventFunctions(this Type type)
	        => (type == null) || (type.IsSubclassOf(c: typeof(MonoBehaviour)) || type.IsSubclassOf(c: typeof(ScriptableObject)));
	}
}
