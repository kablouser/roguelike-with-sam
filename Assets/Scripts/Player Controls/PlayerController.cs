using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerComponents playerComponents;
    private Vector2Int moveInput;
    private TurnManager turnManager;

    private void Awake()
    {
        turnManager = TurnManager.Current;
    }

    private void Update()
    {
        Vector2Int previousInput = moveInput;
        moveInput.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        moveInput.y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        if(previousInput.x == 0 && moveInput.x != 0)
        {
            playerComponents.mover.Move(new Vector3Int(moveInput.x, 0, 0));
            turnManager.EndPlayerTurn();
        }   
        else if(previousInput.y == 0 && moveInput.y != 0)
        {
            playerComponents.mover.Move(new Vector3Int(0, moveInput.y, 0));
            turnManager.EndPlayerTurn();
        }
    }
}
