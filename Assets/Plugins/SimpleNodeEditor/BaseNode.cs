using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNodeEditor
{
    [System.Serializable]
    public abstract class BaseNode 
        : MonoBehaviour
    {
        [SerializeField]
        protected Rect m_rect;
        public Rect Rect { get { return m_rect; } }
        public int Id = 0;

        private string m_name = "Node";
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                gameObject.name = value;
                m_name = value;
            }
        }

        [SerializeField]
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
       
        [SerializeField]
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

        [SerializeField]
        protected List<Let> m_lets = new List<Let>();
        public List<Let> Lets { get { return m_lets; } }

        [SerializeField]
        protected Rect m_closeBoxPos = new Rect(10, -20, 10, 20);

        protected Let MakeLet(LetTypes type)
        {
            Let let = null;

            switch(type)
            {
                case LetTypes.INLET:
                    let = gameObject.AddComponent<Inlet>();
                    break;
                case LetTypes.OUTLET:
                    let = gameObject.AddComponent<Outlet>();
                    break;
            }

            let.Construct(this);

            m_lets.Add(let);

            return let;
        }

        void Start()
        {
            for (int i = 0; i < m_lets.Count; i++)
            {
                if(m_lets[i].Type == LetTypes.OUTLET)
                {
                    for(int j = 0 ; j < m_lets[i].Connections.Count; j++)
                    {
                        ((Outlet)m_lets[i]).Emit += ((Inlet)m_lets[i].Connections[j]).Slot;
                    }
                }
            }

            Inited();
        }

        public abstract void Construct();
        protected abstract void Inited();

        public void Draw()
        {
            m_rect = GUI.Window(Id, m_rect, WindowCallback, gameObject.name);

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

        public void BreakAllLets()
        {
            foreach (Let let in m_lets)
            {
                let.BreakAllConnections();
            }
        }

        void OnDestroy()
        {
            BreakAllLets();
        }
    }
}