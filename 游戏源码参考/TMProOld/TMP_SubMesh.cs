using System;
using UnityEngine;

namespace TMProOld
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class TMP_SubMesh : MonoBehaviour
	{
		[SerializeField]
		private TMP_FontAsset m_fontAsset;

		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		private Material m_material;

		[SerializeField]
		private Material m_sharedMaterial;

		private Material m_fallbackMaterial;

		private Material m_fallbackSourceMaterial;

		[SerializeField]
		private bool m_isDefaultMaterial;

		[SerializeField]
		private float m_padding;

		[SerializeField]
		private Renderer m_renderer;

		[SerializeField]
		private MeshFilter m_meshFilter;

		private Mesh m_mesh;

		[SerializeField]
		private TextMeshPro m_TextComponent;

		[NonSerialized]
		private bool m_isRegisteredForEvents;

		private const HideFlags TARGET_HIDE_FLAGS = HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;

		private TextMeshProClipRect clipRect;

		public TMP_FontAsset fontAsset
		{
			get
			{
				return m_fontAsset;
			}
			set
			{
				m_fontAsset = value;
			}
		}

		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return m_spriteAsset;
			}
			set
			{
				m_spriteAsset = value;
			}
		}

		public Material material
		{
			get
			{
				return GetMaterial(m_sharedMaterial);
			}
			set
			{
				if (m_sharedMaterial.GetInstanceID() != value.GetInstanceID())
				{
					m_sharedMaterial = (m_material = value);
					m_padding = GetPaddingForMaterial();
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public Material sharedMaterial
		{
			get
			{
				return m_sharedMaterial;
			}
			set
			{
				SetSharedMaterial(value);
			}
		}

		public Material fallbackMaterial
		{
			get
			{
				return m_fallbackMaterial;
			}
			set
			{
				if (!(m_fallbackMaterial == value))
				{
					if (m_fallbackMaterial != null && m_fallbackMaterial != value)
					{
						TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
					}
					m_fallbackMaterial = value;
					TMP_MaterialManager.AddFallbackMaterialReference(m_fallbackMaterial);
					SetSharedMaterial(m_fallbackMaterial);
				}
			}
		}

		public Material fallbackSourceMaterial
		{
			get
			{
				return m_fallbackSourceMaterial;
			}
			set
			{
				m_fallbackSourceMaterial = value;
			}
		}

		public bool isDefaultMaterial
		{
			get
			{
				return m_isDefaultMaterial;
			}
			set
			{
				m_isDefaultMaterial = value;
			}
		}

		public float padding
		{
			get
			{
				return m_padding;
			}
			set
			{
				m_padding = value;
			}
		}

		public Renderer renderer
		{
			get
			{
				if (m_renderer == null)
				{
					m_renderer = GetComponent<Renderer>();
				}
				return m_renderer;
			}
		}

		public MeshFilter meshFilter
		{
			get
			{
				if (m_meshFilter == null)
				{
					m_meshFilter = GetComponent<MeshFilter>();
					if (m_meshFilter == null)
					{
						m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
						m_meshFilter.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
					}
				}
				return m_meshFilter;
			}
		}

		public Mesh mesh
		{
			get
			{
				if (m_mesh == null)
				{
					m_mesh = new Mesh();
					m_mesh.name = "TMP_SubMesh_Mesh";
					m_mesh.hideFlags = HideFlags.HideAndDontSave;
					meshFilter.mesh = m_mesh;
				}
				return m_mesh;
			}
			set
			{
				m_mesh = value;
			}
		}

		public TMP_Text textComponent
		{
			get
			{
				if (m_TextComponent == null)
				{
					m_TextComponent = GetComponentInParent<TextMeshPro>();
				}
				return m_TextComponent;
			}
		}

		public static TMP_SubMesh AddSubTextObject(TextMeshPro textComponent, MaterialReference materialReference)
		{
			GameObject obj = new GameObject();
			obj.hideFlags = HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;
			obj.transform.SetParent(textComponent.transform, worldPositionStays: false);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
			obj.layer = textComponent.gameObject.layer;
			TMP_SubMesh tMP_SubMesh = obj.AddComponent<TMP_SubMesh>();
			tMP_SubMesh.m_TextComponent = textComponent;
			tMP_SubMesh.m_fontAsset = materialReference.fontAsset;
			tMP_SubMesh.m_spriteAsset = materialReference.spriteAsset;
			tMP_SubMesh.m_isDefaultMaterial = materialReference.isDefaultMaterial;
			tMP_SubMesh.SetSharedMaterial(materialReference.material);
			tMP_SubMesh.renderer.sortingLayerID = textComponent.renderer.sortingLayerID;
			tMP_SubMesh.renderer.sortingOrder = textComponent.renderer.sortingOrder;
			LinkRendererState linkRendererState = tMP_SubMesh.gameObject.AddComponent<LinkRendererState>();
			linkRendererState.parent = textComponent.renderer;
			linkRendererState.child = tMP_SubMesh.renderer;
			linkRendererState.UpdateLink();
			return tMP_SubMesh;
		}

		private void Awake()
		{
			clipRect = base.gameObject.GetComponentInParent<TextMeshProClipRect>();
			if ((bool)clipRect)
			{
				clipRect.AddSubMesh(this);
			}
		}

		private void OnEnable()
		{
			if (!m_isRegisteredForEvents)
			{
				m_isRegisteredForEvents = true;
			}
			if (base.hideFlags != (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild))
			{
				base.hideFlags = HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;
			}
			meshFilter.sharedMesh = mesh;
			if (m_sharedMaterial != null)
			{
				m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, new Vector4(-10000f, -10000f, 10000f, 10000f));
			}
		}

		private void OnDisable()
		{
			m_meshFilter.sharedMesh = null;
			if (m_fallbackMaterial != null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
				m_fallbackMaterial = null;
			}
		}

		private void OnDestroy()
		{
			if (m_mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(m_mesh);
			}
			if (m_fallbackMaterial != null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(m_fallbackMaterial);
				m_fallbackMaterial = null;
			}
			if (clipRect != null)
			{
				clipRect.RemoveSubMesh(this);
			}
			m_isRegisteredForEvents = false;
			if (m_TextComponent != null)
			{
				m_TextComponent.havePropertiesChanged = true;
				m_TextComponent.SetAllDirty();
			}
		}

		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject, 1f);
		}

		private Material GetMaterial(Material mat)
		{
			if (m_renderer == null)
			{
				m_renderer = GetComponent<Renderer>();
			}
			if (m_material == null || m_material.GetInstanceID() != mat.GetInstanceID())
			{
				m_material = CreateMaterialInstance(mat);
			}
			m_sharedMaterial = m_material;
			m_padding = GetPaddingForMaterial();
			SetVerticesDirty();
			SetMaterialDirty();
			return m_sharedMaterial;
		}

		private Material CreateMaterialInstance(Material source)
		{
			Material obj = new Material(source)
			{
				shaderKeywords = source.shaderKeywords
			};
			obj.name += " (Instance)";
			return obj;
		}

		private Material GetSharedMaterial()
		{
			if (m_renderer == null)
			{
				m_renderer = GetComponent<Renderer>();
			}
			return m_renderer.sharedMaterial;
		}

		private void SetSharedMaterial(Material mat)
		{
			m_sharedMaterial = mat;
			m_padding = GetPaddingForMaterial();
			SetMaterialDirty();
		}

		public float GetPaddingForMaterial()
		{
			return ShaderUtilities.GetPadding(m_sharedMaterial, m_TextComponent.extraPadding, m_TextComponent.isUsingBold);
		}

		public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
		{
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, isExtraPadding, isUsingBold);
		}

		public void SetVerticesDirty()
		{
			if (base.enabled && m_TextComponent != null)
			{
				m_TextComponent.havePropertiesChanged = true;
				m_TextComponent.SetVerticesDirty();
			}
		}

		public void SetMaterialDirty()
		{
			UpdateMaterial();
		}

		protected void UpdateMaterial()
		{
			if (!(renderer == null) && !(m_sharedMaterial == null))
			{
				m_renderer.sharedMaterial = m_sharedMaterial;
				if (m_sharedMaterial.HasProperty(ShaderUtilities.ShaderTag_CullMode) && textComponent.fontSharedMaterial != null)
				{
					float @float = textComponent.fontSharedMaterial.GetFloat(ShaderUtilities.ShaderTag_CullMode);
					m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_CullMode, @float);
				}
			}
		}
	}
}
