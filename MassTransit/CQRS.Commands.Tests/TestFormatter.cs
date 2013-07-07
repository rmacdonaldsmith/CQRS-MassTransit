using System;
using CQRS.DomainTesting;
using CQRS.Interfaces.Commands;

namespace CQRS.Domain.Tests
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
