﻿using UnityEngine;

namespace RPG.Core
{
	public static class GlobalValues
	{
		public static float OutlineOffDelay {get;} = .05f;
		public static Color32 PickupColor {get;} = new Color32(255, 255, 0, 255);
		public static Color32 EnemyColor {get;} = new Color32(255, 0, 0, 255);
		public static Color32 InteractColor {get;} = new Color32(0, 255, 0, 255);
		public static float InteractableRange {get;} = 1f;
		public static float DefaultAttackSpeed {get;} = 1f;
		public static string PlayerName {get;} = "Adramalikh";
	}
}