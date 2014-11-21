using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("ButtonNode", typeof(ButtonNode))]
    public class ButtonNode : BaseNode
    {
        [SerializeField]
        Inlet inlet = null;

        [SerializeField]
        Outlet outlet = null;

        void OnInletReceived(Signal signal)
        {
            // any signal gets converted to bang
            outlet.Send(new SignalArgs());
        }

        protected override void Inited()
        {
            inlet.SlotReceivedSignal += OnInletReceived;
        }

        public override void Construct()
        {
            Name = "ButtonNode";

            inlet = (Inlet)MakeLet(LetTypes.INLET);
            outlet = (Outlet)MakeLet(LetTypes.OUTLET);
            outlet.yOffset = 25;
            Size = new Vector2(Size.x, 100);
        }

        [ContextMenu("BANG")]
        public void Emit()
        {
            outlet.Send(new SignalArgs());
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 100, 50));
            //EditorGUIUtility.LookLikeControls(30, 30);

            if( GUILayout.Button("Emit", GUILayout.MaxWidth(80) ) )
            {
                Emit();
            }

            GUI.EndGroup();

 	        base.WindowCallback(id);
        }
#endif
    }


}
