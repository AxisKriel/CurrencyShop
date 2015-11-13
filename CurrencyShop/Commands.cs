using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CurrencyBank;
using CurrencyBank.DB;
using CurrencyShop.Extensions;
using Terraria;
using TShockAPI;
using static CurrencyShop.CShop;

namespace CurrencyShop
{
	public class Commands
	{
		private static string specifier = TShockAPI.Commands.Specifier;

		public static async void Shop(CommandArgs args)
		{
			var regex = new Regex(@"^\w+ (?<Switch>\S+)\S* *(?:""?(?<Object>.+?)""?)?(?: (?<Digit>\d+))?$");
			Match match = regex.Match(args.Message);
			if (!match.Success)
			{
				args.Player.SendInfoMessage($"{Tag} Syntax: {specifier}shop [-switch]");
				var switches = new List<string>();
				if (args.Player.Group.HasPermission(Permissions.Buy))
					switches.Add("-b/buy");
				if (args.Player.Group.HasPermission(Permissions.KitsBuy))
					switches.Add("-k/kits");
				if (args.Player.Group.HasPermission(Permissions.Help))
					switches.Add("-h/help [cmd]");
				if (args.Player.Group.HasPermission(Permissions.Search))
					switches.Add("-s/search");
				if (args.Player.Group.HasPermission(Permissions.Reload))
					switches.Add("-r/reload");
				args.Player.SendInfoMessage($"{Tag} Switches: {String.Join(" ", switches)}.");
			}
			else
			{
				SItem item;
				Kit kit;
				BankAccount account;
				int digit;
				switch (match.Groups["Switch"].Value.ToLowerInvariant())
				{
					#region B (Purchase an item)

					case "-b":
					case "buy":
						if (!args.Player.Group.HasPermission(Permissions.Buy))
						{
							args.Player.SendErrorMessage("You do not have access to this command.");
							return;
						}

						string itemName = match.Groups["Object"].Value;
						if (String.IsNullOrWhiteSpace(itemName))
						{
							args.Player.SendErrorMessage($"{Tag} Invalid syntax! Proper syntax: {specifier}shop -b <item name | ID> [amount]");
							return;
						}

						if (itemName.Equals("list", StringComparison.OrdinalIgnoreCase))
						{
							IEnumerable<string> dataToPaginate = PaginationTools.BuildLinesFromTerms(Manager.GetItems().Select(i => i.GetName()));
							digit = 1;
							if (!String.IsNullOrWhiteSpace(match.Groups["Digit"].Value) && !Int32.TryParse(match.Groups["Digit"].Value, out digit))
							{
								args.Player.SendErrorMessage($"{Tag} Invalid page number!");
								return;
							}
							PaginationTools.SendPage(args.Player, digit, dataToPaginate.ToList(), new PaginationTools.Settings()
							{
								HeaderFormat = $"{Tag} Items ({{0}}/{{1}}):",
								FooterFormat = $"{Tag} Type {specifier}shop -b list {{0}} for more.",
								NothingToDisplayString = $"{Tag} No items available."
							});
							return;
						}

						List<SItem> matches;
						try
						{
							matches = await Manager.GetMatchingItems(itemName);
						}
						catch (InvalidItemException)
						{
							args.Player.SendErrorMessage($"{Tag} Invalid item!");
							return;
						}
						if (matches.Count != 1)
							args.Player.SendItemMatches(matches);
						else
						{
							item = matches[0];
							digit = 1;
							if ((account = await BankMain.Bank.GetAsync(args.Player.User?.Name)) == null)
								args.Player.SendErrorMessage($"{Tag} You must have a bank account to use this command.");
							else if (!String.IsNullOrWhiteSpace(match.Groups["Digit"].Value) && !Int32.TryParse(match.Groups["Digit"].Value, out digit))
								args.Player.SendErrorMessage($"{Tag} Invalid amount!");
							else if (account.Balance < item.cost * digit)
								args.Player.SendErrorMessage($"{Tag} You're {BankMain.FormatMoney(item.cost * digit - account.Balance)} short!");
							else if (!args.Player.InventorySlotAvailable)
								args.Player.SendErrorMessage($"{Tag} Your inventory seems full.");
							else
							{
								try
								{
									if (CShop.Config.UseGiveItemSSC)
										args.Player.GiveItemSSC(item.netID, item.stack * digit, item.prefix);
									else
										args.Player.GiveItem(item.netID, item.GetName(), 2, 3, item.stack, item.prefix);

									await BankMain.Bank.ChangeByAsync(account.AccountName, -item.cost * digit);
									BankMain.Log.ItemPurchase(account, new SItem(item.netID, item.stack * digit, item.cost, item.prefix));
									string prefix = item.prefix > 0 ? item.GetPrefixName() + " " : "";
									args.Player.SendSuccessMessage($"{Tag} Bought {item.stack * digit} {prefix}{item.GetName()} for {BankMain.FormatMoney(item.cost * digit)}.");
								}
								catch (NullReferenceException)
								{
									args.Player.SendErrorMessage($"{Tag} Invalid bank account!");
								}
								catch (InvalidOperationException)
								{
									args.Player.SendErrorMessage($"{Tag} Error performing transaction. Possible database corruption.");
									args.Player.SendInfoMessage($"{Tag} Double check if there are multiple accounts with the same ID.");
									args.Player.SendInfoMessage($"{Tag} You can try syncing the server with the database by using the reload command.");
								}
								catch (BankLogException ex)
								{
									TShock.Log.Error(ex.ToString());
								}
							}
						}
						return;

					#endregion

					#region H (Help -.-)

					case "-h":
					case "help":
						if (!args.Player.Group.HasPermission(Permissions.Help))
						{
							args.Player.SendErrorMessage("You do not have access to this command.");
							return;
						}

						string helpString = match.Groups["Object"].Value;
						var helpLines = new List<string>();
						if (String.IsNullOrWhiteSpace(helpString) || helpString.ToLowerInvariant() == "all")
						{
							helpLines.Add($"{TShock.Utils.ColorTag("CurrencyShop", new Color(90, 138, 211))} v{Instance.Version}" +
								$" by {TShock.Utils.ColorTag(Instance.Author, new Color(0, 127, 255))}");
							helpLines.Add($"Available commands: {specifier}cshop <switch> [item | kit] [number]");
							helpLines.Add("Switches: -b (buy) -k (buy kits) -h [cmd] (help) -s (search) -r (reload)");
							helpLines.Add($"Type {specifier}shop -h [switch] for more.");
							if (args.Player.RealPlayer)
								// This color looks like this: http://www.colorcombos.com/images/colors/993366.png
								helpLines.ForEach(l => args.Player.SendMessage(l, new Color(153, 51, 102)));
							else
								// Colors break when sending to console :(
								helpLines.ForEach(l => args.Player.SendInfoMessage(l));
						}
						else
						{
							// Now that I'm sure helpString is not null, let me lower it, for god's sake...
							helpString = helpString.ToLowerInvariant();
							if (helpString.StartsWith("b"))
							{
								// Doco buy command
								helpLines.Add($"{Tag} Syntax: {specifier}shop -b[uy] <item name or id> [amount]");
								helpLines.Add($"{Tag} Purchase an item from the shop inventory.");
								helpLines.Add($"{Tag} If 'amount' is given, it will be multiplied by the item's stack.");
								helpLines.Add($"{Tag} Type '{specifier}shop -b list' to get a list of all available items.");
							}
							else if (helpString.StartsWith("c") || helpString.StartsWith("k"))
							{
								// Doco kits command
								helpLines.Add($"{Tag} Syntax: {specifier}shop -k[it] <list | kit name> [confirm:1]");
								helpLines.Add($"{Tag} Get information about one of the available item kits.");
								helpLines.Add($"{Tag} To buy a kit, type '{specifier}shop -k <kit name> 1' (you can only purchase one kit at once).");
								helpLines.Add($"{Tag} Type '{specifier}shop -k list' to get a list of all available kits.");
							}
							else if (helpString.StartsWith("s"))
							{
								// Doco search command
								helpLines.Add($"{Tag} Syntax: {specifier}shop -s[earch] [item: | kit:] <text>");
								helpLines.Add($"{Tag} Search for an item or kit which name starts with 'text'.");
								helpLines.Add($"{Tag} If 'text' matches one item, this will also search for any kits containing said item.");
								helpLines.Add($"{Tag} You can use 'item:' or 'kit:' before 'text' to search only for items or kits, respectively.");
							}
							else if (helpString.StartsWith("r"))
							{
								//Doco reload command
								helpLines.Add($"{Tag} Syntax: {specifier}shop -r[eload] <config | inventory>");
								helpLines.Add($"{Tag} Reloads CurrencyBank's config file.");
							}
							else
							{
								args.Player.SendErrorMessage($"{Tag} Invalid switch! Type '{specifier}shop -h all' for a list of switches.");
								return;
							}
							// Finally, send the help data to the player
							helpLines.ForEach(l => args.Player.SendInfoMessage(l));
						}
						return;

					#endregion

					#region K (Purchase a kit)

					case "-c":
					case "-k":
					case "kit":
					case "kits":
						if (!args.Player.Group.HasPermission(Permissions.KitsBuy))
						{
							args.Player.SendErrorMessage("You do not have access to this command.");
							return;
						}

						string kitName = match.Groups["Object"].Value;
						if (String.IsNullOrWhiteSpace(kitName))
						{
							args.Player.SendErrorMessage($"{Tag} Invalid syntax! Proper syntax: {specifier}shop -k <kit name> [confirm:1]");
							return;
						}

						if (kitName.Equals("list", StringComparison.OrdinalIgnoreCase))
						{
							IEnumerable<string> dataToPaginate = PaginationTools.BuildLinesFromTerms(Manager.GetKits().Select(k => k.name));
							digit = 1;
							if (!String.IsNullOrWhiteSpace(match.Groups["Digit"].Value) && !Int32.TryParse(match.Groups["Digit"].Value, out digit))
							{
								args.Player.SendErrorMessage($"{Tag} Invalid page number!");
								return;
							}
							PaginationTools.SendPage(args.Player, digit, dataToPaginate.ToList(), new PaginationTools.Settings()
							{
								HeaderFormat = $"{Tag} Kits ({{0}}/{{1}}):",
								FooterFormat = $"{Tag} Type {specifier}shop -k list {{0}} for more.",
								NothingToDisplayString = $"{Tag} No kits available."
							});
							return;
						}

						List<Kit> kitMatches = await Manager.GetMatchingKits(kitName);

						if (kitMatches.Count != 1)
							args.Player.SendKitMatches(kitMatches);
						else
						{
							kit = kitMatches[0];
							if (String.IsNullOrWhiteSpace(match.Groups["Digit"].Value) || !Int32.TryParse(match.Groups["Digit"].Value, out digit) || digit < 1)
							{
								// Inform the player about the kit's contents
								args.Player.SendSuccessMessage($"{Tag} You have selected the kit '{kit.name}'.");
								args.Player.SendSuccessMessage($"{Tag} Kit's contents:");
								string prefix = "";
								foreach (SItem i in kit.items)
								{
									prefix = i.prefix > 0 ? i.GetPrefixName() + " " : "";
									args.Player.SendInfoMessage($"{Tag} - {prefix}{i.GetName()} x{i.stack}");
								}
								args.Player.SendInfoMessage($"{Tag} Type '{specifier}shop -k {kit.name} 1' to confirm your purchase.");
							}
							else if ((account = await BankMain.Bank.GetAsync(args.Player.User?.Name)) == null)
								args.Player.SendErrorMessage($"{Tag} You must have a bank account to use this command.");
							else if (account.Balance < kit.cost)
								args.Player.SendErrorMessage($"{Tag} You're {BankMain.FormatMoney(kit.cost - account.Balance)} short!");
							else if (args.Player.GetEmptyInventorySlots().Count < kit.items.Count)
								args.Player.SendErrorMessage($"{Tag} You must have {kit.items.Count} free inventory slots to purchase this kit.");
							else
							{
								try
								{
									await BankMain.Bank.ChangeByAsync(account.AccountName, -kit.cost);
									BankMain.Log.KitPurchase(account, kit);
									foreach (SItem i in kit.items)
									{
										if (CShop.Config.UseGiveItemSSC)
											args.Player.GiveItemSSC(i.netID, i.stack, i.prefix);
										else
											args.Player.GiveItem(i.netID, i.GetName(), 2, 3, i.stack, i.prefix);
									}
									args.Player.SendSuccessMessage($"{Tag} Bought the '{kit.name}' kit for {BankMain.FormatMoney(kit.cost)}.");
								}
								catch (NullReferenceException)
								{
									args.Player.SendErrorMessage($"{Tag} Invalid bank account!");
								}
								catch (InvalidOperationException)
								{
									args.Player.SendErrorMessage($"{Tag} Error performing transaction. Possible database corruption.");
									args.Player.SendInfoMessage($"{Tag} Double check if there are multiple accounts with the same ID.");
									args.Player.SendInfoMessage($"{Tag} You can try syncing the server with the database by using the reload command.");
								}
								catch (BankLogException ex)
								{
									TShock.Log.Error(ex.ToString());
								}
							}
						}
						return;

					#endregion

					#region R (Reload config / inventory)

					case "-r":
					case "reload":
						if (!args.Player.Group.HasPermission(Permissions.Reload))
						{
							args.Player.SendErrorMessage("You do not have access to this command.");
							return;
						}

						string reloadType = match.Groups["Object"].Value;
						try
						{
							if (String.IsNullOrWhiteSpace(reloadType) || reloadType.ToLower() == "all")
							{
								CShop.Config = Config.Read(Path.Combine(TShock.SavePath, "CurrencyBank", "Shop.json"));
								Manager.Reload();
								args.Player.SendSuccessMessage($"{Tag} Reloaded config and inventory!");
							}
							else if (reloadType.ToLower() == "config")
							{
								CShop.Config = Config.Read(Path.Combine(TShock.SavePath, "CurrencyBank", "Shop.json"));
								args.Player.SendSuccessMessage($"{Tag} Reloaded config!");
							}
							else if (reloadType.ToLower().StartsWith("inv"))
							{
								Manager.Reload();
								args.Player.SendSuccessMessage($"{Tag} Reloaded shop inventory!");
							}
							else
								args.Player.SendErrorMessage($"{Tag} Invalid reload type! Available types: config, inventory");
						}
						catch (Exception ex)
						{
							args.Player.SendErrorMessage($"{Tag} Something went wrong while reloading data. Check logs for details.");
							TShock.Log.Error(ex.ToString());
						}
						return;

					#endregion

					#region S (Search for items and categories)

					case "-s":
					case "search":
						if (!args.Player.Group.HasPermission(Permissions.Search))
						{
							args.Player.SendErrorMessage("You do not have access to this command.");
							return;
						}

						string searchString = match.Groups["Object"].Value;
						if (String.IsNullOrWhiteSpace(searchString))
						{
							args.Player.SendErrorMessage($"{Tag} Invalid syntax! Proper syntax: {specifier}shop -s [search term...]");
							return;
						}

						/*
						 * Group 1 - Whether the user is looking for an item (term)
						 * Group 2 - Whether the user is looking for a kit (term)
						 * Group 3 - The item/kit to look for (search)
						 */
						Regex searchRegex = new Regex(@"^(?:(?<IsItem>i(?:tem)?:)|(?<IsKit>k(?:it)?:))? *(?<Object>.+)$");
						Match searchMatch = searchRegex.Match(searchString);
						bool itemOnly = !String.IsNullOrWhiteSpace(searchMatch.Groups["IsItem"].Value);
						bool kitOnly = !String.IsNullOrWhiteSpace(searchMatch.Groups["IsKit"].Value);
						string searchName = searchMatch.Groups["Object"].Value;
						if (itemOnly)
						{
							// List all items on sale starting with the search string
							args.Player.SendInfoMessage($"{Tag} Searching for all items starting with '{searchName}'...");
							try
							{
								args.Player.SendItemMatches(await Manager.GetMatchingItems(searchName));
							}
							catch (InvalidItemException)
							{
								args.Player.SendSuccessMessage($"{Tag} No items matched.");
							}
						}
						else if (kitOnly)
						{
							// List all kits on sale containing an item
							List<Item> items = TShock.Utils.GetItemByName(searchName);
							if (items.Count == 1)
							{
								// Display all kits containing this item
								args.Player.SendInfoMessage($"{Tag} Searching for all kits containing the item '{items[0].name}'...");
								args.Player.SendKitMatches(await Manager.KitContains(items[0].netID));
							}
							else
							{
								// List all kits on sale starting with the search string
								args.Player.SendInfoMessage($"{Tag} Searching for all kits starting with '{searchName}'...");
								args.Player.SendKitMatches(await Manager.GetMatchingKits(searchName));
							}
						}
						else
						{
							// List everything on sale starting with the search string
							args.Player.SendInfoMessage($"{Tag} Searching for anything in stock starting with '{searchName}'...");
							try
							{
								args.Player.SendItemMatches(await Manager.GetMatchingItems(searchName));
							}
							catch (InvalidItemException)
							{
								args.Player.SendSuccessMessage($"{Tag} No items matched.");
							}
							List<Item> items = TShock.Utils.GetItemByName(searchName);
							if (items.Count == 1)
								args.Player.SendKitMatches(await Manager.KitContains(items[0].netID));
							else
								args.Player.SendKitMatches(await Manager.GetMatchingKits(searchName));
						}
						return;

					#endregion

					default:
						args.Player.SendErrorMessage($"{Tag} Invalid switch!");
						args.Player.SendErrorMessage($"{Tag} Valid switches: -b (buy) -k (buy kits) -h [cmd] (help) -s (search) -r (reload)");
						return;
				}
			}
		}
	}
}
