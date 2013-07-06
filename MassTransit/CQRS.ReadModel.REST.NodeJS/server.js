var express 	= require('express'), 
	index 		= require('./routes/index'), 
	elections 	= require('./routes/elections');
    companies   = require('./routes/companies');
    benefits    = require('./routes/benefits');
    claimTypes  = require('./routes/claimTypes');
    claims      = require('./routes/claims');

var app = express();

app.configure(function(){
	app.use(express.logger('dev'));
	app.use(express.bodyParser());
});

app.get('/', index.index);
app.get('/elections', elections.findAll);
app.get('/elections/:id', elections.findById);
app.get('/elections/:id/balance', elections.findBalanceById);
app.get('/elections/forparticipant/:id', elections.findByParticipant);
app.get('/companies', companies.findAll);
app.get('/companies/:id', companies.findById);
app.get('/companies/:id/plans', companies.findAllPlans);
app.get('/companies/plans/:id', companies.findPlanById);
app.get('/companies/plans/:id/planyears', companies.findPlanYearsByPlanId);
app.get('/companies/:id/planyears', companies.findAllPlanYears);
app.get('/companies/:id/planyears/:id', companies.findPlanYearByYear);
app.get('/companies/:id/planyearbenefits/', companies.findPlanYearBenefits);
app.get('/companies/planyears/:id', companies.findPlanYearById);
//app.get('/companies/claimtypes', companies.findAllClaimTypes);
app.get('/benefits', benefits.findAll);
app.get('/benefits/:id', benefits.findById);
app.get('/benefits/:year', benefits.findByYear);
app.get('/claimtypes', claimTypes.findAll);
app.get('/claims', claims.findAll);
app.get('/claims/:id', claims.findById);
app.get('/claims/forparticipant/:id', claims.findByParticipant);

app.listen(3000);

console.log('Server running at http://127.0.0.1:3000/');