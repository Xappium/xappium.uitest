using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Xappium.UITest
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Serializable]

#pragma warning disable CA1032, RCS1194 // AssertionFailedException should never be constructed with an empty message
    public class AssertionFailedException : Exception
#pragma warning restore CA1032, RCS1194
    {
        public AssertionFailedException(string message)
            : base(message)
        {
        }

        protected AssertionFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
