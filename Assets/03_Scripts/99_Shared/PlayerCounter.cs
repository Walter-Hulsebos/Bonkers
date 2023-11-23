namespace Bonkers.Lobby
{
    using System;
    
    using UnityEngine;

    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif

    public sealed class PlayerCounter : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [ShowInInspector] 
        #endif
        public static Int32 OnlineCount => Players.OnlineCount;
        #if ODIN_INSPECTOR
        [ShowInInspector] 
        #endif
        public static Int32 LocalCount  => Players.LocalCount;
        #if ODIN_INSPECTOR
        [ShowInInspector] 
        #endif
        public static Int32 TotalCount  => Players.TotalCount;
    }
}
