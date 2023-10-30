using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Unity.VisualScripting;

public class DataManager : CustomSingleton<DataManager>
{
    public ItemData itemData;
    public Dictionary<int, ItemData> dicItemDatas;  

    public void LoadDatas()
    {
        string path = Path.Combine(Application.dataPath, "Resources/Json/itemTestJson.json");//json������ �ִ� �ּҺҷ�����
        string json = File.ReadAllText(path);//json���Ͼȿ� �ִ°� ���� �о����
        var arrItemDatas = JsonConvert.DeserializeObject<ItemData[]>(json);
        this.dicItemDatas = arrItemDatas.ToDictionary(x => x.id);
    }

    public ItemData GetData(int id) //id�� ������ ���� ���� ����
    {
        return dicItemDatas[id];
    }
}
