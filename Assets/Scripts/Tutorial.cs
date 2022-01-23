﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaDNA;
using UnityEngine.Advertisements;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Analytics;

public class Tutorial : MonoBehaviour, IUnityAdsListener
{
    public GameManager gameManager;
    
    // Unit Ads
    [Header("Unity Ads")]
    public string unityAdsGameId = "3802209";
    public bool unityAdsTestMode = false;
    public string unityAdsPlacementId = "dynamic_placement";

    // Mopub Ads
    [Header("MoPub Ads")]
    public string[] _rewardedAdUnits = { "920b6145fb1546cf8b5cf2ac34638bb7" };
    public bool isMoPubAdLoaded = false;


    // Properties
    [Header("Properties")]
    public bool isMoPubAdsEnabled = false;
    public bool isUnityAdsEnabled = true;
    
    private int adRewardValue;


    string consentIdentifier;
    bool isOptInConsentRequired;


    async void Awake()
    {

    }
    // Start is called before the first frame update
    async void Start()
    {
        // Congifure Enabled Ad Networks
        if (isUnityAdsEnabled) ConfigureUnityAds();
        if (isMoPubAdsEnabled) ConfigureMoPubAds();


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

                
                // Rewarded Ad display controlled by Engage "adShow" game parameter
                if (isUnityAdsEnabled && (adProvider == "ANY" || adProvider == "UNITY"))
                {
                    UnityShowRewardedAd();
                }
                else if (isMoPubAdsEnabled && (adProvider == "ANY" || adProvider == "MOPUB"))
                {
                    MoPubShowRewardedAd();
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

    #region UnityAds
    /// <summary>
    /// Unity Ads Stuff
    /// </summary>
    /// 

    public void ConfigureUnityAds()
    {
        // Unity Ads Configuration
        
        Advertisement.AddListener(this);
        Advertisement.Initialize(unityAdsGameId, unityAdsTestMode);
    }

    public void UnityShowRewardedAd()
    {
        
        // Send the deltaDNA userID to Unity Ads to
        // recveive adRevenue event bacm in DDNA
        ShowOptions options = new ShowOptions();
        options.gamerSid = DDNA.Instance.UserID;

        // Send deltaDNA SessionID to Unity Ads to 
        // attribute adRevenue to correct player session.
        MetaData metaData = new MetaData("DDNA");
        metaData.Set("sessionID", DDNA.Instance.SessionID);
        metaData.Set("collectURL", DDNA.Instance.CollectURL);
        metaData.Set("environmentKey", DDNA.Instance.EnvironmentKey);
        
        Advertisement.SetMetaData(metaData);

        // Show Unity Ad
        Advertisement.Show(unityAdsPlacementId, options);
        Debug.Log("Showing Unity Ad version + " + Advertisement.version);
    }

    // Unity Ads Listeners
    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Unity Ad Ready for PlacementID: " + placementId);
    }
    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("Unity Ads Error: " + message);
    }
    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Unity Ad Started for PlacementID: " + placementId);
    }
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        bool isAdFinished = false;

        if (showResult == ShowResult.Finished)
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
    #endregion

    #region MoPubAds
    /// <summary>
    /// MoPub SDK Stuff
    /// </summary>    
    /// 

    public void ConfigureMoPubAds()
    {
        // MoPub Ads
        MoPubManager.OnRewardedVideoLoadedEvent += OnMopubRewardedVideoLoadedEvent;
        MoPubManager.OnRewardedVideoClosedEvent += OnMoPubRewardedVideoClosedEvent;
        MoPubManager.OnImpressionTrackedEvent += OnMoPubImpressionTrackedEvent;

        MoPub.LoadRewardedVideoPluginsForAdUnits(_rewardedAdUnits);
    }
    public void MoPubSdkInitialized()
    {
        Debug.Log("MoPubSDK Initialised");
        MoPubRequestRewardedAd();
    }
    public void MoPubRequestRewardedAd()
    {
        Debug.Log("MoPubSDK Requesting Ad");
        MoPub.RequestRewardedVideo(_rewardedAdUnits[0]);
    }
    public void MoPubShowRewardedAd()
    {
        if (isMoPubAdLoaded)
        {
            Debug.Log("Showing MoPub Ad");
            MoPub.ShowRewardedVideo(_rewardedAdUnits[0]);
        }
        else
        {
            Debug.Log("Unable to Show MoPub Ad, none loaded at this time!");
        }
    }
    private void OnMopubRewardedVideoLoadedEvent(string adUnitId)
    {
        Debug.Log("Rewarded Ad Loaded for AdUnitID: " + adUnitId);
        isMoPubAdLoaded = true;

    }
    private void OnMoPubRewardedVideoClosedEvent(string adUnitId)
    {
        Debug.Log("MoPubSDK Rewarded Ad Closed - AdUnitID: " + adUnitId);
        isMoPubAdLoaded = false;
        MoPubRequestRewardedAd();
    }
    private void OnMoPubImpressionTrackedEvent(string adUnitId, MoPub.ImpressionData impressionData)
    {
        // The impression data from MoPub does contain additional parameters that haven't been added 
        // to the adImpression event in this example, but it you could extend this event in the Event Manager tool to accomodate them.
        Debug.Log("Impression Data" + impressionData.JsonRepresentation.ToString());


        GameEvent adEvent = new GameEvent("adImpression")
         .AddParam("adCompletionStatus", "COMPLETED")
         .AddParam("adProvider", "MoPub ")
         .AddParam("placementType", "REWARDED AD")
         .AddParam("placementId", impressionData.AdUnitId)
         .AddParam("placementName", impressionData.AdUnitName);

        // Add impression value if available. Multiplying publisher revenue by 1000 to get CPM value
        if (impressionData.PublisherRevenue != null) adEvent.AddParam("adEcpmUsd", System.Convert.ToDouble(impressionData.PublisherRevenue) * 1000);


        DDNA.Instance.RecordEvent(adEvent).Run();
    }
    #endregion

}