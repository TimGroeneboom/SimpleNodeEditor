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
        Inlet inlet = null;
        [SerializeField]
        Inlet setter = null;

        [SerializeField]
        Outlet outlet = null;

        public float Value = 0.0f;

        protected override void Inited()
        {
            inlet.SlotReceivedSignal += OnInletReceived;
            setter.SlotReceivedSignal += OnSetterReceived;
        }

        void OnInletReceived(Signal signal)
        {
            if(signal.Args.Type==SignalTypes.FLOAT)
            {
                Value = ((SignalFloatArgs)signal.Args).Value;
            }

            outlet.Send(new SignalFloatArgs(Value));
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

            inlet = (Inlet)MakeLet(LetTypes.INLET);
            inlet.Name = "Trigger";

            setter = (Inlet)MakeLet(LetTypes.INLET);
            setter.yOffset = 25;
            setter.Name = "Set";

            outlet = (Outlet)MakeLet(LetTypes.OUTLET);
            outlet.yOffset = 50;

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
