using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    public delegate void ShowNodeGraphClickedEventHandler(NodeGraph sender);

    [NodeMenuItem("NodeGraph", typeof(NodeGraph))]
    [ExecuteInEditMode]
    public class NodeGraph 
        : BaseNode
    {
        public event ShowNodeGraphClickedEventHandler ShowNodeGraphClicked = (NodeGraph sender)=>{};

        public Color ConnectionColor = Color.green;

        [SerializeField]
        Inlet m_inlet = null;
        public Inlet Inlet{ get{ return m_inlet; } }

        [SerializeField]
        Outlet m_outlet = null;
        public Outlet Outlet { get { return m_outlet; } }

        public PassThruNode MasterInlet = null;
        public PassThruNode MasterOutlet = null;

        [SerializeField][HideInInspector]
        private bool m_inited = false;

        void OnInletReceived(Signal signal)
        {
            MasterInlet.SendSignal(signal);
        }

        void OnMasterOutletReceivedSignal(Signal signal)
        {
            m_outlet.Send(signal);
        }

        void Awake()
        {
            if(!m_inited)
            {
                Construct();
                m_inited = true;
            }
        }

        public override void Construct()
        {
            if(!m_inited)
            {
                Name = "NodeGraph";

                m_inlet = MakeLet<Inlet>("Input");
                m_outlet = MakeLet<Outlet>("Output", 25);
                Size = new Vector2(Size.x, 100);

                GameObject masterInletObject = new GameObject("Graph Inlet");
                MasterInlet = masterInletObject.AddComponent<PassThruNode>();
                MasterInlet.Position = new Vector2(10, 100);
                MasterInlet.transform.parent = transform;
                MasterInlet.ShowCloseButton = false;
                MasterInlet.Construct();
                MasterInlet.Name = "Graph Inlet";

                MasterInlet.Inlet.Visible = false;

                GameObject masterOutletObject = new GameObject("Graph Outlet");
                MasterOutlet = masterOutletObject.AddComponent<PassThruNode>();
                MasterOutlet.Position = new Vector2(500, 500);
                MasterOutlet.transform.parent = transform;
                MasterOutlet.ShowCloseButton = false;
                

                MasterOutlet.Construct();
                MasterOutlet.Name = "Graph Outlet";

                MasterOutlet.Outlet.Visible = false;
            }

        }

        protected override void Inited()
        {
            m_inlet.SlotReceivedSignal += OnInletReceived;
            MasterOutlet.OnSignalReceived += OnMasterOutletReceivedSignal;
        }

        public void HideLets()
        {
            MasterInlet.Visible = false;
            MasterOutlet.Visible = false;
        }

        public void ShowLets()
        {
            MasterInlet.Visible = true;
            MasterOutlet.Visible = true;
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 100, 50));
            //EditorGUIUtility.LookLikeControls(30, 30);

            if( GUILayout.Button("Show", GUILayout.MaxWidth(80) ) )
            {
                ShowNodeGraphClicked(this);
            }

            GUI.EndGroup();

 	        base.WindowCallback(id);
        }
#endif
    }
}


