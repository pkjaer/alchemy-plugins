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
	p.minimumHeight = 130;
	p.minimumWidth = 400;

	c.fadeAnimation = $controls.getControl(p.element, "Alchemy.Plugins.Peek.Controls.FadeAnimation");
	c.resizeAnimation = $controls.getControl(p.element, "Alchemy.Plugins.Peek.Controls.ResizeAnimation");

	$evt.addEventHandler(c.fadeAnimation, "fadein", this.getDelegate(this.show));
	$evt.addEventHandler(c.fadeAnimation, "fadedout", this.getDelegate(this.hide));

	var view = this.getView();
	if (view)
	{
		$evt.addEventHandler(view, "close", this.getDelegate(this._onCloseButtonClicked));
		$evt.addEventHandler(view, "resize", this.getDelegate(this._onFrameContentResized));
		$evt.addEventHandler(view, "linkClicked", this.getDelegate(this._onLinkClicked));
	}

	p.element.style.height = p.minimumHeight;
	p.element.style.width = p.minimumWidth;
	this.hide();
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

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.hide = function Peek$FrameManager$hide()
{
	var p = this.properties;
	$css.undisplay(this.getElement());
	p.displayed = false;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.hideAnimated = function Peek$FrameManager$hideAnimated()
{
	this.properties.controls.fadeAnimation.fadeOut();
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.isVisible = function Peek$FrameManager$isVisible()
{
	return this.properties.displayed;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.onSelectionChanged = function Peek$FrameManager$onSelectionChanged(selectedItem)
{
	if (this.isVisible() && selectedItem)
	{
		this.setSelectedItem(selectedItem);
	}
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.resize = function Peek$FrameManager$resize(height, width)
{
	this.properties.controls.resizeAnimation.resize(height, width);
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.setSelectedItem = function Peek$FrameManager$setSelectedItem(selectedItem)
{
	var p = this.properties;
	if (p.selectedItem == selectedItem) return;

	// Delayed slightly to avoid spamming requests when you quickly select multiple items in a row (read: scrolling through the list).
	if (p.delayTimer)
	{
		clearTimeout(p.delayTimer);
		p.delayTimer = null;
	}

	var view = this.getView();
	p.selectedItem = selectedItem;
	p.delayTimer = setTimeout(function() { view.setSelectedItem(p.selectedItem); }, p.delay);
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.show = function Peek$FrameManager$show()
{
	var p = this.properties;
	$css.display(this.getElement());
	p.displayed = true;
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.showAnimated = function Peek$FrameManager$showAnimated()
{
	this.properties.controls.fadeAnimation.fadeIn();
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype.toggle = function Peek$FrameManager$toggle(selectedItem)
{
	if (this.isVisible())
	{
		this.hideAnimated();
		return;
	} 

	this.setSelectedItem(selectedItem);
	this.showAnimated();
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._onCloseButtonClicked = function Peek$FrameManager$_onCloseButtonClicked(event)
{
	this.hideAnimated();
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._onFrameContentResized = function Peek$FrameManager$_onFrameContentResized(event)
{
	var p = this.properties;
	var suggestedHeight = event.data.height + 10;
	var suggestedWidth = event.data.width + 20;

	this.resize(Math.max(suggestedHeight, p.minimumHeight), Math.max(suggestedWidth, p.minimumWidth));
};

Alchemy.Plugins.Peek.Controls.FrameManager.prototype._onLinkClicked = function Peek$FrameManager$_onLinkClicked(event)
{
	var selection = new Tridion.Cme.Selection();
	selection.addItem(event.data.itemUri);
	$commands.executeCommand("Open", selection);
};