using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF
{
    /// <summary>
    /// When overriding methods in a container, you should be calling base.OnRender() to render all the children.
    /// But if you want to render a specific child, you should be calling child[i].Render() instead.
    /// This is because the base.Render() method will call OnRender, so if OnRender then calls base.Render, you will get
    /// A stack-overflow exception from infinite recursion.
    /// </summary>
    public class Container : Element
    {
        protected Element[] _children;

        static readonly Element[] NULL_ARRAY = new Element[0];

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
        public Container() {
            _children = NULL_ARRAY;
        }

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

        /// <summary>
        /// For containers, override OnProcessEvents instead
        /// </summary>
        /// <returns></returns>
        public override bool ProcessEvents()
        {
            bool result = false;
            for (int i = 0; i < _children.Length; i++)
            {
                result = result || _children[i].ProcessEvents();
            }

            if (result)
                return true;

            return OnProcessEvents();
        }

        public virtual bool OnProcessEvents()
        {
            return false;
        }
    }
}
