using System.Collections;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using Platform.Maui.Linux.Gtk4.Platform;

namespace Platform.Maui.Linux.Gtk4.Handlers;

/// <summary>
/// Basic CollectionView handler using Gtk.ListView and GTK selection models.
/// Supports ItemsSource, selection mode/selection state, EmptyView, Header/Footer,
/// scrollbar visibility, ItemTemplate, and basic ItemsLayout orientation.
/// </summary>
public class CollectionViewHandler : GtkViewHandler<IView, Gtk.ScrolledWindow>
{
	Gtk.ListView? _listView;
	Gtk.StringList? _model;
	Gtk.SelectionModel? _selectionModel;
	Gtk.Label? _emptyLabel;
	Gtk.Box? _outerBox;
	Gtk.Label? _headerLabel;
	Gtk.Label? _footerLabel;
	readonly List<object?> _items = [];
	readonly List<Gtk.Widget> _templateWidgets = [];
	bool _updatingSelection;

	public static new IPropertyMapper<IView, CollectionViewHandler> Mapper =
		new PropertyMapper<IView, CollectionViewHandler>(ViewHandler.ViewMapper)
		{
			["ItemsSource"] = MapItemsSource,
			["SelectionMode"] = MapSelectionMode,
			["SelectedItem"] = MapSelectedItem,
			["SelectedItems"] = MapSelectedItems,
			["EmptyView"] = MapEmptyView,
			["EmptyViewTemplate"] = MapEmptyView,
			["Header"] = MapHeader,
			["HeaderTemplate"] = MapHeader,
			["Footer"] = MapFooter,
			["FooterTemplate"] = MapFooter,
			["ItemsLayout"] = MapItemsLayout,
			["ItemTemplate"] = MapItemTemplate,
			["ItemSizingStrategy"] = MapItemSizingStrategy,
			["ItemsUpdatingScrollMode"] = MapItemsUpdatingScrollMode,
			["IsGrouped"] = MapIsGrouped,
			["CanReorderItems"] = MapCanReorderItems,
			["HorizontalScrollBarVisibility"] = MapScrollBarVisibility,
			["VerticalScrollBarVisibility"] = MapScrollBarVisibility,
			[nameof(IView.Background)] = MapBackgroundColor,
			["BackgroundColor"] = MapBackgroundColor,
			// Accessibility properties — registered as no-ops to prevent warnings
			["IsInAccessibleTree"] = MapAccessibility,
			["Description"] = MapAccessibility,
			["HeadingLevel"] = MapAccessibility,
			["Hint"] = MapAccessibility,
			["ExcludedWithChildren"] = MapAccessibility,
		};

	public CollectionViewHandler() : base(Mapper) { }

	protected override Gtk.ScrolledWindow CreatePlatformView()
	{
		var scrolled = Gtk.ScrolledWindow.New();
		scrolled.SetVexpand(true);
		scrolled.SetHexpand(true);
		scrolled.SetPolicy(Gtk.PolicyType.Automatic, Gtk.PolicyType.Automatic);

		_model = Gtk.StringList.New(null);

		RebuildListView();

		// Outer box to hold header, list, and footer
		_outerBox = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
		_outerBox.SetVexpand(true);
		_headerLabel = Gtk.Label.New(string.Empty);
		_headerLabel.SetVisible(false);
		_headerLabel.SetHalign(Gtk.Align.Start);
		_headerLabel.SetMarginStart(12);
		_headerLabel.SetMarginTop(8);
		_headerLabel.SetMarginBottom(4);

		_footerLabel = Gtk.Label.New(string.Empty);
		_footerLabel.SetVisible(false);
		_footerLabel.SetHalign(Gtk.Align.Start);
		_footerLabel.SetMarginStart(12);
		_footerLabel.SetMarginTop(4);
		_footerLabel.SetMarginBottom(8);

		_outerBox.Append(_headerLabel);
		_outerBox.Append(_listView!);
		_outerBox.Append(_footerLabel);
		scrolled.SetChild(_outerBox);

		return scrolled;
	}

	void RebuildListView()
	{
		_templateWidgets.Clear();
		var hasTemplate = VirtualView is CollectionView cv && cv.ItemTemplate != null;
		Gtk.ListItemFactory factory;

		if (hasTemplate)
			factory = BuildTemplateFactory();
		else
			factory = BuildStringFactory();

		if (_selectionModel == null)
			_selectionModel = Gtk.NoSelection.New(_model);

		var newListView = Gtk.ListView.New(_selectionModel, factory);
		if (!hasTemplate)
			newListView.AddCssClass("navigation-sidebar");
		newListView.SetVexpand(true);

		if (_listView != null && _outerBox != null)
		{
			_outerBox.InsertChildAfter(newListView, _listView);
			_outerBox.Remove(_listView);
		}

		_listView = newListView;
	}

