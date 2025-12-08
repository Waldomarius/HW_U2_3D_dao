using UnityEngine;

namespace inventory.items
{
    public enum ItemType
    {
        Food,
        Shield,
        Shirts,
        Weapons
    }
    
    public enum Attributes
    {
        Health,
        Mana,
        Stamina,
        Strenth,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom
    }
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item/New Item")]
    public class ItemsObject : ScriptableObject
    {
        public string itemName;
        [TextArea(10,10)]
        public string itemDescription;
        public Sprite uiDisplay;
        public bool isStackable;
        public ItemType itemType;
        public ItemBuff[] buffs;
        public Item data = new Item();
        
        public Item CreateItem()
        {
            Item item = new Item(this);
            return item;
        }
    }

    [System.Serializable]
    public class Item
    {
        public int id = -1;
        public string name;
        public ItemBuff[] buffs;
        public bool isStackable;

        public Item()
        {
            name = ""; 
        }

        public Item(ItemsObject itemsObject)
        {
            name = itemsObject.itemName;
            id = itemsObject.data.id;
            buffs = new ItemBuff[itemsObject.buffs.Length];

            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemBuff(itemsObject.buffs[i].minValue, itemsObject.buffs[i].maxValue)
                {
                    attributes = itemsObject.buffs[i].attributes
                };
            }
        }
    }

    [System.Serializable]
    public class ItemBuff
    {
        public Attributes attributes;
        public int value;
        public int minValue;
        public int maxValue;
        
        public ItemBuff(int min, int max)
        {
            minValue = min;
            maxValue = max;
            GenerateValue();
        }

        private void GenerateValue()
        {
            value =  Random.Range(minValue, maxValue);
        }
    }
}
