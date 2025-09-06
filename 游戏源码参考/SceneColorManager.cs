using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(ColorCorrectionCurves))]
public class SceneColorManager : MonoBehaviour
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

	private List<Keyframe[]> redPairedKeyframes;

	private List<Keyframe[]> greenPairedKeyframes;

	private List<Keyframe[]> bluePairedKeyframes;

	private bool hasCurvesScript;

	private ColorCorrectionCurves curvesScript;

	private const float PAIRING_DISTANCE = 0.01f;

	private const float TANGENT_DISTANCE = 0.0012f;

	private const float UPDATE_RATE = 1f;

	private bool gameplayScene;

	private HeroController hero;

	private GameManager gm;

	private static List<Keyframe> _tempA;

	private static List<Keyframe> _tempB;

	private static List<Keyframe> _finalFramesList;

	private bool changesInEditor = true;

	private float lastFactor;

	private float lastSaturationA;

	private float lastSaturationB;

	private float lastAmbientIntensityA;

	private float lastAmbientIntensityB;

	public bool MarkerActive { get; private set; }

	public bool StartBufferActive { get; private set; }

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

	public void GameInit()
	{
		curvesScript = GetComponent<ColorCorrectionCurves>();
		hasCurvesScript = curvesScript != null;
		_tempA = new List<Keyframe>(128);
		_tempB = new List<Keyframe>(128);
		_finalFramesList = new List<Keyframe>(128);
		gm = GameManager.instance;
		gm.UnloadingLevel += OnLevelUnload;
		UpdateSceneType();
		lastFactor = Factor;
		lastSaturationA = SaturationA;
		lastSaturationB = SaturationB;
		lastAmbientIntensityA = AmbientIntensityA;
		lastAmbientIntensityB = AmbientIntensityB;
		PairCurvesKeyframes();
	}

	public void SceneInit()
	{
		UpdateSceneType();
		if (!gameplayScene)
		{
			Factor = 0f;
			return;
		}
		StartBufferActive = true;
		MarkerActive = true;
		UpdateScript(forceUpdate: true);
		HeroController hc = HeroController.instance;
		HeroController.HeroInPosition temp = null;
		temp = delegate
		{
			FinishBufferPeriod();
			hc.heroInPositionDelayed -= temp;
		};
		hc.heroInPositionDelayed += temp;
	}

	private void Update()
	{
		if ((MarkerActive || StartBufferActive) && (float)Time.frameCount % 1f == 0f)
		{
			UpdateScript();
		}
	}

	private void OnLevelUnload()
	{
		Factor = 0f;
		MarkerActive = false;
	}

	private void OnDisable()
	{
		if (gm != null)
		{
			gm.UnloadingLevel -= OnLevelUnload;
		}
	}

	public IEnumerator ForceRefresh()
	{
		UpdateScript();
		SetFactor(0.0002f);
		yield return new WaitForSeconds(0.1f);
		UpdateScript();
	}

	private void FinishBufferPeriod()
	{
		UpdateScript(forceUpdate: true);
		StartBufferActive = false;
	}

	public void SetMarkerActive(bool active)
	{
		if (!StartBufferActive)
		{
			MarkerActive = active;
		}
	}

	public void UpdateScript(bool forceUpdate = false)
	{
		if (!hasCurvesScript)
		{
			curvesScript = GetComponent<ColorCorrectionCurves>();
			hasCurvesScript = curvesScript != null;
			if (!hasCurvesScript)
			{
				return;
			}
		}
		if (!PairedListsInitiated())
		{
			PairCurvesKeyframes();
		}
		if (changesInEditor)
		{
			PairCurvesKeyframes();
			UpdateScriptParameters();
			curvesScript.UpdateParameters();
			changesInEditor = false;
		}
		else if (forceUpdate)
		{
			PairCurvesKeyframes();
			UpdateScriptParameters();
			curvesScript.UpdateParameters();
		}
		else if (Math.Abs(Factor - lastFactor) > Mathf.Epsilon || Math.Abs(SaturationA - lastSaturationA) > Mathf.Epsilon || Math.Abs(SaturationB - lastSaturationB) > Mathf.Epsilon || Math.Abs(AmbientIntensityA - lastAmbientIntensityA) > Mathf.Epsilon || Math.Abs(AmbientIntensityB - lastAmbientIntensityB) > Mathf.Epsilon)
		{
			UpdateScriptParameters();
			curvesScript.UpdateParameters();
			lastFactor = Factor;
			lastSaturationA = SaturationA;
			lastSaturationB = SaturationB;
			lastAmbientIntensityA = AmbientIntensityA;
			lastAmbientIntensityB = AmbientIntensityB;
		}
	}

	private void EditorHasChanged()
	{
		changesInEditor = true;
		UpdateScript();
	}

	public static List<Keyframe[]> PairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		if (curveA.length == curveB.length)
		{
			return SimplePairKeyframes(curveA, curveB);
		}
		List<Keyframe[]> list = new List<Keyframe[]>();
		_tempA.Clear();
		_tempA.AddRange(curveA.keys);
		_tempB.Clear();
		_tempB.AddRange(curveB.keys);
		int num = 0;
		bool flag = false;
		Keyframe aKeyframe = _tempA[num];
		while (num < _tempA.Count)
		{
			if (flag)
			{
				aKeyframe = _tempA[num];
			}
			int num2 = _tempB.FindIndex((Keyframe bKeyframe) => Mathf.Abs(aKeyframe.time - bKeyframe.time) < 0.01f);
			if (num2 >= 0)
			{
				Keyframe[] item = new Keyframe[2]
				{
					_tempA[num],
					_tempB[num2]
				};
				list.Add(item);
				_tempA.RemoveAt(num);
				_tempB.RemoveAt(num2);
				flag = false;
			}
			else
			{
				num++;
				flag = true;
			}
		}
		foreach (Keyframe item2 in _tempA)
		{
			Keyframe keyframe = CreatePair(item2, curveB);
			list.Add(new Keyframe[2] { item2, keyframe });
		}
		foreach (Keyframe item3 in _tempB)
		{
			Keyframe keyframe2 = CreatePair(item3, curveA);
			list.Add(new Keyframe[2] { keyframe2, item3 });
		}
		return list;
	}

	private static List<Keyframe[]> SimplePairKeyframes(AnimationCurve curveA, AnimationCurve curveB)
	{
		if (curveA.length != curveB.length)
		{
			throw new UnityException("Simple Pair cannot work with curves with different number of Keyframes.");
		}
		List<Keyframe[]> list = new List<Keyframe[]>();
		Keyframe[] keys = curveA.keys;
		Keyframe[] keys2 = curveB.keys;
		for (int i = 0; i < curveA.length; i++)
		{
			list.Add(new Keyframe[2]
			{
				keys[i],
				keys2[i]
			});
		}
		return list;
	}

	private static Keyframe CreatePair(Keyframe kf, AnimationCurve curve)
	{
		Keyframe keyframe = default(Keyframe);
		keyframe.time = kf.time;
		keyframe.value = curve.Evaluate(kf.time);
		Keyframe result = keyframe;
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

	private static AnimationCurve CreateCurveFromKeyframes(IList<Keyframe[]> keyframePairs, float factor)
	{
		_finalFramesList.Clear();
		foreach (Keyframe[] keyframePair in keyframePairs)
		{
			_finalFramesList.Add(AverageKeyframe(keyframePair[0], keyframePair[1], factor));
		}
		return new AnimationCurve(_finalFramesList.ToArray());
	}

	private static Keyframe AverageKeyframe(Keyframe a, Keyframe b, float factor)
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
		redPairedKeyframes = PairKeyframes(RedA, RedB);
		greenPairedKeyframes = PairKeyframes(GreenA, GreenB);
		bluePairedKeyframes = PairKeyframes(BlueA, BlueB);
	}

	private void UpdateScriptParameters()
	{
		if (!hasCurvesScript)
		{
			curvesScript = GetComponent<ColorCorrectionCurves>();
			hasCurvesScript = curvesScript != null;
			if (!hasCurvesScript)
			{
				return;
			}
		}
		Factor = Mathf.Clamp01(Factor);
		SaturationA = Mathf.Clamp(SaturationA, 0f, 5f);
		SaturationB = Mathf.Clamp(SaturationB, 0f, 5f);
		curvesScript.saturation = Mathf.Lerp(SaturationA, SaturationB, Factor);
		curvesScript.redChannel = CreateCurveFromKeyframes(redPairedKeyframes, Factor);
		curvesScript.greenChannel = CreateCurveFromKeyframes(greenPairedKeyframes, Factor);
		curvesScript.blueChannel = CreateCurveFromKeyframes(bluePairedKeyframes, Factor);
		CustomSceneManager.SetLighting(Color.Lerp(AmbientColorA, AmbientColorB, Factor), Mathf.Lerp(AmbientIntensityA, AmbientIntensityB, Factor));
		if (gameplayScene)
		{
			if (hero == null)
			{
				hero = HeroController.instance;
			}
			hero.heroLight.BaseColor = Color.Lerp(HeroLightColorA, HeroLightColorB, Factor);
		}
	}

	private bool PairedListsInitiated()
	{
		if (redPairedKeyframes != null && greenPairedKeyframes != null)
		{
			return bluePairedKeyframes != null;
		}
		return false;
	}

	private void UpdateSceneType()
	{
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		if (gm.IsGameplayScene())
		{
			gameplayScene = true;
			if (hero == null)
			{
				hero = HeroController.instance;
			}
		}
		else
		{
			gameplayScene = false;
		}
	}
}
