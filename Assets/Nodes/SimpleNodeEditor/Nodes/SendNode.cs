using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    public delegate void SignalBroadCaster(string id, Signal signal);

    [NodeMenuItem("SendNode", typeof(SendNode))]
    public class SendNode : BaseNode
    {
        public static SignalBroadCaster BroadCastSignal = (string id, Signal signal) => { };

        [SerializeField]
        Inlet inlet = null;

        public string ReceiveName = "SendTo";

        void OnInletReceived(Signal signal)
        {
            BroadCastSignal(ReceiveName, signal);
        }

        protected override void Inited()
        {
            inlet.SlotReceivedSignal += OnInletReceived;
        }

        public override void Construct()
        {
            Name = "Send";

            inlet = MakeLet<Inlet>("Input");
            Size = new Vector2(Size.x, 75);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 25, 100, 50));
            //EditorGUIUtility.LookLikeControls(30, 30);

            ReceiveName = EditorGUILayout.TextField(ReceiveName, GUILayout.MaxWidth(80));

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }
}
