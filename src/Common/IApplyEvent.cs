using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// Implemented by an aggregate once for each event type it can apply.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IApplyEvent<T>
    {
        void Apply(T ev);
    }
}
