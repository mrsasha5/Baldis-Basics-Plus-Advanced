using System;
using System.Collections.Generic;
using System.Text;

namespace BaldisBasicsPlusAdvanced.Exceptions
{
    public class MessageException : Exception
    {
        public override string StackTrace => stackTrace;

        public override string Message => message;

        private string message = "";

        private string stackTrace = "";

        public MessageException(string message) {
            this.message = message;
        }
    }
}
