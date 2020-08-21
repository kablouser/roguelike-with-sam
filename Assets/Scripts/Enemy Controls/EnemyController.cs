using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public const int maxAttempts = 999;
    public bool IsTurnOver { get; private set; }

    public EnemyComponents character;
    public bool tryMove = true;

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
        }
        while (character.mover.Move(moveDirection, () => IsTurnOver = true) == false
            && attempts < maxAttempts);
    }
}
