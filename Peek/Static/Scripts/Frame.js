Type.registerNamespace("Alchemy.Plugins.Peek.Views");

Alchemy.Plugins.Peek.Views.Frame = function PeekFrame() 
{
	Tridion.OO.enableInterface(this, "Alchemy.Plugins.Peek.Views.Frame");
	this.addInterface("Tridion.Cme.View");
};

Alchemy.Plugins.Peek.Views.Frame.prototype.initialize = function PeekFrame$initialize()
{
	var p = this.properties;
	var c = p.controls;

	p.resources = {
		"By": "by {0}",
		"ClickToOpen": "Click to open '{0}'",
		"ClickToSuggest": "Click here to make a suggestion.",
		"NothingToSeeHere": "No information available for this item.",
		"PeekFailed": "Peeking failed: ",
		"PeekFailedForUnknownReason": "Peek failed for an unknown reason. Is the plugin still installed?",
		"Peeking": "Peeking...",
		"SuggestionLink": "https://github.com/pkjaer/alchemy-plugins/labels/Peek",
		"WindowTitle": "Peek"
	};

	p.results = $("#Results");
	p.error = $("#Error");
	p.loadingText = $(".loadingText");
	$dom.setInnerText(p.loadingText, p.resources.Peeking);
	
	c.links = [];
	c.closeButton = $controls.getControl($("#CloseButton"), "Tridion.Controls.Button");
	$evt.addEventHandler(c.closeButton, "click", this.getDelegate(this._onCloseButtonClicked));

	$dom.setInnerText($("#TitleHeader"), p.resources.WindowTitle);
    this.callBase("Tridion.Cme.View", "initialize");
};

Alchemy.Plugins.Peek.Views.Frame.prototype.clear = function PeekFrame$clear()
{
	var p = this.properties;
	var c = p.controls;

	this._stopListeningToChanges();

	if (c.links.length > 0)
	{
		for (var i = 0; i < c.links.length; i++)
		{
			$evt.removeEventHandler(c.links[i], "click", this.getDelegate(this._onLinkClicked));
			c.links[i].dispose();
		}
		c.links = [];
	}

	p.results.innerHTML = "";
	p.error.innerHTML = "";
};

Alchemy.Plugins.Peek.Views.Frame.prototype.close = function PeekFrame$close()
{
	this.fireEvent("close");
};

Alchemy.Plugins.Peek.Views.Frame.prototype.draw = function PeekFrame$draw(data)
{
	var p = this.properties;

	$css.undisplay(p.loadingText);
	p.results.innerHTML = "";
	p.results.style.width = "10px"; // Force it to only be as wide as it needs to be

	var empty = true;
	var table = document.createElement("table");

	if (data)
	{
		for (var key in data)
		{
			var entry = data[key];
			if (!entry) continue;

			empty = false;

			var row = document.createElement("tr");
			row.className = "result " + key;
			table.appendChild(row);

			var cell = document.createElement("td");
			cell.className = "label";
			$dom.setInnerText(cell, entry.label);
			row.appendChild(cell);

			cell = document.createElement("td");
			cell.className = "value";
			row.appendChild(cell);

			switch (entry.type)
			{
				case "date": this._addDateEntry(cell, key, entry); break;
				case "link": this._addLinkEntry(cell, entry); break;
				default: $dom.setInnerText(cell, entry.value); break;
			}
		}
	}

	p.results.style.width = ""; // Set it back to automatic width

	if (empty)
	{
		this.showSuggestionLink();
		return;
	}

	p.results.appendChild(table);
	this.fireEvent("resize", { height: this.getContentHeight(), width: this.getContentWidth() });
	this._listenToChanges();
};

