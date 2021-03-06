﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RPG.UI
{
	public class ShowHideUIOnButtonPress : MonoBehaviour
	{
		public UnityEvent<bool> ActionOnToggle;

		[SerializeField] private bool toggleOnEnable;
		[SerializeField] private RectTransform uiContainer = null;
		[SerializeField] private KeyCode toggleKey = KeyCode.Escape, alternateToggle = KeyCode.None;
		[SerializeField] private Vector2 hiddenPosition;
		[SerializeField] private float tweenTime = .2f;
		[SerializeField] private bool startState;
		[SerializeField] private bool disableRaycastingOnHide = false;
		[SerializeField] private bool changeSortingOrder = true;

		private static int _sortingOrder;
		private bool _toggleOnEnable;
		private Vector2 _initialPosition;
		private Canvas _canvas;
		private GraphicRaycaster _raycaster;

		private void Awake()
		{
			_canvas = GetComponent<Canvas>();
			_raycaster = GetComponent<GraphicRaycaster>();
			_initialPosition = uiContainer.anchoredPosition;
			Toggle(startState);
		}

		private void Start() => _toggleOnEnable = toggleOnEnable; //To avoid double callback on Awake/Enabled

		private void OnEnable()
		{
			if (_toggleOnEnable) Toggle(true);
		}

		private void OnDisable()
		{
			if (_toggleOnEnable) Toggle(false);
		}

		private void Update()
		{
			if (Input.GetKeyDown(toggleKey) || Input.GetKeyDown(alternateToggle))
			{
				Toggle(!_canvas.enabled);
			}
		}

		[ContextMenu("Toggle")]
		public void Toggle() => Toggle(!_canvas.enabled);

		public void Toggle(bool toggle)
		{
			if (toggle == _canvas.enabled) return;
			ActionOnToggle?.Invoke(toggle);
			if (!toggle)
			{
				LeanTween.cancel(uiContainer);
				LeanTween.move(uiContainer, hiddenPosition, tweenTime).setEaseInOutExpo().setOnComplete(ToggleCanvas).setIgnoreTimeScale(true);
				ToggleRaycaster();
			}
			else
			{
				ToggleCanvas();
				LeanTween.cancel(uiContainer);
				LeanTween.move(uiContainer, _initialPosition, tweenTime).setEaseInOutExpo().setOnComplete(ToggleRaycaster).setIgnoreTimeScale(true);
			}
		}

		private void ToggleCanvas()
		{
			_canvas.enabled = !_canvas.enabled;
			if (!changeSortingOrder) return;
			if (_canvas.enabled) _canvas.sortingOrder = ++_sortingOrder;
			else _canvas.sortingOrder = 0;
		}

		private void ToggleRaycaster()
		{
			if (!disableRaycastingOnHide) return;
			_raycaster.enabled = _canvas.enabled;
		}
	}
}