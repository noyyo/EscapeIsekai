using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState
{
    Field, // �ʵ忡 �ִ� ����
    InInventory, // �κ��丮 �ȿ� �ִ� ����
    Equipped, // ����� ����
    Abandoned // ������ ����
}

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData _itemSO;
    public ItemData ItemSO { get { return _itemSO; } }


    [Header("������ �׽�Ʈ�� ���� ���� ����")]
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
