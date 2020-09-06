# MoPub Rewarded Ad integration notes

## 1. Instal SDK
Following the MoPub intergration guide from [here](https://developers.mopub.com/publishers/unity/integrate/)

Download the Mopub SDK for Unity from their [GitHub Repository](https://github.com/mopub/mopub-unity-sdk/releases) and import it as a custom package then allow the Google Version Handler to clean up any obsolete files. 

Select the "Add Selected Registeries" option in the "Unitry Package Manager Resolver" tool when it is displayed.


## 2.Test MoPub Example project
Load the MoPub\Sample\MoPubDemoScene and build it to a device. 

Whilst you can run trhe demo scene in the Unity Editor you will get this error message when you try to show Ads.  ``Please switch to either Android or iOS platforms to run sample app!``

* Switch platform to Android in the ``Build Settings`` page 
* Add  the MoPub demo scene to the build in the ``Build Settings  > Scenes to Build`` panel and make sure the demo scene is selected 
* Check the ``Project Settings > Player`` settings and make sure all your Android build settings are good.
    * Select "Proguard" option in the ``Publisher Settings > Minify`` panel to prevent DEX count issues 
* Select your "Run Device" from the ``Build Settings`` page then "Build and Run"

The App will let you test Banner, Interstitial and Rewarded Ads, check that you are able to "Request" and "Show" rewarded ads on your device. 

## 3. Add MoPub Manager Prefab to Scene
Drag the MoPub Manager Prefab into the scene and configure the settings in the Inspector.

## 4. Integrate MoPub code.

* Add SDK Initialized callback and wire it up in the MoPub Manager Inpsector.
* Instantiate a string array containing all AdUnit IDs 
```csharp
    // Mopub Ads
    [Header("MoPub Ads")]
    public string[] _rewardedAdUnits = { "920b6145fb1546cf8b5cf2ac34638bb7" };
    public bool isMoPubAdLoaded = false; 
```
* Wire up some more callbacks so we can react to Ads getting loaded, closed and when we receive impression tracking info to send to deltaDNA.

```csharp
        // MoPub Ads
        MoPubManager.OnRewardedVideoLoadedEvent += OnMopubRewardedVideoLoadedEvent;
        MoPubManager.OnRewardedVideoClosedEvent += OnMoPubRewardedVideoClosedEvent;
        MoPubManager.OnImpressionTrackedEvent += OnMoPubImpressionTrackedEvent;

        MoPub.LoadRewardedVideoPluginsForAdUnits(_rewardedAdUnits);
```

* Call the request Ad method when the SDK is Initialized and after showing an Ad
```csharp 
    public void MoPubSdkInitialized()
    {        
        Debug.Log("MoPubSDK Initialised");
        MoPubRequestRewardedAd();
    }

    private void OnMoPubRewardedVideoClosedEvent(string adUnitId)
    {
        Debug.Log("MoPubSDK Rewarded Ad Closed - AdUnitID: " + adUnitId);
        isMoPubAdLoaded = false;
        MoPubRequestRewardedAd();
    }
```

* And create a Show Ad method to be called by the Engage campaign's "game parmeter handler"
```csharp
    public void MoPubShowRewardedAd()
    {
        if (isMoPubAdLoaded)
        {
            Debug.Log("MoPubSDK Showing Ad");
            MoPub.ShowRewardedVideo(_rewardedAdUnits[0]);
        }
        else
        {
            Debug.Log("Unable to Show MoPub Ad, none loaded at this time!");
        }
    }
```
* Send an ``adImpression`` event to deltaDNA using the impression tracking data received from the MoPub SDK when the player closes the Ad. 

There are more parameters in the MoPub impression data than on the default adImpression event, feel free to extend the adImpression event in the ``SETUP > Manage Events`` tool and map the any additional parameters into it

MoPub Ad Impression data can include the value of the impression. If available, multiply it by 1000 and send it in the ``AdEcpmUsd`` parameter and it can be used in deltaDNA Analytics tools to calcualte player level Ad LTV.

```csharp
private void OnMoPubImpressionTrackedEvent(string adUnitId, MoPub.ImpressionData impressionData)
    {
        Debug.Log("Impression Data" + impressionData.JsonRepresentation.ToString());
        

        GameEvent adEvent = new GameEvent("adImpression")
         .AddParam("adCompletionStatus", "COMPLETED")
         .AddParam("adProvider", "MoPub ")
         .AddParam("placementType", "REWARDED AD")
         .AddParam("placementId", impressionData.AdUnitId)
         .AddParam("placementName",impressionData.AdUnitName);

        // Add impression value if available. Multiplying publisher revenue by 1000 to get CPM value
        if (impressionData.PublisherRevenue != null) adEvent.AddParam("adEcpmUsd", System.Convert.ToDouble(impressionData.PublisherRevenue)*1000); 
         

        DDNA.Instance.RecordEvent(adEvent).Run();
    }
```

The resulting ``adImpression`` event will look like this in the deltaDNA ``SETUP > Event Browser`` tool. 
![adImpression Event](Images/MoPub/adImpressionEvent.jpg)



