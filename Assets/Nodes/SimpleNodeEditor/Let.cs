using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

        public List<Let> Connections = new List<Let>();

        [SerializeField]
        protected LetTypes m_type = LetTypes.UNDEFINED;
        public LetTypes Type { get { return m_type; } }

        public abstract void Construct(BaseNode owner);
        public abstract void Construct(BaseNode owner, Rect offset);

        public void RemoveConnection(Let letToRemove)
        {
            Connections.Remove(letToRemove);
        }

        public void BreakAllConnections()
        {
            if (Type == LetTypes.INLET)
            {
                for (int i = 0; i < Connections.Count; i++)
                {
                    ((Outlet)Connections[i]).Emit -= ((Inlet)this).Slot;
                }
            }
            else if (Type == LetTypes.OUTLET)
            {
                for (int i = 0; i < Connections.Count; i++)
                {
                    ((Outlet)this).Emit -= ((Inlet)Connections[i]).Slot;
                }
            }

            for (int i = 0; i < Connections.Count; i++)
            {
                Connections[i].RemoveConnection(this);
            }

            Connections.Clear();
        }

#if UNITY_EDITOR
        public virtual void DrawLet(Rect position)
        {
            Position = new Rect(position.x + Offset.x, position.y + Offset.y + yOffset, Offset.width, Offset.height);
            GUI.Box(Position, "");
        }

        public virtual bool MouseOver(Vector2 mousePos)
        {
            if (mousePos.x > Position.x - m_hitArea.x && mousePos.x < Position.x + Position.width + m_hitArea.x &&
                mousePos.y > Position.y - m_hitArea.y && mousePos.y < Position.y + Position.height + m_hitArea.y)
            {
                return true;
            }

            return false;
        }

        public virtual bool MouseDrag(Vector2 mousePos)
        {
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
            GUI.Label(new Rect(10, 20 + yOffset, 80, 20), Name);
        }
#endif
    }
}