	Gtk.SignalListItemFactory BuildStringFactory()
	{
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
		return factory;
	}

	Gtk.SignalListItemFactory BuildTemplateFactory()
	{
		var factory = Gtk.SignalListItemFactory.New();
		factory.OnSetup += (_, args) =>
		{
			var listItem = (Gtk.ListItem)args.Object;
			// Use a Gtk.Box as container — it respects SetSizeRequest for
			// natural height, unlike Gtk.Fixed which always reports 0.
			var box = Gtk.Box.New(Gtk.Orientation.Vertical, 0);
			box.SetHexpand(true);
			box.SetVexpand(false);
			_templateWidgets.Add(box);
			listItem.SetChild(box);
		};
		factory.OnBind += (_, args) =>
		{
			var listItem = (Gtk.ListItem)args.Object;
			var box = (Gtk.Box)listItem.GetChild()!;

			// Remove previous children
			while (box.GetFirstChild() is Gtk.Widget child)
				box.Remove(child);

			var idx = (int)listItem.GetPosition();
			if (idx < 0 || idx >= _items.Count) return;
			var dataItem = _items[idx];

			if (VirtualView is not CollectionView collectionView || collectionView.ItemTemplate == null)
				return;

			try
			{
				// Create MAUI view from DataTemplate
				var template = collectionView.ItemTemplate;
				var content = template is DataTemplateSelector selector
					? selector.SelectTemplate(dataItem, collectionView)?.CreateContent()
					: template.CreateContent();

				if (content is not View mauiView)
					return;

				mauiView.BindingContext = dataItem;

				// Measure the MAUI view to determine row height
				var widthConstraint = _listView?.GetAllocatedWidth() ?? 400;
				if (widthConstraint <= 0) widthConstraint = 400;

				// Build native GTK widgets from the MAUI template view
				var nativeWidget = BuildNativeFromTemplate(mauiView, widthConstraint);
				_templateWidgets.Add(nativeWidget.widget);
				box.SetSizeRequest(-1, nativeWidget.height);
				box.Append(nativeWidget.widget);
			}
			catch (Exception ex)
			{
				var fallback = Gtk.Label.New(dataItem?.ToString() ?? "(error)");
				fallback.SetHalign(Gtk.Align.Start);
				fallback.SetMarginStart(12);
				box.Append(fallback);
				Console.WriteLine($"[CollectionView] ItemTemplate error: {ex.Message}");
			}
		};
		return factory;
	}

	/// <summary>
	/// Renders a MAUI view template as native GTK widgets for use in ListView rows.
	/// This bypasses the MAUI handler tree to avoid GtkLayoutPanel/Gtk.Fixed zero-height issues.
	/// </summary>
	(Gtk.Widget widget, int height) BuildNativeFromTemplate(View mauiView, double widthConstraint)
	{
		// For horizontal layouts (StackLayout/HorizontalStackLayout), create a Gtk.Box
		if (mauiView is StackLayout { Orientation: StackOrientation.Horizontal } hStack)
			return BuildHorizontalStack(hStack, widthConstraint);
		if (mauiView is HorizontalStackLayout hsl)
			return BuildHorizontalStackFromLayout(hsl, widthConstraint);

		// For vertical layouts
		if (mauiView is StackLayout { Orientation: StackOrientation.Vertical } vStack)
			return BuildVerticalStack(vStack, widthConstraint);
		if (mauiView is VerticalStackLayout vsl)
			return BuildVerticalStackFromLayout(vsl, widthConstraint);

		// For Grid layout — flatten children into a Box
		if (mauiView is Grid grid)
			return BuildFromGrid(grid, widthConstraint);

		// Single element
		return BuildSingleNativeWidget(mauiView);
	}

	(Gtk.Widget widget, int height) BuildHorizontalStack(StackLayout stack, double widthConstraint)
	{
		var box = Gtk.Box.New(Gtk.Orientation.Horizontal, (int)stack.Spacing);
		box.SetHexpand(true);
		ApplyPadding(box, stack.Padding);
		int maxHeight = 0;
		foreach (var child in stack.Children)
		{
			if (child is View childView)
			{
				var (w, h) = BuildSingleNativeWidget(childView);
				box.Append(w);
				maxHeight = Math.Max(maxHeight, h);
			}
		}
		maxHeight += (int)(stack.Padding.VerticalThickness);
		return (box, Math.Max(maxHeight, 40));
	}

