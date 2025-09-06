using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public class CorpseLandTint : MonoBehaviour
{
	private tk2dSprite[] childSprites;

	private bool hasLanded;

	private static readonly int _desaturationProp = Shader.PropertyToID("_Desaturation");

	private void Awake()
	{
		List<tk2dSprite> list = new List<tk2dSprite>();
		tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>(includeInactive: true);
		foreach (tk2dSprite tk2dSprite2 in componentsInChildren)
		{
			if (!(tk2dSprite2 == null) && NonTinter.CanTint(tk2dSprite2.gameObject, NonTinter.TintFlag.CorpseLand))
			{
				list.Add(tk2dSprite2);
			}
		}
		childSprites = list.ToArray();
	}

	public void Landed(bool desaturate)
	{
		if (hasLanded)
		{
			return;
		}
		hasLanded = true;
		int count = childSprites.Length;
		Color[] startColors = new Color[count];
		for (int i = 0; i < count; i++)
		{
			startColors[i] = childSprites[i].color;
		}
		MeshRenderer[] renderers = null;
		MaterialPropertyBlock matPropBlock = null;
		if (desaturate)
		{
			matPropBlock = new MaterialPropertyBlock();
			renderers = new MeshRenderer[count];
			_ = count;
			int num = 0;
			for (int j = 0; j < count; j++)
			{
				renderers[num] = childSprites[j].GetComponent<MeshRenderer>();
			}
		}
		this.StartTimerRoutine(GlobalSettings.Corpse.LandTintWaitTime, GlobalSettings.Corpse.LandTintFadeTime, delegate(float time)
		{
			float value = GlobalSettings.Corpse.LandDesaturationCurve.Evaluate(time);
			time = GlobalSettings.Corpse.LandTintCurve.Evaluate(time);
			for (int k = 0; k < count; k++)
			{
				Color color = startColors[k];
				Color b = color * GlobalSettings.Corpse.LandTint;
				childSprites[k].color = Color.Lerp(color, b, time);
				if (desaturate)
				{
					MeshRenderer obj = renderers[k];
					obj.GetPropertyBlock(matPropBlock);
					matPropBlock.SetFloat(_desaturationProp, value);
					obj.SetPropertyBlock(matPropBlock);
				}
			}
		});
	}
}
