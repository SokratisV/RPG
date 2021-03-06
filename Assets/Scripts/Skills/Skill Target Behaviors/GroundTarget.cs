﻿using System.Collections.Generic;
using UnityEngine;

namespace RPG.Skills.Behaviors
{
	public class GroundTarget : TargetBehavior
	{
		public override bool? RequireTarget() => false;

		public override bool GetTargets(out List<GameObject> targets, GameObject user, GameObject initialTarget = null, Vector3? raycastPoint = null)
		{
			targets = null;
			return false;
		}
	}
}