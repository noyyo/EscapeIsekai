using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChangeBGMPos : MonoBehaviour
{
    [SerializeField] private string enterClipName;
    [Tooltip("���� �Է��� ���ٸ� �ڵ����� �⺻ ������� ���۵˴ϴ�.")][SerializeField] private string exitClipName;
    private BoxCollider boxCollider;
    private SoundManager soundManager;
    private bool isUseExitClipName;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        soundManager = SoundManager.Instance;
        boxCollider.isTrigger = true;
        if(exitClipName != null)
            isUseExitClipName = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagsAndLayers.PlayerTag))
        {
            soundManager.BGMStop();
            soundManager.ChangeBGM(enterClipName);
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagsAndLayers.PlayerTag))
        {
            soundManager.BGMStop();
            if (isUseExitClipName)
                soundManager.ChangeBGM(exitClipName);
            else
                soundManager.PlayDefaultBGM();
        }
    }
}