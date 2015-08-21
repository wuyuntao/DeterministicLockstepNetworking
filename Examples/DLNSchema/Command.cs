using DeterministicLockstepNetworking;

namespace DLNSchema
{
    public sealed class Command : ICommand
    {
        public uint CommandId { get; private set; }

        public uint SessionId { get; private set; }

        public Command(uint commandId, uint sessionId)
        {
            CommandId = commandId;
            SessionId = sessionId;
        }
    }

    public sealed class CommandFrame
    {
        public uint Ticks { get; private set; }

        public Command[] Commands { get; private set; }

        public CommandFrame(uint ticks, Command[] commands)
        {
            Ticks = ticks;
            Commands = commands;
        }
    }
}
