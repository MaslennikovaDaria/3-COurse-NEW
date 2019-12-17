using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _3_Course_
{
    
    public partial class Form1 : Form
    {
        int numOfHeads = 0;
        public Form1()
        {
            InitializeComponent();
        }

        public class Net
        {
            int[,] Matrix;
            public Net(int numOfVertexes, int[,] graph)
            {
                Matrix = new int[numOfVertexes +1, numOfVertexes +1];
                for (int i = 0; i < numOfVertexes; i++)
                    for (int j = 0; j < numOfVertexes; j++)
                        Matrix[i, j] = graph[i, j];
           
                for (int i = 0; i < numOfVertexes; i++)
                {
                    Matrix[numOfVertexes, i] = 1;
                    Matrix[i, numOfVertexes] = 1;
                }
                Matrix[numOfVertexes, numOfVertexes] = 0;
            }
            public int[,] getNet() { return Matrix; }
        }
        public class Edge
        {
            public int In;
            public int Out;
            public Edge(int i,int o)
            {
                In = i;Out = o;
            }
        }
        public class FordFalkerson
        {
            int[,] rGraph; 
            List<Edge> maxCouples=new List<Edge>();
           public List<Edge> fordFalkerson(int[,] graph,int numofVertexes)
            {
                rGraph = graph;
                while (true)
                { 
                    List<int> path = new List<int>();
                    List<Edge> currentCouple = new List<Edge>();
                    List<int> queue = new List<int>();
                    bool[] visitedL = new bool[numofVertexes + 1];
                    bool[] visitedR = new bool[numofVertexes + 1];
                    for (int i = 0; i < visitedL.Length; i++)
                    {
                        visitedL[i] = false;
                        visitedR[i] = false;
                    }
                    visitedL[numofVertexes] = true;
                    queue.Add(numofVertexes * 2);
                    int l = numofVertexes;
                    int k = 0;
                    while (queue.Count != 0)
                    {
                        queue.RemoveAt(0);
                        for (int i = 0; i < numofVertexes + 1; i++)
                        {
                            if (visitedR[i] == false && rGraph[l, i] == 1)
                            {
                                queue.Add(i + numofVertexes);
                                k = i;
                                ///Adding edge
                                if(l!=numofVertexes&&k!=numofVertexes)
                                    currentCouple.Add(new Edge(l, k+numofVertexes));
                                visitedR[i] = true;
                                if(k!=numofVertexes)
                                path.Add(k);
                            }
                        }
                        for (int j = 0; j < numofVertexes + 1; j++)
                            if (visitedL[j] == false && rGraph[j, k] == 1)
                            {
                                l = j;
                                //Adding edge 
                                if(l!=numofVertexes&&k!=numofVertexes)
                                currentCouple.Add(new Edge( k + numofVertexes,l));
                                visitedL[j] = true;
                                queue.Add(j);
                                if (l!=numofVertexes)
                                path.Add(l);
                            }
                    }
                    if (visitedR[numofVertexes]==true)
                    {
                        maxCouples = currentCouple;
                        rGraph[numofVertexes, path.ElementAt(0)] = 0;
                        rGraph[path.ElementAt(path.Count - 1), numofVertexes] = 0;
                    }
                    else break; 
                }
                return maxCouples;
            }
       
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Введите количество вершин графа";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int newNumber = int.Parse(this.textBox1.Text);
                if (newNumber  <1)
                    throw new System.ArgumentOutOfRangeException("Invalid vertex number");
                if (numOfHeads <= newNumber)
                {
                    int difference = newNumber - numOfHeads;
                    for (int i = 0; i < difference; i++)
                        this.dataGridView1.Columns.Add("", "");
                    if (difference > 0)
                        this.dataGridView1.Rows.Add(difference);
                    numOfHeads = newNumber;
                }
                else
                {
                    int difference = numOfHeads - newNumber;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (numOfHeads > newNumber)
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                        numOfHeads--;
                    }
                    for (int i = 0; i < difference; i++)
                        dataGridView1.Columns.RemoveAt(i);
                    numOfHeads = newNumber;
                }
            }
            catch(System.ArgumentOutOfRangeException a)
            {
                MessageBox.Show(a.Message);
            }
            catch(System.FormatException a)
            {
                MessageBox.Show(a.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
                int[,] Matrix = new int[numOfHeads, numOfHeads];
                for (int i = 0; i < numOfHeads; i++)
                    for (int j = 0; j < numOfHeads; j++)
                        Matrix[i, j] = Convert.ToInt32(this.dataGridView1[j, i].Value);
                Net net = new Net(numOfHeads, Matrix);
                FordFalkerson ff = new FordFalkerson();
                List<Edge> couples = ff.fordFalkerson(net.getNet(), numOfHeads);

                label2.Text = "";
                foreach (Edge edge in couples)
                {
                    label2.Text += edge.In.ToString() + "-" + edge.Out.ToString() + ' ';
                }
            
            
        }

    }
}
