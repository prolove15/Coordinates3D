using UnityEngine;

namespace EazyCamera.Events
{
    public abstract class EventData { }

    public class EnterFocusRangeData : EventData
    {
        public ITargetable Target;

        public EnterFocusRangeData(ITargetable target)
        {
            Target = target;
        }
    }

    public class ExitFocusRangeData : EventData
    {
        public ITargetable Target;

        public ExitFocusRangeData(ITargetable target)
        {
            Target = target;
        }
    }
}
