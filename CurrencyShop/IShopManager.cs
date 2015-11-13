using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyShop
{
	public interface IShopManager
	{
		List<SItem> GetItems();

		List<Kit> GetKits();

		/// <summary>
		/// Returns all items which name starts with the input value.
		/// </summary>
		/// <param name="s">The input value to match.</param>
		/// <returns>The list of items which name starts with value.</returns>
		Task<List<SItem>> GetMatchingItems(string s);

		/// <summary>
		/// Returns all kits which name starts with the input value.
		/// </summary>
		/// <param name="s">The input value to match.</param>
		/// <returns>The list of kits which name starts with value.</returns>
		Task<List<Kit>> GetMatchingKits(string s);

		/// <summary>
		/// Returns all kits containing an item with a matching netID.
		/// </summary>
		/// <param name="id">The netID to look for.</param>
		/// <returns>The list of kits containing the item.</returns>
		Task<List<Kit>> KitContains(int id);

		/// <summary>
		/// Reloads the manager to take into account any outside changes.
		/// </summary>
		void Reload();
	}
}
