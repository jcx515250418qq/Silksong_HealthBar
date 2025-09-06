using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	public interface ITextElement
	{
		Material sharedMaterial { get; }

		void Rebuild(CanvasUpdate update);

		int GetInstanceID();
	}
}
