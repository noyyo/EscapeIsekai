using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructor : CustomSingleton<Instructor>
{
    int rank = 0;
    private void GameFail()
    {
        MinigameManager.Instance.ChangeSuccess -= GameFailorSuc;
    }
    private void GameSuc()
    {
        if(rank <= 5 )
        {
            GameManager.Instance.Player.GetComponent<Player>().Playerconditions.Power += 2;
        }
        rank++;
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
