using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{
    public class CompetenceManager
    {


        static Random random = new Random();
        public double GetRandomNumber(int minimum, int maximum)
        {
            double digit=random.NextDouble() * (maximum - minimum) + minimum;

            return Math.Truncate(digit * 100.0) / 100.0;
        }

        //public virtual void GenerateGradeList(int count, int minimum, int maximum)
        //{
        //    List<Certification> Certs = new List<Certification>();
        //    List<Levels> Levels = new List<Levels>();
        //    List<Attributes> Attributes = new List<Attributes>();
        //    List<Competence> Competences = new List<Competence>();
        //    using (CompetenceContext db = new CompetenceContext())
        //    {
        //        Competences = (from k in db._Competence
        //                       select k).ToList<Competence>();
        //        Attributes = (from k in db._Attributes
        //                      select k).ToList<Attributes>();
        //        Levels = (from k in db._Levels
        //                  select k).ToList<Levels>();
        //        Certs = (from k in db._Certification
        //                 select k).ToList<Certification>();
        //        var list = from cert in Certs
        //                   from Lev in Levels
        //                   where cert.LevelID == Lev.ID
        //                   from Comp in Competences
        //                   where Comp.ID == cert.CompetenceID
        //                   from Atr in Attributes
        //                   where Atr.CompetenceID == cert.CompetenceID
        //                   select new { Atr.ID };
        //        for (int i = 12; i <= count; i++)
        //        {


        //            foreach (var item in list)
        //            {
        //                for (int j = 1; j < 4; j++)
        //                {


        //                    ClientsCards ClientsCards = new ClientsCards();
        //                    ClientsCards.ClientID = i;
        //                    ClientsCards.LevelID = j;
        //                    ClientsCards.AttributeID = item.ID;
        //                    ClientsCards.Grade = GetRandomNumber(minimum, maximum);
        //                    db._ClientsCards.Add(ClientsCards);
        //                }
                       

        //            }
        //            db.SaveChanges();
                
        //        }
        //    }
        //}
    }
}