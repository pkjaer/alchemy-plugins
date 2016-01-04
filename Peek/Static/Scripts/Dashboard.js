// Initialize when the display loads
$evt.addEventHandler($display, "start", Peek$onDisplayStarted);

function Peek$onDisplayStarted() 
{
    $evt.removeEventHandler($display, "start", Peek$onDisplayStarted);

	var listContainer = $("#list_container");
	if (listContainer)
	{
		var frame = document.createElement("iframe");
		frame.id = "PeekArea";
		frame.className = "peek-frame";
		frame.src = "${ViewsUrl}Frame.aspx";
		listContainer.appendChild(frame);
	}
};