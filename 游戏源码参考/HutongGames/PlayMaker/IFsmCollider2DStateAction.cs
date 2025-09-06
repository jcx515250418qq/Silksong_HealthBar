using UnityEngine;

namespace HutongGames.PlayMaker
{
	public interface IFsmCollider2DStateAction
	{
		void DoCollisionEnter2D(Collision2D collisionInfo);

		void DoCollisionExit2D(Collision2D collisionInfo);

		void DoCollisionStay2D(Collision2D collisionInfo);
	}
}
