using QFSW.QC;

using Bool = System.Boolean;

namespace Bonkers
{
    public static class VerboseInputLogging
    {
        [Command]
        public static Bool VerboseInputLoggingEnabled { get; set; } = false;
    }
}
