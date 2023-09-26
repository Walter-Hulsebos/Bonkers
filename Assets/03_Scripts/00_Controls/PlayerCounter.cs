using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using I32 = System.Int32;

namespace Bonkers.Controls
{
    using System;

    using JetBrains.Annotations;

    public class PlayerCounter : MonoBehaviour
    {
        public static I32 PlayerCount;

        [PublicAPI]
        public void PlayerJoined()
        {
            PlayerCount++;
        }

        [PublicAPI]
        public void PlayerLeft()
        {
            PlayerCount--;
        }
    }
}