	(Gtk.Widget widget, int height) BuildHorizontalStackFromLayout(HorizontalStackLayout stack, double widthConstraint)
	{
		var box = Gtk.Box.New(Gtk.Orientation.Horizontal, (int)stack.Spacing);
		box.SetHexpand(true);
		ApplyPadding(box, stack.Padding);
		int maxHeight = 0;
		foreach (var child in stack.Children)
		{
			if (child is View childView)
			{
				var (w, h) = BuildSingleNativeWidget(childView);
				box.Append(w);
				maxHeight = Math.Max(maxHeight, h);
			}
		}
		maxHeight += (int)(stack.Padding.VerticalThickness);
		return (box, Math.Max(maxHeight, 40));
	}

	(Gtk.Widget widget, int height) BuildVerticalStack(StackLayout stack, double widthConstraint)
	{
		var box = Gtk.Box.New(Gtk.Orientation.Vertical, (int)stack.Spacing);
		box.SetHexpand(true);
		ApplyPadding(box, stack.Padding);
		int totalHeight = 0;
		foreach (var child in stack.Children)
		{
			if (child is View childView)
			{
				var (w, h) = BuildSingleNativeWidget(childView);
				box.Append(w);
				totalHeight += h + (int)stack.Spacing;
			}
		}
		totalHeight += (int)(stack.Padding.VerticalThickness);
		return (box, Math.Max(totalHeight, 20));
	}

	(Gtk.Widget widget, int height) BuildVerticalStackFromLayout(VerticalStackLayout stack, double widthConstraint)
	{
		var box = Gtk.Box.New(Gtk.Orientation.Vertical, (int)stack.Spacing);
		box.SetHexpand(true);
		ApplyPadding(box, stack.Padding);
		int totalHeight = 0;
		foreach (var child in stack.Children)
		{
			if (child is View childView)
			{
				var (w, h) = BuildSingleNativeWidget(childView);
				box.Append(w);
				totalHeight += h + (int)stack.Spacing;
			}
		}
		totalHeight += (int)(stack.Padding.VerticalThickness);
		return (box, Math.Max(totalHeight, 20));
	}

	(Gtk.Widget widget, int height) BuildFromGrid(Grid grid, double widthConstraint)
	{
		// Simplified: put all grid children in a horizontal box
		var box = Gtk.Box.New(Gtk.Orientation.Horizontal, 8);
		box.SetHexpand(true);
		ApplyPadding(box, grid.Padding);
		int maxHeight = 0;
		foreach (var child in grid.Children)
		{
			if (child is View childView)
			{
				var (w, h) = BuildSingleNativeWidget(childView);
				box.Append(w);
				maxHeight = Math.Max(maxHeight, h);
			}
		}
		maxHeight += (int)(grid.Padding.VerticalThickness);
		return (box, Math.Max(maxHeight, 40));
	}

