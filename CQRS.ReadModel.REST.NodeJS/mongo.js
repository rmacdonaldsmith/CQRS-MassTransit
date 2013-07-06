"use strict";
var CONFIG = require('./config.json').connection,
	mongodb = require('mongodb'),
	Db = mongodb.Db,
	Connection = Db.Connection,
	Server = mongodb.Server,
	domain = require('domain');
	
var mongoDomain = domain.create(),
	intercept = mongoDomain.intercept.bind(mongoDomain);

mongoDomain.on('error', function(er) {
	console.error("Mongo error!", er);
});

mongoDomain.run(function(){
	var db = new Db(CONFIG.dbName, new Server(CONFIG.host, CONFIG.port, {safe:true}));

	function getCollectionInternal(collection, cb) {
		//use the intercept to allow us to catch errors within the current domain
		db.collection(collection, intercept(cb));
	};

});

exports.getCollection = function(collection, cb) {
	getCollectionInternal;
};