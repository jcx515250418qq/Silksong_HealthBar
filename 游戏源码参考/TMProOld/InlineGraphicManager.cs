using System;
using UnityEngine;

namespace TMProOld
{
	[ExecuteInEditMode]
	public class InlineGraphicManager : MonoBehaviour
	{
		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		[HideInInspector]
		private InlineGraphic m_inlineGraphic;

		[SerializeField]
		[HideInInspector]
		private CanvasRenderer m_inlineGraphicCanvasRenderer;

		private UIVertex[] m_uiVertex;

		private RectTransform m_inlineGraphicRectTransform;

		private TMP_Text m_textComponent;

		private bool m_isInitialized;

		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return m_spriteAsset;
			}
			set
			{
				LoadSpriteAsset(value);
			}
		}

		public InlineGraphic inlineGraphic
		{
			get
			{
				return m_inlineGraphic;
			}
			set
			{
				if (m_inlineGraphic != value)
				{
					m_inlineGraphic = value;
				}
			}
		}

		public CanvasRenderer canvasRenderer => m_inlineGraphicCanvasRenderer;

		public UIVertex[] uiVertex => m_uiVertex;

		private void Awake()
		{
			if (!TMP_Settings.warningsDisabled)
			{
				Debug.LogWarning("InlineGraphicManager component is now Obsolete and has been removed from [" + base.gameObject.name + "] along with its InlineGraphic child.", this);
			}
			if (inlineGraphic.gameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(inlineGraphic.gameObject);
				inlineGraphic = null;
			}
			UnityEngine.Object.DestroyImmediate(this);
		}

		private void OnEnable()
		{
			base.enabled = false;
		}

		private void OnDisable()
		{
		}

		private void OnDestroy()
		{
		}

		private void LoadSpriteAsset(TMP_SpriteAsset spriteAsset)
		{
			if (spriteAsset == null)
			{
				spriteAsset = ((!(TMP_Settings.defaultSpriteAsset != null)) ? (Resources.Load("Sprite Assets/Default Sprite Asset") as TMP_SpriteAsset) : TMP_Settings.defaultSpriteAsset);
			}
			m_spriteAsset = spriteAsset;
			m_inlineGraphic.texture = m_spriteAsset.spriteSheet;
			if (m_textComponent != null && m_isInitialized)
			{
				m_textComponent.havePropertiesChanged = true;
				m_textComponent.SetVerticesDirty();
			}
		}

		public void AddInlineGraphicsChild()
		{
			if (!(m_inlineGraphic != null))
			{
				GameObject gameObject = new GameObject("Inline Graphic");
				m_inlineGraphic = gameObject.AddComponent<InlineGraphic>();
				m_inlineGraphicRectTransform = gameObject.GetComponent<RectTransform>();
				m_inlineGraphicCanvasRenderer = gameObject.GetComponent<CanvasRenderer>();
				m_inlineGraphicRectTransform.SetParent(base.transform, worldPositionStays: false);
				m_inlineGraphicRectTransform.localPosition = Vector3.zero;
				m_inlineGraphicRectTransform.anchoredPosition3D = Vector3.zero;
				m_inlineGraphicRectTransform.sizeDelta = Vector2.zero;
				m_inlineGraphicRectTransform.anchorMin = Vector2.zero;
				m_inlineGraphicRectTransform.anchorMax = Vector2.one;
				m_textComponent = GetComponent<TMP_Text>();
			}
		}

		public void AllocatedVertexBuffers(int size)
		{
			if (m_inlineGraphic == null)
			{
				AddInlineGraphicsChild();
				LoadSpriteAsset(m_spriteAsset);
			}
			if (m_uiVertex == null)
			{
				m_uiVertex = new UIVertex[4];
			}
			int num = size * 4;
			if (num > m_uiVertex.Length)
			{
				m_uiVertex = new UIVertex[Mathf.NextPowerOfTwo(num)];
			}
		}

		public void UpdatePivot(Vector2 pivot)
		{
			if (m_inlineGraphicRectTransform == null)
			{
				m_inlineGraphicRectTransform = m_inlineGraphic.GetComponent<RectTransform>();
			}
			m_inlineGraphicRectTransform.pivot = pivot;
		}

		public void ClearUIVertex()
		{
			if (uiVertex != null && uiVertex.Length != 0)
			{
				Array.Clear(uiVertex, 0, uiVertex.Length);
				m_inlineGraphicCanvasRenderer.Clear();
			}
		}

		public void DrawSprite(UIVertex[] uiVertices, int spriteCount)
		{
			if (m_inlineGraphicCanvasRenderer == null)
			{
				m_inlineGraphicCanvasRenderer = m_inlineGraphic.GetComponent<CanvasRenderer>();
			}
			m_inlineGraphicCanvasRenderer.SetVertices(uiVertices, spriteCount * 4);
			m_inlineGraphic.UpdateMaterial();
		}

		public TMP_Sprite GetSprite(int index)
		{
			if (m_spriteAsset == null)
			{
				Debug.LogWarning("No Sprite Asset is assigned.", this);
				return null;
			}
			if (m_spriteAsset.spriteInfoList == null || index > m_spriteAsset.spriteInfoList.Count - 1)
			{
				Debug.LogWarning("Sprite index exceeds the number of sprites in this Sprite Asset.", this);
				return null;
			}
			return m_spriteAsset.spriteInfoList[index];
		}

		public int GetSpriteIndexByHashCode(int hashCode)
		{
			if (m_spriteAsset == null || m_spriteAsset.spriteInfoList == null)
			{
				Debug.LogWarning("No Sprite Asset is assigned.", this);
				return -1;
			}
			return m_spriteAsset.spriteInfoList.FindIndex((TMP_Sprite item) => item.hashCode == hashCode);
		}

		public int GetSpriteIndexByIndex(int index)
		{
			if (m_spriteAsset == null || m_spriteAsset.spriteInfoList == null)
			{
				Debug.LogWarning("No Sprite Asset is assigned.", this);
				return -1;
			}
			return m_spriteAsset.spriteInfoList.FindIndex((TMP_Sprite item) => item.id == index);
		}

		public void SetUIVertex(UIVertex[] uiVertex)
		{
			m_uiVertex = uiVertex;
		}
	}
}
