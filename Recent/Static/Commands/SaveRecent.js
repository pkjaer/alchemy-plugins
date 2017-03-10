Alchemy.command("${PluginName}", "SaveRecent",
{
	isAvailable : function(selection, pipeline)
	{
		if (pipeline)
		{
			pipeline.stop = false;
		}

		return true;
	},

	isEnabled : function(selection, pipeline)
	{
		if (pipeline)
		{
			pipeline.stop = false;
		}

		return true;
	},

	execute : function(selection, pipeline)
	{
		if (pipeline)
		{
			pipeline.stop = false;
		}

		var itemUri = this.getSingleSelection(selection);
		var item = $models.getItem(itemUri);
		if (item)
		{
			Alchemy.Plugins.Recent.Api.RecentService.saveRecentItem({ id: itemUri, title: item.getStaticTitle() || item.getTitle(), icon: item.getItemIcon() })
				.success(this.getDelegate(this._onSuccess))
				.error(this.getDelegate(this._onError));
		}
	},

	_onSuccess: function(result)
	{
		Alchemy.Plugins.Recent.Api.RecentService.getRecentItems().success(this.getDelegate(this._showUpdatedList));
	},

	_showUpdatedList: function(result)
	{
		console.log("Recently viewed:");
		for (var i = 0; i < result.items.length; i++)
		{
			var item = result.items[i];
			console.log(item.title + " (" + item.id + ")");
		}
	},

	_onError: function(error)
	{
		console.warn("Error saving recent item list:", error);
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