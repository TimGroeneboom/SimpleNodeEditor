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
        public List<NodeGraph> m_nodegraphStack = new List<NodeGraph>();

        public Inlet MasterInlet = null;
        public Outlet MasterOutlet = null;

        private List<Vector2> m_livePoints = new List<Vector2>();

        public void GoBack()
        {
            NodeGraph newRoot = m_nodegraphStack[m_nodegraphStack.Count - 1];
            m_nodegraphStack.Remove(newRoot);
            Root = newRoot.gameObject;
        }

        public void OnShowGraphNodeClicked( NodeGraph nodeGraph )
        {
            m_nodegraphStack.Add(Root.GetComponent<NodeGraph>());
            Root = nodeGraph.gameObject;
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        public void Construct()
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

                NodeGraph nodeGraph = node as NodeGraph;
                if( nodeGraph != null )
                {
                    nodeGraph.ShowNodeGraphClicked -= OnShowGraphNodeClicked;
                    nodeGraph.ShowNodeGraphClicked += OnShowGraphNodeClicked;
                }
            }

            if( m_nodegraphStack.Count > 0 )
            {
                NodeGraph nodeGraph = Root.GetComponent<NodeGraph>();
                nodeGraph.ShowLets();

            }else
            {
                NodeGraph nodeGraph = Root.GetComponent<NodeGraph>();
                nodeGraph.HideLets();
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
            Construct();
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

                        if( !inlet.Contains(outlet) && 
                            !outlet.Contains(inlet) )
                        {
                            Connection connection = new Connection((Inlet)inlet, (Outlet)outlet, m_livePoints);
                            inlet.Connections.Add(connection);
                            outlet.Connections.Add(connection);

                            if(Application.isPlaying)
                            {
                                ((Outlet)outlet).MakeConnections();
                             //   ((Outlet)outlet).Emit += ((Inlet)inlet).Slot;
                            }
                        }
                        else
                        {
                        }

                        m_livePoints.Clear();
                        m_currentMouseMode = MouseModes.IDLE;
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

        void BreakConnectionMenu(Inlet inlet, Outlet outlet)
        {
            GenericMenu genericMenu = new GenericMenu();

            genericMenu.AddItem(new GUIContent("Break Connection"), false, () =>
            {
                inlet.RemoveLet(outlet);
                outlet.RemoveLet(inlet);
            });

            genericMenu.ShowAsContext();
        }

        void OnGUI()
        {
            if (m_nodegraphStack.Count > 0 )
            {
                if(GUI.Button(new Rect(10, 10, 200, 50), "Back"))
                {
                    // Pop nodegraph stack
                    GoBack();

                    return;
                }
            }

            BeginWindows();

            for (int i = 0; i < m_nodes.Count; i++)
            {
                m_nodes[i].Draw();
            }

            EndWindows();

            bool isConnectionSelected = false;
            Connection connectionSelected = null;
            float minDistance = float.MaxValue;

            // Collect connections
            List<Connection> connections = new List<Connection>();
            int selectedConnection = -1;
            for (int i = 0; i < m_nodes.Count; i++)
            {
                for (int j = 0; j < m_nodes[i].Lets.Count; j++)
                {
                    Let outlet = m_nodes[i].Lets[j];
                    if (outlet.Type == LetTypes.OUTLET)
                    {
                        for (int k = 0; k < outlet.Connections.Count; k++)
                        {
                            Connection connection = outlet.Connections[k];
                            connections.Add(connection);

                            List<Vector2> points = new List<Vector2>();
                            points.Add(new Vector2(connection.Inlet.Position.center.x, connection.Inlet.Position.center.y));
                            for (int l = 0; l < connection.Points.Length; l++)
                            {
                                points.Add(connection.Points[l]);
                            }
                            points.Add(new Vector2(connection.Outlet.Position.center.x, connection.Outlet.Position.center.y));

                            for(int l = 0; l < points.Count-1; l++ )
                            {
                                float distance = MouseDistanceToLine(points[l], points[l+1]);

                                if (distance < 20.0f)
                                {
                                    if (distance < minDistance)
                                    {
                                        minDistance = distance;
                                        isConnectionSelected = true;
                                        connectionSelected = connection;
                                        selectedConnection = connections.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Draw connections 
            for(int i = 0;i < connections.Count; i++)
            {
                Connection connection = connections[i];

                List<Vector2> points = new List<Vector2>();
                points.Add(connection.Inlet.Position.center);
                for (int j = 0; j < connection.Points.Length; j++)
                {
                    points.Add(connection.Points[j]);
                }
                points.Add(connection.Outlet.Position.center);

                for (int j = 0; j < points.Count-1; j++)
                {
                    if (i != selectedConnection)
                    {
                        DrawLine(points[j], points[j + 1], ConnectionColor);
                    }
                    else
                    {
                        DrawLine(points[j], points[j + 1], Color.blue);
                    }
                }

            }

            // Process events
            if (Event.current.type == EventType.MouseMove)
            {
                bool handled = false;
                for (int i = 0; i < m_nodes.Count; i++)
                {
                    if( m_nodes[i].MouseOver(Event.current.mousePosition) )
                    {
                        handled = true;

                        break;
                    }
                }

                if( !handled )
                {
                    // Do something
                }

                Repaint();
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
                    if (!isConnectionSelected )
                    {
                        CreateNodeMenu();
                    }else
                    {
                        BreakConnectionMenu(connectionSelected.Inlet, connectionSelected.Outlet);
                    }
                }else if(!handled && Event.current.button == 0)
                {
                    m_startMousePos = Event.current.mousePosition;
                }
            }else if (Event.current.type == EventType.MouseDown && m_currentMouseMode == MouseModes.CONNECTING)
            {
                if(Event.current.button == 0)
                {
                    m_livePoints.Add(Event.current.mousePosition);
                    Repaint();
                }
                else if (Event.current.button == 1)
                {
                    m_currentMouseMode = MouseModes.IDLE;
                    m_livePoints.Clear();
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
            }

            if (m_currentMouseMode == MouseModes.CONNECTING)
            {
                List<Vector2> points = new List<Vector2>();
                points.Add(m_startMousePos);
                for(int i = 0; i < m_livePoints.Count; i++)
                {
                    points.Add(m_livePoints[i]);
                }
                points.Add(Event.current.mousePosition);

                for (int i = 0; i < points.Count - 1; i++ )
                {
                    DrawConnectingCurve(points[i], points[i+1]);
                }
                    
                Repaint();
            }

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

        float MouseDistanceToLine(Vector2 start, Vector2 end )
        {
            return DistancePointLine(Event.current.mousePosition, start, end);
        }

        void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            Color guiColor = Handles.color;
            Handles.color = color;
            Handles.DrawLine(start, end);
            GUI.color = guiColor;
        }

        void DrawConnectingCurve(Vector2 start, Vector2 end)
        {
            /*
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
             */


            Color guiColor = Handles.color;
            Handles.color = Color.red;
            Handles.DrawLine(start, end);
            GUI.color = guiColor;
        }

        public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
        }

        public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 rhs = point - lineStart;
            Vector3 vector2 = lineEnd - lineStart;
            float magnitude = vector2.magnitude;
            Vector3 lhs = vector2;
            if (magnitude > 1E-06f)
            {
                lhs = (Vector3)(lhs / magnitude);
            }
            float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
            return (lineStart + ((Vector3)(lhs * num2)));
        }
    }
}
