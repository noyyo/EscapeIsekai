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

    private void Start()
    {
        Init();
        //클릭했을떄 실행할 수 있도록 메서드 넣어줄것
        //_button.onClick.AddListener();
    }

    private void OnMouseEnter()
    {
        //스킬 설명 팝업 open
    }

    private void OnMouseExit()
    {
        //스킬 설명 팝업 close
    }

    public void Init()
    {
        _icon.enabled = false;
        _text.text = "";
    }
}
