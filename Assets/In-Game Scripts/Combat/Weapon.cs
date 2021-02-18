﻿using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
	public class Weapon : MonoBehaviour
	{
		[SerializeField] private UnityEvent onHit;

		// Animation event
		public void OnHit() => onHit.Invoke();

		public void Destroy() => Destroy(gameObject);
	}
}