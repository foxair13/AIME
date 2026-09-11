using MVCDIS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcContrib;
using System.Collections;
using System.Reflection;
using WebMatrix.WebData;

namespace MVCDIS.Controllers
{


    public class TempObject
    {
        public int NN { get; set; }
        public int CompetenceID { get; set; }
        public double Value { get; set; }
        public double ValueComp { get; set; }
    }

    public class groupident
    {
        public int GroupID { get; set; }
    }
    public class clusterspoint
    {
        public int startpoint { get; set; }
        public int endpoint { get; set; }
        public double distance { get; set; }
    }

    public class clusters
    {
        public int clusterid { get; set; }
        public double distance { get; set; }
        public List<additionalcluster> additional { get; set; }
        public List<clientclust> clients { get; set; }

    }

    public class additionalcluster
    {
        public int competenceid { get; set; }
        public String shifr { get; set; }
        public double top { get; set; }
        public double mean { get; set; }
        public double bottom { get; set; }
    }


    public class clientclust
    {
        public int clientid { get; set; }
        public int clientNN { get; set; }
        public String clientfio { get; set; }

        public double rate { get; set; }
        public List<competenceclust> competences { get; set; }
        public int clusterid { get; set; }
    }

    public class competenceclust
    {
        public int competenceid { get; set; }

        public String shifr { get; set; }
        public double value { get; set; }
        public string tittle { get; set; }

    }




    public class CalculateController : Controller
    {
        private CompetenceContext db = new CompetenceContext();
        //

        public static double ZERO = 1.0e-15;
        public static double INFINITY = 1.0e+30;
        public static int MaxVer = Properties.Ver;
        public static double Eps = 1.0e-10;
        public static int N = MaxVer;
        private double[,] Graph;
        public bool BranchAndBounds(double[,] Matr, int Ver, ref int[] Ans)
        {
            BranchAndBoundManager bab = new BranchAndBoundManager();
            double[,] WMatr = new double[N + 1, N + 1];
            Pbound CurBound = new Pbound();
            Pbound Left = new Pbound();
            Pbound Right = new Pbound();
            Pbound Rec = new Pbound();
            Pbound TmpBound = new Pbound();
            Pbound OldBound = new Pbound();
            for (int i = 1; i <= N; i++)
                for (int j = 1; j <= N; j++)
                {
                    if (Math.Abs(Matr[i, j]) < ZERO)
                    {
                        WMatr[i, j] = 2 * INFINITY;
                    }
                    else WMatr[i, j] = Matr[i, j];
                }
            CurBound.M = WMatr;
            CurBound.Fi = 0.0;
            CurBound.RibCol = 0;


            bab.ReductMatr(CurBound, false, N, ref OldBound);

            while (CurBound.RibCol < N - 1)
            {

                Left = null;
                Right = null;
                bab.NewLevel(CurBound, ref Left, ref Right);
                if (Left.Fi <= Right.Fi)
                {
                    CurBound = Left;
                }
                else
                {

                    CurBound = Right;
                }
            }
            bab.BuildRecord(CurBound);
            Rec = CurBound;

            return bab.BuildPath(Rec, WMatr, Ver, ref Ans);
        }
        [CustomAuthorize("Plancreate")]

