using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpectroSaber.Settings.UI
{
	internal class SettingsViewController : BSMLResourceViewController
	{
		public override string ResourceName { get { return "SpectroSaber.Settings.UI.Settings.bsml"; } }

		[UIValue("enabled")]
		public bool Enabled
		{
			get {
				return Plugin.Settings.Enabled;
			}
			set {
				Plugin.Settings.Enabled = value;
			}
		}

		[UIValue("transparency")]
		public float Transparency 
		{ 
			get 
			{
				return Plugin.Settings.Transparency;
			} 
			set 
			{
				Plugin.Settings.Transparency = value;
			} 
		}

		[UIValue("intensity")]
		public int Intensity
		{
			get {
				return Plugin.Settings.Intensity;
			}
			set {
				Plugin.Settings.Intensity = value;
			}
		}

		[UIValue("ss-thickness")]
		public float Thickness
		{
			get {
				return Plugin.Settings.Thickness;
			}
			set {
				Plugin.Settings.Thickness = value;
			}
		}

		[UIValue("trans-modes")]
		public List<object> TransModes = new object[] { "Dither", "GrabPass" }.ToList();

		[UIValue("trans-mode")]
		public string TransMode
		{
			get {
				return Plugin.Settings.TransMode;
			}
			set {
				Plugin.Settings.TransMode = value;
			}
		}

		[UIValue("y-offset")]
		public float YOffset
		{
			get {
				return Plugin.Settings.YOffset;
			}
			set {
				Plugin.Settings.YOffset = value;
			}
		}

		[UIValue("z-offset")]
		public float ZOffset
		{
			get {
				return Plugin.Settings.ZOffset;
			}
			set {
				Plugin.Settings.ZOffset = value;
			}
		}

		[UIValue("rotation")]
		public int Rotation
		{
			get {
				return Plugin.Settings.Rotation;
			}
			set {
				Plugin.Settings.Rotation = value;
			}
		}

		[UIValue("bloom")]
		public bool BloomPrePass
		{
			get {
				return Plugin.Settings.BloomPrePass;
			}
			set {
				Plugin.Settings.BloomPrePass = value;
			}
		}

		[UIAction("setting-changed")]
		private void SettingChanged(object value) {
			StartCoroutine(ApplySettings());
		}

		IEnumerator ApplySettings() {
			yield return new WaitForSecondsRealtime(0.1f);
			SpectrogramManager.Instance.UpdateSpectrogramGroupProperties();
		}
	}
}
