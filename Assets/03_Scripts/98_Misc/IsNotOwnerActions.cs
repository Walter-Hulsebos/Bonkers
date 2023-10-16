namespace Bonkers
{
    using System;

    using UnityEngine;
    using Unity.Netcode;
    
    using UltEvents;

    public sealed class IsNotOwnerActions : NetworkBehaviour
    {
        [SerializeField] private UltEvent[] isNotOwnerSpawnActions = Array.Empty<UltEvent>();
        
        [SerializeField] private UltEvent[] isNotOwnerDespawnActions = Array.Empty<UltEvent>();
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner) return;
            
            foreach (UltEvent __action in isNotOwnerSpawnActions)
            {
                __action?.Invoke();
            }
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (IsOwner) return;
            
            foreach (UltEvent __action in isNotOwnerDespawnActions)
            {
                __action?.Invoke();
            }
        }
    }
}