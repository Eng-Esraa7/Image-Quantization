using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Priority_Queue
{
    public class Vertix
    {
        //weight of vertix
        public double Weight;
        //ind of vertix added in queue
        public int ind_added;
        //vertix || first vertix
        public double vertix;
        //parent || second vertix
        public double Parent;
        //constractor to set vertix, parent and weight
        public Vertix(double v, double p, double w)
        {
            vertix = v;
            Parent = p;
            Weight = w;
        }
    }

    public sealed class Priority_Queue<Virtex> where Virtex : Vertix
    {
        //num of vertices
        private int numVertices;
        //array of vertix
        private Virtex[] Vertices;


        //constractor init num of vertices = 0
        public Priority_Queue(int Num_DistinctColor)
        {
            numVertices = 0;
            Vertices = new Virtex[++Num_DistinctColor];
        }
        //to enable foreach on list of vertices
        public IEnumerator<Virtex> GetEnumerator()
        {
            for (int i = 1; i <= numVertices; i++)
                yield return Vertices[i];
        }

        public int Count() //count num of vertices
        {
            return numVertices;
        }

        private bool HighWeight(Virtex v1, Virtex v2) //check if first vertix has high weight
        {
            return v1.Weight > v2.Weight ? true : false;
        }
        private void Swap(Virtex v1, Virtex v2) //swap two vertices
        {
            //Swap virtices
            Vertices[v1.ind_added] = v2;
            Vertices[v2.ind_added] = v1;

            //Swap index each vertix
            int tmp = v1.ind_added;
            v1.ind_added = v2.ind_added;
            v2.ind_added = tmp;
        }

        public void Max_Heap(Virtex v)
        {
            //get parent
            int p = v.ind_added / 2;
            Virtex pV = Vertices[p];
            //while reach to parent
            while (p > 0)
            {
                bool ret = heapify(v, pV, p);
                if (ret == true)
                    break;
                p = v.ind_added / 2;
            }
        }
        private bool heapify(Virtex v, Virtex pV, int p)
        {
            pV = Vertices[p];
            if (HighWeight(v, pV) == false) //if v has lower weight swap to move up
            {
                Swap(v, pV);
                return false;
            }
            return
                true;
        }

        public void Enqueue(Virtex v)
        {
            //add to queue vertices
            Vertices[++numVertices] = v;
            //save ind
            v.ind_added = numVertices;
            //put it in right place
            Max_Heap(v);
        }
        private void Min_Heap(Virtex v)
        {
            Virtex p;
            int ind_v = v.ind_added;
            while (true)
            {
                p = v;
                //left vertix
                int vL = 2 * ind_v;
                if (vL > numVertices)
                {
                    v.ind_added = ind_v;
                    Vertices[ind_v] = v;
                    break;
                }

                if (HighWeight(p, Vertices[vL])==true)
                    p = Vertices[vL];

                //right vertix
                int vR = vL + 1;
                if (vR <= numVertices)
                    if (HighWeight(p, Vertices[vR]))
                        p = Vertices[vR];

                //parent not high weight
                if (p != v)
                {
                    Vertices[ind_v] = p;
                    int tmp = p.ind_added;
                    p.ind_added = ind_v;
                    ind_v = tmp;
                }
                else
                {
                    v.ind_added = ind_v;
                    Vertices[ind_v] = v;
                    break;
                }
            }
        }

        public void VertixUpdated(Virtex v)
        {
            int p = v.ind_added / 2;
            Virtex pV = Vertices[p];

            if (p > 0)
            {
                if (HighWeight(pV, v) == true) //put vertix in right place
                    Max_Heap(v);
            }
            else
                Min_Heap(v);
        }

        public Virtex Dequeue()
        {
            //min weight
            Virtex v = Vertices[1];
            //Swap the vertix with the last vertix
            Virtex LVert = Vertices[numVertices];
            Swap(v, LVert);
            //decrease num of vert and init null
            Vertices[numVertices--] = null;

            //put last vert in right place
            VertixUpdated(LVert);
            return v;
        }
    }
}