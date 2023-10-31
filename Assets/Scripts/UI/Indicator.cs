using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    //[SerializeField] private Transform _target;

    //private void Update()
    //{
    //    Vector3 direction = _target.position - transform.position;
    //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    //}

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _target;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private float indicatorDistance;
    [SerializeField] private float appearanceDistance;

    private void Update()
    {
        float distance = Vector3.Distance(_player.position, _target.position);
        if(Mathf.Abs(distance) > appearanceDistance)
        {
            Debug.Log(distance);
            _arrow.SetActive(true);
            Vector3 directionToTarget = (_target.position - _player.position).normalized;
            _indicator.transform.position = _player.position + directionToTarget * indicatorDistance;
            //_indicator.transform.position = new Vector3(_indicator.transform.position.x, 2, _indicator.gameObject.transform.position.z);
            _indicator.transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
        else
        {
            _arrow.SetActive(false);
        }
    }

}
