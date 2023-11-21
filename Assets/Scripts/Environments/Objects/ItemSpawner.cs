using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    private GameObject _genItem;
    private ItemData_Test itemData;
    [Tooltip("생성될 아이템의 아이디를 입력")]
    public int itemId;
    private Vector3 _direction = new Vector3(0, 1f, 0);
    private Vector3 _position;
    void Start()
    {
        ItemDB.Instance.GetItemData(itemId, out itemData);
        _genItem = itemData.DropPrefab;
        _position = transform.position;
        _position.y = _position.y + 0.5f;
        GenItem();
    }

    private void GenItem()
    {
        if (!CheckObject())
        {
            Instantiate(_genItem, _position, transform.rotation);
        }
        Invoke(nameof(GenItem), 180f);
    }
    private bool CheckObject()
    {
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData))
        {
            // The Ray hit something!
            if (hitData.collider.tag == "Gather")
            {
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

}
