using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
namespace _3_Course_
{
    public partial class Form1 : Form
    {
        int numOfHeads = 0;//количество вершин в половине двудольного графа
        public Form1()
        {
            InitializeComponent();
        }
        //класс реализующий сеть
        public class Net
        {
            int[,] Matrix;//сеть
            //принимает количество вершин в половине двудольного графа и сам граф
            //возвращает сеть
            public Net(int numOfVertexes, int[,] graph)
            {
                Matrix = new int[numOfVertexes + 1, numOfVertexes + 1];
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
        //класс реализующий ребро графа или сети
        //нужен только для удобства вывода паросочетания 
        //хранит номера вершин которые соединяет
        public class Edge
        {
            public int In;
            public int Out;
            public Edge(int i, int o)
            {
                In = i; Out = o;
            }
        }
        //класс реализующий форд-фалкерсона
        public class FordFalkerson
        {
            int[,] rGraph; //граф для учета инвертированных ребер(иными словами хранит ребра их стока в исток)
            List<Edge> maxCouples = new List<Edge>();//максимальное паросочетание
                                                     //получает сеть и количестов вершин половины двудольного графа
                                                     //возвращает список ребер максимального паросочетания
            public List<Edge> fordFalkerson(int[,] graph, int numofVertexes)
            {
                //изначально все ребра сети напраалены из истока в сто поэтому все 0
                rGraph = graph;
                int[,] lGraph = new int[numofVertexes + 1, numofVertexes + 1];
                for (int i = 0; i < numofVertexes; i++)
                    for (int j = 0; j < numofVertexes; j++)
                        lGraph[i, j] = 0;
                for (int i = 0; i < numofVertexes; i++)
                {
                    lGraph[numofVertexes, i] = 1;
                    lGraph[i, numofVertexes] = 1;
                }
                lGraph[numofVertexes, numofVertexes] = 0;
                while (true)
                {
                    List<int> path = new List<int>();//путь на цикле
                    List<Edge> currentCouple = new List<Edge>();//паросочетание итерации цикла
                    List<int> queue = new List<int>();//очередь вершин(нужно для реализации фф)
                    List<Edge> nodes = new List<Edge>();//список ребер которые надо инвертировать
                    //массивы вершин которые были просмотрены
                    //их два тк работаем с половинами графа
                    bool[] visitedL = new bool[numofVertexes + 1];
                    bool[] visitedR = new bool[numofVertexes + 1];
                    for (int i = 0; i < visitedL.Length; i++)
                    {
                        visitedL[i] = false;
                        visitedR[i] = false;
                    }
                    //добавляем исток в массив просмотренных и очередь вершин
                    visitedL[numofVertexes] = true;
                    queue.Add(numofVertexes * 2);
                    int l = numofVertexes;
                    int k = 0;
                    while (queue.Count() != 0)
                    {
                        //удаляем первый элемент очереди
                        queue.RemoveAt(0);
                        //ищем следующее ребро из вершины l в котором мы еще не были
                        for (int i = 0; i < numofVertexes + 1; i++)
                        {
                            if (visitedR[i] == false && lGraph[l, i] == 1)
                            {
                                //добавляем вершину в очередь и запоминаем ее номер
                                queue.Add(i + numofVertexes);
                                k = i;
                                //добавляем ребро в список ребер для инвертирования 
                                nodes.Add(new Edge(l, i));
                                //если вершины не является стоком или истоком и мы пришли в него со сторны истока а не стока добавляем его в паросочетание
                                if (l != numofVertexes && k != numofVertexes && rGraph[l, i] == 1 && lGraph[l, i] == 0)
                                    currentCouple.Add(new Edge(l, k + numofVertexes));
                                //помечаем вершину как просмотренную 
                                visitedR[i] = true;
                                //добавляем вершину в путь
                                if (k != numofVertexes)
                                    path.Add(k);
                                break;
                            }
                        }
                        //если мы достигли истока выйти из цикла
                        if (visitedR[numofVertexes] == true)
                            break;
                        //ищем следующее ребро из вершины l в котором мы еще не были
                        for (int j = 0; j < numofVertexes + 1; j++)
                            if (visitedL[j] == false && rGraph[j, k] == 1)
                            {
                                //добавляем вершину в очередь и запоминаем ее номер
                                l = j;
                                //добавляем ребро в список ребер для инвертирования 
                                nodes.Add(new Edge(j, k));
                                ////если вершины не является стоком или истоком и мы пришли в него со сторны истока а не стока добавляем его в паросочетание                                if (l != numofVertexes && k != numofVertexes && rGraph[j,k] == 1&&lGraph[j,k]==0)
                                currentCouple.Add(new Edge(l, k + numofVertexes));
                                //помечаем вершину как просмотрен
                                visitedL[j] = true;
                                queue.Add(j);
                                //добавляем вершину в путь
                                if (l != numofVertexes)
                                    path.Add(l);
                                break;
                            }

                    }
                    //инвертирование ребер в соответствии со списком 
                    if (visitedR[numofVertexes] == true)
                    {
                        foreach (Edge edge in nodes)
                        {
                            if (rGraph[edge.In, edge.Out] == 1)
                                rGraph[edge.In, edge.Out] = 1;
                            else rGraph[edge.In, edge.Out] = 0;
                            if (lGraph[edge.In, edge.Out] == 0)
                                lGraph[edge.In, edge.Out] = 1;
                            else lGraph[edge.In, edge.Out] = 0;
                        }
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
                //ввод нового количества ребер
                int newNumber = int.Parse(this.textBox1.Text);
                if (newNumber < 1 || newNumber % 2 == 1)
                    throw new System.ArgumentOutOfRangeException("Invalid vertex number");
                newNumber /= 2;
                //если новое количество ребер больше то добавляем строки и столбцы иначеп удаляем 
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
            catch (System.ArgumentOutOfRangeException a)
            {
                MessageBox.Show(a.Message);
            }
            catch (System.FormatException a)
            {
                MessageBox.Show(a.Message);
            }
        }
        //класс реализующий перманент
        public class Permanent
        {
            public static int DetRec(int[,] matrix)
            {
                if (matrix.Length == 4)
                {
                    return matrix[0, 0] * matrix[1, 1] + matrix[0, 1] * matrix[1, 0];
                }
                int result = 0;
                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    int[,] minor = GetMinor(matrix, i);
                    result += matrix[0, i] * DetRec(minor);
                }
                return result;
            }
            private static int[,] GetMinor(int[,] matrix, int n)
            {
                int[,] result = new int[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
                for (int i = 1; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0, col = 0; j < matrix.GetLength(1); j++)
                    {
                        if (j == n)
                            continue;
                        result[i - 1, col] = matrix[i, j];
                        col++;
                    }
                }
                return result;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //считывание матрицы при этом пустота считается за 0 
                int[,] Matrix = new int[numOfHeads, numOfHeads];
                for (int i = 0; i < numOfHeads; i++)
                    for (int j = 0; j < numOfHeads; j++)
                    {
                        Matrix[i, j] = Convert.ToInt32(this.dataGridView1[j, i].Value);
                        if (Matrix[i, j] > 1 || Matrix[i, j] < 0)
                            throw new IndexOutOfRangeException("Ony 1 and 0 must be used");
                    }
                int permanent = Permanent.DetRec(Matrix);//вычисление перманента
                Net net = new Net(numOfHeads, Matrix);//создание сети
                FordFalkerson ff = new FordFalkerson();//применение фф
                List<Edge> couples = ff.fordFalkerson(net.getNet(), numOfHeads);//получение паросочетания
                label3.Text = "Перманент = " + permanent.ToString();//выводы
                label2.Text = "Максимальное паросочетание: ";
                foreach (Edge edge in couples)
                {
                    label2.Text += edge.In.ToString() + "-" + edge.Out.ToString() + ' ';
                }
            }
            catch (IndexOutOfRangeException a)
            {
                MessageBox.Show(a.Message);
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
        }
    }
}