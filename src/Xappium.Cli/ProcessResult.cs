using System;
using System.Collections.Generic;
using System.Linq;

namespace Xappium
{
    internal class ProcessResult
    {
        public ProcessResult(IEnumerable<string> output, string error)
        {
            Output = output;
            Error = error;
        }

        public IEnumerable<string> Output { get; }

        public string Error { get; }

        public bool IsErred => !string.IsNullOrEmpty(Error.Trim()) &&
            Error.Split(Environment.NewLine).Any(x => !x.StartsWith("Warning", StringComparison.InvariantCultureIgnoreCase));
    }
}
