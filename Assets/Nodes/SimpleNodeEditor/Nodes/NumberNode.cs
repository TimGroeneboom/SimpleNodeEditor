using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleNodeEditor
{
    [NodeMenuItem("NumberNode", typeof(NumberNode))]
    public class NumberNode : BaseNode
    {
        [SerializeField]
        Outlet outlet = null;

        public float Value = 0.0f;

        protected override void Inited()
        {
        }

        public override void Construct()
        {
            Name = "NumberNode";

            outlet = (Outlet)MakeLet(LetTypes.OUTLET);

            Size = new Vector2(Size.x, 100);
        }

#if UNITY_EDITOR
        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 25, 100, 75));
            //EditorGUIUtility.LookLikeControls(30, 30);

            Value = EditorGUILayout.FloatField(Value, GUILayout.MaxWidth(80));

            if (GUILayout.Button("Emit", GUILayout.MaxWidth(80)))
            {
                SignalFloatArgs floatArgs = new SignalFloatArgs();
                floatArgs.Value = Value;

                outlet.Send(floatArgs);
            }

            GUI.EndGroup();

            base.WindowCallback(id);
        }
#endif
    }


}
