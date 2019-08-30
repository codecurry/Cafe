using System;
using System.Collections;

namespace Ddd.Common
{
    public interface IEventStore
    {
        IEnumerable LoadEventsFor<T>(Guid id);
        void SaveEventsFor<T>(Guid id, int eventsLoaded, ArrayList newEvents);
    }
}
