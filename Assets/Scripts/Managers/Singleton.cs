using UnityEngine;

public class CustomSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed
    private static bool _isShutdown = false;
    private static T _instance;


    public static T Instance
    {
        get
        {
            if (_isShutdown)
            {
                Debug.Log("Error");
                return null;
            }

            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject { name = typeof(T).ToString() };
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    private void OnApplicationQuit()
    {
        _isShutdown = true;
    }


    private void OnDestroy()
    {
        _isShutdown = true;
    }
}
