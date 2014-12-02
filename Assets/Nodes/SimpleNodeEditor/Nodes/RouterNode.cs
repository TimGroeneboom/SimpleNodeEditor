using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("RouterNode", typeof(RouterNode))]
    public class RouterNode
        : BaseNode
    {
        [SerializeField]
        Inlet m_inlet = null;

        [SerializeField]
        List<Outlet> m_outlets = new List<Outlet>();

        public int Outlets = 1;

        [SerializeField]
        [HideInInspector]
        private int y = 0;

        void OnInletReceived(Signal signal)
        {
            for (int i = 0; i < m_outlets.Count; i++ )
                m_outlets[i].Send(signal);
        }

        public override void Construct()
        {
            Name = "Router Node";

            m_inlet = MakeLet<Inlet>("Inlet");

            y = 25;
            for (int i = 0; i < Outlets; i++ )
            {
                m_outlets.Add(MakeLet<Outlet>("Outlet " + i.ToString(), y));
                y += 25;
            }
            
            Size = new Vector2(Size.x, 55 + y);
        }

        protected override void Inited()
        {
            m_inlet.SlotReceivedSignal += OnInletReceived;
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, y, 100, 50));

            int newNum = EditorGUILayout.IntField(Outlets, GUILayout.MaxWidth(80));

            newNum = Mathf.Clamp(newNum, 1, 100);
            if(newNum < Outlets )
            {
                int remove = Outlets - newNum;

                for (int i = 0; i < remove; i++)
                {
                    Let letToRemove = m_outlets[m_outlets.Count - 1];
                    DestroyLet(letToRemove);
                    m_outlets.Remove((Outlet)letToRemove);
                    y-=25;
                }

                Size = new Vector2(Size.x, 55 + y);

                Outlets = newNum;
            }else if( newNum > Outlets )
            {
                int add = newNum - Outlets;

                for (int i = 0; i < add; i++)
                {
                    Outlet outlet = MakeLet<Outlet>("Outlet " + m_outlets.Count.ToString(), y);
                    m_outlets.Add(outlet);

                    y+=25;
                }

                Size = new Vector2(Size.x, 55 + y);

                Outlets = newNum;
            }

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }
}


