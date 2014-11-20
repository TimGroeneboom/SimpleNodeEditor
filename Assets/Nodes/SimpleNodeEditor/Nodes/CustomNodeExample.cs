using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    [NodeMenuItem("CustomNodeExample", typeof(CustomNodeExample))]
    public class CustomNodeExample : BaseNode
    {
        [SerializeField]
        Outlet outlet1;

        [SerializeField]
        Outlet outlet2;

        [SerializeField]
        Inlet inlet1;

        void OnInletOneReceivedSignal(Signal signal)
        {
            outlet1.Emit(signal);

            Debug.Log("Received signal!");
        }

        protected override void Inited()
        {
            inlet1.SlotReceivedSignal += OnInletOneReceivedSignal;
        }

        public override void Construct()
        {
            m_lets.Clear();
            Name = "CustomNodeExample2";
            Size = new Vector2(180, 150);

            // create 3 inlets
            int yOffset = 25;
            for (int i = 0; i < 3; i++)
            {
                Inlet inlet = gameObject.AddComponent<Inlet>();
                inlet.Construct(this);

                inlet.yOffset = yOffset * i;
                inlet.Name = "Inlet " + i.ToString();

                m_lets.Add(inlet);

                // add listener to inlet 1
                if( i == 0 )
                {
                    inlet1 = inlet;
                }
            }

            // Create 2 outlets
            for (int i = 0; i < 2; i++)
            {
                Outlet outlet = gameObject.AddComponent<Outlet>();
                outlet.Construct(this);

                outlet.yOffset = yOffset * (i + 3);
                outlet.Name = "Outlet " + i.ToString();

                m_lets.Add(outlet);

                if (i == 0)
                    outlet1 = outlet;
                if (i == 1)
                    outlet2 = outlet;
            }
        }

        [ContextMenu("FireOne")]
        void FireSignalOnOutlet1()
        {
            outlet1.Emit(new Signal(outlet1, new SignalArgs()));
        }

        [ContextMenu("FireTwo")]
        void FireSignalOnOutlet2()
        {
            outlet2.Emit(new Signal(outlet2, new SignalArgs()));
        }
    }

}
