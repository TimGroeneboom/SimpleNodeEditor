using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    public class Inlet : Let
    {
        public Inlet(SimpleNode owner)
            : base(owner)
        {
            Offset = new Rect(-5, 24, 10, 10);
            m_type = LetTypes.INLET;

            Name = "Inlet";
        }

        public Inlet(SimpleNode owner, Rect offset)
            : base(owner)
        {
            m_type = LetTypes.INLET;
            Offset = offset;

            Name = "Inlet";
        }
    }
}
