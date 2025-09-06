using System.Collections;
using UnityEngine;

public class NodeFlyer : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform nodeParent;

	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private bool spriteFacesLeft = true;

	[Space]
	[SerializeField]
	private float speed;

	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private bool activeOnStart;

	private Transform[] nodes;

	private int currentNodeIndex;

	private Coroutine flyRoutine;

	private void Start()
	{
		nodeParent.SetParent(null, worldPositionStays: true);
		int childCount = nodeParent.childCount;
		nodes = new Transform[childCount];
		for (int i = 0; i < childCount; i++)
		{
			nodes[i] = nodeParent.GetChild(i);
		}
		currentNodeIndex = Mathf.Max(0, nodes.Length - 2);
		if (activeOnStart)
		{
			StartFlying();
		}
	}

	public void StartFlying()
	{
		if (flyRoutine == null)
		{
			flyRoutine = StartCoroutine(Fly());
		}
	}

	public void StopFlying()
	{
		if (flyRoutine != null)
		{
			StopCoroutine(flyRoutine);
			flyRoutine = null;
		}
	}

	private IEnumerator Fly()
	{
		if ((bool)animator)
		{
			animator.Play("Fly");
		}
		while (true)
		{
			Vector2 startPos = base.transform.position;
			Vector2 nodePos = nodes[currentNodeIndex].position;
			float num = Vector2.Distance(startPos, nodePos);
			if (num > 0.1f)
			{
				bool num2 = nodePos.x < startPos.x;
				bool flag = base.transform.localScale.x > 0f;
				if (!spriteFacesLeft)
				{
					flag = !flag;
				}
				if (num2 != flag)
				{
					base.transform.FlipLocalScale(x: true);
					if ((bool)animator)
					{
						animator.PlayFromFrame("TurnToFly", 0);
					}
				}
				float duration = num / speed;
				for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
				{
					float t = curve.Evaluate(elapsed / duration);
					Vector2 vector = Vector2.Lerp(startPos, nodePos, t);
					base.transform.position = vector;
					yield return null;
				}
			}
			currentNodeIndex++;
			if (currentNodeIndex >= nodes.Length)
			{
				currentNodeIndex = 0;
			}
			yield return null;
		}
	}
}
