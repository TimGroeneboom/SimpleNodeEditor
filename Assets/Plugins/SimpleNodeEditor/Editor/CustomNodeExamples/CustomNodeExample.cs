using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    [NodeMenuItem("CustomNodeExample", typeof(CustomNodeExample))]
    public class CustomNodeExample : SimpleNode
    {
        public CustomNodeExample() 
          //  : base()
        {
            m_lets.Clear();
            Name = "CustomNodeExample";
            Size = new Vector2(180, 200);

            // create 5 outlets
            int yOffset = 25;
            for (int i = 0; i < 5; i++)
            {
                Outlet outlet = new Outlet(this);
                outlet.yOffset = yOffset * i;
                outlet.Name = "Outlet " + i.ToString();

                m_lets.Add(outlet);
            }
        }
    }

}
