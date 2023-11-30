using UnityEngine;

namespace Bonkers.Shared
{
    public sealed class PlayerTeam : MonoBehaviour
    {
        [SerializeField] private Team _team;
        
        #region Custom Operators
        
        public static implicit operator Team(PlayerTeam playerTeam) => playerTeam._team;
        
        #endregion
    }
}