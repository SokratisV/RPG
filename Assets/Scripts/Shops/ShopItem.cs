﻿using System;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Shops
{
	[Serializable]
	public class ShopItem
	{
		private InventoryItem _item;
		private int _availability;
		private float _price;
		private int _quantityInTransactionInTransaction;

		public ShopItem(InventoryItem item, int availability, float price, int quantityInTransactionInTransaction)
		{
			_item = item;
			_availability = availability;
			_price = price;
			_quantityInTransactionInTransaction = quantityInTransactionInTransaction;
		}

		public string Name => _item.DisplayName;
		public Sprite Icon => _item.Icon;
		public int Availability => _availability;
		public float Price => _price;
		public InventoryItem InventoryItem => _item;
		public int QuantityInTransaction => _quantityInTransactionInTransaction;
	}
}