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
		"WindowTitle": "Peek",
		"Peeking": "Peeking...",
		"PeekFailed": "Peeking failed: ",
		"PeekFailedForUnknownReason": "Peek failed for an unknown reason. Is the plugin still installed?",
		"ClickToOpen": "Click to open '{0}'",
		"Label_id": "ID",
		"Label_lockedBy": "Locked by",
		"Label_directory": "Directory",
		"Label_schema": "Schema",
		"Label_metadataSchema": "Metadata Schema",
		"Label_defaultPageTemplate": "Default Page Template",
		"Label_linkedSchema": "Linked Schema",
		"Label_creationDate": "Created",
		"Label_revisionDate": "Last modified",
	};

	p.results = $("#Results");
	p.error = $("#Error");
	c.links = [];

	c.closeButton = $controls.getControl($("#CloseButton"), "Tridion.Controls.Button");
	$evt.addEventHandler(c.closeButton, "click", this.getDelegate(this._onCloseButtonClicked));

	$dom.setInnerText($("#TitleHeader"), p.resources.WindowTitle);
    this.callBase("Tridion.Cme.View", "initialize");
};

Alchemy.Plugins.Peek.Views.Frame.prototype.close = function PeekFrame$close()
{
	this.fireEvent("close");
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
			title = item.getStaticTitle();
		}
	}

	$dom.setInnerText($("#ItemTitle"), title);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.startPeeking = function PeekFrame$startPeeking()
{
	var p = this.properties;
	if (!p.selectedItem) return;

	$dom.setInnerText(p.results, p.resources.Peeking);

	Alchemy.Plugins["${PluginName}"].Api.PeekService.peek({ ItemUri: p.selectedItem })
		.success(this.getDelegate(this._onSuccess))
		.error(this.getDelegate(this._onError)
	);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.showError = function PeekFrame$_onError(errorText)
{
	var p = this.properties;
	p.results.innerHTML = "";
	$dom.setInnerText(p.error, errorText);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.clear = function PeekFrame$clear()
{
	var p = this.properties;
	var c = p.controls;

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

Alchemy.Plugins.Peek.Views.Frame.prototype.setSelectedItem = function PeekFrame$setSelectedItem(itemUri)
{
	var p = this.properties;
	if (p.selectedItem == itemUri) return;

	this.clear();
	p.selectedItem = itemUri;
	p.selectedItemType = $models.getItemType(itemUri);
	this.showItemTitle();
	this.startPeeking();
};

Alchemy.Plugins.Peek.Views.Frame.prototype.getContentHeight = function PeekFrame$getContentHeight()
{
	var content = $(".content");
	return Math.max(content.scrollHeight, content.offsetHeight);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.getContentWidth = function PeekFrame$getContentWidth()
{
	var content = $(".content");
	return Math.max(content.scrollWidth, content.offsetWidth);
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onSuccess = function PeekFrame$_onSuccess(result)
{
	var p = this.properties;
	var c = p.controls;

	p.results.innerHTML = "";

	var table = document.createElement("table");
	p.results.appendChild(table);

	for (var key in result)
	{
		var value = result[key];
		if (!value) continue;

		var row = document.createElement("tr");
		row.className = "result " + key;
		table.appendChild(row);

		var cell = document.createElement("td");
		$dom.setInnerText(cell, p.resources["Label_" + key] || key);
		row.appendChild(cell);

		cell = document.createElement("td");
		cell.className = "value";
		row.appendChild(cell);

		if (this._isDate(value))
		{
			var date = Tridion.Utils.Date.parse(value);
			$dom.setInnerText(cell, $localization.getFormattedDateTime(date, Tridion.Runtime.LocaleInfo.fullDateTimeFormat));
		} 
		else if (typeof(value) == "object")
		{
			var link = document.createElement("div");
			link.className = "link";
			link.title = p.resources.ClickToOpen.format(value.title);
			link.setAttribute("data-uri", value.id);

			var linkText = document.createElement("span");
			linkText.className = "text";
			$dom.setInnerText(linkText, value.title);
			link.appendChild(linkText);
			cell.appendChild(link);

			var button = $controls.getControl(link, "Tridion.Controls.Button");
			$evt.addEventHandler(button, "click", this.getDelegate(this._onLinkClicked));
			c.links.push(button);
		}
		else
		{
			$dom.setInnerText(cell, value);
		}
	}

	this.fireEvent("resize", { height: this.getContentHeight(), width: this.getContentWidth() });
};

Alchemy.Plugins.Peek.Views.Frame.prototype._isDate = function PeekFrame$_isDate(value)
{
	if (typeof(value) == "string")
	{
		var match = value.match(/\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d+/);
		return (match != null);
	}
	return false;
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onError = function PeekFrame$_onError(type, error)
{
	var p = this.properties;

	if (error && error.message)
	{
		this.showError(p.resources.PeekFailed + error.message);
	} 
	else
	{
		this.showError(p.resources.PeekFailedForUnknownReason);
	}
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onCloseButtonClicked = function PeekFrame$_onCloseButtonClicked(event)
{
	this.close();
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

$display.registerView(Alchemy.Plugins.Peek.Views.Frame); 