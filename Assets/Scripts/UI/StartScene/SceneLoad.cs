using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoad : MonoBehaviour
{
    public void LoadScene()
    {
        loadingSceneController.Instance.LoadScene("QuestUITest");
    }
}
