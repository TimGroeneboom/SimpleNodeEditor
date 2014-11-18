using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    public class Connection
    {
        public Connection(Inlet inlet, Outlet outlet)
        {
            m_inlet = inlet;
            m_outlet = outlet;
        }

        protected Inlet m_inlet = null;
        public Inlet Inlet{ get{ return m_inlet; } }

        protected Outlet m_outlet = null;
        public Outlet Outlet { get { return m_outlet; } }
    }
}
