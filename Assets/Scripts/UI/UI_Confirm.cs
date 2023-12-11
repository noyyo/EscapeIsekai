using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Confirm : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject headTextGO;
    [SerializeField] private GameObject contentGO;
    [SerializeField] private GameObject confirmGO;
    [SerializeField] private GameObject cancelGO;
    [Tooltip("UI의 Head부분에 있는 X를 넣어주시면 됩니다.")][SerializeField] GameObject headCancelGo;
    [SerializeField] private GameObject inputFieldGO;

    private TMP_Text headText;
    private TMP_Text contentText;
    private TMP_Text confirmText;
    private TMP_Text cancelText;
    private Button confirmBtn;
    private Button cancelBtn;
    private Button headCancelBtn;

    private TMP_InputField inputField;
    private Vector2 defaultPos;

    public Action confirmBtnAction;
    public Action cancelBtnAction;
    public Action headCancelBtnAction;

    private void Awake()
    {
        if (!headTextGO.TryGetComponent<TMP_Text>(out headText))
            headText = headTextGO.GetComponentInChildren<TMP_Text>();
        if (headText == null)
            Debug.LogError("headTextGO을 넣어주세요");

        if (!contentGO.TryGetComponent<TMP_Text>(out contentText))
            contentText = contentGO.GetComponentInChildren<TMP_Text>();
        if (contentText == null)
            Debug.LogError("contentGO을 넣어주세요");

        if (!confirmGO.TryGetComponent<TMP_Text>(out confirmText))
            confirmText = confirmGO.GetComponentInChildren<TMP_Text>();
        if (!confirmGO.TryGetComponent<Button>(out confirmBtn))
            confirmBtn = confirmGO.GetComponentInChildren<Button>();
        if (confirmText == null)
            Debug.LogError("confirmGO을 넣어주세요");

        if (!cancelGO.TryGetComponent<TMP_Text>(out cancelText))
            cancelText = cancelGO.GetComponentInChildren<TMP_Text>();
        if (!cancelGO.TryGetComponent<Button>(out cancelBtn))
            cancelBtn = cancelGO.GetComponentInChildren<Button>();
        if (cancelText == null)
            Debug.LogError("cancelGO을 넣어주세요");

        if (!headCancelGo.TryGetComponent<Button>(out headCancelBtn))
            headCancelBtn = headCancelGo.GetComponentInChildren<Button>();
        if (headCancelBtn == null)
            Debug.LogError("headCancelGo을 넣어주세요");

        if (!inputFieldGO.TryGetComponent<TMP_InputField>(out inputField))
            inputField = inputFieldGO.GetComponentInChildren<TMP_InputField>();
    }

    private void Start()
    {
        confirmBtn.onClick.AddListener(() => confirmBtnAction?.Invoke());
        cancelBtn.onClick.AddListener(() => cancelBtnAction?.Invoke());
        headCancelBtn.onClick.AddListener(() => headCancelBtnAction?.Invoke());
    }

    public void headTextUpdate(string str)
    {
        headText.text = str;
    }

    public void headTextFontSize(float size)
    {
        headText.fontSize = size;
    }

    public void contentTextUpdate(string str)
    {
        contentText.text = str;
    }

    public void contentTextFontSize(float size)
    {
        contentText.fontSize = size;
    }

    public void confirmTextUpdate(string str)
    {
        confirmText.text = str;
    }

    public void confirmTextFontSize(float size)
    {
        confirmText.fontSize = size;
    }

    public void cancelTextUpdate(string str)
    {
        cancelText.text = str;
    }

    public void cancelTextFontSize(float size)
    {
        cancelText.fontSize = size;
    }

    public void HeadTextGOTurnOn()
    {
        headTextGO.SetActive(true);
    }

    public void HeadTextGOTurnOff()
    {
        headTextGO.SetActive(false);
    }

    public void ContentGOTurnOn()
    {
        contentGO.SetActive(true);
    }

    public void ContentGOTurnOff()
    {
        contentGO.SetActive(false);
    }

    public void ConfirmGOTurnOn()
    {
        confirmGO.SetActive(true);
    }

    public void ConfirmGOTurnOff()
    {
        confirmGO.SetActive(false);
    }

    public void CancelGOTurnOn()
    {
        cancelGO.SetActive(true);
    }

    public void CancelGOTurnOff()
    {
        cancelGO.SetActive(false);
    }

    public void HeadCancelGoTurnOn()
    {
        headCancelGo.SetActive(true);
    }

    public void HeadCancelGoTurnOff()
    {
        headCancelGo.SetActive(false);
    }

    public void InputFieldGOTurnOn()
    {
        inputFieldGO.SetActive(true);
    }

    public void InputFieldGOTurnOff()
    {
        inputFieldGO.SetActive(false);
    }

    public void InputFieldSetOnEndEdit(Action<string> action)
    {
        inputField.onEndEdit.AddListener((str) => action.Invoke(str));
    }

    public void InputFieldRemoveAllListeners()
    {
        inputField.onEndEdit.RemoveAllListeners();
    }

    /// <summary>
    /// 기본을 정수입니다.
    /// </summary>
    /// <param name="contentType"></param>
    public void InputFieldContentType(TMP_InputField.ContentType contentType)
    {
        inputField.contentType = contentType;
    }

    public void InputFieldFontSize(float size)
    {
        inputField.pointSize = size;
    }

    public void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        defaultPos = new Vector2(this.transform.position.x - eventData.position.x, this.transform.position.y - eventData.position.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = (eventData.position + defaultPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        defaultPos = Vector2.zero;
    }
}
