using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNodeEditor
{
    public enum LetTypes
    {
        UNDEFINED,
        INLET,
        OUTLET
    }

    public delegate void LetEventHandler(object sender, LetTypes type);

    [System.Serializable]
    public abstract class Let 
        : MonoBehaviour
    {
        public bool Visible = true;

        [SerializeField]
        protected BaseNode m_owner = null;
        public BaseNode Owner { get { return m_owner; } set { m_owner = value; } }

        public int yOffset = 0;

        [SerializeField]
        protected Vector2 m_hitArea = new Vector2(10, 10);

        public Rect Offset = new Rect(0, 0, 0, 0);
        public Rect Position = new Rect(0, 0, 0, 0);

        public string Name = "Label";

        public LetEventHandler LetClicked = (object sender, LetTypes type) => { };
        public LetEventHandler LetDrag = (object sender, LetTypes type) => { };
        public LetEventHandler LetUp = (object sender, LetTypes type) => { };

        public List<Connection> Connections = new List<Connection>();

        [SerializeField]
        protected LetTypes m_type = LetTypes.UNDEFINED;
        public LetTypes Type { get { return m_type; } }

        public abstract void Construct(BaseNode owner);
        public abstract void Construct(BaseNode owner, Rect offset);

        public string HelpText = "This is a let.";


        public virtual void RemoveLet(Let letToRemove)
        {
            foreach(Connection connection in Connections)
            {
                if( connection.Outlet == letToRemove || connection.Inlet == letToRemove)
                {
                    Connections.Remove(connection);
                    break;
                }
            }
            
        }

        public void BreakAllConnections()
        {
            if (Type == LetTypes.INLET)
            {
                for (int i = 0; i < Connections.Count; i++)
                {
                    Connections[i].Outlet.Emit -= ((Inlet)this).Slot;
                }
            }
            else if (Type == LetTypes.OUTLET)
            {
                for (int i = 0; i < Connections.Count; i++)
                {
                    ((Outlet)this).Emit -= Connections[i].Inlet.Slot;
                }
            }

            for (int i = 0; i < Connections.Count; i++)
            {
                Connections[i].Outlet.RemoveLet(this);
            }

            Connections.Clear();
        }

        public bool Contains(Let let)
        {
            foreach(Connection connection in Connections)
            {
                if (connection.Inlet == let || connection.Outlet == let)
                    return true;
            }

            return false;
        }

        #region NODE_EDITOR_FUCTIONS
#if UNITY_EDITOR
        public virtual void DrawLet(Rect position)
        {
            if (!Visible)
                return;

            Position = new Rect(position.x + Offset.x, position.y + Offset.y + yOffset, Offset.width, Offset.height);
            GUI.Box(Position, "");
        }

        private GenericMenu m_genericMenu = null;
        public virtual bool MouseOver(Vector2 mousePos)
        {
            if (!Visible)
                return false;

            if (mousePos.x > Position.x - m_hitArea.x && mousePos.x < Position.x + Position.width + m_hitArea.x &&
                mousePos.y > Position.y - m_hitArea.y && mousePos.y < Position.y + Position.height + m_hitArea.y)
            {
                return true;
            }

            return false;
        }

        public virtual bool MouseDrag(Vector2 mousePos)
        {
            if (!Visible)
                return false;

            if (mousePos.x > Position.x - m_hitArea.x && mousePos.x < Position.x + Position.width + m_hitArea.x &&
                mousePos.y > Position.y - m_hitArea.y && mousePos.y < Position.y + Position.height + m_hitArea.y)
            {
                LetDrag(this, Type);
                return true;
            }

            return false;
        }

        public virtual bool MouseDown(Vector2 mousePos, int button)
        {
            if (!Visible)
                return false;

            if (mousePos.x > Position.x - m_hitArea.x && mousePos.x < Position.x + Position.width + m_hitArea.x &&
                mousePos.y > Position.y - m_hitArea.y && mousePos.y < Position.y + Position.height + m_hitArea.y)
            {
                if (button == 0)
                {
                    LetClicked(this, Type);
                }
                else if (button == 1)
                {
                    GenericMenu genericMenu = new GenericMenu();
                    genericMenu.AddItem(new GUIContent("Break all connections"), false, BreakAllConnections);

                    genericMenu.ShowAsContext();
                }

                return true;
            }

            return false;
        }

        public virtual bool MouseUp(Vector2 mousePos)
        {
            if (!Visible)
                return false;

            if (mousePos.x > Position.x - m_hitArea.x && mousePos.x < Position.x + Position.width + m_hitArea.x &&
                mousePos.y > Position.y - m_hitArea.y && mousePos.y < Position.y + Position.height + m_hitArea.y)
            {
                LetUp(this, Type);
                return true;
            }

            return false;
        }

        public virtual void DrawLabel()
        {
            if (!Visible)
                return;

            GUI.Label(new Rect(10, 20 + yOffset, 80, 20), Name);
        }
#endif
        #endregion
    }
}

