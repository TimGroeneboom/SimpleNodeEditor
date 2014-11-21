using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    public enum SignalTypes
    {
        BANG,
        TEXT,
        FLOAT
    }

    public class SignalArgs 
    {
        public SignalArgs(){}
        public SignalArgs(SignalTypes type)
        {
            m_type = type;
        }

        protected SignalTypes m_type = SignalTypes.BANG;
        public SignalTypes Type
        {
            get
            {
                return m_type;
            }
        }
    };

    public delegate void SignalHandler(Signal signal);

    public class Signal
    {
        public Signal(Let sender, SignalArgs args)
        {
            Sender = sender;
            Args = args;
        }

        public Let Sender = null;
        public SignalArgs Args = null;
    }

    public class SignalTextArgs 
        : SignalArgs
    {
        public SignalTextArgs() : base(SignalTypes.TEXT) { }
        public string Text = "Hello World!";
    };

    public class SignalFloatArgs
         : SignalArgs
    {
        public SignalFloatArgs() : base(SignalTypes.FLOAT) { }
        public float Value = 0.0f;
    }
}