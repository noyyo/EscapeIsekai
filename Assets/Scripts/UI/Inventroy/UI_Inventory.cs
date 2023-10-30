using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _slotSpawn;
    
    private GameObject _slotPrefab;
    private Inventory _playerInventory;

    public Slot[] slotArray;

    private void Awake()
    {
        Init();
        CreateSlot();
    }

    private void Init()
    {
        _slotPrefab = Resources.Load<GameObject>("Prefabs/UI/Inventory/Slot");
        _playerInventory = GetComponent<Inventory>();
        slotArray = new Slot[_playerInventory.dataLength];
    }


    private void CreateSlot()
    {
        int slotCount = _playerInventory.dataLength;
        for (int i = 0; i < slotCount; i++)
        {
            GameObject obj = Instantiate(_slotPrefab);
            obj.transform.SetParent(_slotSpawn.transform, false);
            slotArray[i] = obj.GetComponent<Slot>();
        }
    }

    /// <summary>
    /// 나중에 event랑 연결할 것
    /// </summary>
    public void OpenInventroyUI()
    {
        //_ui_inventroy.SetActive(true);
    }
}
