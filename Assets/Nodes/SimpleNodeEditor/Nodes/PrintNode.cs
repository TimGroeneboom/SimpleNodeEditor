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

        public string Value = "Hello World!";

        protected override void Inited()
        {
            trigger.SlotReceivedSignal += OnSignalReceived;
            input.SlotReceivedSignal += OnInputReceived;
        }

        public override void Construct()
        {
            Name = "PrintNode";

            trigger = (Inlet)MakeLet(LetTypes.INLET);
            trigger.Name = "Trigger";

            input = (Inlet)MakeLet(LetTypes.INLET);
            input.yOffset = 25;
            input.Name = "Input";

            Size = new Vector2(200, 100);
        }

        void OnSignalReceived(Signal signal)
        {
            Debug.Log(Value);
        }

        void OnInputReceived(Signal signal)
        {
            string val = "";
            if( Signal.TryParseString(signal.Args, out val) )
            {
                Value = val;
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
