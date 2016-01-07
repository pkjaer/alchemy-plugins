Alchemy.command("${PluginName}", "Peek",
{
	initialize : function()
	{
		var p = this.properties;
		if (p.initialized) return;
		p.frameManager = $controls.getControl($("#PeekArea"), "Alchemy.Plugins.Peek.Controls.FrameManager");
		p.initialized = true;
	},

	isAvailable : function(selection)
	{
		return true;
	},

	isEnabled : function(selection)
	{
		var p = this.properties;
		var item = this.getSingleSelection(selection);

		if (p.frameManager)
		{
			p.frameManager.onSelectionChanged(item);
		}

		return true;
	},

	execute : function(selection)
	{
		var p = this.properties;
		this.initialize();

		var item = this.getSingleSelection(selection);
		p.frameManager.toggle(item);
	},

	getSingleSelection : function(selection)
	{
		$assert.isObject(selection);

		switch (selection.getCount())
		{
			case 0: return (selection.getParentItemUri) ? selection.getParentItemUri() : null;
			case 1: return selection.getItem(0);
			default: return null;
		}
	}
});