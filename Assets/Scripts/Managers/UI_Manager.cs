using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : CustomSingleton<UI_Manager>
{
    [SerializeField] private GameObject _ui_inventroy;
    public GameObject Inventory_UI { get { return _ui_inventroy; } }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
