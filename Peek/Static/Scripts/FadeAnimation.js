Type.registerNamespace("Alchemy.Plugins.Peek.Controls");

Alchemy.Plugins.Peek.Controls.FadeAnimation = function Peek$FadeAnimation(element) 
{
	Tridion.OO.enableInterface(this, "Alchemy.Plugins.Peek.Controls.FadeAnimation");
	this.addInterface("Tridion.ControlBase", [element]);
};

Alchemy.Plugins.Peek.Controls.FadeAnimation.prototype.initialize = function Peek$FadeAnimation$initialize()
{
	var p = this.properties;
	p.fadeInAnimations = [];
	p.fadeOutAnimations = [];

	p.fadeInSettings = {
		duration: 300,
		curve: $animation.curve.linear
	};
	p.fadeOutSettings = {
		duration: 600,
		curve: $animation.curve.linear
	};
};

Alchemy.Plugins.Peek.Controls.FadeAnimation.prototype.stopFadeInAnimation = function Peek$FadeAnimation$stopFadeInAnimation()
{
	var p = this.properties;

	for (var i = 0, j = p.fadeInAnimations.length; i < j; i++)
	{
		$animation.stop(p.fadeInAnimations[i]);
	}
	p.fadeInAnimations = [];
};

Alchemy.Plugins.Peek.Controls.FadeAnimation.prototype.stopFadeOutAnimation = function Peek$FadeAnimation$stopFadeOutAnimation()
{
	var p = this.properties;

	for (var i = 0, j = p.fadeOutAnimations.length; i < j; i++)
	{
		$animation.stop(p.fadeOutAnimations[i]);
	}
	p.fadeOutAnimations = [];
};

Alchemy.Plugins.Peek.Controls.FadeAnimation.prototype.fadeIn = function Peek$FadeAnimation$fadeIn(to)
{
	var self = this;
	var p = this.properties;
	var settings = p.fadeInSettings;

	this.fireEvent("fadein");

	var animation = {
		property: "opacity",
		to: to || "1",
		duration: settings.duration,
		curve: settings.curve
	};

	animation.element = p.element;
	animation.callback = function()
	{
		self.stopFadeInAnimation();
		self.fireEvent("fadedin");
	};
	p.fadeInAnimations.push($animation.add(animation));
};

Alchemy.Plugins.Peek.Controls.FadeAnimation.prototype.fadeOut = function Peek$FadeAnimation$fadeOut(to)
{
	var p = this.properties;
	var self = this;

	this.fireEvent("fadeout");

	var animation = {
		property: "opacity",
		to: to || "0",
		duration: p.fadeOutSettings.duration,
		curve: p.fadeOutSettings.curve
	};

	animation.element = p.element;
	animation.callback = function()
	{
		self.stopFadeOutAnimation();
		self.fireEvent("fadedout");
	};
	p.fadeOutAnimations.push($animation.add(animation));
};

