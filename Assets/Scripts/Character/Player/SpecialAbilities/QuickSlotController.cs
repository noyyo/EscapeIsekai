using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private int _quickSlotCount = 4;
    [SerializeField] private GameObject _quickSlotPrefab;

    private UI_Manager _ui_Manager;
    private ItemCraftingManager _itemCraftingManager;

    //QuickSlot�� �����ϱ� ���� ����Ʈ
    private List<QuickSlot> _quickSlots = new List<QuickSlot>();

    //��ų �����ϴ� �̺�Ʈ
    public event Action<int> OnSkillEvent;

    private void Awake()
    {
        _ui_Manager = UI_Manager.Instance;
        _itemCraftingManager = ItemCraftingManager.Instance;
        if (_quickSlotPrefab == null)
            _quickSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/SpecialAbilities/QuickSlot");
    }

    private void Start()
    {
        _ui_Manager.UI_QuickSlotTurnOnEvent += TurnOnQucikSlotUI;
        _ui_Manager.UI_QuickSlotTurnOffEvent += TurnOffQucikSlotUI;
        CreateSlot();
    }

    private void CreateSlot()
    {
        for (int i = 0; i < _quickSlotCount; i++)
        {
            QuickSlot obj = Instantiate(_quickSlotPrefab, this.transform).GetComponent<QuickSlot>();
            obj.UniqueIndex = i;
            _quickSlots.Add(obj);
        }
    }

    public void OnSkill(int skillIndex)
    {
        OnSkillEvent?.Invoke(skillIndex);
    }

    public void TurnOnQucikSlotUI()
    {
        this.gameObject.SetActive(true);
    }

    public void TurnOffQucikSlotUI()
    {
        this.gameObject.SetActive(false);
    }
}
