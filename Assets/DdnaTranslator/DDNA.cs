using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Analytics;

namespace DeltaDNA
{
    public class DDNA : Singleton<DDNA>
    {

        public string EnvironmentKey { get { return null; } }
        public string CollectURL { get { return null; } }
        public string EngageURL { get { return null; } }
        public string Platform { get { return null; } }
        public string HashSecret { get { return null; } }
        public string ClientVersion { get { return null; } }
        public Settings Settings { get; set; }

        internal const string PF_KEY_USER_ID = "DDSDK_USER_ID";
        //

        protected DDNA()
        {

        }
        public void StartSDK()
        {

        }
        public void StartSDK(string userID)
        {

        }
        public EventAction RecordEvent(GameEvent gameEvent)
        {

            Events.CustomData(gameEvent.Name, gameEvent.AsDictionary());
            return null;

            /*return new EventAction(
                gameEvent as GameEvent,
               EventAction.EMPTY_TRIGGERS, null, null);
            */


            /*
            if (!started)
            {
                throw new Exception("You must first start the SDK via the StartSDK method");
            }
            else if (whitelistEvents.Count != 0 && !whitelistEvents.Contains(gameEvent.Name))
            {
                Logger.LogDebug("Event " + gameEvent.Name + " is not whitelisted, ignoring");
                return EventAction.CreateEmpty(gameEvent as GameEvent);
            }

            gameEvent.AddParam("platform", Platform);
            gameEvent.AddParam("sdkVersion", Settings.SDK_VERSION);

            var eventSchema = gameEvent.AsDictionary();
            eventSchema["userID"] = this.UserID;
            eventSchema["sessionID"] = this.SessionID;
            eventSchema["eventUUID"] = Guid.NewGuid().ToString();

            string currentTimestmp = GetCurrentTimestamp();
            if (currentTimestmp != null)
            {
                eventSchema["eventTimestamp"] = GetCurrentTimestamp();
            }

            try
            {
                string json = MiniJSON.Json.Serialize(eventSchema);
                if (!this.eventStore.Push(json))
                {
                    Logger.LogWarning("Event store full, dropping '" + gameEvent.Name + "' event.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Unable to generate JSON for '" + gameEvent.Name + "' event. " + ex.Message);
            }

            return new EventAction(
                gameEvent as GameEvent,
                eventTriggers.ContainsKey(gameEvent.Name)
                    ? eventTriggers[gameEvent.Name]
                    : EventAction.EMPTY_TRIGGERS, actionStore, Settings);
            */
        }

        public EventAction RecordEvent(string eventName)
        {
            return RecordEvent(new GameEvent(eventName));
        }

        public EventAction RecordEvent(string eventName, Dictionary<string, object> eventParams)
        {


            var gameEvent = new GameEvent(eventName);
            foreach (var key in eventParams.Keys)
            {
                gameEvent.AddParam(key, eventParams[key]);
            }
            return RecordEvent(gameEvent);

        }

        public string UserID
        {
            get
            {
                string v = PlayerPrefs.GetString(PF_KEY_USER_ID, null);
                if (String.IsNullOrEmpty(v))
                {
                    return null;
                }
                return v;
            }
        }
        public string SessionID
        {
            get
            {
                return null;
            }
        }


        internal void NotifyOnSessionConfigured(bool cached) { }
        public event Action OnNewSession;
        public event Action<bool> OnSessionConfigured;
        public event Action OnSessionConfigurationFailed;
        public event Action OnImageCachePopulated;
        public event Action<string> OnImageCachingFailed;

        public void SetLoggingLevel(Logger.Level level) { }
        
    }
}