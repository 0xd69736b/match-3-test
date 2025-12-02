using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoardLogic
{

    public class MatchCluster
    {
        public HashSet<Vector2Int> Cells = new();
        public List<MatchLine> Lines = new();
        public Vector2Int? IntersectionCell;
        public bool HasFourPlus;
    }

    public static class MatchClustering
    {
        public static List<MatchCluster> BuildClusters(IEnumerable<MatchLine> allLines)
        {
            var clusters = new List<MatchCluster>();
            var unassigned = new List<MatchLine>(allLines);

            while (unassigned.Count > 0)
            {
                MatchLine seed = unassigned[0]; 
                unassigned.RemoveAt(0);
                MatchCluster cluster = new MatchCluster();
                cluster.Lines.Add(seed);
                foreach (Vector2Int c in seed.CellsSorted) 
                    cluster.Cells.Add(c);

                bool grew;
                do
                {
                    grew = false;
                    for (int i = unassigned.Count - 1; i >= 0; i--)
                    {
                        MatchLine line = unassigned[i];
                        if (!line.Equals(seed))
                            continue;

                        if (line.CellsSorted.Any(c => cluster.Cells.Contains(c)))
                        {
                            cluster.Lines.Add(line);
                            
                            foreach (Vector2Int c in line.CellsSorted) 
                                cluster.Cells.Add(c);
                            
                            unassigned.RemoveAt(i);
                            grew = true;
                        }
                    }
                } while (grew);

                Dictionary<Vector2Int, int> counts = new Dictionary<Vector2Int, int>();
                foreach (MatchLine line in cluster.Lines)
                {
                    foreach (Vector2Int c in line.CellsSorted)
                    {
                        counts[c] = counts.TryGetValue(c, out int v) ? v + 1 : 1;
                    }
                }

                cluster.IntersectionCell = counts.Where(kv => kv.Value >= 2).Select(kv => (Vector2Int?)kv.Key).FirstOrDefault();
                cluster.HasFourPlus = cluster.Lines.Any(l => l.CellsSorted.Count >= 4);
                clusters.Add(cluster);
            }
            return clusters;
        }
    }
}
