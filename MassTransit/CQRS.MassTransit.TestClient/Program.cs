using System;
using CQRS.Commands;
using MassTransit;
using log4net.Config;

namespace CQRS.MassTransit.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            var queueName = "rabbitmq://localhost/commandqueue";
            var bus = ServiceBusFactory.New(configurator =>
                               {
                                   configurator.UseRabbitMq();
                                   configurator.ReceiveFrom(queueName);
                               });

            Console.WriteLine("Publishing message...");

            //bus.Publish(new MakeAnElection
            //                                {
            //                                    AdministratorCode = "admcode",
            //                                    CompanyCode = "cocode",
            //                                    EmployeeNumber = "empnum",
            //                                    BenefitCode = "benef",
            //                                    PlanCode = 12,
            //                                    PlanYear = 2012,
            //                                    ElectionAmount = 1200,
            //                                    ElectionReason = "reason",
            //                                    PerPayPeriodAmount = 100,
            //                                });

            //bus.Publish(new TerminateElection());

            bus.PublishRequest(new TerminateElection
                                   {
                                       ElectionId = Guid.NewGuid().ToString(),
                                   },
                               configurator =>
                                   {
                                       configurator.Handle<CommandResponse>(
                                           response =>
                                               {
                                                   if (response.CommandStatus == CommandStatusEnum.Succeeded)
                                                   {
                                                       Console.WriteLine("Command succeeded!");
                                                   }
                                                   else
                                                   {
                                                       Console.WriteLine(string.Format("Command failed: {0}",
                                                                                       response.ExceptionDetail));
                                                   }
                                               });

                                       configurator.HandleTimeout(new TimeSpan(0, 0, 0, 5), _ =>
                                                                                            Console.WriteLine(
                                                                                                "Timeout expired while waiting for a response to the command."));
                                   });

            Console.WriteLine("Message published. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
