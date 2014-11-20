using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    public class SignalArgs { };

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
        public string Text = "Hello World!";
    };
}