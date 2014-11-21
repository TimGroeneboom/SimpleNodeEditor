using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    public enum Conditions
    {
        GREATER,
        LESSER,
        EQUAL
    }

    [NodeMenuItem("ConditionalNode", typeof(ConditionalNode))]
    public class ConditionalNode : BaseNode
    {
        [SerializeField]
        Outlet outlet = null;
        [SerializeField]
        Inlet inlet = null;

        public float Value = 0.0f;
        public Conditions Condition = Conditions.GREATER;

        void OnInputReceived(Signal signal)
        {
            switch(signal.Args.Type)
            {
                case SignalTypes.BANG:
                    break;
                case SignalTypes.FLOAT:
                    SignalFloatArgs signalArgs = signal.Args as SignalFloatArgs;
                    if(signalArgs.Value > Value)
                    {
                        outlet.Send(signal.Args);
                    }
                    break;
                case SignalTypes.STRING:
                    SignalStringArgs signalTextArgs = signal.Args as SignalStringArgs;
                    float result = 0.0f;
                    if(float.TryParse(signalTextArgs.Value, out result))
                    {
                        if(result > Value)
                        {
                            outlet.Send(signal.Args);
                        }
                    }
                    break;
            }
        }

        protected override void Inited()
        {
            inlet.SlotReceivedSignal += OnInputReceived;
        }

        public override void Construct()
        {
            Name = "ConditionalNode";
            
            inlet = (Inlet)MakeLet(LetTypes.INLET);
            outlet = (Outlet)MakeLet(LetTypes.OUTLET);
            outlet.yOffset = 25;

            Size = new Vector2(125, 125);
        }

#if UNITY_EDITOR
   public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 125, 75));
            Value = EditorGUILayout.FloatField(Value, GUILayout.MaxWidth(100));
            EditorGUILayout.Space();
            Condition = (Conditions) EditorGUILayout.EnumPopup(Condition, GUILayout.MaxWidth(100));
            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
