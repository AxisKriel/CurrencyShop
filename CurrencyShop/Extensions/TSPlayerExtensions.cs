using System;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using CurrencyBank;
using Terraria;
using Terraria.ID;
using static CurrencyShop.CShop;

namespace CurrencyShop.Extensions
{
	public static class TSPlayerExtensions
	{
		public static void SendItemMatches(this TSPlayer player, List<SItem> matches)
		{
			if (matches.Count < 1)
				player.SendSuccessMessage($"{Tag} No items matched.");
			else
			{
				player.SendSuccessMessage($"{Tag} Item matches:");
				foreach (SItem i in matches)
				{
					string prefix = i.prefix > 0 ? i.GetPrefixName() + " " : "";
					player.SendInfoMessage($"{Tag} " + $"[{BankMain.FormatMoney(i.cost)}] {prefix}{i.GetName()} x{i.stack}");
				}
			}
		}

		public static void SendKitMatches(this TSPlayer player, List<Kit> matches)
		{
			if (matches.Count < 1)
				player.SendSuccessMessage($"{Tag} No kits matched.");
			else
			{
				player.SendSuccessMessage($"{Tag} Kit matches (use -k [kit] for more info):");
				foreach (Kit k in matches)
				{
					string itemPreview = String.Join(", ", k.items.Select(i => i.GetName())).Substring(0, 50 - k.name.Length);
					player.SendInfoMessage($"{Tag} [{BankMain.FormatMoney(k.cost)}] {k.name}: {itemPreview}");
				}
			}
		}

		public static List<int> GetEmptyInventorySlots(this TSPlayer player)
		{
			var empties = new List<int>();
			for (int i = 0; i < Main.realInventory; i++)
			{
				if (player.TPlayer.inventory[i].netID == 0)
					empties.Add(i);
			}
			return empties;
		}

		public static bool GiveItemSSC(this TSPlayer player, int netID, int stack, byte prefix = 0)
		{
			var slots = new List<int>();

			for (int i = 0; i < Main.realInventory && stack > 0; i++)
			{
				if (player.TPlayer.inventory[i].netID == netID && player.TPlayer.inventory[i].stack < player.TPlayer.inventory[i].maxStack)
				{
					slots.Add(i);
					while (player.TPlayer.inventory[i].stack < player.TPlayer.inventory[i].maxStack && stack > 0)
					{
						player.TPlayer.inventory[i].stack++;
						stack--;
					}
				}
			}

			for (int i = 0; i < Main.realInventory && stack > 0; i++)
			{
				if (player.TPlayer.inventory[i].netID == ItemID.None)
				{
					slots.Add(i);
					player.TPlayer.inventory[i].netDefaults(netID);
					player.TPlayer.inventory[i].Prefix(prefix);
					stack--;
					while (player.TPlayer.inventory[i].stack < player.TPlayer.inventory[i].maxStack && stack > 0)
					{
						player.TPlayer.inventory[i].stack++;
						stack--;
					}
				}
			}

			foreach (int s in slots)
				player.SendData(PacketTypes.PlayerSlot, "", player.Index, s);
			return stack == 0;
		}
	}
}
