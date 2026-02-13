using System.Collections;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;

namespace Platform.Maui.Linux.Gtk4.Handlers;

/// <summary>
/// Basic CollectionView handler using Gtk.ListView and GTK selection models.
/// Supports ItemsSource, selection mode/selection state, EmptyView and scrollbar visibility.
/// </summary>
public class CollectionViewHandler : GtkViewHandler<IView, Gtk.ScrolledWindow>
{
	Gtk.ListView? _listView;
	Gtk.StringList? _model;
	Gtk.SelectionModel? _selectionModel;
	Gtk.Label? _emptyLabel;
	readonly List<object?> _items = [];
	bool _updatingSelection;

	public static new IPropertyMapper<IView, CollectionViewHandler> Mapper =
		new PropertyMapper<IView, CollectionViewHandler>(ViewHandler.ViewMapper)
		{
			["ItemsSource"] = MapItemsSource,
			["SelectionMode"] = MapSelectionMode,
			["SelectedItem"] = MapSelectedItem,
			["SelectedItems"] = MapSelectedItems,
			["EmptyView"] = MapEmptyView,
			["HorizontalScrollBarVisibility"] = MapHorizontalScrollBarVisibility,
			["VerticalScrollBarVisibility"] = MapVerticalScrollBarVisibility,
		};

	public CollectionViewHandler() : base(Mapper) { }

