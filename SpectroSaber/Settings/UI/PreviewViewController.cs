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
		private float _maxPreviewVolume = 0.5f;
		private SongPreviewPlayer _songPreviewPlayer;

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
			_songPreviewPlayer.CrossFadeToDefault();
			ClearPreview();
			Instance = null;
		}

		public void UpdatePreviewSpectrogram() {
			if (SpectrogramData.Instance.basicSpectrogramData != null) {
				if (_audioSource != null) {
					SpectrogramData.Instance.SetBasicSpectrogramDataAudioSource(_audioSource);
					_audioSource.volume = Mathf.Lerp(_audioSource.volume, _maxPreviewVolume, Time.deltaTime * 3);
					if (SpectrogramManager.Instance.leftSpectro != null) {
						SpectrogramManager.Instance.leftSpectro.UpdateSpectogramData(SpectrogramData.Instance.basicSpectrogramData.ProcessedSamples);
					}
				}
			}
		}

		public void UpdateBasicSpectrogramData() {
			SpectrogramData.Instance.UpdateBasicSpectrogramData();
		}

		public void ResetPreviewSpectrogram() {
			if (SpectrogramManager.Instance.leftSpectro) {
				SpectrogramManager.Instance.leftSpectro.DestroyBars();
				DestroyImmediate(SpectrogramManager.Instance.leftSpectro);
			}
			SpectrogramManager.Instance.InstantiateLeftSpectrogramGroup();
			SpectrogramManager.Instance.SetPreviewSpectrogramColors();
			SpectrogramManager.Instance.ParentAndPositionSpectrogramGroup(SpectrogramManager.Instance.leftSpectro, previewSaber.transform, previewSaber.transform.position, previewSaber.transform.rotation, Vector3.one * 0.005f);
			List<float> tmpSamples = new float[Plugin.Settings.BarCount].ToList();
			SpectrogramManager.Instance.UpdateSpectrogramData(tmpSamples);
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
			if (SpectrogramData.Instance.basicSpectrogramData)
				SpectrogramData.Instance.SetBasicSpectrogramDataAudioSource(null);
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

					ResetPreviewSpectrogram();

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
			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().Any());
			MainSettingsModelSO mainSettings = Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().FirstOrDefault();
			_maxPreviewVolume = mainSettings.volume * 0.5f;

			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().Any());
			_songPreviewPlayer = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().FirstOrDefault();
			_songPreviewPlayer.FadeOut(1);

			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<BeatmapLevelsModel>().Any());
			BeatmapLevelsModel levelsModel = Resources.FindObjectsOfTypeAll<BeatmapLevelsModel>().FirstOrDefault();
			BeatmapLevelPackCollectionSO packCollectionSO = levelsModel.ostAndExtrasPackCollection;
			BeatmapLevelPackSO[] levelPack = packCollectionSO.GetField<BeatmapLevelPackSO[]>("_beatmapLevelPacks");
			BeatmapLevelCollectionSO levelCollectionSO = levelPack[5].GetField<BeatmapLevelCollectionSO>("_beatmapLevelCollection");
			BeatmapLevelSO[] levels = levelCollectionSO.GetField<BeatmapLevelSO[]>("_beatmapLevels");

			AudioClip clip = levels[1].GetField<AudioClip>("_audioClip");
			_audioSource = new GameObject("SSAudSource").AddComponent<AudioSource>();
			_audioSource.clip = clip;
			_audioSource.spatialBlend = 0;
			_audioSource.loop = true;
			_audioSource.volume = 0;
			_audioSource.time = 20f;
			_audioSource.Play();
		}
	}
}
