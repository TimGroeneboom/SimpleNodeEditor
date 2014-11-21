using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("ButtonNode", typeof(ButtonNode))]
    public class ButtonNode : BaseNode
    {
        [SerializeField]
        Outlet outlet = null;

        protected override void Inited()
        {
        }

        public override void Construct()
        {
            Name = "ButtonNode";

            outlet = (Outlet)MakeLet(LetTypes.OUTLET);

            Size = new Vector2(Size.x, 75);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 25, 100, 50));
            //EditorGUIUtility.LookLikeControls(30, 30);

            if( GUILayout.Button("Emit", GUILayout.MaxWidth(80) ) )
            {
                outlet.Send(new SignalArgs());
            }

            GUI.EndGroup();

 	        base.WindowCallback(id);
        }
#endif
    }


}
