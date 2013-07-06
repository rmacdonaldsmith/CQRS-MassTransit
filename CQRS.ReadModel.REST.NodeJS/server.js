var express 	= require('express'), 
	index 		= require('./routes/index'), 
	elections 	= require('./routes/elections');

var app = express();

app.configure(function(){
	app.use(express.logger('dev'));
	app.use(express.bodyParser());
});

app.get('/', index.index);
app.get('/elections', elections.findAll);
app.get('/elections/:id', elections.findById);
app.get('/elections/forparticipant/:id', elections.findByParticipant);

app.listen(3000);

console.log('Server running at http://127.0.0.1:3000/');