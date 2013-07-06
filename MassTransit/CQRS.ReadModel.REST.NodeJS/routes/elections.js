var mongo = require('mongodb');

var Server = mongo.Server,
	Db = mongo.Db,
	BSON = mongo.BSONPure;

var server = new Server('localhost', 27017, {auto_reconnect: true});
db = new Db('wf1_read_model', server);

db.open(function(err, db){
	if(!err) {
		console.log("Connected to the 'wf1_read_model' database.");
		db.collection('Elections', {safe:true}, function(err, collection){
			if(err) {
				console.log("The 'Elections' collection does not exist.");
				db.createCollection('Elections', function(err, collection) {
					if(err) {
						console.error("Mongo error while creating the 'Elections' collection.");
					}
				});
			}
		});
	}
});

exports.findAll = function(req, res){
	db.collection('Elections', function(err, collection) {
		collection.find().toArray(function(err, items) {
			res.send(items);
		});
	});
};

exports.findById = function(req, res){
	var id = req.params.id;
	db.collection('Elections', function(err, collection) {
		collection.findOne({'_id':id}, function(err, item) {
			res.send(item);
		});
	});
};

exports.findBalanceById = function(req, res){
	var id = req.params.id;
	db.collection('ElectionBalances', function(err, collection) {
		collection.findOne({'ElectionId':id}, function(err, item) {
			res.send(item);
		});
	});
};

exports.findByParticipant = function(req, res) {
	var participantid = req.params.id;
	db.collection('Elections', function(err, collection) {
		collection.find({'ParticipantId':participantid}).toArray(function(err, items) {
			res.send(items);
		});
	});
};