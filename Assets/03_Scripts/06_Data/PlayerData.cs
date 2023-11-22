using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newPlayerData", menuName ="Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float PlayerMaxSpeed = 10f;
    public float MoveSharpness = 15f;
    public float OrientSharpness = 20f;

    [Header("Jump State")]
    //public float jumpVelocity = 15f;
   //public int amountOfJumps = 1;

    [Header("In Air State")]
    //public float coyoteTime = 0.02f;
   // public float variableJumpHeightMultiplier = 0.5f;

    [Header("Druid Rat Combat")]
    public GameObject ratPrefab;

    [Header("Druid Acorn Combat")]
    public GameObject acornPrefab;

    [Header("Druid Audio")]
    public AudioClip druidAudioRats;

}