Alchemy.Plugins.Peek.Views.Frame.prototype.getContentHeight = function PeekFrame$getContentHeight()
{
	var content = $(".content");
	return Math.max(content.scrollHeight, content.offsetHeight);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.getContentWidth = function PeekFrame$getContentWidth()
{
	var p = this.properties;
	return Math.max(p.results.scrollWidth, p.results.offsetWidth);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.isSupportedItemType = function PeekFrame$isSupportedItemType()
{
	var p = this.properties;
	if (!p.selectedItem) return false;

	var itemType = $models.getItemType(p.selectedItem);
	var it = $const.ItemType;

	switch (itemType)
	{
		case it.CATEGORY:
		case it.COMPONENT:
		case it.COMPONENT_TEMPLATE:
		case it.FOLDER:
		case it.GROUP:
		case it.KEYWORD:
		case it.MULTIMEDIA_TYPE:
		case it.PAGE:
		case it.PAGE_TEMPLATE:
		case it.PUBLICATION:
		case it.PUBLICATION_TARGET:
		case it.SCHEMA:
		case it.SHORTCUT_ITEM:
		case it.STRUCTURE_GROUP:
		case it.TARGET_GROUP:
		case it.TARGET_TYPE:
		case it.TEMPLATE_BUILDING_BLOCK:
		case it.USER:
		case it.VIRTUAL_FOLDER:
			return true;
	}

	return false;
};

Alchemy.Plugins.Peek.Views.Frame.prototype.setSelectedItem = function PeekFrame$setSelectedItem(itemUri, forceRefresh)
{
	var p = this.properties;
	if (!forceRefresh && p.selectedItem == itemUri) return;

	this.clear();
	p.selectedItem = itemUri;
	p.selectedItemType = $models.getItemType(itemUri);
	p.referencedItem = undefined;

	if (p.selectedItemType == $const.ItemType.SHORTCUT_ITEM)
	{
		var shortcut = $models.getItem(itemUri);
		p.referencedItem = shortcut.getReferencedId();
	}

	this.showItemTitle();
	this.startPeeking();
};

Alchemy.Plugins.Peek.Views.Frame.prototype.showError = function PeekFrame$showError(errorText)
{
	var p = this.properties;
	p.results.innerHTML = "";
	$dom.setInnerText(p.error, errorText);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.showItemTitle = function PeekFrame$showItemTitle()
{
	var p = this.properties;
	var title = "";

	if (p.selectedItem)
	{
		var item = $models.getItem(p.selectedItem);
		if (item)
		{
			title = "{0} ({1})".format(item.getStaticTitle(), p.referencedItem || p.selectedItem);
		}
	}

	$dom.setInnerText($("#ItemTitle"), title);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.showSuggestionLink = function PeekFrame$showSuggestionLink()
{
	var p = this.properties;

	p.results.innerHTML = "";

	var suggestionContainer = document.createElement("div");
	p.results.appendChild(suggestionContainer);

	var nothingToSeeHere = document.createElement("div");
	nothingToSeeHere.className = "emptyLabel";
	$dom.setInnerText(nothingToSeeHere, p.resources.NothingToSeeHere);
	suggestionContainer.appendChild(nothingToSeeHere);

	var suggestionLink = document.createElement("a");
	suggestionLink.href = p.resources.SuggestionLink;
	suggestionLink.target = "_blank";
	$dom.setInnerText(suggestionLink, p.resources.ClickToSuggest);
	suggestionContainer.appendChild(suggestionLink);

	$css.undisplay(p.loadingText);

	this.fireEvent("resize", { height: this.getContentHeight(), width: this.getContentWidth() });
};

Alchemy.Plugins.Peek.Views.Frame.prototype.startPeeking = function PeekFrame$startPeeking()
{
	var p = this.properties;

	if (!this.isSupportedItemType())
	{
		this.showSuggestionLink();
		return;
	}

	$css.display(p.loadingText);

	Alchemy.Plugins["${PluginName}"].Api.PeekService.peek({ ItemUri: p.referencedItem || p.selectedItem })
		.success(this.getDelegate(this._onSuccess))
		.error(this.getDelegate(this._onError)
	);
};

Alchemy.Plugins.Peek.Views.Frame.prototype._addDateEntry = function PeekFrame$_addDateEntry(cell, key, entry)
{
	var p = this.properties;
	var date = Tridion.Utils.Date.parse(entry.value);
	var formatted = $localization.getFormattedDateTime(date, Tridion.Runtime.LocaleInfo.fullDateTimeFormat);

	$dom.setInnerText(cell, formatted);

	var by = entry.user;
	if (by)
	{
		var label = document.createElement("div");
		$dom.setInnerText(label, p.resources.By.format(by));
		cell.appendChild(label);
	}
};

Alchemy.Plugins.Peek.Views.Frame.prototype._addLinkEntry = function PeekFrame$_addLinkEntry(cell, entry)
{
	var p = this.properties;
	var c = p.controls;

	var link = document.createElement("div");
	link.className = "link";
	link.title = p.resources.ClickToOpen.format(entry.linkTitle);
	link.setAttribute("data-uri", entry.linkId);

	var linkText = document.createElement("span");
	linkText.className = "text";
	$dom.setInnerText(linkText, entry.linkTitle);
	link.appendChild(linkText);
	cell.appendChild(link);

	var button = $controls.getControl(link, "Tridion.Controls.Button");
	$evt.addEventHandler(button, "click", this.getDelegate(this._onLinkClicked));
	c.links.push(button);
};

Alchemy.Plugins.Peek.Views.Frame.prototype._listenToChanges = function PeekFrame$_listenToChanges()
{
	var p = this.properties;
	if (p.selectedItem)
	{
		var item = $models.getItem(p.selectedItem);
		$evt.addEventHandler(item, "load", this.getDelegate(this._onItemChanged));
	}
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onCloseButtonClicked = function PeekFrame$_onCloseButtonClicked(event)
{
	this.close();
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onError = function PeekFrame$_onError(type, error)
{
	var p = this.properties;

	$css.undisplay(p.loadingText);

	if (error && error.message)
	{
		this.showError(p.resources.PeekFailed + error.message);
		return;
	} 

	console.warn(p.resources.PeekFailedForUnknownReason, arguments);
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onItemChanged = function PeekFrame$_onItemChanged(event)
{
	var p = this.properties;
	this.setSelectedItem(p.selectedItem, true);
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onLinkClicked = function PeekFrame$_onLinkClicked(event)
{
	var button = event.source;
	var itemUri = button.getElement().getAttribute("data-uri");

	if (itemUri)
	{
		this.fireEvent("linkClicked", { itemUri: itemUri });
	}
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onSuccess = function PeekFrame$_onSuccess(result)
{
	this.draw(result);
};

Alchemy.Plugins.Peek.Views.Frame.prototype._stopListeningToChanges = function PeekFrame$_stopListeningToChanges()
{
	var p = this.properties;
	if (p.selectedItem)
	{
		var item = $models.getItem(p.selectedItem);
		$evt.removeEventHandler(item, "load", this.getDelegate(this._onItemChanged));
	}
};

$display.registerView(Alchemy.Plugins.Peek.Views.Frame); 