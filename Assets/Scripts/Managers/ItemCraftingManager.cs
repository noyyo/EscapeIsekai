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
    private ItemRecipe clickSlot;
    private Transform playerTransform;
    private readonly string crftingSoundName = "CrftingSound";
    private bool isMake;

    public GameObject CraftingSlotPrefab { get { return craftingSlotPrefab; } }
    public ItemRecipe ClickSlot { get { return clickSlot; } }
    public MaterialsSlot[] MaterialsSlots { get { return materialsSlots; } }
    public bool IsMake { get { return isMake; } }

    public event Action OnClickCraftingSlotEvent;
    public event Action OffOutLineEvent;
    public event Action<ItemRecipe> OnUpdateUIEvent;
    public event Action OnCraftingEvent;
    public event Action OnTextUpdateEvent;

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
        clickSlot = newRecipe;
        isMake = isNewMake;
        OnClickCraftingSlotEvent?.Invoke();
    }

    public void CallOffOutLineEvent()
    {
        OffOutLineEvent?.Invoke();
        OffOutLineEvent = null;
    }

    public void CallUpdateUI()
    {
        OnUpdateUIEvent?.Invoke(ClickSlot);
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

    private void ItemCraftingUITurnOff()
    {
        itemCraftingUI.SetActive(false);
    }

    private void ItemCraftingUITurnOn()
    {
        itemCraftingUI.SetActive(true);
    }

    public void CallAddRecipe(int id)
    {
        craftingController.AddRecipe(id);
    }

    public bool CraftingItem(bool isMake)
    {
        this.isMake = isMake;
        if (this.isMake)
        {
            OnTextUpdateEvent?.Invoke();
            PlayCraftingSound();
            return inventoryManager.CallAddItems(ClickSlot);
        }
        ui_Manager.PlayWrongSound();
        return false;
    }
    public void PlayCraftingSound()
    {
        soundManager.CallPlaySFX(ClipType.UISFX, crftingSoundName, playerTransform, false, soundValue : 3f);
    } 
}
