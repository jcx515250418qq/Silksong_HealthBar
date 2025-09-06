using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	public class InlineGraphic : MaskableGraphic
	{
		public Texture texture;

		private InlineGraphicManager m_manager;

		private RectTransform m_RectTransform;

		private RectTransform m_ParentRectTransform;

		public override Texture mainTexture
		{
			get
			{
				if (texture == null)
				{
					return Graphic.s_WhiteTexture;
				}
				return texture;
			}
		}

		protected override void Awake()
		{
			m_manager = GetComponentInParent<InlineGraphicManager>();
		}

		protected override void OnEnable()
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			if (m_manager != null && m_manager.spriteAsset != null)
			{
				texture = m_manager.spriteAsset.spriteSheet;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnTransformParentChanged()
		{
		}

		protected override void OnRectTransformDimensionsChange()
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = base.gameObject.GetComponent<RectTransform>();
			}
			if (m_ParentRectTransform == null)
			{
				m_ParentRectTransform = m_RectTransform.parent.GetComponent<RectTransform>();
			}
			if (m_RectTransform.pivot != m_ParentRectTransform.pivot)
			{
				m_RectTransform.pivot = m_ParentRectTransform.pivot;
			}
		}

		public new void UpdateMaterial()
		{
			base.UpdateMaterial();
		}

		protected override void UpdateGeometry()
		{
		}
	}
}
