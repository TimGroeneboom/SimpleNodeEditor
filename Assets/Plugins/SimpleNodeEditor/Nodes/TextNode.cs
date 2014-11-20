using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("TextNode", typeof(TextNode))]
    public class TextNode : BaseNode
    {
        [SerializeField]
        Outlet outlet = null;

        public string Value = "";

        protected override void Inited()
        {
        }

        public override void Construct()
        {
            Name = "TextNode";

            outlet = (Outlet)MakeLet(LetTypes.OUTLET);

            Size = new Vector2(Size.x, 100);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 25, 100, 75));
            //EditorGUIUtility.LookLikeControls(30, 30);

            Value = GUILayout.TextField(Value, GUILayout.MaxWidth(80));

            if (GUILayout.Button("Emit", GUILayout.MaxWidth(80)))
            {
                SignalTextArgs textArgs = new SignalTextArgs();
                textArgs.Text = Value;

                outlet.Emit(new Signal(outlet, textArgs));
            }

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
