using IPA.Config.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SpectroSaber.Settings
{
	internal class SettingsStore
	{

		public bool Enabled { get; set; } = true;

		public float Transparency { get; set; } = 0.5f;

		public int Intensity { get; set; } = 20;

		public float Thickness { get; set; } = 1.0f;

		public string TransMode { get; set; } = "GrabPass";

		public float YOffset { get; set; } = 0;

		public float ZOffset { get; set; } = 0;

		public int Rotation { get; set; } = 60;

		public bool BloomPrePass { get; set; } = true;
	}
}
