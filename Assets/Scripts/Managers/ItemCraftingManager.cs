using System;
using UnityEngine;
public class ItemCraftingManager : CustomSingleton<ItemCraftingManager>
{
    protected ItemCraftingManager() { }
    private GameManager gameManager;
    private UI_Manager ui_Manager;
    private InventoryManager inventoryManager;
    private SoundManager soundManager;
    private ItemCraftingController craftingController;
    private GameObject itemCraftingUI;
    private GameObject craftingSlotPrefab;
    private GameObject itemCaftingMaterials_UI;
    private GameObject itemExplanation_UI;
    private MaterialsSlot[] materialsSlots;
    private ItemRecipe currentClickSlot;
    private Transform playerTransform;
    private readonly string crftingSoundName = "CrftingSound";
    private bool currentIsMake;

    public GameObject CraftingSlotPrefab { get { return craftingSlotPrefab; } }
    public ItemRecipe CurrentClickSlot { get { return currentClickSlot; } }
    public MaterialsSlot[] MaterialsSlots { get { return materialsSlots; } }
    public bool CurrentIsMake { get { return currentIsMake; } }

    public event Action OnClickCraftingSlotEvent;
    public event Action OffOutLineEvent;
    public event Action<ItemRecipe> OnUpdateUIEvent;
    public event Action OnCraftingEvent;
    public event Action OnTextUpdateEvent;
    public Func<bool> ChangeCurrentIsMake;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        ui_Manager = UI_Manager.Instance;
        inventoryManager = InventoryManager.Instance;
        soundManager = SoundManager.Instance;
        craftingSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/ItemCrafting/CreftingSlot");
        craftingController = gameManager.Player.GetComponent<ItemCraftingController>();
    }

    private void Start()
    {
        Init();
        ui_Manager.UI_ItemCraftingTurnOnEvent += ItemCraftingUITurnOn;

        OnClickCraftingSlotEvent += ItemMaterialsUITurnOn;
        OnClickCraftingSlotEvent += CallUpdateUI;
        OnClickCraftingSlotEvent += CallOffOutLineEvent;
        OnClickCraftingSlotEvent += ui_Manager.PlayClickSound;

        ui_Manager.UI_ItemCraftingTurnOffEvent += CallOffOutLineEvent;
        ui_Manager.UI_ItemCraftingTurnOffEvent += ItemMaterialsUITurnOff;
        ui_Manager.UI_ItemCraftingTurnOffEvent += ItemCraftingUITurnOff;

        OnCraftingEvent += CraftingItem;

        playerTransform = gameManager.Player.transform;
    }

    private void Init()
    {
        itemCraftingUI = ui_Manager.ItemCrafting_UI;
        itemCaftingMaterials_UI = itemCraftingUI.transform.GetChild(2).gameObject;
        itemExplanation_UI = itemCraftingUI.transform.GetChild(3).gameObject;
        materialsSlots = itemCaftingMaterials_UI.transform.GetChild(3).GetComponentsInChildren<MaterialsSlot>();
    }

    public void CallOnClickCraftingSlotEvent(ItemRecipe newRecipe, bool isNewMake)
    {
        currentClickSlot = newRecipe;
        currentIsMake = isNewMake;
        OnClickCraftingSlotEvent?.Invoke();
    }

    public void CallOffOutLineEvent()
    {
        OffOutLineEvent?.Invoke();
        OffOutLineEvent = null;
    }

    public void CallUpdateUI()
    {
        OnUpdateUIEvent?.Invoke(CurrentClickSlot);
    }

    public void CallOnCrafting()
    {
        OnCraftingEvent?.Invoke();
    }

    private void ItemMaterialsUITurnOn()
    {
        itemCaftingMaterials_UI.SetActive(true);
        itemExplanation_UI.SetActive(true);
    }

    private void ItemMaterialsUITurnOff()
    {
        itemCaftingMaterials_UI.SetActive(false);
        itemExplanation_UI.SetActive(false);
    }

    //자체 UI off
    private void ItemCraftingUITurnOff()
    {
        itemCraftingUI.SetActive(false);
    }
    //자체 UI on
    private void ItemCraftingUITurnOn()
    {
        OnTextUpdateEvent?.Invoke();
        itemCraftingUI.SetActive(true);
    }
    //레시피 추가
    public void CallAddRecipe(int id)
    {
        craftingController.AddRecipe(id);
    }

    public void CraftingItem()
    {
        if (currentIsMake && TradingManager.Instance.PlayerMoney >= currentClickSlot.CraftingPrice)
        {
            inventoryManager.CallAddItems(CurrentClickSlot);
            OnTextUpdateEvent?.Invoke();
            currentIsMake = ChangeCurrentIsMake();
            PlayCraftingSound();
        }
        else
            ui_Manager.PlayWrongSound();
    }

    public void PlayCraftingSound()
    {
        soundManager.CallPlaySFX(ClipType.UISFX, crftingSoundName, playerTransform, false, soundValue : 0.1f);
    } 
}
