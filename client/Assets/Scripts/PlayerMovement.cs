using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement
{
    public struct Movement
    {
        public Position position;
        public Direction direction;
        public float speed;
        public long timestamp;
        public long deltaTime;
        public bool reconciliated;
    }

    public List<Movement> movements = new List<Movement>();

    public Entity player = new Entity();
    public Direction orientation = new Direction();

    public long lastTimestamp = 0;
    public long currentTimestamp = 0;

    public float mapRadius;
    public bool didFirstMovement = false;
    public float lastXSent = 0;
    public float lastYSent = 0;

    public struct ServerInfo
    {
        public Entity player;
        public long timestamp;
    }

    public ServerInfo gameState;

    public void MovePlayer()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        long deltaTime = now - lastTimestamp;

        player.Position.X += player.Direction.X * player.Speed * deltaTime;
        player.Position.Y += player.Direction.Y * player.Speed * deltaTime;
        if (player.Direction.X != 0 || player.Direction.Y != 0)
        {
            this.orientation.X = player.Direction.X;
            this.orientation.Y = player.Direction.Y;
        }

        ClampIfOutOfMap();
        processCollisions();

        lastTimestamp = now;

        Movement movement = new Movement
        {
            position = new Position { X = player.Position.X, Y = player.Position.Y },
            direction = new Direction { X = player.Direction.X, Y = player.Direction.Y },
            speed = player.Speed,
            timestamp = currentTimestamp,
            deltaTime = deltaTime,
            reconciliated = false,
        };
        movements.Add(movement);
    }

    public void ReconciliatePlayer(float reconciliationDistance)
    {
        int index = 0;
        if (movements[index].reconciliated)
        {
            return;
        }

        Movement toModifyMovement = movements[index];
        toModifyMovement.reconciliated = true;
        movements[index] = toModifyMovement;

        float distance = Vector3.Distance(
            new Vector3(movements[index].position.X, 0, movements[index].position.Y),
            new Vector3(gameState.player.Position.X, 0, gameState.player.Position.Y)
        );

        if (distance > reconciliationDistance)
        {
            player.Position = gameState.player.Position;
            for (int i = index + 1; i < movements.Count; i++)
            {
                Movement movement = movements[i];
                long now = lastTimestamp;
                if (i < movements.Count - 1)
                {
                    now = movements[i + 1].timestamp;
                }

                player.Position.X += movement.direction.X * movement.speed * movement.deltaTime;
                player.Position.Y += movement.direction.Y * movement.speed * movement.deltaTime;
            }
        }
    }

    public void AddMovement(Direction direction, long timestamp)
    {
        if (player.Player.ForcedMovement)
        {
            return;
        }
        player.Direction.X = direction.X;
        player.Direction.Y = direction.Y;
        currentTimestamp = timestamp;
    }

    public void AddForcedMovement(Direction direction)
    {
        player.Direction.X = direction.X;
        player.Direction.Y = direction.Y;
    }

    public void SetForcedMovement(bool forcedMovement, Direction direction)
    {
        this.player.Player.ForcedMovement = forcedMovement;
        if (forcedMovement)
        {
            AddForcedMovement(direction);
        }
    }

    public void SetPlayer(Entity player)
    {
        this.player = player;
    }

    public void SetSpeed(float speed)
    {
        this.player.Speed = speed;
    }

    public void StopMovement()
    {
        this.didFirstMovement = true;
        this.lastXSent = 0f;
        this.lastYSent = 0f;
        this.player.Direction.X = 0f;
        this.player.Direction.Y = 0f;
    }

    public void SetGameState(Entity serverPlayer, long timestamp)
    {
        movements.RemoveAll(movement => movement.timestamp < timestamp);
        this.gameState.player = serverPlayer;
        this.gameState.timestamp = timestamp;
    }

    public void SetPlayerPosition(float x, float y)
    {
        this.player.Position.X = x;
        this.player.Position.Y = y;
    }

    public void SetLastSentInputs(float lastXSent, float lastYSent)
    {
        if (didFirstMovement)
        {
            this.lastXSent = lastXSent;
            this.lastYSent = lastYSent;
        }
    }

    private void ClampIfOutOfMap()
    {
        Vector3 mapCenterPosition = new Vector3(0, 0, 0);
        Vector3 playerPositionVector = new Vector3(player.Position.X, 0, player.Position.Y);
        float playerDistanceFromMapCenter =
            Vector3.Distance(playerPositionVector, mapCenterPosition) + player.Radius;

        if (playerDistanceFromMapCenter > mapRadius)
        {
            Vector3 fromOriginToObject = playerPositionVector - mapCenterPosition;
            fromOriginToObject *= mapRadius / playerDistanceFromMapCenter;
            Vector3 newPosition = mapCenterPosition + fromOriginToObject;

            player.Position.X = newPosition.x;
            player.Position.Y = newPosition.z;
        }
    }

    private void processCollisions()
    {
        foreach (Entity obstacle in GameServerConnectionManager.Instance.obstacles)
        {
            switch (obstacle.Shape)
            {
                case "circle":
                    float distance = PositionUtils.DistanceToPosition(
                        player.Position,
                        obstacle.Position
                    );
                    if (distance <= player.Radius + obstacle.Radius)
                    {
                        Position normalized_direction = PositionUtils.NormalizedPosition(
                            PositionUtils.SubPosition(player.Position, obstacle.Position)
                        );
                        player.Position.X =
                            obstacle.Position.X
                            + (normalized_direction.X * player.Radius)
                            + (normalized_direction.X * obstacle.Radius);
                        player.Position.Y =
                            obstacle.Position.Y
                            + (normalized_direction.Y * player.Radius)
                            + (normalized_direction.Y * obstacle.Radius);
                    }

                    break;
                case "polygon":
                    (bool collided, Position direction, float depth) = SAT.IntersectCirclePolygon(
                        player,
                        obstacle
                    );

                    if (collided)
                    {
                        player.Position.X += direction.X * depth;
                        player.Position.Y += direction.Y * depth;
                    }

                    break;
                default:
                    Debug.Log("Missing obstacle shape");
                    break;
            }
        }
    }
}
