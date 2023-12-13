using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChangeBGMPos : MonoBehaviour
{
    [SerializeField] private string enterClipName;
    [Tooltip("���� �Է��� ���ٸ� �ڵ����� �⺻ ������� ���۵˴ϴ�.")][SerializeField] private string exitClipName;
    private SoundManager soundManager;
    private BoxCollider boxCollider;
    private bool isUseExitClipName;
    private void Awake()
    {
        soundManager = SoundManager.Instance;
        boxCollider = GetComponent<BoxCollider>();
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
