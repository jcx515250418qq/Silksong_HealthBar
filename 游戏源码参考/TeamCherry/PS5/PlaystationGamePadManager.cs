using UnityEngine;

namespace TeamCherry.PS5
{
	public sealed class PlaystationGamePadManager : MonoBehaviour
	{
		private void Start()
		{
			GamePad[] gamePads = GamePad.gamePads;
			for (int i = 0; i < gamePads.Length; i++)
			{
				gamePads[i].InitGamepad();
			}
		}

		private void Update()
		{
			GamePad[] gamePads = GamePad.gamePads;
			for (int i = 0; i < gamePads.Length; i++)
			{
				gamePads[i].Update();
			}
		}
	}
}
