using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoPubDemoGUIBasic : MonoBehaviour
{
    [Tooltip("Whether to ignore ad unit states and simply enable all buttons to test invalid MoPub API calls " +
             "(e.g., calling Show before Request).")]
    public bool ForceEnableButtons = false;

    // State maps to enable/disable GUI ad state buttons
    private readonly Dictionary<string, bool> _adUnitToLoadedMapping = new Dictionary<string, bool>();

    private readonly Dictionary<string, bool> _adUnitToShownMapping = new Dictionary<string, bool>();

    private readonly Dictionary<string, List<MoPub.Reward>> _adUnitToRewardsMapping =
        new Dictionary<string, List<MoPub.Reward>>();

    private bool _consentDialogLoaded;

#if UNITY_ANDROID || UNITY_EDITOR
    private readonly string[] _rewardedAdUnits =
        { "920b6145fb1546cf8b5cf2ac34638bb7" };
#endif



    [SerializeField]
    private GUISkin _skin;

    // Label style for no ad unit messages
    private GUIStyle _smallerFont;

    // Buffer space between sections
    private int _sectionMarginSize;

    // Scroll view position
    private Vector2 _scrollPosition;

    // Label style for plugin and SDK version banner
    private GUIStyle _centeredStyle;

    // Default text for custom data fields
    private static string _customDataDefaultText = "Optional custom data";

    // String to fill with custom data for Rewarded ads
    private string _rewardedCustomData = _customDataDefaultText;

    // Flag indicating that personally identifiable information can be collected
    private bool _canCollectPersonalInfo = false;

    // Current consent status of this user to collect personally identifiable information
    private MoPub.Consent.Status _currentConsentStatus = MoPub.Consent.Status.Unknown;

    // Flag indicating that consent should be acquired to collect personally identifiable information
    private bool _shouldShowConsentDialog = false;

    // Flag indicating that the General Data Protection Regulation (GDPR) applies to this user
    private bool? _isGdprApplicable = false;

    // Flag indicating that the General Data Protection Regulation (GDPR) has been forcibly applied by the publisher
    private bool _isGdprForced = false;

    // Status string for tracking current state
    private string _status = string.Empty;

    // Index for current banner ad position, which is incremented after every banner request, starting with BottomCenter
    private int _bannerPositionIndex = 5;

    // All possible banner positions
    private readonly MoPub.AdPosition[] _bannerPositions =
        Enum.GetValues(typeof(MoPub.AdPosition)).Cast<MoPub.AdPosition>().ToArray();


    private static bool IsAdUnitArrayNullOrEmpty(ICollection<string> adUnitArray)
    {
        return (adUnitArray == null || adUnitArray.Count == 0);
    }


    private void AddAdUnitsToStateMaps(IEnumerable<string> adUnits)
    {
        foreach (var adUnit in adUnits) {
            _adUnitToLoadedMapping[adUnit] = false;
            _adUnitToShownMapping[adUnit] = false;
        }
    }


    public void SdkInitialized()
    {
        UpdateConsentValues();
    }


    public void UpdateStatusLabel(string message)
    {
        _status = message;
        Debug.Log("Status Label updated to: " + message);
    }


    public void ClearStatusLabel()
    {
        UpdateStatusLabel(string.Empty);
    }


    public void ConsentStatusChanged(MoPub.Consent.Status oldStatus, MoPub.Consent.Status newStatus, bool canCollectPersonalInfo)
    {
        _canCollectPersonalInfo = canCollectPersonalInfo;
        _currentConsentStatus = newStatus;
        _shouldShowConsentDialog = MoPub.ShouldShowConsentDialog;

        UpdateStatusLabel($"Consent status changed from {oldStatus} to {newStatus}");
    }

    public void LoadAvailableRewards(string adUnitId, List<MoPub.Reward> availableRewards)
    {
        // Remove any existing available rewards associated with this AdUnit from previous ad requests
        _adUnitToRewardsMapping.Remove(adUnitId);

        if (availableRewards != null) {
            _adUnitToRewardsMapping[adUnitId] = availableRewards;
        }
    }


    public void BannerLoaded(string adUnitId, float height)
    {
        AdLoaded(adUnitId);
        _adUnitToShownMapping[adUnitId] = true;
    }


    public void AdLoaded(string adUnit)
    {
        _adUnitToLoadedMapping[adUnit] = true;
        UpdateStatusLabel("Loaded " + adUnit);
    }


    public void AdDismissed(string adUnit)
    {
        _adUnitToLoadedMapping[adUnit] = false;
        ClearStatusLabel();
    }


    public void ImpressionTracked(string adUnit, MoPub.ImpressionData impressionData)
    {
        UpdateStatusLabel("Impression tracked for " + adUnit + " with impression data: "
                          + impressionData.JsonRepresentation);
    }


    public bool ConsentDialogLoaded {
        private get { return _consentDialogLoaded; }
        set {
            _consentDialogLoaded = value;
            if (_consentDialogLoaded) UpdateStatusLabel("Consent dialog loaded");
        }
    }


    private void Awake()
    {
        if (Screen.width < 960 && Screen.height < 960) {
            _skin.button.fixedHeight = 50;
        }

        _smallerFont = new GUIStyle(_skin.label) { fontSize = _skin.button.fontSize };
        _centeredStyle = new GUIStyle(_skin.label) { alignment = TextAnchor.UpperCenter };

        // Buffer space between sections
        _sectionMarginSize = _skin.label.fontSize;


        AddAdUnitsToStateMaps(_rewardedAdUnits);

        ConsentDialogLoaded = false;
    }


    private void Start()
    {
        // The SdkInitialize() call is handled by the MoPubManager prefab now. Please see:
        // https://developers.mopub.com/publishers/unity/initialize/#option-1-use-the-mopub-manager-recommended


        MoPub.LoadRewardedVideoPluginsForAdUnits(_rewardedAdUnits);

#if !(UNITY_ANDROID || UNITY_IOS)
        Debug.LogError("Please switch to either Android or iOS platforms to run sample app!");
#endif

#if UNITY_EDITOR
        Debug.LogWarning("No SDK was loaded since this is not on a mobile device! Real ads will not load.");
#endif

    }


    private void Update()
    {
        // Enable scrollview dragging
        foreach (var touch in Input.touches) {
            if (touch.phase != TouchPhase.Moved) continue;
            _scrollPosition.y += touch.deltaPosition.y;
            _scrollPosition.x -= touch.deltaPosition.x;
        }
    }



    private void OnGUI()
    {
        GUI.skin = _skin;

        // Screen.safeArea was added in Unity 2017.2.0p1
        var guiArea = Screen.safeArea;
        guiArea.x += 20;
        guiArea.y += 20;
        guiArea.width -= 40;
        guiArea.height -= 40;
        GUILayout.BeginArea(guiArea);
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);


        CreateRewardedVideosSection();

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }





    private void CreateRewardedVideosSection()
    {
        GUILayout.Space(_sectionMarginSize);
        GUILayout.Label("Rewarded Videos");
        if (!IsAdUnitArrayNullOrEmpty(_rewardedAdUnits)) {
            CreateCustomDataField("rewardedCustomDataField", ref _rewardedCustomData);
            foreach (var rewardedAdUnit in _rewardedAdUnits) {
                GUILayout.BeginHorizontal();

                GUI.enabled = !_adUnitToLoadedMapping[rewardedAdUnit] || ForceEnableButtons;
                if (GUILayout.Button(CreateRequestButtonLabel(rewardedAdUnit))) {
                    Debug.Log("requesting rewarded ad with AdUnit: " + rewardedAdUnit);
                    UpdateStatusLabel("Requesting " + rewardedAdUnit);
                    MoPub.RequestRewardedVideo(
                        adUnitId: rewardedAdUnit, keywords: "rewarded, video, mopub",
                        latitude: 37.7833, longitude: 122.4167, customerId: "customer101");
                }

                GUI.enabled = _adUnitToLoadedMapping[rewardedAdUnit] || ForceEnableButtons;
                if (GUILayout.Button("Show")) {
                    ClearStatusLabel();
                    MoPub.ShowRewardedVideo(rewardedAdUnit, GetCustomData(_rewardedCustomData));
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();


                // Display rewards if there's a rewarded video loaded and there are multiple rewards available
                if (!MoPub.HasRewardedVideo(rewardedAdUnit)
                    || !_adUnitToRewardsMapping.ContainsKey(rewardedAdUnit)
                    || _adUnitToRewardsMapping[rewardedAdUnit].Count <= 1) continue;

                GUILayout.BeginVertical();
                GUILayout.Space(_sectionMarginSize);
                GUILayout.Label("Select a reward:");

                foreach (var reward in _adUnitToRewardsMapping[rewardedAdUnit]) {
                    if (GUILayout.Button(reward.ToString())) {
                        MoPub.SelectReward(rewardedAdUnit, reward);
                    }
                }

                GUILayout.Space(_sectionMarginSize);
                GUILayout.EndVertical();
            }
        } else {
            GUILayout.Label("No rewarded AdUnits available", _smallerFont, null);
        }
    }




    private void UpdateConsentValues()
    {/*
        _canCollectPersonalInfo = MoPub.CanCollectPersonalInfo;
        _currentConsentStatus = MoPub.CurrentConsentStatus;
        _shouldShowConsentDialog = MoPub.ShouldShowConsentDialog;
        _isGdprApplicable = MoPub.IsGdprApplicable;*/
    }


    private static void CreateCustomDataField(string fieldName, ref string customDataValue)
    {
        GUI.SetNextControlName(fieldName);
        customDataValue = GUILayout.TextField(customDataValue, GUILayout.MinWidth(200));
        if (Event.current.type != EventType.Repaint) return;
        if (GUI.GetNameOfFocusedControl() == fieldName && customDataValue == _customDataDefaultText) {
            // Clear default text when focused
            customDataValue = string.Empty;
        } else if (GUI.GetNameOfFocusedControl() != fieldName && string.IsNullOrEmpty(customDataValue)) {
            // Restore default text when unfocused and empty
            customDataValue = _customDataDefaultText;
        }
    }


    private void CreateStatusSection()
    {
        GUILayout.Space(40);
        GUILayout.Label(_status, _smallerFont);
    }


    private static string GetCustomData(string customDataFieldValue)
    {
        return customDataFieldValue != _customDataDefaultText ? customDataFieldValue : null;
    }


    private static string CreateRequestButtonLabel(string adUnit)
    {
        return adUnit.Length > 10 ? "Request " + adUnit.Substring(0, 10) + "..." : adUnit;
    }
}
