Type.registerNamespace("Alchemy.Plugins.Peek.Controls");

Alchemy.Plugins.Peek.Controls.FrameManager = function Peek$FrameManager(element) 
{
	Tridion.OO.enableInterface(this, "Alchemy.Plugins.Peek.Controls.FrameManager");
	this.addInterface("Tridion.ControlBase", [element]);
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.initialize = function FrameManager$initialize()
{
	var p = this.properties;
	var c = p.controls;

	p.delay = 300;

	var view = this.getView();
	if (view)
	{
		$evt.addEventHandler(view, "close", this.getDelegate(this._onCloseButtonClicked));
		$evt.addEventHandler(view, "resize", this.getDelegate(this._onFrameContentResized));
	}
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.setSelectedItem = function(selectedItem)
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

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._setSelectedItemDelayed = function()
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

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.hide = function()
{
	var p = this.properties;
	$css.undisplay(this.getElement());
	p.displayed = false;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.show = function()
{
	var p = this.properties;
	$css.display(this.getElement());
	p.displayed = true;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.isVisible = function()
{
	return this.properties.displayed;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.toggle = function(selectedItem)
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

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.getView = function()
{
	try
	{
		return this.getElement().contentWindow.$display.getView();
	} 
	catch (e) { };
	return null;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.onSelectionChanged = function(selectedItem)
{
	if (this.isVisible())
	{
		this.setSelectedItem(selectedItem);
	}
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._onCloseButtonClicked = function(event)
{
	this.hide();
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._onFrameContentResized = function(event)
{
	var width = event.data.width;
	var height = event.data.height;
	console.log("Resizing window to fit the content:", height, width);

	var frame = this.getElement();
	frame.style.height = (30 + height) + "px";
};