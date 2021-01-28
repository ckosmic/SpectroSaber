using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpectroSaber
{
	/// <summary>
	/// Monobehaviours (scripts) are added to GameObjects.
	/// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
	/// </summary>
	internal class SaberManager : MonoBehaviour
	{
		public static SaberManager Instance { get; private set; }

		public Saber leftSaber;
		public Saber rightSaber;

		private void Awake() {
			DontDestroyOnLoad(this);
			Instance = this;
		}

		public void GetSabers(Action callback) {
			StartCoroutine(IEGetSabers(callback));
		}

		IEnumerator IEGetSabers(Action callback) {
			yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());
			Saber[] sabers = Resources.FindObjectsOfTypeAll<Saber>();
			leftSaber = sabers[0];
			rightSaber = sabers[1];
			callback();
		}
	}
}
