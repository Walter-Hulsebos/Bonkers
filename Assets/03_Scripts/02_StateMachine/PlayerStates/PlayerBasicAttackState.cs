using System;

//using Bonkers.Characters;

using CGTK.Utils.Extensions.Math.Math;

using Sirenix.OdinInspector;

using UnityEngine;
using static UnityEngine.Mathf;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

//using static Bonkers.Characters.OrientationMethod;

using F32 = System.Single;
using F32x3 = Unity.Mathematics.float3;

[Serializable]
public sealed class PlayerBasicAttackState : PlayerBaseState
{
    #region Variables
    private InBasicAttack inBasicAttack;
    #endregion

    #region Constructor
    public PlayerBasicAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory){}
    #endregion
    private enum InBasicAttack
    {
        none,
        BasicAttack
    }

    public override void EnterState()
    {
        Ctx.Anims.SetTrigger(Ctx.LightAttackHash);
        HandleLightAttack();
    }

    public override void ExitState(){}

    void HandleLightAttack()
    {
        inBasicAttack = InBasicAttack.BasicAttack;
    }


    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        throw new NotImplementedException();
    }

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = Vector3.zero;

        //if(Ctx.Anims.GetCurrentAnimatorClipInfo(layerIndex: 0))
        AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);

        Debug.Log($"Expected Hash = {Ctx.LightAttackHash}, tag hash {__animStateInfo.tagHash}, short hash {__animStateInfo.shortNameHash}, long hash {__animStateInfo.fullPathHash}");
        // if (__animStateInfo.tagHash == Ctx.Special1Hash)
        //{
        F32 animPercentage = __animStateInfo.normalizedTime;

        if (animPercentage > 1.0f)
        {

        }
    }
    #endregion
}
