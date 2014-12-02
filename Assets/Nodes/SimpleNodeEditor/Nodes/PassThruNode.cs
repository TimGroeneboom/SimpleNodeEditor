using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    public delegate void PassThuSignalReceivedEventHandler(Signal signal);

    [NodeMenuItem("PassThruNode", typeof(PassThruNode))]
    public class PassThruNode
        : BaseNode
    {
        public PassThuSignalReceivedEventHandler OnSignalReceived = (Signal signal)=>{};

        [SerializeField]
        Inlet m_inlet = null;
        public Inlet Inlet { get { return m_inlet; } }

        [SerializeField]
        Outlet m_outlet = null;
        public Outlet Outlet { get { return m_outlet; } }

        void OnInletReceived(Signal signal)
        {
            // pass on the signal
            m_outlet.Send(signal);

            OnSignalReceived(signal);
        }

        public void SendSignal(Signal signal)
        {
            m_outlet.Send(signal);
        }

        public override void Construct()
        {
            Name = "Pass Thru Node";

            m_inlet = MakeLet<Inlet>("Inlet");
            m_outlet = MakeLet<Outlet>("Outlet", 25);

            Size = new Vector2(Size.x, 80);
        }

        protected override void Inited()
        {
            m_inlet.SlotReceivedSignal += OnInletReceived;
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            base.WindowCallback(id);
        }
#endif
    }
}


