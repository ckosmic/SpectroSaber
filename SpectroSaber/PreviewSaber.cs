using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IPA.Utilities;

namespace SpectroSaber
{
	internal class PreviewSaber : MonoBehaviour
	{

		private void Start() {
			ColorScheme colorScheme = ColorSchemeManager.GetMainColorScheme();
			Color color = colorScheme.saberAColor;

			Renderer[] childrenRenderers = gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < childrenRenderers.Length; i++) {
				foreach (Material mat in childrenRenderers[i].materials) {
					mat.color = color;
					if (mat.HasProperty("_Color"))
						mat.SetColor("_Color", color);
					if (mat.HasProperty("_TintColor"))
						mat.SetColor("_TintColor", color);
					if (mat.HasProperty("_AddColor"))
						mat.SetColor("_AddColor", (color * 0.5f).ColorWithAlpha(0f));
				}
			}
		}

	}
}
