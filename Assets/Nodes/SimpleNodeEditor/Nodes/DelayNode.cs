using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    public class DelayedSignal
    {
        public float RunningTime = 0.0f;
        public Signal Signal = null;
    }

    [NodeMenuItem("DelayNode", typeof(DelayNode))]
    public class DelayNode : BaseNode
    {
        public float Delay = 1.0f;

        [SerializeField]
        Inlet m_inlet = null;

        [SerializeField]
        Outlet m_outlet = null;

        List<DelayedSignal> m_delayedSignals = new List<DelayedSignal>();
        List<DelayedSignal> m_signalsToAdd = new List<DelayedSignal>();

        void OnInlet(Signal signal)
        {
            DelayedSignal delayedSignal = new DelayedSignal();
            delayedSignal.Signal = signal;

            m_signalsToAdd.Add(delayedSignal);
        }

        protected override void Inited()
        {
            m_inlet.SlotReceivedSignal += OnInlet;
        }

        public override void Construct()
        {
            Name = "DelayNode";

            m_inlet = MakeLet<Inlet>("Inlet");
            m_outlet = MakeLet<Outlet>("Outlet", 25);

            Size = new Vector2(125, Size.y);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 100, 50));
            EditorGUIUtility.LookLikeControls(50, 50);
            Delay = EditorGUILayout.FloatField("Delay", Delay, GUILayout.MaxWidth(80));
            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif

        void Update()
        {
            if (m_signalsToAdd.Count > 0)
            {
                foreach (DelayedSignal delayedSignal in m_signalsToAdd)
                    m_delayedSignals.Add(delayedSignal);

                m_signalsToAdd.Clear();
            }

            if( m_delayedSignals.Count > 0 )
            {
                List<DelayedSignal> SignalsToRemove = null;

                foreach (DelayedSignal delayedSignal in m_delayedSignals)
                {
                    delayedSignal.RunningTime += Time.deltaTime;
                    if (delayedSignal.RunningTime > Delay)
                    {
                        m_outlet.Send(delayedSignal.Signal.Args);

                        if (SignalsToRemove == null)
                            SignalsToRemove = new List<DelayedSignal>();

                        SignalsToRemove.Add(delayedSignal);
                    }
                }

                if( SignalsToRemove != null )
                {
                    foreach(DelayedSignal delayedSignal in SignalsToRemove)
                    {
                        m_delayedSignals.Remove(delayedSignal);
                    }
                }
            }
        }
    }
}