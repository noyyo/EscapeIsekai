using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private Item item;
    public Item Item { get { return item; } }

    public void SetItem(Item newitem)
    {
        if (item == null)
            item = newitem;
    }

    private void OnTriggerEnter(Collider other)
    {
        //UI팝업 띄우면서 상호작용시 다시 주울수 있습니다.
    }

    private void OnTriggerExit(Collider other)
    {
        //UI팝업 오프
    }
}
