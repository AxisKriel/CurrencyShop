using CurrencyBank;
using CurrencyBank.DB;

namespace CurrencyShop.Extensions
{
	public static class BankLogExtensions
	{
		private static void purchase(BankLog log, BankAccount account, string item, int cost)
		{
			log.Write($"{account.AccountName} bought {item} for {BankMain.FormatMoney(cost)}");
		}

		public static void ItemPurchase(this BankLog log, BankAccount account, SItem item)
		{
			purchase(log, account, $"{item.stack} {(item.prefix > 0 ? item.GetPrefixName() + " " : "")}{item.GetName()}", item.cost);
		}

		public static void KitPurchase(this BankLog log, BankAccount account, Kit kit)
		{
			purchase(log, account, "the kit '" + kit.name + "'", kit.cost);
		}
	}
}
