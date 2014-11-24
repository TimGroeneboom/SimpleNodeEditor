using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("PrintNode", typeof(PrintNode))]
    public class PrintNode : BaseNode
    {
        [SerializeField]
        Inlet trigger = null;

        [SerializeField]
        Inlet m_input = null;

        [SerializeField]
        Inlet m_setter = null;

        public string Value = "Hello World!";

        protected override void Inited()
        {
            m_input.SlotReceivedSignal += OnInputReceived;
        }

        public override void Construct()
        {
            Name = "PrintNode";

            m_input = MakeLet<Inlet>("Input");
            m_setter = MakeLet<Inlet>("Setter", 25);

            Size = new Vector2(200, 100);
        }

        void OnInputReceived(Signal signal)
        {
            if( signal.Args.Type == SignalTypes.BANG )
            {
                Debug.Log(Value);
            }else
            {
                string val = "";
                if (Signal.TryParseString(signal.Args, out val))
                {
                    Value = val;
                }

                Debug.Log(Value);
            }
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 200, 50));

            Value = GUILayout.TextField(Value, GUILayout.MaxWidth(180));

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
