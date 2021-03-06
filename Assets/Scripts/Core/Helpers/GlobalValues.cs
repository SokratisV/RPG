﻿using UnityEngine;

namespace RPG.Core
{
	public static class GlobalValues
	{
		public static float OutlineOffDelay { get; } = .05f;
		public static Color32 PickupColor { get; } = new Color32(255, 255, 0, 255);
		public static Color32 EnemyColor { get; } = new Color32(255, 0, 0, 255);
		public static Color32 InteractColor { get; } = new Color32(0, 255, 0, 255);
		public static Color32 ShopInteractableColor { get; } = new Color32(0, 180, 180, 255);
		public static float InteractableRange { get; } = 1f;
		public static float MaxNavPathLength { get; } = 40f;
		public static int ActionBarCount { get; } = 4;
		public static float GlobalCooldown { get; } = 1f;
		public static string PlayerName { get; } = "Adramalikh";
		public static float DodgeDuration { get; } = .2f;
		public const int MaxLevel = 10;
		public const string Resolution = "resolution";
		public const string MusicVolume = "musicVolume";
		public const string CameraRotationSpeed = "cameraRotationSpeed";
		public const string CameraZoomSpeed = "cameraZoomSpeed";
	}
}