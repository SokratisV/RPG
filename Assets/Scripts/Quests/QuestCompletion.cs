﻿using RPG.Core;
using UnityEngine;

namespace RPG.Quests
{
	public class QuestCompletion : MonoBehaviour
	{
		[SerializeField] private Quest quest;
		[SerializeField] private string objective;

		//Unity Event
		public void CompleteObjective()
		{
			var questList = PlayerFinder.Player.GetComponent<QuestList>();
			questList.CompleteObjective(quest, objective);
		}
	}
}