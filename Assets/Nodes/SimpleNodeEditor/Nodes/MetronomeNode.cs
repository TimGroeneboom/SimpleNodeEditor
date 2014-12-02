using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("MetronomeNode", typeof(MetronomeNode))]
    public class MetronomeNode : BaseNode
    {
        [SerializeField]
        Inlet m_inlet = null;

        [SerializeField]
        Outlet m_outlet = null;

        public bool m_on = false;

        private float m_time = 0.0f;
        private float m_interval = 1.0f;

        void OnInletReceived(Signal signal)
        {
            bool val = false;
            if( Signal.TryParseBool(signal.Args, out val))
            {
                m_on = val;
            }
        }

        protected override void Inited()
        {
            m_inlet.SlotReceivedSignal += OnInletReceived;
        }

        public override void Construct()
        {
            Name = "MetronomeNode";

            m_inlet = MakeLet<Inlet>("Input");
            m_outlet = MakeLet<Outlet>("Output", 25);
            Size = new Vector2(Size.x, 125);
        }

        void Update()
        {
            if (m_on)
            {
                m_time += Time.deltaTime;

                if (m_time > m_interval)
                {
                    m_time %= m_interval;

                    // sendout bang
                    m_outlet.Send(new Signal(m_outlet, new SignalArgs()));
                }
            }
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 100, 75));

            m_on = GUILayout.Toggle(m_on, "On");
            GUILayout.Space(10);

            m_interval = EditorGUILayout.FloatField(m_interval, GUILayout.MaxWidth( 50 ));
            m_interval = Mathf.Clamp(m_interval, 0.001f, float.MaxValue);

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
