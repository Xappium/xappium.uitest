﻿using System.Collections.Generic;

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

        public bool IsErred => !string.IsNullOrEmpty(Error.Trim());
    }
}