        public PartialViewResult Clst(String levelid, String jidt, String rate, String ratemax, string values)
        {
            string[] selectedValues = values.Split(',');
            int jt = Convert.ToInt32(jidt);
            double rtmax = Convert.ToDouble(ratemax);
            int li = Convert.ToInt32(levelid);
            double rt = Convert.ToDouble(rate);
            List<ClientCalcValueEdit> LCVC = new List<ClientCalcValueEdit>();
            int gg = 0;
            foreach (CalcValueCompetence item in db._CalcValueCompetence.Where(x => x.LevelID == li && x.JobTitleID == jt).OrderByDescending(x => x.value).ToList<CalcValueCompetence>())
            {
                gg++;
                LCVC.Add(new ClientCalcValueEdit { ID = gg, ClientID = item.ClientID, ClientName = db._Clients.Where(x => x.ID == item.ClientID && x.Manager == false).Select(x => x.ClientsInfo.FIO).SingleOrDefault(), value = item.value });
            }


            List<ValueCompetence> VC = db._ValueCompetence.Where(x => x.JobTitleID == jt && x.LevelID == li).OrderBy(x => x.ClientID).OrderBy(x => x.CompetenceID).ToList<ValueCompetence>();
            VC = (from x in VC
                  from z in selectedValues
                  join t in LCVC on x.ClientID equals t.ClientID
                  where t.value >= rt && t.value <= rtmax && z.Contains(x.CompetenceID.ToString()) && t.value!=0
                  select x).ToList<ValueCompetence>();

            int nc = 0;
            int nk = 0;
            nc = VC.GroupBy(x => x.ClientID).Count();
            List<int> lc = db._Clients.Where(x => x.JobTitleID == jt && x.Manager == false).Select(x => x.ID).ToList<int>();
            lc = (from x in lc
                  join t in LCVC on x equals t.ClientID
                  where t.value >= rt && t.value < rtmax
                  select x).ToList<int>();
            nk = VC.GroupBy(x => x.CompetenceID).Count();
            alglib.clusterizerstate s;
            alglib.ahcreport rep;

            double[,] xy = new double[nc, nk];
            double[,] xye = new double[nc, nk];
            int i = 0;
            int j = 0;
            foreach (int itemC in lc)
            {

                List<ValueCompetence> Vclient = VC.Where(x => x.ClientID == itemC).OrderBy(x => x.CompetenceID).ToList<ValueCompetence>();
                foreach (ValueCompetence itemvc in Vclient)
                {
                    xy[i, j] = itemvc.value;
                    j++;
                }
                j = 0;
                i++;
            }
            for (int k = 0; k < xy.GetLength(1); k++)
            {
                double sumcomp = 0;
                double srcomp = 0;
                double srcomppow = 0;
                double sco = 0;
                for (int l = 0; l < xy.GetLength(0); l++)
                {
                    sumcomp += xy[l, k];
                }
                srcomp = sumcomp / (xy.GetLength(0));

                for (int l = 0; l < xy.GetLength(0); l++)
                {
                    xye[l, k] = Math.Pow(xy[l, k] - srcomp, 2);
                    srcomppow += xye[l, k];
                }
                sco = Math.Sqrt(srcomppow / (xy.GetLength(0) - 1));
                for (int l = 0; l < xy.GetLength(0); l++)
                {
                    xye[l, k] = (xy[l, k] - srcomp) / sco;
                }



            }
            alglib.clusterizercreate(out s);
            alglib.clusterizersetpoints(s, xye, 2);
            alglib.clusterizerrunahc(s, out rep);
            int[] pa = new int[rep.p.Length];
            for (int l = 0; l < rep.p.Length; l++)
            {
                int ind = rep.p[l];
                pa[ind] = l;
            }
            int[] Clt = new int[6];

            List<clusterspoint> tdp = new List<clusterspoint>();
            for (int k = 0; k < rep.z.GetLength(0); k++)
            {



                for (int l = 0; l < 6; l++)
                {
                    Clt[l] = rep.pm[k, l];
                }


                int n = Math.Abs(Clt[0] - Clt[1]);
                int v = Math.Abs(Clt[2] - Clt[3]);
                int[] a = new int[0];
                if (n > 1)
                {
                    a = new int[(n - 1) + 2];
                    int t = 0;
                    for (int l = Clt[0]; l <= Clt[1]; l++)
                    {
                        a[t] = l;
                        t++;
                    }
                }
                else if (n == 1)
                {
                    a = new int[2];
                    a[0] = Clt[0];
                    a[1] = Clt[1];
                }
                else if (n == 0)
                {
                    a = new int[1];
                    a[0] = Clt[0];
                }
                int[] b = new int[0];
                if (v > 1)
                {
                    b = new int[(v - 1) + 2];
                    int t = 0;
                    for (int l = Clt[2]; l <= Clt[3]; l++)
                    {
                        b[t] = l;
                        t++;
                    }
                }
                else if (v == 1)
                {
                    b = new int[2];
                    b[0] = Clt[2];
                    b[1] = Clt[3];
                }
                else if (v == 0)
                {
                    b = new int[1];
                    b[0] = Clt[2];
                }



                for (int l = 0; l < a.Length; l++)
                {
                    for (int m = 0; m < b.Length; m++)
                    {
                        if (pa[a[l]] < pa[b[m]])
                        {
                            tdp.Add(new clusterspoint
                            {
                                startpoint = pa[a[l]],
                                endpoint = pa[b[m]],
                                distance = rep.mergedist[k]
                            });
                        }
                        else if (pa[a[l]] > pa[b[m]])
                        {
                            tdp.Add(new clusterspoint
                            {
                                startpoint = pa[b[m]],
                                endpoint = pa[a[l]],
                                distance = rep.mergedist[k]
                            });
                        }


                    }
                }

            }
            ViewBag.levelid = levelid;
            ViewBag.jidt = jidt;
            ViewBag.SortMas = pa;
            String[] FiosStr = new String[rep.p.Length];
            for (int l = 0; l < lc.Count(); l++)
            {
                int id = lc[l];
                Clients cff = db._Clients.Where(x => x.JobTitleID == jt && x.ID == id && x.Manager == false).SingleOrDefault<Clients>();
                FiosStr[l] = cff.ClientsInfo.FIO;
            }

            ViewBag.FiosStr = FiosStr;
            return PartialView("ClusterDetail", tdp.ToList<clusterspoint>());
        }

