using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpectroSaber
{
	internal class ColorSchemeManager
	{
		public static ColorScheme GetMainColorScheme() {
			ColorSchemesSettings colorSchemesSettings = ReflectionUtil.GetField<PlayerData, PlayerDataModel>(Resources.FindObjectsOfTypeAll<PlayerDataModel>().FirstOrDefault(), "_playerData").colorSchemesSettings;
			ColorScheme colorScheme = colorSchemesSettings.GetSelectedColorScheme();
			return colorScheme;
		}
	}
}
