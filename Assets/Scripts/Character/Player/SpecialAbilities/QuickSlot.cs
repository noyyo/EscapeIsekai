using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _text;

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

    private void Start()
    {
        Init();
        //Ŭ�������� ������ �� �ֵ��� �޼��� �־��ٰ�
        //_button.onClick.AddListener();
    }

    private void OnMouseEnter()
    {
        //��ų ���� �˾� open
    }

    private void OnMouseExit()
    {
        //��ų ���� �˾� close
    }

    public void Init()
    {
        _icon.enabled = false;
        _text.text = "";
    }
}
