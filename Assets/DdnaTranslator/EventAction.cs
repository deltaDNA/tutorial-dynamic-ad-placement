//
// Copyright (c) 2018 deltaDNA Ltd. All rights reserved.
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace DeltaDNA
{
    internal class EventTrigger 
    {
    }

    public sealed class EventAction
    {
        /*
        internal static readonly ReadOnlyCollection<EventTrigger> EMPTY_TRIGGERS =
            new ReadOnlyCollection<EventTrigger>(new List<EventTrigger>(0));
        
        private readonly GameEvent evnt;
        private readonly ReadOnlyCollection<EventTrigger> triggers;
        private readonly Settings settings;
        private readonly ActionStore store;

        private readonly List<EventActionHandler> handlers =
            new List<EventActionHandler>();

        internal EventAction(
            GameEvent evnt,
            ReadOnlyCollection<EventTrigger> triggers,
            ActionStore store,
            Settings settings)
        {

            this.evnt = evnt;
            this.triggers = triggers;
            this.settings = settings;
            this.store = store;
        }
        */
        /// <summary>
        /// Register a handler to handle the parametrised action.
        /// </summary>
        /// <param name="handler">The handler to register</param>
        /// <returns>This <see cref="EventAction"/> instance</returns>

        // Todo Put this back in if we want to wrap Parameter Handlers in Remote Config
        /*
        public EventAction Add(EventActionHandler handler)
        {
            if (!handlers.Contains(handler))
            {
                handlers.Add(handler);
            }
            return this;
        }
        */
        /// <summary>
        /// Evaluates the registered handlers against the event and triggers
        /// associated for the event.
        /// </summary>
        public void Run()
        {

        }

        internal static EventAction CreateEmpty(GameEvent evnt)
        {
            return new EventAction();
        }
    }
}