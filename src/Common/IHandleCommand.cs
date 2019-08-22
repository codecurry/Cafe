using System.Collections;

namespace Common
{
    public interface IHandleCommand<TCommand>
    {
        IEnumerable Handle(TCommand c);
    }
}
