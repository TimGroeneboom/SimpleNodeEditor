using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    [System.Serializable]
    public class Outlet : Let
    {
        public SignalHandler Emit = (Signal signal) => { };

        override public void Construct(BaseNode owner)
        {
            Owner = owner;

            Offset = new Rect(owner.Size.x - 5, 24, 10, 10);
            m_type = LetTypes.OUTLET;

            Name = "Outlet";
        }

        override public void Construct(BaseNode owner, Rect offset)
        {
            Owner = owner;

            m_type = LetTypes.OUTLET;
            Offset = offset;

            Name = "Outlet";
        }
    }
}
