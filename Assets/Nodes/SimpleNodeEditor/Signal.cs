using UnityEngine;
using System.Collections;

namespace SimpleNodeEditor
{
    public enum SignalTypes
    {
        BANG,
        STRING,
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

        public override string ToString()
        {
            return m_type.ToString();
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

        public override string ToString()
        {
           return Args.ToString();
        }

        #region HELPER_METHODS
        static public bool TryParseBool(SignalArgs args, out bool value)
        {
            bool result = false;
            switch (args.Type)
            {
                case SignalTypes.FLOAT:
                    result = System.Convert.ToBoolean(Mathf.RoundToInt(((SignalFloatArgs)args).Value));
                    value = result;
                    return true;
                case SignalTypes.STRING:
                    result = false;
                    if (bool.TryParse(((SignalStringArgs)args).Value, out result))
                    {
                        value = result;
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Cannot convert " + ((SignalStringArgs)args).Value + "to bool");
                    }
                    break;
                default:
                    Debug.LogWarning("Cannot convert signal of type " + args.ToString() + " to bool");
                    break;
            }

            value = result;

            return false;
        }
        
        static public bool TryParseInt(SignalArgs args, out int value)
        {
            int result = 0;
            switch (args.Type)
            {
                case SignalTypes.FLOAT:
                    result = System.Convert.ToInt32(Mathf.RoundToInt(((SignalFloatArgs)args).Value));
                    value = result;
                    return true;
                case SignalTypes.STRING:
                    result = 0;
                    if (int.TryParse(((SignalStringArgs)args).Value, out result))
                    {
                        value = result;
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Cannot convert " + ((SignalStringArgs)args).Value + "to int");
                    }
                    break;
                default:
                    Debug.LogWarning("Cannot convert signal of type " + args.ToString() + " to int");
                    break;
            }

            value = result;

            return false;
        }

        static public bool TryParseFloat(SignalArgs args, out float value)
        {
            float result = 0;
            switch (args.Type)
            {
                case SignalTypes.FLOAT:
                    result = ((SignalFloatArgs)args).Value;
                    value = result;
                    return true;
                case SignalTypes.STRING:
                    result = 0;
                    if (float.TryParse(((SignalStringArgs)args).Value, out result))
                    {
                        value = result;
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Cannot convert " + ((SignalStringArgs)args).Value + "to float");
                    }
                    break;
                default:
                    Debug.LogWarning("Cannot convert signal of type " + args.ToString() + " to float");
                    break;
            }

            value = result;

            return false;
        }

        static public bool TryParseString(SignalArgs args, out string value)
        {
            string result = "";
            switch (args.Type)
            {
                case SignalTypes.FLOAT:
                    value = ((SignalFloatArgs)args).Value.ToString();
                    return true;
                case SignalTypes.STRING:
                    value = ((SignalStringArgs)args).Value;
                    return true;
                default:
                    Debug.LogWarning("Cannot convert signal of type " + args.ToString() + " to string");
                    break;
            }

            value = result;

            return false;
        }
        #endregion
    }

    public class SignalStringArgs 
        : SignalArgs
    {
        public SignalStringArgs(string val) : base(SignalTypes.STRING) { Value = val; }
        public SignalStringArgs() : base(SignalTypes.STRING) { }
        public string Value = "Hello World!";
    };

    public class SignalFloatArgs
         : SignalArgs
    {
        public SignalFloatArgs(float val) : base(SignalTypes.FLOAT) { Value = val; }
        public SignalFloatArgs() : base(SignalTypes.FLOAT) { }
        public float Value = 0.0f;
    }
}