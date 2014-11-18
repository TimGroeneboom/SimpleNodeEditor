using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SimpleNodeEditor
{
    public class NodeEditor : EditorWindow
    {
        protected List<SimpleNode> m_nodes = new List<SimpleNode>();
        protected List<Connection> m_connections = new List<Connection>();

        protected int NodeID = 0;
        protected MouseModes m_currentMouseMode = MouseModes.IDLE;
        protected Let m_currentSelectedLet = null;

        protected Vector2 m_startMousePos = Vector2.zero;

        [MenuItem("Window/Simple node editor")]
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
            List<Connection> connectionsToDelete = new List<Connection>();
            foreach (Connection connection in m_connections)
            {
                if (sender == connection.Inlet || sender == connection.Outlet)
                {
                    connectionsToDelete.Add(connection);
                }
            }

            foreach (Connection connection in connectionsToDelete)
            {
                m_connections.Remove(connection);
            }
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
                        Inlet inlet = null;
                        Outlet outlet = null;

                        if (m_currentSelectedLet.Type == LetTypes.INLET)
                        {
                            inlet = (Inlet)m_currentSelectedLet;
                            outlet = (Outlet)sender;
                        }
                        else
                        {
                            outlet = (Outlet)m_currentSelectedLet;
                            inlet = (Inlet)sender;
                        }

                        Connection connection = new Connection(inlet, outlet);
                        m_connections.Add(connection);
                    }
                }
            }
        }

        void CreateNode(Vector2 pos, System.Type nodeType)
        {
            // TODO : make this better ( for example, get the first available NodeID )
            NodeID++;

            SimpleNode node = (SimpleNode)System.Activator.CreateInstance(nodeType);
            node.Id = NodeID;
            node.Position = new Vector2(pos.x, pos.y);
            m_nodes.Add(node);

            for (int i = 0; i < node.Lets.Count; i++)
            {
                node.Lets[i].LetClicked += OnLetPressed;
                node.Lets[i].LetDrag += OnLetDrag;
                node.Lets[i].LetUp += OnLetUp;
                node.Lets[i].BreakAllConnections += OnBreakAllConnections;
            }
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

            for (int i = 0; i < m_connections.Count; i++)
            {
                DrawConnection(m_connections[i].Inlet.Position.center, m_connections[i].Outlet.Position.center, Color.blue);
            }

            EndWindows();

            List<SimpleNode> nodesToDelete = new List<SimpleNode>();
            foreach (SimpleNode node in m_nodes)
            {
                if (!node.Valid)
                {
                    nodesToDelete.Add(node);
                }
            }

            foreach (SimpleNode node in nodesToDelete)
            {
                m_nodes.Remove(node);
            }

            if( nodesToDelete.Count > 0)
                Repaint();
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
