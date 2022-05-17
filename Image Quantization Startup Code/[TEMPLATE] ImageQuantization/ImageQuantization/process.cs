using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class process
    {
        //List of Distinct Color
        public static List<int> DistinctColorList = new List<int>();
        public static void DistinctColor() //get distinct color
        {
            //get image
            var img = MainForm.ImageMatrix;
            //get width and height of image
            int w = ImageOperations.GetWidth(img);
            int h = ImageOperations.GetHeight(img);
            //List of Distinct Color
            HashSet<int> dist = new HashSet<int>();
            //calc RGB
            int red, green, blue;

            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    red = img[row, col].red;
                    green = img[row, col].green;
                    blue = img[row, col].blue;
                    //shift left to each pixel color then save in list
                    int colorPixel = (red << 16) + (green << 8) + blue;
                    //if not found found before add to list
                    if (!dist.Contains(colorPixel))
                        dist.Add(colorPixel);
                }
            }
            DistinctColorList = dist.ToList();

        }

        private static double Weight_EuclideanDistance_2virtices(Vertix V1, Vertix V2)
        {
            //calc Weight with EuclideanDistance between 2virtices
            int red_V1, red_V2, green_V1, green_V2, blue_V1, blue_V2,color1, color2;
            //get pixel of each color
            color1 = Convert.ToInt32(V1.vertix);
            color2 = Convert.ToInt32(V2.vertix);
            //shift right to get colors
            red_V1 = (byte)(color1 >> 16);
            red_V2 = (byte)(color2 >> 16);
            green_V1 = (byte)(color1 >> 8);
            green_V2 = (byte)(color2 >> 8);
            blue_V1 = (byte)(color1);
            blue_V2 = (byte)(color2);
            //(color1 - color2 * color1 - color2) then sqrt
            double d1 = (red_V2 - red_V1),
                    d2 = (green_V2 - green_V1),
                    d3 = (blue_V2 - blue_V1),
                    sum = (double)Math.Sqrt(Math.Abs(d1*d1) + Math.Abs(d2*d2) + Math.Abs(d3*d3));
            return sum;
        }

        //list of mst
        public static List<Vertix> MST = new List<Vertix>();
        public static double Generate_MST()
        {
            //priority queue with num of distinct color
            Priority_Queue<Vertix> priorityQueue = new Priority_Queue<Vertix>(DistinctColorList.Count);

            //loop on distinct color and put it in queue
            for (int i = 0; i < DistinctColorList.Count; i++)
            {
                double OO = double.MaxValue;
                //fisrt vertix = first distinct color && parent = null(-1) && weight=0
                if (i == 0)
                    priorityQueue.Enqueue(new Vertix(DistinctColorList[i], -1, 0));
                else
                    //set weight oo and parent oo
                    priorityQueue.Enqueue(new Vertix(DistinctColorList[i], OO, OO));
            }

            while (true)
            {
                //get min weight
                Vertix MinVert = priorityQueue.Dequeue();
                //save edge and add to Mst List
                MST.Add(MinVert);

                //loop on queue and set weight again
                double calcWeight = -1;
                foreach (var v in priorityQueue)
                {
                    //calc weight between min vert and each Vertix in queue
                    calcWeight = Weight_EuclideanDistance_2virtices(MinVert, v);
                    //if weight less than me set weight
                    if (v.Weight > calcWeight)
                    {
                        //set weight
                        v.Weight = calcWeight;
                        //set parent
                        v.Parent = MinVert.vertix;
                        //update graph
                        priorityQueue.VertixUpdated(v);
                    }
                }
                //not more vertix in queue break
                if (priorityQueue.Count() == 1)
                {
                    //dequeue last edge
                    MinVert = priorityQueue.Dequeue();
                    //add last edge to Mst List
                    MST.Add(MinVert);
                    break;
                }
            }
            //loop on vertix in mst and sum it
            double weight = 0;
            foreach (var v in MST)
            {
                weight += v.Weight;
            }
            return weight; //return min weight
        }

        private static void delete_max_edge(int k)
        {
            while (--k != 0) //set max weight edge with -10
            {
                double maxweight = -10;
                int index = -10;
                //loopinh on edges to get edges with max weight
                for (int i = 0; i < MST.Count; i++)
                {
                    //condition to filter maximum weight
                    if (MST[i].Weight >= maxweight)
                    {
                        // swap to set max_weight at max variable
                        maxweight = MST[i].Weight;
                        //save index edge with max weight
                        index = i;
                    }
                }
                //set weight with abnormal number
                MST[index].Weight = 0;
            }
        }
        //list of list of clusters
        public static HashSet<HashSet<int>> clusters = new HashSet<HashSet<int>>();
        public static void Cluster(int k)
        {
            HashSet<int> Deleteed_set = new HashSet<int>();
            HashSet<int> Set = new HashSet<int>();
            //loop on k-1 and delete max weight
            delete_max_edge(k);

            for (int i = 0; i < MST.Count; i++)
            {
                //create set with orher edges ->cluster 
                if (MST[i].Weight == 0)
                {
                    // set edges with abnormal weight at cut set then delete its from graph
                    Deleteed_set.Add((int)MST[i].vertix);
                    Deleteed_set.Add((int)MST[i].Parent);
                    // check set count to add to clusre or not
                    if (Set.Count != 0) //not empty
                    {
                        // declare new set to copy set
                        HashSet<int> Copy_srt = new HashSet<int>();
                        foreach (var unit in Set)
                        {
                            Copy_srt.Add(unit);
                        }
                        clusters.Add(Copy_srt);
                    }
                    //clear old set (clean to re fill it)
                    Set.Clear();
                }
                else //!=0 with max edge
                {
                    // add its as a set to cluster
                    Set.Add((int)MST[i].vertix);
                    Set.Add((int)MST[i].Parent);
                }
            }
            //if more vertix in list add to clusters
            if (Set.Count != 0)
            {
                clusters.Add(Set);
            }
            // add athor sets to cluster
            foreach (var vertic in Deleteed_set) // set with max weight
            {
                int f = 0;
                foreach (var set in clusters)
                {
                    //if cluster contain deleted set element do nothing else added it ->cluster
                    if (set.Contains(vertic))
                    {
                        f = 1;
                        break;
                    }

                }
                // not found
                if (f == 0)
                {
                    HashSet<int> s = new HashSet<int>();
                    // set of set
                    s.Add(vertic);
                    //hash set of hash set
                    clusters.Add(s);
                }
            }

        }
        
        public static int shaft_right(int var, int index)//shift right to get pixel of color
        {
            for (int j = 0; j < index; j++)
            {
                var /= 2;
            }
            return var;
        }
        public static int shaft_lift(int var, int index)//shift left to add colors to each others
        {
            for (int j = 0; j < index; j++)
            {
                var *= 2;
            }
            return var;
        }
        public static int Averadge_Cluster(HashSet<int> var) //avg of each list in cluster
        {
            int redd = 0, greenn = 0, bluee = 0, value = 0, key1 = 0, key2 = 0;
            foreach (var var1 in var)
            {
                var red = var1;
                // redd += (byte)(shaft_right(ref red, 16));
                // r << 16
                redd += (byte)(red >> 16);
                var green = var1;
                greenn += (byte)(green >> 8);
                bluee += (byte)var1;
            }
            int nums = var.Count;
            redd /= nums;
            bluee /= nums;
            greenn /= nums;
            key1 = redd;
            key2 = greenn;
            value = (key1 << 16) + (key2 << 8) + bluee;
            return value; //return new color
        }
        public static Dictionary<int, int> resultImageDictionary = new Dictionary<int, int>();
        public static void ImageDictionary()
        {
            HashSet<int> var1;
            foreach (var var in clusters)
            {
                var1 = var;
                int x = 0, y = 0;
                foreach (var var2 in var)
                {
                    //calc avg color
                    if (x == 0)
                    {
                        y = Averadge_Cluster(var1);
                        x = 1;
                    }

                    resultImageDictionary.Add(var2, y);
                }

            }
        }
        public static void Quantization()
        {
            int keyy = 0, key1 = 0, key2 = 0, redd, greenn, bluee;
            var img = MainForm.ImageMatrix;
            //ii>>height,i>>width
            for (int ii = 0; ii < ImageOperations.GetHeight(img); ii++)
            {
                for (int i = 0; i < ImageOperations.GetWidth(img); i++)
                {
                    //git pixels of each color
                    redd = img[ii, i].red;
                    greenn = img[ii, i].green;
                    bluee = img[ii, i].blue;
                    key1 = redd;
                    key2 = greenn;
                    //add each color to other
                    keyy = (shaft_lift(key1, 16)) + (shaft_lift(key2, 8)) + bluee;
                    //get color in dictionary
                    var val1 = resultImageDictionary[keyy];
                    var val2 = resultImageDictionary[keyy];
                    //replace each color with new color
                    img[ii, i].green = (byte)(shaft_right(val2, 8));
                    img[ii, i].red = (byte)(shaft_right(val1, 16));
                    img[ii, i].blue = (byte)(resultImageDictionary[keyy]);
                }
            }
        }
    }
}
