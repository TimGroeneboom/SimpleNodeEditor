using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    public class Outlet : Let
    {
        public Outlet(SimpleNode owner)
            : base(owner)
        {
            Offset = new Rect(owner.Size.x - 5, 24, 10, 10);
            m_type = LetTypes.OUTLET;

            Name = "Outlet";
        }

        public Outlet(SimpleNode owner, Rect offset)
            : base(owner)
        {
            m_type = LetTypes.OUTLET;
            Offset = offset;

            Name = "Outlet";
        }
    }
}
