namespace Bonkers.Lobby
{
    using System;

    using Unity.Cinemachine;

    using UnityEngine;

    public class CameraInitializer : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain  cinemachineBrain;
        [SerializeField] private CinemachineCamera cinemachineCamera;

        private void Reset()
        {
            cinemachineBrain  = GetComponentInChildren<CinemachineBrain>();
            cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        }

        private void Start()
        {
            // Shift one bit per player count.
            cinemachineBrain.ChannelMask    = (OutputChannels)(1 << PlayerCounter.playerCount);
            cinemachineCamera.OutputChannel = (OutputChannels)(1 << PlayerCounter.playerCount);
        }
    }
}