using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("ReceiveNode", typeof(ReceiveNode))]
    public class ReceiveNode : BaseNode
    {
        [SerializeField]
        Outlet m_outlet = null;

        public string ReceiveName = "SendTo";

        void OnBroadcastedSignalReceived(string id, Signal signal)
        {
            if(id==ReceiveName)
            {
                m_outlet.Send(signal);
            }
        }

        protected override void Inited()
        {
            SendNode.BroadCastSignal += OnBroadcastedSignalReceived;
        }

        public override void Construct()
        {
            Name = "Receive";

            m_outlet = MakeLet<Outlet>("Outlet");
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
