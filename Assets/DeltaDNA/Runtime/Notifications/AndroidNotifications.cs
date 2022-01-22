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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeltaDNA
{

    /// <summary>
    /// Android Notifications enables a game to register with Google's GCM service.
    /// Is uses our native Android plugin to retreive the registration id required
    /// to send a push notification to a game. This id is sent to our platform as
    /// appropriate.
    /// </summary>
    public class AndroidNotifications : MonoBehaviour
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        private Android.DDNANotifications ddnaNotifications;
        #endif

        /// <summary>
        /// Called with the JSON string of the notification payload when the
        /// user opens the app from the background through an interaction on
        /// the push notification.
        /// </summary>
        public event Action<string> OnDidLaunchWithPushNotification;
        /// <summary>
        /// Called with the JSON string of the notification payload when the
        /// push notification is received while the app is in the foreground.
        /// </summary>
        public event Action<string> OnDidReceivePushNotification;
        /// <summary>
        /// Called with the registration id when the app registers for push
        /// notifications, or when the registration id is refreshed.
        /// </summary>
        public event Action<string> OnDidRegisterForPushNotifications;
        /// <summary>
        /// Called with the error string when registering for push notifications
        /// fails.
        /// </summary>
        public event Action<string> OnDidFailToRegisterForPushNotifications;

        /// <summary>
        /// Holds whether notifications are present in the project as they can
        /// be removed.
        /// </summary>
        private bool? notificationsPresent;


        /// <summary>
        /// Registers for push notifications.
        /// 
        /// If you have multiple Firebase Cloud Messaging senders in your project
        /// then you can use the deltaDNA sender either as a main one or as a
        /// secondary one by setting the secondary parameter. If you set secondary
        /// to true then the default FCM sender will need to have been initialised
        /// beforehand.
        /// 
        /// In the case when the app is already registered for push notifications
        /// and the registration id is up to date then the callbacks will not be
        /// invoked.
        /// </summary>
        /// <param name="secondary">Whether the Firebase instance used for the
        /// deltaDNA notifications should be registered as a secondary (non-main)
        /// instance.</param>
        public void RegisterForPushNotifications(bool secondary = false) {
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

        public void DidRegisterForPushNotifications(string registrationId)
        {

        }

        public void DidFailToRegisterForPushNotifications(string error)
        {

        }

        #endregion
    }

} // namespace DeltaDNA
