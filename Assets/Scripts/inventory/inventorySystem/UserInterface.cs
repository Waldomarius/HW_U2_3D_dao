using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace inventory.inventorySystem
{
    public abstract class UserInterface : MonoBehaviour
    {
        public InventoryObject inventory;
        public Dictionary<GameObject, InventorySlot> slotOnInteface = new Dictionary<GameObject, InventorySlot>();

        [SerializeField] private int dragItemSizeX = 80;
        [SerializeField] private int dragItemSizeY = 80;
        
        private void OnEnable()
        {
            inventory.OnInventaryChanged += UpdateSlotUI;
        }

        public abstract void CreateSlot();

        private void Start()
        {
            for (int i = 0; i < inventory.container.items.Length; i++)
            {
                inventory.container.items[i].parent = this;
            }
            
            CreateSlot();
            slotOnInteface.UpdateSlotUI();
        }

        // private void Update()
        // {
        //     slotOnInteface.UpdateSlotUI();
        // }

        protected void OnEnter(GameObject obj)
        {
            MouseData.SlotHoveredOver = obj;
        }
        
        protected void OnExit(GameObject obj)
        {
            MouseData.SlotHoveredOver = null;
        }
        
        protected void OnDragStart(GameObject obj)
        {
            MouseData.TempItemBeginDragged = CreateTempItem(obj);
        }
        
        protected void OnDragged(GameObject obj)
        {
            if (MouseData.TempItemBeginDragged)
            {
                MouseData.TempItemBeginDragged.GetComponent<RectTransform>().position = Input.mousePosition;
            }
        }
        
        protected void OnDragEnd(GameObject obj)
        {
            Destroy(MouseData.TempItemBeginDragged);
            if (MouseData.SlotHoveredOver)
            {
                InventorySlot mouseHoverSlotData = slotOnInteface[MouseData.SlotHoveredOver];
                inventory.SwapItem(slotOnInteface[obj], mouseHoverSlotData);
                // Обновим слоты в меню
                slotOnInteface.UpdateSlotUI();
            }
        }
        
        private GameObject CreateTempItem(GameObject obj)
        {
            GameObject tempItem = null;

            if (slotOnInteface[obj].item.id >= 0)
            {
                tempItem = new GameObject();
                // RectTransform для 2D объекта, Transform для 3D объекта
                var rt =  tempItem.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(dragItemSizeX, dragItemSizeY);
                tempItem.transform.SetParent(transform.parent);
                var image = tempItem.AddComponent<Image>();
                image.sprite = slotOnInteface[obj].itemsObject.uiDisplay;
                image.raycastTarget = false;
            }

            return tempItem;
        }

        /**
         * Метод ивентов для обработки событий под мышкой.
         */
        protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            var eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }
        
        private void UpdateSlotUI(bool isUpdate)
        {
            slotOnInteface.UpdateSlotUI();
        }
        
        private void OnDisable()
        {
            inventory.OnInventaryChanged -= UpdateSlotUI;
        }
    }

    /**
     * Отдельный класс, что бы понимать что у нас под мышкой при перетаскивании.
     */
    public static class MouseData
    {
        public static GameObject TempItemBeginDragged;
        public static GameObject SlotHoveredOver;
    }

    public static class ExtensionMethod
    {
        public static void UpdateSlotUI(this Dictionary<GameObject, InventorySlot> slotOnInteface)
        {
            foreach (KeyValuePair<GameObject, InventorySlot> slot in slotOnInteface)
            {
                var iconImage = slot.Key.transform.GetChild(0).GetComponentInChildren<Image>();
                var textComponent = slot.Key.GetComponentInChildren<TextMeshProUGUI>();
                
                if (slot.Value.item.id >= 0)
                {
                    iconImage.sprite = slot.Value.itemsObject.uiDisplay;
                    iconImage.color = new Color(1, 1, 1, 1);
                    textComponent.text = slot.Value.amount == 1 ? "" : slot.Value.amount.ToString("N0");
                }
                else
                {
                    iconImage.sprite = null;
                    iconImage.color = new Color(1, 1, 1, 0);
                    textComponent.text = "";
                }
            }
        }
    }
}