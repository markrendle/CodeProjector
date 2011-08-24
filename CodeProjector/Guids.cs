// Guids.cs
// MUST match guids.h
using System;

namespace MarkRendle.CodeProjector
{
    static class GuidList
    {
        public const string guidCodeProjectorPkgString = "134db713-1ece-4746-afbd-08043e3c8bb7";
        public const string guidCodeProjectorCmdSetString = "38196c18-7f52-4bc7-8fba-84ca7465a9ac";

        public static readonly Guid guidCodeProjectorCmdSet = new Guid(guidCodeProjectorCmdSetString);
    };
}