/**
 * Created with JetBrains WebStorm.
 * User: rmsmith
 * Date: 2/21/13
 * Time: 1:55 PM
 * To change this template use File | Settings | File Templates.
 */
var mongo = require('mongodb');

var Server = mongo.Server,
    Db = mongo.Db,
    BSON = mongo.BSONPure;

var server = new Server('localhost', 27017, {auto_reconnect: true});
db = new Db('wf1_read_model', server);

db.open(function(err, db){
    if(!err) {
        console.log("Connected to the 'wf1_read_model' database.");
        db.collection('Claims', {safe:true}, function(err, collection){
            if(err) {
                console.log("The 'Claims' collection does not exist.");
            }
        });
    }
});

exports.findAll = function(req, res){
    db.collection('Claims', function(err, collection) {
        collection.find().toArray(function(err, items) {
            res.send(items);
        });
    });
};

exports.findById = function(req, res) {
    var claimId = req.params.id;
    db.collection('Claims', function(err, collection) {
        collection.findOne({'_id':claimId}, function (err, item) {
            res.send(item);
        });
    });
};

exports.findByParticipant = function(req, res){
    var participantId = req.params.id;
    db.collection('Claims', function(err, collection) {
        collection.find({'ParticipantId':participantId}).toArray(function(err, items) {
            res.send(items);
        });
    });
};