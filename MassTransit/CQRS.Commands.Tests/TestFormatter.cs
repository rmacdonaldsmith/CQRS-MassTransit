using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CQRS.DomainTesting;
using MHM.WinFlexOne.CQRS.Interfaces.Commands;

namespace MHM.WinFlexOne.CQRS.Domain.Tests
{
    public class TestFormatter
    {
        public void Print<TCommand>(EventSpecification<TCommand> testSpecification) where TCommand : class, ICommand
        {
            Console.WriteLine("Specification: " + testSpecification.GetType().Name.Replace("_", " "));
            Console.WriteLine();
            Console.WriteLine("Given that: ");
            foreach (var @event in testSpecification.Given())
            {
                Console.WriteLine("\t" + @event);
            }
            Console.WriteLine();
            Console.WriteLine("When " + testSpecification.When());
            Console.WriteLine();
            Console.WriteLine("Expect that:");
            foreach (var @event in testSpecification.Then())
            {
                Console.WriteLine("\t" + @event);
            }
        }
    }
}
