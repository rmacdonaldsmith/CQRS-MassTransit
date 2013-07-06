/**
 * Created with JetBrains WebStorm.
 * User: rmsmith
 * Date: 2/21/13
 * Time: 9:40 AM
 * To change this template use File | Settings | File Templates.
 */
var mongo = require('../mongo.js')

exports.findAll = function(req, res) {
    mongo.getCollection("ClaimTypes", function(collection, err){
        if(!err) {
            collection.find().toArray(function(err, items){
                res.send(items);
            });
        }
        else {
            console.log("Error attempting to get the ClaimTypes collection:");
            console.log(err);
        }
    });
};