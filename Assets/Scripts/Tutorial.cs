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
    
    string consentIdentifier;
    bool isOptInConsentRequired;


    // Start is called before the first frame update
    async void Start()
    {

        
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.StartSDK();
       
        // Start UGS SDK
        try
        {
            var options = new InitializationOptions();
            options.SetOption("Environment", "production");
            // Todo options.SetAnalyticsUserId(DDNA.Instance.UserID);

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

}