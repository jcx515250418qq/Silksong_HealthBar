using System;

[Serializable]
public class QuestCompletionData : SerializableNamedList<QuestCompletionData.Completion, QuestCompletionData.NamedCompletion>
{
	[Serializable]
	public class NamedCompletion : SerializableNamedData<Completion>
	{
	}

	[Serializable]
	public struct Completion
	{
		public bool HasBeenSeen;

		public bool IsAccepted;

		public int CompletedCount;

		public bool IsCompleted;

		public bool WasEverCompleted;

		public void SetCompleted()
		{
			IsCompleted = true;
			WasEverCompleted = true;
		}
	}

	public static Completion Accepted
	{
		get
		{
			Completion result = default(Completion);
			result.IsAccepted = true;
			return result;
		}
	}

	public static Completion Completed
	{
		get
		{
			Completion result = default(Completion);
			result.IsAccepted = true;
			result.IsCompleted = true;
			return result;
		}
	}
}
