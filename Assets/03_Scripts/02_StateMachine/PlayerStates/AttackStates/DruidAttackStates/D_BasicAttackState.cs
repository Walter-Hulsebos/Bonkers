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
using Unity.VisualScripting;

[Serializable]
public sealed class D_BasicAttackState : PlayerBaseState
    {
    #region Enums
    private InBasicAttack inBasic;
    private bool _isPlayerenemyInTrigger = false;
    private GameObject _enemyCharacter;
    #endregion

    #region Constructor
    public D_BasicAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        currentContext.OnTriggerEnterEvent += OnTriggerEnter;
        currentContext.OnTriggerStayEvent += OnTriggerStay;
        currentContext.OnTriggerExitEvent += OnTriggerExit;
    }

    ~D_BasicAttackState()
    {
        Ctx.OnTriggerEnterEvent -= OnTriggerEnter;
        Ctx.OnTriggerStayEvent -= OnTriggerStay;
        Ctx.OnTriggerExitEvent -= OnTriggerExit;
    }
    #endregion 

    private enum InBasicAttack
    {
        None,
        Basic
    }

    public override void EnterState()
    {
        Ctx.Anims.SetTrigger(Ctx.BasicAttackHash);
        HandleDruidBasicAttack();
    }

    public override void ExitState(){}

    void HandleDruidBasicAttack()
    {
        inBasic = InBasicAttack.Basic;
        if (_isPlayerenemyInTrigger)
        {
            
        }
    }

    void OnTriggerEnter(Collider collider)
    {

    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            _isPlayerenemyInTrigger = true;
            _enemyCharacter = collider.gameObject;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            _isPlayerenemyInTrigger = false;
            _enemyCharacter = null;
        }
    }

    #region Updates

    protected override void UpdateRotation(ref Quaternion currentRotation, float deltaTime){}

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