using System.Collections.Generic;
using Unity.VisualScripting;
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
        //UI�˾� ���鼭 ��ȣ�ۿ�� �ٽ� �ֿ�� �ֽ��ϴ�.
    }

    private void OnTriggerExit(Collider other)
    {
        //UI�˾� ����
    }
}
