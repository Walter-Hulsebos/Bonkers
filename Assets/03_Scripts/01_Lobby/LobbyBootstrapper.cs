using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Lobby
{
    public sealed class LobbyBootstrapper : MonoBehaviour
    {
        public void Start()
        {
            GameLobby.InitializeUnityAuthentication();
        }
    }
}
