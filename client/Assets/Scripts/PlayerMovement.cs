using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] MMTouchJoystick joystickL;
    public Queue<PlayerUpdate> playerUpdates = new Queue<PlayerUpdate>();
    public Direction nextAttackDirection;
    public bool isAttacking = false;

    public struct PlayerUpdate
    {
        public long x;
        public long y;
        public int player_id;
        public long health;
        public PlayerAction action;
    }

    public enum PlayerAction
    {
        Nothing = 0,
        Attacking = 1,
    }

    void Start()
    {
        // Send the player's action every 30 ms approximately.
        float tickRate = 1f / 30f;
        InvokeRepeating("SendAction", tickRate, tickRate);
    }

    void Update()
    {
        if (
            SocketConnectionManager.Instance.gameUpdate != null
            && SocketConnectionManager.Instance.players.Count > 0
            && SocketConnectionManager.Instance.gameUpdate.Players.Count > 0
        )
        {
            UpdatePlayerActions();
            checkForAttacks();
            ExecutePlayerAction();
        }
    }

    public void SendAction()
    {
        var inputFromPhysicalJoystick = Input.GetJoystickNames().Length > 0;
        var inputFromVirtualJoystick = joystickL is not null;
        if (inputFromPhysicalJoystick)
        {
            var hInput = Input.GetAxis("Horizontal");
            var vInput = Input.GetAxis("Vertical");
            GetComponent<PlayerControls>().SendJoystickValues(hInput, -vInput);
        }
        else if (inputFromVirtualJoystick && joystickL.RawValue.x != 0 || joystickL.RawValue.y != 0)
        {
            GetComponent<PlayerControls>().SendJoystickValues(joystickL.RawValue.x, joystickL.RawValue.y);
        }
        else
        {
            GetComponent<PlayerControls>().SendAction();
        }
        sendAttack();
    }

    void sendAttack()
    {
        if (isAttacking)
        {
            ClientAction clientAction = new ClientAction { Action = Action.Attack, Direction = nextAttackDirection };
            SocketConnectionManager.Instance.SendAction(clientAction);
            isAttacking = false;
        }
    }

    void checkForAttacks()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            nextAttackDirection = Direction.Down;
            isAttacking = true;

        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            nextAttackDirection = Direction.Up;
            isAttacking = true;

        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            nextAttackDirection = Direction.Right;
            isAttacking = true;

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            nextAttackDirection = Direction.Left;
            isAttacking = true;

        }
        // Hardcoded dual sense square button
        if (Input.GetKeyDown("joystick 1 button 0"))
        {
            nextAttackDirection = Direction.Up;
            isAttacking = true;
        }
    }

    void ExecutePlayerAction()
    {
        while (playerUpdates.TryDequeue(out var playerUpdate))
        {
            SocketConnectionManager.Instance.players[playerUpdate.player_id].transform.position =
                new Vector3(
                    playerUpdate.x / 10f - 50.0f,
                    SocketConnectionManager.Instance.players[playerUpdate.player_id]
                        .transform
                        .position
                        .y,
                    playerUpdate.y / 10f + 50.0f
                );
            Health healthComponent = SocketConnectionManager.Instance.players[
                playerUpdate.player_id
            ].GetComponent<Health>();
            healthComponent.SetHealth(playerUpdate.health);

            bool isAttacking = playerUpdate.action == PlayerAction.Attacking;
            SocketConnectionManager.Instance.players[playerUpdate.player_id]
                .GetComponent<AttackController>()
                .SwordAttack(isAttacking);
        }
    }

    void UpdatePlayerActions()
    {
        GameStateUpdate game_update = SocketConnectionManager.Instance.gameUpdate;
        for (int i = 0; i < SocketConnectionManager.Instance.players.Count; i++)
        {
            var new_position = game_update.Players[i].Position;
            playerUpdates.Enqueue(
                new PlayerUpdate
                {
                    x = (long)new_position.Y,
                    y = -((long)new_position.X),
                    player_id = i,
                    health = game_update.Players[i].Health,
                    action = (PlayerAction)game_update.Players[i].Action,
                }
            );
            if (game_update.Players[i].Health == 0)
            {
                print(SocketConnectionManager.instance.players[i + 1].name);
                SocketConnectionManager.instance.players[i + 1].SetActive(false);
            }
        }
    }
}
