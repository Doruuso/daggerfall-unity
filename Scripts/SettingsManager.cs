﻿// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;
using IniParser;
using IniParser.Model;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Settings manager for reading game configuration from INI file.
    /// Can read from settings.ini in game data directory or from built-in fallback.ini in Resources.
    /// For any setting failing a read (or settings.ini missing), fallback.ini will be used instead.
    /// </summary>
    public class SettingsManager
    {
        const string settingsIniName = "settings.ini";
        const string fallbackIniName = "fallback.ini";

        const string sectionDaggerfall = "Daggerfall";
        const string sectionVideo = "Video";
        const string sectionChildGuard = "ChildGuard";
        const string sectionGUI = "GUI";
        const string sectionControls = "Controls";
        const string sectionStartup = "Startup";
        const string sectionEnhancements = "Enhancements";

        bool usingFallback = false;
        FileIniDataParser iniParser = new FileIniDataParser();
        IniData fallbackIniData = null;
        IniData userIniData = null;

        public SettingsManager()
        {
            ReadSettings();
        }

        #region Settings Properties

        // [Daggerfall]

        public string MyDaggerfallPath
        {
            get { return GetString(sectionDaggerfall, "MyDaggerfallPath"); }
            set { SetString(sectionDaggerfall, "MyDaggerfallPath", value); }
        }

        public string MyDaggerfallUnitySavePath
        {
            get { return GetString(sectionDaggerfall, "MyDaggerfallUnitySavePath"); }
            set { SetString(sectionDaggerfall, "MyDaggerfallUnitySavePath", value); }
        }

        // [Video]

        public int ViewportX
        {
            get { return GetInt(sectionVideo, "ViewportX"); }
            set { SetInt(sectionVideo, "ViewportX", value); }
        }

        public int ViewportY
        {
            get { return GetInt(sectionVideo, "ViewportY"); }
            set { SetInt(sectionVideo, "ViewportY", value); }
        }

        public bool Fullscreen
        {
            get { return GetBool(sectionVideo, "Fullscreen"); }
            set { SetBool(sectionVideo, "Fullscreen", value); }
        }

        public bool VSync
        {
            get { return GetBool(sectionVideo, "VSync"); }
            set { SetBool(sectionVideo, "VSync", value); }
        }

        public int FieldOfView
        {
            get { return GetInt(sectionVideo, "FieldOfView", 60, 80); }
            set { SetInt(sectionVideo, "FieldOfView", value); }
        }

        public int MainFilterMode
        {
            get { return GetInt(sectionVideo, "MainFilterMode", 0, 2); }
            set { SetInt(sectionVideo, "MainFilterMode", value); }
        }

        public bool UseLegacyDeferred
        {
            get { return GetBool(sectionVideo, "UseLegacyDeferred"); }
            set { SetBool(sectionVideo, "UseLegacyDeferred", value); }
        }

        // [ChildGuard]

        public bool NoPlayerNudity
        {
            get { return GetBool(sectionChildGuard, "NoPlayerNudity"); }
            set { SetBool(sectionChildGuard, "NoPlayerNudity", value); }
        }

        // [GUI]

        public int GUIFilterMode
        {
            get { return GetInt(sectionGUI, "GUIFilterMode", 0, 2); }
            set { SetInt(sectionGUI, "GUIFilterMode", value); }
        }

        public int VideoFilterMode
        {
            get { return GetInt(sectionGUI, "VideoFilterMode"); }
            set { SetInt(sectionGUI, "VideoFilterMode", value); }
        }

        public bool Crosshair
        {
            get { return GetBool(sectionGUI, "Crosshair"); }
            set { SetBool(sectionGUI, "Crosshair", value); }
        }

        public bool SwapHealthAndFatigueColors
        {
            get { return GetBool(sectionGUI, "SwapHealthAndFatigueColors"); }
            set { SetBool(sectionGUI, "SwapHealthAndFatigueColors", value); }
        }

        public float DimAlphaStrength
        {
            get { return GetFloat(sectionGUI, "DimAlphaStrength", 0, 1); }
            set { SetFloat(sectionGUI, "DimAlphaStrength", value); }
        }

        public bool FreeScaling
        {
            get { return GetBool(sectionGUI, "FreeScaling"); }
            set { SetBool(sectionGUI, "FreeScaling", value); }
        }

        public bool EnableToolTips
        {
            get { return GetBool(sectionGUI, "EnableToolTips"); }
            set { SetBool(sectionGUI, "EnableToolTips", value); }
        }

        public float ToolTipDelayInSeconds
        {
            get { return GetFloat(sectionGUI, "ToolTipDelayInSeconds", 0, 10); }
            set { SetFloat(sectionGUI, "ToolTipDelayInSeconds", value); }
        }

        public Color32 ToolTipBackgroundColor
        {
            get { return GetColor(sectionGUI, "ToolTipBackgroundColor", DaggerfallUI.DaggerfallUnityDefaultToolTipBackgroundColor); }
            set { SetColor(sectionGUI, "ToolTipBackgroundColor", value); }
        }

        public Color32 ToolTipTextColor
        {
            get { return GetColor(sectionGUI, "ToolTipTextColor", DaggerfallUI.DaggerfallUnityDefaultToolTipTextColor); }
            set { SetColor(sectionGUI, "ToolTipTextColor", value); }
        }

        // [Controls]

        public bool InvertMouseVertical
        {
            get { return GetBool(sectionControls, "InvertMouseVertical"); }
            set { SetBool(sectionControls, "InvertMouseVertical", value); }
        }

        public bool MouseLookSmoothing
        {
            get { return GetBool(sectionControls, "MouseLookSmoothing"); }
            set { SetBool(sectionControls, "MouseLookSmoothing", value); }
        }

        public float MouseLookSensitivity
        {
            get { return GetFloat(sectionControls, "MouseLookSensitivity", 0.1f, 4.0f); }
            set { SetFloat(sectionControls, "MouseLookSensitivity", value); }
        }

        public bool HeadBobbing
        {
            get { return GetBool(sectionControls, "HeadBobbing"); }
            set { SetBool(sectionControls, "HeadBobbing", value); }
        }

        public bool ShowWeaponLeftHand
        {
            get { return GetBool(sectionControls, "ShowWeaponLeftHand"); }
            set { SetBool(sectionControls, "ShowWeaponLeftHand", value); }
        }

        public float WeaponSwingThreshold
        {
            get { return GetFloat(sectionControls, "WeaponSwingThreshold", 0.1f, 1.0f); }
            set { SetFloat(sectionControls, "WeaponSwingThreshold", value); }
        }

        public int WeaponSwingTriggerCount
        {
            get { return GetInt(sectionControls, "WeaponSwingTriggerCount", 1, 10); }
            set { SetInt(sectionControls, "WeaponSwingTriggerCount", value); }
        }

        // [Startup]

        public int StartCellX
        {
            get { return GetInt(sectionStartup, "StartCellX", 2, 997); }
            set { SetInt(sectionStartup, "StartCellX", value); }
        }

        public int StartCellY
        {
            get { return GetInt(sectionStartup, "StartCellY", 2, 497); }
            set { SetInt(sectionStartup, "StartCellY", value); }
        }

        public bool StartInDungeon
        {
            get { return GetBool(sectionStartup, "StartInDungeon"); }
            set { SetBool(sectionStartup, "StartInDungeon", value); }
        }

        // [Enhancements]

        public bool LypyL_GameConsole
        {
            get { return GetBool(sectionEnhancements, "LypyL_GameConsole"); }
            set { SetBool(sectionEnhancements, "LypyL_GameConsole", value); }
        }

        public bool LypyL_EnhancedSky
        {
            get { return GetBool(sectionEnhancements, "LypyL_EnhancedSky"); }
            set { SetBool(sectionEnhancements, "LypyL_EnhancedSky", value); }
        }

        public bool Nystul_IncreasedTerrainDistance
        {
            get { return GetBool(sectionEnhancements, "Nystul_IncreasedTerrainDistance"); }
            set { SetBool(sectionEnhancements, "Nystul_IncreasedTerrainDistance", value); }
        }

        public bool Nystul_RealtimeReflections
        {
            get { return GetBool(sectionEnhancements, "Nystul_RealtimeReflections"); }
            set { SetBool(sectionEnhancements, "Nystul_RealtimeReflections", value); }
        }

        public bool UncannyValley_RealGrass
        {
            get { return GetBool(sectionEnhancements, "UncannyValley_RealGrass"); }
            set { SetBool(sectionEnhancements, "UncannyValley_RealGrass", value); }
        }

        public bool UncannyValley_BirdsInDaggerfall
        {
            get { return GetBool(sectionEnhancements, "UncannyValley_BirdsInDaggerfall"); }
            set { SetBool(sectionEnhancements, "UncannyValley_BirdsInDaggerfall", value); }
        }

        #endregion

        #region Public Methods

        public void RereadSettings()
        {
            ReadSettings();
        }

        #endregion

        #region Private Methods

        void ReadSettings()
        {
            // Attempt to load settings.ini
            bool loadedUserSettings = false;
            string userIniPath = Path.Combine(Application.dataPath, settingsIniName);
            if (File.Exists(userIniPath))
            {
                userIniData = iniParser.ReadFile(userIniPath);
                loadedUserSettings = true;
            }

            // Load fallback.ini
            bool loadedFallbackSettings = false;
            TextAsset asset = Resources.Load<TextAsset>(fallbackIniName);
            if (asset != null)
            {
                MemoryStream stream = new MemoryStream(asset.bytes);
                StreamReader reader = new StreamReader(stream);
                fallbackIniData = iniParser.ReadData(reader);
                reader.Close();
                usingFallback = true;
                loadedFallbackSettings = true;

                // Create new settings.ini in data directory from fallback.ini if settings.ini not found
                if (!Application.isEditor && loadedFallbackSettings && !loadedUserSettings)
                    File.WriteAllBytes(Path.Combine(Application.dataPath, settingsIniName), asset.bytes);
            }

            // Report on primary ini file
            if (loadedUserSettings)
                DaggerfallUnity.LogMessage("Using settings.ini.");
            else if (!loadedUserSettings && loadedFallbackSettings)
                DaggerfallUnity.LogMessage("Using fallback.ini");
            else
                DaggerfallUnity.LogMessage("Failed to load fallback.ini.");
        }

        void WriteSettings()
        {
            if (iniParser != null && !usingFallback)
            {
                try
                {
                    string path = Path.Combine(Application.dataPath, settingsIniName);
                    if (File.Exists(path))
                    {
                        iniParser.WriteFile(path, userIniData);
                    }
                }
                catch
                {
                    DaggerfallUnity.LogMessage("Failed to write settings.ini.");
                }
            }
        }

        string GetData(string sectionName, string valueName)
        {
            try
            {
                if (userIniData != null)
                    return userIniData[sectionName][valueName];
                else
                    throw new Exception();
            }
            catch
            {
                if (fallbackIniData != null)
                    return fallbackIniData[sectionName][valueName];
                else
                    throw new Exception("GetData() could not find settings.ini or fallback.ini.");
            }
        }

        void SetData(string sectionName, string valueName, string valueData)
        {
            try
            {
                if (userIniData != null)
                    userIniData[sectionName][valueName] = valueData;
                else
                    throw new Exception();
            }
            catch
            {
                throw new Exception(string.Format("Failed to SetData() for value {0}.", valueName));
            }
        }

        string GetString(string sectionName, string valueName)
        {
            return GetData(sectionName, valueName);
        }

        void SetString(string sectionName, string valueName, string value)
        {
            SetData(sectionName, valueName, value);
        }

        bool GetBool(string sectionName, string valueName)
        {
            return bool.Parse(GetData(sectionName, valueName));
        }

        void SetBool(string sectionName, string valueName, bool value)
        {
            SetData(sectionName, valueName, value.ToString());
        }

        int GetInt(string sectionName, string valueName)
        {
            return int.Parse(GetData(sectionName, valueName));
        }

        int GetInt(string sectionName, string valueName, int min, int max)
        {
            int value = int.Parse(GetData(sectionName, valueName));
            return Mathf.Clamp(value, min, max);
        }

        void SetInt(string sectionName, string valueName, int value)
        {
            SetData(sectionName, valueName, value.ToString());
        }

        float GetFloat(string sectionName, string valueName)
        {
            return float.Parse(GetData(sectionName, valueName), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        float GetFloat(string sectionName, string valueName, float min, float max)
        {
            float value = float.Parse(GetData(sectionName, valueName), NumberStyles.Float, CultureInfo.InvariantCulture);
            return Mathf.Clamp(value, min, max);
        }

        void SetFloat(string sectionName, string valueName, float value)
        {
            SetData(sectionName, valueName, value.ToString());
        }

        Color GetColor(string sectionName, string valueName, Color defaultColor)
        {
            try
            {
                string colorStr = GetData(sectionName, valueName);
                Color result = StringToColor(colorStr);
                return result;
            }
            catch
            {
                return defaultColor;
            }
        }

        void SetColor(string sectionName, string valueName, Color value)
        {
            SetData(sectionName, valueName, ColorToString(value));
        }

        #endregion

        #region Custom Parsers

        string ColorToString(Color32 color)
        {
            return string.Format("{0:X02}{1:X02}{2:X02}{3:X02}", color.r, color.g, color.b, color.a);
        }

        Color32 StringToColor(string colorStr)
        {
            const string errorMsg = "Color must be in 32-bit RGBA format, e.g. black is 000000FF.";

            if (colorStr.Length != 8)
                throw new Exception(errorMsg);

            byte r = byte.Parse(colorStr.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(colorStr.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(colorStr.Substring(4, 2), NumberStyles.HexNumber);
            byte a = byte.Parse(colorStr.Substring(6, 2), NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }

        #endregion
    }
}