using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Priority_Queue
{
    public class Vertix
    {
        //weight of vertix
        public float Weight;
        //ind of vertix added in queue
        public int ind_added;
        //vertix
        public float vertix;
        //parent
        public float Parent;
        public Vertix(float v, float p, float w)
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
            //while not reach to parent
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
            if (HighWeight(v, pV) == false)
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
            //pit it in right place
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
                if (HighWeight(pV, v) == true)
                    Max_Heap(v);
            }
            else
                Min_Heap(v);
        }

        public Virtex Dequeue()
        {
            Virtex v = Vertices[1];
            //Swap the vertix with the last vertix
            Virtex LVert = Vertices[numVertices];
            Swap(v, LVert);
            //decrease num of vert and init null
            Vertices[numVertices--] = null;

            //put lvert in right place
            VertixUpdated(LVert);
            return v;
        }
    }
}