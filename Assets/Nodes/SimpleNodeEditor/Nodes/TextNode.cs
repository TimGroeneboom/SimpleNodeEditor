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

            Size = new Vector2(200, 100);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 25, 180, 75));
            //EditorGUIUtility.LookLikeControls(30, 30);

            Value = GUILayout.TextField(Value, GUILayout.MaxWidth(180));

            EditorGUILayout.Space();

            if (GUILayout.Button("Emit", GUILayout.MaxWidth(180)))
            {
                SignalStringArgs textArgs = new SignalStringArgs();
                textArgs.String = Value;

                outlet.Send(textArgs);
            }

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
