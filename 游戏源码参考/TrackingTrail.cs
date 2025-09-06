using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.Splines;
using UnityEngine;

[ExecuteInEditMode]
public class TrackingTrail : MonoBehaviour
{
	private enum FadeStates
	{
		Off = 0,
		FadeUp = 1,
		On = 2,
		FadeDown = 3,
		Ended = 4
	}

	private class FadeTracker
	{
		public FadeStates State;

		public float TimeLeftInState;
	}

	[Serializable]
	private struct CrossSceneFadeSetup
	{
		public string SceneName;

		public string Id;
	}

	private class CrossSceneFade
	{
		public string SceneName;

		public string Id;

		public int LastIndex;

		public float TimeElapsed;

		public int SceneIndex;
	}

	[SerializeField]
	private HermiteSplineFullPath spline;

	[SerializeField]
	private float fadeUpTravelDelay;

	[SerializeField]
	private float crossSceneAddDelay;

	[SerializeField]
	private float fadeUpDuration;

	[SerializeField]
	private float stayUpTime;

	[SerializeField]
	private float fadeDownDuration;

	[SerializeField]
	private float reFadeDuration;

	[Space]
	[SerializeField]
	private Transform silhouettesParent;

	[Space]
	[SerializeField]
	private CrossSceneFadeSetup continueInScene;

	private Dictionary<int, SpriteRenderer> silhouetteDict;

	private List<FadeTracker> runningFades;

	private bool queuedEnd;

	private bool refading;

	private CrossSceneFade previousCrossSceneFade;

	private CrossSceneFade nextCrossSceneFade;

	private static readonly int _cutoutsCountProp = Shader.PropertyToID("_CutoutsCount");

	private static readonly int _cutoutsProp = Shader.PropertyToID("_Cutouts");

	private static readonly List<CrossSceneFade> _crossSceneFades = new List<CrossSceneFade>();

	private static readonly List<TrackingTrail> _activeTrails = new List<TrackingTrail>();

	private static readonly int TINT_COLOR = Shader.PropertyToID("_TintColor");

	private static readonly int COLOR = Shader.PropertyToID("_Color");

	private static readonly int SECONDARY_COLOR = Shader.PropertyToID("_SecondaryColor");

	private const int SHADER_ARRAY_SIZE = 10;

	private void OnEnable()
	{
		_activeTrails.AddIfNotPresent(this);
	}

