using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DeltaDNA; 

public class GameManager : MonoBehaviour
{
    // HUD Elements
    public HudManager hud;
    public Text txtStart;
    public Text txtMessage;
    public Text txtCost;
    public Button bttnStart;

    // Start Button Size and Color
    private Color sourceColor;
    private Color targetColor;
    private Vector3 initialScale;
    private Vector3 finalScale;

    // Game Arena Walls
    private Transform rBorder;
    private Transform lBorder;
    private Transform tBorder;
    private Transform bBorder;

    public Game game = new Game();
    public GameObject food;
    private List<GameObject> foodList = new List<GameObject>();

    // Player Snake
    public GameObject snakePrefab;
    public GameObject playerSnake;
    public GameObject bodyPrefab;
    public List<GameObject> bodyParts = new List<GameObject>();
    private Transform curBodyPart;
    private Transform prevBodyPart;
    public int beginSize = 4;
    public float minDist = 10f;

    // Input Movement
    private Vector2 vector = Vector2.up;
    private bool vertical = false;
    private bool horizontal = true;       
    public float speed = 20.0f;
    private float currentSpeed; 
    public float acceleration = 0.3f;




    // Start is called before the first frame update
    void Start()
    {
        // These are for pulsing the start button
        initialScale = transform.localScale;
        finalScale = new Vector3(initialScale.x + 0.08f, initialScale.y + 0.08f, initialScale.z);
        sourceColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        targetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        // Find Arena Walls
        rBorder = GameObject.Find("border-right").transform;
        lBorder = GameObject.Find("border-left").transform;
        tBorder = GameObject.Find("border-top").transform;
        bBorder = GameObject.Find("border-bottom").transform;

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
        else if (game.gameState == GameState.PLAYING)
        {
            MoveSnake();
        }
        else if (game.gameState == GameState.NEXTLEVEL)
        {
            // Record MissionCompleted Event
            GameEvent missionCompleted = new GameEvent("missionCompleted")
                .AddParam("missionID", game.currentLevel.ToString())
                .AddParam("missionName", string.Format("Mission {0}", game.currentLevel))
                .AddParam("foodTarget", game.levels[game.currentLevel].food);

            DDNA.Instance.RecordEvent(missionCompleted);

            game.currentLevel++; 
            if (game.currentLevel == game.levels.Count)
            {
                game.currentLevel = 0;
            }
            StartLevel(game.currentLevel);
        }
        else if(game.gameState == GameState.DEAD)
        {
            // Record MissionFailed event
            GameEvent missionFailed = new GameEvent("missionFailed")
                .AddParam("missionID", game.currentLevel.ToString())
                .AddParam("missionName", string.Format("Mission {0}", game.currentLevel))
                .AddParam("foodRemaining", foodList.Count);

            DDNA.Instance.RecordEvent(missionFailed);

            
            ResetGame();

            txtStart.gameObject.SetActive(true);
            bttnStart.gameObject.SetActive(true);            
            txtCost.gameObject.SetActive(true);

            game.gameState = GameState.READY; 
        }
    }


    public void ResetGame()
    {
        game.gameState = GameState.INIT; 
        
        for (int i = foodList.Count-1; i >=0; i--)
        {
            GameObject o = foodList[i];
            foodList.RemoveAt(i);
            Destroy(o);
        }
        
        for (int i = bodyParts.Count-1; i>=0; i--)
        {
            GameObject o = bodyParts[i];
            bodyParts.RemoveAt(i);
            Destroy(o);
        }

        Destroy(playerSnake);
    }
    public void NewGame()
    {
        Debug.Log("New Game");
        currentSpeed = speed;
        game.currentLevel = 0;
        txtStart.gameObject.SetActive(false);
        bttnStart.gameObject.SetActive(false);
        txtMessage.gameObject.SetActive(false);
        txtCost.gameObject.SetActive(false);

        StartLevel(game.currentLevel);
    }

    public void StartLevel(int LevelNo)
    {
        SpawnFood(game.levels[game.currentLevel].food);
        if (playerSnake == null)
        {
            SpawnSnake();
        }
        hud.UpdateHud(0, game.currentLevel + 1, foodList.Count);

        game.gameState = GameState.PLAYING;


        // Record MissionStarted event
        GameEvent missionStarted = new GameEvent("missionStarted")
            .AddParam("missionID", game.currentLevel.ToString())
            .AddParam("missionName", string.Format("Mission {0}", game.currentLevel))
            .AddParam("foodTarget", game.levels[game.currentLevel].food); 

        DDNA.Instance.RecordEvent(missionStarted);
            
    }


