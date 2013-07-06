using System.Configuration;
using System.Linq;
using MHM.WinFlexOne.CQRS.Dtos;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Nancy;

namespace CQRS.ReadModel.REST.Modules
{
    public class ElectionsModule : NancyModule
    {
        private readonly MongoDatabase m_mongoDataBase;

        public ElectionsModule()
            : base("/elections")
        {
            //setup Mongo
            m_mongoDataBase = MongoServer
                .Create(ConfigurationManager.ConnectionStrings["mongoDbConnectionString"].ConnectionString)
                .GetDatabase("wf1_read_model", SafeMode.False);

            //configure routes

            //return all elections
            Get["/"] = parameters => m_mongoDataBase.GetCollection<ElectionDto>("Elections");

            //return specific election
            Get["/{electionid}"] = parameters =>
                                      {
                                          //todo: content negotiation.
                                          var electionCollection = m_mongoDataBase.GetCollection<ElectionDto>("Elections");
                                          string electionId = parameters.electionid;
                                          var query = Query<ElectionDto>.EQ(election => election.Id, electionId);
                                          return electionCollection.Find(query).FirstOrDefault();
                                      };

            //return all elections for a participant
            Get["/forparticipant/{participantid}"] = parameters =>
                             {
                                 var electionCollection = m_mongoDataBase.GetCollection<ElectionDto>("Elections");
                                 string participantId = parameters.participantid;
                                 var query = Query<ElectionDto>.EQ(election => election.ParticipantId, participantId);
                                 return electionCollection.Find(query).ToList();
                             };
        }
    }
}