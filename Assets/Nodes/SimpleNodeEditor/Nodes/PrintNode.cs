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
        Inlet input = null;

        [SerializeField]
        Inlet setter = null;

        public string Value = "Hello World!";

        protected override void Inited()
        {
            input.SlotReceivedSignal += OnInputReceived;
        }

        public override void Construct()
        {
            Name = "PrintNode";

            input = (Inlet)MakeLet(LetTypes.INLET);
            input.Name = "Input";

            setter = (Inlet)MakeLet(LetTypes.INLET);
            setter.Name = "Setter";

            Size = new Vector2(200, 75);
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
            GUI.BeginGroup(new Rect(5, 25, 200, 50));

            Value = GUILayout.TextField(Value, GUILayout.MaxWidth(180));

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
