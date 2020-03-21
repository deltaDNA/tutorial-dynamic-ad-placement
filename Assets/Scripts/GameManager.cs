using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // HUD Elements
    public Text txtStart;
    public Text txtGameOver;
    public Text txtCost;
    public Button bttnStart;

    // Start Button Size and Color
    private Color sourceColor;
    private Color targetColor;
    private Vector3 initialScale;
    private Vector3 finalScale;

    public Game game = new Game();
               

    // Start is called before the first frame update
    void Start()
    {
        // These are for pulsing the start button
        initialScale = transform.localScale;
        finalScale = new Vector3(initialScale.x + 0.08f, initialScale.y + 0.08f, initialScale.z);
        sourceColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        targetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        // Load Levels
        game.LoadLevels();

    }

    // Update is called once per frame
    void Update()
    {
        if (game.gameState == GameState.READY)
        {
            // Pulse the Start Button Size and Color
            bttnStart.image.color = Color.Lerp(sourceColor, targetColor, Mathf.PingPong(Time.time, 1.2f));
            bttnStart.transform.localScale = Vector3.Lerp(initialScale, finalScale, Mathf.PingPong(Time.time, 1.2f));
        }
    }

    public void NewGame()
    {
        Debug.Log("New Game");

        txtStart.gameObject.SetActive(false);
        bttnStart.gameObject.SetActive(false);
        txtGameOver.gameObject.SetActive(false);
        txtCost.gameObject.SetActive(false);

        StartLevel(game.currentLevel);

        
        
    }

    public void StartLevel(int LevelNo)
    {
        game.gameState = GameState.PLAYING;


    }


}
