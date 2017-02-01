using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DSBroker.Node
{
    public class BrokerNode
    {
        private readonly Dictionary<string, BrokerNode> Children = new Dictionary<string, BrokerNode>();
        private bool _accessible = true;

        public readonly string Name;
        public readonly BrokerNode Parent;
        public readonly string Profile;
        public readonly string Path;

        public bool Accessible
        {
            get
            {
                return _accessible;
            }
            set
            {
                // TODO
                /*boolean post = this.accessible != accessible;
                if (post && parent != null)
                {
                    BrokerNode parent = this.parent.get();
                    if (parent != null)
                    {
                        parent.childUpdate(this, !accessible);
                    }
                }*/
                _accessible = value;
            }
        }

        public BrokerNode(string name, BrokerNode parent)
            : this(name, parent, "node")
        {
        }

        public BrokerNode(string name, BrokerNode parent, string profile)
        {
            if (string.IsNullOrEmpty(profile))
            {
                throw new ArgumentException("Profile must not be null or empty");
            }
            Profile = profile;
            if (parent == null)
            {
                Path = "";
            }
            else
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Name must not be null or empty");
                }
                Parent = parent;
                Name = name;
                Path = Parent.Path + "/" + name;
            }
        }

        public BrokerNode GetChild(string name)
        {
            if (!Children.ContainsKey(name))
            {
                return null;
            }
            return Children[name];
        }

        public bool HasChild(string name)
        {
            return Children.ContainsKey(name);
        }

        public void AddChild(BrokerNode child)
        {
            Children[child.Name] = child;
            // TODO
            //if (child.accessible())
            //{
            //    childUpdate(child, false);
            //}
        }

        public void ToTree(JObject parent)
        {
            var self = new JObject();
            var name = Name == null ? "root" : Name;
            parent.Add(name, self);
            foreach (KeyValuePair<string, BrokerNode> child in Children)
            {
                child.Value.ToTree(self);
            }
        }
    }
}
