using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Confirm : MonoBehaviour
{
    [SerializeField] private TMP_Text content;
    [SerializeField] private Button confirm;
    [SerializeField] private Button cancel;
    [Tooltip("UI의 Head부분에 있는 X를 넣어주시면 됩니다.")][SerializeField] Button back;

    [Header("Is Custom")]
    [SerializeField] private bool isCustom;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
