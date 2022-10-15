using DeltaDNA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DeltaDNA
{
    static class NewMethodClass
    {

        /// <summary>
        /// Records an event using the GameEvent class.
        /// </summary>
        /// <param name="gameEvent">Event to record.</param>
        /// <returns><see cref="EventAction"/> for this event</returns>
        /// <exception cref="System.Exception">Thrown if the SDK has not been started.</exception>
        public static EventAction RecordSampledEvent<T>(this DDNA d, T gameEvent) where T : GameEvent<T>
        {
            Debug.Log("Woof Bark <T>" + gameEvent.Name);
            return DDNA.Instance.RecordEvent(gameEvent);
        }

        /// <summary>
        /// Records an event with no custom parameters.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <returns><see cref="EventAction"/> for this event</returns>
        /// <exception cref="System.Exception">Thrown if the SDK has not been started.</exception>
        public static EventAction RecordSampledEvent(this DDNA d, string eventName)
        {
            Debug.Log("Woof Bark - eventName " + eventName);
            return DDNA.Instance.RecordEvent(eventName);
        }

        /// <summary>
        /// Records an event with a name and a dictionary of event parameters.  The eventParams dictionary
        /// should match the 'eventParams' branch of the event schema.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="eventParams">Event parameters.</param>
        /// <returns><see cref="EventAction"/> for this event</returns>
        /// <exception cref="System.Exception">Thrown if the SDK has not been started.</exception>
        public static EventAction RecordSampledEvent(this DDNA d, string eventName, Dictionary<string, object> eventParams)
        {
            Debug.Log("Woof Bark eventName, eventParams " + eventName);
            return DDNA.Instance.RecordEvent(eventName, eventParams);
        }




    }
    /*
    internal class RemoteEventDsable : DDNAImpl
    {


        public void EnhancedRecordEvent()
        {
            this.RecordEvent("woof");
        }
        override internal EventAction RecordEvent(string eventName)
        {
            Debug.Log("################ OVERRIDEN #####################");
            return base.RecordEvent(new GameEvent(eventName));
        }

        /*
        override internal IEnumerator PostEvents(string[] events, Action<bool, int> resultCallback)
        {
            string bulkEvent = "{\"eventList\":[" + String.Join(",", events) + "]}";
            string url;
            if (HashSecret != null)
            {
                string md5Hash = DDNA.GenerateHash(bulkEvent, this.HashSecret);
                url = DDNA.FormatURI(Settings.COLLECT_HASH_URL_PATTERN, this.CollectURL, this.EnvironmentKey, md5Hash);
            }
            else
            {
                url = DDNA.FormatURI(Settings.COLLECT_URL_PATTERN, this.CollectURL, this.EnvironmentKey, null);
            }

            int attempts = 0;
            bool succeeded = false;
            int status = 0;

            Action<int, string, string> completionHandler = (statusCode, data, error) => {
                if (statusCode > 0 && statusCode < 400)
                {
                    succeeded = true;
                }
                else
                {
                    Logger.LogDebug("Error posting events: " + error + " " + data);
                }
                status = statusCode;
            };

            HttpRequest request = new HttpRequest(url);
            request.HTTPMethod = HttpRequest.HTTPMethodType.POST;
            request.HTTPBody = bulkEvent;
            request.setHeader("Content-Type", "application/json");

            do
            {
                yield return StartCoroutine(Network.SendRequest(request, completionHandler));

                if (succeeded || ++attempts < Settings.HttpRequestMaxRetries) break;

                yield return new WaitForSeconds(Settings.HttpRequestRetryDelaySeconds);
            } while (attempts < Settings.HttpRequestMaxRetries);

            resultCallback(succeeded, status);
        }
    }*/
}
