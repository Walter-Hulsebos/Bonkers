namespace Bonkers
{
    using System;

    using UnityEngine;
    using Unity.Netcode;
    
    using UltEvents;

    public sealed class IsOwnerActions : NetworkBehaviour
    {
        [SerializeField] private UltEvent[] isOwnerSpawnActions = Array.Empty<UltEvent>();
        
        [SerializeField] private UltEvent[] isOwnerDespawnActions = Array.Empty<UltEvent>();
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner) return;
            
            foreach (UltEvent __action in isOwnerSpawnActions)
            {
                __action?.Invoke();
            }
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (!IsOwner) return;
            
            foreach (UltEvent __action in isOwnerDespawnActions)
            {
                __action?.Invoke();
            }
        }
    }
}