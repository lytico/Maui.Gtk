using System.Collections;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace Maui.Linux.Handlers;

/// <summary>
/// CollectionView handler using Gtk.ListView with virtualized scrolling.
/// Binds MAUI ItemsSource to a Gtk.StringList model.
/// </summary>
public class CollectionViewHandler : GtkViewHandler<IView, Gtk.ScrolledWindow>
{
	private Gtk.ListView? _listView;
	private Gtk.StringList? _model;
	private Gtk.SingleSelection? _selectionModel;
	private Action<int>? _selectionChanged;

	public static new IPropertyMapper<IView, CollectionViewHandler> Mapper =
		new PropertyMapper<IView, CollectionViewHandler>(ViewHandler.ViewMapper);

	public CollectionViewHandler() : base(Mapper) { }

	protected override Gtk.ScrolledWindow CreatePlatformView()
	{
		var scrolled = Gtk.ScrolledWindow.New();
		scrolled.SetVexpand(true);
		scrolled.SetHexpand(true);

		_model = Gtk.StringList.New(null);

		var factory = Gtk.SignalListItemFactory.New();
		factory.OnSetup += (_, args) =>
		{
			var listItem = (Gtk.ListItem)args.Object;
			var label = Gtk.Label.New("");
			label.SetHalign(Gtk.Align.Start);
			label.SetMarginStart(12);
			label.SetMarginEnd(12);
			label.SetMarginTop(8);
			label.SetMarginBottom(8);
			listItem.SetChild(label);
		};
		factory.OnBind += (_, args) =>
		{
			var listItem = (Gtk.ListItem)args.Object;
			var label = (Gtk.Label)listItem.GetChild()!;
			var item = (Gtk.StringObject)listItem.GetItem()!;
			label.SetText(item.GetString());
		};

		_selectionModel = Gtk.SingleSelection.New(_model);
		_listView = Gtk.ListView.New(_selectionModel, factory);
		scrolled.SetChild(_listView);

		return scrolled;
	}

	/// <summary>
	/// Sets the items to display. Call from the page code since MAUI's
	/// IView doesn't expose ItemsSource directly for custom backends.
	/// </summary>
	public void SetItems(IEnumerable items)
	{
		if (_model == null) return;

		// Clear existing
		while (_model.GetNItems() > 0)
			_model.Remove(0);

		// Add new items
		foreach (var item in items)
			_model.Append(item?.ToString() ?? "");
	}

	/// <summary>
	/// Register a callback for selection changes.
	/// </summary>
	public void SetSelectionChanged(Action<int> callback)
	{
		_selectionChanged = callback;
		if (_selectionModel != null)
		{
			_selectionModel.OnSelectionChanged += (_, args) =>
			{
				_selectionChanged?.Invoke((int)_selectionModel.GetSelected());
			};
		}
	}
}
