namespace Bonkers
{
    using UnityEngine;
    using Unity.Netcode.Components;
    
    using Bool = System.Boolean;
    
    [DisallowMultipleComponent]
    [AddComponentMenu("Netcode/Network Animator (Client Authoritative)")]
    public sealed class NetworkAnimatorClientAuthoritative : NetworkAnimator
    {
        /// <summary>
        /// Used to determine who can write to this transform. Owner client only.
        /// This imposes state to the server. This is putting trust on your clients. Make sure no security-sensitive features use this transform.
        /// </summary>
        protected override Bool OnIsServerAuthoritative() => false;

        #if UNITY_EDITOR
        private void Reset()
        {
            this.Animator = GetComponent<Animator>();    
        }
        #endif
    }
}