	(Gtk.Widget widget, int height) BuildSingleNativeWidget(View mauiView)
	{
		// Recurse for layout containers
		if (mauiView is StackLayout sl)
			return sl.Orientation == StackOrientation.Horizontal
				? BuildHorizontalStack(sl, 400)
				: BuildVerticalStack(sl, 400);
		if (mauiView is HorizontalStackLayout hsl)
			return BuildHorizontalStackFromLayout(hsl, 400);
		if (mauiView is VerticalStackLayout vsl)
			return BuildVerticalStackFromLayout(vsl, 400);
		if (mauiView is Grid grid)
			return BuildFromGrid(grid, 400);

		if (mauiView is Label label)
		{
			var gtkLabel = Gtk.Label.New(label.Text ?? "");
			gtkLabel.SetHalign(Gtk.Align.Start);
			gtkLabel.SetValign(Gtk.Align.Center);

			if (label.FontSize > 0 && label.FontSize != 14)
			{
				var cssProvider = Gtk.CssProvider.New();
				cssProvider.LoadFromString($"label {{ font-size: {(int)label.FontSize}px; }}");
				gtkLabel.GetStyleContext().AddProvider(cssProvider, Gtk.Constants.STYLE_PROVIDER_PRIORITY_APPLICATION);
			}
			if (label.TextColor != null)
			{
				var c = label.TextColor;
				var cssProvider = Gtk.CssProvider.New();
				cssProvider.LoadFromString($"label {{ color: rgba({(int)(c.Red*255)},{(int)(c.Green*255)},{(int)(c.Blue*255)},{c.Alpha}); }}");
				gtkLabel.GetStyleContext().AddProvider(cssProvider, Gtk.Constants.STYLE_PROVIDER_PRIORITY_APPLICATION);
			}
			if (label.FontAttributes.HasFlag(FontAttributes.Bold))
			{
				var cssProvider = Gtk.CssProvider.New();
				cssProvider.LoadFromString("label { font-weight: bold; }");
				gtkLabel.GetStyleContext().AddProvider(cssProvider, Gtk.Constants.STYLE_PROVIDER_PRIORITY_APPLICATION);
			}

			// Evaluate bindings to resolve text from BindingContext
			if (string.IsNullOrEmpty(label.Text) && mauiView.BindingContext != null)
			{
				// Trigger binding evaluation
				mauiView.BindingContext = mauiView.BindingContext;
				gtkLabel.SetLabel(label.Text ?? "");
			}

			ApplyMargin(gtkLabel, label.Margin);
			int h = label.FontSize > 20 ? (int)label.FontSize + 8 : 22;
			return (gtkLabel, h);
		}

		if (mauiView is BoxView boxView)
		{
			var area = Gtk.DrawingArea.New();
			var w = (int)(boxView.WidthRequest > 0 ? boxView.WidthRequest : 40);
			var h = (int)(boxView.HeightRequest > 0 ? boxView.HeightRequest : 40);
			area.SetContentWidth(w);
			area.SetContentHeight(h);
			var color = boxView.Color ?? (boxView.Background is SolidColorBrush scb ? scb.Color : null);
			if (color != null)
			{
				var c = color;
				area.SetDrawFunc((_, cr, width, height) =>
				{
					var cornerRadius = (float)boxView.CornerRadius.TopLeft;
					if (cornerRadius > 0)
					{
						DrawRoundedRect(cr, 0, 0, width, height, cornerRadius);
						Cairo.Internal.Context.SetSourceRgba(cr.Handle, c.Red, c.Green, c.Blue, c.Alpha);
						Cairo.Internal.Context.Fill(cr.Handle);
					}
					else
					{
						Cairo.Internal.Context.SetSourceRgba(cr.Handle, c.Red, c.Green, c.Blue, c.Alpha);
						Cairo.Internal.Context.Rectangle(cr.Handle, 0, 0, width, height);
						Cairo.Internal.Context.Fill(cr.Handle);
					}
				});
			}
			ApplyMargin(area, boxView.Margin);
			return (area, h);
		}

		if (mauiView is Image image)
		{
			var gtkImage = Gtk.Image.New();
			if (image.Source is FontImageSource fontSrc)
			{
				gtkImage.SetFromIconName(fontSrc.Glyph ?? "image-missing");
				var size = (int)(fontSrc.Size > 0 ? fontSrc.Size : 24);
				gtkImage.SetPixelSize(size);
			}
			var imgSize = (int)(image.HeightRequest > 0 ? image.HeightRequest : 24);
			ApplyMargin(gtkImage, image.Margin);
			return (gtkImage, imgSize);
		}

		// Fallback: create label with ToString
		var fallback = Gtk.Label.New(mauiView.GetType().Name);
		fallback.SetHalign(Gtk.Align.Start);
		return (fallback, 22);
	}

	static void DrawRoundedRect(Cairo.Context cr, double x, double y, double w, double h, float r)
	{
		Cairo.Internal.Context.NewSubPath(cr.Handle);
		Cairo.Internal.Context.Arc(cr.Handle, x + w - r, y + r, r, -Math.PI / 2, 0);
		Cairo.Internal.Context.Arc(cr.Handle, x + w - r, y + h - r, r, 0, Math.PI / 2);
		Cairo.Internal.Context.Arc(cr.Handle, x + r, y + h - r, r, Math.PI / 2, Math.PI);
		Cairo.Internal.Context.Arc(cr.Handle, x + r, y + r, r, Math.PI, 3 * Math.PI / 2);
		Cairo.Internal.Context.ClosePath(cr.Handle);
	}

	static void ApplyPadding(Gtk.Widget widget, Thickness padding)
	{
		if (padding.Left > 0) widget.SetMarginStart((int)padding.Left);
		if (padding.Right > 0) widget.SetMarginEnd((int)padding.Right);
		if (padding.Top > 0) widget.SetMarginTop((int)padding.Top);
		if (padding.Bottom > 0) widget.SetMarginBottom((int)padding.Bottom);
	}

	static void ApplyMargin(Gtk.Widget widget, Thickness margin)
	{
		if (margin.Left > 0) widget.SetMarginStart((int)margin.Left);
		if (margin.Right > 0) widget.SetMarginEnd((int)margin.Right);
		if (margin.Top > 0) widget.SetMarginTop((int)margin.Top);
		if (margin.Bottom > 0) widget.SetMarginBottom((int)margin.Bottom);
	}

