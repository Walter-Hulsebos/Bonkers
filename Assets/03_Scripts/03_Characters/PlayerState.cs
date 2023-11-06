namespace Bonkers.Characters
{
    using System;

    using KinematicCharacterController;

    using UnityEngine;
    
    using F32  = System.Single;
    using Bool = System.Boolean;

    [Serializable]
    public abstract class PlayerState : MonoBehaviour, IPlayerState
    {
        public abstract void OnEnter();
        public abstract void OnExit();
        
        public abstract void UpdateRotation(ref Quaternion currentRotation, F32 deltaTime);
        public abstract void UpdateVelocity(ref Vector3    currentVelocity, F32 deltaTime);

        public virtual void BeforeCharacterUpdate(F32 deltaTime) { }
        public virtual void PostGroundingUpdate(F32 deltaTime) { }
        public virtual void AfterCharacterUpdate(F32 deltaTime) { }

        public virtual Bool IsColliderValidForCollisions(Collider coll) => true;
        public virtual void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
        public virtual void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

        public virtual void ProcessHitStabilityReport
        (
            Collider               hitCollider,
            Vector3                hitNormal,
            Vector3                hitPoint,
            Vector3                atCharacterPosition,
            Quaternion             atCharacterRotation,
            ref HitStabilityReport hitStabilityReport
        ) { }

        public virtual void OnDiscreteCollisionDetected(Collider hitCollider) { }
    }
}