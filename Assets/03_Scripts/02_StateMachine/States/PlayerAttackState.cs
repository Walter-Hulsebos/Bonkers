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
    public sealed class PlayerAttackState : PlayerBaseState
    {

    #region Variables
    private InSpecial inSpecial;
    #endregion

    #region Constructor
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    #endregion
    private enum InSpecial
    {
        None,
        Special
    }
    public override void EnterState()
    {
        Ctx.Anims.SetTrigger(Ctx.Special1Hash);
        HandleSpecial1();
    }

    public override void ExitState()
    {
    }

    void HandleSpecial1()
    {
        inSpecial = InSpecial.Special;
        SpawnRats();
    }
    async void SpawnRats()
    {
        GameObject.Instantiate(Ctx.Data.ratPrefab, Ctx.transform.position + (Ctx.transform.forward * 2), Quaternion.identity);
        await UniTask.WaitForEndOfFrame(Ctx);
        GameObject.Instantiate(Ctx.Data.ratPrefab, Ctx.transform.position + (Ctx.transform.forward * 2) + (Ctx.transform.right * 1), Quaternion.identity);
        await UniTask.WaitForEndOfFrame(Ctx);
        GameObject.Instantiate(Ctx.Data.ratPrefab, Ctx.transform.position + (Ctx.transform.forward * 2) + (Ctx.transform.right * -2), Quaternion.identity);
        await UniTask.WaitForEndOfFrame(Ctx);
        GameObject.Instantiate(Ctx.Data.ratPrefab, Ctx.transform.position + (Ctx.transform.forward * 1) + (Ctx.transform.right * 0.5f), Quaternion.identity);
        await UniTask.WaitForEndOfFrame(Ctx);
        GameObject.Instantiate(Ctx.Data.ratPrefab, Ctx.transform.position + (Ctx.transform.forward * 1) + (Ctx.transform.right * -0.5f), Quaternion.identity);
    }

    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
        }

    protected override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
        currentVelocity = Vector3.zero;

        //if(Ctx.Anims.GetCurrentAnimatorClipInfo(layerIndex: 0))
        AnimatorStateInfo __animStateInfo = Ctx.Anims.GetCurrentAnimatorStateInfo(layerIndex: 0);

        Debug.Log($"Expected Hash = {Ctx.Special1Hash}, tag hash {__animStateInfo.tagHash}, short hash {__animStateInfo.shortNameHash}, long hash {__animStateInfo.fullPathHash}");
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

