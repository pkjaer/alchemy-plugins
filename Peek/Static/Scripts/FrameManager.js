Type.registerNamespace("Alchemy.Plugins.Peek.Controls");

Alchemy.Plugins.Peek.Controls.FrameManager = function Peek$FrameManager(element) 
{
	Tridion.OO.enableInterface(this, "Alchemy.Plugins.Peek.Controls.FrameManager");
	this.addInterface("Tridion.ControlBase", [element]);
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.initialize = function Peek$FrameManager$initialize()
{
	var p = this.properties;
	var c = p.controls;

	p.delay = 300;

	var view = this.getView();
	if (view)
	{
		$evt.addEventHandler(view, "close", this.getDelegate(this._onCloseButtonClicked));
		$evt.addEventHandler(view, "resize", this.getDelegate(this._onFrameContentResized));
		$evt.addEventHandler(view, "linkClicked", this.getDelegate(this._onLinkClicked));
	}
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.setSelectedItem = function Peek$FrameManager$setSelectedItem(selectedItem)
{
	var p = this.properties;
	if (p.selectedItem == selectedItem) return;
	if (!selectedItem)
	{
		this.hide();
		return;
	}

	// Delayed slightly to avoid spamming requests when you quickly select multiple items in a row (think scrolling through the list).
	if (p.delayTimer)
	{
		clearTimeout(p.delayTimer);
		p.delayTimer = null;
	}

	p.selectedItem = selectedItem;
	p.delayTimer = setTimeout(this.getDelegate(this._setSelectedItemDelayed), p.delay);
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._setSelectedItemDelayed = function Peek$FrameManager$_setSelectedItemDelayed()
{
	var p = this.properties;
	p.delayTimer = null;

	var view = this.getView();
	if (view)
	{
		view.setSelectedItem(p.selectedItem);
	} 
	else
	{
		console.warn("Failed to set selected item for Peek.");
	}
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.hide = function Peek$FrameManager$hide()
{
	var p = this.properties;
	$css.undisplay(this.getElement());
	p.displayed = false;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.show = function Peek$FrameManager$show()
{
	var p = this.properties;
	$css.display(this.getElement());
	p.displayed = true;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.isVisible = function Peek$FrameManager$isVisible()
{
	return this.properties.displayed;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.toggle = function Peek$FrameManager$toggle(selectedItem)
{
	if (this.isVisible())
	{
		this.hide();
	} 
	else
	{
		this.setSelectedItem(selectedItem);
		this.show();
	}
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.getView = function Peek$FrameManager$getView()
{
	try
	{
		return this.getElement().contentWindow.$display.getView();
	} 
	catch (e) { };
	return null;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.onSelectionChanged = function Peek$FrameManager$onSelectionChanged(selectedItem)
{
	if (this.isVisible())
	{
		this.setSelectedItem(selectedItem);
	}
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._onCloseButtonClicked = function Peek$FrameManager$_onCloseButtonClicked(event)
{
	this.hide();
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._onFrameContentResized = function Peek$FrameManager$_onFrameContentResized(event)
{
	var height = event.data.height;
	var frame = this.getElement();
	frame.style.height = (30 + height) + "px";
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._onLinkClicked = function Peek$FrameManager$_onLinkClicked(event)
{
	var selection = new Tridion.Cme.Selection();
	selection.addItem(event.data.itemUri);
	$commands.executeCommand("Open", selection);
};