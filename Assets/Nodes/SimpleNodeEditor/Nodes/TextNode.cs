using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("TextNode", typeof(TextNode))]
    public class TextNode : BaseNode
    {
        [SerializeField]
        Inlet inlet = null;
        [SerializeField]
        Inlet setter = null;

        [SerializeField]
        Outlet outlet = null;

        public string Value = "";

        protected override void Inited()
        {
            inlet.SlotReceivedSignal += OnInletReceived;
            setter.SlotReceivedSignal += OnSetterReceived;
        }

        void OnInletReceived(Signal signal)
        {
            if (signal.Args.Type == SignalTypes.STRING)
            {
                Value = ((SignalStringArgs)signal.Args).Value;
            }

            outlet.Send(new SignalStringArgs(Value));
        }

        void OnSetterReceived(Signal signal)
        {
            if (signal.Args.Type == SignalTypes.STRING)
            {
                Value = ((SignalStringArgs)signal.Args).Value;
            }
            else
            {
                string val = "";
                if (Signal.TryParseString(signal.Args, out val))
                {
                    Value = val;
                }
            }
        }

        public override void Construct()
        {
            Name = "TextNode";

            inlet = (Inlet)MakeLet(LetTypes.INLET);
            inlet.Name = "Trigger";

            setter = (Inlet)MakeLet(LetTypes.INLET);
            setter.yOffset = 25;
            setter.Name = "Set";

            outlet = (Outlet)MakeLet(LetTypes.OUTLET);
            outlet.yOffset = 50;
            outlet.Name = "Output";

            Size = new Vector2(200, 125);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 75, 180, 75));
            //EditorGUIUtility.LookLikeControls(30, 30);

            Value = GUILayout.TextField(Value, GUILayout.MaxWidth(180));

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
