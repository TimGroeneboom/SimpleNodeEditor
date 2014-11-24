using UnityEngine;
using System.Collections;
using UnityEditor;

namespace SimpleNodeEditor
{
    [CustomEditor(typeof(NodeGraph))]
    public class NodeGraphInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            NodeGraph myTarget = (NodeGraph)target;
            if( GUILayout.Button("Show Graph") )
            {
                ShowGraph(myTarget);
            }
        }

        public void OnShowGraphClicked()
        {

        }

        public void ShowGraph(NodeGraph sender)
        {
            NodeEditor nodeEditor = (NodeEditor) EditorWindow.GetWindow(typeof(NodeEditor));
            nodeEditor.ConnectionColor = sender.ConnectionColor;
            nodeEditor.Root = sender.gameObject;
            nodeEditor.MasterInlet = sender.Inlet;
            nodeEditor.MasterOutlet = sender.Outlet;
            nodeEditor.Show();
        }
    }
}
