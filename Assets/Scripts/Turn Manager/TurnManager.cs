using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    private PlayerComponents player;
    private List<EnemyComponents> enemies;
    private List<EnemyComponents> deadEnemies;

    public void RegisterPlayer(PlayerComponents player) =>
        this.player = player;

    public void RegisterEnemy(EnemyComponents enemy) =>
        enemies.Add(enemy);

    public void ReportDead(CharacterComponents character)
    {
        if (character is EnemyComponents enemy)
        {
            deadEnemies.Add(enemy);
        }
        else if (character is PlayerComponents player)
        {
            //The player died!
        }
    }

    public void EndPlayerTurn()
    {
        StartCoroutine(MoveEnemies());
    }

    public override void Awake()
    {
        base.Awake();
        if (Current == this)
        {
            enemies = new List<EnemyComponents>();
            deadEnemies = new List<EnemyComponents>();
        }
    }

    private void Start()
    {
        player.playerController.UnlockControl();
    }

    private IEnumerator MoveEnemies()
    {
        for (int i = 0; i < deadEnemies.Count; i++)
            enemies.Remove(deadEnemies[i]);
        deadEnemies.Clear();

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemySheet = enemies[i].characterSheet;
            enemySheet.NewTurn();
            yield return new WaitUntil(() => enemySheet.IsAnimationOver);

            var enemyController = enemies[i].enemyController;
            enemyController.StartTurn();
            yield return new WaitWhile(() => enemyController.IsTurnOver == false);
        }

        for(int i = 0; i < deadEnemies.Count; i++)
            enemies.Remove(deadEnemies[i]);
        deadEnemies.Clear();

        //start player turn
        player.characterSheet.NewTurn();
        player.playerController.UnlockControl();
    }
}
