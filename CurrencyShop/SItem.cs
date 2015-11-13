using System;
using Terraria;
using TShockAPI;

namespace CurrencyShop
{
	public class SItem
	{
		public int netID;
		public int stack;
		public byte prefix;
		public int cost;

		public SItem()
		{

		}

		public SItem(int netID, int stack, int cost, byte prefix = 0)
		{
			this.netID = netID;
			this.stack = stack;
			this.prefix = prefix;
			this.cost = cost;

			if (TShock.Utils.GetItemById(netID) == null)
				throw new InvalidItemException(netID);
			if (prefix != 0 && String.IsNullOrWhiteSpace(TShock.Utils.GetPrefixById(prefix)))
				throw new InvalidPrefixException(prefix);
		}

		//public int GetNetID()
		//{
		//	if (string.IsNullOrWhiteSpace(netID))
		//		return -1;
		//	List<Item> items = TShock.Utils.GetItemByIdOrName(netID);
		//	if (items.Count != 1)
		//		return -1;
		//	return items[0].netID;
		//}

		public string GetName()
		{
			Item item = TShock.Utils.GetItemById(netID);
			if (item == null)
				throw new InvalidItemException(netID);
			return item.name;
		}

		public string GetPrefixName()
		{
			Item item = TShock.Utils.GetItemById(netID);
			if (item == null)
				throw new InvalidItemException(netID);
			return TShock.Utils.GetPrefixById(prefix);
		}

		public override string ToString()
		{
			return netID.ToString();
		}
	}
}
