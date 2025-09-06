using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestTargetCounter : SavedItem, ICollectableUIMsgItem, IUIMsgPopupItem
{
	protected virtual bool ShowCounterOnConsume => false;

	public virtual bool CanConsume => false;

	public static event Action<QuestTargetCounter> OnIncrement;

	public void Increment()
	{
		QuestTargetCounter.OnIncrement?.Invoke(this);
	}

	public static void ClearStatic()
	{
		QuestTargetCounter.OnIncrement = null;
	}

	public virtual bool ShouldIncrementQuestCounter(QuestTargetCounter eventSender)
	{
		return eventSender == this;
	}

	public virtual int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return sourceCompletion.CompletedCount;
	}

	public virtual void Consume(int amount, bool showCounter)
	{
	}

	public override void Get(bool showPopup = true)
	{
		Debug.LogErrorFormat(this, "\"{0}\" has no \"Get()\" implementation.", base.name);
	}

	public virtual Sprite GetQuestCounterSprite(int index)
	{
		return GetPopupIcon();
	}

	public Sprite GetUIMsgSprite()
	{
		return GetPopupIcon();
	}

	public string GetUIMsgName()
	{
		return GetPopupName();
	}

	public virtual float GetUIMsgIconScale()
	{
		return 1f;
	}

	public UnityEngine.Object GetRepresentingObject()
	{
		return this;
	}

	public virtual IEnumerable<QuestTargetCounter> EnumerateSubTargets()
	{
		yield break;
	}
}
