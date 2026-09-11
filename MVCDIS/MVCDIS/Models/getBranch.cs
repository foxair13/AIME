using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{
    public class Branch
    {

        CompetenceContext db = new CompetenceContext();
        public bool isLast(int id)
        {

            return db.tableIasaWorktypesTree.Where(p => p.ParentId == id && p.IsDeleted != true).Count() > 0 ? false : true;
        }
    }

    public class Nodes
    {
        public List<iasaWorktypesTree> breadcrumbs { get; set; }
        public Dictionary<iasaWorktypesTree, bool?> lNodes { get; set; }
        public int currentId { get; set; }
    }
    public class BranchString
    {
        public int id { get; set; }
        public int? pId { get; set; }
        public string name { get; set; }
        public bool isParent { get; set; }

    }
}