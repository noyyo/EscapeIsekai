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
    [SerializeField] private ItemSO _itemSO;
    public ItemSO ItemSO { get { return _itemSO; } }


    [Header("아이템 테스트를 위한 상태 구별")]
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
