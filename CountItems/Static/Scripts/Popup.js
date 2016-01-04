Type.registerNamespace("Alchemy.Plugins.CountItems.Views");

Alchemy.Plugins.CountItems.Views.CountItemsPopup = function CountItemsPopup() 
{
	Tridion.OO.enableInterface(this, "Alchemy.Plugins.CountItems.Views.CountItemsPopup");
	this.addInterface("Tridion.Cme.View");
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.initialize = function CountItemsPopup$initialize()
{
	var p = this.properties;
    var c = p.controls;
	var self = this;

	// TODO: Find out how to add client resources in Alchemy, so we can use $localization.getResource(..)
	p.resources = {
		"WindowTitle": "Count Items",
		"DefaultResultLabel": "Press Count to calculate the number of items.",
		"Counting": "Counting...",
		"CountingFailed": "Counting of items failed: ",
		"CountingFailedForUnknownReason": "Counting of items failed for an unknown reason. Is the plugin still installed?",
		"ErrorNoOptionsSelected": "You need to select at least one type of item to count.",
		"LabelFolders": "Folders", 
		"LabelComponents": "Components",
		"LabelSchemas": "Schemas",
		"LabelComponentTemplates": "Component Templates",
		"LabelPageTemplates": "Page Templates",
		"LabelTemplateBuildingBlocks": "Template Building Blocks",
		"LabelStructureGroups": "Structure Groups",
		"LabelPages": "Pages",
		"LabelCategories": "Categories",
		"LabelKeywords": "Keywords",
		"Results": "Results"
	};

	p.selectedItem = $url.getHashParam("selectedItem");
	p.selectedItemType = $models.getItemType(p.selectedItem);
	p.optionsArea = $("#optionsArea");
	p.results = $("#Results");
	p.error = $("#Error");
	p.options = {};

	$dom.setInnerText(p.results, p.resources.DefaultResultLabel);

	c.countButton = $controls.getControl($("#CountButton"), "Tridion.Controls.Button");
	c.countButton.disable();
    $evt.addEventHandler(c.countButton, "click", this.getDelegate(this.onCountClicked));
	$evt.addEventHandler(document, "keyup", this.getDelegate(this.onKeyUp));

	switch (p.selectedItemType)
	{
		case $const.ItemType.FOLDER:
			this.addOptions(["Folders", "Schemas", "Components", "ComponentTemplates", "PageTemplates", "TemplateBuildingBlocks"]);
			break;
		case $const.ItemType.PUBLICATION:
			this.addOptions(["Folders", "Schemas", "Components", "ComponentTemplates","PageTemplates", "TemplateBuildingBlocks", "StructureGroups", "Pages", "Categories", "Keywords"]);
			break;
		case $const.ItemType.STRUCTURE_GROUP:
			this.addOptions(["StructureGroups", "Pages"]);
			break;
		case $const.ItemType.CATEGORY:
			this.addOptions(["Categories", "Keywords"]);
			break;
		case $const.ItemType.CATMAN:
			p.selectedItem = p.selectedItem.substr(7); // Change it to look in the Publication as "Categories and Keywords" isn't a real item
			this.addOptions(["Categories", "Keywords"]);
			break;
	}

	this.displayOptions();

	Alchemy.Plugins["${PluginName}"].Api.getSettings()
		.success(function (settings) {
			if (settings && settings.startCountingImmediately == true)
			{
				self.confirm();
			}
		})
		.complete(function() {
			c.countButton.enable();
		});

	this.showItemTitle();

    this.callBase("Tridion.Cme.View", "initialize");
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.addOptions = function CountItemsPopup$addOptions(arrayOfRootNames)
{
	$assert.isArray(arrayOfRootNames);
	for (var i = 0; i < arrayOfRootNames.length; i++)
	{
		this.addOption(arrayOfRootNames[i]);
	}
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.addOption = function CountItemsPopup$addOption(rootName)
{
	var p = this.properties;

	var capitalized = this.capitalize(rootName);
	var requestKey = "Count" + capitalized;
	var resultKey = this.camelCase(rootName);
	var label = p.resources["Label" + capitalized];

	p.options[resultKey] = {requestKey: requestKey, resultKey: resultKey, label: label, button: null};
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.displayOptions = function CountItemsPopup$displayOptions()
{
	var p = this.properties;
	var c = p.controls;

	for (var key in p.options)
	{
		var option = p.options[key];
		var name = option.requestKey;

		var container = document.createElement("div");
		container.className = "option button2013 gray tridion button toggle on " + name;
		container.title = option.label;
		p.optionsArea.appendChild(container);

		var image = document.createElement("div");
		image.className = "image";
		image.innerHTML = "&nbsp;";
		container.appendChild(image);

		var button = c['optionButton' + name] = $controls.getControl(container, "Tridion.Controls.Button");
		option.button = button;
	}
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.close = function CountItemsPopup$close()
{
	this.fireEvent("close");
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.confirm = function CountItemsPopup$confirm()
{
	var p = this.properties;
	var c = p.controls;

	var params = { OrganizationalItemId : p.selectedItem };

	var nothingSelected = true;
	for (var key in p.options)
	{
		var option = p.options[key];
		if (option.button && option.button.isOn())
		{
			params[option.requestKey] = true;
			nothingSelected = false;
		}
	}

	if (nothingSelected)
	{
		this.showError(p.resources.ErrorNoOptionsSelected);
		return;
	}

	$dom.setInnerText(p.error, "");
	$dom.setInnerText(p.results, p.resources.Counting);
	c.countButton.disable();

	Alchemy.Plugins["${PluginName}"].Api.CountItemsService.getCount(params)
		.success(this.getDelegate(this._onSuccess))
		.error(this.getDelegate(this._onError)
	);
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.onKeyUp = function CountItemsPopup$onKeyUp(event)
{
	if (!event || !event.keyCode) return;

	switch (event.keyCode)
	{
		case 13: this.confirm(); break;
		case 27: this.close(); break;
	}
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.onCancelClicked = function CountItemsPopup$onCancelClicked(event)
{
	this.close();
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.onCountClicked = function CountItemsPopup$onCountClicked(event)
{
	this.confirm();
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.capitalize = function CountItemsPopup$capitalize(text)
{
	if (!text) return "";
	return text.substr(0, 1).toUpperCase() + text.substr(1);
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.camelCase = function CountItemsPopup$camelCase(text)
{
	if (!text) return "";
	return text.substr(0, 1).toLowerCase() + text.substr(1);
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.showItemTitle = function CountItemsPopup$showItemTitle()
{
	var p = this.properties;
	var title = p.resources.WindowTitle;

	if (p.selectedItem)
	{
		var item = $models.getItem(p.selectedItem);
		if (item)
		{
			title += " - " + item.getStaticTitle();
		}
	}

	window.title = document.title = title;
	$dom.setInnerText($("#TitleHeader"), title);
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype._onSuccess = function CountItemsPopup$_onSuccess(result)
{
	var p = this.properties;
	var c = p.controls;

	p.results.innerHTML = "";

	var header = document.createElement("h3");
	$dom.setInnerText(header, p.resources.Results);
	p.results.appendChild(header);

	var table = document.createElement("table");
	p.results.appendChild(table);

	for (var key in p.options)
	{
		var option = p.options[key];
		if (option.button && option.button.isOn())
		{
			var count = result[option.resultKey];
			var row = document.createElement("tr");
			row.className = "result " + option.requestKey;
			table.appendChild(row);

			var cell = document.createElement("td");
			$dom.setInnerText(cell, option.label);
			row.appendChild(cell);

			cell = document.createElement("td");
			cell.className = "count";
			$dom.setInnerText(cell, count);
			row.appendChild(cell);
		}
	}

	c.countButton.enable();
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype.showError = function CountItemsPopup$_onError(errorText)
{
	var p = this.properties;
	p.results.innerHTML = "";
	$dom.setInnerText(p.error, errorText);
};

Alchemy.Plugins.CountItems.Views.CountItemsPopup.prototype._onError = function CountItemsPopup$_onError(type, error)
{
	var p = this.properties;
	var c = p.controls;

	if (error && error.message)
	{
		this.showError(p.resources.CountingFailed + error.message);
	} 
	else
	{
		this.showError(p.resources.CountingFailedForUnknownReason);
	}
	c.countButton.enable();
};

$display.registerView(Alchemy.Plugins.CountItems.Views.CountItemsPopup); 