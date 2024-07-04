using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public void SendJoystickValues(float x, float y)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (
            ShouldSendMovement(
                x,
                y,
                GameServerConnectionManager.Instance.playerMovement.lastXSent,
                GameServerConnectionManager.Instance.playerMovement.lastYSent,
                timestamp
            )
        )
        {
            var valuesToSend = new Direction { X = x, Y = y };

            GameServerConnectionManager.Instance.SendMove(x, y, timestamp);
            GameServerConnectionManager.Instance.playerMovement.SetLastSentInputs(x, y);
        }
    }

    bool ShouldSendMovement(float x, float y, float lastXSent, float lastYSent, long timestamp)
    {
        float movementThreshold = 1f;
        //Fetch the first GameObject's position
        Vector2 currentDirection = new Vector2(x, y);
        //Fetch the second GameObject's position
        Vector2 lastDirection = new Vector2(lastXSent, lastYSent);
        //Find the angle for the two Vectors
        float angleBetweenDirections = Vector2.Angle(currentDirection, lastDirection);

        bool movedFromStatic = (lastXSent == 0 && lastYSent == 0 && (x != 0 || y != 0));
        bool stoppedMoving = (x == 0 && y == 0 && (lastXSent != 0 || lastYSent != 0));
        bool changedDirection = (angleBetweenDirections > movementThreshold);
        bool movedFromPool = false;
        
        // This is a placeholder to reconciliate your position with server's when being pulled
        // by Valtimer's singularity. We should implement this mechanic in the front so we can
        // change our position smoothly by singularity's effect.
        // Issue: https://github.com/lambdaclass/champions_of_mirra/issues/1891
        Player selfPlayer = Utils.GetGamePlayer(GameServerConnectionManager.Instance.playerId).Player;
        foreach (var effect in selfPlayer.Effects)
        {
            if (effect.Name == "singularity")
            {
                movedFromPool = true;
                break;
            }
        }

        Vector2 movementDirection = new Vector2(x, y);
        movementDirection.Normalize();

        GameServerConnectionManager
            .Instance
            .playerMovement
            .AddMovement(
                new Direction { X = movementDirection.x, Y = movementDirection.y },
                (movedFromStatic || stoppedMoving || changedDirection || movedFromPool)
                    ? timestamp
                    : GameServerConnectionManager.Instance.playerMovement.currentTimestamp
            );

        // Here we can add a validaion to check if
        // the movement is significant enough to be sent to the server
        return (movedFromStatic || stoppedMoving || changedDirection || movedFromPool);
    }

    public (float, float) SendAction(bool forceSend = false)
    {
        float x = 0;
        float y = 0;
        if (Input.GetKey(KeyCode.W))
        {
            y += 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x += -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            y += -1f;
        }

        SendJoystickValues(x, y);

        return (x, y);
    }

    public bool KeysPressed()
    {
        return Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.S);
    }

    public bool JoytickUsed(float x, float y)
    {
        return x != 0 || y != 0;
    }
}
