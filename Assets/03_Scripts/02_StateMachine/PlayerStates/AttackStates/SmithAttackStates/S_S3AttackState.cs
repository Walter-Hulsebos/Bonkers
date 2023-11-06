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
using Cysharp.Threading.Tasks;

[Serializable]
public sealed class S_S3AttackState : PlayerBaseState
    {
    #region Enums
    private InSpecial3 inSpecial3;
    #endregion

    #region Constructor
    public S_S3AttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    #endregion
    private enum InSpecial3
    {
        None,
        Special3
    }
    public override void EnterState()
    {
        Ctx.Anims.SetTrigger(Ctx.Special3Hash);
        HandleSmithSpecial3();
    }

    public override void ExitState(){}

    void HandleSmithSpecial3()
    {
        inSpecial3 = InSpecial3.Special3;
        // do your special here 
    }

    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        currentVelocity = Vector3.zero;

        //if(Ctx.Anims.GetCurrentAnimatorClipInfo(layerIndex: 0))
        AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);

        Debug.Log($"Expected Hash = {Ctx.Special3Hash}, tag hash {__animStateInfo.tagHash}, short hash {__animStateInfo.shortNameHash}, long hash {__animStateInfo.fullPathHash}");
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
