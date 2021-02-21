﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
	public class DialogueNode : ScriptableObject
	{
		[SerializeField] private string text;
		[SerializeField] private List<string> children = new List<string>();
		[SerializeField] private Rect rect = new Rect(0, 0, 200, 100);

		public string Text => text;
		public List<string> Children => children;
		public Rect Rect => rect;

#if UNITY_EDITOR
		public void SetPosition(Vector2 position)
		{
			Undo.RecordObject(this, "Move Dialogue Node ");
			rect.position = position;
			EditorUtility.SetDirty(this);
		}

		public void SetText(string value)
		{
			if(!string.Equals(value, text, StringComparison.Ordinal))
			{
				Undo.RecordObject(this, "Update Dialogue Text ");
				text = value;
				EditorUtility.SetDirty(this);
			}
		}

		public void AddChild(string childID)
		{
			Undo.RecordObject(this, "Add Dialogue Link ");
			children.Add(childID);
			EditorUtility.SetDirty(this);
		}
		
		public void RemoveChild(string childID)
		{
			Undo.RecordObject(this, "Remove Dialogue Link ");
			children.Remove(childID);
			EditorUtility.SetDirty(this);
		}
#endif
	}
}