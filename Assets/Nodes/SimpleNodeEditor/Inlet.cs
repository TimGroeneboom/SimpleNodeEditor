using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    [System.Serializable]
    public class Inlet : Let
    {
        public SignalHandler SlotReceivedSignal = (Signal signal) => { };

        public void Slot(Signal signal)
        {
            SlotReceivedSignal(signal);
        }

        override public void Construct(BaseNode owner)
        {
            Owner = owner;

            Offset = new Rect(-5, 24, 10, 10);
            m_type = LetTypes.INLET;

            Name = "Inlet";
        }

        override public void Construct(BaseNode owner, Rect offset)
        {
            Owner = owner;

            m_type = LetTypes.INLET;
            Offset = offset;

            Name = "Inlet";
        }

        void OnDestroy()
        {
            SlotReceivedSignal = null;
        }
    }
}
