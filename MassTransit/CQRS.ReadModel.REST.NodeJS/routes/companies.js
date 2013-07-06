/**
 * Created with JetBrains WebStorm.
 * User: rmsmith
 * Date: 1/22/13
 * Time: 4:39 PM
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
        db.collection('Companies', {safe:true}, function(err, collection){
            if(err) {
                console.log("The 'Companies' collection does not exist.");
            }
        });
    }
});

exports.findAll = function(req, res){
    db.collection('Companies', function(err, collection) {
        collection.find().toArray(function(err, items) {
            res.send(items);
        });
    });
};

exports.findById = function(req, res){
    var id = req.params.id;
    db.collection('Companies', function(err, collection) {
        collection.findOne({'_id':id}, function(err, item) {
            res.send(item);
        });
    });
};

exports.findAllPlans = function(req, res){
    var companyId = req.params.id;
    db.collection('Plans', function(err, collection){
        collection.find({'CompanyId':companyId}).toArray(function(err, items){
            res.send(items);
        });
    });
};

exports.findAllPlanYears = function(req, res){
    var companyId = req.params.id;
    db.collection('PlanYears', function(err, collection){
        collection.find({'CompanyId':companyId}).toArray(function(err, items){
            res.send(items);
        });
    });
};

exports.findPlanYearsByPlanId = function(req, res){
    var planId = req.params.id;
    db.collection('PlanYears', function(err, collection){
        collection.find({'PlanId':planId}).toArray(function(err, items){
            res.send(items);
        });
    });
};

exports.findPlanYearBenefits = function(req,res){
    var companyId = req.params.id;
    db.collection('PlanYearBenefits', function(err, collection){
        collection.find({'CompanyId':companyId}).toArray(function(err, items){
            res.send(items);
        });
    });
};

exports.findAllClaimTypes = function(req, res){
    db.collection('ClaimTypes', function(err, collection) {
        collection.find().toArray(function(err, items) {
            res.send(items);
        });
    });
};