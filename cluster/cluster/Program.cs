using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cluster
{
    class Program
    {
        static new List<Dictionary<string, object>> clusters;
        static List<int[]> points;
        static Dictionary<int, Dictionary<int, double>> distances;

        static void Main(string[] args)
        {
            // initialise points
            points = new List<int[]>();

            points.Add(new int[] {2, 10});
            points.Add(new int[] {2, 5});
            points.Add(new int[] {8, 4});
            points.Add(new int[] {5, 8});
            points.Add(new int[] {7, 5});
            points.Add(new int[] {6, 4});
            points.Add(new int[] {1, 2});
            points.Add(new int[] {4, 9});


            // initialise distance matrix
            distances = new Dictionary<int, Dictionary<int, double>>();
            for (int i = 0; i < 8; i++)
            {
                Dictionary<int, double> dict = new Dictionary<int, double>();
                distances[i] = dict;
            }

            for (int i = 0; i < 8; i++)
            {
                int[] p1 = points[i];
                for (int j = 0; j < 8; j++)
                {
                    int[] p2 = points[j];

                    double x = Math.Sqrt( Math.Pow(p1[0] - p2[0], 2) + Math.Pow(p1[1] - p2[1], 2));
                    distances[i][j] = x;
                    distances[j][i] = x;
                }
            }


            // initialise clusters
            clusters = new List<Dictionary<string, object>>();
            for (int i = 0; i < 8; i++)
            {
                Dictionary<string, object> d = new Dictionary<string, object>();
                List<int> mem = new List<int> { i };
                d["members"] = mem;
                d["leaf"] = true;
                d["root"] = false;
                clusters.Add(d);
            }

            // group until no clusters left
            while (clusters.Count > 1)
            {
                double minDist = 99999;
                int min_c1 = 0, min_c2 = 0;

                // find pair of clusters with min distance
                for (int c1 = 0; c1 < clusters.Count; c1++)
                {
                    for (int c2 = c1 + 1; c2 < clusters.Count; c2++)
                    {
                        double dist = getDist(clusters[c1], clusters[c2]);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            min_c1 = c1;
                            min_c2 = c2;
                        }
                    }
                }

                // make tree
                Dictionary<string, object> parent = new Dictionary<string, object>();
                parent["leaf"] = false;
                parent["root"] = false;
                parent["child_dist"] = minDist;

                Dictionary<string, object> left_child = clusters[min_c1];
                Dictionary<string, object> right_child = clusters[min_c2];
                

                parent["left"] = left_child;
                parent["right"] = right_child;
                left_child["parent"] = parent;
                right_child["parent"] = parent;

                List<int> mem = new List<int>();
                mem.AddRange((List<int>)(left_child["members"]));
                mem.AddRange((List<int>)(right_child["members"]));
                parent["members"] = mem;

                parent["dist_left"] = 0;
                if ((bool)left_child["leaf"] != true)
                {
                    parent["dist_left"] = (minDist - (double)left_child["child_dist"]) / 2;
                }

                parent["dist_right"] = 0;
                if ((bool)right_child["leaf"] != true)
                {
                    parent["dist_right"] = (minDist - (double)right_child["child_dist"]) / 2;
                }

                // update the list of clusters to reflect the merge
                // important to remove c2 first
                clusters.RemoveAt(min_c2);
                clusters.RemoveAt(min_c1);
                clusters.Add(parent);
            }

            // navigate the tree
            Dictionary<string, object> curr = clusters[0];
            curr["root"] = true;
            while (true)
            {
                // display info available at this node
                string mems = "";
                List<int> members = (List<int>)curr["members"];
                foreach (int x in members)
                {
                    mems += x + ", ";
                }
                Console.WriteLine("this node contains " + mems);
                if ((bool)curr["leaf"] != true)
                {
                    Console.WriteLine("left weight " + curr["dist_left"]);
                    Console.WriteLine("right weight " + curr["dist_right"]);
                    Console.WriteLine("child dist " + curr["child_dist"]);
                }

                // read user input and navigate
                string cmd = Console.ReadLine();
                if (cmd == "w")
                {
                    if ((bool)curr["root"])
                    {
                        Console.WriteLine("Already at root!");
                    }
                    else
                    {
                        curr = (Dictionary<string, object>)curr["parent"];
                    }
                }
                else if ((bool) curr["leaf"])
                {
                    Console.WriteLine("Already at leaf!");
                }
                else
                {
                    if (cmd == "a")
                    {
                        curr = (Dictionary<string, object>)curr["left"];
                    }
                    else if (cmd == "d")
                    {
                        curr = (Dictionary<string, object>)curr["right"];
                    }
                    else
                    {
                        Console.WriteLine("error, unknown command");
                    }
                }
            }
        }

        static double getDist(Dictionary<string, object> cluster1, Dictionary<string, object> cluster2)
        {
            List<int> c1 = (List<int>) cluster1["members"];
            List<int> c2 = (List<int>) cluster2["members"];

            double sum = 0;

            foreach (int p1 in c1)
            {
                foreach (int p2 in c2)
                {
                    //Console.WriteLine(distances[p1][p2]);
                    sum += distances[p1][p2];
                }
            }
            //Console.WriteLine("done");
            sum /= (c1.Count * c2.Count);

            return sum;
        }


    }
}
