using QuickGraph;
using QuickGraph.Algorithms;

namespace QbeGenerator;

class QbeTypeSorter
{
    private List<QbeAggregateType> typeDefs = new ();
    
    public void AddType(QbeAggregateType type)
    {
        typeDefs.Add(type);
    }

    public List<QbeAggregateType> SortTypes()
    {
        var graph = new AdjacencyGraph<QbeAggregateType, Edge<QbeAggregateType>>();

        foreach (var pair in typeDefs)
        {
            graph.AddVertex(pair);
        }

        foreach (var pair in typeDefs)
        {
            foreach (var dep in pair.Members)
            {
                var depType = typeDefs.FirstOrDefault(t =>
                {
                    if (dep.Type is QbeAggregateType aggregateType)
                    {
                        return aggregateType.Identifier == t.Identifier;
                    }

                    return false;
                });

                if (depType != null)
                {
                    graph.AddEdge(new Edge<QbeAggregateType>(pair, depType));
                }
            }
        }

        var sorted = graph.TopologicalSort().Reverse().ToList();
        return sorted;
    }
}