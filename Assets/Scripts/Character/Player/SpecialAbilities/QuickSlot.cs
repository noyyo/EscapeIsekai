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

    //슬롯의 위치 한번 설정한 후 절대 바뀌지 않을 값
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

    //private type name 스킬의 정보를 담는 변수

    private void Awake()
    {
        _icon = _iconObject.GetComponent<Image>();
        _number = _numberObject.GetComponent<Image>();
    }

    private void Start()
    {
        Init();
        //클릭했을떄 실행할 수 있도록 메서드 넣어줄것
        //_button.onClick.AddListener();
    }

    //마우스 올렸을시 팝업열기 나중에 작업해야됨
    private void OnMouseEnter()
    {
        //스킬 설명 팝업 open
    }

    //마우스 제거시 팝업닫기 나중에 작업해야됨
    private void OnMouseExit()
    {
        //스킬 설명 팝업 close
    }

    private void Init()
    {
        _icon.enabled = false;
        _number.enabled = false;

        _icon.sprite = null;
        _number.sprite = null;
    }
}
