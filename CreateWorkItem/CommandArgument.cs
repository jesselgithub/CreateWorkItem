using System;

namespace CreateWorkItem
{
    class CommandArgument : IEquatable<CommandArgument>
    {
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public uint Flags { get; set; }
        public string ParameterName { get; set; }
        public Action<CommandParser, string> Action { get; set; }

        public bool Equals(CommandArgument other)
        {
            bool equals = false;

            if (this.Name.Equals(other.Name) &&
                this.LongName.Equals(other.LongName) &&
                this.Description.Equals(other.Description) &&
                this.Flags == other.Flags &&
                this.ParameterName.Equals(other.ParameterName))
            {
                equals = true;
            }

            return equals;
        }
    }
}