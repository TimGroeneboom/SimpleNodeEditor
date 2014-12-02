using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("MessageNode", typeof(MessageNode))]
    public class MessageNode : BaseNode
    {
        [SerializeField]
        Inlet input = null;

        [SerializeField]
        Inlet setter = null;

        public string Message = "Message";
        public GameObject Target = null;

        void OnSetterReceived(Signal signal)
        {
            string val = "";
            if (Signal.TryParseString(signal.Args, out val))
            {
                Message = val;
            }
        }

        void OnInputReceived(Signal signal)
        {
            if (Target == null)
                return;

            switch(signal.Args.Type)
            {
                case SignalTypes.BANG:
                    Target.SendMessage(Message);
                    break;
                case SignalTypes.FLOAT:
                     Target.SendMessage(Message, ((SignalFloatArgs)signal.Args).Value);
                    break;
                case SignalTypes.STRING:
                     Target.SendMessage(Message, ((SignalStringArgs)signal.Args).Value);
                    break;
            }
        }

        protected override void Inited()
        {
            input.SlotReceivedSignal += OnInputReceived;
            setter.SlotReceivedSignal += OnSetterReceived;
        }

        public override void Construct()
        {
            Name = "Message Node";

            input = MakeLet<Inlet>("Input");
            setter = MakeLet<Inlet>("Setter", 25);

            Size = new Vector2(300, 125);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 300, 75));

            Message = EditorGUILayout.TextField("Message", Message, GUILayout.MaxWidth(280));
            EditorGUILayout.Space();
            Target = (GameObject)EditorGUILayout.ObjectField("Target", Target, typeof(GameObject), GUILayout.MaxWidth(280));
            
            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
