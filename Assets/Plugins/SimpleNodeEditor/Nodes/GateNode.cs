using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("GateNode", typeof(GateNode))]
    public class GateNode : BaseNode
    {
        [SerializeField]
        Outlet outlet = null;
        [SerializeField]
        Inlet inlet = null;

        public bool Value = false;

        void OnSignalReceived(Signal signal)
        {
            if(Value)
            {
                outlet.Emit(signal);
            }
        }

        protected override void Inited()
        {
            inlet.SlotReceivedSignal += OnSignalReceived;
        }

        public override void Construct()
        {
            Name = "GateNode";

            inlet = (Inlet)MakeLet(LetTypes.INLET);
            
            outlet = (Outlet)MakeLet(LetTypes.OUTLET);
            outlet.yOffset = 25;

            Size = new Vector2(Size.x, 100);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 100, 50));
            EditorGUIUtility.LookLikeControls(30, 30);

            Value = GUILayout.Toggle(Value, "Is open");

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }
}
