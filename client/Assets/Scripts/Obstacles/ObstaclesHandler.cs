using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstaclesHandler : MonoBehaviour
{
    Dictionary<string, string> obstacles = new Dictionary<string, string>();

    [SerializeField]
    List<GameObject> mapSpikes;

    private Dictionary<string, GameObject> mapSpikesDictionary;

    void Start()
    {
        mapSpikesDictionary = mapSpikes.ToDictionary(spikes => spikes.name);
    }

    public void GenerateObstacles(List<Entity> obstacles)
    {
        obstacles.ForEach(obstacle => GenerateObstacle(obstacle));
    }

    private void GenerateObstacle(Entity obstacle)
    {
        obstacles.Add(obstacle.Name, obstacle.Obstacle.Status);
    }

    public void UpdateObstacles(List<Entity> obstacles)
    {
        foreach(Entity obstacle in obstacles)
        {
            UpdateObstacle(obstacle);
        }
    }

    private void UpdateObstacle(Entity obstacle)
    {
        if(this.obstacles[obstacle.Name] != obstacle.Obstacle.Status)
        {
            if(mapSpikesDictionary.TryGetValue(obstacle.Name, out GameObject inGameSpikes))
            { 
                if(obstacle.Obstacle.Status == "transitioning" && this.obstacles[obstacle.Name] == "raised")
                {
                    inGameSpikes.GetComponent<SpikesContainer>().ToggleSpikes(false);
                }
                else if(obstacle.Obstacle.Status == "raised")
                {
                    inGameSpikes.GetComponent<SpikesContainer>().ToggleSpikes(true);
                }
                else if(obstacle.Obstacle.Status == "transitioning" && this.obstacles[obstacle.Name] == "underground")
                {
                    inGameSpikes.GetComponent<SpikesContainer>().TransitionIndicator();
                }

                this.obstacles[obstacle.Name] = obstacle.Obstacle.Status;
            }
            else
            {
                Debug.LogWarning($"Obstacle not found on the client: {obstacle.Name}");
            }
        }
    }
}