        [CustomAuthorize("Plancreate")]
        [OutputCache(Duration = 0)]
        public PartialViewResult ClstKmeans(String levelid, String jidto, String kcount, String rate, String ratemax, string values)
        {
            ModelState.Clear();
            List<clusters> listclusters = new List<clusters>();
            string[] selectedValues = values.Split(',');
            int jt = Convert.ToInt32(jidto);
            int li = Convert.ToInt32(levelid);
            double rt = Convert.ToDouble(rate);
            double rtmax = Convert.ToDouble(ratemax);
            List<ValueCompetence> VC = db._ValueCompetence.Where(x => x.JobTitleID == jt && x.LevelID == li).OrderBy(x => x.ClientID).OrderBy(x => x.CompetenceID).ToList<ValueCompetence>();
            List<ClientCalcValueEdit> LCVC = new List<ClientCalcValueEdit>();
            int gg = 0;
            foreach (CalcValueCompetence item in db._CalcValueCompetence.Where(x => x.LevelID == li && x.JobTitleID == jt).OrderByDescending(x => x.value).ToList<CalcValueCompetence>())
            {
                gg++;
                LCVC.Add(new ClientCalcValueEdit { ID = gg, ClientID = item.ClientID, ClientName = db._Clients.Where(x => x.ID == item.ClientID && x.Manager == false).Select(x => x.ClientsInfo.FIO).SingleOrDefault(), value = item.value });
            }
            VC = (from x in VC
                  from z in selectedValues
                  join t in LCVC on x.ClientID equals t.ClientID
                  where t.value >= rt && t.value <= rtmax && z.Contains(x.CompetenceID.ToString())
                  select x).ToList<ValueCompetence>();
            int nc = 0;
            int nk = 0;
            nc = VC.GroupBy(x => x.ClientID).Count();
            List<int> lc = db._Clients.Where(x => x.JobTitleID == jt && x.Manager == false).Select(x => x.ID).ToList<int>();
            lc = (from x in lc
                  join t in LCVC on x equals t.ClientID
                  where t.value >= rt && t.value <= rtmax
                  select x).ToList<int>();
            nk = VC.GroupBy(x => x.CompetenceID).Count();
            alglib.clusterizerstate s;
            alglib.ahcreport rep;

            double[,] xy = new double[nc, nk];
            double[,] xye = new double[nc, nk];

            int i = 0;
            int j = 0;
            foreach (int itemC in lc)
            {

                List<ValueCompetence> Vclient = VC.Where(x => x.ClientID == itemC).ToList<ValueCompetence>();
                foreach (ValueCompetence itemvc in Vclient)
                {
                    xy[i, j] = itemvc.value;
                    j++;
                }
                j = 0;
                i++;
            }
            for (int k = 0; k < xy.GetLength(1); k++)
            {
                double sumcomp = 0;
                double srcomp = 0;
                double srcomppow = 0;
                double sco = 0;
                for (int l = 0; l < xy.GetLength(0); l++)
                {
                    sumcomp += xy[l, k];
                }
                srcomp = sumcomp / (xy.GetLength(0));

                for (int l = 0; l < xy.GetLength(0); l++)
                {
                    xye[l, k] = Math.Pow(xy[l, k] - srcomp, 2);
                    srcomppow += xye[l, k];
                }
                sco = Math.Sqrt(srcomppow / (xy.GetLength(0) - 1));
                for (int l = 0; l < xy.GetLength(0); l++)
                {
                    xye[l, k] = (xy[l, k] - srcomp) / sco;
                }



            }
            int[] cidx;
            int[] cz;
            alglib.clusterizercreate(out s);
            alglib.clusterizersetpoints(s, xye, 2);

            alglib.clusterizerrunahc(s, out rep);
            int[] pa = new int[rep.p.Length];
            for (int l = 0; l < rep.p.Length; l++)
            {
                int ind = rep.p[l];
                pa[ind] = l;
            }
            alglib.clusterizergetkclusters(rep, Convert.ToInt32(kcount), out cidx, out cz);
            //alglib.clusterizersetkmeanslimits(s, 5, 0);
            //alglib.clusterizerrunkmeans(s, Convert.ToInt32(kcount), out rep1);
            if (rep.terminationtype == 1)
            {
                List<clientclust> listclients = new List<clientclust>();

                List<clientclust> listclientsclust = new List<clientclust>();






                for (int l = 0; l < lc.Count(); l++)
                {

                    int id = lc[l];
                    ClientCalcValueEdit itemlcvc = LCVC.Where(x => x.ClientID == id).SingleOrDefault<ClientCalcValueEdit>();
                    Clients cff = db._Clients.Where(x => x.JobTitleID == jt && x.ID == id && x.Manager == false).SingleOrDefault<Clients>();
                    try
                    {
                        listclients.Add(new clientclust { clusterid = cidx[l], clientfio = cff.ClientsInfo.FIO, clientid = cff.ID, clientNN = l, rate = itemlcvc.value });
                    }
                    catch (Exception)
                    {

                    }


                }
                listclients = listclients.OrderByDescending(x => x.rate).ToList<clientclust>();
                var toupdate = new object();
                for (int k = 0; k < Convert.ToInt32(kcount); k++)
                {
                    listclientsclust = (from g in listclients
                                        where g.clusterid == k
                                        select g).ToList<clientclust>();
                    foreach (var itemcclust in listclientsclust)
                    {
                        List<competenceclust> lcc = new List<competenceclust>();
                        foreach (ValueCompetence itemcomp in VC.Where(x => x.ClientID == itemcclust.clientid).ToList<ValueCompetence>())
                        {
                            lcc.Add(new competenceclust { competenceid = itemcomp.CompetenceID, shifr = itemcomp.Competences.Shifr, value = itemcomp.value, tittle = itemcomp.Competences.Name });
                        }

                        itemcclust.competences = lcc;
                    }


                    List<additionalcluster> lac = new List<additionalcluster>();
                    for (int l = 0; l < xy.GetLength(1); l++)
                    {
                        double[] compsclients = new double[listclientsclust.Count()];
                        for (int m = 0; m < listclientsclust.Count(); m++)
                        {
                            int nn = listclientsclust[m].clientNN;
                            compsclients[m] = xy[nn, l];
                        }
                        Endpoints ep = new Endpoints();
                        ep = NMath.ConfidenceInterval(compsclients, 1.96);
                        try
                        {
                            lac.Add(new additionalcluster { competenceid = listclientsclust[0].competences[l].competenceid, mean = ep.Average, bottom = ep.Left, top = ep.Right, shifr = listclientsclust[0].competences[l].shifr + ": " + listclientsclust[0].competences[l].tittle });
                        }
                        catch (Exception)
                        {


                        }

                    }





                    listclusters.Add(new clusters { clusterid = k, distance = rep.mergedist[k], clients = listclientsclust, additional = lac });
                }

            }
            else
            {
                return PartialView("ClusterDetailAdditional", null);
            }

            return PartialView("ClusterDetailAdditional", listclusters);
        }

        [CustomAuthorize("Plancreate")]
        public PartialViewResult ResultPlan(int competenceid, String levelid, String jidt, String clientid, int min, int max, int mark)
        {
            List<Competence> _competence = new List<Competence>();
            List<Certification> cert = db._Certification.ToList();
            List<CompetenceEdit> _come = new List<CompetenceEdit>();

            int iid = Convert.ToInt32(clientid);
            int jid = Convert.ToInt32(jidt);
            int Levidi = Convert.ToInt32(levelid);
            var JobTitleParam = new SqlParameter
            {
                ParameterName = "JobTitleID",
                Value = jid,
            };
            var LevelParam = new SqlParameter
            {
                ParameterName = "LevelID",
                Value = Levidi,
            };
            var ClientParam = new SqlParameter
            {
                ParameterName = "ClientID",
                Value = iid,
            };

            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == Levidi
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           orderby c.ID
                           select c).ToList<Competence>();

            foreach (Competence comp in _competence)
            {
                var z = db._JobTitleCo.Where(x => x.CompetenceID == comp.ID && x.JobTitleID == jid && x.LevelID == Levidi).Select(x => x.Value).SingleOrDefault();
                _come.Add(new CompetenceEdit { ID = comp.ID, Name = comp.Name, Shifr = comp.Shifr, Value = z.ToString() });


            }
            int i = 0;
            int j = 0;
            int n = _competence.Count();
            int an = 0;
            double sum2 = 0;
            double sum1 = 0;
            double maxsum1 = 0;
            double percent = 0;
            double maxsum2 = 0;

            List<CalcResult> LCR = db.Database.SqlQuery<CalcResult>("exec CalcResult @JobTitleID, @LevelID, @ClientID", JobTitleParam, LevelParam, ClientParam).ToList<CalcResult>();
            List<RANGEVALUES> RVC = null;


            RVC = new List<RANGEVALUES>();
            foreach (Competence comp in _competence)
            {
                List<CalcResult> c = LCR.Where(x => x.CompetenceID == comp.ID).ToList<CalcResult>();
                i = 0;
                j++;
                sum1 = 0;
                an = c.Count();
                maxsum1 = 0;
                foreach (CalcResult itemCalc in c)
                {
                    i++;
                    sum1 += itemCalc.CalcCol;
                    maxsum1 += itemCalc.Value * 5;
                }

                sum2 = sum1;
                maxsum2 = maxsum1;
                percent = Math.Round(sum2 * 100 / maxsum2, 2);
                RVC.Add(new RANGEVALUES { CompetenceID = comp.ID, Shifr = comp.Shifr, Name = comp.Name, _MAX = percent });
            }

