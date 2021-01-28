using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using HMUI;

namespace SpectroSaber.Settings.UI
{
	internal class SettingsUI
	{
		public static bool buttonCreated;
		public static SettingsFlowCoordinator flowCoordinator;

		public static void CreateButton() {
			if (!buttonCreated) {
				MenuButton menuButton = new MenuButton("SpectroSaber", "Edit your sabers' visualizers here!", new Action(OnMenuButtonPressed), true);
				PersistentSingleton<MenuButtons>.instance.RegisterButton(menuButton);
				buttonCreated = true;
			}
		}

		public static void CreateFlowCoordinator() {
			if (flowCoordinator == null)
				flowCoordinator = BeatSaberUI.CreateFlowCoordinator<SettingsFlowCoordinator>();
			BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator, null, ViewController.AnimationDirection.Horizontal, false, false);
		}

		private static void OnMenuButtonPressed() {
			CreateFlowCoordinator();
		}
	}
}
