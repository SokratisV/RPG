﻿using System;
using System.Linq;
using RPG.Utils;
using UnityEngine;

namespace RPG.Stats
{
	public class BaseStats : MonoBehaviour
	{
		[SerializeField] [Range(1, 99)] private int startingLevel = 1;
		[SerializeField] private CharacterClass characterClass;
		[SerializeField] private Progression progression = null;
		[SerializeField] private GameObject levelUpParticleEffect = null;
		[SerializeField] private bool shouldUseModifiers = false;

		public event Action OnLevelUp;
		private Experience _experience;
		private LazyValue<int> _currentLevel;

		private void Awake()
		{
			_experience = GetComponent<Experience>();
			_currentLevel = new LazyValue<int>(CalculateLevel);
		}

		private void Start() => _currentLevel.ForceInit();

		private void OnEnable()
		{
			if (_experience != null)
			{
				_experience.OnExperienceGained += UpdateLevel;
			}
		}

		private void OnDisable()
		{
			if (_experience != null)
			{
				_experience.OnExperienceGained -= UpdateLevel;
			}
		}

		private void UpdateLevel(float _)
		{
			var newLevel = CalculateLevel();
			if (newLevel > _currentLevel.Value)
			{
				_currentLevel.Value = newLevel;
				LevelUpEffect();
				OnLevelUp?.Invoke();
			}
		}

		private void LevelUpEffect() => Instantiate(levelUpParticleEffect, transform);

		public float GetStat(Stat stat) => (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);

		private float GetBaseStat(Stat stat) => progression.GetStat(stat, characterClass, GetLevel());

		private float GetAdditiveModifier(Stat stat)
		{
			if (!shouldUseModifiers) return 0;
			return GetComponents<IModifierProvider>().SelectMany(provider => provider.GetAdditiveModifiers(stat)).Sum();
		}

		private float GetPercentageModifier(Stat stat)
		{
			if (!shouldUseModifiers) return 0;
			return GetComponents<IModifierProvider>().SelectMany(provider => provider.GetPercentageModifiers(stat)).Sum();
		}

		public int GetLevel() => _currentLevel.Value;

		public float GetCurrentLevelExperience() => progression.GetStat(Stat.ExperienceToLevelUp, characterClass, GetLevel());
		public float GetPreviousLevelExperience() => progression.GetStat(Stat.ExperienceToLevelUp, characterClass, GetLevel() - 1);

		public int CalculateLevel()
		{
			if (_experience == null) return startingLevel;

			var currentXp = _experience.GetPoints();
			var penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
			for (var level = 1; level <= penultimateLevel; level++)
			{
				var xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
				if (xpToLevelUp > currentXp)
				{
					return level;
				}
			}

			return penultimateLevel + 1;
		}
	}
}