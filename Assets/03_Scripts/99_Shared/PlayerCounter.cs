namespace Bonkers.Lobby
{
    using System;
    
    using UnityEngine;

    using Sirenix.OdinInspector;

    public sealed class PlayerCounter : MonoBehaviour
    {
        [ShowInInspector] public static Int32 OnlineCount => Players.OnlineCount;
        [ShowInInspector] public static Int32 LocalCount  => Players.LocalCount;
        [ShowInInspector] public static Int32 TotalCount  => Players.TotalCount;
    }
}
