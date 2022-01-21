//
// Copyright (c) 2017 deltaDNA Ltd. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEditor;

namespace DeltaDNA.Editor {

    [InitializeOnLoad]
    sealed class DeltaDNASdkChecker : SdkChecker {

        static DeltaDNASdkChecker() {
            new DeltaDNASdkChecker().Register();
        }

        protected override void PerformCheck(IList<DDNATuple<string, Severity>> problems) {
             if (File.Exists(Configuration.FULL_ASSET_PATH)) {
                Configuration config = Configuration.GetAssetInstance();   

                if (string.IsNullOrEmpty(config.environmentKeyDev)
                    && string.IsNullOrEmpty(config.environmentKeyLive)) {
                    problems.Add(DDNATuple.New(
                        "[Analytics] Environment key has not been configured.",
                        Severity.ERROR));
                }
                if (string.IsNullOrEmpty(config.collectUrl)) {
                    problems.Add(DDNATuple.New(
                        "[Analytics] Collect URL has not been configured.",
                        Severity.ERROR));
                }
                if (string.IsNullOrEmpty(config.engageUrl)) {
                    problems.Add(DDNATuple.New(
                        "[Analytics] Engage URL has not been configured.",
                        Severity.ERROR));
                }
                
                if (string.IsNullOrEmpty(config.clientVersion) && !config.useApplicationVersion) {
                    problems.Add(DDNATuple.New(
                        "[Analytics] Client version has not been configured.",
                        Severity.WARNING));
                }
            } else {
                problems.Add(DDNATuple.New(
                    "[Analytics] Application has not been configured with Platform keys and URLs.",
                    Severity.ERROR));
            }
        }
    }
}
