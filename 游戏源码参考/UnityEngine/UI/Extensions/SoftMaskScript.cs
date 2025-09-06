namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Effects/Extensions/SoftMaskScript")]
	public class SoftMaskScript : MonoBehaviour
	{
		private Material mat;

		private Canvas cachedCanvas;

		private Transform cachedCanvasTransform;

		private readonly Vector3[] m_WorldCorners = new Vector3[4];

		private readonly Vector3[] m_CanvasCorners = new Vector3[4];

		[Tooltip("The area that is to be used as the container.")]
		public RectTransform MaskArea;

		[Tooltip("Texture to be used to do the soft alpha")]
		public Texture AlphaMask;

		[Tooltip("At what point to apply the alpha min range 0-1")]
		[Range(0f, 1f)]
		public float CutOff;

		[Tooltip("Implement a hard blend based on the Cutoff")]
		public bool HardBlend;

		[Tooltip("Flip the masks alpha value")]
		public bool FlipAlphaMask;

		private Vector2 maskOffset = Vector2.zero;

		private Vector2 maskScale = Vector2.one;

		private void Start()
		{
			if (MaskArea == null)
			{
				MaskArea = GetComponent<RectTransform>();
			}
			Text component = GetComponent<Text>();
			if (component != null)
			{
				mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
				component.material = mat;
				cachedCanvas = component.canvas;
				cachedCanvasTransform = cachedCanvas.transform;
				if (base.transform.parent.GetComponent<Mask>() == null)
				{
					base.transform.parent.gameObject.AddComponent<Mask>();
				}
				base.transform.parent.GetComponent<Mask>().enabled = false;
			}
			else
			{
				Graphic component2 = GetComponent<Graphic>();
				if (component2 != null)
				{
					mat = new Material(Shader.Find("UI Extensions/SoftMaskShader"));
					component2.material = mat;
					cachedCanvas = component2.canvas;
					cachedCanvasTransform = cachedCanvas.transform;
				}
			}
		}

		private void Update()
		{
			if (cachedCanvas != null)
			{
				SetMask();
			}
		}

		private void SetMask()
		{
			Rect canvasRect = GetCanvasRect();
			Vector2 size = canvasRect.size;
			maskScale.Set(1f / size.x, 1f / size.y);
			maskOffset = -canvasRect.min;
			maskOffset.Scale(maskScale);
			mat.SetTextureOffset("_AlphaMask", maskOffset);
			mat.SetTextureScale("_AlphaMask", maskScale);
			mat.SetTexture("_AlphaMask", AlphaMask);
			mat.SetFloat("_HardBlend", HardBlend ? 1 : 0);
			mat.SetInt("_FlipAlphaMask", FlipAlphaMask ? 1 : 0);
			mat.SetFloat("_CutOff", CutOff);
		}

		public Rect GetCanvasRect()
		{
			if (cachedCanvas == null)
			{
				return default(Rect);
			}
			MaskArea.GetWorldCorners(m_WorldCorners);
			for (int i = 0; i < 4; i++)
			{
				m_CanvasCorners[i] = cachedCanvasTransform.InverseTransformPoint(m_WorldCorners[i]);
			}
			return new Rect(m_CanvasCorners[0].x, m_CanvasCorners[0].y, m_CanvasCorners[2].x - m_CanvasCorners[0].x, m_CanvasCorners[2].y - m_CanvasCorners[0].y);
		}
	}
}
