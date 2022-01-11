using System;

namespace DefenceGameSystem.OS.Kernel
{
    public class NotExistObjectException : Exception
    {
        public NotExistObjectException(string message)
        : base(message)
        {

        }
    }
}