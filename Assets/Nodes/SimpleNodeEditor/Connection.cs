using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNodeEditor
{
    [System.Serializable]
    public class Connection : IComparable<Connection>
    {
        public Connection(Inlet inlet, Outlet outlet, List<Vector2> points)
        {
            Outlet = outlet;
            Inlet = inlet;

            // TODO : make this better
            // remove last point if to close to connecting inlet
            if (points.Count > 0)
            {
                if (Vector2.Distance(inlet.Position.center, points[points.Count - 1]) < 5)
                {
                    points.RemoveAt(points.Count - 1);
                }
            }

            points.Reverse();
            Points = points.ToArray();
        }

        public int CompareTo(Connection compareConnection)
        {
            // A null value means that this object is greater. 
            if (compareConnection == null)
                return 1;

            else
                return this.Outlet.Position.y.CompareTo(compareConnection.Outlet.Position.y);
        }

        public Outlet Outlet = null;
        public Inlet Inlet = null;

        public Vector2[] Points = null;
    }
}
