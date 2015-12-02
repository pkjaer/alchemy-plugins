Alchemy.command("${PluginName}", "AddUserByName", 
{
	execute: function()
	{
		var url = "${ViewsUrl}AddUserPopup.aspx";
		var parameters = { width: 400, height: 210 };
		var args = { popupType: Tridion.Controls.PopupManager.Type.MODAL_IFRAME };

		var popup = $popup.create(url, parameters, args);

		var confirm = function(event)
		{
			$evt.removeEventHandler(popup, "confirm", confirm);

			var service = Alchemy.Plugins["${PluginName}"].Api.AddUserService;
			var username = event.data.username;
			var fullName = event.data.fullName;

			console.log("Adding user: ", { name: username, description: fullName });
			service.newUser(username, fullName)
				.success(function(response) {
					console.log("Added new user: ", response);
				})
				.error(function(response) {
					console.log("Error!", response);
				})
				.complete(function() {
					console.log("Completed.");
					closePopup();
				});
		};

		var closePopup = function()
		{
			$evt.removeEventHandler(popup, "close", closePopup);
			popup.close();
		};

		$evt.addEventHandler(popup, "confirm", confirm);
		$evt.addEventHandler(popup, "close", closePopup);

		popup.open();
	}
});