using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaDNA;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Analytics;

public class Tutorial : MonoBehaviour
{
    public GameManager gameManager;
    
    // Properties
    [Header("Properties")]
    public bool isMoPubAdsEnabled = false;
    
    private int adRewardValue;


    string consentIdentifier;
    bool isOptInConsentRequired;


    // Start is called before the first frame update
    async void Start()
    {

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


        try
        {
            var options = new InitializationOptions();
            options.SetOption("Environment", "production");
            options.SetAnalyticsUserId(DDNA.Instance.UserID);

            await UnityServices.InitializeAsync(options);
            List<string> consentIdentifiers = await Events.CheckForRequiredConsents();
            if (consentIdentifiers.Count > 0)
            {
                consentIdentifier = consentIdentifiers[0];
                isOptInConsentRequired = consentIdentifier == "pipl";

                if (isOptInConsentRequired)
                    CheckUserConsent();
            }
        }
        catch (ConsentCheckException e)
        {
            Debug.Log("Something went wring with GeoIP consent check : " + e.Reason);
        }

    }

    public void CheckUserConsent()
    {
        try
        {
            if (isOptInConsentRequired)
            {
                // Show a PIPL consent flow
                // ...

                // If consent is provided for both use and export
                Events.ProvideOptInConsent(consentIdentifier, true);

                // If consent is not provided
                Events.ProvideOptInConsent(consentIdentifier, false);
            }
        }
        catch (ConsentCheckException e)
        {
            // Handle the exception by checking e.Reason
        }
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
            .AddParam("userLevel", gameManager.game.currentLevel);

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
                // adProvider can be used to remotely define which of the installed Ad Networks should show an Ad
                // ANY / UNITY / MOPUB / IRONSOURCE
                string adProvider = "ANY"; 
                if (gameParameters.ContainsKey("adProvider"))
                {
                    adProvider = gameParameters["adProvider"].ToString();
                }
                
                // Rewarded Ad Value controlled by Engage "adRewardValue" game parameter                               
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
        
        if (gameParameters.ContainsKey("debugMode"))
        {
            gameManager.debugMode = System.Convert.ToInt32(gameParameters["debugMode"]);
            gameManager.bttnConsole.gameObject.SetActive(gameManager.debugMode == 1 ? true : false);
        }
        // --------------------------------------------------------------------------------------------

    }
}