    public void SpawnFood(int num)
    {
        for (int i = 0; i < num; i++)
        {
            float x = (float)Random.Range(lBorder.position.x + 1, rBorder.position.x - 1);
            float y = (float)Random.Range(bBorder.position.y + 1, tBorder.position.y - 1);
            GameObject f = Instantiate(food, new Vector3(x, y, -1), Quaternion.identity);
            foodList.Add(f);

        }
    }

    public void SpawnSnake()
    {

        playerSnake = Instantiate(snakePrefab, new Vector3(0f, 0f, -1f), Quaternion.identity);
        for (int i = 0; i < beginSize; i++)
        {
            AddBodyPart();
        }

    }
    public void AddBodyPart()
    {
        Transform newPart = (Instantiate(bodyPrefab,
            playerSnake.transform.position,
            playerSnake.transform.rotation) as GameObject).transform;

        if (bodyParts.Count > 0)
        {

            newPart.name = "Tail";
        }
        bodyParts.Add(newPart.gameObject);
    }


    public void MoveSnake()
    {
        float s = currentSpeed;

        // Change Direction
        if (Input.GetKey(KeyCode.RightArrow) && horizontal)
        {
            horizontal = false;
            vertical = true;
            vector = Vector3.right;
            //Debug.Log("Right");
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && horizontal)
        {
            horizontal = false;
            vertical = true;
            vector = Vector3.left;
            //Debug.Log("Left");
        }
        else if (Input.GetKey(KeyCode.UpArrow) && vertical)
        {
            horizontal = true;
            vertical = false;
            vector = Vector3.up;
            //Debug.Log("Up");
        }
        else if (Input.GetKey(KeyCode.DownArrow) && vertical)
        {
            horizontal = true;
            vertical = false;
            vector = Vector3.down;
            //Debug.Log("Down");
        }


        // Move Player Snake Head
        playerSnake.transform.Translate(vector * s * Time.smoothDeltaTime);

        if(IsSnakeOutsideArena())
        {
            game.gameState = GameState.DYING;

            StartCoroutine(DisplayMessage("GameOver", GameState.DEAD));
            
        }

        // Lerp BodyParts to follow Snake head
        for (int i = 0; i < bodyParts.Count; i++)
        {
            curBodyPart = bodyParts[i].transform;

            if (i == 0)
            {
                prevBodyPart = playerSnake.transform;
            }
            else
            {
                prevBodyPart = bodyParts[i - 1].transform;
            }

            float d = Vector3.Distance(prevBodyPart.position, curBodyPart.position);
            Vector3 newPos = prevBodyPart.position;
            float t = Time.deltaTime * d * minDist * s;

            if (t > 0.5f)
            { t = 0.5f; }
            curBodyPart.position = Vector3.Slerp(curBodyPart.position, newPos, t);
        }

    }

    private bool IsSnakeOutsideArena()
    {
        bool isOutside = false; 

        if (playerSnake.transform.position.x < lBorder.position.x || playerSnake.transform.position.x > rBorder.position.x
            || playerSnake.transform.position.y < bBorder.position.y || playerSnake.transform.position.y > tBorder.position.y)
        {
            isOutside = true;
        }

        return isOutside; 
    }


    public void EatFood(GameObject f)
    {
        Debug.Log("Munch");

        // Destroy Food Object
        foreach (GameObject o in foodList)
        {
            if (o==f)
            {
                foodList.Remove(o);
                Destroy(o);
                AddBodyPart();
                currentSpeed += acceleration;
                break; 
            }
        }
        
        // Update Hud
        hud.UpdateHud(0, game.currentLevel + 1, foodList.Count);

        // Check Level Complete
        if(foodList.Count == 0 )
        {
            game.gameState = GameState.LEVELCOMPLETE;
            StartCoroutine(DisplayMessage("Level Up", GameState.NEXTLEVEL));
        }

    }


    IEnumerator DisplayMessage(string message, GameState nextState)
    {
        txtMessage.text = message; 
        txtMessage.gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        
        txtMessage.gameObject.SetActive(false);

        game.gameState = nextState; 

    }



}
