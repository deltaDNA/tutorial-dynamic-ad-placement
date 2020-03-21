using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    INIT,
    READY,
    PLAYING,
    DEAD
}


[System.Serializable]
public class Level
{

    public int food;
    public int poison;
    public int cost;
    public int reward;
    public int timelimit;


}

[System.Serializable]
public class Game
{
    public int currentLevel;
    public int maxLevel;


    public List<Level> levels;
    public GameState gameState;

    public Game()
    {
        levels = new List<Level>();
        gameState = GameState.INIT;

    }

    public void LoadLevels()
    {
        var jsonLevels = Resources.Load<TextAsset>("Levels");
        this.levels = JsonUtility.FromJson<Game>(jsonLevels.text).levels;


        if (levels.Count > 0)
        {
            Debug.Log("Loaded " + levels.Count + " levls. Status : READY");
            gameState = GameState.READY;
        }

    }
}