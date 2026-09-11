using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{
    public class DataTableModel
    {


        /// <summary>
        /// Gets or Sets the equest sequence number sent by DataTables, the same value must be returned in the json response.
        /// </summary>
        public string sEcho { get; set; }

        /// <summary>
        /// Gets or Sets the text used for filtering.
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Gets or Sets the number of records that should be shown in the table.
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iTotalRecords { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }





        
    }
}