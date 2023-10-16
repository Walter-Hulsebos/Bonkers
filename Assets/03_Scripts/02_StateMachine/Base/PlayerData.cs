using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{

    [Header("Move State")]
    public float movementSpeed = 50f;
    public float runMultiplier = 8.0f;

    [Header("Jump State")]
    public float maxJumpHeight = 4.0f;
    public float maxJumpTime = 0.75f;

    [Header("Gravity")]
    public float gravity = -9.8f;
    public float groundedGravity = -.05f;
}
