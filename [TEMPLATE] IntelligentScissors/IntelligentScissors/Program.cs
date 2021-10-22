using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
namespace IntelligentScissors
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static List<List<Tuple<int, double>>> adjlist;
       
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

        }
        private static int generate_id(int i,int j,int w)
        {
            return i * w + j;
        }
        public static void Build_Graph()
        {
            RGBPixel[,] Matrix = MainForm.ImageMatrix;
            int height = ImageOperations.GetHeight(Matrix);
            int width = ImageOperations.GetWidth(Matrix);
            Console.WriteLine(height + " " + width);
            //List<List<Tuple<int, double>>> 
                adjlist = new List<List<Tuple<int, double>>>( new List<Tuple<int,double>>[width*height+10] );
            for(int i=0;i<adjlist.Count;++i)
            {
                adjlist[i] = new List<Tuple<int, double>>(5);
            }
            //Building Graph
            for (int i=0;i<height;++i)
            {

                for (int j = 0; j < width; ++j)
                {
                    Vector2D energies = ImageOperations.CalculatePixelEnergies(j, i, Matrix);
                    double Gx = 1 / energies.X, Gy = 1 / energies.Y;
                    // creating node for every pixel 
                    int cur_node = generate_id(i, j,width);
                    int X_next = generate_id(i, j+1 ,width);
                    int Y_next = generate_id(i + 1, j,width);
                    //not border pixels
                    if (i<height-1&&j<width-1)
                    {
                        adjlist.ElementAt(cur_node).Add(new Tuple<int, double>(X_next, Gx));
                        adjlist.ElementAt(cur_node).Add(new Tuple<int, double>(Y_next, Gy));
                        adjlist.ElementAt(X_next).Add(new Tuple<int, double>(cur_node, Gx));
                        adjlist.ElementAt(Y_next).Add(new Tuple<int, double>(cur_node, Gy));
                    }
                    else
                    {
                        //border pixels
                        if (i == height - 1 && j == width - 1)
                        {
                            //do nothing last pixel in photo
                        }
                        else if (i == height - 1)
                        {
                            adjlist[cur_node].Add(new Tuple<int, double>(X_next, Gx));
                            adjlist[X_next].Add(new Tuple<int, double>(cur_node, Gx));
                        }
                        else
                        {
                            adjlist[cur_node].Add(new Tuple<int, double>(Y_next, Gy));
                            adjlist[Y_next].Add(new Tuple<int, double>(cur_node, Gy));
                        }
                    }
                }
            }
           
            for(int i=0;i<adjlist.Count;++i)
            {
                Console.Write(i + " ");
                for (int j = 0; j < adjlist[i].Count;++j)
                {
                    Console.Write(adjlist[i][j].Item1 + " "+  adjlist[i][j].Item2 + " ");
                }
                Console.WriteLine();
            }
            
        }
        public static void WriteFileOutput(List<List<Tuple<int, double>>> L, int size)
        {
            FileStream FS = new FileStream("Output.txt", FileMode.Create, FileAccess.Write);
            StreamWriter SW = new StreamWriter(FS);
            SW.Write("The constructed graph");
            SW.WriteLine();
            for (int i = 0; i < size; i++)
            {
                SW.WriteLine("The index node" + i);
                SW.WriteLine("Edges");
                for (int j = 0; j < L[i].Count; j++)
                {
                    SW.WriteLine("edge from " + i + " to " + L[i][j].Item1 + "with weights" + L[i][j].Item2);
                }
                SW.WriteLine();
            }
            SW.Close();
            FS.Close();
        }
        public static void WriteFilePath(List<int> L, int size,int width, int src,int dest)
        {
            FileStream FS = new FileStream("Path.txt", FileMode.Create, FileAccess.Write);
            StreamWriter SW = new StreamWriter(FS);
            int x = src / width;
            int y = src - (x * width);
            int x1 = dest/ width;
            int y1 = dest- (x * width);
            SW.WriteLine("The Shortest path from Node" + src+ " at position  " + x + " " + y +
                " The Shortest path to Node  "+dest + " at position "+x1+" "+y1);
            for (int i=L.Count-1;i>=0;i--)
            {
                 x = L[i] / width;
                 y = L[i] - (x * width);
                SW.WriteLine("Node" + "{X=" + x+"," + " Y=" + y+"}"+"at position "+" x"+"at position"+"y"+y);
            }
            SW.Close();
            FS.Close();
        }
        public  static void shortest_path(int x,int y)
        {

            int width = ImageOperations.GetWidth(MainForm.ImageMatrix);
            int height = ImageOperations.GetHeight(MainForm.ImageMatrix);
            Console.WriteLine(x + " " + y);
            int starting_node = generate_id(x, y, ImageOperations.GetWidth(MainForm.ImageMatrix));
            int[] parent1 = new int[width*height+5];
            bool[] vis = new bool[width*height +5];
            double[] values = new double[width*height+5];
            for(int i=0;i<width*height+2;++i)
            {
                values[i] = double.MaxValue;
            }
            PriorityQueue q = new PriorityQueue();
            Node_info n = new Node_info(1258, -1, 0);
            q.Insert(n);
            while (!q.is_empty())
            {
                n = q.poll();
                double val = Convert.ToDouble(n.get_item3());
                int cur = Convert.ToInt32(n.get_item1());
                int par = Convert.ToInt32(n.get_item2());
                    if (vis[cur]) { continue; }
                vis[cur] = true;
                values[cur] = val;
                parent1[cur] = par;
                for (int i = 0; i < adjlist[cur].Count; i++)
                {
                    double weight = adjlist[cur][i].Item2;
                    weight += val;
                    Node_info n1 = new Node_info(adjlist[cur][i].Item1, cur, weight);
                    q.Insert(n1);
                }
            }
            List<int> Par=new List<int>();
            int nn = 4334;
            while (parent1[nn] != -1)
            {
                Console.WriteLine(parent1[nn]);
                Par.Add(parent1[nn]);
                nn = parent1[nn];

            }
            WriteFilePath(Par, Par.Count, width, 1258,4334);
            WriteFileOutput(adjlist,height);
        }
     
    }
    
    // pair class to create an object you must specify 2 values the first is double the second is integer
    public class Node_info 
    {
        //node 
        private int item1;
        //parent
        private int item2;
        //cost
        private double item3;
        public Node_info(int item1,int item2,double item3)
        {
            this.item1 = item1;
            this.item2 = item2;
            this.item3 = item3;
        }
        // compares 2 pairs return 0 if both are equal 1 if PHANTOM object is greater -1 if parameter object is greater
        public int CompareTo(Node_info P)
        {
            if (Math.Abs(item3 - P.item3) < double.Epsilon) return 0;
            else if (item3 - P.item3 < 0) return -1;
            return 1;
        }
        public  double get_item1()
        {
            return item1;
        }
        public int get_item2()
        {
            return item2;
        }
        public double get_item3()
        {
            return item3;
        }
        public void set_item1(int val)
        {
            this.item1 = val;
        }
        public void set_item2(int val)
        {
            this.item2 = val;
        }
        public void set_item3(double val)
        {
            this.item3 = val;
        }
    }
    public class PriorityQueue
    {
        private List<Node_info> heap;
        private int size;
        private int capacity;
        
        public PriorityQueue()
        {
            capacity = 20;
            heap = new List<Node_info>(capacity);
            size = 0;
        }
        private void CopyHeap(List<Node_info> Desination)
        {
            foreach(Node_info item in heap)
            {
                Desination.Add(item);
            }
        }
        private int get_parent_index(int idx)
        {
            if (idx % 2 == 0) return (idx / 2) - 1;
            return idx / 2;
        }
        private int get_left_child_index(int idx)
        {
            return idx * 2 + 1;
        }
        private int get_right_child_index(int idx)
        {
            return idx * 2 + 2;
        }
        private void EnsureHeapCapacity()
        {
            if (size == capacity)
            {
                capacity *= 10;
                List<Node_info> temp = new List<Node_info>(capacity);
                CopyHeap(temp);
                heap = temp;
            }
        }
        private void heapify_up(int idx)
        {
            int parent = get_parent_index(idx);
            while (idx > 0 && heap[idx].CompareTo(heap[parent]) == -1)
            {
                Node_info temp = heap[idx];
                heap[idx] = heap[parent];
                heap[parent] = temp;
                idx = parent;
                parent = get_parent_index(idx);
            }
        }
        private void heapify_down(int idx)
        {
            while(get_left_child_index(idx) < size)
            {
                int left_child_idx = get_left_child_index(idx);
                int min_idx = idx;
                if(heap[min_idx].CompareTo(heap[left_child_idx])== 1)
                {
                    min_idx = left_child_idx;
                }
                int right_child_idx = get_right_child_index(idx);
                if (right_child_idx < size && heap[min_idx].CompareTo(heap[right_child_idx])==1)
                {
                    min_idx = right_child_idx;
                }
                if(min_idx != idx)
                {
                    Node_info temp = heap[idx];
                    heap[idx] = heap[min_idx];
                    heap[min_idx] = temp;
                }
                else
                {
                    return;
                }
                idx = min_idx;
            }
        }
        // put an element of type pair in priority queue 
        public void Insert(Node_info value)
        {
            EnsureHeapCapacity();
            heap.Add(value);
            heapify_up(size);
            size++;
        }
        // return and remove the minimum pair in Priority queue
        public Node_info poll()
        {
            if (size == 0) throw new IndexOutOfRangeException();

            Node_info temp = heap[0];
            heap[0] = heap[size-1];
            heap[size - 1] = temp;
            heap.RemoveAt(size - 1);
            size--;
            heapify_down(0);            
            return temp;
        }
        // test function
        public void display()
        {
            for(int i=0;i<size;++i)
            {
                Console.Write(heap[i].get_item3() + " ");
            }
            Console.WriteLine();

        }
        public bool is_empty()
        {
            if (size == 0) return true;
            return false;
        }
        // get the minimum value (The most priority one) without removing it
        public Node_info peek()
        {
            if (size == 0) throw new AccessViolationException();
            return heap[0];
        }

    }
}
