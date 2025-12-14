using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace inventory.inventorySystem
{
    public class DynamicInterface : UserInterface
    {
        [SerializeField] private GameObject inventoryPrefab;
        [SerializeField] private int X_START;
        [SerializeField] private int Y_START;
        [SerializeField] private int X_SPASE_BETWEEN_ITEMS;
        [SerializeField] private int Y_SPASE_BETWEEN_ITEMS;
        [SerializeField] private int NUMBER_OF_COLUMN;

        public override void CreateSlot()
        {
            slotOnInteface = new Dictionary<GameObject, InventorySlot>();

            for (int i = 0; i < inventory.container.items.Length; i++)
            {
                GameObject obj = Instantiate(inventoryPrefab,  Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<RectTransform>().localPosition = GetPosition(i);
                
                // На каждый слот вешаем свой конкретный слушатель и свой конкретный триггер
                AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj);});
                AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj);});
                AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj);});
                AddEvent(obj, EventTriggerType.Drag, delegate { OnDragged(obj);});
                AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj);});
                
                slotOnInteface.Add(obj, inventory.container.items[i]);
            }
        }
        
        private Vector3 GetPosition(int i)
        {
            return new Vector3(
                X_START + (X_SPASE_BETWEEN_ITEMS * (i % NUMBER_OF_COLUMN)),
                Y_START - (Y_SPASE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)),
                0
            );
        }
        
        
    }
}