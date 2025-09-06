using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class GameMapScene : MonoBehaviour, IInitialisable
{
	public enum States
	{
		Hidden = 0,
		Rough = 1,
		Full = 2
	}

	[Serializable]
	private struct SpriteCondition
	{
		public Sprite Sprite;

		public PlayerDataTest Condition;
	}

	[Serializable]
	private struct ColorCondition
	{
		public Color Color;

		public PlayerDataTest Condition;
	}

	[SerializeField]
	private GameMapScene mappedParent;

	[Space]
	[SerializeField]
	private States initialState;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsInitialStateRough", true, true, false)]
	private Sprite fullSprite;

	[SerializeField]
	private SpriteCondition[] altFullSprites;

	[SerializeField]
	private ColorCondition[] altColors = new ColorCondition[0];

	[SerializeField]
	private bool unmappedNoBounds;

	[Space]
	[SerializeField]
	private GameMapScene[] mappedIfAllMapped;

	[Space]
	[Tooltip("Hides map while retaining mapped status")]
	[SerializeField]
	private PlayerDataTest hideCondition;

	[SerializeField]
	private bool excludeBounds;

	private bool isNameCached;

	private string cachedName;

	private bool checkedSprite;

	private bool hasSpriteRenderer;

	private bool hasBeenSet;

	private SpriteRenderer spriteRenderer;

	private Sprite initialSprite;

	private Color initialColor;

	private bool purgedNulls;

	private bool isMapped;

	private bool isVisited;

	private bool hasAwaken;

	private bool hasStarted;

	private bool cachedSpriteBounds;

	private bool hasValidBounds;

	private Bounds localSpriteBounds;

	private Sprite currentSprite;

	public string Name
	{
		get
		{
			if (isNameCached)
			{
				return cachedName;
			}
			isNameCached = true;
			cachedName = base.name;
			return cachedName;
		}
	}

	public States InitialState => initialState;

	public Sprite BoundsSprite
	{
		get
		{
			if (unmappedNoBounds && !IsMapped)
			{
				return null;
			}
			if (IsInitialStateRough() && (bool)fullSprite)
			{
				return fullSprite;
			}
			if (!hasSpriteRenderer)
			{
				return null;
			}
			return initialSprite;
		}
	}

	public bool IsMapped
	{
		get
		{
			if (!isMapped)
			{
				if ((bool)mappedParent)
				{
					return mappedParent.IsMapped;
				}
				return false;
			}
			return true;
		}
		private set
		{
			isMapped = value;
		}
	}

	public bool IsVisited
	{
		get
		{
			if (!isVisited)
			{
				if ((bool)mappedParent)
				{
					return mappedParent.IsVisited;
				}
				return false;
			}
			return true;
		}
		private set
		{
			isVisited = value;
		}
	}

	GameObject IInitialisable.gameObject => base.gameObject;

	[UsedImplicitly]
	private bool IsInitialStateRough()
	{
		return initialState == States.Rough;
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		GetMissingComponents();
		PurgeNull();
		if (!isNameCached)
		{
			isNameCached = true;
			cachedName = base.name;
		}
		if (!IsMapped)
		{
			SetNotMapped();
		}
		if (!hasSpriteRenderer)
		{
			return true;
		}
		SpriteRenderer obj = spriteRenderer;
		Color color = spriteRenderer.color;
		float? a = 1f;
		obj.color = color.Where(null, null, null, a);
		spriteRenderer.sortingOrder = 11;
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	private void OnEnable()
	{
		if (hideCondition.TestGroups != null && hideCondition.TestGroups.Length != 0 && hideCondition.IsFulfilled && hasSpriteRenderer)
		{
			spriteRenderer.enabled = false;
		}
	}

	public void ResetMapped()
	{
		IsMapped = false;
		IsVisited = false;
	}

	private void PurgeNull()
	{
		if (purgedNulls)
		{
			return;
		}
		purgedNulls = true;
		if (mappedIfAllMapped != null && mappedIfAllMapped.Length != 0)
		{
			List<GameMapScene> list = mappedIfAllMapped.ToList();
			list.RemoveAll((GameMapScene o) => o == null);
			mappedIfAllMapped = list.ToArray();
		}
	}

	public void SetVisited()
	{
		IsVisited = true;
	}

	public void SetMapped()
	{
		GameObject gameObject = base.gameObject;
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(value: true);
		}
		if (IsMapped && hasBeenSet && altFullSprites.Length == 0 && altColors.Length == 0)
		{
			return;
		}
		IsMapped = true;
		hasBeenSet = true;
		GetMissingComponents();
		if (!hasSpriteRenderer)
		{
			return;
		}
		cachedSpriteBounds = false;
		spriteRenderer.color = GetColor();
		Sprite sprite = ((initialState != States.Rough) ? initialSprite : (fullSprite ? fullSprite : initialSprite));
		Sprite sprite2 = sprite;
		SpriteCondition[] array = altFullSprites;
		for (int i = 0; i < array.Length; i++)
		{
			SpriteCondition spriteCondition = array[i];
			if (spriteCondition.Condition.IsFulfilled)
			{
				sprite2 = spriteCondition.Sprite;
				break;
			}
		}
		spriteRenderer.sprite = sprite2;
	}

	public void SetNotMapped()
	{
		if (!IsMapped && hasBeenSet)
		{
			return;
		}
		IsMapped = false;
		hasBeenSet = true;
		GetMissingComponents();
		if (!hasSpriteRenderer)
		{
			return;
		}
		cachedSpriteBounds = false;
		switch (initialState)
		{
		case States.Hidden:
			spriteRenderer.sprite = null;
			break;
		case States.Rough:
			if ((bool)fullSprite)
			{
				spriteRenderer.sprite = initialSprite;
				break;
			}
			spriteRenderer.sprite = initialSprite;
			spriteRenderer.color = Color.grey;
			break;
		}
	}

	private void GetMissingComponents()
	{
		if (!hasSpriteRenderer && !checkedSprite)
		{
			checkedSprite = true;
			spriteRenderer = GetComponent<SpriteRenderer>();
			hasSpriteRenderer = spriteRenderer;
			if (hasSpriteRenderer)
			{
				initialSprite = spriteRenderer.sprite;
				Color color = spriteRenderer.color;
				float? a = 1f;
				initialColor = color.Where(null, null, null, a);
			}
		}
	}

	private Color GetColor()
	{
		ColorCondition[] array = altColors;
		for (int i = 0; i < array.Length; i++)
		{
			ColorCondition colorCondition = array[i];
			if (colorCondition.Condition.IsFulfilled)
			{
				return colorCondition.Color;
			}
		}
		return initialColor;
	}

	public bool IsOtherMapped(HashSet<string> scenesMapped)
	{
		PurgeNull();
		if (mappedIfAllMapped == null || mappedIfAllMapped.Length == 0)
		{
			return false;
		}
		GameMapScene[] array = mappedIfAllMapped;
		foreach (GameMapScene gameMapScene in array)
		{
			if (!scenesMapped.Contains(gameMapScene.name))
			{
				return false;
			}
		}
		return true;
	}

	private bool TryGetSpriteBounds(out Bounds bounds)
	{
		Transform transform = base.transform;
		if (hasValidBounds && currentSprite == spriteRenderer.sprite)
		{
			bounds = new Bounds(transform.TransformPoint(localSpriteBounds.center), transform.TransformVector(localSpriteBounds.size));
			return true;
		}
		CacheSpriteBounds();
		if (hasValidBounds && currentSprite == spriteRenderer.sprite)
		{
			bounds = new Bounds(transform.TransformPoint(localSpriteBounds.center), transform.TransformVector(localSpriteBounds.size));
			return true;
		}
		bounds = default(Bounds);
		return false;
	}

	public bool TryGetSpriteBounds(Transform targetSpace, out Bounds bounds)
	{
		if (excludeBounds)
		{
			bounds = default(Bounds);
			return false;
		}
		Transform transform = base.transform;
		if (transform.IsChildOf(targetSpace) || transform == targetSpace)
		{
			if (hasValidBounds && currentSprite == spriteRenderer.sprite)
			{
				Vector3 vector = transform.localPosition;
				Vector3 vector2 = transform.localScale;
				Transform parent = transform.parent;
				while (parent != null && parent != targetSpace)
				{
					vector = Vector3.Scale(vector, parent.localScale) + parent.localPosition;
					vector2 = Vector3.Scale(vector2, parent.localScale);
					parent = parent.parent;
				}
				Vector3 center = vector + Vector3.Scale(localSpriteBounds.center, vector2);
				Vector3 size = Vector3.Scale(localSpriteBounds.size, vector2);
				bounds = new Bounds(center, size);
				return true;
			}
			CacheSpriteBounds();
			if (hasValidBounds && currentSprite == spriteRenderer.sprite)
			{
				Vector3 vector3 = transform.localPosition;
				Vector3 vector4 = transform.localScale;
				Transform parent2 = transform.parent;
				while (parent2 != null && parent2 != targetSpace)
				{
					vector3 = Vector3.Scale(vector3, parent2.localScale) + parent2.localPosition;
					vector4 = Vector3.Scale(vector4, parent2.localScale);
					parent2 = parent2.parent;
				}
				Vector3 center2 = vector3 + Vector3.Scale(localSpriteBounds.center, vector4);
				Vector3 size2 = Vector3.Scale(localSpriteBounds.size, vector4);
				bounds = new Bounds(center2, size2);
				return true;
			}
		}
		else
		{
			if (hasValidBounds && currentSprite == spriteRenderer.sprite)
			{
				bounds = new Bounds(targetSpace.InverseTransformPoint(transform.TransformPoint(localSpriteBounds.center)), targetSpace.TransformVector(transform.TransformVector(localSpriteBounds.size)));
				return true;
			}
			CacheSpriteBounds();
			if (hasValidBounds && currentSprite == spriteRenderer.sprite)
			{
				bounds = new Bounds(targetSpace.InverseTransformPoint(transform.TransformPoint(localSpriteBounds.center)), targetSpace.TransformVector(transform.TransformVector(localSpriteBounds.size)));
				return true;
			}
		}
		bounds = default(Bounds);
		return false;
	}

	private void CacheSpriteBounds()
	{
		if (!cachedSpriteBounds)
		{
			UpdateSpriteBounds();
		}
	}

	private void UpdateSpriteBounds()
	{
		GetMissingComponents();
		if (spriteRenderer != null)
		{
			UpdateSpriteBounds(spriteRenderer.sprite);
		}
		else
		{
			UpdateSpriteBounds(null);
		}
	}

	private void UpdateSpriteBounds(Sprite sprite)
	{
		cachedSpriteBounds = true;
		hasValidBounds = sprite != null;
		currentSprite = sprite;
		if (hasValidBounds)
		{
			localSpriteBounds = GetCroppedBounds(sprite);
		}
	}

	private static Bounds GetCroppedBounds(Sprite sprite)
	{
		Vector2[] vertices = sprite.vertices;
		Vector2 vector = vertices[0];
		Vector2 vector2 = vertices[0];
		for (int i = 1; i < vertices.Length; i++)
		{
			Vector2 vector3 = vertices[i];
			if (vector3.x < vector.x)
			{
				vector.x = vector3.x;
			}
			if (vector3.y < vector.y)
			{
				vector.y = vector3.y;
			}
			if (vector3.x > vector2.x)
			{
				vector2.x = vector3.x;
			}
			if (vector3.y > vector2.y)
			{
				vector2.y = vector3.y;
			}
		}
		Vector2 vector4 = vector2 - vector;
		Vector2 vector5 = (vector + vector2) / 2f;
		return new Bounds(vector5, vector4);
	}

	private void OnDrawGizmosSelected()
	{
		if (GizmoUtility.IsSelfOrChildSelected(base.transform))
		{
			DrawGizmos();
		}
	}

	private void DrawGizmos()
	{
		CacheSpriteBounds();
		DrawBounds(localSpriteBounds, Color.yellow.SetAlpha(0.5f));
		if (TryGetSpriteBounds(out var bounds))
		{
			DrawBounds(bounds, Color.magenta.SetAlpha(0.5f), useLocalMatrix: false);
		}
	}

	private void DrawBounds(Bounds bounds, Color color, bool useLocalMatrix = true)
	{
		Gizmos.color = color;
		if (useLocalMatrix)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
		}
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		Gizmos.matrix = Matrix4x4.identity;
	}
}
