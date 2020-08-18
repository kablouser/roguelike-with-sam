using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    public interface ITurnable
    {
        bool IsTurnOver();
        void StartTurn();
        void EndTurn();
    }

    public ITurnable player;
    public List<ITurnable> enemies;
    public List<ITurnable> enemiesDiedThisTurn;

    public void EndPlayerTurn()
    {
        player.EndTurn();
        //move all enemies
        StartCoroutine(MoveEnemies());
    }

    private IEnumerator MoveEnemies()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].StartTurn();
            yield return new WaitUntil(() => enemies[i].IsTurnOver() == false);
            enemies[i].EndTurn();
        }

        //remove dead enemies from list
        for(int i = 0; i < enemiesDiedThisTurn.Count; i++)
        {
            enemies.Remove(enemiesDiedThisTurn[i]);
        }

        //start player turn
        player.StartTurn();
    }
}
