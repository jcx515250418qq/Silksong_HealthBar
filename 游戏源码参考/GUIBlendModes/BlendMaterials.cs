using UnityEngine;

namespace GUIBlendModes
{
	public static class BlendMaterials
	{
		public static Material[] Materials;

		public static bool Initialized;

		public static void Initialize()
		{
			Materials = new Material[84];
			for (int i = 0; i < 21; i++)
			{
				Materials[i] = Resources.Load<Material>("UIBlend" + (BlendMode)(i + 1)/*cast due to .constrained prefix*/);
			}
			for (int j = 21; j < 42; j++)
			{
				Materials[j] = Resources.Load<Material>("UIBlend" + ((BlendMode)(j - 20)/*cast due to .constrained prefix*/).ToString() + "Optimized");
			}
			for (int k = 42; k < 63; k++)
			{
				Materials[k] = Resources.Load<Material>("UIFontBlend" + (BlendMode)(k - 41)/*cast due to .constrained prefix*/);
			}
			for (int l = 63; l < 84; l++)
			{
				Materials[l] = Resources.Load<Material>("UIFontBlend" + ((BlendMode)(l - 62)/*cast due to .constrained prefix*/).ToString() + "Optimized");
			}
			Initialized = true;
		}

		public static Material GetMaterial(BlendMode mode, bool font, bool optimized)
		{
			if (!Initialized)
			{
				Initialize();
			}
			if (font)
			{
				if (mode != 0)
				{
					return Materials[(int)(mode - 1 + (optimized ? 63 : 42))];
				}
				return null;
			}
			if (mode != 0)
			{
				return Materials[(int)(mode - 1 + (optimized ? 21 : 0))];
			}
			return null;
		}
	}
}
