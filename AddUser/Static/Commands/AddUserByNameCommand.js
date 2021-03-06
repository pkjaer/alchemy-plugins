﻿Alchemy.command("${PluginName}", "AddUserByName", 
{
	resources: 
	{
		addingUser: "Adding user",
		addedUser: "User added."
	},

	isAvailable: function(selection, pipeline)
	{
		return $commands.getCommand("NewUser").isAvailable(selection, pipeline);
	},

	isEnabled: function(selection, pipeline)
	{
		return $commands.getCommand("NewUser").isEnabled(selection, pipeline);
	},

	execute: function()
	{
		var p = this.properties;
		var url = "${ViewsUrl}AddUserPopup.aspx";
		var parameters = { width: 400, height: 210 };
		var args = { popupType: Tridion.Controls.PopupManager.Type.MODAL_IFRAME };

		p.popup = $popup.create(url, parameters, args);
		$evt.addEventHandler(p.popup, "confirm", this.getDelegate(this.onConfirm));
		$evt.addEventHandler(p.popup, "close", this.getDelegate(this.closePopup));
		p.popup.open();
	},

	onConfirm: function(event)
	{
		var p = this.properties;
		var self = this;
		var service = Alchemy.Plugins["${PluginName}"].Api.AddUserService;
		var username = event.data.username;
		var fullName = event.data.fullName;
		var openAfterwards = event.data.openAfterwards;

		$evt.removeEventHandler(p.popup, "confirm", this.getDelegate(this.onConfirm));

		this.closePopup();

		var progress = $messages.registerProgress(this.resources.addingUser);
		progress.setOnSuccessMessage(this.resources.addedUser, fullName + " (" + username + ")");

		service.newUser({ name: username, description: fullName })
			.success(function(userId) 
			{
				progress.finish({ success: true });
				self.updateList();
				if (openAfterwards)
				{
					self.openUser(userId);
				}
			})
			.error(function(errorType, error) 
			{
				progress.finish({ success: false });
				$messages.registerError(error.message);
			});
	},

	closePopup: function()
	{
		var popup = this.properties.popup;
		$evt.removeEventHandler(popup, "close", this.getDelegate(this.closePopup));
		popup.close();
		this.properties.popup = null;
	},

	updateList: function()
	{
		var systemRoot = $models.getItem($const.TCMROOT);
		var list = systemRoot.getListUsers(null, false, false);
		if (list) list.unload();
	},

	openUser: function(userId)
	{
		var selection = new Tridion.Cme.Selection();
		selection.addItem(userId);
		$commands.executeCommand("Open", selection);
	}
});