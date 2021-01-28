using BS_Utils.Utilities;
using IPA.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpectroSaber
{
	internal class SpectrogramGroup : MonoBehaviour
	{
		private MeshRenderer[] _meshRenderers;
		private LineRenderer[] _lineRenderers;
		private Material _barMaterial = null;

		private void CreateBarMaterial() {
			Shader shader = SpectroSaberController.Instance.assetBundle.LoadAsset<Shader>("Assets/Spectrogram/SpectroShader.shader");
			_barMaterial = new Material(shader);
		}

		public void InstantiateBars(GameObject barPrefab) {
			CreateBarMaterial();

			_meshRenderers = new MeshRenderer[64];
			_lineRenderers = new LineRenderer[64];

			for (int i = 0; i < _meshRenderers.Length; i++) {
				MeshRenderer barRenderer = Instantiate(barPrefab, new Vector3(0, 0, (i + 3) * 3), Quaternion.identity, transform).GetComponent<MeshRenderer>();
				barRenderer.sharedMaterial = _barMaterial;
				_meshRenderers[i] = barRenderer;
				_lineRenderers[i] = barRenderer.transform.GetChild(0).GetComponent<LineRenderer>();
				_lineRenderers[i].widthMultiplier = 0.05f;
			}
		}

		public void SetColor(Color color) {
			_barMaterial.color = color;
			Color brighter = color;
			brighter.r += 0.25f;
			brighter.g += 0.25f;
			brighter.b += 0.25f;
			_barMaterial.SetColor("_FresnelColor", brighter);
			for (int i = 0; i < _lineRenderers.Length; i++) {
				Color colorWithTransparency = color;
				colorWithTransparency.a = _lineRenderers[i].startColor.a;
				_lineRenderers[i].startColor = colorWithTransparency;
				_lineRenderers[i].endColor = colorWithTransparency;
			}
		}

		public void SetLightDir(Vector4 lightDir) {
			_barMaterial.SetVector("_LightDir", lightDir);
		}

		public void SetGlow(float glow) {
			_barMaterial.SetFloat("_Glow", glow);
		}

		public void SetAmbient(float ambient) {
			_barMaterial.SetFloat("_Ambient", ambient);
		}

		public void SetTransparency(float transparency) {
			_barMaterial.SetFloat("_Transparency", transparency);
			for (int i = 0; i < _meshRenderers.Length; i++) {
				Color colorWithTransparency = _lineRenderers[i].startColor;
				colorWithTransparency.a = transparency * 0.5f;
				_lineRenderers[i].startColor = colorWithTransparency;
				_lineRenderers[i].endColor = colorWithTransparency;
			}
		}

		public void UpdateMaterial() {
			for (int i = 0; i < _meshRenderers.Length; i++) {
				_meshRenderers[i].sharedMaterial = _barMaterial;
			}
		}

		public void UseGrabpass(bool useGrabpass) {
			if (useGrabpass) {
				_barMaterial.SetShaderPassEnabled("Always", true);
				_barMaterial.EnableKeyword("USE_GRABPASS");
			} else {
				_barMaterial.SetShaderPassEnabled("Always", false);
				_barMaterial.DisableKeyword("USE_GRABPASS");
			}
		}

		public void UseBloomPrePass(bool useBloom) {
			for (int i = 0; i < _lineRenderers.Length; i++) {
				_lineRenderers[i].enabled = useBloom;
			}
		}

		public void UpdateSpectogramData(List<float> samples) {
			for (int i = 0; i < _meshRenderers.Length; i++) {
				_meshRenderers[i].transform.localScale = new Vector3(Plugin.Settings.Thickness, samples[i] * Plugin.Settings.Intensity, Plugin.Settings.Thickness);
				if (_lineRenderers[i].enabled) {
					_lineRenderers[i].transform.localPosition = new Vector3(0, -samples[i] * 0.1f, 0);
					_lineRenderers[i].transform.localScale = new Vector3(1, 1 + samples[i] * 0.2f, 1);
					_lineRenderers[i].widthMultiplier = 0.05f * Plugin.Settings.Thickness;
				}
			}
		}
	}
}
