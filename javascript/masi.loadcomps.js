var count = 0;

function loadComposition(){
	var d = new Dict('masi.composition');
	var p = this.patcher;
	var keys = d.getkeys();
	if(typeof(keys) == "string"){
		var names = d.get(keys);
		var source = p.newdefault(0, count * 60, keys);
		if (typeof(names) == "string"){
			var encoder = p.newdefault(0, count * 60 + 30, "masi.encoder~", "@name", names);
			p.connect(source, 0, encoder, 0);
		}
		
		else if (typeof(names) == "object"){
			for (var j = 0; j < names.length; j++){
				var encoder = p.newdefault(j * 175, count * 60 + 30, "masi.encoder~", "@name", names[j]);
				p.connect(source, j, encoder, 0);
			}
		}
		count++;
	}
	
	else if (typeof(keys) == "object"){
		for (var i = 0; i < keys.length; i++){
			var names = d.get(keys[i]);
			var source = p.newdefault(0, count * 60, keys[i]);
			if (typeof(names) == "string"){
				var encoder = p.newdefault(0, count * 60 + 30, "masi.encoder~", "@name", names);
				p.connect(source, 0, encoder, 0);
			}
		
			else if (typeof(names) == "object"){
				for (var j = 0; j < names.length; j++){
					var encoder = p.newdefault(j * 175, count * 60 + 30, "masi.encoder~", "@name", names[j]);
					p.connect(source, j, encoder, 0);
				}
			}
			count++;
		}
	}
}

function unloadComposition(){
	this.patcher.applyif(destroy, test);
	count = 0;
}

function destroy(a){
	this.patcher.remove(a);
}

function test(a){
	if (a.maxclass == "patcher"){
		return 1;
	}
	
	else{
		return 0;
	}
}