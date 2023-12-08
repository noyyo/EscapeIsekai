using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IPositionable
{
    [field: Header("References")]
    [field: SerializeField] public PlayerSO Data { get; private set; }

    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public CapsuleCollider Collider { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInputSystem Input { get; private set; }
    public CharacterController Controller { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public Weapon Weapon { get; private set; }
    public Playerconditions Playerconditions { get; private set; }
    public Buff Buff { get; private set; }

    public PlayerStateMachine StateMachine;
    [field: SerializeField] public AnimationEventReceiver AnimationEventReceiver { get; private set; }

    [HideInInspector] public PlayerUI playerUI;


    public GameObject[] grenades;
    [HideInInspector] public int hasGrenades;
    public GameObject grenadeObj;
    public Transform throwPoint;

    private void Awake()
    {
        // �ִϸ��̼� �����ʹ� �׻� �ʱ�ȭ�� ���־�� �Ѵ�
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

            // ���� �÷��̾� UI ������Ʈ�� ã�ƿ��� playerUIPrefab ���������� �Ҵ�
            GameObject playerUIPrefab = Resources.Load<GameObject>("Prefabs/UI/Player UI");


            // ã�ƿ��� ������ ��� ���(����ó��)
            if (playerUIPrefab == null)
            {
                Debug.LogError("�÷��̾� UI �������� ã�� �� �����ϴ�");
                return;
            }

            // ã�ƿ� ������Ʈ�� �����ϰ� go ���������� �Ҵ�
            GameObject go = Instantiate(playerUIPrefab);

            playerUI = go.GetComponent<PlayerUI>();
        }


        // To do : �÷��̾� UI�� ã�Ұų� ������ �ϰų� ������ ������. �װ��� ������ �ִ� �͵��� ã�Ƽ� �����;���
        Playerconditions.Initialize(playerUI);
    }

    private void Start()
    {
        // ���콺 Ŀ���� ������� ��
        Cursor.lockState = CursorLockMode.Locked;
        // ĳ���Ͱ� �� ó�� �����ؾ� �� Idle ���·� ������ֱ�
        StateMachine.ChangeState(StateMachine.IdleState);
        //Health.OnDie += OnDie;
        StateMachine.OnDie += OnDie;
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
            Debug.Log("���Ϻ��� �����ϴ�.");
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

}
