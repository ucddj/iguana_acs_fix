﻿using ModLoaderLite.Config;
using ModLoaderLite;
using System;
using System.Collections.Generic;
using FairyGUI;

namespace iguana_acs_fix
{
    public class iguana_acs_fix
    {
        public static bool configLoaded = false;
        public static Dictionary<string, bool> config = new Dictionary<string, bool>()
        {
            { "ShowResourcesInWorldMap", ShowResourcesInWorldMap.enabled},
            { "Correct Caged Animals Perception", true }
        };

        static Dictionary<string, List<Action>> loadSaveSubmods = new Dictionary<string, List<Action>>()
        {
            { "ShowResourcesInWorldMap", new List<Action>(){ ShowResourcesInWorldMap.OnLoad, null} }
        };

        public static void OnInit()
        {
            Dictionary<string, bool> loadConfig = MLLMain.GetSaveOrDefault<Dictionary<string, bool>>("iguana_acs_fix_config");
            if (loadConfig != null)
            {
                // We don't directly overwrite as we must handle loaded configs from previous versions of the mod
                foreach (KeyValuePair<string, bool> kvp in loadConfig)
                {
                    config[kvp.Key] = kvp.Value;
                };
            }
            if (!Configuration.ListItems.ContainsKey("iguana_acs_fix"))
            {
                foreach (KeyValuePair<string, bool> kvp in config)
                {
                    Configuration.AddCheckBox("iguana_acs_fix", kvp.Key, kvp.Key, kvp.Value);
                }
                Configuration.Subscribe(new EventCallback0(HandleConfig));
            }
            HandleConfig(); // Needed or the loaded config isn't applied immediately
            foreach (KeyValuePair<string, List<Action>> kvp in loadSaveSubmods)
            {
                if ((!config.ContainsKey(kvp.Key) || config[kvp.Key]) && kvp.Value[0] != null)
                {
                    KLog.Dbg("iguana_acs_fix: OnLoad submod " + kvp.Key);
                    kvp.Value[0]();
                }
            }
            configLoaded = true;
        }
        public static Action OnLoad = OnInit;

        public static void OnSave()
        {
            MLLMain.AddOrOverWriteSave("iguana_acs_fix_config", config);
            foreach (KeyValuePair<string, List<Action>> kvp in loadSaveSubmods)
            {
                if ((!config.ContainsKey(kvp.Key) || config[kvp.Key]) && kvp.Value[1] != null)
                {
                    KLog.Dbg("iguana_acs_fix: OnSave submod " + kvp.Key);
                    kvp.Value[1]();
                }
            }
        }

        private static void HandleConfig()
        {
            ShowResourcesInWorldMap.enabled = Configuration.GetCheckBox("iguana_acs_fix", "ShowResourcesInWorldMap");
            Dictionary<string, bool> newConfig = new Dictionary<string, bool>();
            foreach (KeyValuePair<string, bool> kvp in config)
            {
                bool newValue = Configuration.GetCheckBox("iguana_acs_fix", kvp.Key);
                if (kvp.Value != newValue)
                {
                    newConfig[kvp.Key] = newValue;
                }
            }
            foreach (KeyValuePair<string, bool> kvp in newConfig)
            {
                config[kvp.Key] = kvp.Value;
            }
        }
    }

}
