﻿using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
	/// <summary>
	/// Provides the storage for an action bar. The bar has a finite number of
	/// slots that can be filled and actions in the slots can be "used".
	/// 
	/// This component should be placed on the GameObject tagged "Player".
	/// </summary>
	public class ActionStore : MonoBehaviour, ISaveable
	{
		// STATE
		private Dictionary<int, DockedItemSlot> dockedItems = new Dictionary<int, DockedItemSlot>();

		private class DockedItemSlot
		{
			public ActionItem item;
			public int number;
		}

		// PUBLIC

		/// <summary>
		/// Broadcasts when the items in the slots are added/removed.
		/// </summary>
		public event Action StoreUpdated;

		/// <summary>
		/// Get the action at the given index.
		/// </summary>
		public ActionItem GetAction(int index) => dockedItems.ContainsKey(index)? dockedItems[index].item:null;

		/// <summary>
		/// Get the number of items left at the given index.
		/// </summary>
		/// <returns>
		/// Will return 0 if no item is in the index or the item has
		/// been fully consumed.
		/// </returns>
		public int GetNumber(int index) => dockedItems.ContainsKey(index)? dockedItems[index].number:0;

		/// <summary>
		/// Add an item to the given index.
		/// </summary>
		/// <param name="item">What item should be added.</param>
		/// <param name="index">Where should the item be added.</param>
		/// <param name="number">How many items to add.</param>
		public void AddAction(InventoryItem item, int index, int number)
		{
			if(dockedItems.ContainsKey(index))
			{
				if(ReferenceEquals(item, dockedItems[index].item))
				{
					dockedItems[index].number += number;
				}
			}
			else
			{
				var slot = new DockedItemSlot {item = item as ActionItem, number = number};
				dockedItems[index] = slot;
			}

			StoreUpdated?.Invoke();
		}

		/// <summary>
		/// Use the item at the given slot. If the item is consumable one
		/// instance will be destroyed until the item is removed completely.
		/// </summary>
		/// <param name="user">The character that wants to use this action.</param>
		/// <returns>False if the action could not be executed.</returns>
		public bool Use(int index, GameObject user)
		{
			if(!dockedItems.ContainsKey(index)) return false;
			dockedItems[index].item.Use(user);
			if(dockedItems[index].item.IsConsumable)
			{
				RemoveItems(index, 1);
			}

			return true;

		}

		/// <summary>
		/// Remove a given number of items from the given slot.
		/// </summary>
		public void RemoveItems(int index, int number)
		{
			if(dockedItems.ContainsKey(index))
			{
				dockedItems[index].number -= number;
				if(dockedItems[index].number <= 0)
				{
					dockedItems.Remove(index);
				}

				StoreUpdated?.Invoke();
			}
		}

		/// <summary>
		/// What is the maximum number of items allowed in this slot.
		/// 
		/// This takes into account whether the slot already contains an item
		/// and whether it is the same type. Will only accept multiple if the
		/// item is consumable.
		/// </summary>
		/// <returns>Will return int.MaxValue when there is not effective bound.</returns>
		public int MaxAcceptable(InventoryItem item, int index)
		{
			var actionItem = item as ActionItem;
			if(!actionItem) return 0;

			if(dockedItems.ContainsKey(index) && !object.ReferenceEquals(item, dockedItems[index].item))
				return 0;

			if(actionItem.IsConsumable)
				return int.MaxValue;

			return dockedItems.ContainsKey(index)? 0:1;
		}

		[Serializable]
		private struct DockedItemRecord
		{
			public string itemID;
			public int number;
		}

		object ISaveable.CaptureState()
		{
			var state = new Dictionary<int, DockedItemRecord>();
			foreach(var pair in dockedItems)
			{
				var record = new DockedItemRecord {itemID = pair.Value.item.ItemID, number = pair.Value.number};
				state[pair.Key] = record;
			}

			return state;
		}

		void ISaveable.RestoreState(object state)
		{
			var stateDict = (Dictionary<int, DockedItemRecord>)state;
			foreach(var pair in stateDict)
			{
				AddAction(InventoryItem.GetFromID(pair.Value.itemID), pair.Key, pair.Value.number);
			}
		}
	}
}