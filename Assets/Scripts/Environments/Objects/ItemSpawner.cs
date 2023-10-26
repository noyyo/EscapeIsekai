using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _genItem;
    private Vector3 _direction = new Vector3(0, 1f, 0);
    // Start is called before the first frame update
    void Start()
    {
        GenItem();
    }

    private void GenItem()
    {
        if (!CheckObject())
        {
            Instantiate(_genItem, transform.position, transform.rotation);
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
