using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructor : CustomSingleton<Instructor>
{
    int rank = 0;
    GameObject effect;
    private void Awake()
    {
        effect = Instantiate(Resources.Load<GameObject>("Prefabs/Npc/RankUpEffect"));
    }
    private void GameFail()
    {
        MinigameManager.Instance.ChangeSuccess -= GameFailorSuc;
    }
    private void GameSuc()
    {
        if(rank <= 5 )
        {
            effect.transform.position = GameManager.Instance.Player.transform.position;
            effect.GetComponent<ParticleSystem>().Play();
            GameManager.Instance.Player.GetComponent<Player>().Playerconditions.Power += 2;
            rank++;
        }
        MinigameManager.Instance.ChangeSuccess -= GameFailorSuc;
    }
    public void GameFailorSuc(int val)
    {
        if (val == 1)
            GameSuc();
        if (val == -1)
            GameFail();
    }
}
