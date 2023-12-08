namespace Bonkers
{
    using UnityEngine;
    using Unity.Netcode.Components;
    
    using Bool = System.Boolean;
    
    [DisallowMultipleComponent]
    [AddComponentMenu("Netcode/Network Transform (Client Authoritative)")]
    public sealed class NetworkTransformClientAuthoritative : NetworkTransform
    {
      
        /// <summary>
        /// Used to determine who can write to this transform. Owner client only.
        /// This imposes state to the server. This is putting trust on your clients. Make sure no security-sensitive features use this transform.
        /// </summary>
        protected override Bool OnIsServerAuthoritative() => false;
    }
}
