using com.absence.attributes;
using com.game.itemsystem.gamedependent;
using UnityEngine;

namespace com.game.itemsystem
{
    /// <summary>
    /// The reference type used for holding the data of a custom action held by an item.
    /// </summary>
    [System.Serializable]
    public class ItemCustomAction
    {
        public ItemActionType ActionType = ItemActionType.None;

        [HideIf(nameof(ActionType), ItemActionType.None)]
        public string Description;

        [ShowIf(nameof(ActionType), ItemActionType.SpawnItemBehaviour)]
        public ItemBehaviour ItemBehaviour;
    }
}
