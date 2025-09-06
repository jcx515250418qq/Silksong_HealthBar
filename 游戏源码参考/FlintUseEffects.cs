using System;
using UnityEngine;

public class FlintUseEffects : MonoBehaviour
{
	[Serializable]
	private class EffectGroup
	{
		public GameObject Parent;

		public GameObject Pt1;

		public GameObject Pt2;

		public Color EffectTintColor;

		private ParticleSystem[] pt1Particles;

		private ParticleSystem[] pt2Particles;

		private bool hasPt1Started;

		private bool hasPt2Started;

		public void Awake()
		{
			pt1Particles = (Pt1 ? Pt1.GetComponentsInChildren<ParticleSystem>(includeInactive: true) : new ParticleSystem[0]);
			pt2Particles = (Pt2 ? Pt2.GetComponentsInChildren<ParticleSystem>(includeInactive: true) : new ParticleSystem[0]);
		}

		public void StartPt1()
		{
			hasPt1Started = true;
			if ((bool)Pt1)
			{
				Pt1.SetActive(value: true);
			}
			ParticleSystem[] array = pt1Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play(withChildren: true);
			}
		}

		public void StopPt1()
		{
			ParticleSystem[] array = pt1Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}

		public void StartPt2()
		{
			hasPt2Started = true;
			if ((bool)Pt2)
			{
				Pt2.SetActive(value: true);
			}
			ParticleSystem[] array = pt2Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play(withChildren: true);
			}
		}

		public void StopPt2()
		{
			ParticleSystem[] array = pt2Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}

		public void StopAll()
		{
			StopPt1();
			StopPt2();
		}

		public void Reset()
		{
			hasPt1Started = false;
			hasPt2Started = false;
			if ((bool)Parent)
			{
				Parent.SetActive(value: false);
			}
			ParticleSystem[] array = pt1Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			array = pt2Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			if ((bool)Pt1)
			{
				Pt1.SetActive(value: false);
			}
			if ((bool)Pt2)
			{
				Pt2.SetActive(value: false);
			}
		}

		public bool HasEffectsEnded()
		{
			if (!hasPt1Started || !hasPt2Started)
			{
				return false;
			}
			ParticleSystem[] array = pt1Particles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive(withChildren: true))
				{
					return false;
				}
			}
			array = pt2Particles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive(withChildren: true))
				{
					return false;
				}
			}
			return true;
		}
	}

	[SerializeField]
	private GameObject pt1;

	[SerializeField]
	private GameObject pt2;

	[SerializeField]
	private SpriteRenderer[] tintSpriteRenderers;

	[SerializeField]
	private tk2dSprite[] tintTK2DSprites;

	[SerializeField]
	[ArrayForEnum(typeof(NailElements))]
	private EffectGroup[] effectGroups;

	private EffectGroup currentGroup;

	private Transform hero;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref effectGroups, typeof(NailElements));
	}

	private void Awake()
	{
		OnValidate();
	}

	private void OnEnable()
	{
		ResetParts();
		EffectGroup[] array = effectGroups;
		foreach (EffectGroup obj in array)
		{
			obj.Awake();
			obj.Reset();
		}
	}

	private void OnDisable()
	{
		ResetParts();
		if (currentGroup != null)
		{
			currentGroup.Reset();
			currentGroup = null;
		}
	}

	private void Update()
	{
		if (currentGroup != null && currentGroup.HasEffectsEnded())
		{
			base.gameObject.Recycle();
		}
	}

	private void LateUpdate()
	{
		if ((bool)hero)
		{
			base.transform.SetPosition2D(hero.position);
		}
	}

	private void ResetParts()
	{
		if ((bool)pt1)
		{
			pt1.SetActive(value: false);
		}
		if ((bool)pt2)
		{
			pt2.SetActive(value: false);
		}
	}

	public void SetGroup(NailElements element)
	{
		if (!hero)
		{
			hero = HeroController.instance.transform;
		}
		if ((bool)hero)
		{
			base.transform.SetPosition2D(hero.position);
		}
		float x = hero.localScale.x;
		Vector3 localScale = base.transform.localScale;
		localScale.x = Mathf.Abs(localScale.x) * x;
		base.transform.localScale = localScale;
		currentGroup = effectGroups[(int)element];
		currentGroup.Reset();
		if ((bool)currentGroup.Parent)
		{
			currentGroup.Parent.SetActive(value: true);
		}
		SpriteRenderer[] array = tintSpriteRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = currentGroup.EffectTintColor;
		}
		tk2dSprite[] array2 = tintTK2DSprites;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].color = currentGroup.EffectTintColor;
		}
	}

	public void SetPt1()
	{
		if ((bool)pt1)
		{
			pt1.SetActive(value: true);
		}
		if (currentGroup != null)
		{
			currentGroup.StartPt1();
		}
	}

	public void SetPt2()
	{
		if ((bool)pt1)
		{
			pt1.SetActive(value: false);
		}
		if ((bool)pt2)
		{
			pt2.SetActive(value: true);
		}
		if (currentGroup != null)
		{
			currentGroup.StopPt1();
			currentGroup.StartPt2();
		}
		HeroController.instance.AddFrost(-25f);
	}

	public void SetEnd()
	{
		ResetParts();
		if (currentGroup != null)
		{
			currentGroup.StopAll();
		}
	}
}
