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

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;
using Application = UnityEngine.Application;

namespace DeltaDNA.Editor
{
    public sealed class ConfigurationWindow : EditorWindow {

        // UI
        
        private Texture logo;
        private GUIStyle styleFoldout;
        private bool foldoutAnalytics = true;
        private bool foldoutAndroidNotifications = true;
        private bool foldoutiOSNotifications = true;
        private Vector2 scrollPosition;
        
        // config
        
      
        void OnEnable() {
            titleContent = new GUIContent(
                "Configuration",
                AssetDatabase.LoadAssetAtPath<Texture>(WindowHelper.FindFile("Editor/Resources/Logo_16.png")));
            
            Load();
        }

        void OnGUI() {
            // workaround for OnEnable weirdness when initialising values
            if (logo == null) logo = AssetDatabase.LoadAssetAtPath<Texture>(WindowHelper.FindFile("Editor/Resources/Logo.png"));
            if (styleFoldout == null) styleFoldout = new GUIStyle(EditorStyles.foldout) {
                fontStyle = FontStyle.Bold,
                fontSize = 12
            };

            SerializedObject cfg = GetSerializedConfig();
            
            GUILayout.Label(logo, GUILayout.ExpandWidth(false));
            
            GUILayout.Space(WindowHelper.HEIGHT_SEPARATOR);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foldoutAnalytics = CreateFoldout(
                foldoutAnalytics,
                "Analytics",
                true,
                styleFoldout);
            if (foldoutAnalytics) 
            {
                GUILayout.Label("Required", EditorStyles.boldLabel);
                
                EditorGUILayout.PropertyField(
                    cfg.FindProperty("environmentKeyDev"),
                    new GUIContent(
                        "Environment key (dev)",
                        "Enter your game's development environment key"));

                EditorGUILayout.PropertyField(
                    cfg.FindProperty("environmentKeyLive"),
                    new GUIContent(
                        "Environment key (live)",
                        "Enter your game's live environment key"));

                SerializedProperty env_key = cfg.FindProperty("environmentKey");

                env_key.intValue = EditorGUILayout.Popup(

                    new GUIContent(
                        "Selected key",
                        "Select which environment key to use for the build"),
                    env_key.intValue,
                    new GUIContent[] {
                        new GUIContent("Development"),
                        new GUIContent("Live")});
                
                EditorGUILayout.PropertyField(
                    cfg.FindProperty("collectUrl"),
                    new GUIContent(
                        "Collect URL",
                        "Enter your game's collect URL"));

                EditorGUILayout.PropertyField(
                    cfg.FindProperty("engageUrl"),
                    new GUIContent(
                        "Engage URL",
                        "Enter your game's engage URL"));


                
                GUILayout.Label("Optional", EditorStyles.boldLabel);
                
                EditorGUILayout.PropertyField(
                    cfg.FindProperty("hashSecret"),
                    new GUIContent(
                        "Hash secret",
                        "Enter your game's hash secret if hashing is enabled"));

                EditorGUI.BeginDisabledGroup(cfg.FindProperty("useApplicationVersion").boolValue);

                EditorGUILayout.PropertyField(
                    cfg.FindProperty("clientVersion"),
                    new GUIContent(
                        "Client version",
                        "Enter your game's version or use the Editor value by enabling the checkbox below"));

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(
                    cfg.FindProperty("useApplicationVersion"),
                    new GUIContent(
                        "Use application version",
                        "Check to use the application/bundle version as set in the Editor"));

                if (cfg.hasModifiedProperties)
                {
                    cfg.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                }
            }
            
            GUILayout.Space(WindowHelper.HEIGHT_SEPARATOR);
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Space(WindowHelper.HEIGHT_SEPARATOR);



            
            EditorGUILayout.EndScrollView();
        }


        
        private void Load() {
        }

        private static bool CreateFoldout(
            bool foldout,
            string content,
            bool toggleOnLabelClick,
            GUIStyle style) {

#if UNITY_5_5_OR_NEWER
            return EditorGUILayout.Foldout(foldout, content, toggleOnLabelClick, style);
#else
            return EditorGUILayout.Foldout(foldout, content, style);
#endif
        }
        


        private SerializedObject GetSerializedConfig()
        {
            Configuration cfg = AssetDatabase.LoadAssetAtPath<Configuration>(Configuration.FULL_ASSET_PATH);

            if (cfg != null)
            {
                return new SerializedObject(cfg);
            }

            // If we couldn't load the asset we should create a new instance.
            cfg = ScriptableObject.CreateInstance<Configuration>();

            if (!AssetDatabase.IsValidFolder(Configuration.ASSET_DIRECTORY))
            {
                AssetDatabase.CreateFolder(Configuration.RESOURCES_CONTAINER, Configuration.RESOURCES_DIRECTORY);
            }
            AssetDatabase.CreateAsset(cfg, Configuration.FULL_ASSET_PATH);
            AssetDatabase.SaveAssets();

            return new SerializedObject(cfg);
        }
        

        // Adapted from https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        // as there was no inbuilt method to copy a directory recursively.
        private static void DirectoryCopy(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            
            Directory.CreateDirectory(destDirName);        

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension.EndsWith("meta"))
                {
                    continue;
                }
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }
            
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }
        }
    }
    

}