            RVC = (from mmm in RVC
                   orderby mmm.CompetenceID
                   select mmm).ToList<RANGEVALUES>();
            RVC = (from x in RVC
                   where x._MAX <= max && x._MAX >= min
                   select x).ToList<RANGEVALUES>();
            an = 0;
            double calc = 0;
            i = 0;
            List<TempObject> TO;
            TO = null;
            TO = new List<TempObject>();

            foreach (RANGEVALUES _rvc in RVC)
            {
                i++;
                double ValueComp = Convert.ToDouble(_come.Where(x => x.ID == _rvc.CompetenceID).Select(x => x.Value).SingleOrDefault());
                List<CalcResult> c = LCR.Where(x => x.CompetenceID == _rvc.CompetenceID).Where(x => x.Grade <= mark).ToList<CalcResult>();
                double calccompet = 0;
                foreach (var calcitem in c)
                {
                    calccompet += calcitem.CalcCol;

                }
                calccompet = calccompet / 100;
                an = c.Count;
                calc = Math.Round(5 - calccompet, 2);

                TO.Add(new TempObject { NN = i, CompetenceID = _rvc.CompetenceID, Value = calc, ValueComp = ValueComp });

            }
            int index = (from x in TO
                         where x.CompetenceID == competenceid
                         select x.NN).FirstOrDefault();
            Properties.Ver = 0;
            Properties.Ver = TO.Count;
            N = Properties.Ver;
            MaxVer = N;
            Graph = new double[N + 1, N + 1];
            string str = "";
            for (int t = 0; t <= TO.Count - 1; t++)
            {
                str = "";
                for (int m = 0; m <= TO.Count - 1; m++)
                {


                    if (t == m)
                    {
                        Graph[t + 1, m + 1] = 0;
                        str = str + "0 ";
                    }
                    else
                    {

                        Graph[t + 1, m + 1] = Math.Round((TO[t].ValueComp / TO[m].ValueComp) * (TO[t].Value + TO[m].Value), 2);








                        str = str + Graph[t + 1, m + 1].ToString() + " ";

                    }

                }
            }
            int[] ShortPath = new int[N + 2];
            BranchAndBounds(Graph, index, ref ShortPath);
            String[] TL = new String[ShortPath.Length];
            String[] TL1 = new String[ShortPath.Length];
            double tsum1 = 0;
            double tsum2 = 0;
            for (int sh = 1; sh < ShortPath.Length - 2; sh++)
            {
                tsum1 += Graph[ShortPath[sh], ShortPath[sh + 1]];
            }

            for (int z = ShortPath.Length - 1; z > 2; z--)
            {
                tsum2 += Graph[ShortPath[z], ShortPath[z - 1]];
            }
            List<Competence> Compet = new List<Competence>();
            int coef = 0;
            if (tsum1 <= tsum2)
            {
                for (int z = 1; z < ShortPath.Length - 1; z++)
                {
                    coef++;
                    int compid = TO.Where(x => x.NN == ShortPath[z]).Select(x => x.CompetenceID).SingleOrDefault();
                    TL[coef] = RVC.Where(x => x.CompetenceID == compid).Select(x => x.Shifr).SingleOrDefault();
                    TL1[coef] = compid.ToString();
                    Competence comp = _competence.Where(x => x.ID == compid).SingleOrDefault();
                    Compet.Add(comp);
                }
            }
            else
            {
                for (int z = ShortPath.Length - 1; z > 1; z--)
                {
                    coef++;
                    int compid = TO.Where(x => x.NN == ShortPath[z]).Select(x => x.CompetenceID).SingleOrDefault();
                    TL[coef] = RVC.Where(x => x.CompetenceID == compid).Select(x => x.Shifr).SingleOrDefault();
                    TL1[coef] = compid.ToString();
                    Competence comp = _competence.Where(x => x.ID == compid).SingleOrDefault();
                    Compet.Add(comp);
                }
            }



            //for (int z = 1;z<ShortPath.Length; z++)
            //{
            //    coef++;
            //    int compid = TO.Where(x => x.NN == ShortPath[z]).Select(x => x.CompetenceID).SingleOrDefault();
            //    TL[coef] = RVC.Where(x => x.CompetenceID == compid).Select(x => x.Shifr).SingleOrDefault();
            //    TL1[coef] = compid.ToString();
            //    Competence comp = _competence.Where(x => x.ID == compid).SingleOrDefault();
            //    Compet.Add(comp);
            //}
            ViewBag.Competence = Compet;
            ViewBag.breadcrumb = TL;
            ViewBag.breadcrumb1 = TL1;
            ViewBag.mark = mark;

