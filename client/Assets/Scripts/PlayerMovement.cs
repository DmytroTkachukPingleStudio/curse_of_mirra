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
        public long aoe_x;
        public long aoe_y;
    }

    public enum PlayerAction
    {
        Nothing = 0,
        Attacking = 1,
        AttackingAOE = 2,
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
            SocketConnectionManager.Instance.gamePlayers != null
            && SocketConnectionManager.Instance.players.Count > 0
            && SocketConnectionManager.Instance.gamePlayers.Count > 0
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
            GameObject player = SocketConnectionManager.Instance.players[playerUpdate.player_id];
            player.transform.position =
                new Vector3(
                    playerUpdate.x / 10f - 50.0f,
                    player
                        .transform
                        .position
                        .y,
                    playerUpdate.y / 10f + 50.0f
                );
            Health healthComponent = player.GetComponent<Health>();
            healthComponent.SetHealth(playerUpdate.health);

            bool isAttacking = playerUpdate.action == PlayerAction.Attacking;
            player.GetComponent<AttackController>().SwordAttack(isAttacking);
            if (isAttacking)
            {
                print("attack");
            }
            //if dead remove the player from the scene
            if (healthComponent.CurrentHealth <= 0)
            {
                healthComponent.Model.gameObject.SetActive(false);
            }
            bool isAttackingAOE = playerUpdate.action == PlayerAction.AttackingAOE;
            if (isAttackingAOE){
                print(playerUpdate.aoe_x  / 10f - 50.0f);
                print(playerUpdate.aoe_y  / 10f + 50.0f);
            }
            
            SocketConnectionManager.Instance.players[playerUpdate.player_id]
                .GetComponent<AttackController>()
                .SwordAttack(isAttacking);
        }
    }

    void UpdatePlayerActions()
    {
        List<Player> gamePlayers = SocketConnectionManager.Instance.gamePlayers;
        for (int i = 0; i < SocketConnectionManager.Instance.players.Count; i++)
        {
            var new_position = gamePlayers[i].Position;
            var aoe_position = gamePlayers[i].AoePosition;
            playerUpdates.Enqueue(
                new PlayerUpdate
                {
                    x = (long)new_position.Y,
                    y = -((long)new_position.X),
                    player_id = i,
                    health = gamePlayers[i].Health,
                    action = (PlayerAction)gamePlayers[i].Action,
                    aoe_x = (long)aoe_position.Y,
                    aoe_y = -((long)aoe_position.X),
                }
            );
            if (gamePlayers[i].Health == 0)
            {
                print(SocketConnectionManager.instance.players[i + 1].name);
                SocketConnectionManager.instance.players[i + 1].SetActive(false);
            }
        }
    }
}
