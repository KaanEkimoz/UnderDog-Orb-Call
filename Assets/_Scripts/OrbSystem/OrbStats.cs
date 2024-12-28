using com.absence.attributes;
using UnityEngine;

namespace com.game.orbsystem
{
    public class OrbStats : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField, Readonly] private OrbStatHolder m_statHolder;
    }
}
