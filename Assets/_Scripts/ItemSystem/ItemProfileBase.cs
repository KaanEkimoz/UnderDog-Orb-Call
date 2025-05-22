using com.absence.attributes;
using com.absence.attributes.experimental;
using com.absence.utilities.experimental.databases;
using com.game.itemsystem.gamedependent;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.itemsystem.scriptables
{
    /// <summary>
    /// The abstract scriptable object used for holding built-in data of an item.
    /// </summary>
    public abstract class ItemProfileBase : ScriptableObject, IDatabaseMember<string>
    {
        [Header1("Item Profile")]

        [Readonly]
        public string Guid = System.Guid.NewGuid().ToString();
        public string CustomItemId = string.Empty;
        public ItemRarity Rarity = ItemRarity.Common; 

        [Space, Header3("Information")]

        [SpriteField, Tooltip("Icon of this item that will be displayed in the UI.")]
        public Sprite Icon;

        [Tooltip("Name of this item that will be displayed in the UI.")] 
        public string DisplayName;

        [Multiline, Tooltip("Description of this item. It <b>MUST NOT</b> contain the stat modification log of the item.")] 
        public string Description;

        [Space, Header3("Actions and Behaviour")]
        public List<ItemCustomAction> CustomActions = new();
        [InlineEditor] public ItemBehaviour Behaviour;

        [Button("Generate Description")]
        protected void PrintFullDescription()
        {
            Debug.Log(ItemSystemHelpers.Text.GenerateDescription(this, true));
        }

        public virtual string TypeName => null;
        public abstract string GenerateFurtherDescription(ItemObject context, bool richText);

        public string GetDatabaseKey()
        {
            return Guid;
        }
    }
}