            Clients clin = db._Clients.Where(m => m.JobTitleID == jid && m.ID == iid && m.Manager == false).FirstOrDefault();
            ViewBag.FIO = clin.ClientsInfo.FIO;
            ViewBag.jobstr = db._JobTitle.Where(x => x.ID == jid).Select(x => x.Name).SingleOrDefault();
            ViewBag.ShortPath = ShortPath;
            ViewBag.competenceid = competenceid;
            ViewBag.levelid = levelid;
            ViewBag.jidt = jidt;
            ViewBag.clientid = clientid;
            ViewBag.min = min;
            ViewBag.max = max;
            ViewBag.mark = mark;
            List<DiscipSort> lds = new List<DiscipSort>();
            List<DiscipSort> ldsf = new List<DiscipSort>();
            using (var dbc = new CompetenceContext())
            {
                List<ClientsCards> lcc = dbc._ClientsCards.Include("Attributes").Where(x => x.ClientID == iid && x.LevelID == Levidi).ToList<ClientsCards>();
                List<DisciplineAttributes> lda = dbc._DisciplineAttributes.ToList<DisciplineAttributes>();
                List<DisciplineClient> ldc = dbc._DisciplineClient.Include("Disciplines").Where(x => x.ClientID == iid).ToList<DisciplineClient>();


                List<Listofvar> lac = (from itemx in lcc
                                       from itemy in lda
                                       where itemx.AttributeID == itemy.AttributesID
                                       from itemz in ldc
                                       where itemy.DisciplineID == itemz.DisciplineID && itemz.Mark <= mark
                                       from itemt in LCR
                                       where itemt.CompetenceID == itemy.Attributes.CompetenceID && itemt.AttributesID == itemy.AttributesID
                                       select new Listofvar { DisciplineID = itemy.DisciplineID, Discipline = itemz.Disciplines.Discipline, Mark = (int)itemz.Mark, CompetenceID = itemx.Attributes.CompetenceID, Value = itemt.Value, Time = itemz.Disciplines.Time }).Distinct().OrderBy(x => x.DisciplineID).ToList<Listofvar>();

                double R = 0f;
                double Rmax = 0f;
                double coeff = 0f;
                double ft = 0f;
                foreach (Listofvar item in lac)
                {

                    R = (int)item.Mark * (item.Value / 100);
                    Rmax = 5 * (item.Value / 100);

                    coeff = -1 * Math.Log(1 - (R / Rmax)) / item.Time;
                    ft = Rmax - (Rmax - R) * Math.Exp(-1 * coeff * item.Time);
                    Disciplines src = db._Disciplines.Where(x => x.ID == item.DisciplineID).FirstOrDefault();
                    lds.Add(new DiscipSort { ID = item.DisciplineID, name = item.Discipline, Rstart = R, Rend = Rmax, ft = ft, Time = item.Time, Source = src.Source != null ? src.Source : "" });


                }

                lds = lds.OrderBy(x => x.ID).ToList<DiscipSort>();
                var filterdisc = from s in lds
                                 group s by s.ID into g
                                 select new { ID = g.Key, ft = g.Max(s => s.ft) };

                foreach (var item in filterdisc)
                {
                    string name = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.name).FirstOrDefault();
                    double Rstart = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.Rstart).FirstOrDefault();
                    double Rend = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.Rend).FirstOrDefault();
                    int Time = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.Time).FirstOrDefault();
                    String Source = lds.Where(x => x.ID == item.ID && x.ft.ToString() == item.ft.ToString()).Select(x => x.Source).FirstOrDefault();
                    ldsf.Add(new DiscipSort { ID = item.ID, ft = item.ft, name = name, Rstart = Rstart, Rend = Rend, Time = Time, Source = Source != null ? Source : "" });
                }
            }

            ldsf = ldsf.OrderBy(x => x.ft).ToList();
            ViewBag.ldsf = ldsf;
            return PartialView("ResultPlan", LCR);



        }
        // GET: /Calculate/
        [CustomAuthorize("Plancreate")]
        public PartialViewResult SaveResultPlan(int competenceid, String levelid, String jidt, String clientid, int min, int max, int mark, string Competence)
        {

            List<Competence> _competence = new List<Competence>();
            List<Certification> cert = db._Certification.ToList();
            List<CompetenceEdit> _come = new List<CompetenceEdit>();

            int iid = Convert.ToInt32(clientid);
            int jid = Convert.ToInt32(jidt);
            int Levidi = Convert.ToInt32(levelid);
            var JobTitleParam = new SqlParameter
            {
                ParameterName = "JobTitleID",
                Value = jid,
            };
            var LevelParam = new SqlParameter
            {
                ParameterName = "LevelID",
                Value = Levidi,
            };
            var ClientParam = new SqlParameter
            {
                ParameterName = "ClientID",
                Value = iid,
            };

            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == Levidi
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           orderby c.ID
                           select c).ToList<Competence>();

            foreach (Competence comp in _competence)
            {
                var z = db._JobTitleCo.Where(x => x.CompetenceID == comp.ID && x.JobTitleID == jid && x.LevelID == Levidi).Select(x => x.Value).SingleOrDefault();
                _come.Add(new CompetenceEdit { ID = comp.ID, Name = comp.Name, Shifr = comp.Shifr, Value = z.ToString() });


            }
            int i = 0;
            int j = 0;
            int n = _competence.Count();
            int an = 0;
            double sum2 = 0;
            double sum1 = 0;
            double maxsum1 = 0;
            double percent = 0;
            double maxsum2 = 0;

            List<CalcResult> LCR = db.Database.SqlQuery<CalcResult>("exec CalcResult @JobTitleID, @LevelID, @ClientID", JobTitleParam, LevelParam, ClientParam).ToList<CalcResult>();
            List<RANGEVALUES> RVC = null;


            RVC = new List<RANGEVALUES>();
            foreach (Competence comp in _competence)
            {
                List<CalcResult> c = LCR.Where(x => x.CompetenceID == comp.ID).ToList<CalcResult>();
                i = 0;
                j++;
                sum1 = 0;
                an = c.Count();
                maxsum1 = 0;
                foreach (CalcResult itemCalc in c)
                {
                    i++;
                    sum1 += itemCalc.CalcCol;
                    maxsum1 += itemCalc.Value * 5;
                }

                sum2 = sum1;
                maxsum2 = maxsum1;
                percent = Math.Round(sum2 * 100 / maxsum2, 2);
                RVC.Add(new RANGEVALUES { CompetenceID = comp.ID, Shifr = comp.Shifr, Name = comp.Name, _MAX = percent });
            }

            RVC = (from mmm in RVC
                   orderby mmm.CompetenceID
                   select mmm).ToList<RANGEVALUES>();
            RVC = (from x in RVC
                   where x._MAX <= max && x._MAX >= min
                   select x).ToList<RANGEVALUES>();
            an = 0;
            double calc = 0;
            i = 0;
            List<TempObject> TO;
            TO = null;
            TO = new List<TempObject>();

            foreach (RANGEVALUES _rvc in RVC)
            {
                i++;
                double ValueComp = Convert.ToDouble(_come.Where(x => x.ID == _rvc.CompetenceID).Select(x => x.Value).SingleOrDefault());
                List<CalcResult> c = LCR.Where(x => x.CompetenceID == _rvc.CompetenceID).Where(x => x.Grade <= mark).ToList<CalcResult>();
                double calccompet = 0;
                foreach (var calcitem in c)
                {
                    calccompet += calcitem.CalcCol;

                }
                calccompet = calccompet / 100;
                an = c.Count;
                calc = Math.Round(5 - calccompet, 2);

                TO.Add(new TempObject { NN = i, CompetenceID = _rvc.CompetenceID, Value = calc, ValueComp = ValueComp });

            }
            int index = (from x in TO
                         where x.CompetenceID == competenceid
                         select x.NN).FirstOrDefault();
            Properties.Ver = 0;
            Properties.Ver = TO.Count;
            N = Properties.Ver;
            MaxVer = N;


            List<int> ShortPath = Competence.Split(',').Select(Int32.Parse).ToList();
            List<Competence> Compet = new List<Competence>();
            String[] TL = new String[ShortPath.Count];
            String[] TL1 = new String[ShortPath.Count];
            int coef = 0;



            double tsum1 = 0;
            double tsum2 = 0;





            if (tsum1 <= tsum2)
            {
                for (int z = 1; z < ShortPath.Count - 1; z++)
                {
                    coef++;
                    int compid = TO.Where(x => x.NN == ShortPath[z]).Select(x => x.CompetenceID).SingleOrDefault();
                    TL[coef] = RVC.Where(x => x.CompetenceID == compid).Select(x => x.Shifr).SingleOrDefault();
                    TL1[coef] = compid.ToString();
                    Competence comp = _competence.Where(x => x.ID == compid).SingleOrDefault();
                    Compet.Add(comp);
                }
            }
            else
            {
                for (int z = ShortPath.Count - 1; z > 1; z--)
                {
                    coef++;
                    int compid = TO.Where(x => x.NN == ShortPath[z]).Select(x => x.CompetenceID).SingleOrDefault();
                    TL[coef] = RVC.Where(x => x.CompetenceID == compid).Select(x => x.Shifr).SingleOrDefault();
                    TL1[coef] = compid.ToString();
                    Competence comp = _competence.Where(x => x.ID == compid).SingleOrDefault();
                    Compet.Add(comp);
                }
            }






            n = Compet.Count();

            int clientidint = Convert.ToInt32(clientid);
            groupident LastGroupID = new groupident();

            Plans plan = new Plans();
            DateTime DateTime = DateTime.Now;





            plan.BeginCompetence = competenceid;
            plan.ClientID = Convert.ToInt32(clientid);

            plan.LevelID = Convert.ToInt32(levelid);
            plan.JobID = Convert.ToInt32(jidt);
            plan.min = min;
            plan.max = max;
            plan.mark = mark;

            List<Plans> planed = db._Plans.Where(x => x.ClientID == plan.ClientID).ToList<Plans>();
            db._Plans.RemoveRange(planed);
            db.SaveChanges();

            db._Plans.Add(plan);
            db.SaveChanges();


















            return PartialView("SaveResultPlan", null);
        }
        [CustomAuthorize("Plancreate")]
        public ActionResult BranchAndBound(String IDCLIENT, String levid, String pid, String jid, String Cvalif, String PComp)
        {

            List<CompetenceEdit> _come = new List<CompetenceEdit>();
            List<Competence> _competence = new List<Competence>();
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            int ipid = Convert.ToInt32(pid);
            int iid = Convert.ToInt32(IDCLIENT);
            int ijid = Convert.ToInt32(jid);
            var profstr = db._ProfArea.Where(x => x.ID == ipid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == ijid).Select(x => x.Name).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();

            int Levidi = Convert.ToInt32(levid);
            List<Certification> cert = db._Certification.ToList();

            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == Levidi
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           select c).ToList<Competence>();

            foreach (Competence comp in _competence)
            {
                var z = db._JobTitleCo.Where(x => x.CompetenceID == comp.ID && x.JobTitleID == ijid && x.LevelID == Levidi).Select(x => x.Value).SingleOrDefault();
                _come.Add(new CompetenceEdit { ID = comp.ID, Name = comp.Name, Shifr = comp.Shifr, Value = z.ToString() });


            }
            ViewBag.Come = _come;
            ViewBag.ClienD = iid;
            ViewBag.ClientId = db._Clients.Where(x => x.JobTitleID == ijid && x.ID == iid && x.Manager == false).Select(x => x.ClientInfoID).SingleOrDefault().ToString();
            ViewBag.ClientName = db._Clients.Where(x => x.JobTitleID == ijid && x.ID == iid && x.Manager == false).Select(x => x.ClientsInfo.FIO).SingleOrDefault().ToString();
            ViewBag.LevelName = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Name).SingleOrDefault().ToString();
            ViewBag.LevelLev = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Lev).SingleOrDefault().ToString(); ;
            ViewBag.LevelID = Levidi;
            ViewBag.Cvalif = Cvalif;
            ViewBag.PComp = PComp;
            ViewBag.FIO = db._Clients.Where(m => m.JobTitleID == ijid && m.ID == iid && m.Manager == false).Select(m => m.ClientsInfo.FIO).SingleOrDefault().ToString();
            var model = new CompetenceEditViewModel();

            var JobTitleParam = new SqlParameter
            {
                ParameterName = "JobTitleID",
                Value = jid,
            };
            var LevelParam = new SqlParameter
            {
                ParameterName = "LevelID",
                Value = Levidi,
            };
            var ClientParam = new SqlParameter
            {
                ParameterName = "ClientID",
                Value = iid,
            };
            List<CalcResult> LCR = db.Database.SqlQuery<CalcResult>("exec CalcResult @JobTitleID, @LevelID, @ClientID", JobTitleParam, LevelParam, ClientParam).ToList<CalcResult>();

            int i = 0;
            int j = 0;
            int n = _competence.Count();
            int an = 0;
            double sum2 = 0;
            double sum1 = 0;
            double maxsum1 = 0;
            double percent = 0;
            double maxsum2 = 0;
            List<RANGEVALUES> RVC = new List<RANGEVALUES>();
            foreach (Competence comp in _competence)
            {
                List<CalcResult> c = LCR.Where(x => x.CompetenceID == comp.ID).ToList<CalcResult>();
                i = 0;
                j++;
                sum1 = 0;
                an = c.Count() + 1;
                maxsum1 = 0;
                foreach (CalcResult itemCalc in c)
                {
                    i++;
                    sum1 += itemCalc.CalcCol;
                    maxsum1 += itemCalc.Value * 5;
                }

                sum2 = sum1;
                maxsum2 = maxsum1;
                percent = Math.Round(sum2 * 100 / maxsum2, 2);
                RVC.Add(new RANGEVALUES { CompetenceID = comp.ID, Shifr = comp.Shifr, Name = comp.Name, _MAX = percent });
            }
            RVC = (from mmm in RVC
                   orderby mmm._MAX descending
                   where double.IsNaN(mmm._MAX) == false
                   select mmm).ToList<RANGEVALUES>();
            ViewBag.RVC = RVC;
            model.CompetenceEdits = _come;
            return View(model);

        }
        [CustomAuthorize("Plancreate")]
        public PartialViewResult GetRowDetail(int id, string pid, string jid, string lid)
        {
            List<Competence> _competence = new List<Competence>();
            int pidi = Convert.ToInt32(pid);
            int jidi = Convert.ToInt32(jid);
            int lidi = Convert.ToInt32(lid);
            var JobTitleParam = new SqlParameter
            {
                ParameterName = "JobTitleID",
                Value = jidi,
            };
            var LevelParam = new SqlParameter
            {
                ParameterName = "LevelID",
                Value = lidi,
            };
            var ClientParam = new SqlParameter
            {
                ParameterName = "ClientID",
                Value = id,
            };
            List<CalcResult> LCR = db.Database.SqlQuery<CalcResult>("exec CalcResult @JobTitleID, @LevelID, @ClientID", JobTitleParam, LevelParam, ClientParam).ToList<CalcResult>();
            List<Certification> cert = db._Certification.ToList();
            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == lidi
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           select c).ToList<Competence>();

            ViewBag.Competence = _competence;
            ViewBag.Rate = (from k in LCR
                            join t in _competence on k.CompetenceID equals t.ID
                            group k by new { k.CompetenceID, t.Shifr }
                                into grouped
                                select new CalcGroupCOResult
                                {
                                    Shifr = grouped.Key.Shifr,
                                    Value = Math.Round(grouped.Sum(x => x.CalcCol) / 100, 2)
                                }).ToList<CalcGroupCOResult>();


            ViewBag.FIO = db._Clients.Where(m => m.JobTitleID == jidi && m.ID == id && m.Manager == false).Select(m => m.ClientsInfo.FIO).SingleOrDefault().ToString();
            ViewBag.jobstr = db._JobTitle.Where(x => x.ID == jidi).Select(x => x.Name).SingleOrDefault();
            return PartialView("ResultDetails", LCR);

        }
        [CustomAuthorize("Plancreate")]
        public ActionResult PJList()
        {
            bool Manage = false;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            UserProfile up = new UserProfile();
            int prof = 0;
            using (var db = new UsersContext())
            {
                up = db.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                if (up.ClientsID != null)
                {
                    using (var dbc = new CompetenceContext())
                    {
                        Clients client = dbc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                        Manage = client.Manager;
                        prof = client.JobTitle.ProfAreaID;
                    }
                }

            }
            ViewBag.Manage = Manage;
            List<SelectListItem> pa = new List<SelectListItem>();
            pa.Add(new SelectListItem { Value = "0", Text = "Не выбрано" });
            if (Manage)
            {
                foreach (ProfArea item in db._ProfArea.ToList())
                {
                    if (item.ID == prof)
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, Selected = true });
                    }
                    else
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                    }

                }
            }
            else
            {
                foreach (ProfArea item in db._ProfArea.ToList())
                {

                    pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                }
            }
            ViewBag.Prof = pa;
            return View(pa);
        }
        [CustomAuthorize("Plancreate")]
        public ActionResult ShowPJList(String targv, String Prof)
        {
            int pid = Convert.ToInt32(Prof);
            int jid = Convert.ToInt32(targv);
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            var profstr = db._ProfArea.Where(x => x.ID == pid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == jid).Select(x => x.Name).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();
            return PartialView("SelectCalculateMethod", db._Levels.ToList());


        }
        [CustomAuthorize("Plancreate")]
        public PartialViewResult GetListDetail(int id)
        {
            bool Manage = false;
            int uid = WebSecurity.GetUserId(User.Identity.Name);
            UserProfile up = new UserProfile();
            int job = 0;
            using (var dbm = new UsersContext())
            {
                up = dbm.UserProfiles.Where(x => x.UserId == uid).SingleOrDefault();
                if (up.ClientsID != null)
                {
                    using (var dbc = new CompetenceContext())
                    {
                        Clients client = dbc._Clients.Where(x => x.ID == up.ClientsID).SingleOrDefault();
                        Manage = client.Manager;
                        job = client.JobTitleID;
                    }
                }

            }
            List<JobTitle> _attributes = db._JobTitle.Where(a => a.ProfAreaID == id).ToList<JobTitle>();
            List<SelectListItem> pa = new List<SelectListItem>();
            if (Manage)
            {
                foreach (JobTitle item in _attributes)
                {
                    if (item.ID == job)
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name, Selected = true });
                    }
                    else
                    {
                        pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                    }

                }
            }
            else
            {
                foreach (JobTitle item in _attributes)
                {

                    pa.Add(new SelectListItem { Value = item.ID.ToString(), Text = item.Name });
                }
            }
            var model = pa;
            return PartialView("ListDetails", model);

        }
        [CustomAuthorize("Plancreate")]
        public ActionResult ShowMethods(String levelid, int pidt, int jidt)
        {
            ViewBag.Levid = levelid;
            ViewBag.pid = pidt;
            ViewBag.jid = jidt;
            return PartialView("Methods", null);


        }
        [CustomAuthorize("Plancreate")]
        public ActionResult ShowResult(String IDCLIENT, String Levid, String pid, String jid)
        {




            using (var dbv = new UsersContext())
            {
                foreach (var item in dbv.UserProfiles)
                {

                    if (item.ClientsID != null)
                    {
                        using (var cc = new CompetenceContext())
                        {
                            Clients client = new Clients();
                            client = cc._Clients.Where(x => x.ID == item.ClientsID).SingleOrDefault();
                            var ClientParam = new SqlParameter
                            {
                                ParameterName = "ClientID",
                                Value = (int)item.ClientsID,
                            };
                            var ActParam = new SqlParameter
                            {
                                ParameterName = "ActID",
                                Value = (int)client.DefaultActivityId,
                            };
                            List<object> LCR = cc.Database.SqlQuery<CalcResult>("exec UpdateGrade  @ClientID, @ActID", ClientParam, ActParam).ToList<object>();


                        }
                    }
                }




            }



            List<CompetenceEdit> _come = new List<CompetenceEdit>();
            List<ClientCalcValueEdit> LCCV = new List<ClientCalcValueEdit>();
            ViewBag.pid = pid;
            ViewBag.jid = jid;
            int ipid = Convert.ToInt32(pid);
            int iid = Convert.ToInt32(IDCLIENT);
            int ijid = Convert.ToInt32(jid);
            int levidi = Convert.ToInt32(Levid);
            var profstr = db._ProfArea.Where(x => x.ID == ipid).Select(x => x.Name).SingleOrDefault();
            var jobstr = db._JobTitle.Where(x => x.ID == ijid).Select(x => x.Name).SingleOrDefault();
            ViewBag.Profstr = profstr.ToString();
            ViewBag.Jobstr = jobstr.ToString();
            int Levidi = Convert.ToInt32(Levid);
            List<Certification> cert = db._Certification.ToList();

            var JobTitleParam = new SqlParameter
            {
                ParameterName = "JobTitleID",
                Value = ijid,
            };
            var LevelParam = new SqlParameter
            {
                ParameterName = "LevelID",
                Value = levidi,
            };
            List<CalcCo> lsc = db.Database.SqlQuery<CalcCo>("exec CalcCo @JobTitleID, @LevelID", JobTitleParam, LevelParam).ToList<CalcCo>();
            List<Clients> clients = db._Clients.Where(x => x.JobTitleID == ijid && x.Manager == false).ToList<Clients>();
            List<ValueCompetence> VC = db._ValueCompetence.Where(x => x.JobTitleID == ijid && x.LevelID == levidi).ToList<ValueCompetence>();
            var select = from x in db._CalcValueCompetence
                         where x.LevelID == Levidi && x.JobTitleID == ijid
                         select x;
            db._CalcValueCompetence.RemoveRange(select);
            db.SaveChanges();
            foreach (Clients client in clients)
            {

                List<Calculate> CVC = new List<Calculate>();
                List<ValueCompetence> _clientVC = VC.Where(x => x.ClientID == client.ID).OrderBy(x => x.CompetenceID).ToList<ValueCompetence>();
                foreach (ValueCompetence clientVC in _clientVC)
                {
                    double Max = lsc.Where(x => x.CompetenceID == clientVC.CompetenceID).Select(x => x._MAX).SingleOrDefault();
                    double Min = lsc.Where(x => x.CompetenceID == clientVC.CompetenceID).Select(x => x._MiN).SingleOrDefault();
                    double z = db._JobTitleCo.Where(x => x.CompetenceID == clientVC.CompetenceID && x.JobTitleID == ijid && x.LevelID == Levidi).Select(x => x.Value).SingleOrDefault();
                    double Val = 0;
                    if (clientVC.value <= Min)
                    {
                        if (iid == 0)
                        {
                            Val = 0;
                        }
                        else if (iid == 1)
                        {
                            Val = 1;
                        }

                    }
                    else
                        if (clientVC.value > Min && clientVC.value <= Max)
                        {
                            if (iid == 0)
                            {
                                Val = (clientVC.value - Min) / (Max - Min);
                            }
                            else if (iid == 1)
                            {
                                Val = (Max - clientVC.value) / (Max - Min);
                            }
                        }
                        else
                            if (clientVC.value > Max)
                            {
                                if (iid == 0)
                                {
                                    Val = 1;
                                }
                                else if (iid == 1)
                                {
                                    Val = 0;
                                }
                            }

                    Val = Val * z;
                    CVC.Add(new Calculate { value = Val });

                }
                double sum = 0;
                foreach (Calculate item in CVC)
                {
                    sum = sum + item.value;
                }
                db._CalcValueCompetence.Add(new CalcValueCompetence { ClientID = client.ID, JobTitleID = ijid, LevelID = Levidi, value = Math.Round(sum, 2) });


            }

            db.SaveChanges();
            List<ClientCalcValueEdit> LCVC = new List<ClientCalcValueEdit>();
            int i = 0;
            foreach (CalcValueCompetence item in db._CalcValueCompetence.Where(x => x.LevelID == Levidi && x.JobTitleID == ijid).OrderByDescending(x => x.value).ToList<CalcValueCompetence>())
            {
                i++;
                LCCV.Add(new ClientCalcValueEdit { ID = i, ClientID = item.ClientID, ClientName = db._Clients.Where(x => x.ID == item.ClientID && x.Manager == false).Select(x => x.ClientsInfo.FIO).SingleOrDefault(), value = item.value });
            }

            var model = new ClientCalcValueEditViewModel();
            model.ClientCalcValueEdits = LCCV;
            ViewBag.LevelName = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Name).SingleOrDefault().ToString();
            ViewBag.LevelLev = db._Levels.Where(x => x.ID == Levidi).Select(x => x.Lev).SingleOrDefault().ToString(); ;
            ViewBag.LevelID = Levid;
            if (iid == 0)
            {
                ViewBag.Cvalif = "СПЕЦИАЛИСТЫ С ВЫСОКИМ УРОВНЕМ КВАЛИФИКАЦИИ";
            }
            else
                if (iid == 1)
                {
                    ViewBag.Cvalif = "СПЕЦИАЛИСТЫ С НИЗКИМ УРОВНЕМ КВАЛИФИКАЦИИ";
                }
            ViewBag.MinMaxTable = (from x in lsc
                                   join t in db._Competence on x.CompetenceID equals t.ID
                                   where x.LevelID == levidi && x.JobTitleID == ijid
                                   select new RANGEVALUES { Shifr = t.Shifr, Name = t.Name, CompetenceID = x.CompetenceID, _MiN = Math.Round(x._MiN / 100, 2), _MAX = Math.Round(x._MAX / 100, 2), MinClientID = x.MinClientID, MinClientName = db._Clients.Where(m => m.JobTitleID == ijid && m.ID == x.MinClientID).Select(m => m.ClientsInfo.FIO).SingleOrDefault().ToString(), MaxClientID = x.MaxClientID, MaxClientName = db._Clients.Where(m => m.JobTitleID == ijid && m.ID == x.MaxClientID).Select(m => m.ClientsInfo.FIO).SingleOrDefault().ToString() }).ToList<RANGEVALUES>();

            List<Competence> _competence = new List<Competence>();
            _competence = (from c in db._Competence.ToList()
                           where c.LevelID == Levidi
                           from k in cert
                           where k.LevelID == c.LevelID && k.CompetenceID == c.ID
                           select c).ToList<Competence>();

            foreach (Competence comp in _competence)
            {
                var z = db._JobTitleCo.Where(x => x.CompetenceID == comp.ID && x.JobTitleID == ijid && x.LevelID == Levidi).Select(x => x.Value).SingleOrDefault();
                _come.Add(new CompetenceEdit { ID = comp.ID, Name = comp.Name, Shifr = comp.Shifr, Value = z.ToString() });


            }
            ViewBag.Come = _come;
            return View(model);

        }



    }
}