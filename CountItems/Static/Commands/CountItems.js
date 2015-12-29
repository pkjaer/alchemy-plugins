Alchemy.command("${PluginName}", "CountItems",
{
	isAvailable: function (selection)
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

	isEnabled: function (selection)
	{
		return this.isAvailable(selection);
	},

	execute: function (selection)
	{
		var p = this.properties;
		var selectedItem = this._getSelectedItem(selection);
		var url = "${ViewsUrl}CountItemsPopup.aspx#selectedItem=" + selectedItem;
		var height = this._getPopupHeight(selectedItem);
		var parameters = "width=432px, height=" + height + "px";
		var args = { popupType: Tridion.Controls.PopupManager.Type.EXTERNAL };

		p.popup = $popupManager.createExternalContentPopup(url, parameters, args);
		$evt.addEventHandler(p.popup, "close", this.getDelegate(this.closePopup));
		p.popup.open();
	},

	closePopup: function ()
	{
		var p = this.properties;
		if (p.popup)
		{
			p.popup.close();
			p.popup = null;
		}
	},

	_getPopupHeight: function (selectedItem)
	{
		switch ($models.getItemType(selectedItem))
		{
			case $const.ItemType.FOLDER:
				return 390;
			case $const.ItemType.PUBLICATION:
				return 490;
		}

		return 270;
	},

	_getSelectedItem: function (selection)
	{
		$assert.isObject(selection);

		switch (selection.getCount())
		{
			case 0: return (selection.getParentItemUri) ? selection.getParentItemUri() : null;
			case 1: return selection.getItem(0);
			default: return null;
		}
	},
});