namespace Bonkers
{
    using System;

    using UnityEngine;
    using Unity.Netcode;
    
    using UltEvents;

    public sealed class IsClientActions : NetworkBehaviour
    {
        [SerializeField] private UltEvent[] isClientSpawnActions = Array.Empty<UltEvent>();
        
        [SerializeField] private UltEvent[] isClientDespawnActions = Array.Empty<UltEvent>();
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsClient) return;
            
            foreach (UltEvent __action in isClientSpawnActions)
            {
                __action?.Invoke();
            }
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (!IsClient) return;
            
            foreach (UltEvent __action in isClientDespawnActions)
            {
                __action?.Invoke();
            }
        }
    }
}
