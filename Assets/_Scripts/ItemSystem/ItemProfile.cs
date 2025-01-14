using com.absence.attributes;
using com.game.utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace com.game.itemsystem.scriptables
{
    /// <summary>
    /// The abstract scriptable object used for holding built-in data of an item.
    /// </summary>
    public abstract class ItemProfile : ScriptableObject
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
            Debug.Log(GenerateFullDescription(true));
        }

        public string GenerateFullDescription(bool richText)
        {
            string trimmedRawDesc = Description.Trim();

            StringBuilder sb = new(trimmedRawDesc);
            if (!string.IsNullOrWhiteSpace(trimmedRawDesc)) sb.Append("\n\n");

            sb.Append(GenerateCustomActionDescription(richText));
            sb.Append(GenerateStatDescription(richText));

            return sb.ToString();
        }

        public string GenerateCustomActionDescription(bool richText)
        {
            StringBuilder sb = new();

            CustomActions.ForEach(act =>
            {
                sb.Append(ItemSystemHelpers.Text.GenerateActionDescription(act, richText));
                sb.Append("\n");
            });

            return sb.ToString();
        }

        public abstract string GenerateStatDescription(bool richText);
    }
}
