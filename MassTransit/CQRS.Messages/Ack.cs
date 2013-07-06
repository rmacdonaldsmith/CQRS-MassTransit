namespace MHM.WinFlexOne.CQRS.Messages
{
    public class Ack
    {
        public Ack(bool succeeded)
        {
            Succeeded = succeeded;
        }

        public bool Succeeded { get; set; }
    }
}