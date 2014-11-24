using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("GateNode", typeof(GateNode))]
    public class GateNode : BaseNode
    {
        [SerializeField]
        Outlet m_outlet = null;
        [SerializeField]
        Inlet m_inlet1 = null;
        [SerializeField]
        Inlet m_inlet2 = null;

        public bool Value = false;

        void OnInlet1Received(Signal signal)
        {
            if(Value)
            {
                m_outlet.Send(signal.Args);
            }
        }

        void OnInlet2Received(Signal signal)
        {
            bool val = false;
            if (Signal.TryParseBool(signal.Args, out val))
            {
                Value = val;
            }  
        }

        protected override void Inited()
        {
            m_inlet1.SlotReceivedSignal += OnInlet1Received;
            m_inlet2.SlotReceivedSignal += OnInlet2Received;
        }

        public override void Construct()
        {
            Name = "GateNode";

            m_inlet1 = MakeLet<Inlet>("Inlet 1");
            m_inlet2 = MakeLet<Inlet>("Inlet 2", 25);
            m_outlet = MakeLet<Outlet>("Outlet", 50);

            Size = new Vector2(Size.x, 125);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 75, 100, 50));
            EditorGUIUtility.LookLikeControls(30, 30);

            Value = GUILayout.Toggle(Value, "Is open");

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }
}
