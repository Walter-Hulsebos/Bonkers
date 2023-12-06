namespace Bonkers.Shared
{
    using UnityEngine;
    using JetBrains.Annotations;

    public sealed class PlayerTeam : MonoBehaviour
    {
        [field:SerializeField] public Team Team { get; [UsedImplicitly] private set; }
        
        #region Custom Operators
        
        public static implicit operator Team(PlayerTeam playerTeam) => playerTeam.Team;
        
        #endregion
    }
}