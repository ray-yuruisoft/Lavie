var flashvars = {
	MemberName: typeof(memberName)!= 'undefined'?memberName:"",
	MemberPassword: typeof(memberPassword)!= 'undefined'?memberPassword:""
};

var params = {
	menu: "false",
	scale: "noScale",
	allowFullscreen: "true",
	allowScriptAccess: "always",
	bgcolor: "#000000",
	wmode: "direct" // can cause issues with FP settings & webcam
};
var attributes = {
	id: "Player"
};
swfobject.embedSWF(
			"Player.swf",
			//"http://www.tomybb.com/MiniPlayer/Player.swf",
			"altContent", "100%", "100%", "10.0.0",
			"",
			flashvars, params, attributes);


		function getPlayer(movieName) {
			if (window.document[movieName]) {
				return window.document[movieName];
			}
			if (navigator.appName.indexOf("Microsoft Internet") == -1) {
				if (document.embeds && document.embeds[movieName])
					return document.embeds[movieName];
			}
			else {
				return document.getElementById(movieName);
			}
		}

                var isReady = false;
	        function playerIsReady() {
	           isReady = true;
	        }
		function playWithName(memberName, memberPassword) {
			getPlayer("Player").playWithName(memberName, memberPassword);
		}
		function playWithKey(memberKey)
		{
			getPlayer("Player").playWithKey(memberKey);
		}