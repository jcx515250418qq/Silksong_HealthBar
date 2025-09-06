using HutongGames.PlayMaker;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyUpdraftRegion : MonoBehaviour
{
	private BoxCollider2D collider;

	private void Awake()
	{
		collider = GetComponent<BoxCollider2D>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		_ = collision.transform;
		PlayMakerFSM component = collision.GetComponent<PlayMakerFSM>();
		if ((bool)component)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			FsmGameObject fsmGameObject = component.FsmVariables.FindFsmGameObject("Top Point");
			FsmGameObject fsmGameObject2 = component.FsmVariables.FindFsmGameObject("Start Point");
			if (fsmGameObject != null)
			{
				gameObject = fsmGameObject.Value;
			}
			if (fsmGameObject2 != null)
			{
				gameObject2 = fsmGameObject2.Value;
			}
			Vector2 vector = collider.size / 2f;
			Vector2 position = base.transform.TransformPoint(new Vector2(collider.offset.x, collider.offset.y + vector.y));
			Vector2 position2 = base.transform.TransformPoint(new Vector2(collider.offset.x, collider.offset.y - vector.y));
			if ((bool)gameObject)
			{
				gameObject.transform.SetPosition2D(position);
			}
			if ((bool)gameObject2)
			{
				gameObject2.transform.SetPosition2D(position2);
			}
			FsmBool fsmBool = component.FsmVariables.FindFsmBool("Is In Updraft");
			if (fsmBool != null)
			{
				fsmBool.Value = true;
			}
			FsmGameObject fsmGameObject3 = component.FsmVariables.FindFsmGameObject("Updraft Obj");
			if (fsmGameObject3 != null)
			{
				fsmGameObject3.Value = base.gameObject;
			}
			component.SendEvent("ENTERED UPDRAFT");
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		PlayMakerFSM component = collision.GetComponent<PlayMakerFSM>();
		if ((bool)component)
		{
			FsmBool fsmBool = component.FsmVariables.FindFsmBool("Is In Updraft");
			FsmGameObject fsmGameObject = component.FsmVariables.FindFsmGameObject("Updraft Obj");
			if (fsmBool != null && fsmGameObject != null && fsmGameObject.Value == base.gameObject)
			{
				fsmBool.Value = false;
				fsmGameObject.Value = null;
				component.SendEvent("EXITED UPDRAFT");
			}
		}
	}
}
