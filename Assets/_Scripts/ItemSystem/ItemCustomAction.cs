using com.absence.attributes;
using UnityEngine;

namespace com.game.itemsystem
{
    [System.Serializable]
    public class ItemCustomAction
    {
        public ItemActionType ActionType = ItemActionType.None;

        [HideIf(nameof(ActionType), ItemActionType.None)]
        public string Description;

        [ShowIf(nameof(ActionType), ItemActionType.SpawnItemBehaviour)]
        public GameObject ItemBehaviour;
    }
}
