using System;
using System.Collections.Generic;

using CGTK.Utils.Extensions.Math.Math;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

using F32 = System.Single;
using F32x3 = Unity.Mathematics.float3;

using Bool = System.Boolean;
using Cysharp.Threading.Tasks;


public sealed class G_UltimateState : PlayerBaseState
    {
    #region Enums
    private InUltimate inUltimate;
    #endregion

    #region Constructor
    public G_UltimateState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    #endregion

    #region Variables

    [SerializeField] private Vector3 offset = new(x: 0, y: 0, z: 1.5f);
    [SerializeField] private F32 radius = 1.5f;

    #endregion

    private enum InUltimate
    {
        None,
        Ultimate
    }

    public override void EnterState()
    {
        //Ctx.KnockBackPlane.SetActive(true);
        Ctx.Anims.SetTrigger(Ctx.UltimateHash);
        HandleGabrielUltimate();

        Collider[] __colliders = Physics.OverlapSphere(position: Ctx.transform.position + offset, radius: radius, layerMask: LayerMask.NameToLayer("Player"));

        Bool __hitOtherPlayers = false;
        HashSet<Collider> __otherPlayerColliders = new();
        foreach (Collider __collider in __colliders)
        {
            if (__collider.gameObject != Ctx.gameObject)
            {
                __hitOtherPlayers = true;
                __otherPlayerColliders.Add(__collider);
            }
        }

        if (__hitOtherPlayers)
        {
            foreach (Collider __collider in __otherPlayerColliders)
            {
                PlayerStateMachine __playerStateMachine = __collider.gameObject.GetComponent<PlayerStateMachine>();

                //TODO: Send packet of knockbackinfo to other players
            }
        }
    }

    public override void ExitState()
    {

        //Ctx.KnockBackPlane.SetActive(false);
    }

    void HandleGabrielUltimate()
    {
        inUltimate = InUltimate.Ultimate;
        //put in your basic attack here
    }

    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = Vector3.zero;

        //if(Ctx.Anims.GetCurrentAnimatorClipInfo(layerIndex: 0))
        AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);

        //Debug.Log($"Expected Hash = {Ctx.BasicAttackHash}, tag hash {__animStateInfo.tagHash}, short hash {__animStateInfo.shortNameHash}, long hash {__animStateInfo.fullPathHash}");
        // if (__animStateInfo.tagHash == Ctx.Special1Hash)
        //{
        F32 animPercentage = __animStateInfo.normalizedTime;

        if (animPercentage > 1.0f)
        {

        }
        //}
    }

    #endregion
}
