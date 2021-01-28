using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BS_Utils.Utilities;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace SpectroSaber.Settings.UI
{
	internal class PreviewViewController : BSMLResourceViewController
	{
		public override string ResourceName { get { return "SpectroSaber.Settings.UI.Preview.bsml"; } }
		public static PreviewViewController Instance { get; set; }

		public GameObject previewParent;
		public GameObject previewSaber;
		public bool generatingPreview;
		public bool doneGeneratingPreview;

		private AudioSource _audioSource;
		private BasicSpectrogramData _basicSpectrogramData;

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
			base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

			Instance = this;

			if (!previewParent) {
				previewParent = new GameObject("SSPreviewParent");
				previewParent.transform.position = new Vector3(-3f, 1.3f, 1f);
				previewParent.transform.Rotate(0f, 30f, 0f);
			}
			doneGeneratingPreview = false;
			GeneratePreview();

			StartCoroutine(IEInstantiateAudioSource());
		}

		protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling) {
			base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
			ClearPreview();
			Instance = null;
		}

		public void UpdatePreviewSpectrogram() {
			if (_basicSpectrogramData == null) {
				_basicSpectrogramData = new GameObject("SSBasicSpectrogramData").AddComponent<BasicSpectrogramData>();
			} else {
				if (_audioSource != null) {
					if (_basicSpectrogramData.GetField<AudioSource>("_audioSource") == null) {
						_basicSpectrogramData.SetField("_audioSource", _audioSource);
					}
					_audioSource.volume = Mathf.Lerp(_audioSource.volume, 0.8f, Time.deltaTime * 3);
					if (SpectrogramManager.Instance.leftSpectro != null) {
						SpectrogramManager.Instance.leftSpectro.UpdateSpectogramData(_basicSpectrogramData.ProcessedSamples);
					}
				}
			}
		}

		private void ClearSabers() {
			if(previewSaber)
				DestroyImmediate(previewSaber);
			previewSaber = null;
			if(SpectrogramManager.Instance.leftSpectro)
				DestroyImmediate(SpectrogramManager.Instance.leftSpectro);
		}

		private void ClearPreview() {
			ClearSabers();
			if(_audioSource)
				DestroyImmediate(_audioSource);
			_audioSource = null;
			if (_basicSpectrogramData)
				DestroyImmediate(_basicSpectrogramData);
			_basicSpectrogramData = null;
		}

		public void GeneratePreview() {
			StartCoroutine(IEGeneratePreview());
		}

		IEnumerator IEGeneratePreview() {
			if (!generatingPreview) {
				yield return new WaitUntil(() => DefaultSaberGrabber.isCompleted);
				try {
					generatingPreview = true;
					ClearSabers();

					previewSaber = Instantiate(DefaultSaberGrabber.defaultLeftSaber, previewParent.transform);
					previewSaber.transform.localPosition = Vector3.zero;
					previewSaber.transform.localRotation = Quaternion.identity;
					previewSaber.transform.localScale = Vector3.one * 2;
					previewSaber.name = "SSPreviewSaber";
					previewSaber.SetActive(true);

					SpectrogramManager.Instance.InstantiateLeftSpectrogramGroup();
					SpectrogramManager.Instance.SetPreviewSpectrogramColors();
					SpectrogramManager.Instance.ParentAndPositionSpectrogramGroup(SpectrogramManager.Instance.leftSpectro, previewSaber.transform, previewSaber.transform.position, previewSaber.transform.rotation, Vector3.one * 0.005f);
					List<float> tmpSamples = new float[64].ToList();
					SpectrogramManager.Instance.UpdateSpectrogramData(tmpSamples);

					yield break;
				} catch (Exception e) {
					Plugin.Log.Error(e);
				} finally {
					doneGeneratingPreview = true;
					generatingPreview = false;
				}
			}
			yield break;
		}

		IEnumerator IEInstantiateAudioSource() {
			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<BeatmapLevelsModel>().Any());
			BeatmapLevelsModel levelsModel = Resources.FindObjectsOfTypeAll<BeatmapLevelsModel>().FirstOrDefault();
			BeatmapLevelPackCollectionSO packCollectionSO = levelsModel.ostAndExtrasPackCollection;
			BeatmapLevelPackSO[] levelPack = packCollectionSO.GetField<BeatmapLevelPackSO[]>("_beatmapLevelPacks");
			BeatmapLevelCollectionSO levelCollectionSO = levelPack[0].GetField<BeatmapLevelCollectionSO>("_beatmapLevelCollection");
			BeatmapLevelSO[] levels = levelCollectionSO.GetField<BeatmapLevelSO[]>("_beatmapLevels");
			AudioClip clip = levels[0].GetField<AudioClip>("_audioClip");
			_audioSource = new GameObject("SSAudSource").AddComponent<AudioSource>();
			_audioSource.clip = clip;
			_audioSource.spatialBlend = 0;
			_audioSource.loop = true;
			_audioSource.volume = 0;
			_audioSource.time = 15f;
			_audioSource.Play();
		}
	}
}