	protected override void ConnectHandler(Gtk.ScrolledWindow platformView)
	{
		base.ConnectHandler(platformView);
		HookSelectionChanged();
	}

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
	{
		var size = base.GetDesiredSize(widthConstraint, heightConstraint);

		// Scrollable views should fill available space rather than reporting
		// their natural (often zero) height, so the MAUI layout allocates
		// the full area to them.
		if (VirtualView is Microsoft.Maui.Controls.View v &&
			v.VerticalOptions.Alignment == Microsoft.Maui.Controls.LayoutAlignment.Fill &&
			size.Height < heightConstraint &&
			!double.IsInfinity(heightConstraint))
		{
			size = new Size(size.Width, heightConstraint);
		}

		return size;
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

	public static void MapScrollBarVisibility(CollectionViewHandler handler, IView view)
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

	public static void MapHeader(CollectionViewHandler handler, IView view)
	{
		if (view is not CollectionView collectionView || handler._headerLabel == null)
			return;

		var headerText = collectionView.Header?.ToString();
		if (!string.IsNullOrEmpty(headerText))
		{
			handler._headerLabel.SetText(headerText);
			handler._headerLabel.SetVisible(true);
		}
		else
		{
			handler._headerLabel.SetVisible(false);
		}
	}

	public static void MapFooter(CollectionViewHandler handler, IView view)
	{
		if (view is not CollectionView collectionView || handler._footerLabel == null)
			return;

		var footerText = collectionView.Footer?.ToString();
		if (!string.IsNullOrEmpty(footerText))
		{
			handler._footerLabel.SetText(footerText);
			handler._footerLabel.SetVisible(true);
		}
		else
		{
			handler._footerLabel.SetVisible(false);
		}
	}

	public static void MapItemsLayout(CollectionViewHandler handler, IView view)
	{
		if (view is not CollectionView collectionView || handler._listView == null)
			return;

		// GTK ListView is always vertical; orientation awareness is a best-effort hint.
		if (collectionView.ItemsLayout is LinearItemsLayout linear &&
			linear.Orientation == ItemsLayoutOrientation.Horizontal)
		{
			handler._listView.SetOrientation(Gtk.Orientation.Horizontal);
		}
		else
		{
			handler._listView.SetOrientation(Gtk.Orientation.Vertical);
		}
	}

	public static void MapBackgroundColor(CollectionViewHandler handler, IView view)
	{
		if (view is CollectionView cv && cv.BackgroundColor != null)
			handler.ApplyCss(handler.PlatformView, $"background-color: {ToGtkColor(cv.BackgroundColor)};");
	}

	public static void MapItemTemplate(CollectionViewHandler handler, IView view)
	{
		// Rebuild the ListView with the appropriate factory (string vs template)
		handler.RebuildListView();
		handler.HookSelectionChanged();
		MapItemsSource(handler, view);
	}

	public static void MapItemSizingStrategy(CollectionViewHandler handler, IView view) { }
	public static void MapItemsUpdatingScrollMode(CollectionViewHandler handler, IView view) { }
	public static void MapIsGrouped(CollectionViewHandler handler, IView view) { }
	public static void MapCanReorderItems(CollectionViewHandler handler, IView view) { }
	public static void MapAccessibility(CollectionViewHandler handler, IView view) { }

	void UpdateDisplayedChild(CollectionView collectionView)
	{
		if (PlatformView == null || _listView == null || _outerBox == null)
			return;

		var hasItems = _items.Count > 0;
		if (hasItems || collectionView.EmptyView == null)
		{
			_listView.SetVisible(true);
			_emptyLabel?.SetVisible(false);
			return;
		}

		// Show empty view
		if (_emptyLabel == null)
		{
			_emptyLabel = Gtk.Label.New(string.Empty);
			_emptyLabel.SetHalign(Gtk.Align.Center);
			_emptyLabel.SetValign(Gtk.Align.Center);
			_emptyLabel.SetWrap(true);
			_emptyLabel.SetJustify(Gtk.Justification.Center);
			_emptyLabel.SetVexpand(true);
			// Insert before footer
			if (_footerLabel != null)
				_outerBox.InsertChildAfter(_emptyLabel, _listView);
			else
				_outerBox.Append(_emptyLabel);
		}

		_emptyLabel.SetText(collectionView.EmptyView?.ToString() ?? string.Empty);
		_emptyLabel.SetVisible(true);
		_listView.SetVisible(false);
	}
}
