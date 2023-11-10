using System;

using CGTK.Utils.Extensions.Math.Math;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;


using F32 = System.Single;
using F32x3 = Unity.Mathematics.float3;
using Cysharp.Threading.Tasks;

public sealed class S_BasicAttackState : PlayerBaseState
{
    #region Enums
    private InBasicAttack inBasic;
    #endregion

    #region Constructor
    public S_BasicAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    #endregion 
    private enum InBasicAttack
    {
        None,
        Basic
    }
    public override void EnterState()
    {
        Ctx.Anims.SetTrigger(Ctx.BasicAttackHash);
        HandleSmithBasicAttack();
    }

    public override void ExitState(){}

    void HandleSmithBasicAttack()
    {
        inBasic = InBasicAttack.Basic;
        //put in your basic attack here
    }

    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime){}

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = Vector3.zero;

        //if(Ctx.Anims.GetCurrentAnimatorClipInfo(layerIndex: 0))
        AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);

        Debug.Log($"Expected Hash = {Ctx.BasicAttackHash}, tag hash {__animStateInfo.tagHash}, short hash {__animStateInfo.shortNameHash}, long hash {__animStateInfo.fullPathHash}");
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

