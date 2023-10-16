namespace Bonkers
{
    using System;

    using UnityEngine;
    using Unity.Netcode;
    
    using UltEvents;

    public sealed class IsServerActions : NetworkBehaviour
    {
        [SerializeField] private UltEvent[] isServerSpawnActions = Array.Empty<UltEvent>();
        
        [SerializeField] private UltEvent[] isServerDespawnActions = Array.Empty<UltEvent>();
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsServer) return;
            
            foreach (UltEvent __action in isServerSpawnActions)
            {
                __action?.Invoke();
            }
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (!IsServer) return;
            
            foreach (UltEvent __action in isServerDespawnActions)
            {
                __action?.Invoke();
            }
        }
    }
}