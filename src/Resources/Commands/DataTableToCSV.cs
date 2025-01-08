using System.Data;
using System.Text;

namespace IAMHeimdall.Resources
{
    public class DataTableToCSV
    {
        #region Functions
        // Create a String on passed Datatable and Seperator into a CSV String
        public static string DataTableCSVConversion(DataTable datatable, char seperator)
        {
            StringBuilder sb = new();
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                sb.Append(datatable.Columns[i]);
                if (i < datatable.Columns.Count - 1)
                    sb.Append(seperator);
            }
            sb.AppendLine();
            foreach (DataRow dr in datatable.Rows)
            {
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    sb.Append(dr[i].ToString());

                    if (i < datatable.Columns.Count - 1)
                        sb.Append(seperator);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        #endregion
    }
}
