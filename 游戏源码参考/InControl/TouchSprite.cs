using System;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public class TouchSprite
	{
		[SerializeField]
		private Sprite idleSprite;

		[SerializeField]
		private Sprite busySprite;

		[SerializeField]
		private Color idleColor = new Color(1f, 1f, 1f, 0.5f);

		[SerializeField]
		private Color busyColor = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		private TouchSpriteShape shape;

		[SerializeField]
		private TouchUnitType sizeUnitType;

		[SerializeField]
		private Vector2 size = new Vector2(10f, 10f);

		[SerializeField]
		private bool lockAspectRatio = true;

		[SerializeField]
		[HideInInspector]
		private Vector2 worldSize;

		private GameObject spriteGameObject;

		private SpriteRenderer spriteRenderer;

		private bool state;

		private static Shader spriteRendererShader;

		private static Material spriteRendererMaterial;

		private static int spriteRendererPixelSnapId;

		public bool Dirty { get; set; }

		public bool Ready { get; set; }

		public bool State
		{
			get
			{
				return state;
			}
			set
			{
				if (state != value)
				{
					state = value;
					Dirty = true;
				}
			}
		}

		public Sprite BusySprite
		{
			get
			{
				return busySprite;
			}
			set
			{
				if (busySprite != value)
				{
					busySprite = value;
					Dirty = true;
				}
			}
		}

		public Sprite IdleSprite
		{
			get
			{
				return idleSprite;
			}
			set
			{
				if (idleSprite != value)
				{
					idleSprite = value;
					Dirty = true;
				}
			}
		}

		public Sprite Sprite
		{
			set
			{
				if (idleSprite != value)
				{
					idleSprite = value;
					Dirty = true;
				}
				if (busySprite != value)
				{
					busySprite = value;
					Dirty = true;
				}
			}
		}

		public Color BusyColor
		{
			get
			{
				return busyColor;
			}
			set
			{
				if (busyColor != value)
				{
					busyColor = value;
					Dirty = true;
				}
			}
		}

		public Color IdleColor
		{
			get
			{
				return idleColor;
			}
			set
			{
				if (idleColor != value)
				{
					idleColor = value;
					Dirty = true;
				}
			}
		}

		public TouchSpriteShape Shape
		{
			get
			{
				return shape;
			}
			set
			{
				if (shape != value)
				{
					shape = value;
					Dirty = true;
				}
			}
		}

		public TouchUnitType SizeUnitType
		{
			get
			{
				return sizeUnitType;
			}
			set
			{
				if (sizeUnitType != value)
				{
					sizeUnitType = value;
					Dirty = true;
				}
			}
		}

		public Vector2 Size
		{
			get
			{
				return size;
			}
			set
			{
				if (size != value)
				{
					size = value;
					Dirty = true;
				}
			}
		}

		public Vector2 WorldSize => worldSize;

		public Vector3 Position
		{
			get
			{
				if (!spriteGameObject)
				{
					return Vector3.zero;
				}
				return spriteGameObject.transform.position;
			}
			set
			{
				if ((bool)spriteGameObject)
				{
					spriteGameObject.transform.position = value;
				}
			}
		}

		public TouchSprite()
		{
		}

		public TouchSprite(float size)
		{
			this.size = Vector2.one * size;
		}

		public void Create(string gameObjectName, Transform parentTransform, int sortingOrder)
		{
			spriteGameObject = CreateSpriteGameObject(gameObjectName, parentTransform);
			spriteRenderer = CreateSpriteRenderer(spriteGameObject, idleSprite, sortingOrder);
			spriteRenderer.color = idleColor;
			Ready = true;
		}

		public void Delete()
		{
			Ready = false;
			UnityEngine.Object.Destroy(spriteGameObject);
		}

		public void Update()
		{
			Update(forceUpdate: false);
		}

		public void Update(bool forceUpdate)
		{
			if (Dirty || forceUpdate)
			{
				if (spriteRenderer != null)
				{
					spriteRenderer.sprite = (State ? busySprite : idleSprite);
				}
				if (sizeUnitType == TouchUnitType.Pixels)
				{
					Vector2 vector = TouchUtility.RoundVector(size);
					ScaleSpriteInPixels(spriteGameObject, spriteRenderer, vector);
					worldSize = vector * TouchManager.PixelToWorld;
				}
				else
				{
					ScaleSpriteInPercent(spriteGameObject, spriteRenderer, size);
					if (lockAspectRatio)
					{
						worldSize = size * TouchManager.PercentToWorld;
					}
					else
					{
						worldSize = Vector2.Scale(size, TouchManager.ViewSize);
					}
				}
				Dirty = false;
			}
			if (spriteRenderer != null)
			{
				Color color = (State ? busyColor : idleColor);
				if (spriteRenderer.color != color)
				{
					spriteRenderer.color = Utility.MoveColorTowards(spriteRenderer.color, color, 5f * Time.unscaledDeltaTime);
				}
			}
		}

		private GameObject CreateSpriteGameObject(string name, Transform parentTransform)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = parentTransform.gameObject.layer;
			return gameObject;
		}

		private SpriteRenderer CreateSpriteRenderer(GameObject spriteGameObject, Sprite sprite, int sortingOrder)
		{
			if (!spriteRendererMaterial)
			{
				spriteRendererShader = Shader.Find("Sprites/Default");
				spriteRendererMaterial = new Material(spriteRendererShader);
				spriteRendererPixelSnapId = Shader.PropertyToID("PixelSnap");
			}
			SpriteRenderer obj = spriteGameObject.AddComponent<SpriteRenderer>();
			obj.sprite = sprite;
			obj.sortingOrder = sortingOrder;
			obj.sharedMaterial = spriteRendererMaterial;
			obj.sharedMaterial.SetFloat(spriteRendererPixelSnapId, 1f);
			return obj;
		}

		private void ScaleSpriteInPixels(GameObject spriteGameObject, SpriteRenderer spriteRenderer, Vector2 size)
		{
			if (!(spriteGameObject == null) && !(spriteRenderer == null) && !(spriteRenderer.sprite == null))
			{
				float num = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.bounds.size.x;
				float num2 = TouchManager.PixelToWorld * num;
				float x = num2 * size.x / spriteRenderer.sprite.rect.width;
				float y = num2 * size.y / spriteRenderer.sprite.rect.height;
				spriteGameObject.transform.localScale = new Vector3(x, y);
			}
		}

		private void ScaleSpriteInPercent(GameObject spriteGameObject, SpriteRenderer spriteRenderer, Vector2 size)
		{
			if (!(spriteGameObject == null) && !(spriteRenderer == null) && !(spriteRenderer.sprite == null))
			{
				if (lockAspectRatio)
				{
					float num = Mathf.Min(TouchManager.ViewSize.x, TouchManager.ViewSize.y);
					float x = num * size.x / spriteRenderer.sprite.bounds.size.x;
					float y = num * size.y / spriteRenderer.sprite.bounds.size.y;
					spriteGameObject.transform.localScale = new Vector3(x, y);
				}
				else
				{
					float x2 = TouchManager.ViewSize.x * size.x / spriteRenderer.sprite.bounds.size.x;
					float y2 = TouchManager.ViewSize.y * size.y / spriteRenderer.sprite.bounds.size.y;
					spriteGameObject.transform.localScale = new Vector3(x2, y2);
				}
			}
		}

		public bool Contains(Vector2 testWorldPoint)
		{
			if (shape == TouchSpriteShape.Oval)
			{
				float num = (testWorldPoint.x - Position.x) / worldSize.x;
				float num2 = (testWorldPoint.y - Position.y) / worldSize.y;
				return num * num + num2 * num2 < 0.25f;
			}
			float num3 = Utility.Abs(testWorldPoint.x - Position.x) * 2f;
			float num4 = Utility.Abs(testWorldPoint.y - Position.y) * 2f;
			if (num3 <= worldSize.x)
			{
				return num4 <= worldSize.y;
			}
			return false;
		}

		public bool Contains(Touch touch)
		{
			return Contains(TouchManager.ScreenToWorldPoint(touch.position));
		}

		public void DrawGizmos(Vector3 position, Color color)
		{
			if (shape == TouchSpriteShape.Oval)
			{
				Utility.DrawOvalGizmo(position, WorldSize, color);
			}
			else
			{
				Utility.DrawRectGizmo(position, WorldSize, color);
			}
		}
	}
}
