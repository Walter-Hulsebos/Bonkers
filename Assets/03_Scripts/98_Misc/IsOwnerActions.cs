namespace Bonkers
{
    using System;

    using UnityEngine;
    using Unity.Netcode;
    
    using UltEvents;

    public sealed class IsOwnerActions : NetworkBehaviour
    {
        [SerializeField] private UltEvent isOwnerSpawnActions   = new ();
        
        [SerializeField] private UltEvent isOwnerDespawnActions = new ();
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner) return;
            
            isOwnerSpawnActions?.Invoke();
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (!IsOwner) return;
            
            isOwnerDespawnActions?.Invoke();
        }
    }
}