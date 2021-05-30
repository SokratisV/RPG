﻿using RPG.Core;
using UnityEngine;

namespace RPG.Dialogue
{
	public class AIConversant : MonoBehaviour, IRaycastable, IInteractable
	{
		[SerializeField] private Dialogue dialogue;
		[SerializeField] private string characterName;

		private OutlineableComponent _outlineableComponent;
		private PlayerConversant _playerConversant = null;

		public string Name => characterName;

		#region Unity

		private void Awake() => _outlineableComponent = new OutlineableComponent(gameObject, GlobalValues.InteractColor);

		private void Start() => _playerConversant = PlayerFinder.Player.GetComponent<PlayerConversant>();

		#endregion

		#region Public

		public CursorType GetCursorType() => CursorType.Dialogue;

		public bool HandleRaycast(GameObject player)
		{
			if(dialogue == null) return false;
			CheckPressedButtons(_playerConversant);
			return true;
		}
		
		public void ShowInteractivity() => _outlineableComponent.ShowOutline(this);

		public Transform GetTransform() => transform;

		public void Interact() => _playerConversant.StartDialogue(this, dialogue);

		public float InteractionDistance() => 2f;

		#endregion

		#region Private

		private void CheckPressedButtons(PlayerConversant playerConversant)
		{
			if(Input.GetKey(KeyCode.LeftControl))
			{
				if(Input.GetMouseButtonDown(0))
				{
					playerConversant.QueueInteractAction(gameObject);
				}
			}
			else
			{
				if(Input.GetMouseButton(0))
				{
					playerConversant.StartInteractAction(this);
				}
			}
		}

		#endregion
	}
}