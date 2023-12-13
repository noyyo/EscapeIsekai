using System;

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
