using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF
{
    public class Container : Element
    {
        protected Element[] _children;

        public Element[] Children {
            get {
                return _children;
            }
            set {
                _children = value;
                for (int i = 0; i < _children.Length; i++)
                {
                    _children[i].Parent = this;
                }
            }
        }

        /// <summary>
        /// <para>
        /// Use this.<see cref="SetChildren"/> to define the children. 
        /// </para>
        /// <para>
        /// (You have to use the . notation, because it is an extension method
        /// and not a normal method)
        /// </para>
        /// </summary>
        public Container() {}

        public override void OnStart()
        {
            for(int i = 0; i < _children.Length; i++)
            {
                _children[i].Start();
            }
        }

        public override void OnUpdate()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].Update();
            }
        }

        public override void OnRender()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].Render();
            }
        }

        public override void OnResize()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].Resize();
            }
        }

        public override void OnCleanup()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                _children[i].Cleanup();
            }
        }

        public override bool ProcessEvents()
        {
            for (int i = 0; i < _children.Length; i++)
            {
                if (_children[i].ProcessEvents())
                    return true;
            }
            return false;
        }
    }
}
