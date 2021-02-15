using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using UnityEngine;

namespace SpectroSaber
{
	internal class SpectrogramData : MonoBehaviour {
		public static SpectrogramData Instance { get; private set; }

		public BasicSpectrogramData basicSpectrogramData;

		private bool _isReady = false;

		private void Awake() {
			DontDestroyOnLoad(this);
			Instance = this;
		}

		public List<float> GetProcessedSamples() {
			if (basicSpectrogramData != null) {
				try {
					return basicSpectrogramData.ProcessedSamples;
				} catch (Exception e) {
					Plugin.Log.Error("Failed to retrieve processed samples: " + e);
					return null;
				}
			} else {
				Plugin.Log.Error("BasicSpectrogramData object is null, can't get processed samples!");
				return null;
			}
		}

		public void SetBasicSpectrogramDataAudioSourceGame(Action callback) {
			StartCoroutine(IESetBasicSpectrogramDataAudioSourceGame(callback));
		}

		public void CreateBasicSpectrogramData() {
			basicSpectrogramData = new GameObject("SSBasicSpectrogramData").AddComponent<BasicSpectrogramData>();
			DontDestroyOnLoad(basicSpectrogramData);
			UpdateBasicSpectrogramData();
		}

		public void UpdateBasicSpectrogramData() {
			if (basicSpectrogramData != null) {
				List<float> processedSamplesList = new List<float>(Plugin.Settings.BarCount);
				for (int i = 0; i < Plugin.Settings.BarCount; i++) {
					processedSamplesList.Add(0f);
				}
				basicSpectrogramData.SetField<BasicSpectrogramData, List<float>>("_processedSamples", processedSamplesList);
				basicSpectrogramData.SetField<BasicSpectrogramData, float[]>("_samples", new float[Plugin.Settings.BarCount]);
				basicSpectrogramData.SetField<BasicSpectrogramData, bool>("_hasData", false);
				basicSpectrogramData.SetField<BasicSpectrogramData, bool>("_hasProcessedData", false);
			} else {
				Plugin.Log.Error("BasicSpectrogramData object is null, can't reinitialize sample lists!");
			}
		}

		public void SetBasicSpectrogramDataAudioSource(AudioSource aud) {
			basicSpectrogramData.SetField<BasicSpectrogramData, AudioSource>("_audioSource", aud);
		}

		IEnumerator IESetBasicSpectrogramDataAudioSourceGame(Action callback) {
			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<BasicSpectrogramData>().Any());
			BasicSpectrogramData gameSpectrogramData = Resources.FindObjectsOfTypeAll<BasicSpectrogramData>().First(bsd => bsd.transform.name != "SSBasicSpectrogramData");
			yield return new WaitUntil(() => gameSpectrogramData.GetField<List<float>, BasicSpectrogramData>("_processedSamples").Count == gameSpectrogramData.Samples.Length);
			AudioSource gameAudioSource = gameSpectrogramData.GetField<AudioSource, BasicSpectrogramData>("_audioSource");
			SetBasicSpectrogramDataAudioSource(gameAudioSource);
			callback();
		}
	}
}
