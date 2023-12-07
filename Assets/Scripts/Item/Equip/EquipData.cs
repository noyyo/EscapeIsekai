using System;

//나중에 수정해야될 방향
//public enum ItemEquipmentType
//{
//    //방어구는 0 ~ 9, 무기 10 ~ 19, 장신구 20 ~ 29
//    HelmetArmor,
//    TopArmor,
//    BottomArmor,
//    ShoulderArmor,
//    GlovesArmor,
//    ShoesArmor,
//    OneHandedWeapon = 10,
//    TwoHandedWeapon = 11,
//    SecondaryWeapon = 20,
//    Cape = 30,
//    Belt = 31,
//    Ring = 32,
//    Pendant = 33
//}

//현재의 아이템이 저장되는 타입분류
public enum ItemEquipmentType
{
    HelmetArmor,        //0
    TopArmor,           //1
    BottomArmor,
    ShoulderArmor,
    GlovesArmor,
    ShoesArmor,         //5
    OneHandedWeapon,    //6
    SecondaryWeapon,    //7
    Cape,               //8
    Belt,               //9
    Ring,               //10
    Pendant             //11
}

public class EquipData
{
    public Item[] equipItemDatas;
    public int[] equipIndex;
    
    public EquipData()
    { 
        equipItemDatas = new Item[12];
        equipIndex = new int[12];
        Array.Fill(equipIndex, -1);
    }
}
