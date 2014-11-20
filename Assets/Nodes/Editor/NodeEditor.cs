using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNodeEditor
{
    public class NodeEditor : EditorWindow
    {
        public Color ConnectionColor = Color.green;

        protected List<BaseNode> m_nodes = new List<BaseNode>();

        protected int NodeID = 0;
        protected MouseModes m_currentMouseMode = MouseModes.IDLE;
        protected Let m_currentSelectedLet = null;

        protected Vector2 m_startMousePos = Vector2.zero;

        private GameObject m_root = null;
        public GameObject Root
        {
            set
            {
                m_root = value;
                Construct();
            }
            get
            {
                return m_root;
            }
        }

        void Construct()
        {
            if (Root == null)
                return;

            m_nodes.Clear();
            BaseNode[] nodes = m_root.GetComponentsInChildren<BaseNode>();

            foreach (BaseNode node in nodes)
            {
                if (node.transform.parent == Root.transform)
                {
                    m_nodes.Add(node);

                    node.Id = NodeID;

                    for (int i = 0; i < node.Lets.Count; i++)
                    {
                        node.Lets[i].LetClicked += OnLetPressed;
                        node.Lets[i].LetDrag += OnLetDrag;
                        node.Lets[i].LetUp += OnLetUp;
                    }

                    NodeID++;
                }
            }

            Repaint();
        }

        void OnHierarchyChange()
        {
            Construct();
        }

        static void ShowEditor()
        {
            EditorWindow.GetWindow<NodeEditor>();
        }

        void OnFocus()
        {
            wantsMouseMove = true;
        }

        void OnLetPressed(object sender, LetTypes type)
        {
            m_currentSelectedLet = (Let)sender;
            m_startMousePos = Event.current.mousePosition;
            m_currentMouseMode = MouseModes.CONNECTING;
        }

        void OnLetDrag(object sender, LetTypes type)
        {
            if (sender != m_currentSelectedLet)
            {
                Let senderLet = (Let)sender;
                if (senderLet.Owner != m_currentSelectedLet.Owner)
                {
                    if ((m_currentSelectedLet.Type == LetTypes.INLET && type == LetTypes.OUTLET) ||
                        (m_currentSelectedLet.Type == LetTypes.OUTLET && type == LetTypes.INLET))
                    {
                        // Valid connection
                    }
                }
            }
        }

        void OnBreakAllConnections(object sender, LetTypes type)
        {
        }

        void OnLetUp(object sender, LetTypes type)
        {
            if (sender != m_currentSelectedLet)
            {
                Let senderLet = (Let)sender;
                if (senderLet.Owner != m_currentSelectedLet.Owner)
                {
                    if ((m_currentSelectedLet.Type == LetTypes.INLET && type == LetTypes.OUTLET) ||
                        (m_currentSelectedLet.Type == LetTypes.OUTLET && type == LetTypes.INLET))
                    {
                        Let inlet = null;
                        Let outlet = null;

                        if (m_currentSelectedLet.Type == LetTypes.INLET)
                        {
                            inlet = m_currentSelectedLet;
                            outlet = (Let)sender;
                        }
                        else
                        {
                            outlet = m_currentSelectedLet;
                            inlet = (Let)sender;
                        }

                        if( !inlet.Connections.Contains(outlet) && !outlet.Connections.Contains(inlet) )
                        {
                            inlet.Connections.Add(outlet);
                            outlet.Connections.Add(inlet);

                            if(Application.isPlaying)
                            {
                                ((Outlet)outlet).Emit += ((Inlet)inlet).Slot;
                            }
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        void CreateNode(Vector2 pos, System.Type nodeType)
        {
            // TODO : make this better ( for example, get the first available NodeID )
            NodeID++;

            GameObject nodeObject = new GameObject("Node");
            BaseNode simpleNode = (BaseNode) nodeObject.AddComponent(nodeType);

            simpleNode.Construct();
            simpleNode.Id = NodeID;
            simpleNode.Position = new Vector2(pos.x, pos.y);
            m_nodes.Add(simpleNode);

            for (int i = 0; i < simpleNode.Lets.Count; i++)
            {
                simpleNode.Lets[i].LetClicked += OnLetPressed;
                simpleNode.Lets[i].LetDrag += OnLetDrag;
                simpleNode.Lets[i].LetUp += OnLetUp;
            }

            simpleNode.transform.parent = Root.transform;
        }

        void CreateNodeMenu()
        {
            GenericMenu genericMenu = new GenericMenu();

            Vector2 mousePos = Event.current.mousePosition;

            // this is making the assumption that all assemblies we need are already loaded.
            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    var attribs = type.GetCustomAttributes(typeof(NodeMenuItem), false);
                    if (attribs.Length > 0)
                    {
                        for (int i = 0; i < attribs.Length; i++)
                        {
                            string name = ((NodeMenuItem)attribs[i]).Name;
                            System.Type nodeType = ((NodeMenuItem)attribs[i]).Type;

                            genericMenu.AddItem(new GUIContent(name), false, () =>
                            {
                                CreateNode(mousePos, nodeType);
                            });
                        }
                    }

                }
            }

            genericMenu.ShowAsContext();
        }

        void OnGUI()
        {
            BeginWindows();

            for (int i = 0; i < m_nodes.Count; i++)
            {
                m_nodes[i].Draw();
            }

            if (Event.current.type == EventType.MouseMove)
            {
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    m_nodes[i].MouseOver(Event.current.mousePosition);
                }
            }
            else if (Event.current.type == EventType.MouseDown && m_currentMouseMode != MouseModes.CONNECTING)
            {
                bool handled = false;
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    if (m_nodes[i].MouseDown(Event.current.mousePosition, Event.current.button))
                        handled = true;
                }

                if (!handled && Event.current.button == 1)
                {
                    CreateNodeMenu();
                }else if(!handled && Event.current.button == 0)
                {
                    m_startMousePos = Event.current.mousePosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                bool handled = false;
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    if( m_nodes[i].MouseDrag(Event.current.mousePosition) )
                    {
                        handled = true;
                        break;
                    }
                }

                if(!handled)
                {
                    if( Event.current.shift )
                    {
                        Vector2 offset = Event.current.mousePosition - m_startMousePos;
                        for (int i = 0; i < m_nodes.Count; i++)
                        {
                            m_nodes[i].Position += offset;
                        }

                        Repaint();

                        m_startMousePos = Event.current.mousePosition;
                        handled = true;
                    }
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    m_nodes[i].MouseUp(Event.current.mousePosition);
                }

                m_currentMouseMode = MouseModes.IDLE;
            }

            if (m_currentMouseMode == MouseModes.CONNECTING)
            {
                DrawConnectingCurve(m_startMousePos, Event.current.mousePosition);
                Repaint();
            }


            for (int i = 0; i < m_nodes.Count; i++ )
            {
                for(int j = 0 ; j < m_nodes[i].Lets.Count; j++)
                {
                    Let outlet = m_nodes[i].Lets[j];
                    if (outlet.Type == LetTypes.OUTLET)
                    {
                        for (int k = 0; k < outlet.Connections.Count; k++)
                        {
                            DrawConnection(outlet.Connections[k].Position.center, outlet.Position.center, ConnectionColor);
                        }
                    }
                }
            }

            EndWindows();

            List<BaseNode> nodesToDelete = new List<BaseNode>();
            foreach (BaseNode node in m_nodes)
            {
                if (!node.Valid)
                {
                    nodesToDelete.Add(node);
                }
            }

            foreach (BaseNode node in nodesToDelete)
            {
                m_nodes.Remove(node);

                node.BreakAllLets();

                DestroyImmediate(node.gameObject);
            }

            if( nodesToDelete.Count > 0)
                Repaint();
        }

        void OnDestroy()
        {
            // Delete all listeners
            foreach(BaseNode simpleNode in m_nodes )
            {
                for (int i = 0; i < simpleNode.Lets.Count; i++)
                {
                    simpleNode.Lets[i].LetClicked -= OnLetPressed;
                    simpleNode.Lets[i].LetDrag -= OnLetDrag;
                    simpleNode.Lets[i].LetUp -= OnLetUp;
                }
            }
        }

        void DrawConnection(Vector2 start, Vector2 end, Color color)
        {
            Vector3 startPos = new Vector3(start.x, start.y, 0);
            Vector3 endPos = new Vector3(end.x, end.y, 0);

            Vector3 startTan = startPos + Vector3.left * 50;
            Vector3 endTan = endPos + Vector3.right * 50;

            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 2);
        }

        void DrawConnectingCurve(Vector2 start, Vector2 end)
        {
            Vector3 startPos = new Vector3(start.x, start.y, 0);
            Vector3 endPos = new Vector3(end.x, end.y, 0);

            Vector3 startTan = startPos + Vector3.left * 50;
            Vector3 endTan = endPos + Vector3.right * 50;

            if (m_currentSelectedLet.Type == LetTypes.OUTLET)
            {
                startTan = startPos + Vector3.right * 50;
                endTan = endPos + Vector3.left * 50;
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.red, null, 4);
        }
    }
}
