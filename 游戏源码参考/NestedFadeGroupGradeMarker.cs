using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

[ExecuteInEditMode]
[NestedFadeGroupBridge(new Type[] { typeof(GradeMarker) })]
[RequireComponent(typeof(GradeMarker))]
public class NestedFadeGroupGradeMarker : NestedFadeGroupBase
{
	private GradeMarker gradeMarker;

	protected override void GetMissingReferences()
	{
		if (!gradeMarker)
		{
			gradeMarker = GetComponent<GradeMarker>();
		}
	}

	protected override void OnAlphaChanged(float alpha)
	{
		gradeMarker.Alpha = alpha;
	}
}
