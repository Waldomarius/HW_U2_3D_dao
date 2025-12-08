using inventory.items;
using UnityEngine;

namespace inventory.ItemDataBase
{
    [CreateAssetMenu(fileName = "Item Database", menuName = "Inventory/Item/Item Database")]
    public class ItemDataBaseObject : ScriptableObject, ISerializationCallbackReceiver
    {
        public ItemsObject[] items;
        


        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            UpdateId();
        }
        
        public void UpdateId()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items != null && items[i].data.id != i)
                {
                    items[i].data.id = i;
                }
            }
        }
    }
}