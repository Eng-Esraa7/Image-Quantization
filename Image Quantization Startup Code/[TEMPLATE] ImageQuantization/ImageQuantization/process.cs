using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageQuantization
{
    struct Edge
    {
        public int first_vertix;
        public int second_vertix;
        public float weight;
    }
    class process
    {
        public static List<int> DistinctColor(RGBPixel[,] ImageMatrix)
        {
            //get width and height of image
            int w = ImageOperations.GetWidth(ImageMatrix);
            int h = ImageOperations.GetHeight(ImageMatrix);
            //calc RGB
            int red, green, blue;
            //List of Distinct Color
            List<int> dist = new List<int>();

            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    red = ImageMatrix[row, col].red;
                    green = ImageMatrix[row, col].green;
                    blue = ImageMatrix[row, col].blue;
                    int colorPixel = (blue << 16) + (green << 8) + red;
                    //if not found found before add to list
                    if (!dist.Contains(colorPixel))
                        dist.Add(colorPixel);
                }
            }
            return dist.ToList();
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
        public static List<Edge> MST = new List<Edge>();
        public static float Generate_MST(List<int> DistinctColor)
        {
            //priority queue
            Priority_Queue<Vertix> priorityQueue = new Priority_Queue<Vertix>(DistinctColor.Count);

            //set each vertix with parent
            Vertix[] queue = new Vertix[DistinctColor.Count];

            //fisrt vertix = first distinct color && parent = null(-1) && weight=0
            queue[0] = new Vertix(DistinctColor[0], -1,0);

            //put first virtix to queue
            priorityQueue.Enqueue(queue[0]);

            //loop on distinct color and put it in queue
            for (int i = 1; i < DistinctColor.Count; i++)
            {
                float OO = float.MaxValue;
                //set weight oo and parent oo
                priorityQueue.Enqueue(new Vertix(DistinctColor[i], OO,OO));
            }

            while (true)
            {
                //get min weight
                Vertix MinVert = priorityQueue.Dequeue();
                //save edge and add to Mst List
                Edge edge;
                edge.first_vertix = Convert.ToInt32(MinVert.vertix);
                edge.second_vertix = Convert.ToInt32(MinVert.Parent);
                edge.weight = (MinVert.Weight);
                MST.Add(edge);

                //loop on queue and set piriority again
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
                        priorityQueue.VertixUpdated(v);
                    }
                }

                if (priorityQueue.Count() == 0)
                    break;
            }
            float weight = 0;
            foreach (var v in MST)
            {
                weight = weight + v.weight;
            }
            return weight;
        }
    }
}
