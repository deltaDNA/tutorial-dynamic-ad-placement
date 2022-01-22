//
// Copyright (c) 2016 deltaDNA Ltd. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace DeltaDNA
{

/// <summary>
/// iOS Notifications Plugin enables a game to register with Apple's push notification service.  It provides
/// some additional functionality not easily accessible from Unity.  By using the events, a game can be
/// notified when a game has registered with the service and when push notification has occured.  We use
/// these events to log notifications with the DeltaDNA platform.
/// </summary>
public class IosNotifications : MonoBehaviour
{

    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern void _markUnityLoaded();
    #endif

    // Called with JSON string of the notification payload.
    public event Action<string> OnDidLaunchWithPushNotification;

    // Called with JSON string of the notification payload.
    public event Action<string> OnDidReceivePushNotification;

    // Called with the deviceToken.
    public event Action<string> OnDidRegisterForPushNotifications;

    // Called with the error string.
    public event Action<string> OnDidFailToRegisterForPushNotifications;

    void Awake()
    {/*
        gameObject.name = this.GetType().ToString();
        DontDestroyOnLoad(this);

        #if UNITY_IOS && !UNITY_EDITOR
        _markUnityLoaded();
        #endif
            */
    }

    /// <summary>
    /// Registers for push notifications.
    /// </summary>
    public void RegisterForPushNotifications()
    {

    }

    /// <summary>
    /// Unregisters for push notifications.
    /// </summary>
    public void UnregisterForPushNotifications()
    {

    }

    #region Native Bridge

    public void DidReceivePushNotification(string notification)
    {

    }

    public void DidRegisterForPushNotifications(string deviceToken)
    {

    }

    public void DidFailToRegisterForPushNotifications(string error)
    {

    }

    #endregion

}

} // namespace DeltaDNA
