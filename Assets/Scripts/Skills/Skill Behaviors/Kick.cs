﻿using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Skills.Behaviors
{
	public class Kick : SkillBehavior
	{
		[Min(0)] [SerializeField] private float damage;
		[Min(0)] [SerializeField] private float kickRange;

		private Trigger _trigger;

		public override bool HasCastTime() => true;
		public override int SkillAnimationNumber() => 3;
		public override float GetCastingRange() => kickRange;
		public override bool AdjustAnimationSpeed => false;

		public override IEnumerator BehaviorUpdate(GameObject user, List<GameObject> targets, Vector3? point = null)
		{
			while (true)
			{
				if (_trigger.Value)
				{
					if (targets[0] != null)
					{
						var health = targets[0].GetComponent<Health>();
						health.TakeDamage(user, damage);
						yield break;
					}
				}
				else yield return null;
			}
		}

		public override void OnAnimationEvent() => _trigger.Value = true;
	}
}