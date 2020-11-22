using System;
using Gtk;
using System.Collections.Generic;

namespace Pinta.Core
{
	public class EffectsActions
	{
		private Menu effects_menu;
		private Dictionary<Gtk.Action, MenuItem> menu_items;

		public Dictionary<string, Gtk.Menu> Menus { get; private set; }
		public List<Gtk.Action> Actions { get; private set; }

		public EffectsActions ()
		{
			Actions = new List<Gtk.Action> ();
			Menus = new Dictionary<string,Menu> ();
			menu_items = new Dictionary<Gtk.Action, MenuItem> ();
		}
		
		#region Initialization
		public void CreateMainMenu (Gtk.Menu menu)
		{
			effects_menu = menu;
		}

		public void AddEffect (string category, Gtk.Action action)
		{
			if (!Menus.ContainsKey (category)) {
				Gtk.Action menu_action = new Gtk.Action (category, Mono.Unix.Catalog.GetString (category), null, null);
				Menu category_menu = (Menu)effects_menu.AppendMenuItemSorted ((MenuItem)(menu_action.CreateSubMenuItem ())).Submenu;
				
				Menus.Add (category, category_menu);
			}
			
			Actions.Add (action);
			var menu_item = (MenuItem)action.CreateMenuItem ();

			Menu m = Menus[category];
			m.AppendMenuItemSorted (menu_item);

			menu_items.Add (action, menu_item);
		}

		// TODO: Remove menu category if empty
		internal void RemoveEffect (string category, Gtk.Action action)
		{
			if (!Menus.ContainsKey (category))
				return;
			if (!menu_items.ContainsKey (action))
				return;

			var menu = Menus[category];
			menu.Remove (menu_items[action]);
		}
		#endregion

		#region Public Methods
		public void ToggleActionsSensitive (bool sensitive)
		{
			foreach (Gtk.Action a in Actions)
				a.Sensitive = sensitive;
		}
		#endregion
	}
}
