using BS_Utils.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpectroSaber
{
	/// <summary>
	/// Monobehaviours (scripts) are added to GameObjects.
	/// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
	/// </summary>
	public class SpectroSaberController : MonoBehaviour
	{
		public static SpectroSaberController Instance { get; private set; }

		public AssetBundle assetBundle;

		#region Monobehaviour Messages
		private void Awake() {
			if (Instance != null) {
				Plugin.Log?.Warn($"Instance of {GetType().Name} already exists, destroying.");
				GameObject.DestroyImmediate(this);
				return;
			}
			GameObject.DontDestroyOnLoad(this);
			Instance = this;

			assetBundle = AssetBundleManager.LoadAssetBundleFromResource($"SpectroSaber.spectrosaber");
		}

		private void Update() {
			if (Plugin.Settings.Enabled) {
				if (SpectrogramManager.Instance.SpectrosLoaded()) {
					SpectrogramManager.Instance.UpdateSpectrogramData();
				}
			}
			if (Settings.UI.PreviewViewController.Instance) {
				Settings.UI.PreviewViewController.Instance.UpdatePreviewSpectrogram();
			}
		}

		private void OnDestroy() {
			if (Instance == this)
				Instance = null; // This MonoBehaviour is being destroyed, so set the static instance property to null.

		}
		#endregion

		public void OnGameSceneLoaded() {
			if (Plugin.Settings.Enabled) {
				SaberManager.Instance.GetSabers(() => {
					SpectrogramData.Instance.GetBasicSpectrumData(() => {
						SpectrogramManager.Instance.InstantiateSpectrogramGroups();
						SpectrogramManager.Instance.ParentSpectrogramsToSabers();
						SpectrogramManager.Instance.SetSpectrogramColors();
					});
				});
			}
		}

		public void OnLevelDidFinish(object sender, LevelFinishedEventArgs args) {
			if (Plugin.Settings.Enabled) {
				SpectrogramManager.Instance.CleanupSpectrogramGroups();
			}
		}
	}
}
