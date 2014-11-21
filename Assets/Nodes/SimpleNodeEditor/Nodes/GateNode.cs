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
        Outlet outlet = null;
        [SerializeField]
        Inlet inlet1 = null;
        [SerializeField]
        Inlet inlet2 = null;

        public bool Value = false;

        void OnInlet1Received(Signal signal)
        {
            if(Value)
            {
                outlet.Send(signal.Args);
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
            inlet1.SlotReceivedSignal += OnInlet1Received;
            inlet2.SlotReceivedSignal += OnInlet2Received;
        }

        public override void Construct()
        {
            Name = "GateNode";

            inlet1 = (Inlet)MakeLet(LetTypes.INLET);
            inlet2 = (Inlet)MakeLet(LetTypes.INLET);
            inlet2.yOffset = 25;

            outlet = (Outlet)MakeLet(LetTypes.OUTLET);
            outlet.yOffset = 50;

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
