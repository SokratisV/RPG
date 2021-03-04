﻿using System;
using System.Collections.Generic;
using RPG.Skills.Behaviors;
using UnityEngine;

namespace RPG.Skills
{
	[CreateAssetMenu(fileName = "Skill", menuName = "RPG/Skills/New Skill", order = 0)]
	public class Skill : ScriptableObject, ISerializationCallbackReceiver
	{
		[Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")] [SerializeField]
		private string skillID;

		[Tooltip("Item name to be displayed in UI.")] [SerializeField]
		private string displayName = null;

		[Tooltip("Item description to be displayed in UI.")] [SerializeField] [TextArea]
		private string description = null;

		[Tooltip("The UI icon to represent this item in the inventory.")] [SerializeField]
		private Sprite icon = null;

		[SerializeField] private float duration;
		[SerializeField] private TargetBehavior targetBehavior;
		[SerializeField] private SkillBehavior skillBehavior;
		[SerializeField] private GameObject[] vfxOnUser, vfxOnTarget;
		[SerializeField] private AudioClip[] sfxOnUser, sfxOnTarget;

		public string SkillID => skillID;
		public Sprite Icon => icon;
		public string DisplayName => displayName;
		public string Description => description;
		public float Duration => duration;

		private static Dictionary<string, Skill> ItemLookupCache;

		public void OnStart(GameObject user)
		{
			skillBehavior.OnStart += PlayCasterVfx;
			skillBehavior.BehaviorStart(user, targetBehavior.GetTarget(user));
		}

		public void OnUpdate()
		{
		}

		public void OnEnd()
		{
		}

		private void PlayCasterVfx(GameObject user, CustomTarget customTarget)
		{
			foreach(var vfx in vfxOnUser)
			{
				Instantiate(vfx, user.transform);
			}
		}

		public static Skill GetFromID(string skillID)
		{
			if(ItemLookupCache == null)
			{
				ItemLookupCache = new Dictionary<string, Skill>();
				var itemList = Resources.LoadAll<Skill>("");
				foreach(var item in itemList)
				{
					if(ItemLookupCache.ContainsKey(item.skillID))
					{
						Debug.LogError($"Looks like there's a duplicate RPG.UI.InventorySystem ID for objects: {ItemLookupCache[item.skillID]} and {item}");
						continue;
					}

					ItemLookupCache[item.skillID] = item;
				}
			}

			if(skillID == null || !ItemLookupCache.ContainsKey(skillID)) return null;
			return ItemLookupCache[skillID];
		}

		public void OnBeforeSerialize()
		{
			if(string.IsNullOrWhiteSpace(skillID))
			{
				skillID = Guid.NewGuid().ToString();
			}

			if(string.IsNullOrWhiteSpace(displayName))
			{
				displayName = name;
			}
		}

		public void OnAfterDeserialize()
		{
		}
	}
}