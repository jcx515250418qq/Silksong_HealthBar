using UnityEngine;
using UnityEngine.UI;

namespace GUIBlendModes
{
	[AddComponentMenu("UI/Effects/Blend Mode")]
	[RequireComponent(typeof(MaskableGraphic))]
	[ExecuteInEditMode]
	public class UIBlendMode : MonoBehaviour
	{
		[SerializeField]
		private BlendMode editorBlendMode;

		private BlendMode blendMode;

		[SerializeField]
		private bool editorShaderOptimization;

		private bool shaderOptimization;

		private MaskableGraphic source;

		private bool isDisabled;

		public BlendMode BlendMode
		{
			get
			{
				return blendMode;
			}
			set
			{
				SetBlendMode(value, ShaderOptimization);
			}
		}

		public bool ShaderOptimization
		{
			get
			{
				return shaderOptimization;
			}
			set
			{
				SetBlendMode(BlendMode, value);
			}
		}

		private void OnEnable()
		{
			isDisabled = false;
			SetBlendMode(editorBlendMode, editorShaderOptimization);
		}

		private void OnDisable()
		{
			isDisabled = true;
			SetBlendMode(BlendMode.Normal);
		}

		public void SetBlendMode(BlendMode blendMode, bool shaderOptimization = false)
		{
			if (this.blendMode != blendMode || this.shaderOptimization != shaderOptimization)
			{
				if (!source)
				{
					source = GetComponent<MaskableGraphic>();
				}
				source.material = BlendMaterials.GetMaterial(blendMode, source is Text, shaderOptimization);
				this.blendMode = blendMode;
				this.shaderOptimization = shaderOptimization;
				if (!isDisabled)
				{
					editorBlendMode = blendMode;
					editorShaderOptimization = shaderOptimization;
				}
			}
		}

		public void SyncEditor()
		{
			if (Application.isEditor && !isDisabled && (BlendMode != editorBlendMode || ShaderOptimization != editorShaderOptimization))
			{
				SetBlendMode(editorBlendMode, editorShaderOptimization);
			}
		}
	}
}
