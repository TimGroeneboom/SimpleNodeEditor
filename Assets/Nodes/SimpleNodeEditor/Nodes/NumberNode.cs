using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("NumberNode", typeof(NumberNode))]
    public class NumberNode : BaseNode
    {
        [SerializeField]
        Inlet m_trigger = null;
        [SerializeField]
        Inlet m_setter = null;

        [SerializeField]
        Outlet m_outlet = null;

        public float Value = 0.0f;

        protected override void Inited()
        {
            m_trigger.SlotReceivedSignal += OnInletReceived;
            m_setter.SlotReceivedSignal += OnSetterReceived;
        }

        void OnInletReceived(Signal signal)
        {
            if(signal.Args.Type==SignalTypes.FLOAT)
            {
                Value = ((SignalFloatArgs)signal.Args).Value;
            }

            m_outlet.Send(new SignalFloatArgs(Value));
        }

        void OnSetterReceived(Signal signal)
        {
            if (signal.Args.Type == SignalTypes.FLOAT)
            {
                Value = ((SignalFloatArgs)signal.Args).Value;
            }else
            {
                float val = 0.0f;
                if( Signal.TryParseFloat(signal.Args, out val))
                {
                    Value = val;
                }
            }
        }

        public override void Construct()
        {
            Name = "NumberNode";

            m_trigger = MakeLet<Inlet>("Trigger");
            m_setter = MakeLet<Inlet>("Set", 25);
            m_outlet = MakeLet<Outlet>("Outlet", 50);

            Size = new Vector2(Size.x, 125);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 75, 100, 75));
            //EditorGUIUtility.LookLikeControls(30, 30);

            Value = EditorGUILayout.FloatField(Value, GUILayout.MaxWidth(80));

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
