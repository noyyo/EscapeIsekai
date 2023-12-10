using UnityEngine;

public class EquipCotroller : MonoBehaviour
{
    private EquipData myEquipItemData;
    private void Awake()
    {
        myEquipItemData = new EquipData();
    }

    public bool Equip(Item newItem, int newIndex, out Item oldItem, out int oldIndex)
    {
        oldIndex = -1;
        oldItem = null;
        if ((newItem.ID / 1000) % 100 < 10)//방어구
        {
            int n = (newItem.ID / 1000) % 10;
            switch (n)
            {
                case (int)ItemEquipmentType.HelmetArmor:
                    oldIndex = myEquipItemData.equipIndex[0];
                    myEquipItemData.equipIndex[0] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[0];
                    myEquipItemData.equipItemDatas[0] = newItem;
                    break;
                case (int)ItemEquipmentType.TopArmor:
                    oldIndex = myEquipItemData.equipIndex[1];
                    myEquipItemData.equipIndex[1] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[1];
                    myEquipItemData.equipItemDatas[1] = newItem;
                    break;
                case (int)ItemEquipmentType.BottomArmor:
                    oldIndex = myEquipItemData.equipIndex[2];
                    myEquipItemData.equipIndex[2] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[2];
                    myEquipItemData.equipItemDatas[2] = newItem;
                    break;
                case (int)ItemEquipmentType.ShoulderArmor:
                    oldIndex = myEquipItemData.equipIndex[3];
                    myEquipItemData.equipIndex[3] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[3];
                    myEquipItemData.equipItemDatas[3] = newItem;
                    break;
                case (int)ItemEquipmentType.GlovesArmor:
                    oldIndex = myEquipItemData.equipIndex[4];
                    myEquipItemData.equipIndex[4] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[4];
                    myEquipItemData.equipItemDatas[4] = newItem;
                    break;
                case (int)ItemEquipmentType.ShoesArmor:
                    oldIndex = myEquipItemData.equipIndex[5];
                    myEquipItemData.equipIndex[5] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[5];
                    myEquipItemData.equipItemDatas[5] = newItem;
                    break;
                default:
                    Debug.LogError("장비에 새로운 타입이 추가되었습니다 수정해주세요.");
                    break;
            }
        }
        else if ((newItem.ID / 1000) % 100 < 20)//무기
        {
            oldIndex = myEquipItemData.equipIndex[6];
            myEquipItemData.equipIndex[6] = newIndex;
            oldItem = myEquipItemData.equipItemDatas[6];
            myEquipItemData.equipItemDatas[6] = newItem;
        }
        else if ((newItem.ID / 1000) % 100 < 30)//보조무기
        {
            oldIndex = myEquipItemData.equipIndex[7];
            myEquipItemData.equipIndex[7] = newIndex;
            oldItem = myEquipItemData.equipItemDatas[7];
            myEquipItemData.equipItemDatas[7] = newItem;
        }
        else if ((newItem.ID / 1000) % 100 < 40)//장신구
        {
            int n = (newItem.ID / 1000) % 10;
            switch (n)
            {
                case (int)ItemEquipmentType.Cape:
                    oldIndex = myEquipItemData.equipIndex[8];
                    myEquipItemData.equipIndex[8] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[8];
                    myEquipItemData.equipItemDatas[8] = newItem;
                    break;
                case (int)ItemEquipmentType.Belt:
                    oldIndex = myEquipItemData.equipIndex[9];
                    myEquipItemData.equipIndex[9] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[9];
                    myEquipItemData.equipItemDatas[9] = newItem;
                    break;
                case (int)ItemEquipmentType.Ring:
                    oldIndex = myEquipItemData.equipIndex[10];
                    myEquipItemData.equipIndex[10] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[10];
                    myEquipItemData.equipItemDatas[10] = newItem;
                    break;
                case (int)ItemEquipmentType.Pendant:
                    oldIndex = myEquipItemData.equipIndex[11];
                    myEquipItemData.equipIndex[11] = newIndex;
                    oldItem = myEquipItemData.equipItemDatas[11];
                    myEquipItemData.equipItemDatas[11] = newItem;
                    break;
                default:
                    Debug.LogError("장비에 새로운 타입이 추가되었습니다 수정해주세요.");
                    break;
            }
        }
        else
        {
            Debug.LogError("아직 설정되지 않았습니다 EquipData에서 설정해 주세요");
        }

        if (oldItem == null)
            return false;
        else
            return true;
    }

    public void UnEquip(Item newItem)
    {
        int n = myEquipItemData.equipItemDatas.Length;
        for (int i = 0; i < n; i++)
        {
            if (myEquipItemData.equipItemDatas[i] == newItem)
            {
                myEquipItemData.equipIndex[i] = -1;
                myEquipItemData.equipItemDatas[i] = null;
                return;
            }
        }
    }
}
