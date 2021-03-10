﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using BS_Utils;
using BeatSaberMarkupLanguage.Settings;
using SpectroSaber.Settings.UI;
using SpectroSaber.Settings;

namespace SpectroSaber
{

	[Plugin(RuntimeOptions.SingleStartInit)]
	public class Plugin
	{
		internal static Plugin Instance { get; private set; }
		internal static IPALogger Log { get; private set; }
		internal static SettingsStore Settings { get; private set; }

		[Init]
		/// <summary>
		/// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
		/// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
		/// Only use [Init] with one Constructor.
		/// </summary>
		public void Init(IPALogger logger, Config config) {
			Instance = this;
			Log = logger;

			Settings = config.Generated<SettingsStore>();

			Log.Info("SpectroSaber initialized.");
		}

		#region BSIPA Config
		//Uncomment to use BSIPA's config
		/*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
		#endregion

		[OnStart]
		public void OnApplicationStart() {
			Log.Debug("OnApplicationStart");
			new GameObject("SpectroSaberController").AddComponent<SpectroSaberController>();
			new GameObject("SSSaberManager").AddComponent<SaberManager>();
			new GameObject("SSSpectrogramManager").AddComponent<SpectrogramManager>();
			SpectrogramData sd = new GameObject("SSSpectrogramData").AddComponent<SpectrogramData>();
			sd.CreateBasicSpectrogramData();

			SettingsUI.CreateButton();

			BS_Utils.Utilities.BSEvents.gameSceneLoaded += SpectroSaberController.Instance.OnGameSceneLoaded;
			BS_Utils.Utilities.BSEvents.LevelFinished += SpectroSaberController.Instance.OnLevelDidFinish;
		}

		[OnExit]
		public void OnApplicationQuit() {
			Log.Debug("OnApplicationQuit");

			BS_Utils.Utilities.BSEvents.gameSceneLoaded -= SpectroSaberController.Instance.OnGameSceneLoaded;
			BS_Utils.Utilities.BSEvents.LevelFinished -= SpectroSaberController.Instance.OnLevelDidFinish;
		}
	}
}
