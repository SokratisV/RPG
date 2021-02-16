using System;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.AnimatorBehaviors;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
	public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
	{
		public event Action OnActionComplete;

		[SerializeField] private float timeBetweenAttacks = 1f;
		[SerializeField] private Transform rightHandTransform = null;
		[SerializeField] private Transform leftHandTransform = null;
		[SerializeField] private WeaponConfig defaultWeapon = null;

		private WeaponConfig _currentWeaponConfig;
		private Mover _mover;
		private ActionScheduler _actionScheduler;
		private Animator _animator;
		private LazyValue<Weapon> _currentWeapon;
		private Health _target;
		private BaseStats _stats;
		private float _timeSinceLastAttack = Mathf.Infinity;
		private bool _isCurrentAnimationDone = true;
		private static readonly int StopAttackHash = Animator.StringToHash("stopAttack");
		private static readonly int AttackHash = Animator.StringToHash("attack");

		private void Start()
		{
			_currentWeapon.ForceInit();
			var attackSpeedBehaviors = _animator.GetBehaviours<RandomAttackAnimBehavior>();
			foreach(var behaviour in attackSpeedBehaviors)
			{
				behaviour.TimeBetweenAttacks = timeBetweenAttacks;
			}
		}

		private void Awake()
		{
			_currentWeaponConfig = defaultWeapon;
			_currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
			_animator = GetComponent<Animator>();
			_actionScheduler = GetComponent<ActionScheduler>();
			_mover = GetComponent<Mover>();
			_stats = GetComponent<BaseStats>();
			var attackListenerBehavior = _animator.GetBehaviour<AttackAnimationInfo>();
			attackListenerBehavior.OnAnimationComplete += () => _isCurrentAnimationDone = true;
		}

		private Weapon SetupDefaultWeapon() => AttachWeapon(defaultWeapon);

		public void EquipWeapon(WeaponConfig weapon)
		{
			_currentWeaponConfig = weapon;
			_currentWeapon.Value = AttachWeapon(weapon);
		}

		private Weapon AttachWeapon(WeaponConfig weapon) => weapon.Spawn(rightHandTransform, leftHandTransform, _animator);

		public Health GetTarget() => _target;

		public WeaponConfig GetWeaponConfig() => _currentWeaponConfig;

		private void Update()
		{
			_timeSinceLastAttack += Time.deltaTime;
			if(_target == null) return;
			if(_target.IsDead)
			{
				CompleteAction();
				_target = null;
				return;
			}

			if(IsInRange(_target.transform))
			{
				_mover.CancelAction();
				Attack();
			}
			else
			{
				if (_isCurrentAnimationDone)
					_mover.MoveWithoutAction(_target.transform.position);
			}
		}

		private void Attack()
		{
			transform.LookAt(_target.transform);
			if(!(_timeSinceLastAttack > timeBetweenAttacks)) return;
			AttackAnimation();
			_timeSinceLastAttack = 0;
		}

		private void AttackAnimation()
		{
			_isCurrentAnimationDone = false;
			_animator.ResetTrigger(StopAttackHash);
			_animator.SetTrigger(AttackHash);
		}

		// Animation Event
		private void Hit()
		{
			var damage = _stats.GetStat(Stat.Damage);
			if(_currentWeapon.Value != null) _currentWeapon.Value.OnHit();

			if(_target == null) return;
			if(_currentWeaponConfig.HasProjectile())
			{
				_currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _target, gameObject, damage);
			}
			else
			{
				_target.TakeDamage(gameObject, damage);
			}
		}

		private void Shoot() => Hit();

		public bool CanAttack(GameObject target)
		{
			if(target == null) return false;
			if(!_mover.CanMoveTo(target.transform.position) && !IsInRange(target.transform)) return false;
			var health = target.GetComponent<Health>();
			return health != null && !health.IsDead;
		}

		private bool IsInRange(Transform target) => Helper.IsWithinDistance(transform, target, _currentWeaponConfig.GetRange());

		public void StartAttackAction(GameObject combatTarget)
		{
			_target ??= combatTarget.GetComponent<Health>();
			_actionScheduler.StartAction(this);
		}

		public void QueueAttackAction(GameObject obj) => _actionScheduler.EnqueueAction(new FighterActionData(this, obj));

		public void CancelAction()
		{
			StopAttack();
			_mover.CancelAction();
			_target = null;
		}

		private void StopAttack()
		{
			_animator.ResetTrigger(AttackHash);
			_animator.SetTrigger(StopAttackHash);
		}

		public object CaptureState() => _currentWeaponConfig.name;

		public void RestoreState(object state)
		{
			_currentWeaponConfig = Resources.Load<WeaponConfig>((string)state);
			EquipWeapon(_currentWeaponConfig);
		}

		public IEnumerable<float> GetAdditiveModifiers(Stat stat)
		{
			if(stat == Stat.Damage)
				yield return _currentWeaponConfig.GetDamage();
		}

		public IEnumerable<float> GetPercentageModifiers(Stat stat)
		{
			if(stat == Stat.Damage)
				yield return _currentWeaponConfig.GetPercentageBonus();
		}

		public void CompleteAction()
		{
			OnActionComplete?.Invoke();
			_actionScheduler.CompleteAction();
		}

		public void ExecuteAction(IActionData data) => _target = ((FighterActionData)data).Target.GetComponent<Health>();
	}
}