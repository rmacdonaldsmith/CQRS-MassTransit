using System;
using System.Linq;
using CQRS.UI.Web.Nancy.Models;
using CQRS.UI.Web.Nancy.Services;
using MHM.WinFlexOne.CQRS.Commands;
using MassTransit;
using Nancy;
using Nancy.ModelBinding;

namespace CQRS.UI.Web.Nancy.Modules
{
    public class ElectionModule : NancyModule
    {
        private const string Participantid = "empnum";
        //private readonly IDispatchCommandsAndWaitForResponse m_commandDispatcher;

        public ElectionModule(IReadModelFacade readModel) 
            : base("/elections")
        {
            //initialize stuff
            //m_commandDispatcher = commandDispatcher;
            var bus = ServiceBusFactory.New(configurator =>
            {
                configurator.UseRabbitMq();
                configurator.ReceiveFrom("rabbitmq://localhost/command_request_queue");
            });

            //define routes
            Get["/"] = parameters =>
                                    {
                                        //get non-terminated elections for this participant from our read model
                                        //note: we can filter (only get un-terminated elections) on the read model side so that we do not send back
                                        //a load of election records that we discard here
                                        var elections = readModel.GetElectionsForParticipant(Participantid)
                                            .Where(election => election.IsTerminated == false)
                                            .ToList();

                                        var electionsModel = new ElectionsModel
                                                                 {
                                                                     Elections = elections,
                                                                     ShowElectionDetail = false,
                                                                     SelectedElection = null,
                                                                 };

                                        return View["Elections", electionsModel];
                                    };

            Get["/{electionid}"] = parameters =>
                                       {

                                           var elections = readModel.GetElectionsForParticipant(Participantid)
                                               .Where(election => election.IsTerminated == false)
                                               .ToList();
                                           var selectedElection =
                                               elections.FirstOrDefault(election => election.Id == parameters.electionid);

                                           var electionsModel = new ElectionsModel
                                           {
                                               Elections = elections,
                                               ShowElectionDetail = selectedElection != null,
                                               SelectedElection = selectedElection,
                                           };

                                           return View["Elections", electionsModel];
                                       };

            Get["/newelection"] = paramerters => View["NewElection"];

            Post["/newelection"] = parameters =>
                            {
                                MakeAnElection command = this.Bind();
                                command.PlanYearBenefitId = Guid.NewGuid().ToString();
                                command.Id = Guid.NewGuid().ToString();
                                bool commandFailed = false;
                                var model = new ErrorModel { LinkAddress = "/elections/newelection" };
                                bus.PublishRequest(command,
                                                   configurator =>
                                                       {
                                                           configurator.Handle<CommandResponse>(
                                                               cmdResponse =>
                                                                   {
                                                                       if (cmdResponse.CommandStatus !=
                                                                           CommandStatusEnum.Succeeded)
                                                                       {
                                                                           model.HasError = true;
                                                                           model.ErrorMessage = cmdResponse.Message;
                                                                           commandFailed = true;
                                                                       }
                                                                   });
                                                           configurator.HandleTimeout(TimeSpan.FromSeconds(30),
                                                                () =>
                                                                    {
                                                                        model.HasError = true;
                                                                        model.ErrorMessage = "Your request timed out.";
                                                                        commandFailed = true;
                                                                    });
                                                       });

                                //m_commandDispatcher.Dispatch(command,
                                //                             cmdResponse =>
                                //                                 {
                                //                                     if (cmdResponse.CommandStatus !=
                                //                                         CommandStatusEnum.Succeeded)
                                //                                     {
                                //                                         model.HasError = true;
                                //                                         model.ErrorMessage = cmdResponse.Message;
                                //                                         commandFailed = true;
                                //                                     }
                                //                                 },
                                //                             () =>
                                //                                 {
                                //                                     model.HasError = true;
                                //                                     model.ErrorMessage = "Your request timed out.";
                                //                                     commandFailed = true;
                                //                                 });

                                
                                if (commandFailed)
                                    return View["/errorview", model];

                                return Response.AsRedirect("/elections"); //why cant i just return a View here? Why redirect?
                            };

            Get["/terminate/{electionid}"] = parameters =>
                                                 {
                                                     var terminateCommand = new TerminateElection
                                                                                {
                                                                                    ElectionId = parameters.electionid,
                                                                                    TerminationDate = DateTime.Now,
                                                                                };
                                                     bool commandFailed = false;
                                                     var model = new ErrorModel{LinkAddress = "/elections"};

                                                     bus.PublishRequest(terminateCommand,
                                                        configurator =>
                                                            {
                                                                configurator.Handle<CommandResponse>(
                                                                    cmdResponse =>
                                                                        {
                                                                            if (cmdResponse.CommandStatus != CommandStatusEnum.Succeeded)
                                                                            {
                                                                                model.HasError = true;
                                                                                model.ErrorMessage = cmdResponse.Message;
                                                                                commandFailed = true;
                                                                            }
                                                                        });
                                                                configurator.HandleTimeout(
                                                                    TimeSpan.FromSeconds(30),
                                                                    () =>
                                                                        {
                                                                            model.HasError = true;
                                                                            model.ErrorMessage = "Your request timed out.";
                                                                            commandFailed = true;
                                                                        });
                                                            });

                                                     //m_commandDispatcher.Dispatch(terminateCommand, 
                                                     //    response =>
                                                     //        {
                                                     //            if (response.CommandStatus != CommandStatusEnum.Succeeded)
                                                     //            {
                                                     //                model.HasError = true;
                                                     //                model.ErrorMessage = response.Message;
                                                     //                commandFailed = true;
                                                     //            }
                                                     //        }, 
                                                     //    () =>
                                                     //        {
                                                     //            model.HasError = true;
                                                     //            model.ErrorMessage = "Your request timed out.";
                                                     //            commandFailed = true;
                                                     //        });

                                                     //return Response.AsRedirect("/elections");
                                                     
                                                     if (commandFailed)
                                                         return View["/errorview", model];

                                                     return Response.AsRedirect("/elections"); //why cant i just return a View here? Why redirect?
                                                 };
        }
    }
}