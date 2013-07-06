/**
 * Created with JetBrains WebStorm.
 * User: rmsmith
 * Date: 2/21/13
 * Time: 11:06 AM
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
        db.collection('ClaimTypes', {safe:true}, function(err, collection){
            if(err) {
                console.log("The 'ClaimTypes' collection does not exist.");
            }
        });
    }
});

exports.findAll = function(req, res){
    db.collection('ClaimTypes', function(err, collection) {
        collection.find({}, {"_id":0, "ClaimType":1}).toArray(function(err, items) {
            res.send(items);
        });
    });
};