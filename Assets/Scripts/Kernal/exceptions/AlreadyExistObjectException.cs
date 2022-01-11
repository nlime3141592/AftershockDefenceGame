using System;

namespace DefenceGameSystem.OS.Kernel
{
    public class AlreadyExistObjectException : Exception
    {
        public AlreadyExistObjectException(string message)
        : base(message)
        {

        }
    }
}