using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

public class DataManager : MonoBehaviour
{
    public static DataManager _dataInstance = null;
    public ItemData itemData;
    public Dictionary<int, ItemData> dicItemDatas;  //= new Dictionary<int, ItemData>()

    private void Awake()
    {
        if( _dataInstance == null)
        {
            _dataInstance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static DataManager GatInstance()
    {
        if(DataManager._dataInstance == null)
        {
            DataManager._dataInstance = new DataManager();
        }
        return DataManager._dataInstance;
    }
    public void LoadDatas()
    {
        string path = Path.Combine(Application.dataPath, "Resources/Json/itemTestJson.json");//json������ �ִ� �ּҺҷ�����
        string json = File.ReadAllText(path);//json���Ͼȿ� �ִ°� ���� �о����
        var arrItemDatas = JsonConvert.DeserializeObject<ItemData[]>(json);
        this.dicItemDatas = arrItemDatas.ToDictionary(x => x.id);
    }
}
