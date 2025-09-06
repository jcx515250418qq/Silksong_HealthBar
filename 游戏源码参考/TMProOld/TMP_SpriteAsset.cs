using System.Collections.Generic;
using UnityEngine;

namespace TMProOld
{
	public class TMP_SpriteAsset : TMP_Asset
	{
		public static TMP_SpriteAsset m_defaultSpriteAsset;

		public Texture spriteSheet;

		public List<TMP_Sprite> spriteInfoList;

		private List<Sprite> m_sprites;

		public static TMP_SpriteAsset defaultSpriteAsset
		{
			get
			{
				if (m_defaultSpriteAsset == null)
				{
					m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
				}
				return m_defaultSpriteAsset;
			}
		}

		private void OnEnable()
		{
		}

		private Material GetDefaultSpriteMaterial()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			Material obj = new Material(Shader.Find("TextMeshPro/Sprite"));
			obj.SetTexture(ShaderUtilities.ID_MainTex, spriteSheet);
			obj.hideFlags = HideFlags.HideInHierarchy;
			return obj;
		}

		public int GetSpriteIndex(int hashCode)
		{
			for (int i = 0; i < spriteInfoList.Count; i++)
			{
				if (spriteInfoList[i].hashCode == hashCode)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
