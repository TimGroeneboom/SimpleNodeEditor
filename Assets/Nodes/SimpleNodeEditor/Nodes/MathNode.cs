using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    public enum MathNodeModes
    {
        ADD,
        SUBTRACT,
        MULTIPLY,
        DIVIDE,
        POW
    }

    [NodeMenuItem("MathNode", typeof(MathNode))]
    public class MathNode : BaseNode
    {
        public MathNodeModes Mode = MathNodeModes.ADD;

        [SerializeField]
        Inlet input = null;

        [SerializeField]
        Inlet setter = null;

        [SerializeField]
        Outlet outlet = null;

        public float Value = 1.0f;

        void OnSetterReceived(Signal signal)
        {
            float val = 0.0f;
            if (Signal.TryParseFloat(signal.Args, out val))
            {
                Value = val;
            }
        }

        void OnInputReceived(Signal signal)
        {
            float val = 0.0f;
            if (Signal.TryParseFloat(signal.Args, out val))
            {
                SignalFloatArgs signalFloatArgs = new SignalFloatArgs();

                switch(Mode)
                {
                    case MathNodeModes.ADD:
                        signalFloatArgs.Value = val + Value;
                        break;
                    case MathNodeModes.DIVIDE:
                        signalFloatArgs.Value = val / Value;
                        break;
                    case MathNodeModes.MULTIPLY:
                        signalFloatArgs.Value = val * Value;
                        break;
                    case MathNodeModes.POW:
                        signalFloatArgs.Value = Mathf.Pow(val, Value);
                        break;
                    case MathNodeModes.SUBTRACT:
                        signalFloatArgs.Value = val - Value;
                        break;
                }

                outlet.Send(signalFloatArgs);
            }
        }

        protected override void Inited()
        {
            input.SlotReceivedSignal += OnInputReceived;
            setter.SlotReceivedSignal += OnSetterReceived;
        }

        public override void Construct()
        {
            Name = "Math Node";

            input = MakeLet<Inlet>("Input");
            setter = MakeLet<Inlet>("Setter", 25);
            outlet = MakeLet<Outlet>("Outlet", 50);

            Size = new Vector2(125, 150);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 75, 100, 75));

            Mode = (MathNodeModes)EditorGUILayout.EnumPopup((System.Enum)Mode, GUILayout.MaxWidth(80));
            EditorGUILayout.Space();
            Value = EditorGUILayout.FloatField(Value, GUILayout.MaxWidth(50));
            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
