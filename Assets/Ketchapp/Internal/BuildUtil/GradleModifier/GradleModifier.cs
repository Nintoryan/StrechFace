using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Ketchapp.Internal.BuildUtil.Gradle
{
    public class GradleModifier
    {
        private GradleNode _root;
        private string _filePath;
        private GradleNode _curNode;

        public GradleNode Root => _root;

        public GradleModifier(string filePath)
        {
            string file = File.ReadAllText(filePath);
            TextReader reader = new StringReader(file);

            _filePath = filePath;
            _root = new GradleNode("root");
            _curNode = _root;

            StringBuilder str = new StringBuilder();
            var prevC = (char)0;

            while (reader.Peek() > 0)
            {
                char c = (char)reader.Read();
                switch (c)
                {
                    case ' ':
                        if (reader.Peek() == '/' && prevC == '\u0027')
                        {
                            var formatString = FormatStr(str);

                            if (!string.IsNullOrEmpty(formatString))
                            {
                                _curNode.AppendChildNode(new GradleContentNode(formatString, _curNode));
                            }

                            str = new StringBuilder();
                        }
                        else
                        {
                            str.Append(c);
                        }

                        break;
                    case '/':
                        if (reader.Peek() == '/' && prevC != ':' && prevC != '/')
                        {
                            reader.Read();
                            string comment = reader.ReadLine();
                            _curNode.AppendChildNode(new GradleCommentNode(comment, _curNode));
                        }
                        else
                        {
                            str.Append('/');
                        }

                        break;
                    case '\n':
                        {
                            var strf = FormatStr(str);
                            if (!string.IsNullOrEmpty(strf))
                            {
                                _curNode.AppendChildNode(new GradleContentNode(strf, _curNode));
                            }
                        }

                        str = new StringBuilder();
                        break;
                    case '\r':
                        break;
                    case '\t':
                        break;
                    case '{':
                        {
                            var n = FormatStr(str);
                            if (!string.IsNullOrEmpty(n))
                            {
                                GradleNode node = new GradleNode(n, _curNode);
                                _curNode.AppendChildNode(node);
                                _curNode = node;
                            }
                        }

                        str = new StringBuilder();
                        break;
                    case '}':
                        {
                            var strf = FormatStr(str);
                            if (!string.IsNullOrEmpty(strf))
                            {
                                _curNode.AppendChildNode(new GradleContentNode(strf, _curNode));
                            }

                            _curNode = _curNode.Parent;
                        }

                        str = new StringBuilder();
                        break;
                    default:
                        str.Append(c);
                        break;
                }

                prevC = c;
            }
        }

        public void Save(string path = null)
        {
            if (path == null)
            {
                path = _filePath;
            }

            File.WriteAllText(path, Print());
        }

        private string FormatStr(StringBuilder sb)
        {
            string str = sb.ToString();
            str = str.Trim();
            return str;
        }

        public string Print()
        {
            StringBuilder stringBuilder = new StringBuilder();
            PrintNode(stringBuilder, _root, 0);
            return stringBuilder.ToString();
        }

        private string GetLevelIndent(int level)
        {
            if (level <= 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(string.Empty);
            for (int i = 0; i < level; i++)
            {
                sb.Append('\t');
            }

            return sb.ToString();
        }

        private void PrintNode(StringBuilder stringBuilder, GradleNode node, int level)
        {
            if (node.Parent != null)
            {
                if (node is GradleCommentNode)
                {
                    stringBuilder.Append("\n" + GetLevelIndent(level) + @"//" + node.Name);
                }
                else
                {
                    stringBuilder.Append("\n" + GetLevelIndent(level) + node.Name);
                }

            }

            if (!(node is GradleContentNode) && !(node is GradleCommentNode))
            {
                if (node.Parent != null)
                {
                    stringBuilder.Append(" {");
                }

                foreach (var c in node.Children)
                {
                    PrintNode(stringBuilder, c, level + 1);
                }

                if (node.Parent != null)
                {
                    stringBuilder.Append("\n" + GetLevelIndent(level) + "}");
                }
            }
        }
    }

    public class GradleNode
    {
        [JsonIgnore]
        protected List<GradleNode> _children = new List<GradleNode>();
        [JsonIgnore]
        protected GradleNode _parent;
        [JsonIgnore]
        protected string _name;
        [JsonIgnore]
        public GradleNode Parent => _parent;

        public string Name => _name;

        public List<GradleNode> Children => _children;

        public GradleNode(string name, GradleNode parent = null)
        {
            _parent = parent;
            _name = name;
        }

        public void Each(Action<GradleNode> f)
        {
            f(this);
            foreach (var n in _children)
            {
                n.Each(f);
            }
        }

        public void AppendChildNode(GradleNode node)
        {
            if (_children == null)
            {
                _children = new List<GradleNode>();
            }

            _children.Add(node);
            node._parent = this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"> Sample "android/signingConfigs/release"</param>
        /// <returns></returns>
        public GradleNode TryGetNode(string path)
        {
            string[] subPath = path.Split('/');
            GradleNode cNode = this;
            for (int i = 0; i < subPath.Length; i++)
            {
                var p = subPath[i];
                if (string.IsNullOrEmpty(p))
                {
                    continue;
                }

                GradleNode tNode = cNode.FindChildNodeByName(p);
                if (tNode == null)
                {
                    Debug.Log("Can't find Node:" + p);
                    return null;
                }

                cNode = tNode;
                tNode = null;
            }

            return cNode;
        }

        public GradleNode FindChildNodeByName(string name)
        {
            foreach (var n in _children)
            {
                if (n is GradleCommentNode || n is GradleContentNode)
                {
                    continue;
                }

                if (n.Name == name)
                {
                    return n;
                }
            }

            return null;
        }

        public bool ReplaceContenStartsWith(string patten, string value)
        {
            foreach (var n in _children)
            {
                if (!(n is GradleContentNode))
                {
                    continue;
                }

                if (n._name.StartsWith(patten))
                {
                    n._name = value;
                    return true;
                }
            }

            return false;
        }

        public GradleContentNode ReplaceContentOrAddStartsWith(string patten, string value)
        {
            foreach (var n in _children)
            {
                if (!(n is GradleContentNode))
                {
                    continue;
                }

                if (n._name.StartsWith(patten))
                {
                    n._name = value;
                    return (GradleContentNode)n;
                }
            }

            return AppendContentNode(value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public GradleContentNode AppendContentNode(string content)
        {
            foreach (var n in _children)
            {
                if (!(n is GradleContentNode))
                {
                    continue;
                }

                if (n._name == content)
                {
                    Debug.Log("GradleContentNode with " + content + " already exists!");
                    return null;
                }
            }

            GradleContentNode cNode = new GradleContentNode(content, this);
            AppendChildNode(cNode);
            return cNode;
        }


        public bool RemoveContentNode(string contentPattern)
        {
            for (var i = 0; i < _children.Count; i++)
            {
                if (!(_children[i] is GradleContentNode))
                {
                    continue;
                }

                if (!_children[i]._name.Contains(contentPattern))
                {
                    continue;
                }

                _children.RemoveAt(i);
                return true;
            }

            return false;
        }
    }

    public sealed class GradleContentNode : GradleNode
    {
        public GradleContentNode(string content, GradleNode parent)
            : base(content, parent)
        {
        }

        public void SetContent(string content)
        {
            _name = content;
        }
    }

    public sealed class GradleCommentNode : GradleNode
    {
        public GradleCommentNode(string content, GradleNode parent)
            : base(content, parent)
        {
        }

        public string GetComment()
        {
            return _name;
        }
    }
}
