var scrollSpeed = 0.90;
 
var scrollSpeed2 = 0.90;

var scrollEnabled = true;
 
function FixedUpdate(){
	if(scrollEnabled){
		
		var offset = Time.time * scrollSpeed;
		 
		var offset2 = Time.time * scrollSpeed2;
		 
		renderer.material.mainTextureOffset = Vector2 (offset2,-offset);
	} 
}

function scrollOff(){
	scrollEnabled = false;
}

function scrollOn(){
	scrollEnabled = true;
}