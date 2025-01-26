using com.absence.attributes;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.itemsystem.scriptables
{
    /// <summary>
    /// The abstract scriptable object used for holding built-in data of an item.
    /// </summary>
    public abstract class ItemProfileBase : ScriptableObject
    {
        [Header1("Item Profile")]

        [Readonly]
        public string Guid = System.Guid.NewGuid().ToString();

        [Space, Header3("Information")]

        [SpriteField, Tooltip("Icon of this item that will be displayed in the UI.")]
        public Sprite Icon;

        [Tooltip("Name of this item that will be displayed in the UI.")] 
        public string DisplayName;

        [Multiline, Tooltip("Description of this item. It <b>MUST NOT</b> contain the stat modification log of the item.")] 
        public string Description;

        [Space, Header3("Custom")]
        public List<ItemCustomAction> CustomActions = new();

        [Button("Generate Description")]
        protected void PrintFullDescription()
        {
            Debug.Log(ItemSystemHelpers.Text.GenerateDescription(this, true));
        }

        public abstract string GenerateFurtherDescription(ItemObject context, bool richText);
    }
}
