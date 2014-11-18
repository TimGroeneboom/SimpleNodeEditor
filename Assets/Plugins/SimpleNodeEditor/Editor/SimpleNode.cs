using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNodeEditor
{
    [NodeMenuItem("SimpleNode", typeof(SimpleNode))]
    public class SimpleNode
    {
        protected Rect m_rect;
        public Rect Rect { get { return m_rect; } }
        public int Id = 0;

        public string Name = "Node";

        private Vector2 m_position = new Vector2(10,10);
        public Vector2 Position
        {
            set
            {
                m_position = value;
                m_rect = new Rect(m_position.x, m_position.y, m_size.x, m_size.y);
            }
            get
            {
                return m_position;
            }
        }
       
        private Vector2 m_size = new Vector2(100,100);
        public Vector2 Size
        {
            set
            {
                m_size = value;
                m_rect = new Rect(Position.x, Position.y, m_size.x, m_size.y);
            }
            get
            {
                return m_size;
            }
        }

        protected bool m_valid = true;
        public bool Valid { get { return m_valid; } }

        protected List<Let> m_lets = new List<Let>();
        public List<Let> Lets { get { return m_lets; } }

        protected Rect m_closeBoxPos = new Rect(10, -20, 10, 20);
        
        public SimpleNode() 
        {
            Size = new Vector2(100, 100);

            Inlet inlet = new Inlet(this);
            m_lets.Add(inlet);

            Outlet outlet = new Outlet(this);
            outlet.yOffset = 25;
            m_lets.Add(outlet);
        }

        public virtual void Draw()
        {
            m_rect = GUI.Window(Id, m_rect, WindowCallback, Name);

            Position = new Vector2(m_rect.x, m_rect.y);
            m_size = new Vector2(m_rect.width, m_rect.height);

            // Draw Let(s)
            for (int i = 0; i < m_lets.Count; i++)
            {
                m_lets[i].DrawLet(m_rect);
            }

            // draw close box
            GUI.Box(m_closeBoxPos, "X");
        }

        public virtual bool MouseOver(Vector2 mousePos)
        {
            bool handled = false;
            for (int i = 0; i < m_lets.Count; i++)
            {
                if (m_lets[i].MouseOver(mousePos))
                {
                    handled = true;
                    break;
                }
            }

            return handled;
        }

        public virtual bool MouseDrag(Vector2 mousePos)
        {
            bool handled = false;
            for (int i = 0; i < m_lets.Count; i++)
            {
                if (m_lets[i].MouseDrag(mousePos))
                {
                    handled = true;
                    break;
                }
            }

            return handled;
        }

        public virtual bool MouseDown(Vector2 mousePos, int button)
        {
            bool handled = false;
            for (int i = 0; i < m_lets.Count; i++)
            {
                if (m_lets[i].MouseDown(mousePos, button))
                {
                    handled = true;
                    break;
                }
            }

            // check if mouse is on close box if mouseevent is not handled by Input
            if (!handled && m_closeBoxPos.Contains(mousePos))
            {
                m_valid = false;

                foreach (Let let in m_lets)
                {
                    let.BreakAllConnections(let, let.Type);
                }
            }

            return handled;
        }

        public virtual bool MouseUp(Vector2 mousePos)
        {
            bool handled = false;
            for (int i = 0; i < m_lets.Count; i++)
            {
                if (m_lets[i].MouseUp(mousePos))
                {
                    handled = true;
                    break;
                }
            }

            return handled;
        }

        public virtual void WindowCallback(int id)
        {
            GUI.DragWindow();

            for (int i = 0; i < m_lets.Count; i++)
            {
                m_lets[i].DrawLabel();
            }

            m_closeBoxPos = new Rect(Position.x + 5, Position.y - 25, 20, 20);
        }
    }
}