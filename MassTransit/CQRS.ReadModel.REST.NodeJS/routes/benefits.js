var mongo = require('mongodb');

var Server = mongo.Server,
    Db = mongo.Db,
    BSON = mongo.BSONPure;

var server = new Server('localhost', 27017, {auto_reconnect: true});
db = new Db('wf1_read_model', server);

db.open(function(err, db){
    if(!err) {
        console.log("Connected to the 'wf1_read_model' database.");
        db.collection('Benefits', {safe:true}, function(err, collection){
            if(err) {
                console.log("The 'Benefits' collection does not exist.");
                db.createCollection('Benefits', function(err, collection) {
                    if(err) {
                        console.error("Mongo error while creating the 'Benefits' collection.");
                    }
                });
            }
        });
    }
});

exports.findAll = function(req, res){
    db.collection('Benefits', function(err, collection) {
        collection.find().toArray(function(err, items) {
            res.send(items);
        });
    });
};

exports.findById = function(req, res){
    var id = req.params.id;
    db.collection('Benefits', function(err, collection) {
        collection.findOne({'_id':id}, function(err, item) {
            res.send(item);
        });
    });
};

exports.findByYear = function(req,res){
    var year = req.params.year;
};

exports.findByCompany = function(req, res){
    var companyCode = req.params.companyid;
};