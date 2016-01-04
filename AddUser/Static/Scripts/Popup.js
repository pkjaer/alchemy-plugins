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
	p.openAfterwards = $("#OpenAfterwards");

	c.cancelButton = $controls.getControl($("#CancelButton"), "Tridion.Controls.Button");
	c.saveButton = $controls.getControl($("#SaveButton"), "Tridion.Controls.Button");

    this.callBase("Tridion.Cme.View", "initialize");

    $evt.addEventHandler(c.cancelButton, "click", this.getDelegate(this.onCancelClicked));
    $evt.addEventHandler(c.saveButton, "click", this.getDelegate(this.onSaveClicked));
	$evt.addEventHandler(document, "keyup", this.getDelegate(this.onKeyUp));
	$evt.addEventHandler(p.username, "change", this.getDelegate(this.onUserNameChanged));
	$evt.addEventHandler(p.username, "keyup", this.getDelegate(this.onUserNameChanged));

	p.username.focus();
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.close = function AddUserPopup$close()
{
	this.fireEvent("close");
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.confirm = function AddUserPopup$confirm(openUserAfterwards)
{
	var p = this.properties;
	var c = p.controls;

	if (!p.username.value)
	{
		$css.display(p.usernameRequiredError);
		return;
	}

	c.cancelButton.disable();
	c.saveButton.disable();

	this.fireEvent("confirm", { username: p.username.value, fullName: p.fullName.value, openAfterwards: p.openAfterwards.checked });
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

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.onUserNameChanged = function AddUserPopup$onUserNameChanged(event)
{
	var p = this.properties;
	if (p.username.value)
	{
		$css.undisplay(p.usernameRequiredError);
	} 
	else
	{
		$css.display(p.usernameRequiredError);
	}
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.onCancelClicked = function AddUserPopup$onCancelClicked(event)
{
	this.close();
};

Alchemy.Plugins.AddUser.Views.AddUserPopup.prototype.onSaveClicked = function AddUserPopup$onSaveClicked(event)
{
	this.confirm();
};

$display.registerView(Alchemy.Plugins.AddUser.Views.AddUserPopup); 