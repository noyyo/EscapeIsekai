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
        string path = Path.Combine(Application.dataPath, "Resources/Json/itemTestJson.json");//json파일이 있는 주소불러오기
        string json = File.ReadAllText(path);//json파일안에 있는거 전부 읽어오기
        var arrItemDatas = JsonConvert.DeserializeObject<ItemData[]>(json);
        this.dicItemDatas = arrItemDatas.ToDictionary(x => x.id);
    }

    public ItemData GetData(int id) //id로 아이템 정보 습득 가능
    {
        return dicItemDatas[id];
    }
}
