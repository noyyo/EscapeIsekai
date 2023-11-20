using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Windows;
using System.IO;


public enum Grade
{
    Normal,
    Gather,
    Craft,
    Food
}
public class Capture : MonoBehaviour
{
    public Camera cam;
    public RenderTexture rt;
    public Image bg;
    public int num;

    public Grade grade;

    public GameObject[] obj;
    int nowCnt = 0;

    private void Start()
    {
        cam = Camera.main;
        SettingColor();

    }
    public void Create()
    {
        StartCoroutine(CaptureImage());
    }
    IEnumerator CaptureImage()
    {
        yield return null;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        yield return null;

        var data = tex.EncodeToPNG();
        string name = $"{grade}_{num}";
        string extention = ".png";
        string path = Application.dataPath + "/Resources/Sprite/Icon/";

        Debug.Log(path);

        if(!Directory.Exists(path)) Directory.CreateDirectory(path);

        File.WriteAllBytes(path + name + extention, data);

        yield return null;
    }
    public void AllCreate()
    {
        StartCoroutine(AllCaptureImage());
    }
    IEnumerator AllCaptureImage()
    {
        while (nowCnt < obj.Length)
        {
            var nowObj = Instantiate(obj[nowCnt].gameObject);

            yield return null;

            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

            yield return null;

            var data = tex.EncodeToPNG();
            //string name = $"Thumbnail_{obj[nowCnt].gameObject.name}";
            string name = $"{grade}_{num}";
            string extention = ".png";
            string path = Application.dataPath + "/Resources/Sprite/Icon/";

            Debug.Log(path);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            File.WriteAllBytes(path + name + extention, data);

            yield return null;

            DestroyImmediate(nowObj);

            num++;
            nowCnt++;
            yield return null;
        }
    }

    void SettingColor()
    {
        switch(grade)
        {
            case Grade.Normal:
                cam.backgroundColor = Color.white;
                bg.color = Color.white;
                break;                
            case Grade.Gather:
                cam.backgroundColor = new Color(219f/255f, 248f / 255f, 170f / 255f);
                bg.color = new Color(219f / 255f, 248f / 255f, 170f / 255f);
                break;
            case Grade.Craft:
                cam.backgroundColor = new Color(106f / 255f, 200f / 255f, 253f / 255f);
                bg.color = new Color(106f / 255f, 200f / 255f, 253f / 255f);
                break;
            case Grade.Food:
                cam.backgroundColor = new Color(255f / 255f, 156f / 255f, 11f / 255f);
                bg.color = new Color(255f / 255f, 156f / 255f, 11f / 255f);
                break;
            default:
                break;
        }
    }
}
