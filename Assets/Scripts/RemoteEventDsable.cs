using DeltaDNA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeltaDNA
{
    internal class RemoteEventDsable : DDNAImpl
    {
        internal RemoteEventDsable(DDNA ddna) : base(ddna)
        {

        }

        override internal EventAction RecordEvent(string eventName)
        {
            Debug.Log("################ OVERRIDEN #####################");
            return RecordEvent(new GameEvent(eventName));
        }

    }
}
