using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : CustomSingleton<GameManager>
{
    protected GameManager() { }
    [SerializeField] private GameObject _player;
    [Range(0.0f, 1.0f)]
    public float time; //하루 사이클 시간  0.2~0.8 해떠있는 시간
    public bool IsDay;

    private UI_Manager _ui_Manager;
    private SoundManager _soundManager;
    private PlayerInputSystem _playerInputSystem;
    private GameObject _soundManagerObject;
    private GameObject timeSlip;
    //초기화 순서에 따른 문제 또는 Scene이동, 의도치 않은 Player 삭제를 위한 안전장치
    public GameObject Player 
    { 
        get 
        {
            if (_player == null)
                PlayerInit();
            return _player; 
        } 
    }

    public event Action OnPauseEvent;
    public event Action OnUnpauseEvent;

    private void Awake()
    {
        if (_soundManagerObject == null)
            _soundManagerObject = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/SoundManager"));
        _soundManager = _soundManagerObject.GetComponent<SoundManager>();

        if(timeSlip ==null)
        {
            timeSlip = Instantiate(Resources.Load<GameObject>("Prefabs/UI/TimeSlip"));
        }
        PlayerInit();
        _ui_Manager = UI_Manager.Instance;
    }

    private void Start()
    {
        _ui_Manager.UI_InventoryTurnOnEvent += CursorEnable;
        _ui_Manager.UI_InventoryTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_InventoryTurnOnEvent += CallOnPauseEvent;

        _ui_Manager.UI_InventoryTurnOffEvent += CursorDisable;
        _ui_Manager.UI_InventoryTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_InventoryTurnOffEvent += CallOnUnpauseEvent;

        _ui_Manager.UI_ItemCraftingTurnOnEvent += CursorEnable;
        _ui_Manager.UI_ItemCraftingTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_ItemCraftingTurnOnEvent += CallOnPauseEvent;

        _ui_Manager.UI_ItemCraftingTurnOffEvent += CursorDisable;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += CallOnUnpauseEvent;
    }

    private void PlayerInit()
    {
        //다른 오브젝트에 Player태그가 설정되어가 있을경우 걸러내기 위한 foreach문
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(Tags.PlayerTag);
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.name == "Player")
                _player = gameObject;
        }
        if (_player == null)
            _player = Instantiate(Resources.Load<GameObject>("Prefabs/Player/Player"));

        _playerInputSystem = _player.GetComponent<PlayerInputSystem>();
    }

    private void CallOnPauseEvent()
    {
        Time.timeScale = 0f; //임시
        OnPauseEvent?.Invoke();
    }

    private void CallOnUnpauseEvent()
    {
        Time.timeScale = 1f; //임시
        OnUnpauseEvent?.Invoke();
    }

    private void PlayInputSystemEnable()
    {
        _playerInputSystem.PlayerActions.Enable();
    }

    private void PlayInputSystemDisable()
    {
        _playerInputSystem.PlayerActions.Disable();
    }

    private void CursorEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CursorDisable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
