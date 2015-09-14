using System.Collections.Generic;
using JetBrains.Annotations;

namespace PathFinder
{
    public delegate float HeuristicFunc(INode from, INode to);

    public interface IPathFinder
    {
        HeuristicFunc Heuristic { get; }

        IEnumerable<INode> FindPath(INode startNode, INode endNode);
    }
}