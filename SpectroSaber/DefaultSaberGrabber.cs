using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

// Mostly from the Custom Saber plugin by nalulululuna

namespace SpectroSaber
{
	public class DefaultSaberGrabber : MonoBehaviour
	{
		public static bool isCompleted { get; private set; }

		public static GameObject defaultLeftSaber;
		public static GameObject defaultRightSaber;

		private void Awake() {
			DontDestroyOnLoad(this);
			if (!isCompleted) {
				StartCoroutine(PreloadDefaultSabers());
			}
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}

		private void OnActiveSceneChanged(Scene current, Scene next) {
			if (next.name == "PCInit") {
				if (defaultLeftSaber != null)
					Destroy(defaultLeftSaber);
				if (defaultRightSaber != null)
					Destroy(defaultRightSaber);
				isCompleted = false;
			}
		}

		private IEnumerator PreloadDefaultSabers() {
			bool sceneLoaded = false;
			try {
				string sceneName = "StandardGameplay";
				AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
				while (!loadScene.isDone) {
					yield return null;
				}
				sceneName = "GameCore";
				loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
				while (!loadScene.isDone) {
					yield return null;
				}
				sceneLoaded = true;
				yield return new WaitForSecondsRealtime(0.1f);

				SaberModelController saberModelController = Resources.FindObjectsOfTypeAll<SaberModelController>().FirstOrDefault();

				defaultLeftSaber = Instantiate(saberModelController).gameObject;
				DestroyImmediate(defaultLeftSaber.GetComponent<SaberModelController>());
				DestroyImmediate(defaultLeftSaber.GetComponentInChildren<ConditionalMaterialSwitcher>());
				SetSaberGlowColor[] componentsInChildren = defaultLeftSaber.GetComponentsInChildren<SetSaberGlowColor>();
				for (int i = 0; i < componentsInChildren.Length; i++) {
					DestroyImmediate(componentsInChildren[i]);
				}
				SetSaberFakeGlowColor[] componentsInChildren2 = defaultLeftSaber.GetComponentsInChildren<SetSaberFakeGlowColor>();
				for (int i = 0; i < componentsInChildren2.Length; i++) {
					DestroyImmediate(componentsInChildren2[i]);
				}
				SaberTrail[] componentsInChildren3 = defaultLeftSaber.GetComponentsInChildren<SaberTrail>();
				for (int i = 0; i < componentsInChildren3.Length; i++) {
					DestroyImmediate(componentsInChildren3[i]);
				}
				DontDestroyOnLoad(defaultLeftSaber);
				defaultLeftSaber.transform.SetParent(transform);
				defaultLeftSaber.gameObject.name = "SSLeftSaber";
				defaultLeftSaber.transform.localPosition = Vector3.zero;
				defaultLeftSaber.transform.localRotation = Quaternion.identity;
				defaultLeftSaber.AddComponent<PreviewSaber>();

				if (defaultLeftSaber) {
					defaultLeftSaber.SetActive(false);
					isCompleted = true;
				}
				
				loadScene = null;
			} finally {
				if (sceneLoaded) {
					string sceneName = "StandardGameplay";
					SceneManager.UnloadSceneAsync(sceneName);
					sceneName = "GameCore";
					SceneManager.UnloadSceneAsync(sceneName);
				}
			}
			yield break;
		}
	}
}
