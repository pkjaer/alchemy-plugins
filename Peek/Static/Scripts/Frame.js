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
		"By": "by {0}",
		"NothingToSeeHere": "No information available for this item.",
		"ClickToSuggest": "Click here to make a suggestion.",
		"SuggestionLink": "https://github.com/pkjaer/alchemy-plugins/labels/Peek",
		"Label_id": "ID",
		"Label_lockedBy": "Locked by",
		"Label_description": "Description",
		"Label_key": "Key",
		"Label_xmlName": "XML Name",
		"Label_useForIdentification": "For identification",
		"Label_isAbstract": "Abstract",
        "Label_publicationPath": "Publication Path",
		"Label_publicationUrl": "Publication URL",
        "Label_multimediaPath": "Multimedia Path",
        "Label_multimediaUrl": "Multimedia URL",
		"Label_directory": "Directory",
		"Label_fullDirectory": "Directory (full)",
		"Label_publishable": "Publishable",
		"Label_namespace": "Namespace",
		"Label_rootElementName": "Root Element",
		"Label_fieldsSummary": "Fields",
		"Label_metadataFieldsSummary": "Metadata Fields",
		"Label_status": "Status",
		"Label_isAdministrator": "Administrator",
		"Label_language": "Language",
		"Label_locale": "Regional Settings",
		"Label_groupMemberships": "Group Memberships",
		"Label_scope": "Scope",
		"Label_schema": "Schema",
		"Label_metadataSchema": "Metadata Schema",
		"Label_template": "Template",
		"Label_templateType": "Template Type",
		"Label_parametersSchema": "Parameters Schema",
		"Label_dynamicTemplateInfo": "Dynamic",
		"Label_priority": "Priority",
		"Label_extension": "Extension",
		"Label_fileName": "File Name",
		"Label_fileSize": "File Size",
		"Label_componentPresentations": "Component Presentations",
		"Label_conditions": "Conditions",
		"Label_mimeType": "MIME Type",
		"Label_fileExtensions": "File Extensions",
		"Label_linkedSchema": "Linked Schema",
		"Label_versions": "Versions",
		"Label_creationDate": "Created",
		"Label_revisionDate": "Last modified",
		"Label_webDavUrl": "WebDAV URL"
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

Alchemy.Plugins.Peek.Views.Frame.prototype.draw = function PeekFrame$draw()
{
	var p = this.properties;
	var c = p.controls;
	var data = p.data;

	if (!data)
	{
		this.showSuggestionLink();
		return;
	}

	$css.undisplay(p.loadingText);
	p.results.innerHTML = "";
	p.results.style.width = "10px"; // Force it to only be as wide as it needs to be

	var empty = true;
	var table = document.createElement("table");

	for (var key in data)
	{
		var value = data[key];
		if (!value) continue;
		if (key == "revisor" || key == "creator") continue;
		
		empty = false;

		var row = document.createElement("tr");
		row.className = "result " + key;
		table.appendChild(row);

		var cell = document.createElement("td");
		cell.className = "label";
		$dom.setInnerText(cell, p.resources["Label_" + key] || key);
		row.appendChild(cell);

		cell = document.createElement("td");
		cell.className = "value";
		row.appendChild(cell);

		if (this._isDate(value))
		{
			var date = Tridion.Utils.Date.parse(value);
			var formatted = $localization.getFormattedDateTime(date, Tridion.Runtime.LocaleInfo.fullDateTimeFormat);
			var by;

			switch (key)
			{
				case "creationDate": by = data["creator"]; break;
				case "revisionDate": by = data["revisor"]; break;
			}

			$dom.setInnerText(cell, formatted);
			if (by)
			{
				var byLabel = document.createElement("div");
				$dom.setInnerText(byLabel, p.resources.By.format(by));
				cell.appendChild(byLabel);
			} 
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

	if (empty)
	{
		this.showSuggestionLink();
	}
	else
	{
		p.results.appendChild(table);
	}

	this.fireEvent("resize", { height: this.getContentHeight(), width: this.getContentWidth() });
	p.results.style.width = ""; // Set it back to automatic width

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
		case it.PUBLICATION:
		case it.FOLDER:
		case it.STRUCTURE_GROUP:
		case it.SCHEMA:
		case it.COMPONENT:
		case it.COMPONENT_TEMPLATE:
		case it.PAGE:
		case it.PAGE_TEMPLATE:
		case it.TARGET_GROUP:
		case it.CATEGORY:
		case it.KEYWORD:
		case it.TEMPLATE_BUILDING_BLOCK:
		case it.VIRTUAL_FOLDER:
		case it.PUBLICATION_TARGET:
		case it.TARGET_TYPE:
		case it.MULTIMEDIA_TYPE:
		case it.USER:
		case it.GROUP:
		case it.SHORTCUT_ITEM:
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

Alchemy.Plugins.Peek.Views.Frame.prototype._isDate = function PeekFrame$_isDate(value)
{
	if (typeof(value) == "string")
	{
		var match = value.match(/\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d+/);
		return (match != null);
	}
	return false;
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
	} 
	else
	{
		console.warn(p.resources.PeekFailedForUnknownReason, arguments);
	}
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
	var p = this.properties;
	p.data = result;
	this.draw();
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