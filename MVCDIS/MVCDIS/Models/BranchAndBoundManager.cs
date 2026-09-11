
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{
    public static class Properties
    {
        public static int Ver = 5;
    }
    public class BranchAndBoundManager
    {
        public static double ZERO = 1.0e-15;
        public static double INFINITY = 1.0e+30;
        public static int MaxVer = Properties.Ver;
        public static double Eps = 1.0e-10;
        public static int N = Properties.Ver;
        public int[] ShortPath = new int[Properties.Ver + 2];
        public double[,] Matr = new double[Properties.Ver + 1, Properties.Ver + 1];
        //Вычисляет длину пути из N+1 вершин или возвращает
        //0, если пути не существует (некоторые рёбра отсутствуют)




        /*Возвращает True, если вершина Ver является начальной
       вершиной какого-либо ребра Bound^.Ribs и False в
       противном случае*/
        public bool BegVerInRibs(int Ver, Pbound Bound)
        {
            for (int i = 1; i <= Bound.RibCol; i++)
            {
                if (Bound.Ribs[i, 1] == Ver)
                {
                    return true;

                }
            }
            return false;
        }

        /*Возвращает True, если вершина Ver является конечной
         вершиной какого-либо ребра Bound^.Ribs и False в
         противном случае*/

        public bool EndVerInRibs(int Ver, Pbound Bound)
        {
            for (int i = 1; i <= Bound.RibCol; i++)
            {
                if (Bound.Ribs[i, 2] == Ver)
                {
                    return true;

                }
            }
            return false;
        }
        /*Осуществляет приведение матрицы Bound^.M размером NxN
Увеличивает Bound^.Fi на сумму констант приведения */
        public void ReductMatr(Pbound Boundr, bool copy,int n, ref Pbound newBound)
        {
            N = n;
            Pbound Bound = null;
            Bound =new Pbound();
            Bound.M = new double[N + 1, N + 1];
            Bound.Ribs = new int[N + 1, 3];
            if (copy)
            {
                if (Boundr.Ribs==null)
                {
                       Bound.Ribs = null;
                }
                for (int i = 1; i <= N; i++)
                {
                    for (int j = 1; j <= N; j++)
                    {

                        Bound.M[i, j] = Boundr.M[i, j];


                    }
                    if (Boundr.Ribs != null)
                    {
                        for (int j = 1; j < 3; j++)
                        {

                            Bound.Ribs[i , j] = Boundr.Ribs[i , j];


                        }
                    }

                }
                Bound.RibCol = Boundr.RibCol;
                Bound.Fi = Boundr.Fi;
             
            }
            else Bound = Boundr;
            double Min = new double();
            for (int i = 1; i <= N; i++)
            {
                if (!BegVerInRibs(i, Bound))
                {
                    Min = 2 * INFINITY;
                    for (int j = 1; j <= N; j++)
                    {
                        if ((EndVerInRibs(j, Bound) == false) && (Bound.M[i, j] < Min) && (i != j))
                        {
                            Min = Bound.M[i, j];
                        }
                    }
                    Bound.Fi += Min;
                    for (int j = 1; j <= N; j++)
                    {
                        if (!EndVerInRibs(j, Bound))
                        {
                            Bound.M[i, j] -= Min;
                        }
                    }

                }
            }
            for (int j = 1; j <= N; j++)
            {
                if (!EndVerInRibs(j, Bound))
                {
                    Min = 2 * INFINITY;
                    for (int i = 1; i <= N; i++)
                    {
                        if ((BegVerInRibs(i, Bound) == false) && (Bound.M[i, j] < Min) && (i != j))
                        {
                            Min = Bound.M[i, j];
                        }
                    }
                    Bound.Fi += Min;
                    for (int i = 1; i <= N; i++)
                    {
                        if (!BegVerInRibs(i, Bound))
                        {
                            Bound.M[i, j] -= Min;
                        }
                    }
                }
            }

            newBound.M = new double[N + 1, N + 1];
            newBound.Ribs = new int[N + 1, 3];
             if (Bound.Ribs==null)
                {
                        newBound.Ribs = null;
                }
                for (int i = 1; i <= N; i++)
                {
                    for (int j = 1; j <= N; j++)
                    {

                         newBound.M[i, j] = Bound.M[i, j];


                    }
                    if (Bound.Ribs != null)
                    {
                        for (int j = 1; j < 3; j++)
                        {

                            newBound.Ribs[i , j] = Bound.Ribs[i , j];


                        }
                    }

                }
                 newBound.RibCol = Bound.RibCol;
                 newBound.Fi = Bound.Fi;

             


        }
        /*Находит "самый тяжелый ноль" матрицы Bound^.M размером NxN и возвращает
строку Row и столбец Col в котором этот ноль был найден*/
        public void FindHeavyZero(Pbound Bound, ref int Row, ref int Col)
        {


            double MaxW = new double();
            Row = 0;
            Col = 0;
            MaxW = -1.0;
            Pbound Tmp = new Pbound();
            Tmp.M = new double[N + 1, N + 1];
            Tmp.Ribs = new int[N + 1, 3];
           
            for (int i = 1; i <= N; i++)
            {
            
               
                if (!BegVerInRibs(i, Bound))
                {
                    for (int j = 1; j <= N; j++)
                    {
                        if (Bound.M[i, j] < ZERO)
                        {


                            if (Bound.Ribs == null)
                            {
                                Tmp.Ribs = null;
                            }
                            for (int k = 1; k <= N; k++)
                            {
                                for (int m = 1; m <= N; m++)
                                {

                                    Tmp.M[k,m] = Bound.M[k, m];


                                }
                                if (Bound.Ribs != null)
                                {
                                    for (int m = 1; m < 3; m++)
                                    {

                                        Tmp.Ribs[k, m] = Bound.Ribs[k, m];


                                    }
                                }

                            }
                            Tmp.RibCol = Bound.RibCol;
                            Tmp.M[i, j] = 2 * INFINITY;
                            Tmp.Fi = 0.0;
                            ReductMatr(Tmp, true,N, ref Tmp);
                            if (Tmp.Fi > MaxW)
                            {
                                Row = i;
                                Col = j;
                                MaxW = Tmp.Fi;
                            }
                        }
                    }
                }
            }
        }

        /*Проверяет, образует ли ребро (V1,V2) замкнутый контур с ребрами из
 Bound^.Ribs*/
        public bool IsCycle(Pbound Bound, int V1, int V2)
        {
            int V = 0;
            int CycLen = 0;
            V = V2;
            CycLen = 1;

            while (CycLen < Bound.RibCol + 1)
            {
                for (int i = 1; i <= Bound.RibCol; i++)
                {
                    if (Bound.Ribs[i, 1] == V)
                    {
                        V = Bound.Ribs[i, 2];
                        CycLen += 1;
                        if (V == V1)
                        {
                            return true;
                        }
                        else goto loop;
                    }

                }
                break;
            loop: ;
            }
            return false;
        }
        /*Разбивает границу Bound на левую и правую часть (Left и Right).
  - в левой части остаются все циклы, в которые входит ребро,
    соответствующее клетке с наиболее "тяжелым нулем" (список отобранных
    ребер пополняется данным ребром).
  - в правой части остаются все циклы в которые не входит ребро, отобранное
    для левой части
 Затем матрицы приводятся*/
        public void NewLevel(Pbound Bound, ref Pbound Left, ref Pbound Right)
        {

            int Row = 0;
            int Col = 0;
            Pbound hb = new Pbound();

            hb = Bound;
            FindHeavyZero(hb, ref Row, ref Col);
            Left = new Pbound();


            Left.Ribs = null;
            Left.M = new double[N + 1, N + 1];
            Left.Ribs = new int[N + 1, 3];

            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {

                    Left.M[i, j] = Bound.M[i, j];


                }
                if (Bound.Ribs != null)
                {
                    for (int j = 1; j < 3; j++)
                    {

                        Left.Ribs[i, j] = Bound.Ribs[i, j];


                    }
                }

            }
            Left.RibCol = Bound.RibCol;
            Left.Fi = Bound.Fi;
            Left.RibCol += 1;
            Left.Ribs[Left.RibCol, 1] = Row;
            Left.Ribs[Left.RibCol, 2] = Col;

            if (Left.RibCol < N - 1)
                for (int i = 1; i <= N; i++)
                    if (!BegVerInRibs(i, Left))
                        for (int j = 1; j <= N; j++)
                            if (!EndVerInRibs(j, Left))
                                if (Left.M[i, j] < INFINITY)
                                    if (IsCycle(Left, i, j))
                                        Left.M[i, j] = 2 * INFINITY;

            ReductMatr(Left, true,N,ref Left);
            Right = new Pbound();

            
         


            Right.M = new double[N + 1, N + 1];
            Right.Ribs = new int[N + 1, 3];

            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {

                    Right.M[i, j] = Bound.M[i, j];


                }
                if (Bound.Ribs != null)
                {
                    for (int j = 1; j < 3; j++)
                    {

                        Right.Ribs[i, j] = Bound.Ribs[i, j];


                    }
                }

            }
            Right.RibCol = Bound.RibCol;
            Right.Fi = Bound.Fi;
            Right.M[Row, Col] = 2 * INFINITY;
            ReductMatr(Right, true,N,ref Right);
        }
        /*Превращение в рекорд границы Bound с матрицей NxN и одним невычеркнутым
 ребром добавлением этого невычеркнутого ребра в список ребер Ribs*/
        public void BuildRecord(Pbound Bound)
        {

            for (int i = 1; i <= N; i++)
            {
                if (!BegVerInRibs(i, Bound))
                {
                    for (int j = 1; j <= N; j++)
                    {
                        if (!EndVerInRibs(j, Bound))
                        {
                            Bound.RibCol += 1;
                            Bound.Ribs[Bound.RibCol, 1] = i;
                            Bound.Ribs[Bound.RibCol, 2] = j;
                            Bound.Fi += Bound.M[i, j];
                            return;
                        }
                    }
                }
            }
        }
        /*По лучшему рекорду Bound строит последовательный путь обхода Path, начиная
 с вершины BegVer. С помощью исходной весовой матрицы Matr размером NxN,
 подсчитывается длина пути. Если длина пути >= бесконечности, возвращается
 False - пути нет, иначе возвращается True*/
        public bool BuildPath(Pbound Bound, double[,] Matr, int BegVer, ref int[] Path)
        {
            double PathLen = new double();
            PathLen = 0.0;
            Path[1] = BegVer;
            for (int i = 2; i <= N; i++)

                for (int j = 1; j <= Bound.RibCol; j++)

                    if (Bound.Ribs[j, 1] == Path[i - 1])
                    {
                        Path[i] = Bound.Ribs[j, 2];
                        PathLen += Matr[Path[i - 1], Path[i]];
                        break;
                    }
            Path[Bound.RibCol + 1] = BegVer;
            PathLen += Matr[Path[Bound.RibCol], Path[Bound.RibCol + 1]];

            Bound = null;
            return PathLen < INFINITY;
        }

        /*Решает задачу "о коммивояжёре" методом "ветвей и
 границ".
   Matr - матрица весов в орграфе на N вершинах (0-нет ребра).
          Исходная вершина имеет номер Ver
   Ans  - оптимальный путь в графе
 Функция возвращает True, если задача решена и False,
 если в графе вообще нет гамильтоновых циклов */
        public double PathLength(int[] Path, double[,] Matr)
        {


            double Res = new double();
            Res = 0;
            for (int i = 1; i <= N; i++)
            {
                if (Matr[Path[i], Path[i + 1]] <= Eps)
                {
                    return 0.0;
                }
                else
                {
                    Res += Matr[Path[i], Path[i + 1]];
                }
            }
            return Res;
        }
        /*решает задачу "о коммивояжёре" методом "полного
перебора" возможных путей.
  Matr - матрица весов в орграфе на N вершинах.
         Исходная вершина имеет номер Ver
  Ans  - оптимальный путь в графе
Функция возвращает True, если задача решена и False,
если в графе вообще нет гамильтоновых циклов }*/
        public bool Exhaustive(double[,] Matr, int Ver, ref int[] Ans)
        {
            double Res = new double();
            double Tmp = new double();
            int[] OldPath = new int[MaxVer + 2];
            int[] CurPath = new int[MaxVer + 2];
            int[] z = new int[MaxVer + 2];
            int[] p = new int[MaxVer + 2];
            int[] d = new int[MaxVer + 2];
            int k = 0, pm = 0, dm = 0, zpm = 0, m = 0, w = 0;
            Res = 1.0e301;
            for (int i = 1; i <= N; i++)
            {
                z[i] = i;
                p[i] = i;
                d[i] = -1;
            }
            d[1] = 0;
            m = N + 1;
            z[0] = m;
            z[N + 1] = m;
            while (m != 1)
            {
                CurPath[1] = Ver;
                k = 1;
                for (int j = 1; j <= N; j++)
                {
                    if (z[j] != Ver)
                    {
                        k++;
                        CurPath[k] = z[j];
                    }
                }
                CurPath[N + 1] = Ver;
                Tmp = PathLength(CurPath, Matr);
                if (Tmp > Eps)
                {
                    if (Tmp < Res)
                    {
                        Res = Tmp;
                        OldPath = CurPath;
                    }
                }
                m = N;
                while (z[p[m] + d[m]] > m)
                {
                    d[m] = -d[m];
                    m -= 1;
                }
                pm = p[m];
                dm = pm + d[m];
                w = z[pm];
                z[pm] = z[dm];
                z[dm] = w;
                zpm = z[pm];
                w = p[zpm];
                p[zpm] = pm;
                p[m] = w;

            }
            if (Res < 1.0e301)
            {
                Ans = OldPath;
                return true;
            }
            else
            {
                Ans = null;
                return false;
            }
        }
    }
}