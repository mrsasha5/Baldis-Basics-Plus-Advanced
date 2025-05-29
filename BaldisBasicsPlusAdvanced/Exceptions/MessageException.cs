using System;
namespace BaldisBasicsPlusAdvanced.Exceptions
{
    public class MessageException : Exception
    {
        public override string StackTrace => stackTrace;

        private string stackTrace = "";

        public MessageException(string message) : base(message)
        {
        }
    }
}
