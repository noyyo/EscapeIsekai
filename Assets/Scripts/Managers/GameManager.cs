using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : CustomSingleton<GameManager>
{
    protected GameManager() { }
    [SerializeField] private GameObject _player;
    [SerializeField] private CinemachineVirtualCamera characterCamera;
    [Range(0.0f, 1.0f)]
    public float time; //�Ϸ� ����Ŭ �ð�  0.2~0.8 �ض��ִ� �ð�
    public bool IsDay;
    public GameObject dialogCamera;
    private UI_Manager _ui_Manager;
    private SoundManager _soundManager;
    private PlayerInputSystem _playerInputSystem;
    private GameObject _soundManagerObject;
    public GameObject timeSlip;
    public UI_Manager Ui_Manager { get { return _ui_Manager; } }
    //�ʱ�ȭ ������ ���� ���� �Ǵ� Scene�̵�, �ǵ�ġ ���� Player ������ ���� ������ġ
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
        PlayerInit();
        _ui_Manager = UI_Manager.Instance;
        if (timeSlip == null)
        {
            timeSlip = Instantiate(Resources.Load<GameObject>("Prefabs/UI/TimeSlip"));
        }

        if(dialogCamera ==null)
        {
            dialogCamera = Player.GetComponentInChildren<Camera>().gameObject;
            dialogCamera.SetActive(false);
        }

        if (characterCamera == null)
            characterCamera = GameObject.FindGameObjectWithTag("CharacterCamera").GetComponent<CinemachineVirtualCamera>();

        CursorEnable();
    }

    private void Start()
    {
        _ui_Manager.UI_InventoryTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_InventoryTurnOnEvent += CursorEnable;
        _ui_Manager.UI_InventoryTurnOnEvent += CallOnPauseEvent;
        _ui_Manager.UI_InventoryTurnOnEvent += CameraLock;

        _ui_Manager.UI_InventoryTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_InventoryTurnOffEvent += CursorDisable;
        _ui_Manager.UI_InventoryTurnOffEvent += CallOnUnpauseEvent;
        _ui_Manager.UI_InventoryTurnOffEvent += CameraUnLock;

        _ui_Manager.UI_ItemCraftingTurnOnEvent += CursorEnable;
        _ui_Manager.UI_ItemCraftingTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_ItemCraftingTurnOnEvent += CallOnPauseEvent;
        _ui_Manager.UI_ItemCraftingTurnOnEvent += CameraLock;

        _ui_Manager.UI_ItemCraftingTurnOffEvent += CursorDisable;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += CallOnUnpauseEvent;
        _ui_Manager.UI_ItemCraftingTurnOffEvent += CameraUnLock;

        _ui_Manager.UI_TradingTurnOnEvent += CursorEnable;
        _ui_Manager.UI_TradingTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_TradingTurnOnEvent += CallOnPauseEvent;
        _ui_Manager.UI_TradingTurnOnEvent += CameraLock;

        _ui_Manager.UI_TradingTurnOffEvent += CursorDisable;
        _ui_Manager.UI_TradingTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_TradingTurnOffEvent += CallOnUnpauseEvent;
        _ui_Manager.UI_TradingTurnOffEvent += CameraUnLock;

        _ui_Manager.UI_OptionTurnOnEvent += CursorEnable;
        _ui_Manager.UI_OptionTurnOnEvent += PlayInputSystemDisable;
        _ui_Manager.UI_OptionTurnOnEvent += CallOnPauseEvent;
        _ui_Manager.UI_OptionTurnOnEvent += CameraLock;

        _ui_Manager.UI_OptionTurnOffEvent += CursorDisable;
        _ui_Manager.UI_OptionTurnOffEvent += PlayInputSystemEnable;
        _ui_Manager.UI_OptionTurnOffEvent += CallOnUnpauseEvent;
        _ui_Manager.UI_OptionTurnOffEvent += CameraUnLock;
    }

    private void PlayerInit()
    {
        //�ٸ� ������Ʈ�� Player�±װ� �����Ǿ ������� �ɷ����� ���� foreach��
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(TagsAndLayers.PlayerTag);
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
        Time.timeScale = 0f; //�ӽ�
        OnPauseEvent?.Invoke();
    }

    private void CallOnUnpauseEvent()
    {
        Time.timeScale = 1f; //�ӽ�
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

    private void CameraLock()
    {
        characterCamera.enabled = false;
    }

    private void CameraUnLock()
    {
        characterCamera.enabled = true;
    }

}
