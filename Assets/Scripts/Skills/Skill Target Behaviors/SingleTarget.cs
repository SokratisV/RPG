﻿using System.Collections.Generic;
using UnityEngine;

namespace RPG.Skills.Behaviors
{
	public class SingleTarget : TargetBehavior
	{
		public override bool? RequireTarget() => true;

		public override bool GetTargets(out List<GameObject> targets, GameObject user, GameObject initialTarget = null, Vector3? raycastPoint = null)
		{
			targets = initialTarget == null? null:targets = new List<GameObject> {initialTarget};
			return true;
		}
	}
}