Type.registerNamespace("Alchemy.Plugins.Peek.Controls");

Alchemy.Plugins.Peek.Controls.ResizeAnimation = function Peek$ResizeAnimation(element) 
{
	Tridion.OO.enableInterface(this, "Alchemy.Plugins.Peek.Controls.ResizeAnimation");
	this.addInterface("Tridion.ControlBase", [element]);
};

Alchemy.Plugins.Peek.Controls.ResizeAnimation.prototype.initialize = function Peek$ResizeAnimation$initialize()
{
	var p = this.properties;
	p.animations = [];

	p.animationSettings = {
		duration: 300,
		curve: $animation.curve.linear
	};
};

Alchemy.Plugins.Peek.Controls.ResizeAnimation.prototype.resize = function Peek$ResizeAnimation$fadeIn(height, width)
{
	var self = this;
	var p = this.properties;
	var settings = p.animationSettings;

	this.fireEvent("resizing");

	p.heightAnimationComplete = false;
	p.widthAnimationComplete = false;

	var heightAnimation = {
		property: "height",
		to: height + "px",
		duration: settings.duration,
		curve: settings.curve,
		element: p.element,
		callback: function()
		{
			p.heightAnimationComplete = true;
			self._checkComplete();
		}
	};

	var widthAnimation = {
		property: "width",
		to: width + "px",
		duration: settings.duration,
		curve: settings.curve,
		element: p.element,
		callback: function()
		{
			p.widthAnimationComplete = true;
			self._checkComplete();
		}
	};

	p.animations.push($animation.add(heightAnimation));
	p.animations.push($animation.add(widthAnimation));
};

Alchemy.Plugins.Peek.Controls.ResizeAnimation.prototype.stop = function Peek$ResizeAnimation$stop()
{
	var p = this.properties;

	for (var i = 0, j = p.animations.length; i < j; i++)
	{
		$animation.stop(p.animations[i]);
	}
	p.animations = [];
};

Alchemy.Plugins.Peek.Controls.ResizeAnimation.prototype._checkComplete = function Peek$ResizeAnimation$_checkComplete()
{
	var p = this.properties;
	if (p.heightAnimationComplete && p.widthAnimationComplete)
	{
		this.stop();
		this.fireEvent("resized");
	}
};