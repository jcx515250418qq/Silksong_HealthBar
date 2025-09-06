using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(ColorCorrectionCurves))]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Dynamic Color Correction (Curves, Saturation)")]
public class ColorCurvesManager : MonoBehaviour
{
	public float Factor;

	public float SaturationA = 1f;

	public AnimationCurve RedA = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve GreenA = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve BlueA = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public Color AmbientColorA = Color.white;

	public float AmbientIntensityA = 1f;

	public Color HeroLightColorA = Color.white;

	public float SaturationB = 1f;

	public AnimationCurve RedB = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve GreenB = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public AnimationCurve BlueB = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	public Color AmbientColorB = Color.white;

	public float AmbientIntensityB = 1f;

	public Color HeroLightColorB = Color.white;

	private List<Keyframe[]> RedPairedKeyframes;

	private List<Keyframe[]> GreenPairedKeyframes;

	private List<Keyframe[]> BluePairedKeyframes;

	private List<Keyframe[]> DepthRedPairedKeyframes;

	private List<Keyframe[]> DepthGreenPairedKeyframes;

	private List<Keyframe[]> DepthBluePairedKeyframes;

	private List<Keyframe[]> ZCurvePairedKeyframes;

	private ColorCorrectionCurves CurvesScript;

	private const float PAIRING_DISTANCE = 0.01f;

	private const float TANGENT_DISTANCE = 0.0012f;

	private bool ChangesInEditor = true;

	private float LastFactor;

	private float LastSaturationA;

	private float LastSaturationB;

	public void SetFactor(float factor)
	{
		Factor = factor;
	}

	public void SetSaturationA(float saturationA)
	{
		SaturationA = saturationA;
	}

	public void SetSaturationB(float saturationB)
	{
		SaturationB = saturationB;
	}

	private void Start()
	{
		LastFactor = Factor;
		LastSaturationA = SaturationA;
		LastSaturationB = SaturationB;
		CurvesScript = GetComponent<ColorCorrectionCurves>();
		PairCurvesKeyframes();
	}

	private void Update()
	{
		UpdateScript();
	}

	private void UpdateScript()
	{
		if (!PairedListsInitiated())
		{
			PairCurvesKeyframes();
		}
		if (ChangesInEditor)
		{
			PairCurvesKeyframes();
			UpdateScriptParameters();
			CurvesScript.UpdateParameters();
			ChangesInEditor = false;
		}
		else if (Factor != LastFactor || SaturationA != LastSaturationA || SaturationB != LastSaturationB)
		{
			UpdateScriptParameters();
			CurvesScript.UpdateParameters();
			LastFactor = Factor;
			LastSaturationA = SaturationA;
			LastSaturationB = SaturationB;
		}
	}

	private void EditorHasChanged()
	{
		ChangesInEditor = true;
		UpdateScript();
	}

	public static List<Keyframe[]> PairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		if (curveA.length == curveB.length)
		{
			return SimplePairKeyframes(curveA, curveB);
		}
		List<Keyframe[]> list = new List<Keyframe[]>();
		List<Keyframe> list2 = new List<Keyframe>();
		List<Keyframe> list3 = new List<Keyframe>();
		list2.AddRange(curveA.keys);
		list3.AddRange(curveB.keys);
		int num = 0;
		while (num < list2.Count)
		{
			Keyframe aKeyframe = list2[num];
			int num2 = list3.FindIndex((Keyframe bKeyframe) => Mathf.Abs(aKeyframe.time - bKeyframe.time) < 0.01f);
			if (num2 >= 0)
			{
				Keyframe[] item = new Keyframe[2]
				{
					list2[num],
					list3[num2]
				};
				list.Add(item);
				list2.RemoveAt(num);
				list3.RemoveAt(num2);
			}
			else
			{
				num++;
			}
		}
		foreach (Keyframe item2 in list2)
		{
			Keyframe keyframe = CreatePair(item2, curveB);
			list.Add(new Keyframe[2] { item2, keyframe });
		}
		foreach (Keyframe item3 in list3)
		{
			Keyframe keyframe2 = CreatePair(item3, curveA);
			list.Add(new Keyframe[2] { keyframe2, item3 });
		}
		return list;
	}

	private static List<Keyframe[]> SimplePairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		List<Keyframe[]> list = new List<Keyframe[]>();
		if (curveA.length != curveB.length)
		{
			throw new UnityException("Simple Pair cannot work with curves with different number of Keyframes.");
		}
		for (int i = 0; i < curveA.length; i++)
		{
			list.Add(new Keyframe[2]
			{
				curveA.keys[i],
				curveB.keys[i]
			});
		}
		return list;
	}

	private static Keyframe CreatePair(Keyframe kf, AnimationCurve curve)
	{
		Keyframe result = default(Keyframe);
		result.time = kf.time;
		result.value = curve.Evaluate(kf.time);
		if (kf.time >= 0.0012f)
		{
			float num = kf.time - 0.0012f;
			result.inTangent = (curve.Evaluate(num) - curve.Evaluate(kf.time)) / (num - kf.time);
		}
		if (kf.time + 0.0012f <= 1f)
		{
			float num2 = kf.time + 0.0012f;
			result.outTangent = (curve.Evaluate(num2) - curve.Evaluate(kf.time)) / (num2 - kf.time);
		}
		return result;
	}

	public static AnimationCurve CreateCurveFromKeyframes(IList<Keyframe[]> keyframePairs, float factor)
	{
		Keyframe[] array = new Keyframe[keyframePairs.Count];
		for (int i = 0; i < keyframePairs.Count; i++)
		{
			Keyframe[] array2 = keyframePairs[i];
			array[i] = AverageKeyframe(array2[0], array2[1], factor);
		}
		return new AnimationCurve(array);
	}

	public static Keyframe AverageKeyframe(Keyframe a, Keyframe b, float factor)
	{
		Keyframe result = default(Keyframe);
		result.time = a.time * (1f - factor) + b.time * factor;
		result.value = a.value * (1f - factor) + b.value * factor;
		result.inTangent = a.inTangent * (1f - factor) + b.inTangent * factor;
		result.outTangent = a.outTangent * (1f - factor) + b.outTangent * factor;
		return result;
	}

	private void PairCurvesKeyframes()
	{
		RedPairedKeyframes = PairKeyframes(RedA, RedB);
		GreenPairedKeyframes = PairKeyframes(GreenA, GreenB);
		BluePairedKeyframes = PairKeyframes(BlueA, BlueB);
	}

	private void UpdateScriptParameters()
	{
		Factor = Mathf.Clamp01(Factor);
		SaturationA = Mathf.Clamp(SaturationA, 0f, 5f);
		SaturationB = Mathf.Clamp(SaturationB, 0f, 5f);
		CurvesScript.saturation = Mathf.Lerp(SaturationA, SaturationB, Factor);
		CurvesScript.redChannel = CreateCurveFromKeyframes(RedPairedKeyframes, Factor);
		CurvesScript.greenChannel = CreateCurveFromKeyframes(GreenPairedKeyframes, Factor);
		CurvesScript.blueChannel = CreateCurveFromKeyframes(BluePairedKeyframes, Factor);
	}

	private bool PairedListsInitiated()
	{
		if (RedPairedKeyframes != null && GreenPairedKeyframes != null && BluePairedKeyframes != null && DepthRedPairedKeyframes != null && DepthGreenPairedKeyframes != null)
		{
			return DepthBluePairedKeyframes != null;
		}
		return false;
	}
}
