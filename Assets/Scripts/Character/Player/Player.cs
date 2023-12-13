using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour, IPositionable
{
    [field: Header("References")]
    [field: SerializeField] public PlayerSO Data { get; private set; }

    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }
    [field: SerializeField] public Weapon Weapon { get; private set; }
    [field: SerializeField] public AnimationEventReceiver AnimationEventReceiver { get; private set; }
    [HideInInspector] public PlayerUI playerUI;
    [HideInInspector] public int hasGrenades;
    public Rigidbody Rigidbody { get; private set; }
    public CapsuleCollider Collider { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInputSystem Input { get; private set; }
    public CharacterController Controller { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }
    public Playerconditions Playerconditions { get; private set; }
    public Buff Buff { get; private set; }
    public PlayerStateMachine StateMachine;
    public GameObject[] grenades;
    public GameObject grenadeObj;
    public Transform throwPoint;
    public Transform teleportPosition;

    private void Awake()
    {
        AnimationData.Initialize();

        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();
        Animator = GetComponentInChildren<Animator>();
        Input = GetComponent<PlayerInputSystem>();
        Controller = GetComponent<CharacterController>();
        ForceReceiver = GetComponent<ForceReceiver>();
        Playerconditions = GetComponent<Playerconditions>();
        
        StateMachine = new PlayerStateMachine(this);

        playerUI = GameObject.FindObjectOfType<PlayerUI>();
        if (playerUI == null)
        {
            GameObject playerUIPrefab = Resources.Load<GameObject>("Prefabs/UI/Player UI");

            if (playerUIPrefab == null)
            {
                Debug.LogError("플레이어 UI 프리팹을 찾을 수 없습니다");
                return;
            }
            GameObject go = Instantiate(playerUIPrefab);
            playerUI = go.GetComponent<PlayerUI>();
        }
        Playerconditions.Initialize(playerUI);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        StateMachine.ChangeState(StateMachine.IdleState);
        StateMachine.OnDie += OnDie;
        Input.PlayerActions.Escape.started += Escape;
    }

    private void Update()
    {
        StateMachine.HandleInput();
        StateMachine.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.PhysicsUpdate();
    }

    void OnDie()
    {
        Animator.SetBool("Die", true);
        enabled = false;
    }

    public void CreateGrenadeWithDelay(float delayInSeconds)
    {
        if (InventoryManager.Instance.CallTryAddItem(10163000, -1))
        {
            StartCoroutine(CreateGrenadeCoroutine(delayInSeconds));
        }
        else
        {
            Debug.Log("스턴볼이 없습니다.");
        }
    }

    private IEnumerator CreateGrenadeCoroutine(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        Instantiate(grenadeObj, throwPoint.position, transform.rotation);
    }

    public Vector3 GetObjectCenterPosition()
    {
        return Collider.bounds.center;
    }

    protected virtual void Escape(InputAction.CallbackContext context)
    {
        Teleport(teleportPosition.position);
    }

    public void Teleport(Vector3 teleportPosition)
    {
        Controller.enabled = false;
        transform.position = teleportPosition;
        Controller.enabled = true;
        StateMachine.ChangeState(StateMachine.IdleState);
    }
}
