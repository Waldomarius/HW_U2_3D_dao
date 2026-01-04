using System;
using inventory.inventorySystem;
using inventory.items;
using UnityEngine;

namespace inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private InventoryObject _inventoryObject;

        private void OnTriggerEnter(Collider other)
        {
            var groundItem = other.GetComponent<GroundedItem>();

            if (groundItem != null)
            {
                Item item = new Item(groundItem.item);
                
                Debug.Log($"GroundedItem id : {item.id}");
                Debug.Log($"GroundedItem name : {item.name}");

                // добавляем в инвентарь и убираем с улицы
                if (_inventoryObject.AddItem(item, 1))
                {
                    Destroy(other.gameObject);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                _inventoryObject.SaveInventory();
            }
            
            if (Input.GetKeyDown(KeyCode.F6))
            {
                _inventoryObject.LoadInventory();
            }
        }

        private void OnApplicationQuit()
        {
            _inventoryObject.container.Clear();
        }
    }
}
