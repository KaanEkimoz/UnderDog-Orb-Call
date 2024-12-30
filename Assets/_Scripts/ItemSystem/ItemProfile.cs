using com.absence.attributes;
using com.game.itemsystem.ui;
using com.game.utilities;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.itemsystem.scriptables
{
    public abstract class ItemProfile : ScriptableObject
    {
        [Header1("Item Profile")]

        [Space, Header3("Information")]

        [SerializeField, SpriteField, Tooltip("Icon of this item that will be displayed in the UI.")] 
        protected Sprite m_icon;

        [SerializeField, Tooltip("Name of this item that will be displayed in the UI.")] 
        protected string m_displayName;

        [SerializeField, Multiline, Tooltip("Description of this item. It <b>MUST NOT</b> contain the stat modification log of the item.")] 
        protected string m_description;

        [Space, Header3("Utilities")]

        public List<ItemCustomAction> CustomActions = new();

        public ItemUIData GenerateUIData()
        {
            return new ItemUIData()
            {
                Icon = m_icon,
                DisplayName = m_displayName,
                Description = m_description,
            };
        }
    }
}