	protected override Gtk.ScrolledWindow CreatePlatformView()
	{
		var scrolled = Gtk.ScrolledWindow.New();
		scrolled.SetVexpand(true);
		scrolled.SetHexpand(true);
		scrolled.SetPolicy(Gtk.PolicyType.Automatic, Gtk.PolicyType.Automatic);

		_model = Gtk.StringList.New(null);

		var factory = Gtk.SignalListItemFactory.New();
		factory.OnSetup += (_, args) =>
		{
			var listItem = (Gtk.ListItem)args.Object;
			var label = Gtk.Label.New(string.Empty);
			label.SetHalign(Gtk.Align.Start);
			label.SetXalign(0f);
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

		_selectionModel = Gtk.NoSelection.New(_model);
		_listView = Gtk.ListView.New(_selectionModel, factory);
		scrolled.SetChild(_listView);

		return scrolled;
	}

	protected override void ConnectHandler(Gtk.ScrolledWindow platformView)
	{
		base.ConnectHandler(platformView);
		HookSelectionChanged();
	}

	void HookSelectionChanged()
	{
		if (_selectionModel == null)
			return;

		_selectionModel.OnSelectionChanged += OnSelectionChanged;
	}

	void UnhookSelectionChanged()
	{
		if (_selectionModel == null)
			return;

		_selectionModel.OnSelectionChanged -= OnSelectionChanged;
	}

	void OnSelectionChanged(Gtk.SelectionModel sender, Gtk.SelectionModel.SelectionChangedSignalArgs args)
	{
		if (_updatingSelection || VirtualView is not CollectionView collectionView)
			return;

		try
		{
			_updatingSelection = true;
			switch (collectionView.SelectionMode)
			{
				case SelectionMode.None:
					collectionView.SelectedItem = null;
					collectionView.SelectedItems = [];
					break;
				case SelectionMode.Single:
					if (_selectionModel is Gtk.SingleSelection single)
					{
						var idx = (int)single.GetSelected();
						collectionView.SelectedItem = idx >= 0 && idx < _items.Count ? _items[idx] : null;
						collectionView.SelectedItems = collectionView.SelectedItem != null
							? [collectionView.SelectedItem]
							: [];
					}
					break;
				case SelectionMode.Multiple:
					if (_selectionModel is Gtk.MultiSelection multi)
					{
						var selected = new List<object>();
						var set = multi.GetSelection();
						var count = (uint)set.GetSize();
						for (uint i = 0; i < count; i++)
						{
							var idx = (int)set.GetNth(i);
							if (idx >= 0 && idx < _items.Count && _items[idx] != null)
								selected.Add(_items[idx]!);
						}

						collectionView.SelectedItems = selected;
						collectionView.SelectedItem = selected.Count > 0 ? selected[0] : null;
					}
					break;
			}
		}
		finally
		{
			_updatingSelection = false;
		}
	}

	public static void MapItemsSource(CollectionViewHandler handler, IView view)
	{
		if (view is not CollectionView collectionView || handler._model == null)
			return;

		handler._items.Clear();
		while (handler._model.GetNItems() > 0)
			handler._model.Remove(0);

		if (collectionView.ItemsSource != null)
		{
			foreach (var item in collectionView.ItemsSource)
			{
				handler._items.Add(item);
				handler._model.Append(item?.ToString() ?? string.Empty);
			}
		}

		handler.UpdateDisplayedChild(collectionView);
		MapSelectedItem(handler, view);
	}

	public static void MapSelectionMode(CollectionViewHandler handler, IView view)
	{
		if (view is not CollectionView collectionView || handler._model == null || handler._listView == null)
			return;

		handler.UnhookSelectionChanged();

		handler._selectionModel = collectionView.SelectionMode switch
		{
			SelectionMode.None => Gtk.NoSelection.New(handler._model),
			SelectionMode.Single => Gtk.SingleSelection.New(handler._model),
			SelectionMode.Multiple => Gtk.MultiSelection.New(handler._model),
			_ => Gtk.SingleSelection.New(handler._model),
		};

		handler._listView.SetModel(handler._selectionModel);
		handler.HookSelectionChanged();
		MapSelectedItem(handler, view);
	}

	public static void MapSelectedItem(CollectionViewHandler handler, IView view)
	{
		if (view is not CollectionView collectionView || handler._selectionModel == null)
			return;

		try
		{
			handler._updatingSelection = true;
			if (collectionView.SelectionMode == SelectionMode.None)
			{
				handler._selectionModel.UnselectAll();
				return;
			}

			if (collectionView.SelectionMode == SelectionMode.Single && handler._selectionModel is Gtk.SingleSelection single)
			{
				var selectedIndex = collectionView.SelectedItem != null
					? handler._items.IndexOf(collectionView.SelectedItem)
					: -1;
				single.SetSelected(selectedIndex >= 0 ? (uint)selectedIndex : uint.MaxValue);
			}

			if (collectionView.SelectionMode == SelectionMode.Multiple && handler._selectionModel is Gtk.MultiSelection multi)
			{
				multi.UnselectAll();
				var selectedItems = collectionView.SelectedItems;
				if (selectedItems == null)
					return;

				foreach (var selected in selectedItems)
				{
					var idx = handler._items.IndexOf(selected);
					if (idx >= 0)
						multi.SelectItem((uint)idx, true);
				}
			}
		}
		finally
		{
			handler._updatingSelection = false;
		}
	}

	public static void MapSelectedItems(CollectionViewHandler handler, IView view)
	{
		MapSelectedItem(handler, view);
	}

	public static void MapEmptyView(CollectionViewHandler handler, IView view)
	{
		if (view is CollectionView collectionView)
			handler.UpdateDisplayedChild(collectionView);
	}

	public static void MapHorizontalScrollBarVisibility(CollectionViewHandler handler, IView view)
	{
		if (view is not CollectionView collectionView || handler.PlatformView == null)
			return;

		var h = collectionView.HorizontalScrollBarVisibility switch
		{
			ScrollBarVisibility.Always => Gtk.PolicyType.Always,
			ScrollBarVisibility.Never => Gtk.PolicyType.Never,
			_ => Gtk.PolicyType.Automatic,
		};
		var v = collectionView.VerticalScrollBarVisibility switch
		{
			ScrollBarVisibility.Always => Gtk.PolicyType.Always,
			ScrollBarVisibility.Never => Gtk.PolicyType.Never,
			_ => Gtk.PolicyType.Automatic,
		};
		handler.PlatformView.SetPolicy(h, v);
	}

	public static void MapVerticalScrollBarVisibility(CollectionViewHandler handler, IView view)
	{
		if (view is not CollectionView collectionView || handler.PlatformView == null)
			return;

		var h = collectionView.HorizontalScrollBarVisibility switch
		{
			ScrollBarVisibility.Always => Gtk.PolicyType.Always,
			ScrollBarVisibility.Never => Gtk.PolicyType.Never,
			_ => Gtk.PolicyType.Automatic,
		};
		var v = collectionView.VerticalScrollBarVisibility switch
		{
			ScrollBarVisibility.Always => Gtk.PolicyType.Always,
			ScrollBarVisibility.Never => Gtk.PolicyType.Never,
			_ => Gtk.PolicyType.Automatic,
		};
		handler.PlatformView.SetPolicy(h, v);
	}

	void UpdateDisplayedChild(CollectionView collectionView)
	{
		if (PlatformView == null || _listView == null)
			return;

		var hasItems = _items.Count > 0;
		if (hasItems || collectionView.EmptyView == null)
		{
			if (PlatformView.GetChild() != _listView)
				PlatformView.SetChild(_listView);
			return;
		}

		_emptyLabel ??= Gtk.Label.New(string.Empty);
		_emptyLabel.SetHalign(Gtk.Align.Center);
		_emptyLabel.SetValign(Gtk.Align.Center);
		_emptyLabel.SetWrap(true);
		_emptyLabel.SetJustify(Gtk.Justification.Center);
		_emptyLabel.SetText(collectionView.EmptyView?.ToString() ?? string.Empty);
		if (PlatformView.GetChild() != _emptyLabel)
			PlatformView.SetChild(_emptyLabel);
	}
}
