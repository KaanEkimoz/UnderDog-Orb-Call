using com.absence.attributes;
using UnityEngine;

namespace com.game.orbsystem.statsystemextensions
{
    public class OrbStats : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField, Readonly] private OrbStatHolder m_statHolder;
    }
}
