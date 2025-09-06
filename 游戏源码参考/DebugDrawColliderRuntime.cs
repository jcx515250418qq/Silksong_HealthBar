using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawColliderRuntime : MonoBehaviour
{
	public enum ColorType
	{
		None = -1,
		Tilemap = 0,
		TerrainCollider = 1,
		Danger = 2,
		Roof = 3,
		Region = 4,
		Enemy = 5,
		Water = 6,
		TransitionPoint = 7,
		SandRegion = 8,
		ShardRegion = 9,
		CameraLock = 10
	}

	[SerializeField]
	private ColorType type;

	private bool isInitialized;

	private Material material;

	private EdgeCollider2D[] edgeCollider2Ds;

	private PolygonCollider2D[] polygonCollider2Ds;

	private BoxCollider2D[] boxCollider2Ds;

	private CircleCollider2D[] circleCollider2Ds;

	private Vector2[] currentBoxColliderPoints;

	private DamageHero damageHero;

	private DamageEnemies damageEnemies;

	private static readonly List<DebugDrawColliderRuntime> _actives = new List<DebugDrawColliderRuntime>();

	private static bool _isShowing;

	public static bool IsShowing
	{
		get
		{
			return _isShowing;
		}
		set
		{
			_isShowing = value;
			if (!_isShowing)
			{
				return;
			}
			foreach (DebugDrawColliderRuntime active in _actives)
			{
				active.Init();
			}
		}
	}

	private void OnEnable()
	{
		_actives.Add(this);
		if (IsShowing)
		{
			Init();
		}
	}

	private void OnDisable()
	{
		_actives.Remove(this);
		DeInit();
	}

	private void Init()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			damageHero = GetComponent<DamageHero>();
			damageEnemies = GetComponent<DamageEnemies>();
			CameraRenderHooks.CameraPostRender += OnPostRenderCamera;
			material = new Material(Shader.Find("Sprites/Default"));
			GetColliders();
		}
	}

	private void GetColliders()
	{
		edgeCollider2Ds = GetComponents<EdgeCollider2D>();
		polygonCollider2Ds = GetComponents<PolygonCollider2D>();
		boxCollider2Ds = GetComponents<BoxCollider2D>();
		circleCollider2Ds = GetComponents<CircleCollider2D>();
	}

	private void DeInit()
	{
		if (isInitialized)
		{
			isInitialized = false;
			CameraRenderHooks.CameraPostRender -= OnPostRenderCamera;
			if (material != null)
			{
				UnityEngine.Object.Destroy(material);
				material = null;
			}
			edgeCollider2Ds = null;
			polygonCollider2Ds = null;
			boxCollider2Ds = null;
			currentBoxColliderPoints = null;
		}
	}

	private void OnPostRenderCamera(CameraRenderHooks.CameraSource source)
	{
		if (source != CameraRenderHooks.CameraSource.MainCamera || !IsShowing || (!damageEnemies && (bool)damageHero && damageHero.damageDealt <= 0))
		{
			return;
		}
		GL.PushMatrix();
		material.SetPass(0);
		EdgeCollider2D[] array = edgeCollider2Ds;
		foreach (EdgeCollider2D edgeCollider2D in array)
		{
			if (edgeCollider2D.enabled)
			{
				DrawLines(edgeCollider2D.offset, edgeCollider2D.points);
			}
		}
		PolygonCollider2D[] array2 = polygonCollider2Ds;
		foreach (PolygonCollider2D polygonCollider2D in array2)
		{
			if (polygonCollider2D.enabled)
			{
				DrawLines(polygonCollider2D.offset, polygonCollider2D.points);
			}
		}
		BoxCollider2D[] array3 = boxCollider2Ds;
		foreach (BoxCollider2D boxCollider2D in array3)
		{
			if (boxCollider2D.enabled)
			{
				Vector2 size = boxCollider2D.size;
				Vector2 offset = boxCollider2D.offset;
				Vector2 vector = size * 0.5f;
				Vector2 vector2 = offset - vector;
				Vector2 vector3 = offset + vector;
				if (currentBoxColliderPoints == null)
				{
					currentBoxColliderPoints = new Vector2[4];
				}
				currentBoxColliderPoints[0] = vector2;
				currentBoxColliderPoints[1] = new Vector2(vector2.x, vector3.y);
				currentBoxColliderPoints[2] = vector3;
				currentBoxColliderPoints[3] = new Vector2(vector3.x, vector2.y);
				DrawLines(Vector2.zero, currentBoxColliderPoints);
			}
		}
		CircleCollider2D[] array4 = circleCollider2Ds;
		foreach (CircleCollider2D circleCollider2D in array4)
		{
			if (circleCollider2D.enabled)
			{
				DrawCircle(circleCollider2D.offset, circleCollider2D.radius);
			}
		}
		GL.PopMatrix();
	}

	private void SetGlColour()
	{
		switch (type)
		{
		case ColorType.Tilemap:
			GL.Color(new Color(0f, 0.44f, 0f));
			break;
		case ColorType.TerrainCollider:
			GL.Color(Color.green);
			break;
		case ColorType.Danger:
			GL.Color(Color.red);
			break;
		case ColorType.Roof:
			GL.Color(new Color(0.8f, 1f, 0f));
			break;
		case ColorType.Region:
			GL.Color(new Color(0.4f, 0.75f, 1f));
			break;
		case ColorType.Enemy:
			GL.Color(new Color(1f, 0.7f, 0f));
			break;
		case ColorType.Water:
			GL.Color(new Color(0.2f, 0.5f, 1f));
			break;
		case ColorType.TransitionPoint:
			GL.Color(Color.magenta);
			break;
		case ColorType.SandRegion:
			GL.Color(new Color(1f, 0.7f, 0.7f));
			break;
		case ColorType.ShardRegion:
			GL.Color(Color.grey);
			break;
		case ColorType.CameraLock:
			GL.Color(new Color(0.16f, 0.17f, 0.28f));
			break;
		default:
			GL.Color(Color.white);
			break;
		}
	}

	private void DrawLines(Vector2 offset, Vector2[] points)
	{
		GL.Begin(1);
		SetGlColour();
		for (int i = 0; i < points.Length; i++)
		{
			GL.Vertex((Vector2)base.transform.TransformPoint(points[i] + offset));
			GL.Vertex((Vector2)((i == 0) ? base.transform.TransformPoint(points[^1] + offset) : base.transform.TransformPoint(points[i - 1] + offset)));
		}
		GL.End();
	}

	private void DrawCircle(Vector2 offset, float radius)
	{
		GL.Begin(1);
		GL.PushMatrix();
		GL.MultMatrix(base.transform.localToWorldMatrix);
		SetGlColour();
		Vector3 lossyScale = base.transform.lossyScale;
		int points = Mathf.RoundToInt(Mathf.Log(radius * Mathf.Max(lossyScale.x, lossyScale.y) * 100f) * 10f);
		for (int j = 0; j < points; j++)
		{
			DrawCircleSection(j);
			if (j == 0)
			{
				DrawCircleSection(points - 1);
			}
			else
			{
				DrawCircleSection(j - 1);
			}
		}
		GL.End();
		GL.PopMatrix();
		void DrawCircleSection(int i)
		{
			float f = (float)i / (float)points * MathF.PI * 2f;
			Vector3 vector = new Vector3(Mathf.Cos(f) * radius, Mathf.Sin(f) * radius, 0f);
			GL.Vertex(offset.ToVector3(0f) + vector);
		}
	}

	public static void AddOrUpdate(GameObject gameObject, ColorType type = ColorType.None, bool forceAdd = false)
	{
		if (!_isShowing && !forceAdd)
		{
			return;
		}
		DebugDrawColliderRuntime component = gameObject.GetComponent<DebugDrawColliderRuntime>();
		if ((bool)component)
		{
			component.GetColliders();
			return;
		}
		if (type == ColorType.None)
		{
			HealthManager component2 = gameObject.GetComponent<HealthManager>();
			DamageHero component3 = gameObject.GetComponent<DamageHero>();
			if ((bool)component2 || (bool)component3)
			{
				type = ColorType.Enemy;
			}
		}
		gameObject.AddComponent<DebugDrawColliderRuntime>().type = type;
	}
}
