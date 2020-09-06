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
Drag the MoPub Manager Prefab into the scene and 

