using System;

namespace DeterministicLockstepNetworking
{
    public interface ICommand
    {
        uint SessionId { get;  }
    }
}