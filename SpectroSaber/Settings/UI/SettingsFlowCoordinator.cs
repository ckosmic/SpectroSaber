using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpectroSaber.Settings.UI
{
	internal class SettingsFlowCoordinator : FlowCoordinator
	{
		private SettingsViewController _settingsViewController;
		private PreviewViewController _previewViewController;

		public void Awake() {
			if (!_settingsViewController)
				_settingsViewController = BeatSaberUI.CreateViewController<SettingsViewController>();
			if (!_previewViewController)
				_previewViewController = BeatSaberUI.CreateViewController<PreviewViewController>();
		}

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
			try {
				if (firstActivation) {
					SetTitle("SpectroSaber", ViewController.AnimationType.In);
					showBackButton = true;
					ProvideInitialViewControllers(_settingsViewController, _previewViewController, null, null, null);
					DontDestroyOnLoad(new GameObject("SSDefaultSaberGrabber").AddComponent<DefaultSaberGrabber>());
				}
			} catch (Exception e) {
				Plugin.Log.Error(e);
			}
		}

		protected override void BackButtonWasPressed(ViewController topViewController) {
			BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Horizontal, false);
		}
	}
}
