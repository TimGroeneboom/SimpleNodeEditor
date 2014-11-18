using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNodeEditor
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class NodeMenuItem : System.Attribute
    {
        private string m_name;
        public string Name { get { return m_name; } }
        public System.Type Type;

        public NodeMenuItem(string name, System.Type type)
        {
            m_name = name;
            Type = type;
        }
    }
}
