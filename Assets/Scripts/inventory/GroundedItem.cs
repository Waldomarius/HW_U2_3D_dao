using System.Collections;
using System.Collections.Generic;
using inventory.items;
using UnityEditor;
using UnityEngine;

public class GroundedItem : MonoBehaviour, ISerializationCallbackReceiver
{
    public ItemsObject item;

    // Для 3D
    public GameObject prefabObject;
    
    
    // Сереализация только для 2D
    public void OnBeforeSerialize()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = item.uiDisplay;
        // для обновления компонента
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
    }

    public void OnAfterDeserialize()
    {
        
    }
}