	private void Start()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		spline.VertexColor = Color.clear;
		spline.UpdateMesh();
		if ((bool)silhouettesParent)
		{
			int pointCount = spline.GetPointCount();
			Transform transform = spline.transform;
			SpriteRenderer[] componentsInChildren = silhouettesParent.GetComponentsInChildren<SpriteRenderer>();
			silhouetteDict = new Dictionary<int, SpriteRenderer>(componentsInChildren.Length);
			SpriteRenderer[] array = componentsInChildren;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				Vector2 a = spriteRenderer.transform.position;
				float num = float.MaxValue;
				int num2 = -1;
				for (int j = 0; j < pointCount; j++)
				{
					Vector3 vector = transform.TransformPoint(spline.GetPoint(j).Position);
					float num3 = Vector2.Distance(a, vector);
					if (!(num3 > num))
					{
						num = num3;
						num2 = j;
					}
				}
				if (num2 >= 0)
				{
					silhouetteDict[num2] = spriteRenderer;
				}
				spriteRenderer.color = Color.clear;
			}
		}
		UpdateShaderPositions();
		GameObject obj = base.gameObject;
		string text = obj.name;
		string text2 = obj.scene.name;
		foreach (CrossSceneFade crossSceneFade in _crossSceneFades)
		{
			if (!(crossSceneFade.SceneName != text2) && !(crossSceneFade.Id != text))
			{
				previousCrossSceneFade = crossSceneFade;
				break;
			}
		}
		foreach (CrossSceneFade crossSceneFade2 in _crossSceneFades)
		{
			if (!(crossSceneFade2.SceneName != continueInScene.SceneName) && !(crossSceneFade2.Id != continueInScene.Id))
			{
				nextCrossSceneFade = crossSceneFade2;
				break;
			}
		}
		if (previousCrossSceneFade != null || nextCrossSceneFade != null)
		{
			StartTrailInternal(endCrossSceneFade: false);
		}
	}

	private void OnDisable()
	{
		_activeTrails.Remove(this);
	}

	private void UpdateShaderPositions()
	{
		if (!silhouettesParent)
		{
			return;
		}
		Renderer component = spline.GetComponent<Renderer>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		component.GetPropertyBlock(materialPropertyBlock);
		int num = silhouettesParent.childCount;
		if (num > 10)
		{
			num = 10;
		}
		materialPropertyBlock.SetFloat(_cutoutsCountProp, num);
		Vector4[] array = new Vector4[num];
		for (int i = 0; i < num; i++)
		{
			Transform child = silhouettesParent.GetChild(i);
			if (child.gameObject.activeInHierarchy)
			{
				Vector3 position = child.position;
				array[i] = position;
			}
		}
		materialPropertyBlock.SetVectorArray(_cutoutsProp, array);
		component.SetPropertyBlock(materialPropertyBlock);
	}

	public static void ClearStatic()
	{
		_crossSceneFades.Clear();
	}

	public void StartTrail(GameObject starter)
	{
		queuedEnd = false;
		_crossSceneFades.Clear();
		FadeDownAll();
		List<FadeTracker> list = runningFades;
		refading = list != null && list.Count > 0;
		StartTrailInternal(endCrossSceneFade: true);
	}

	public static void FadeDownAll()
	{
		foreach (TrackingTrail activeTrail in _activeTrails)
		{
			activeTrail.refading = false;
			List<FadeTracker> list = activeTrail.runningFades;
			if (list != null && list.Count > 0)
			{
				activeTrail.queuedEnd = true;
			}
		}
	}

	private void StartTrailInternal(bool endCrossSceneFade)
	{
		StartCoroutine(RunFades(endCrossSceneFade));
	}

	private IEnumerator RunFades(bool endCrossSceneFade)
	{
		if (runningFades != null)
		{
			while (runningFades.Count > 0)
			{
				yield return null;
			}
		}
		refading = false;
		if (endCrossSceneFade)
		{
			EndCrossSceneFade();
		}
		int pointCount = spline.GetPointCount();
		if (runningFades == null)
		{
			runningFades = new List<FadeTracker>(pointCount);
		}
		else
		{
			runningFades.Clear();
		}
		int num = previousCrossSceneFade?.LastIndex ?? 0;
		CrossSceneFade crossSceneFade = previousCrossSceneFade;
		int num2 = ((crossSceneFade != null) ? (crossSceneFade.SceneIndex + 1) : 0);
		float num3 = crossSceneAddDelay * (float)num2;
		for (int i = 0; i < pointCount; i++)
		{
			int num4 = i + num;
			runningFades.Add(new FadeTracker
			{
				State = FadeStates.Off,
				TimeLeftInState = fadeUpTravelDelay * (float)num4 + num3
			});
		}
		if (nextCrossSceneFade == null && !string.IsNullOrEmpty(continueInScene.SceneName) && !string.IsNullOrEmpty(continueInScene.SceneName))
		{
			nextCrossSceneFade = new CrossSceneFade
			{
				SceneName = continueInScene.SceneName,
				Id = continueInScene.Id,
				LastIndex = pointCount - 1,
				SceneIndex = num2
			};
			_crossSceneFades.Add(nextCrossSceneFade);
		}
		float num5 = ((previousCrossSceneFade != null) ? previousCrossSceneFade.TimeElapsed : ((nextCrossSceneFade == null) ? 0f : nextCrossSceneFade.TimeElapsed));
		if (num5 > Mathf.Epsilon)
		{
			for (float num6 = 0f; num6 <= num5; num6 += 1f / 60f)
			{
				for (int j = 0; j < pointCount; j++)
				{
					TickTracker(1f / 60f, j, out var _);
				}
			}
			if (nextCrossSceneFade != null)
			{
				nextCrossSceneFade.TimeElapsed = num5;
			}
			if (previousCrossSceneFade != null)
			{
				previousCrossSceneFade.TimeElapsed = num5;
			}
		}
		spline.VertexColor = Color.white;
		while (true)
		{
			if (queuedEnd)
			{
				queuedEnd = false;
				foreach (FadeTracker runningFade in runningFades)
				{
					if (runningFade.State < FadeStates.FadeDown)
					{
						runningFade.State = FadeStates.On;
						runningFade.TimeLeftInState = 0f;
					}
				}
			}
			bool flag = false;
			for (int k = 0; k < pointCount; k++)
			{
				if (TickTracker(Time.deltaTime, k, out var pointColor2))
				{
					flag = true;
					spline.SetPointColor(k, pointColor2);
				}
			}
			if (!flag)
			{
				break;
			}
			spline.UpdateMesh();
			foreach (CrossSceneFade crossSceneFade2 in _crossSceneFades)
			{
				crossSceneFade2.TimeElapsed += Time.deltaTime;
			}
			yield return null;
		}
		runningFades.Clear();
		EndCrossSceneFade();
	}

	private void EndCrossSceneFade()
	{
		if (nextCrossSceneFade != null)
		{
			_crossSceneFades.Remove(nextCrossSceneFade);
			nextCrossSceneFade = null;
		}
		if (previousCrossSceneFade != null)
		{
			_crossSceneFades.Remove(previousCrossSceneFade);
			previousCrossSceneFade = null;
		}
	}

	private bool TickTracker(float deltaTime, int trackerIndex, out Color pointColor)
	{
		FadeTracker fadeTracker = runningFades[trackerIndex];
		silhouetteDict.TryGetValue(trackerIndex, out var value);
		Color color = new Color(1f, 1f, 1f, 0f);
		Color color2 = new Color(1f, 1f, 1f, 1f);
		switch (fadeTracker.State)
		{
		case FadeStates.Off:
			pointColor = color;
			fadeTracker.TimeLeftInState -= deltaTime;
			if (fadeTracker.TimeLeftInState > 0f)
			{
				return true;
			}
			fadeTracker.State = FadeStates.FadeUp;
			fadeTracker.TimeLeftInState = fadeUpDuration;
			return true;
		case FadeStates.FadeUp:
		{
			float num2 = (refading ? reFadeDuration : fadeUpDuration);
			fadeTracker.TimeLeftInState -= deltaTime;
			float t2 = fadeTracker.TimeLeftInState / num2;
			pointColor = Color.Lerp(color2, color, t2);
			if (value != null)
			{
				value.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), t2);
			}
			if (fadeTracker.TimeLeftInState > 0f)
			{
				return true;
			}
			fadeTracker.State = FadeStates.On;
			fadeTracker.TimeLeftInState = stayUpTime;
			return true;
		}
		case FadeStates.On:
			fadeTracker.TimeLeftInState -= deltaTime;
			pointColor = color2;
			if (value != null)
			{
				value.color = Color.white;
			}
			if (!refading && fadeTracker.TimeLeftInState > 0f)
			{
				return true;
			}
			fadeTracker.State = FadeStates.FadeDown;
			fadeTracker.TimeLeftInState = (refading ? reFadeDuration : fadeDownDuration);
			return true;
		case FadeStates.FadeDown:
		{
			float num = (refading ? reFadeDuration : fadeDownDuration);
			fadeTracker.TimeLeftInState -= deltaTime;
			float t = fadeTracker.TimeLeftInState / num;
			pointColor = Color.Lerp(color, color2, t);
			if (value != null)
			{
				value.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, t);
			}
			if (fadeTracker.TimeLeftInState > 0f)
			{
				return true;
			}
			fadeTracker.State = FadeStates.Ended;
			return true;
		}
		case FadeStates.Ended:
			pointColor = color;
			refading = false;
			return false;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
