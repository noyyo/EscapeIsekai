using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private int _quickSlotCount = 4;
    [SerializeField] private GameObject _quickSlotPrefab;
    [SerializeField] private GameObject _quickSlotSpawn;

    private UI_Manager _ui_Manager;

    //QuickSlot�� �����ϱ� ���� ����Ʈ
    private List<QuickSlot> _quickSlots = new List<QuickSlot>();
    
    //�κ��丮�� ����â�� ������ ������� ���ϰ� �ϱ����� bool
    private bool isAvailable = true;

    //��ų �����ϴ� �̺�Ʈ
    public event Action<int> OnSkillEvent;

    private void Awake()
    {
        _ui_Manager = UI_Manager.Instance;

        if (_quickSlotPrefab == null)
            _quickSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/SpecialAbilities/QuickSlot");

        //���߿� ��ġ �������� ������ ��.
        if (_quickSlotSpawn == null)
            _quickSlotSpawn = _ui_Manager.quickSlot_UI.transform.GetChild(0).gameObject;

        CreateSlot();
    }

    private void Start()
    {
        _ui_Manager.TurnOnQuickSlotEvent += TurnOnQucikSlotUI;
        _ui_Manager.TurnOffQuickSlotEvent += TurnOffQucikSlotUI;
    }

    private void CreateSlot()
    {
        for (int i = 0; i < _quickSlotCount; i++)
        {
            GameObject obj = Instantiate(_quickSlotPrefab);
            obj.transform.SetParent(_quickSlotSpawn.transform, false);
            obj.GetComponent<QuickSlot>().UniqueIndex = i;
            _quickSlots.Add(obj.GetComponent<QuickSlot>());
        }
    }

    public void OnSkill(int skillIndex)
    {
        if(isAvailable) 
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
