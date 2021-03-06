﻿using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
	public class DialogueEditor : EditorWindow
	{
		private Dialogue _selectedDialogue = null;
		private Vector2 _scrollPosition;
		[NonSerialized] private bool _draggingCanvas;
		[NonSerialized] private Vector2 _draggingCanvasOffset, _draggingOffset;
		[NonSerialized] private GUIStyle _nodeStyle, _playerNodeStyle, _textFieldStyle;
		[NonSerialized] private DialogueNode _draggingNode = null;
		[NonSerialized] private DialogueNode _creatingNode = null;
		[NonSerialized] private DialogueNode _deletingNode = null;
		[NonSerialized] private DialogueNode _linkingNode = null;
		private Texture2D background;

		private const float CanvasSize = 4000;
		private const float BackgroundSize = 50;

		[MenuItem("RPG/Dialogues")]
		private static void ShowWindow() => GetWindow<DialogueEditor>(false, "Dialogue");

		[OnOpenAsset(1)]
		private static bool OpenDialogue(int instanceID, int line)
		{
			if (EditorUtility.InstanceIDToObject(instanceID) is Dialogue)
			{
				ShowWindow();
			}

			return false;
		}

		private void OnEnable()
		{
			background = Resources.Load("background") as Texture2D;
			Selection.selectionChanged += OnSelectionChanged;
			_nodeStyle = new GUIStyle
			{
				normal = {background = EditorGUIUtility.Load("node0") as Texture2D, textColor = Color.white},
				padding = new RectOffset(20, 20, 20, 20),
				border = new RectOffset(12, 12, 12, 12)
			};
			_playerNodeStyle = new GUIStyle
			{
				normal = {background = EditorGUIUtility.Load("node1") as Texture2D, textColor = Color.white},
				padding = new RectOffset(20, 20, 20, 20),
				border = new RectOffset(12, 12, 12, 12)
			};
		}

		private void OnSelectionChanged()
		{
			var obj = Selection.activeObject;
			if (obj is Dialogue dialogue)
			{
				_selectedDialogue = dialogue;
				Repaint();
			}
		}

		private void OnGUI()
		{
			if (_selectedDialogue == null)
			{
				EditorGUILayout.LabelField("No Dialogue Selected");
			}
			else
			{
				ProcessEvents();
				EditorGUILayout.BeginScrollView(_scrollPosition);
				var canvas = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
				GUI.DrawTextureWithTexCoords(canvas, background, new Rect(0, 0, CanvasSize / BackgroundSize, CanvasSize / BackgroundSize));

				foreach (var node in _selectedDialogue.GetAllNodes())
				{
					DrawConnections(node);
				}

				foreach (var node in _selectedDialogue.GetAllNodes())
				{
					DrawNode(node);
				}

				EditorGUILayout.EndScrollView();

				if (_creatingNode != null)
				{
					_selectedDialogue.CreateNode(_creatingNode);
					_creatingNode = null;
				}

				if (_deletingNode != null)
				{
					_selectedDialogue.DeleteNode(_deletingNode);
					_deletingNode = null;
				}
			}
		}

		private void ProcessEvents()
		{
			if (Event.current.type == EventType.MouseDown && _draggingNode == null)
			{
				_draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
				if (_draggingNode != null)
				{
					_draggingOffset = _draggingNode.Rect.position - Event.current.mousePosition;
					Selection.activeObject = _draggingNode;
				}
				else
				{
					_draggingCanvas = true;
					_draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
					Selection.activeObject = _selectedDialogue;
				}
			}
			else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
			{
				Undo.RecordObject(_selectedDialogue, "Move Dialogue Node");
				_draggingNode.SetPosition(Event.current.mousePosition + _draggingOffset);
				GUI.changed = true;
			}
			else if (Event.current.type == EventType.MouseDrag && _draggingCanvas)
			{
				_scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
				GUI.changed = true;
			}
			else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
			{
				_draggingNode = null;
			}
			else if (Event.current.type == EventType.MouseUp && _draggingCanvas)
			{
				_draggingCanvas = false;
			}
		}

		private void DrawNode(DialogueNode node)
		{
			var style = _nodeStyle;
			if (node.IsPlayerSpeaking) style = _playerNodeStyle;
			GUILayout.BeginArea(node.Rect, style);
			node.SetText(EditorGUILayout.TextField(node.Text));
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("X")) _deletingNode = node;
			DrawLinkButtons(node);
			if (GUILayout.Button("+")) _creatingNode = node;
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void DrawLinkButtons(DialogueNode node)
		{
			if (_linkingNode == null)
			{
				if (GUILayout.Button("Link"))
				{
					_linkingNode = node;
				}
			}
			else if (_linkingNode == node)
			{
				if (GUILayout.Button("Link"))
				{
					_linkingNode = null;
				}
			}
			else if (_linkingNode.Children.Contains(node.name))
			{
				if (GUILayout.Button("Unlink"))
				{
					_linkingNode.RemoveChild(node.name);
					_linkingNode = null;
				}
			}
			else
			{
				if (GUILayout.Button("Child"))
				{
					Undo.RecordObject(_selectedDialogue, "Add Dialogue Link");
					_linkingNode.AddChild(node.name);
					_linkingNode = null;
				}
			}
		}

		private void DrawConnections(DialogueNode node)
		{
			Vector3 startPosition = new Vector2(node.Rect.xMax, node.Rect.center.y);
			foreach (var childNode in _selectedDialogue.GetAllChildren(node))
			{
				Vector3 endPosition = new Vector2(childNode.Rect.xMin, childNode.Rect.center.y);
				Vector3 controlPointOffset = endPosition - startPosition;
				controlPointOffset.y = 0;
				controlPointOffset.x *= 0.8f;
				Handles.DrawBezier(
					startPosition, endPosition,
					startPosition + controlPointOffset,
					endPosition - controlPointOffset,
					Color.white, null, 4f);
			}
		}

		private DialogueNode GetNodeAtPoint(Vector2 point) => _selectedDialogue.GetAllNodes().LastOrDefault(node => node.Rect.Contains(point));
	}
}