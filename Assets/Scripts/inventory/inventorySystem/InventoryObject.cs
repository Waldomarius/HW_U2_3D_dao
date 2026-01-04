using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using inventory.ItemDataBase;
using inventory.items;
using UnityEngine;

namespace inventory.inventorySystem
{
    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory/New Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public ItemDataBaseObject dataBase;
        public Inventory container;
        public string savePath;
        
        public Action<bool> OnInventaryChanged;

        public bool AddItem(Item item, int amount)
        {
            InventorySlot slot = FindItemOnInventorySlot(item);
            
            if (!dataBase.items[item.id] || slot == null)
            {
                SetEmptySlot(item,  amount);
                OnInventaryChanged?.Invoke(true);
                return true;
            }
            slot.AddAmount(amount);
            OnInventaryChanged?.Invoke(true);
            return true;
        }

        private InventorySlot SetEmptySlot(Item item, int amount)
        {
            for (int i = 0; i < container.items.Length; i++)
            {
                if (container.items[i].item.id <= -1)
                {
                    container.items[i].UpdateSlot(item, amount);

                    return container.items[i];
                }
            }

            return null;
        }
        
        private InventorySlot FindItemOnInventorySlot(Item item)
        {
            for (int i = 0; i < container.items.Length; i++)
            {
                if (container.items[i].item.id == item.id)
                {
                    return container.items[i];
                }
            }
            
            return null;
        }

        /**
         * Меняем слоты местами.
         */
        public void SwapItem(InventorySlot itemIn, InventorySlot itemOut)
        {
            InventorySlot temp = new InventorySlot(itemOut.item, itemOut.amount);
            itemOut.UpdateSlot(itemIn.item, itemIn.amount);
            itemIn.UpdateSlot(temp.item, temp.amount);
        }
        
        public void RemoveItem(Item item)
        {
            for (int i = 0; i < container.items.Length; i++)
            {
                if (container.items[i].item == item)
                {
                    // Убрали айтем из контейнера
                    container.items[i].UpdateSlot(null, 0);
                }
            }
        }

        [ContextMenu("Inventory Save")]
        public void SaveInventory()
        {
            string saveData = JsonUtility.ToJson(this, true);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.absoluteURL, savePath));
            bf.Serialize(file, saveData);
            file.Close();
        }

        [ContextMenu("Inventory Load")]
        public void LoadInventory()
        {
            if (File.Exists(string.Concat(Application.absoluteURL, savePath)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(string.Concat(Application.absoluteURL, savePath), FileMode.Open);
                JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);

                file.Position = 0;
                // Inventory newContainer = JsonUtility.FromJson<Inventory>(bf.Deserialize(file).ToString());
                InventoryObject obj = (InventoryObject) bf.Deserialize(file);
                // file.Close();
                
                
                for (int i = 0; i < container.items.Length; i++)
                {
                    container.items[i].UpdateSlot(obj.container.items[i].item, obj.container.items[i].amount);
                }
                
                
                file.Close();
                
                OnInventaryChanged?.Invoke(true);
            }
        }
        
        [ContextMenu("Inventory Clear")]
        public void ClearInventory()
        {
            container = new Inventory();
        }
    }

    [Serializable]
    public class Inventory
    {
        public InventorySlot[] items = new InventorySlot[20];

        public void Clear()
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].UpdateSlot(new Item(), 0);
            }
        }
    }

    [Serializable]
    public class InventorySlot
    {
        public ItemType[] allowedTypes = new ItemType[0];
        public UserInterface parent;
        public Item item;
        public int amount;

        public ItemsObject itemsObject
        {
            get
            {
                if (item.id >= 0)
                {
                    return parent.inventory.dataBase.items[item.id];
                }
                return null;
            }
        }
        
        public InventorySlot()
        {
            item = null;
            amount = 0;
        }
        
        public InventorySlot(Item item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }

        public void UpdateSlot(Item item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
        
        public void RemoveItem()
        {
            item = new Item();
            amount = 0;
        }

        public void AddAmount(int value)
        {
            amount += value;
        }

        public bool CanPlaceInSlot(ItemsObject itemsObject)
        {
            if (allowedTypes.Length <= 0 || !itemsObject || itemsObject.data.id < 0)
            {
                return true;
            }

            for (int i = 0; i < allowedTypes.Length; i++)
            {
                if (itemsObject.itemType == allowedTypes[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}