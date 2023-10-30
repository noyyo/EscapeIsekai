using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    [SerializeField] private GameObject _inventory_UI;
    [SerializeField] private GameObject _inventory_ItemPopUp;
    public GameObject Inventory_UI { get { return _inventory_UI; } }
    public GameObject Inventory_ItemPopUp {  get { return _inventory_ItemPopUp; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
