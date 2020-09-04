using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public const int maxAttempts = 999;
    public bool IsTurnOver { get; private set; }
    public Vector3Int GetPlayerPosition => PlayerController.Current.character.mover.GetPosition;

    public EnemyComponents character;
    public bool tryMove = true;

    public float idleWeight = 1.0f;
    public float aggroWeight = 1.0f;
    public float wanderWeight = 1.0f;

    private void Awake()
    {
        TurnManager.Current.RegisterEnemy(character);
    }

    public void StartTurn()
    {
        if(tryMove == false)
        {
            IsTurnOver = true;
            return;
        }

        IsTurnOver = false;
        float randomChoice = Random.Range(0, idleWeight + aggroWeight + wanderWeight);
        if(randomChoice < idleWeight)
        {
            //idle
            EndTurn();
        }
        else if(randomChoice < idleWeight + aggroWeight)
        {
            //aggro
            if (TryHitPlayer() == false)
            {
                //if it fails
                MoveRandomly();                
            }
        }
        else
        {
            //wander
            MoveRandomly();
        }
    }

    private bool TryHitPlayer()
    {
        Vector3Int playerDirection = GetPlayerPosition - character.mover.GetPosition;
        if(Mathf.Abs(playerDirection.x) <= 1 && Mathf.Abs(playerDirection.y) <= 1)
        {
            //attack range
            return character.combatant.Attack(new Vector2Int(playerDirection.x, playerDirection.y), EndTurn);
        }

        if (playerDirection.y == 0)
            //move in x
            return character.mover.Move(new Vector3Int(SnapValue(playerDirection.x), 0, 0), EndTurn);
        else if(playerDirection.x == 0)
            //move in y
            return character.mover.Move(new Vector3Int(0, SnapValue(playerDirection.y), 0), EndTurn);

        if (Mathf.Abs(playerDirection.x) < Mathf.Abs(playerDirection.y))
            //move in x
            return character.mover.Move(new Vector3Int(SnapValue(playerDirection.x), 0, 0), EndTurn);
        else
            //move in y
            return character.mover.Move(new Vector3Int(0, SnapValue(playerDirection.y), 0), EndTurn);
    }

    private void MoveRandomly()
    {
        Vector3Int moveDirection;
        int attempts = 0;
        do
        {
            //either -1 or 1
            int negativeOrOne = Random.value < .5f ? -1 : 1;

            moveDirection = new Vector3Int();
            if (Random.value < .5f)
                moveDirection.x = negativeOrOne;
            else
                moveDirection.y = negativeOrOne;

            attempts++;
            if (maxAttempts < attempts)
            {
                EndTurn();
                return;
            }
        }
        while (character.mover.Move(moveDirection, EndTurn) == false);
    }

    /// <summary>
    /// Snaps a value to -1 or 1, which ever is closer
    /// </summary>
    private int SnapValue(int value) => 0 < value ? 1 : -1;

    private void EndTurn() => IsTurnOver = true;
}
