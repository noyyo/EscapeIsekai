using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    [SerializeField] private GameObject _iconObject;
    [SerializeField] private GameObject _numberObject;
    [SerializeField] private Button _button;

    private Image _icon;
    private Image _number;

    //������ ��ġ �ѹ� ������ �� ���� �ٲ��� ���� ��
    private int _uniqueIndex = -1;
    public int UniqueIndex
    {
        get { return _uniqueIndex; }
        set
        {
            if (_uniqueIndex == -1)
                _uniqueIndex = value;
        }
    }

    //private type name ��ų�� ������ ��� ����

    private void Awake()
    {
        _icon = _iconObject.GetComponent<Image>();
        _number = _numberObject.GetComponent<Image>();
    }

    private void Start()
    {
        Init();
        //Ŭ�������� ������ �� �ֵ��� �޼��� �־��ٰ�
        //_button.onClick.AddListener();
    }

    //���콺 �÷����� �˾����� ���߿� �۾��ؾߵ�
    private void OnMouseEnter()
    {
        //��ų ���� �˾� open
    }

    //���콺 ���Ž� �˾��ݱ� ���߿� �۾��ؾߵ�
    private void OnMouseExit()
    {
        //��ų ���� �˾� close
    }

    private void Init()
    {
        _icon.enabled = false;
        _number.enabled = false;

        _icon.sprite = null;
        _number.sprite = null;
    }
}
