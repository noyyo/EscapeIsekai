using UnityEngine;
using UnityEngine.InputSystem;

public class GatheringObject : MonoBehaviour
{
    private int _itemId;
    private bool _gathering = false;
    private ItemData itemData;
    private Player _playerInputSystem;
    private UI_Manager _UI_Manager;
    private ItemSpawner itemSpawner;
    private Transform playerTransform;
    private ParticleSystem particle;
    private Player player;
    private float playerDistance;
    static readonly private float distanceCheckDelay = 1f;
    static readonly private float particlePlayDistance = 30f;
    private float lastDistanceCheckTime;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        _UI_Manager = UI_Manager.Instance;
        player = GameManager.Instance.Player.GetComponent<Player>();
        playerTransform = GameManager.Instance.Player.transform;
    }
    private void Update()
    {
        if (player == null)
        {
            Debug.LogError("플레이어가 없습니다.");
            return;
        }
        if (Time.time - lastDistanceCheckTime > distanceCheckDelay)
        {
            CheckPlayerDistance();
        }
    }
    private void CheckPlayerDistance()
    {
        playerDistance = Vector3.Distance(player.transform.position, transform.position);
        lastDistanceCheckTime = Time.time;
        if (playerDistance > particlePlayDistance && particle.isPlaying)
        {
            particle.Stop();
        }
        else if (playerDistance <= particlePlayDistance && !particle.isPlaying)
        {
            particle.Play();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == TagsAndLayers.ItemSpawnerTag)
        {
            itemSpawner = other.GetComponent<ItemSpawner>();
            _itemId = itemSpawner.itemId;
            ItemDB.Instance.GetItemData(_itemId, out itemData);
        }
        if (other.tag == TagsAndLayers.PlayerTag)
        {
            _gathering = true;
            if (_playerInputSystem == null)
            {
                _playerInputSystem = other.GetComponent<Player>();

            }
            _playerInputSystem.Input.PlayerActions.Interaction.started += Gathering;

            _UI_Manager.itemName = itemData.ItemName;
            _UI_Manager.itemExplanation = itemData.ItemExplanation;
            _UI_Manager.gathering.SetActive(true);
            _UI_Manager.UI_gathering.Setting();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == TagsAndLayers.PlayerTag)
        {
            _playerInputSystem.Input.PlayerActions.Interaction.started -= Gathering;
            UI_Manager.Instance.gathering.SetActive(false);
            _gathering = false;
        }
    }



    private void Gathering(InputAction.CallbackContext context)
    {
        if (_gathering)
        {
            SoundManager.Instance.CallPlaySFX(ClipType.EnvironmentSFX, "pick", playerTransform, false, soundValue: 0.05f);
            _UI_Manager.gathering.SetActive(false);
            InventoryManager.Instance.CallAddItem(_itemId, 1);
            _playerInputSystem.Input.PlayerActions.Interaction.started -= Gathering;
            _gathering = false;
            itemSpawner.EnableItem(gameObject);
            gameObject.SetActive(false);
        }
    }
}
