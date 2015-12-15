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
		var url = "${ViewsUrl}CountItemsPopup.aspx#selectedItem=" + this._getSelectedItem(selection);
		var parameters = "width=432px, height=440px";
		var args = { popupType: Tridion.Controls.PopupManager.Type.EXTERNAL };

		p.popup = $popupManager.createExternalContentPopup(url, parameters, args);
		$evt.addEventHandler(p.popup, "close", this.getDelegate(this.closePopup));
		p.popup.open();
    },

	closePopup : function()
	{
		var p = this.properties;
		if (p.popup)
		{
			p.popup.close();
			p.popup = null;
		}
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
});