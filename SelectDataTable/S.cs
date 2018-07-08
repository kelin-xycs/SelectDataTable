using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace SelectDataTable
{
    public class S
    {
        public static DataTable Select(DataTable dt, string sql)
        {
            List<string> columnList;
            Express whereExpress;

            SqlParser.Parse(sql, out columnList, out whereExpress);

            DataTable dt2 = new DataTable();

            DataRow dr;
            DataRow dr2;

            string columnName;

            DataColumn dataColumn;

            for(int i = 0; i<columnList.Count; i++)
            {
                columnName = columnList[i];

                if (columnName == "*")
                {
                    AddAllColumns(dt, dt2);
                    continue;
                }

                dataColumn = new DataColumn(columnName, dt.Columns[columnName].DataType);

                dt2.Columns.Add(dataColumn);
            }

            for(int i = 0; i<dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];


                if (whereExpress != null)
                {
                    if (!whereExpress.Exec(dr))
                        continue;
                }


                dr2 = dt2.NewRow();

                for(int j = 0; j<columnList.Count; j++)
                {
                    columnName = columnList[j];

                    dr2[columnName] = dr[columnName];
                }

                dt2.Rows.Add(dr2);
                
            }

            return dt2;
        }

        private static void AddAllColumns(DataTable dt, DataTable dt2)
        {

            DataColumn dataColumn;

            DataColumn dataColumn2;


            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dataColumn = dt.Columns[i];

                dataColumn2 = new DataColumn(dataColumn.ColumnName, dataColumn.DataType);

                dt2.Columns.Add(dataColumn2);
            }
        }
    }
}
