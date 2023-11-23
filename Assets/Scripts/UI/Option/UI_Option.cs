using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum MiXType
{
    MasterVolume,
    BgmVolume,
    SfxVolume
}

public class UI_Option : MonoBehaviour
{
    private SoundManager soundManager;
    private UI_Manager uiManager;
    private Slider MasterVolume;
    private Slider bgmVolume;
    private Slider sfxVolume;
    private TMP_InputField MasterVolumeInputField;
    private TMP_InputField bgmVolumeInputField;
    private TMP_InputField sfxVolumeInputField;
    private Button back;
    private Button inventory;
    private string max;
    private string min;

    private void Awake()
    {
        uiManager = UI_Manager.Instance;
        MasterVolume = this.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Slider>();
        MasterVolumeInputField = this.transform.GetChild(2).GetChild(0).GetChild(2).GetComponent<TMP_InputField>();
        bgmVolume = this.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Slider>();
        bgmVolumeInputField = this.transform.GetChild(2).GetChild(1).GetChild(2).GetComponent<TMP_InputField>();
        sfxVolume = this.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Slider>();
        sfxVolumeInputField = this.transform.GetChild(2).GetChild(2).GetChild(2).GetComponent<TMP_InputField>();
        back = this.transform.GetChild(1).GetChild(0).GetComponent<Button>();
        inventory = this.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        uiManager.UI_OptionTurnOnEvent += Activate;
        uiManager.UI_OptionTurnOffEvent += Deactivate;
        max = "100";
        min = "0";
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        soundManager = SoundManager.Instance;
        MasterVolume.onValueChanged.AddListener((n) => SliderValueChange(MasterVolumeInputField, n, MiXType.MasterVolume));
        bgmVolume.onValueChanged.AddListener((n) => SliderValueChange(bgmVolumeInputField, n, MiXType.BgmVolume));
        sfxVolume.onValueChanged.AddListener((n) => SliderValueChange(sfxVolumeInputField, n, MiXType.SfxVolume));
        MasterVolumeInputField.onEndEdit.AddListener((n) => ChangeText(n, MasterVolumeInputField, MasterVolume , MiXType.MasterVolume));
        bgmVolumeInputField.onEndEdit.AddListener((n) => ChangeText(n, MasterVolumeInputField, bgmVolume, MiXType.BgmVolume));
        sfxVolumeInputField.onEndEdit.AddListener((n) => ChangeText(n, MasterVolumeInputField, sfxVolume, MiXType.SfxVolume));
        back.onClick.AddListener(uiManager.CallUI_OptionTurnOff);
        inventory.onClick.AddListener(() => { uiManager.CallUI_OptionTurnOff(); uiManager.CallUI_InventoryTurnOn(); });
    }

    private void SliderValueChange(TMP_InputField inputField, float n, MiXType mixType)
    {
        inputField.text = n.ToString();
        VolumeChange(mixType, n);
    }

    private void ChangeText(string str, TMP_InputField inputField, Slider slider, MiXType mixType)
    {
        if (float.TryParse(str, out float result))
        {
            if (result >= 0 && result <= 100f)
                slider.value = result;
            else
                slider.value = result > 100 ? 100 : 0;
            VolumeChange(mixType, slider.value);
        }
        inputField.text = slider.value.ToString();
    }

    private void VolumeChange(MiXType mixType, float n)
    {
        switch (mixType)
        {
            case MiXType.MasterVolume:
                soundManager.MasterVolume(n);
                break;
            case MiXType.BgmVolume:
                soundManager.BGMVolume(n);
                break;
            case MiXType.SfxVolume:
                soundManager.SFXVolume(n);
                break;
        }
    }

    private void Activate()
    {
        this.gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
