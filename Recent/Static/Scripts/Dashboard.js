function Recent$onDisplayStarted() 
{
	$evt.removeEventHandler($display, "start", Recent$onDisplayStarted);

	var favoritesTree = $("#FavoritesTree");
	if (!favoritesTree) return;

	var treeControl = $controls.getControl(favoritesTree, "Tridion.Controls.Tree");
	if (!treeControl) return;


};

$evt.addEventHandler($display, "start", Recent$onDisplayStarted);
