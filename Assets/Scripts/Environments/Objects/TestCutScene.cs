using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.InputSystem;

public class TestCutScene : MonoBehaviour
{
    private PlayableDirector _pd;
    [SerializeField] private TimelineAsset _ta;
    [SerializeField] private GameObject _cylinder;
    void Start()
    {
        _pd = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        /*if (Input.GetKey(KeyCode.M))
        {
            _cylinder.SetActive(true);
            _pd.Play(_ta);
            StartCoroutine(LightCylinder());
        }*/
    }

    private IEnumerator LightCylinder()
    {
        yield return new WaitForSecondsRealtime(10f);
        _cylinder.SetActive(false);
    }

    void OnNavigate(InputValue value)
    {
        _cylinder.SetActive(true);
        _pd.Play(_ta);
        StartCoroutine(LightCylinder());
    }
}
