using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aryes.BL
{
    internal static class Graph
    {
        public delegate void MessageEventHandler(string message);

        public static event MessageEventHandler OnError = null;

        internal static BE.Graph CsvToGraph(string text)
        {
            var edges = new List<BE.Edge>();
            foreach (var line in text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith("#")) continue; //ignore comments

                var a = line.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
                if (a.Length != 2)
                {
                    OnError?.Invoke($"Line '{line}' does not contain 2 nodes.");
                    return null;
                }
                edges.Add(new BE.Edge { Source = a[0], Target = a[1] });
            }

            return new BE.Graph { Nodes = GetNodes(edges), Edges = edges.ToArray() };
        }

        private static string[] GetNodes(IEnumerable<BE.Edge> edges)
        {
            var nodes = new List<string>();
            foreach (var edge in edges)
            {
                if (!nodes.Contains(edge.Source))
                    nodes.Add(edge.Source);
                if (!nodes.Contains(edge.Target))
                    nodes.Add(edge.Target);
            }
            return nodes.ToArray();
        }

        internal static string GraphToTgf(BE.Graph graph)
        {
            var nodes = graph.Nodes;
            var edges = graph.Edges;
            var sb = new StringBuilder();
            for (var i = 0; i < nodes.Length; i++)
            {
                sb.AppendFormat("{0} {1}", i, nodes[i]);
                sb.AppendLine();
            }

            sb.AppendLine("#");

            foreach (var edge in edges)
            {
                sb.AppendFormat("{0} {1}", Array.IndexOf(nodes, edge.Source), Array.IndexOf(nodes, edge.Target));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        internal static char[] Delimiters { get; set; }

        public static BE.Graph Isolate(BE.Graph graph, string root)
        {
            var edges = new List<BE.Edge>();
            AddDescendents(edges, graph, root);
            return new BE.Graph { Nodes = GetNodes(edges), Edges = edges.ToArray() };
        }

        private static void AddDescendents(List<BE.Edge> edges, BE.Graph graph, string root)
        {
            var more = graph.Edges
                .Where(edge => edge.Source.Equals(root)
                    && !edges.Exists(o => o.Source.Equals(edge.Source) && o.Target.Equals(edge.Target)))
                .ToList();
            if (more.Count > 0)
            {
                edges.AddRange(more);
                foreach (var edge in more)
                    AddDescendents(edges, graph, edge.Target);
            }
        }
    }
}