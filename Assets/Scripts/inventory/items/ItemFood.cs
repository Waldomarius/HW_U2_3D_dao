using UnityEngine;

namespace inventory.items
{
    [CreateAssetMenu(fileName = "ItemFood", menuName = "Inventory/Item/ItemFood")]
    public class ItemFood : ItemsObject
    {
        private void Awake()
        {
            itemType = ItemType.Food;
        }
        
        
    }
}