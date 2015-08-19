using System;

namespace DeterministicLockstepNetworking
{
    public sealed class CommandFrame
    {
        public uint Ticks { get; private set; }

        public Command[] Commands { get; private set; }

        internal CommandFrame(uint ticks, Command[] commands)
        {
            Ticks = ticks;
            Commands = commands;
        }
    }
}