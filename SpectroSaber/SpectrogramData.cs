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
	internal class SpectrogramData : MonoBehaviour
	{
		public static SpectrogramData Instance { get; private set; }

		private BasicSpectrogramData _spectrogramData;
		private bool _isReady = false;

		private void Awake() {
			DontDestroyOnLoad(this);
			Instance = this;
		}

		public List<float> GetProcessedSamples() {
			if (_spectrogramData != null && _isReady) {
				try {
					return _spectrogramData.ProcessedSamples;
				} catch (Exception e) {
					Plugin.Log.Error("Failed to retrieve processed samples: " + e);
					return null;
				}
			} else {
				Plugin.Log.Error("BasicSpectrogramData object is null!");
				return null;
			}
		}

		public void GetBasicSpectrumData(Action callback) {
			StartCoroutine(IEGetBasicSpectrumData(callback));
		}

		IEnumerator IEGetBasicSpectrumData(Action callback) {
			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<BasicSpectrogramData>().Any());
			_spectrogramData = Resources.FindObjectsOfTypeAll<BasicSpectrogramData>().FirstOrDefault();
			yield return new WaitUntil(() => _spectrogramData.GetField<List<float>, BasicSpectrogramData>("_processedSamples").Count == _spectrogramData.Samples.Length);
			_isReady = true;
			callback();
		}
	}
}
