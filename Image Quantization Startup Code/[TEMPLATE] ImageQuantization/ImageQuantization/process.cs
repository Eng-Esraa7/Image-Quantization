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
        public static List<int> DistinctColorList= new List<int>();
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
                    int colorPixel = (blue << 16) + (green << 8) + red;
                    //if not found found before add to list
                    if (!dist.Contains(colorPixel))
                        dist.Add(colorPixel);
                }
            }
            DistinctColorList = dist.ToList();

        }

        private static float Weight_EuclideanDistance_2virtices(Vertix V1, Vertix V2)
        {
            //calc Weight with EuclideanDistance between 2virtices
            int red_V1, red_V2, green_V1, green_V2, blue_V1, blue_V2;
            int color1 =Convert.ToInt32(V1.vertix);
            int color2 = Convert.ToInt32(V2.vertix);
            red_V1 = (byte)(color1);
            red_V2 = (byte)(color2);
            green_V1 = (byte)(color1 >> 8);
            green_V2 = (byte)(color2 >> 8);
            blue_V1 = (byte)(color1 >> 16);
            blue_V2 = (byte)(color2 >> 16);
            float  d1 = (red_V2 - red_V1) * (red_V2 - red_V1),
                    d2 = (green_V2 - green_V1) * (green_V2 - green_V1),
                    d3 = (blue_V2 - blue_V1) * (blue_V2 - blue_V1),
                    sum = (float)Math.Sqrt(Math.Abs(d1) + Math.Abs(d2) + Math.Abs(d3));
            return (sum);
        }

        //list of mst
        public static List<Vertix> MST = new List<Vertix>();
        public static float Generate_MST()
        {
            //priority queue
            Priority_Queue<Vertix> priorityQueue = new Priority_Queue<Vertix>(DistinctColorList.Count);

            //set each vertix with parent
            Vertix[] queue = new Vertix[DistinctColorList.Count];

            //loop on distinct color and put it in queue
            for (int i = 0; i < DistinctColorList.Count; i++)
            {
                float OO = float.MaxValue;
                //fisrt vertix = first distinct color && parent = null(-1) && weight=0
                if (i==0)
                    priorityQueue.Enqueue(new Vertix(DistinctColorList[i], -1, 0));
                else
                    //set weight oo and parent oo
                    priorityQueue.Enqueue(new Vertix(DistinctColorList[i], OO,OO));
            }

            while (true)
            {
                //get min weight
                Vertix MinVert = priorityQueue.Dequeue();
                //save edge and add to Mst List
                MST.Add(MinVert);

                //loop on queue and set weight again
                float calcWeight = -1;
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
            float weight = 0;
            foreach (var v in MST)
            {
                weight += v.Weight;
            }
            return weight;
        }
    }
}
