using System;

namespace StrangeEngine
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class HasSortingLayerName : Attribute
    {
        string[] _names;
        public string[] Names { get { return _names; } }
        public HasSortingLayerName(params string[] names) { _names = names; }
    }
}
