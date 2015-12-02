Type.registerNamespace("Alchemy.Plugins.AddUser.Views");

Alchemy.Plugins.AddUser.Views.AddUserPopup = function AddUserPopup() 
{
	Tridion.OO.enableInterface(this, "Alchemy.Plugins.AddUser.Views.AddUserPopup");
	this.addInterface("Tridion.Cme.View");
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.initialize = function AddUserPopup$initialize()
{
	var p = this.properties;
    var c = p.controls;

	p.username = $("#Username");
	p.fullName = $("#FullName");
	p.usernameRequiredError = $("#UserNameRequiredError");

	c.cancelButton = $controls.getControl($("#CancelButton"), "Tridion.Controls.Button");
	c.okButton = $controls.getControl($("#OkButton"), "Tridion.Controls.Button");

    this.callBase("Tridion.Cme.View", "initialize");

    $evt.addEventHandler(c.cancelButton, "click", this.getDelegate(this.onCancelClicked));
    $evt.addEventHandler(c.okButton, "click", this.getDelegate(this.onOkClicked));
	$evt.addEventHandler(document, "keyup", this.getDelegate(this.onKeyUp));

	p.username.focus();
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.close = function AddUserPopup$close()
{
	this.fireEvent("close");
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.confirm = function AddUserPopup$close()
{
	var p = this.properties;
	if (!p.username.value)
	{
		$css.display(p.usernameRequiredError);
		return;
	}

	this.fireEvent("confirm", { username: p.username.value, fullName: p.fullName.value });
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.onKeyUp = function AddUserPopup$onKeyUp(event)
{
	if (!event || !event.keyCode) return;

	switch (event.keyCode)
	{
		case 13: this.confirm(); break;
		case 27: this.close(); break;
	}
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.onCancelClicked = function AddUserPopup$onCancelClicked(event)
{
	this.close();
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.onOkClicked = function AddUserPopup$onOkClicked(event)
{
	this.confirm();
};

$display.registerView(Alchemy.Plugins.AddUser.Views.AddUserPopup); 