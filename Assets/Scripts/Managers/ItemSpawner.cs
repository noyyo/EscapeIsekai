using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private GameObject _genItem;
    private ItemData itemData;
    [Tooltip("생성될 아이템의 아이디를 입력")]
    public int itemId;
    private Vector3 _position;

    private WaitForSecondsRealtime waitForSeconds = new WaitForSecondsRealtime(30f);  //아이템 리젠 텀
    void Start()
    {
        ItemDB.Instance.GetItemData(itemId, out itemData);
        _genItem = itemData.DropPrefab;
        _position = transform.position;
        _position.y += 0.3f;
        Instantiate(_genItem, _position, transform.rotation);
    }

    public void EnableItem(GameObject Item)
    {
        StopAllCoroutines();
        StartCoroutine(RespawnItem(Item));
    }

    IEnumerator RespawnItem(GameObject Item)
    {
        yield return waitForSeconds;
        Item.SetActive(true);
    }

}
