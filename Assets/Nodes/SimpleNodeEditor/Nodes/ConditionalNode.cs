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
        Outlet m_outlet = null;
        [SerializeField]
        Inlet m_inlet = null;

        public float Value = 0.0f;
        public Conditions Condition = Conditions.GREATER;

        void OnInputReceived(Signal signal)
        {
            float val = 0.0f;
            if(Signal.TryParseFloat(signal.Args, out val))
            {
                switch (Condition)
                {
                    case Conditions.GREATER:
                        if (val > Value)
                            GenerateBang();
                        break;
                    case Conditions.LESSER:
                        if (val < Value)
                            GenerateBang();
                        break;
                    case Conditions.EQUAL:
                        if (val == Value)
                            GenerateBang();
                        break;
                }
            }

        }

        void GenerateBang()
        {
            m_outlet.Send(new SignalArgs());
        }

        protected override void Inited()
        {
            m_inlet.SlotReceivedSignal += OnInputReceived;
        }

        public override void Construct()
        {
            Name = "ConditionalNode";

            m_inlet = MakeLet<Inlet>("Inlet");
            m_outlet = MakeLet<Outlet>("Outlet", 25);

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
