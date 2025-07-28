using QuickGraph;
using QuickGraph.Algorithms;

namespace QbeGenerator;

class QbeTypeSorter
{
    private List<QbeType> typeDefs = new ();
    
    public void AddType(QbeType type)
    {
        typeDefs.Add(type);
    }

    public List<QbeType> SortTypes()
    {
        var graph = new AdjacencyGraph<QbeType, Edge<QbeType>>();

        foreach (var pair in typeDefs)
        {
            graph.AddVertex(pair);
        }

        foreach (var pair in typeDefs)
        {
            var definition = pair.GetDefinition();
            foreach (var def in definition.Definitions)
            {
                if (def.RefStruct != null)
                {
                    graph.AddEdge(new Edge<QbeType>(pair, def.RefStruct));
                }
            }
        }

        var sorted = graph.TopologicalSort().Reverse().ToList();
        return sorted;
    }
}