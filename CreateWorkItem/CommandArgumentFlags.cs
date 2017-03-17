namespace CreateWorkItem
{
    public class CommandArgumentFlags {
        public const uint None = 0x00000000;
        public const uint TakesParameter = 0x00000001;
        public const uint Required = 0x00000002;
        public const uint HideInUsage = 0x00000004;

        public static bool FlagEnabled(uint f0, uint f1) {
            return (f0 & f1) != 0;
        }

        public static bool FlagDisabled(uint f0, uint f1) {
            return (f0 & f1) == 0;
        }
    }
}