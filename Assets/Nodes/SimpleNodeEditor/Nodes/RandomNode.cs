using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("RandomIntNode", typeof(RandomIntNode))]
    public class RandomIntNode : BaseNode
    {
        [SerializeField]
        Inlet inlet = null;

        [SerializeField]
        Outlet outlet = null;

        public int Range = 10;

        void OnInputReceived(Signal signal)
        {
            if(signal.Args.Type == SignalTypes.BANG )
            {
                SignalFloatArgs args = new SignalFloatArgs();
                args.Value = Random.Range(0, Range);
                outlet.Send(args);
            }else
            {
                int val = 0;
                if( Signal.TryParseInt(signal.Args, out val) )
                {
                    Range = val;
                }
            }
        }

        protected override void Inited()
        {
            inlet.SlotReceivedSignal += OnInputReceived;
        }

        public override void Construct()
        {
            Name = "RandomIntNode";

            inlet = MakeLet<Inlet>();

            outlet = MakeLet<Outlet>();
            outlet.yOffset = 25;

            Size = new Vector2(125, 100);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 100, 50));
            Range = EditorGUILayout.IntField(Range, GUILayout.MaxWidth(50));
            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
