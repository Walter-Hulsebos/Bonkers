using UnityEngine;

namespace Bonkers.Lobby
{
    using System;

    [CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
    public class Character : ScriptableObject
    {
        [SerializeField] private Int32  id          = -1;
        [SerializeField] private String displayName = "New Display Name";
        [SerializeField] private Sprite icon;

        public Int32  Id          => id;
        public String DisplayName => displayName;
        public Sprite Icon        => icon;
    }
}