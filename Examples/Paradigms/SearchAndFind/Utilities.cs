using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.BeMoBI.Paradigms.SearchAndFind
{
    public static class Utilities
    {
        public static Dictionary<int, List<PathInMaze>> GetGroupedByDifficulty(this List<PathInMaze> paths)
        {
           var pathsAsSetOfPathElements = paths.GroupBy(p => p.GetDifficultyByCountTJunctions());

            return pathsAsSetOfPathElements.ToDictionary(g => g.Key, g => g.ToList());
        }

        public static int GetDifficultyByCountTJunctions(this PathInMaze path)
        {
            var pathElements = path.PathAsLinkedList.ToList();

            return pathElements.Count(e => e.Type == UnitType.T);
        }
    }
}
