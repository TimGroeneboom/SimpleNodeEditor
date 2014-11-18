using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    [NodeMenuItem("CustomNodeExample2", typeof(CustomNodeExample2))]
    public class CustomNodeExample2 : SimpleNode
    {
        public CustomNodeExample2()
        //  : base()
        {
            m_lets.Clear();
            Name = "CustomNodeExample2";
            Size = new Vector2(180, 150);

            // create 3 inlets
            int yOffset = 25;
            for (int i = 0; i < 3; i++)
            {
                Inlet inlet = new Inlet(this);
                inlet.yOffset = yOffset * i;
                inlet.Name = "Inlet " + i.ToString();

                m_lets.Add(inlet);
            }

            // Create 2 outlets
            for (int i = 0; i < 2; i++)
            {
                Outlet outlet = new Outlet(this);
                outlet.yOffset = yOffset * (i + 3);
                outlet.Name = "Outlet " + i.ToString();

                m_lets.Add(outlet);
            }
        }
    }

}
