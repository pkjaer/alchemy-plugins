﻿Type.registerNamespace("Alchemy.Plugins.Peek.Views");

Alchemy.Plugins.Peek.Views.Frame = function Frame() 
{
	Tridion.OO.enableInterface(this, "Alchemy.Plugins.Peek.Views.Frame");
	this.addInterface("Tridion.Cme.View");
};

Alchemy.Plugins.Peek.Views.Frame.prototype.initialize = function Frame$initialize()
{
	var p = this.properties;
	var c = p.controls;

	p.resources = {
		"WindowTitle": "Peek",
		"Peeking": "Peeking...",
		"PeekFailed": "Peeking failed: ",
		"PeekFailedForUnknownReason": "Peek failed for an unknown reason. Is the plugin still installed?"
	};

	p.results = $("#Results");
	p.error = $("#Error");

	c.closeButton = $controls.getControl($("#CloseButton"), "Tridion.Controls.Button");
	$evt.addEventHandler(c.closeButton, "click", this.getDelegate(this._onCloseButtonClicked));

	$dom.setInnerText($("#TitleHeader"), p.resources.WindowTitle);
    this.callBase("Tridion.Cme.View", "initialize");
};

Alchemy.Plugins.Peek.Views.Frame.prototype.close = function Frame$close()
{
	this.fireEvent("close");
};

Alchemy.Plugins.Peek.Views.Frame.prototype.showItemTitle = function Frame$showItemTitle()
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

Alchemy.Plugins.Peek.Views.Frame.prototype.startPeeking = function Frame$startPeeking()
{
	var p = this.properties;
	if (!p.selectedItem) return;

	$dom.setInnerText(p.results, p.resources.Peeking);

	Alchemy.Plugins["${PluginName}"].Api.PeekService.peek({ ItemUri: p.selectedItem })
		.success(this.getDelegate(this._onSuccess))
		.error(this.getDelegate(this._onError)
	);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.showError = function Frame$_onError(errorText)
{
	var p = this.properties;
	p.results.innerHTML = "";
	$dom.setInnerText(p.error, errorText);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.clear = function Frame$clear()
{
	var p = this.properties;
	p.results.innerHTML = "";
	p.error.innerHTML = "";
};

Alchemy.Plugins.Peek.Views.Frame.prototype.setSelectedItem = function Frame$setSelectedItem(itemUri)
{
	var p = this.properties;
	if (p.selectedItem == itemUri) return;

	this.clear();
	p.selectedItem = itemUri;
	p.selectedItemType = $models.getItemType(itemUri);
	this.showItemTitle();
	this.startPeeking();
};

Alchemy.Plugins.Peek.Views.Frame.prototype.getContentHeight = function Frame$getContentHeight()
{
	var content = $(".content");
	return Math.max(content.scrollHeight, content.offsetHeight);
};

Alchemy.Plugins.Peek.Views.Frame.prototype.getContentWidth = function Frame$getContentWidth()
{
	var content = $(".content");
	return Math.max(content.scrollWidth, content.offsetWidth);
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onSuccess = function Frame$_onSuccess(result)
{
	var p = this.properties;
	var c = p.controls;

	p.results.innerHTML = "";

	var table = document.createElement("table");
	p.results.appendChild(table);

	for (var key in result)
	{
		if (!result[key]) continue;

		var row = document.createElement("tr");
		row.className = "result " + key;
		table.appendChild(row);

		var cell = document.createElement("td");
		$dom.setInnerText(cell, p.resources[key] || key);
		row.appendChild(cell);

		cell = document.createElement("td");
		cell.className = "value";
		$dom.setInnerText(cell, result[key]);
		row.appendChild(cell);
	}

	this.fireEvent("resize", { height: this.getContentHeight(), width: this.getContentWidth() });
};

Alchemy.Plugins.Peek.Views.Frame.prototype._onError = function Frame$_onError(type, error)
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

Alchemy.Plugins.Peek.Views.Frame.prototype._onCloseButtonClicked = function Frame$_onCloseButtonClicked(event)
{
	this.close();
};

$display.registerView(Alchemy.Plugins.Peek.Views.Frame); 