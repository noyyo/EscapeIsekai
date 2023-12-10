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
        // 애니메이션 데이터는 항상 초기화를 해주어야 한다
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

            // 씬의 플레이어 UI 오브젝트를 찾아오고 playerUIPrefab 지역변수에 할당
            GameObject playerUIPrefab = Resources.Load<GameObject>("Prefabs/UI/Player UI");


            // 찾아오지 못했을 경우 출력(예외처리)
            if (playerUIPrefab == null)
            {
                Debug.LogError("플레이어 UI 프리팹을 찾을 수 없습니다");
                return;
            }

            // 찾아온 오브젝트를 생성하고 go 지역변수에 할당
            GameObject go = Instantiate(playerUIPrefab);

            playerUI = go.GetComponent<PlayerUI>();
        }


        // To do : 플레이어 UI를 찾았거나 생성을 하거나 가지고 있을것. 그것의 하위에 있는 것들을 찾아서 가져와야함
        Playerconditions.Initialize(playerUI);
    }

    private void Start()
    {
        // 마우스 커서를 사라지게 함
        Cursor.lockState = CursorLockMode.Locked;
        // 캐릭터가 맨 처음 동작해야 할 Idle 상태로 만들어주기
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

}
