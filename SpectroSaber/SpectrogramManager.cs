using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BS_Utils.Utilities;
using SiraUtil.Interfaces;
using SiraUtil;

namespace SpectroSaber
{

	internal class SpectrogramManager : MonoBehaviour
	{
		public static SpectrogramManager Instance { get; private set; }

		public SpectrogramGroup leftSpectro;
		public SpectrogramGroup rightSpectro;

		private GameObject _spectroBarPrefab;

		private void Awake() {
			DontDestroyOnLoad(this);
			Instance = this;
		}

		private void LoadSpectrogramPrefab() {
			if(!_spectroBarPrefab)
				_spectroBarPrefab = SpectroSaberController.Instance.assetBundle.LoadAsset<GameObject>("Assets/Spectrogram/SpectroBar.prefab");
		}

		private Vector3 GetSaberLocalPositionTarget() {
			return new Vector3(0, Plugin.Settings.YOffset / 10f, Plugin.Settings.ZOffset);
		}

		public void InstantiateSpectrogramGroups() {
			InstantiateLeftSpectrogramGroup();
			InstantiateRightSpectrogramGroup();
		}

		public bool ShouldUseGrabpass() {
			return Plugin.Settings.TransMode == "GrabPass" && Plugin.Settings.Transparency < 1.0f;
		}

		public void InstantiateLeftSpectrogramGroup() {
			LoadSpectrogramPrefab();

			leftSpectro = new GameObject("LeftSpectrogram").AddComponent<SpectrogramGroup>();
			leftSpectro.InstantiateBars(_spectroBarPrefab);
			leftSpectro.SetLightDir(new Vector4(1, -1, 0, 1));
			leftSpectro.SetAmbient(0.25f);
			leftSpectro.SetGlow(0.25f);
			leftSpectro.SetTransparency(Plugin.Settings.Transparency);
			leftSpectro.UseGrabpass(ShouldUseGrabpass());
			leftSpectro.UseBloomPrePass(Plugin.Settings.BloomPrePass);
		}

		public void InstantiateRightSpectrogramGroup() {
			LoadSpectrogramPrefab();

			rightSpectro = new GameObject("RightSpectrogram").AddComponent<SpectrogramGroup>();
			rightSpectro.InstantiateBars(_spectroBarPrefab);
			rightSpectro.SetLightDir(new Vector4(-1, -1, 0, 1));
			rightSpectro.SetAmbient(0.25f);
			rightSpectro.SetGlow(0.25f);
			rightSpectro.SetTransparency(Plugin.Settings.Transparency);
			rightSpectro.UseGrabpass(ShouldUseGrabpass());
			rightSpectro.UseBloomPrePass(Plugin.Settings.BloomPrePass);
		}

		public void UpdateSpectrogramGroupProperties() {
			if (leftSpectro) {
				leftSpectro.SetTransparency(Plugin.Settings.Transparency);
				leftSpectro.UseGrabpass(ShouldUseGrabpass());
				leftSpectro.transform.localPosition = GetSaberLocalPositionTarget();
				leftSpectro.UseBloomPrePass(Plugin.Settings.BloomPrePass);

				leftSpectro.UpdateMaterial();
			}
			if (rightSpectro) {
				rightSpectro.SetTransparency(Plugin.Settings.Transparency);
				rightSpectro.UseGrabpass(ShouldUseGrabpass());
				rightSpectro.transform.localPosition = GetSaberLocalPositionTarget();
				rightSpectro.UseBloomPrePass(Plugin.Settings.BloomPrePass);

				rightSpectro.UpdateMaterial();
			}
		}

		public void ParentSpectrogramsToSabers() {
			Saber leftSaber = SaberManager.Instance.leftSaber;
			Saber rightSaber = SaberManager.Instance.rightSaber;

			ParentAndPositionSpectrogramGroup(leftSpectro, leftSaber.transform, leftSaber.handlePos, leftSaber.handleRot * Quaternion.Euler(0, 0, -Plugin.Settings.Rotation), Vector3.one * 0.005f);
			ParentAndPositionSpectrogramGroup(rightSpectro, rightSaber.transform, rightSaber.handlePos, rightSaber.handleRot * Quaternion.Euler(0, 0, Plugin.Settings.Rotation), Vector3.one * 0.005f);
		}

		public void ParentAndPositionSpectrogramGroup(SpectrogramGroup group, Transform parent, Vector3 position, Quaternion rotation, Vector3 localScale) {
			if (group) {
				group.transform.SetParent(parent);
				group.transform.position = position;
				group.transform.localPosition = GetSaberLocalPositionTarget();
				group.transform.rotation = rotation;
				group.transform.localScale = localScale;
			}
		}

		public void UpdateSpectrogramData() {
			List<float> samples = SpectrogramData.Instance.GetProcessedSamples();
			if (leftSpectro)
				leftSpectro.UpdateSpectogramData(samples);
			if (rightSpectro)
				rightSpectro.UpdateSpectogramData(samples);
		}

		public void UpdateSpectrogramData(List<float> samples) {
			if(leftSpectro)
				leftSpectro.UpdateSpectogramData(samples);
			if(rightSpectro)
				rightSpectro.UpdateSpectogramData(samples);
		}

		public void CleanupSpectrogramGroups() {
			if (leftSpectro)
				Destroy(leftSpectro);
			if (rightSpectro)
				Destroy(rightSpectro);
			leftSpectro = null;
			rightSpectro = null;
		}

		public bool SpectrosLoaded() {
			return leftSpectro != null && rightSpectro != null;
		}

		public void SetSpectrogramColors() {
			foreach (Saber saber in Resources.FindObjectsOfTypeAll<Saber>()) {
				if (saber != null && saber.isActiveAndEnabled) {
					if (saber.saberType == SaberType.SaberB) {
						leftSpectro.SetColor(saber.GetColor());
					} else if (saber.saberType == SaberType.SaberA) {
						rightSpectro.SetColor(saber.GetColor());
					}
				}
			}
		}

		public void SetPreviewSpectrogramColors() {
			ColorScheme colorScheme = ColorSchemeManager.GetMainColorScheme();
			if (leftSpectro)
				leftSpectro.SetColor(colorScheme.saberAColor);
			if (rightSpectro)
				rightSpectro.SetColor(colorScheme.saberBColor);
		}
	}
}
