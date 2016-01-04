Alchemy.command("${PluginName}", "Peek",
{
	initialize: function ()
	{
		var p = this.properties;
		if (p.initialized) return;
		p.frameManager = $controls.getControl($("#PeekArea"), "Alchemy.Plugins.Peek.Controls.FrameManager");
		p.initialized = true;
	},

	isAvailable: function (selection)
	{
		var item = this.getSingleSelection(selection);
		return (item != null);
	},

	isEnabled: function (selection)
	{
		var p = this.properties;

		var result = this.isAvailable(selection);
		if (p.frameManager)
		{
			p.frameManager.onSelectionChanged(this.getSingleSelection(selection));
		}
		return result;
	},

	execute: function (selection)
	{
		var p = this.properties;
		this.initialize();
		p.frameManager.toggle(this.getSingleSelection(selection));
	},

	getSingleSelection: function (selection)
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