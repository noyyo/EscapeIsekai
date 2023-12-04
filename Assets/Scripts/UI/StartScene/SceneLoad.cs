using UnityEngine;

public class SceneLoad : MonoBehaviour
{
    public void LoadScene()
    {
        loadingSceneController.Instance.LoadScene("MainScene");
    }
}
