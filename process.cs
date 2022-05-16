using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    class process
    {
        public static HashSet<HashSet<int>> clusters = new HashSet<HashSet<int>>();
        public static void Cluster(int k)
        {
            HashSet<int> Deleteed_set = new HashSet<int>();
            HashSet<int> Set = new HashSet<int>();

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
                        // swape to set max_weight at max variable
                        maxweight = MST[i].Weight;
                        //save index edge with max weight
                        index = i;
                    }
                }
                //set weight with abnormal number
                MST[index].Weight = 0;
            }


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
                else //==0 with max edge
                {
                    // add its as a set to cluster
                    Set.Add((int)MST[i].vertix);
                    Set.Add((int)MST[i].Parent);
                }
            }
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
        //List of Distinct Color
        public static List<int> DistinctColorList = new List<int>();
        public static void DistinctColor()
        {
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
            int red_V1, red_V2, green_V1, green_V2, blue_V1, blue_V2;
            int color1 = Convert.ToInt32(V1.vertix);
            int color2 = Convert.ToInt32(V2.vertix);
            red_V1 = (byte)(color1 >> 16);
            red_V2 = (byte)(color2 >> 16);
            green_V1 = (byte)(color1 >> 8);
            green_V2 = (byte)(color2 >> 8);
            blue_V1 = (byte)(color1);
            blue_V2 = (byte)(color2);
            double d1 = (red_V2 - red_V1) * (red_V2 - red_V1),
                    d2 = (green_V2 - green_V1) * (green_V2 - green_V1),
                    d3 = (blue_V2 - blue_V1) * (blue_V2 - blue_V1),
                    sum = (double)Math.Sqrt(Math.Abs(d1) + Math.Abs(d2) + Math.Abs(d3));
            return (sum);
        }

        //list of mst
        public static List<Vertix> MST = new List<Vertix>();
        public static double Generate_MST()
        {
            //priority queue
            Priority_Queue<Vertix> priorityQueue = new Priority_Queue<Vertix>(DistinctColorList.Count);

            //set each vertix with parent
            Vertix[] queue = new Vertix[DistinctColorList.Count];

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
                if (priorityQueue.Count() == 0)
                    break;
            }
            double weight = 0;
            foreach (var v in MST)
            {
                weight += v.Weight;
            }
            return weight;
        }
        public static int shaft_right(int var, int index)
        {
            for (int j = 0; j < index; j++)
            {
                var /= 2;
            }
            return var;
        }
        public static int shaft_lift(int var, int index)
        {
            for (int j = 0; j < index; j++)
            {
                var *= 2;
            }
            return var;
        }
        public static int Averadge_Cluster(HashSet<int> var)
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
            redd /= var.Count;
            bluee /= var.Count;
            greenn /= var.Count;
            key1 = redd;
            key2 = greenn;
            value = (key1 << 16) + (key2 << 8) + bluee;
            return value;
        }
        public static Dictionary<int, int> ImageDictionary(HashSet<HashSet<int>> Cluster)
        {
            Dictionary<int, int> resultImageDictionary = new Dictionary<int, int>();
            HashSet<int> var1;
            foreach (var var in Cluster)
            {
                var1 = var;
                foreach (var var2 in var)
                {
                    //if (!resultImageDictionary.ContainsKey(var2))
                    resultImageDictionary.Add(var2, Averadge_Cluster(var1));
                }

            }
            return resultImageDictionary;
        }
        public static void Quantize(Dictionary<int, int> ImageDictionary)
        {
            int keyy = 0, key1 = 0, key2 = 0, redd, greenn, bluee;
            var img = MainForm.ImageMatrix;
            //ii>>height,i>>width
            for (int ii = 0; ii < img.GetLength(0); ii++)
            {
                for (int i = 0; i < img.GetLength(1); i++)
                {
                    redd = img[ii, i].red;
                    greenn = img[ii, i].green;
                    bluee = img[ii, i].blue;
                    key1 = redd;
                    key2 = greenn;
                    keyy = (shaft_lift(key1, 16)) + (shaft_lift(key2, 8)) + bluee;
                    var val1 = ImageDictionary[keyy];
                    var val2 = ImageDictionary[keyy];
                    img[ii, i].green = (byte)(shaft_right(val2, 8));
                    img[ii, i].red = (byte)(shaft_right(val1, 16));
                    img[ii, i].blue = (byte)(ImageDictionary[keyy]);
                }
            }
        }
    }
}
