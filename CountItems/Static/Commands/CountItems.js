Alchemy.command("${PluginName}", "CountItems", 
{
    isAvailable : function(selection) 
	{
		var item = this._getSelectedItem(selection);
		if (item != null)
		{
			switch ($models.getItemType(item))
			{
				case $const.ItemType.FOLDER:
				case $const.ItemType.PUBLICATION:
				case $const.ItemType.STRUCTURE_GROUP:
				case $const.ItemType.CATEGORY:
				case $const.ItemType.CATMAN:
					return true;
			}
		}

		return false;
    },

    isEnabled : function(selection) 
	{
        return this.isAvailable(selection);
    },

    execute : function(selection) 
	{
		var p = this.properties;
        p.progress = $messages.registerProgress("Counting items...", null);

		var params = {
			OrganizationalItemId : this._getSelectedItem(selection),
			CountFolders : true,
			CountComponents : true,
			CountSchemas : true,
			CountComponentTemplates : true,
			CountPageTemplates : true,
			CountTemplateBuildingBlocks : true,
			CountStructureGroups : true,
			CountPages : true,
			CountCategories : true,
			CountKeywords : true
		};

        Alchemy.Plugins["${PluginName}"].Api.CountItemsService.getCount(params)
			.success(this.getDelegate(this._onSuccess))
			.error(this.getDelegate(this._onError)
		);
    },

	_getSelectedItem : function(selection)
	{
		$assert.isObject(selection);

		switch (selection.getCount())
		{
			case 0: // check the Tree selection
				var treeView = $controls.getControl($("#DashboardTree"), "Tridion.Controls.FilteredTree");
				return treeView.getSelection().getItem(0);
			case 1: // single item selected in the main list
				return selection.getItem(0);
			default:
				return null;
		}
	},

	_onSuccess : function(result)
	{
		var p = this.properties;
		var lines = [];
		lines.push("Categories: " + result.categories);
		lines.push("Components: " + result.components);
		lines.push("Component Templates: " + result.componentTemplates);
		lines.push("Folders: " + result.folders);
		lines.push("Keywords: " + result.keywords);
		lines.push("Pages: " + result.pages);
		lines.push("Page Templates: " + result.pageTemplates);
		lines.push("Schemas: " + result.schemas);
		lines.push("Structure Groups: " + result.structureGroups);
		lines.push("TBBs: " + result.templateBuildingBlocks);

		p.progress.setOnSuccessMessage("Count complete", lines.join("\n"));
		p.progress.finish({success : true});
		console.log("Count result: ", lines.join("\n"));
	},

	_onError : function(type, error)
	{
		var p = this.properties;
		p.progress.finish({success : false});
		$messages.registerError("Counting of items failed", error.message);
	},
});