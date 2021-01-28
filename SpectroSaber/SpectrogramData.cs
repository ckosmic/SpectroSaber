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

		private void Awake() {
			DontDestroyOnLoad(this);
			Instance = this;
		}

		public List<float> GetProcessedSamples() {
			if (_spectrogramData != null)
				return _spectrogramData.ProcessedSamples;
			else
				Plugin.Log.Error("Processed samples is null!");
				
			return null;
		}

		public void GetBasicSpectrumData(Action callback) {
			StartCoroutine(IEGetBasicSpectrumData(callback));
		}

		IEnumerator IEGetBasicSpectrumData(Action callback) {
			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<BasicSpectrogramData>().Any());
			_spectrogramData = Resources.FindObjectsOfTypeAll<BasicSpectrogramData>().FirstOrDefault();
			callback();
		}
	}
}
