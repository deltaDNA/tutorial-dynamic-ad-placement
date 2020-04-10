using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaDNA;
using UnityEngine.Advertisements;

public class Tutorial : MonoBehaviour, IUnityAdsListener
{
    public GameManager gameManager;
    
    // Advertising
    string gameID = "3521373";
    bool testMode = false;
    string placementId = "dynamic_placement";

    private int adRewardValue;

    // Start is called before the first frame update
    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameID, testMode);


        // Hook up callback to fire when DDNA SDK has received session config info, including Event Triggered campaigns.
        DDNA.Instance.NotifyOnSessionConfigured(true);
        DDNA.Instance.OnSessionConfigured += (bool cachedConfig) => GetGameConfig(cachedConfig);


        // Allow multiple game parameter actions callbacks from a single event trigger        
        DDNA.Instance.Settings.MultipleActionsForEventTriggerEnabled = true;

        //Register default handlers for event triggered campaigns. These will be candidates for handling ANY Event-Triggered Campaigns. 
        //Any handlers added to RegisterEvent() calls with the .Add method will be evaluated before these default handlers. 
        DDNA.Instance.Settings.DefaultImageMessageHandler =
            new ImageMessageHandler(DDNA.Instance, imageMessage => {
                // do something with the image message
                myImageMessageHandler(imageMessage);
            });
        DDNA.Instance.Settings.DefaultGameParameterHandler = new GameParametersHandler(gameParameters => {
            // do something with the game parameters
            myGameParameterHandler(gameParameters);
        });

        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.StartSDK();
    }


    // The callback indicating that the deltaDNA has downloaded its session configuration, including
    // Event Triggered Campaign actions and logic, is used to record a "sdkConfigured" event 
    // that can be used provision remotely configured parameters. 
    // i.e. deferring the game session config until it knows it has received any info it might need
    public void GetGameConfig(bool cachedConfig)
    {
        Debug.Log("Configuration Loaded, Cached =  " + cachedConfig.ToString());
        Debug.Log("Recording a sdkConfigured event for Event Triggered Campaign to react to");

        // Create an sdkConfigured event object
        var gameEvent = new GameEvent("sdkConfigured")
            .AddParam("clientVersion", DDNA.Instance.ClientVersion)
            .AddParam("userLevel",gameManager.game.currentLevel);

        // Record sdkConfigured event and run default response hander
        DDNA.Instance.RecordEvent(gameEvent).Run();
    }



    private void myImageMessageHandler(ImageMessage imageMessage)
    {
        // Add a handler for the 'dismiss' action.
        imageMessage.OnDismiss += (ImageMessage.EventArgs obj) => {
            Debug.Log("Image Message dismissed by " + obj.ID);

            // NB : parameters not processed if player dismisses action
        };

        // Add a handler for the 'action' action.
        imageMessage.OnAction += (ImageMessage.EventArgs obj) => {
            Debug.Log("Image Message actioned by " + obj.ID + " with command " + obj.ActionValue);

            // Process parameters on image message if player triggers image message action
            if (imageMessage.Parameters != null) myGameParameterHandler(imageMessage.Parameters);
        };
        
        imageMessage.OnDidReceiveResources += () =>
        {
            Debug.Log("Received Image Message Assets");
        };


        // the image message is already cached and prepared so it will show instantly
        imageMessage.Show();
    }

    private void myGameParameterHandler(Dictionary<string, object> gameParameters)
    {
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        // Parameters to remotely control Ads Received      
        // ------------------------------------------------------------------------------------------
        Debug.Log("Received game parameters from event trigger: " + DeltaDNA.MiniJSON.Json.Serialize(gameParameters));
        if (gameParameters.ContainsKey("adShow"))
        {
            if (System.Convert.ToInt32(gameParameters["adShow"]) == 1)
            {
                Advertisement.Show(placementId); // <<<< This line of code was moved to turn fixed placements into dynamic ones <<<<

                if (gameParameters.ContainsKey("adRewardValue"))
                {
                    adRewardValue = System.Convert.ToInt32(gameParameters["adRewardValue"]);
                }
            }
        }
        else if (gameParameters.ContainsKey("realCurrencyAmount"))
        {
            gameManager.ReceiveCurrency(System.Convert.ToInt32(gameParameters["realCurrencyAmount"]));
        }
        // --------------------------------------------------------------------------------------------

    }


    // Unity Ads Listeners
    public void OnUnityAdsReady(string placementId)

    {

    }

    public void OnUnityAdsDidStart(string placementId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        bool isAdFinished = false; 

        if(showResult == ShowResult.Finished)
        {
            isAdFinished = true;
            gameManager.ReceiveCurrency(adRewardValue);
            
        }

        GameEvent adEvent = new GameEvent("adImpression")
                .AddParam("adCompletionStatus", isAdFinished ? "COMPLETED" : "INCOMPLETE")
                .AddParam("adProvider", "Unity Ads")
                .AddParam("placementType", "REWARDED AD")
                .AddParam("placementId", placementId)
                .AddParam("placementType", placementId);

        DDNA.Instance.RecordEvent(adEvent).Run();
    }



}
