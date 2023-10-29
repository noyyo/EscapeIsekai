using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    Field, // 필드에 있는 상태
    InInventory, // 인벤토리 안에 있는 상태
    Equipped, // 장비한 상태
    Abandoned // 버려진 상태
}

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData _itemSO;
    public ItemData ItemSO { get { return _itemSO; } }


    [Header("테스트용")]
    [SerializeField] private ItemState _itemState;
    public ItemState ItemState { get { return _itemState; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
