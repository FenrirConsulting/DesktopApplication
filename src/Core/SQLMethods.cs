using System;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Threading.Tasks;
using IAMHeimdall.MVVM.Model;
using System.DirectoryServices.AccountManagement;

namespace IAMHeimdall.Core
{
    public class SQLMethods : IDisposable
    {
        #region Delegates
        private readonly string connectionString = Program.ConnectionString;
        private SqlConnection conn;
        #endregion

        #region Methods
        public SQLMethods()
        {
            //Constructor
        }

        ~SQLMethods()
        {
            conn = null;
        }

        public string ConnectToDatabase()
        {
            conn = new SqlConnection(Program.ConnectionString);
            try
            {
                conn.Open();
                return "Connected";
            }
            catch (SqlException e)
            {
                conn = null;
                return e.Message;
            }
        }

        public void Disconnect()
        {
            conn.Close();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region SQL Select Functions
        // Return a Specific Record with Table, Column, Reference Parameters
        public DataTable GetSelectedRecord(string table, string col, string referenceID)
        {
            DataTable dt = new();

            try
            {
                ConnectToDatabase();
                string strCommand = "SELECT * FROM " + table + " WHERE " + col + "=@ID";
                SqlCommand cmd = new(strCommand, conn);
                cmd.Parameters.AddWithValue("@ID", referenceID);
                SqlDataReader da = cmd.ExecuteReader();

                dt.Load(da);
                Disconnect();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Return a Specific Record with Table, Column, Reference Parameters as an Async Task
        public async Task<DataTable> GetSelectedRecordAsync(string table, string col, string referenceID)
        {
            DataTable dt = new();

            try
            {
                ConnectToDatabase();
                string strCommand = "SELECT * FROM " + table + " WHERE " + col + "=@ID";
                SqlCommand cmd = new(strCommand, conn);
                cmd.Parameters.AddWithValue("@ID", referenceID);
                SqlDataReader da = cmd.ExecuteReader();
                await Task.Run(() => { dt.Load(da); });
                Disconnect();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }

        }

        // Return a list of Doubles given a Table and Column Parameter
        public List<double> GetColumnListDouble(string table, string col)
        {
            try
            {
                ConnectToDatabase();
                var columnList = new List<double>();
                string query = "Select " + col + " FROM " + table;

                using (SqlCommand sqlCommand = new("Select " + col + " FROM " + table, conn))
                {
                    using var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        columnList.Add(reader.GetDouble(reader.GetOrdinal(col)));
                    }
                }
                conn.Close();
                return columnList;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Return a list of Ints given a Table and Column Parameter
        public List<int> GetColumnListInt(string table, string col)
        {
            try
            {
                ConnectToDatabase();
                var columnList = new List<int>();
                string query = "Select " + col + " FROM " + table;

                using (SqlCommand sqlCommand = new("Select " + col + " FROM " + table, conn))
                {
                    using var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        columnList.Add(reader.GetInt32(reader.GetOrdinal(col)));
                    }
                }
                conn.Close();
                return columnList;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Return a list of Strings given a Table and Column Parameter
        public List<string> GetColumnList(string table, string col)
        {

            try
            {
                ConnectToDatabase();
                var columnList = new List<string>();
                string query = "Select " + col + " FROM " + table;

                using (SqlCommand sqlCommand = new("Select " + col + " FROM " + table, conn))
                {
                    using var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            columnList.Add(reader.GetString(0));
                    }
                }
                conn.Close();
                return columnList;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Returns Table from Select
        public DataTable GetTable(string table)
        {
            try
            {
                ConnectToDatabase();
                string query = "select * from " + table;
                SqlCommand cmd = new(query, conn);
                SqlDataReader da = cmd.ExecuteReader();
                DataTable dt = new();
                dt.Load(da);
                da.Dispose();
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Returns Joined Table from Select
        public DataTable GetJoinedTable(string table, string joinedTable ,string col)
        {
            try
            {
                ConnectToDatabase();
                string query = "select * from " + table + " LEFT JOIN " + joinedTable + " ON " + table + "@val1" + " = " + joinedTable + "@val1";
                SqlCommand cmd = new(query, conn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@Val", col);
                SqlDataReader da = cmd.ExecuteReader();
                DataTable dt = new();
                dt.Load(da);
                da.Dispose();
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Returns Table from Select As Async Task
        public async Task<DataTable> GetTableAsync(string table)
        {
            try
            {
                ConnectToDatabase();
                string query = "select * from " + table;
                SqlCommand cmd = new(query, conn);
                SqlDataReader da = cmd.ExecuteReader();
                DataTable dt = new();
                await Task.Run(() => { dt.Load(da); });
                da.Dispose();
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Returns Joined Table from Select As Async Task
        public async Task<DataTable> GetJoinedTableAsync(string table, string joinedTable, string col)
        {
            try
            {
                ConnectToDatabase();
                string query = "select * from " + table + " LEFT JOIN " + joinedTable + " ON " + table + "." + "@val1" + " = " + joinedTable + "." + "@val1";
                MessageBox.Show(query);
                SqlCommand cmd = new(query, conn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                SqlDataReader da = cmd.ExecuteReader();
                DataTable dt = new();
                await Task.Run(() => { dt.Load(da); });
                da.Dispose();
                conn.Close();
                return dt;
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Source + "\n" + e.Message + "\n" + e.StackTrace);
                ExceptionOutput.Output(e.ToString());
                Disconnect();
                return null;
            }
        }

        // Returns Joined Request Table Query
        public async Task<DataTable> GetJoinedRequestTable(string table, string joinedTable, string col)
        {
            try
            {
                ConnectToDatabase();
                string query = "select * from IAMHFactoolRequestStatus LEFT JOIN IAMHFactoolRequest ON IAMHFactoolRequestStatus.ReferenceNumber = IAMHFactoolRequest.ReferenceNumber";
                SqlCommand cmd = new(query, conn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                SqlDataReader da = cmd.ExecuteReader();
                DataTable dt = new();
                await Task.Run(() => { dt.Load(da); });
                da.Dispose();
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Returns Joined Request Table Query With a Date Range
        public async Task<DataTable> GetJoinedRequestTableWithRange(string table, string joinedTable, string col, string lowval, string highval)
        {
            try
            {
                ConnectToDatabase();
                string query = "select * from IAMHFactoolRequestStatus LEFT JOIN IAMHFactoolRequest ON IAMHFactoolRequestStatus.ReferenceNumber = IAMHFactoolRequest.ReferenceNumber where " + col + " >= @val1 and " + col + "<= @val2";
                SqlCommand cmd = new(query, conn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@val1", lowval);
                cmd.Parameters.AddWithValue("@val2", highval);
                SqlDataReader da = cmd.ExecuteReader();
                DataTable dt = new();
                await Task.Run(() => { dt.Load(da); });
                da.Dispose();
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }

        // Returns Table from Select As Async Task
        public async Task<DataTable> GetTableWithRange(string table, string col, string lowval, string highval)
        {
            try
            {
                DataTable dt = new();
                ConnectToDatabase();
                string query = "select * from " + table + " where " + col + " >= @val1 and " + col + "<= @val2";
                SqlCommand cmd = new(query, conn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@val1", lowval);
                cmd.Parameters.AddWithValue("@val2", highval);
                SqlDataReader da = cmd.ExecuteReader();
                await Task.Run(() => { dt.Load(da); });
                da.Dispose();
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }
        #endregion

        #region SQL Procedure Functions
        // Returns DataTable from Stored MSSQL Procedure
        public async Task<DataTable> GetStoredProcedure(string proc)
        {
            DataTable dt = new();
            try
            {
                ConnectToDatabase();
                using (var cmd = new SqlCommand(proc, conn))
                using (SqlDataReader da = cmd.ExecuteReader())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await Task.Run(() => { dt.Load(da); });
                }
                Disconnect();
                return dt;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return null;
            }
        }
        #endregion

        #region SQL Delete Functions
        // Delete Matching Record Given Table, Column, & Value Parameters
        public int DeleteMatchingRecords(string table, string col, string value)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "DELETE FROM " + table + " WHERE " + col + "=@Val";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@Val", value);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Delete Records Greater than Passed Value
        public int DeleteRecordsLessThan(string table, string col, string value)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "DELETE FROM " + table + " WHERE " + col + " <@Val";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@Val", value);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Records Deleted.");
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Delete Records From Factool Request Status that do not exist in Request Table
        public int DeleteFactoolStatusRecords()
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Delete from IAMHFactoolRequestStatus " +
                                    "Where ReferenceNumber in " +
                                    "( SELECT IAMHFactoolRequestStatus.ReferenceNumber FROM IAMHFactoolRequestStatus " +
                                    "LEFT JOIN IAMHFactoolRequestTestData ON IAMHFactoolRequestTestData.ReferenceNumber = IAMHFactoolRequestStatus.ReferenceNumber " +
                                    "WHERE IAMHFactoolRequestTestData.ReferenceNumber IS NULL )";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Delete All Records from Passed Table
        public int DeleteAllTableRecords(string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "DELETE FROM " + table;
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }
        #endregion

        #region SQL Update Functions
        //Updates A Single Column in a Table Record Given : Table, Reference Column, Reference ID Value, Update Column, Update Value
        public int UpdateTableRecord(string table, string idValue, string idCol, string updateCol, string updateValue)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set " + updateCol + " =@updateValue " + " WHERE " + idCol + " =@idValue ";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@idValue", idValue);
                cmdUpdate.Parameters.AddWithValue("@updateValue", updateValue);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A UIDTable Record Given Parameters for Columns
        public int UidTableRecordUpdate(string uid_uidref, string uid_logid, string uid_adid, string uid_lob, string uid_empid, string uid_firstname, string uid_lastname, string uid_tickets, string uid_history)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update tblUID Set uid_logid=@uid_logid, uid_adid=@uid_adid, uid_lob=@uid_lob, uid_empid=@uid_empid, " +
                    "uid_firstname=@uid_firstname, uid_lastname=@uid_lastname, uid_tickets=@uid_tickets, uid_history=@uid_history WHERE uid_uidref=@uid_uidref";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@uid_uidref", uid_uidref);
                cmdUpdate.Parameters.AddWithValue("@uid_logid", uid_logid);
                cmdUpdate.Parameters.AddWithValue("@uid_adid", uid_adid);
                cmdUpdate.Parameters.AddWithValue("@uid_lob", uid_lob);
                cmdUpdate.Parameters.AddWithValue("@uid_empid", uid_empid);
                cmdUpdate.Parameters.AddWithValue("@uid_firstname", uid_firstname);
                cmdUpdate.Parameters.AddWithValue("@uid_lastname", uid_lastname);
                cmdUpdate.Parameters.AddWithValue("@uid_tickets", uid_tickets);
                cmdUpdate.Parameters.AddWithValue("@uid_history", uid_history);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A Users Record Given Parameters for Columns
        public int UsersTableRecordUpdate(string iamhu_id,string iamhu_uid, string iamhu_logid, string iamhu_lob, string iamhu_name, string iamhu_adid, string iamhu_empid, string iamhu_comments, string iamhu_history)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update IAMHUsers Set iamhu_uid=@iamhu_uid, iamhu_logid=@iamhu_logid, iamhu_lob=@iamhu_lob, iamhu_name=@iamhu_name, " +
                    "iamhu_adid=@iamhu_adid, iamhu_empid=@iamhu_empid, iamhu_comments=@iamhu_comments, iamhu_history=@iamhu_history WHERE iamhu_id=@iamhu_id";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@iamhu_id", iamhu_id);
                cmdUpdate.Parameters.AddWithValue("@iamhu_uid", iamhu_uid);
                cmdUpdate.Parameters.AddWithValue("@iamhu_logid", iamhu_logid);
                cmdUpdate.Parameters.AddWithValue("@iamhu_lob", iamhu_lob);
                cmdUpdate.Parameters.AddWithValue("@iamhu_name", iamhu_name);
                cmdUpdate.Parameters.AddWithValue("@iamhu_adid", iamhu_adid);
                cmdUpdate.Parameters.AddWithValue("@iamhu_empid", iamhu_empid);
                cmdUpdate.Parameters.AddWithValue("@iamhu_comments", iamhu_comments);
                cmdUpdate.Parameters.AddWithValue("@iamhu_history", iamhu_history);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A FactoolConfigTable Record Given Parameters for Columns
        public int FactoolConfigUpdate(string table, int id, string name, string value, string displayorder, string modifiedby, string modified)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update "+ table + " Set Name=@name, Value=@value, DisplayOrder=@displayorder, ModifiedBy=@modifiedby, Modified=@modified WHERE Id=@id";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@id", id);
                cmdUpdate.Parameters.AddWithValue("@name", name);
                cmdUpdate.Parameters.AddWithValue("@value", value);
                cmdUpdate.Parameters.AddWithValue("@displayorder", displayorder);
                cmdUpdate.Parameters.AddWithValue("@modifiedby", modifiedby);
                cmdUpdate.Parameters.AddWithValue("@modified", modified);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A FactoolRequest Record 
        public int FacToolUpdateRequest(FacToolRequest request, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set ModifiedDate=@val1, ModifiedTick=@val2, SamAccount=@val3, DisplayName=@val4, NewRequest=@val5, TotalUsers=@val6, " +
                    "RequestStatus=@val7, FormType=@val8, RequestType=@val9, DefectReason=@val10, Systems=@val11, " +
                    "ReplyTypes=@val12, Comments=@val13, XREF1=@val14, XREF2=@val15, LOB=@val16 WHERE ReferenceNumber=@val17";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                if (request.SamAccount == null) { request.SamAccount = ""; }
                if (request.DisplayName == null) { request.DisplayName = ""; }
                if (request.ReferenceNumber == null) { request.ReferenceNumber = ""; }
                if (request.NewRequest == null) { request.NewRequest = ""; }
                if (request.TotalUsers == null) { request.TotalUsers = ""; }
                if (request.RequestStatus == null) { request.RequestStatus = ""; }
                if (request.FormType == null) { request.FormType = ""; }
                string joinedDefects = string.Join(",", request.DefectReason);
                string joinedSystems = string.Join(",", request.Systems);
                string joinedReplys = string.Join(",", request.ReplyTypes);
                string joinedRequests = string.Join(",", request.RequestType);
                if (request.Comments == null) { request.Comments = ""; }
                if (request.XREF1 == null) { request.XREF1 = ""; }
                if (request.XREF2 == null) { request.XREF2 = ""; }
                if (request.LOBType == null) { request.LOBType = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", request.ModifiedDate);
                cmdUpdate.Parameters.AddWithValue("@val2", request.ModifiedTick);
                cmdUpdate.Parameters.AddWithValue("@val3", request.SamAccount);
                cmdUpdate.Parameters.AddWithValue("@val4", request.DisplayName);
                cmdUpdate.Parameters.AddWithValue("@val5", request.NewRequest);
                cmdUpdate.Parameters.AddWithValue("@val6", request.TotalUsers);
                cmdUpdate.Parameters.AddWithValue("@val7", request.RequestStatus);
                cmdUpdate.Parameters.AddWithValue("@val8", request.FormType);
                cmdUpdate.Parameters.AddWithValue("@val9", joinedRequests);
                cmdUpdate.Parameters.AddWithValue("@val10", joinedDefects);
                cmdUpdate.Parameters.AddWithValue("@val11", joinedSystems);
                cmdUpdate.Parameters.AddWithValue("@val12", joinedReplys);
                cmdUpdate.Parameters.AddWithValue("@val13", request.Comments);
                cmdUpdate.Parameters.AddWithValue("@val14", request.XREF1);
                cmdUpdate.Parameters.AddWithValue("@val15", request.XREF2);
                cmdUpdate.Parameters.AddWithValue("@val16", request.LOBType);
                cmdUpdate.Parameters.AddWithValue("@val17", request.ReferenceNumber);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A FactoolRequest Status Record 
        public int FacToolUpdateRequestStatus(FacToolRequest request, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set SentStatus=@val2, RequestState=@val3, TouchPoints=@val4, TimesReturned=@val5, " +
                    "SLAStart=@val6, CompletionDate=@val7, SLACompletionTime=@val8, AgentComments=@val9, AgentsWorked=@val10 WHERE ReferenceNumber=@val1";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (request.SamAccount == null) { request.SamAccount = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", request.ReferenceNumber);
                cmdUpdate.Parameters.AddWithValue("@val2", request.SentStatus);
                cmdUpdate.Parameters.AddWithValue("@val3", request.RequestState);
                cmdUpdate.Parameters.AddWithValue("@val4", request.TouchPoints);
                cmdUpdate.Parameters.AddWithValue("@val5", request.TimesReturned);
                cmdUpdate.Parameters.AddWithValue("@val6", request.SLAStart);
                cmdUpdate.Parameters.AddWithValue("@val7", request.CompletionDate);
                cmdUpdate.Parameters.AddWithValue("@val8", request.SLACompletionTime);
                cmdUpdate.Parameters.AddWithValue("@val9", request.AgentComments);
                cmdUpdate.Parameters.AddWithValue("@val10", request.AgentsWorked);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A FactoolRequest History Record 
        public int FacToolUpdateRequestHistory(FactoolRequestHistory request, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set ReferenceNumber=@val2, DisplayName=@val3, DateModified=@val4, Changes=@val5 WHERE HistoryID=@val1";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (request.DisplayName == null) { request.DisplayName = ""; }
                if (request.DateModified == null) { request.DateModified = ""; }
                if (request.Changes == null) { request.Changes = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", request.HistoryID);
                cmdUpdate.Parameters.AddWithValue("@val2", request.ReferenceNumber);
                cmdUpdate.Parameters.AddWithValue("@val3", request.DisplayName);
                cmdUpdate.Parameters.AddWithValue("@val4", request.DateModified);
                cmdUpdate.Parameters.AddWithValue("@val5", request.Changes);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A FactoolRequestNumber Record 
        public int FacToolUpdateRequestNumber(FacToolRefNumber refNumber, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set ReferenceDate=@referenceDate, Sequence=@sequence, ReferenceSequence=@referenceSequence, " +
                    "ReferenceNumber=@referenceNumber, PreviousReferenceNumber=@previousReference WHERE Id=@id";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@id", refNumber._id);
                cmdUpdate.Parameters.AddWithValue("@referenceDate", refNumber.ReferenceDate);
                cmdUpdate.Parameters.AddWithValue("@sequence", refNumber.Sequence);
                cmdUpdate.Parameters.AddWithValue("@referenceSequence", refNumber.ReferenceSequence);
                cmdUpdate.Parameters.AddWithValue("@referenceNumber", refNumber.ReferenceNumber);
                cmdUpdate.Parameters.AddWithValue("@previousReference", refNumber.PreviousReferenceNumber);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A Service Catalog Record
        public int ServiceCatalogItemUpdate(ServiceCatalogItemModel passedModel, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set Services=@Services, SubSystem=@SubSystem, Description=@Description, RequestSource=@RequestSource, " +
                    "AssignmentGroup=@AssignmentGroup, ExpediteProcess=@ExpediteProcess, SLA=@SLA, ArcherAppID=@ArcherAppID, ITPMAppID=@ITPMAppID," +
                    " Hints=@Hints, Tasks=@Tasks, Domain=@Domain, Article=@Article, ProvisioningTeam=@ProvisioningTeam, URL=@URL, AutomationStatus=@AutomationStatus, Environment=@Environment WHERE ID=@ID";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (passedModel.Services == null) { passedModel.Services = ""; }
                if (passedModel.SubSystem == null) { passedModel.SubSystem = ""; }
                if (passedModel.Description == null) { passedModel.Description = ""; }
                if (passedModel.RequestSource == null) { passedModel.RequestSource = ""; }
                if (passedModel.AssignmentGroup == null) { passedModel.AssignmentGroup = ""; }
                if (passedModel.ExpediteProcess == null) { passedModel.ExpediteProcess = ""; }
                if (passedModel.SLA == null) { passedModel.SLA = ""; }
                if (passedModel.ArcherAppID == null) { passedModel.ArcherAppID = ""; }
                if (passedModel.ITPMAppID == null) { passedModel.ITPMAppID = ""; }
                if (passedModel.Hints == null) { passedModel.Hints = ""; }
                if (passedModel.Tasks == null) { passedModel.Tasks = ""; }
                if (passedModel.Domain == null) { passedModel.Domain = new(); }
                string joinedDomains = string.Join(";", passedModel.Domain);
                if (passedModel.Article == null) { passedModel.Article = ""; }
                if (passedModel.ProvisioningTeam == null) { passedModel.ProvisioningTeam = ""; }
                if (passedModel.URL == null) { passedModel.URL = ""; }
                if (passedModel.Environment == null) { passedModel.Environment = ""; }
                if (passedModel.AutomationStatus == null) { passedModel.AutomationStatus = "Manual"; }
                cmdUpdate.Parameters.AddWithValue("@ID", passedModel.ID);
                cmdUpdate.Parameters.AddWithValue("@Services", passedModel.Services);
                cmdUpdate.Parameters.AddWithValue("@SubSystem", passedModel.SubSystem);
                cmdUpdate.Parameters.AddWithValue("@Description", passedModel.Description);
                cmdUpdate.Parameters.AddWithValue("@RequestSource", passedModel.RequestSource);
                cmdUpdate.Parameters.AddWithValue("@AssignmentGroup", passedModel.AssignmentGroup);
                cmdUpdate.Parameters.AddWithValue("@ExpediteProcess", passedModel.ExpediteProcess);
                cmdUpdate.Parameters.AddWithValue("@SLA", passedModel.SLA);
                cmdUpdate.Parameters.AddWithValue("@ArcherAppID", passedModel.ArcherAppID);
                cmdUpdate.Parameters.AddWithValue("@ITPMAppID", passedModel.ITPMAppID);
                cmdUpdate.Parameters.AddWithValue("@Hints", passedModel.Hints);
                cmdUpdate.Parameters.AddWithValue("@Tasks", passedModel.Tasks);
                cmdUpdate.Parameters.AddWithValue("@Domain", joinedDomains);
                cmdUpdate.Parameters.AddWithValue("@Article", passedModel.Article);
                cmdUpdate.Parameters.AddWithValue("@ProvisioningTeam", passedModel.ProvisioningTeam);
                cmdUpdate.Parameters.AddWithValue("@URL", passedModel.URL);
                cmdUpdate.Parameters.AddWithValue("@Environment", passedModel.Environment);
                cmdUpdate.Parameters.AddWithValue("@AutomationStatus", passedModel.AutomationStatus);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates A Service Catalog Source Record 
        public int ServiceCatalogSourceUpdate(ServiceCatalogSourceItemModel passedModel, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set RequestSource=@RequestSource, URL=@URL WHERE Id=@Id";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (passedModel.RequestSource == null) { passedModel.RequestSource = ""; }
                if (passedModel.URL == null) { passedModel.URL = ""; }
                cmdUpdate.Parameters.AddWithValue("@Id", passedModel.Id);
                cmdUpdate.Parameters.AddWithValue("@RequestSource", passedModel.RequestSource);
                cmdUpdate.Parameters.AddWithValue("@URL", passedModel.URL);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates An Access Now Data Record 
        public int AccessNowDataUpdate(AccessNowDataItemModel passedRequest, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set requested_on=@val2, request_type=@val3, status=@val4, closed_on=@val5, asset=@val6, access=@val7, sub_request_type=@val8, account_name=@val9, requestor_name=@val10," +
                    " requestor_employee_id=@val11, ticket_id=@val12, ticket_status=@val13, mail_to=@val14, mail_sent_on=@val15, mail_sent_on_date=@val16 WHERE request_id=@val1";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                if (passedRequest.Requested_on == null) { passedRequest.Requested_on = ""; }
                if (passedRequest.Request_type == null) { passedRequest.Request_type = ""; }
                if (passedRequest.Status == null) { passedRequest.Status = ""; }
                if (passedRequest.Closed_on == null) { passedRequest.Closed_on = ""; }
                if (passedRequest.Asset == null) { passedRequest.Asset = ""; }
                if (passedRequest.Access == null) { passedRequest.Access = ""; }
                if (passedRequest.Sub_request_type == null) { passedRequest.Sub_request_type = ""; }
                if (passedRequest.Account_name == null) { passedRequest.Account_name = ""; }
                if (passedRequest.Requestor_name == null) { passedRequest.Requestor_name = ""; }
                if (passedRequest.Requestor_employee_id == null) { passedRequest.Requestor_employee_id = ""; }
                if (passedRequest.Ticket_id == null) { passedRequest.Ticket_id = ""; }
                if (passedRequest.Ticket_status == null) { passedRequest.Ticket_status = ""; }
                if (passedRequest.Mail_to == null) { passedRequest.Mail_to = ""; }
                if (passedRequest.Mail_sent_on == null) { passedRequest.Mail_sent_on = ""; }
                if (passedRequest.Mail_sent_on_date == null) { passedRequest.Mail_sent_on_date = ""; }

                cmdUpdate.Parameters.AddWithValue("@val1", passedRequest.Request_id);
                cmdUpdate.Parameters.AddWithValue("@val2", passedRequest.Requested_on);
                cmdUpdate.Parameters.AddWithValue("@val3", passedRequest.Request_type);
                cmdUpdate.Parameters.AddWithValue("@val4", passedRequest.Status);
                cmdUpdate.Parameters.AddWithValue("@val5", passedRequest.Closed_on);
                cmdUpdate.Parameters.AddWithValue("@val6", passedRequest.Asset);
                cmdUpdate.Parameters.AddWithValue("@val7", passedRequest.Access);
                cmdUpdate.Parameters.AddWithValue("@val8", passedRequest.Sub_request_type);
                cmdUpdate.Parameters.AddWithValue("@val9", passedRequest.Account_name);
                cmdUpdate.Parameters.AddWithValue("@val10", passedRequest.Requestor_name);
                cmdUpdate.Parameters.AddWithValue("@val11", passedRequest.Requestor_employee_id);
                cmdUpdate.Parameters.AddWithValue("@val12", passedRequest.Ticket_id);
                cmdUpdate.Parameters.AddWithValue("@val13", passedRequest.Ticket_status);
                cmdUpdate.Parameters.AddWithValue("@val14", passedRequest.Mail_to);
                cmdUpdate.Parameters.AddWithValue("@val15", passedRequest.Mail_sent_on);
                cmdUpdate.Parameters.AddWithValue("@val16", passedRequest.Mail_sent_on_date);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates An Access Now Data Record in Bulk
        public int AccessNowDataUpdateBulk(List<AccessNowDataItemModel> passedRequestList, string table)
        {
            try
            {
                ConnectToDatabase();
                int returnValue = -1;
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = "CREATE TABLE #TmpTableHeimdall(request_id float not null,requested_on varchar(MAX) null,request_type varchar(MAX) null,status varchar(MAX) null, " +
                    "closed_on varchar(MAX) null,asset varchar(MAX) null,access varchar(MAX) null, sub_request_type varchar(MAX) null,account_name varchar(MAX) null,requestor_name varchar(MAX) null," +
                    " requestor_employee_id varchar(MAX) null,ticket_id varchar(MAX) null,ticket_status varchar(MAX) null,mail_to varchar(MAX) null,mail_sent_on varchar(MAX) null,mail_sent_on_date varchar(MAX) null)";
                cmdUpdate.ExecuteNonQuery();

                SqlBulkCopy objbulk = new(conn);
                objbulk.DestinationTableName = "#TmpTableHeimdall";

                DataTable tbl = new DataTable();
                tbl.Columns.Add(new DataColumn("request_id", typeof(long)));
                tbl.Columns.Add(new DataColumn("requested_on", typeof(string)));
                tbl.Columns.Add(new DataColumn("request_type", typeof(string)));
                tbl.Columns.Add(new DataColumn("status", typeof(string)));
                tbl.Columns.Add(new DataColumn("closed_on", typeof(string)));
                tbl.Columns.Add(new DataColumn("asset", typeof(string)));
                tbl.Columns.Add(new DataColumn("access", typeof(string)));
                tbl.Columns.Add(new DataColumn("sub_request_type", typeof(string)));
                tbl.Columns.Add(new DataColumn("account_name", typeof(string)));
                tbl.Columns.Add(new DataColumn("requestor_name", typeof(string)));
                tbl.Columns.Add(new DataColumn("requestor_employee_id", typeof(string)));
                tbl.Columns.Add(new DataColumn("ticket_id", typeof(string)));
                tbl.Columns.Add(new DataColumn("ticket_status", typeof(string)));
                tbl.Columns.Add(new DataColumn("mail_to", typeof(string)));
                tbl.Columns.Add(new DataColumn("mail_sent_on", typeof(string)));
                tbl.Columns.Add(new DataColumn("mail_sent_on_date", typeof(string)));

                foreach (AccessNowDataItemModel currentItem in passedRequestList)
                {
                    DataRow dr = tbl.NewRow();
                    dr["request_id"] = currentItem.Request_id;
                    dr["requested_on"] = currentItem.Requested_on;
                    dr["request_type"] = currentItem.Request_type;
                    dr["status"] = currentItem.Status;
                    dr["closed_on"] = currentItem.Closed_on;
                    dr["asset"] = currentItem.Asset;
                    dr["access"] = currentItem.Access;
                    dr["sub_request_type"] = currentItem.Sub_request_type;
                    dr["account_name"] = currentItem.Account_name;
                    dr["requestor_name"] = currentItem.Requestor_name;
                    dr["requestor_employee_id"] = currentItem.Requestor_employee_id;
                    dr["ticket_id"] = currentItem.Ticket_id;
                    dr["ticket_status"] = currentItem.Ticket_status;
                    dr["mail_to"] = currentItem.Mail_to;
                    dr["mail_sent_on"] = currentItem.Mail_sent_on;
                    dr["mail_sent_on_date"] = currentItem.Mail_sent_on_date;
                    tbl.Rows.Add(dr);
                }


                objbulk.ColumnMappings.Add("request_id", "request_id");
                objbulk.ColumnMappings.Add("requested_on", "requested_on");
                objbulk.ColumnMappings.Add("request_type", "request_type");
                objbulk.ColumnMappings.Add("status", "status");
                objbulk.ColumnMappings.Add("closed_on", "closed_on");
                objbulk.ColumnMappings.Add("asset", "asset");
                objbulk.ColumnMappings.Add("access", "access");
                objbulk.ColumnMappings.Add("sub_request_type", "sub_request_type");
                objbulk.ColumnMappings.Add("account_name", "account_name");
                objbulk.ColumnMappings.Add("requestor_name", "requestor_name");
                objbulk.ColumnMappings.Add("requestor_employee_id", "requestor_employee_id");
                objbulk.ColumnMappings.Add("ticket_id", "ticket_id");
                objbulk.ColumnMappings.Add("ticket_status", "ticket_status");
                objbulk.ColumnMappings.Add("mail_to", "mail_to");
                objbulk.ColumnMappings.Add("mail_sent_on", "mail_sent_on");
                objbulk.ColumnMappings.Add("mail_sent_on_date", "mail_sent_on_date");

                objbulk.WriteToServer(tbl);
                objbulk.Close();

                string strCommand = "UPDATE P SET P.requested_on=T.requested_on, P.request_type=T.request_type, P.status=T.status, P.closed_on=T.closed_on, P.asset=T.asset, P.access=T.access, P.sub_request_type=T.sub_request_type," +
                    " P.account_name=T.account_name, P.requestor_name=T.requestor_name, P.requestor_employee_id=T.requestor_employee_id, P.ticket_id=T.ticket_id, P.ticket_status=T.ticket_status, P.mail_to=T.mail_to, P.mail_sent_on=T.mail_sent_on, P.mail_sent_on_date=T.mail_sent_on_date " +
                    "FROM " + table + " AS P INNER JOIN #TmpTableHeimdall AS T ON P.request_id = T.request_id ;DROP TABLE #TmpTableHeimdall;";

                cmdUpdate.CommandTimeout = 300;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.ExecuteNonQuery();

                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        // Updates AccessNow Operations Record
        public void UpdateAccessNowOperationsRecord(AccessNowRequestModel.SQLObject AccessNowObject, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set " +
                    "requested_on=@requested_on," +
                    "request_type=@request_type," +
                    "status=@status," +
                    "closed_on=@closed_on," +
                    "asset=@asset," +
                    "access=@access," +
                    "sub_request_type=@sub_request_type," +
                    "account_name=@account_name," +
                    "requestor_name=@requestor_name," +
                    "requestor_employee_id=@requestor_employee_id," +
                    "ticket_id=@ticket_id," +
                    "ticket_status=@ticket_status," +
                    "mail_to=@mail_to," +
                    "mail_sent_on=@mail_sent_on," +
                    "mail_sent_on_date=@mail_sent_on_date," +
                    "TicketCount=@TicketCount," +
                    "Category=@Category," +
                    "Type=@Type," +
                    "SLADays=@SLADays " +
                    "WHERE request_id=@request_id";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                cmdUpdate.Parameters.AddWithValue("@request_id", AccessNowObject.request_id);
                cmdUpdate.Parameters.AddWithValue("@requested_on", AccessNowObject.requested_on);
                cmdUpdate.Parameters.AddWithValue("@request_type", AccessNowObject.request_type);
                cmdUpdate.Parameters.AddWithValue("@status", AccessNowObject.status);
                cmdUpdate.Parameters.AddWithValue("@closed_on", AccessNowObject.closed_on);
                cmdUpdate.Parameters.AddWithValue("@asset", AccessNowObject.asset);
                cmdUpdate.Parameters.AddWithValue("@access", AccessNowObject.access);
                cmdUpdate.Parameters.AddWithValue("@sub_request_type", AccessNowObject.sub_request_type);
                cmdUpdate.Parameters.AddWithValue("@account_name", AccessNowObject.account_name);
                cmdUpdate.Parameters.AddWithValue("@requestor_name", AccessNowObject.requestor_name);
                cmdUpdate.Parameters.AddWithValue("@requestor_employee_id", AccessNowObject.requestor_employee_id);
                cmdUpdate.Parameters.AddWithValue("@ticket_id", AccessNowObject.ticket_id);
                cmdUpdate.Parameters.AddWithValue("@ticket_status", AccessNowObject.ticket_status);
                cmdUpdate.Parameters.AddWithValue("@mail_to", AccessNowObject.mail_to);
                cmdUpdate.Parameters.AddWithValue("@mail_sent_on", AccessNowObject.mail_sent_on);
                cmdUpdate.Parameters.AddWithValue("@mail_sent_on_date", AccessNowObject.mail_sent_on_date);
                cmdUpdate.Parameters.AddWithValue("@TicketCount", AccessNowObject.TicketCount);
                cmdUpdate.Parameters.AddWithValue("@Category", AccessNowObject.Category);
                cmdUpdate.Parameters.AddWithValue("@Type", AccessNowObject.Type);
                cmdUpdate.Parameters.AddWithValue("@SLADays", AccessNowObject.SLADays);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();

                Disconnect();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }


        // Updates A Factool Expected Record 
        public int FactoolExpectedTableRecordUpdate(FactoolExpectedModel passedModel, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "Update " + table + " Set ClassName=@ClassName, N_Number=@N_Number, PERSONA=@PERSONA, ExpectedUsers=@ExpectedUsers," +
                    "ExpectedApps=@ExpectedApps, ExpectedTouchpoints=@ExpectedTouchpoints, StartDate=@StartDate, ClassOwner=@ClassOwner WHERE _iD=@_iD";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                if (passedModel.ClassName == null) { passedModel.ClassName = ""; }
                if (passedModel.ClassOwner == null) { passedModel.ClassOwner = ""; }
                if (passedModel.N_Number == null) { passedModel.N_Number = ""; }
                if (passedModel.PERSONA == null) { passedModel.PERSONA = ""; }
                if (passedModel.ExpectedUsers == null) { passedModel.ExpectedUsers = ""; }
                if (passedModel.ExpectedApps == null) { passedModel.ExpectedApps = ""; }
                if (passedModel.ExpectedTouchpoints == null) { passedModel.ExpectedTouchpoints = ""; }
                if (passedModel.StartDate == null) { passedModel.StartDate = ""; }

                cmdUpdate.Parameters.AddWithValue("@_iD", passedModel._iD);
                cmdUpdate.Parameters.AddWithValue("@ClassName", passedModel.ClassName);
                cmdUpdate.Parameters.AddWithValue("@N_Number", passedModel.N_Number);
                cmdUpdate.Parameters.AddWithValue("@PERSONA", passedModel.PERSONA);
                cmdUpdate.Parameters.AddWithValue("@ExpectedUsers", passedModel.ExpectedUsers);
                cmdUpdate.Parameters.AddWithValue("@ExpectedApps", passedModel.ExpectedApps);
                cmdUpdate.Parameters.AddWithValue("@ExpectedTouchpoints", passedModel.ExpectedTouchpoints);
                cmdUpdate.Parameters.AddWithValue("@StartDate", passedModel.StartDate);
                cmdUpdate.Parameters.AddWithValue("@ClassOwner", passedModel.ClassOwner);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }
        #endregion

        #region SQL Insert Functions
        //Adds a record to UID Table given a list of parameters for new record
        public int AddUIDRecord(string uid_uidref, string uid_logid, string uid_adid, string uid_lob, string uid_empid, string uid_firstname, string uid_lastname, string uid_tickets, string uid_history, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO " + table + " (uid_uidref, uid_logid, uid_adid, uid_lob, uid_empid, uid_firstname, uid_lastname, uid_tickets, uid_history ) VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", uid_uidref);
                cmdUpdate.Parameters.AddWithValue("@val2", uid_logid);
                cmdUpdate.Parameters.AddWithValue("@val3", uid_adid);
                cmdUpdate.Parameters.AddWithValue("@val4", uid_lob);
                cmdUpdate.Parameters.AddWithValue("@val5", uid_empid);
                cmdUpdate.Parameters.AddWithValue("@val6", uid_firstname);
                cmdUpdate.Parameters.AddWithValue("@val7", uid_lastname);
                cmdUpdate.Parameters.AddWithValue("@val8", uid_tickets);
                cmdUpdate.Parameters.AddWithValue("@val9", uid_history);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();

                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a record to Feedback table given a list of parameters for new record
        public int AddFeedbackRecord(double iamf_reqid, string iamf_requestor, string iamf_requesttype, string iamf_requestdate, string iamf_updatedate, string iamf_request, string iamf_requestupdate, string iamf_requeststatus)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO IAMHFeedback (iamf_reqid, iamf_requestor, iamf_requesttype, iamf_requestdate, iamf_updatedate, iamf_request, iamf_requestupdate, iamf_requeststatus) VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", iamf_reqid);
                cmdUpdate.Parameters.AddWithValue("@val2", iamf_requestor);
                cmdUpdate.Parameters.AddWithValue("@val3", iamf_requesttype);
                cmdUpdate.Parameters.AddWithValue("@val4", iamf_requestdate);
                cmdUpdate.Parameters.AddWithValue("@val5", iamf_updatedate);
                cmdUpdate.Parameters.AddWithValue("@val6", iamf_request);
                cmdUpdate.Parameters.AddWithValue("@val7", iamf_requestupdate);
                cmdUpdate.Parameters.AddWithValue("@val8", iamf_requeststatus);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a record to Retail Group table given a list of parameters for new record
        public int AddRetailGroupRecord(string iamhrg_gid, string iamhrg_group, string iamhrg_lob, string iamhrg_history)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO IAMHRg (iamhrg_gid, iamhrg_group, iamhrg_lob, iamhrg_history) VALUES (@val1,@val2,@val3,@val4)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", iamhrg_gid);
                cmdUpdate.Parameters.AddWithValue("@val2", iamhrg_group);
                cmdUpdate.Parameters.AddWithValue("@val3", iamhrg_lob);
                cmdUpdate.Parameters.AddWithValue("@val4", iamhrg_history);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Record added succesfully.", "Entry Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a record to Retail Group table given a list of parameters for new record
        public int AddKerberosGroupRecord(string iamhkg_gid, string iamhkg_group, string iamhkg_comments, string iamhkg_servers, string iamhkg_history)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO IAMHKGroups (iamhkg_gid, iamhkg_group, iamhkg_comments, iamhkg_servers, iamhkg_history) VALUES (@val1,@val2,@val3,@val4,@val5)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", iamhkg_gid);
                cmdUpdate.Parameters.AddWithValue("@val2", iamhkg_group);
                cmdUpdate.Parameters.AddWithValue("@val3", iamhkg_comments);
                cmdUpdate.Parameters.AddWithValue("@val4", iamhkg_servers);
                cmdUpdate.Parameters.AddWithValue("@val5", iamhkg_history);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Record added succesfully.", "Entry Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a record to Retail Group table given a list of parameters for new record
        public int AddKerberosUserRecord(string iamhku_uid, string iamhku_logid, string iamhku_empid, string iamhku_adid, string iamhku_name, string iamhku_comments, string iamhku_servers, string iamhku_history)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO IAMHKUsers (iamhku_uid, iamhku_logid, iamhku_empid, iamhku_adid, iamhku_name, iamhku_comments, iamhku_servers, iamhku_history) VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", iamhku_uid);
                cmdUpdate.Parameters.AddWithValue("@val2", iamhku_logid);
                cmdUpdate.Parameters.AddWithValue("@val3", iamhku_empid);
                cmdUpdate.Parameters.AddWithValue("@val4", iamhku_adid);
                cmdUpdate.Parameters.AddWithValue("@val5", iamhku_name);
                cmdUpdate.Parameters.AddWithValue("@val6", iamhku_comments);
                cmdUpdate.Parameters.AddWithValue("@val7", iamhku_servers);
                cmdUpdate.Parameters.AddWithValue("@val8", iamhku_history);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Record added succesfully.", "Entry Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a record to Retail Group table given a list of parameters for new record
        public int AddGroupsRecord(string iamhg_gid, string iamhg_group, string iamhg_lob, string iamhg_history)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO IAMHGroups (iamhg_gid, iamhg_group, iamhg_lob, iamhg_history) VALUES (@val1,@val2,@val3,@val4)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", iamhg_gid);
                cmdUpdate.Parameters.AddWithValue("@val2", iamhg_group);
                cmdUpdate.Parameters.AddWithValue("@val3", iamhg_lob);
                cmdUpdate.Parameters.AddWithValue("@val4", iamhg_history);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Record added succesfully.", "Entry Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a record to Retail Group table given a list of parameters for new record
        public int AddUsersRecord(string iamhu_uid, string iamhu_logid, string iamhu_lob, string iamhu_name, string iamhu_adid, string iamhu_empid, string iamhu_comments, string iamhu_history, string iamhu_queue)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO IAMHUsers (iamhu_uid, iamhu_logid, iamhu_lob, iamhu_name, iamhu_adid, iamhu_empid, iamhu_comments, iamhu_history, iamhu_queue) VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", iamhu_uid);
                cmdUpdate.Parameters.AddWithValue("@val2", iamhu_logid);
                cmdUpdate.Parameters.AddWithValue("@val3", iamhu_lob);
                cmdUpdate.Parameters.AddWithValue("@val4", iamhu_name);
                cmdUpdate.Parameters.AddWithValue("@val5", iamhu_adid);
                cmdUpdate.Parameters.AddWithValue("@val6", iamhu_empid);
                cmdUpdate.Parameters.AddWithValue("@val7", iamhu_comments);
                cmdUpdate.Parameters.AddWithValue("@val8", iamhu_history);
                cmdUpdate.Parameters.AddWithValue("@val9", iamhu_queue);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Record added succesfully.", "Entry Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a record to Retail User table given a list of parameters for new record
        public int AddRetailUserRecord(string iamhru_uid, string iamhru_unixid, string iamhru_comments, string iamhru_eid, string iamhru_lob, string iamhru_history)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO IAMHRu (iamhru_uid, iamhru_unixid, iamhru_comments, iamhru_eid, iamhru_lob, iamhru_history) VALUES (@val1,@val2,@val3,@val4,@val5,@val6)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", iamhru_uid);
                cmdUpdate.Parameters.AddWithValue("@val2", iamhru_unixid);
                cmdUpdate.Parameters.AddWithValue("@val3", iamhru_comments);
                cmdUpdate.Parameters.AddWithValue("@val4", iamhru_eid);
                cmdUpdate.Parameters.AddWithValue("@val5", iamhru_lob);
                cmdUpdate.Parameters.AddWithValue("@val6", iamhru_history);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                MessageBox.Show("Record added succesfully.", "Entry Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the FactoolRequest Table
        public int AddFactoolRequest(FacToolRequest request, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (CreateDate, CreateTick, ModifiedDate, ModifiedTick, SamAccount, DisplayName, ReferenceNumber, NewRequest, TotalUsers, RequestStatus, FormType, RequestType," +
                    "DefectReason, Systems, ReplyTypes, Comments, XREF1, XREF2, LOB) " +
                    "VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9,@val10,@val11,@val12,@val13,@val14,@val15,@val16,@val17,@val18,@val19)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (request.CreateDate == null) { request.CreateDate = ""; }
                if (request.CreateTick == null) { request.CreateTick = ""; }
                if (request.ModifiedDate == null) { request.ModifiedDate = ""; }
                if (request.ModifiedTick == null) { request.ModifiedTick = ""; }
                if (request.SamAccount == null) { request.SamAccount = ""; }
                if (request.DisplayName == null) { request.DisplayName = ""; }
                if (request.ReferenceNumber == null) { request.ReferenceNumber = ""; }
                if (request.NewRequest == null) { request.NewRequest = ""; }
                if (request.TotalUsers == null) { request.TotalUsers = ""; }
                if (request.RequestStatus == null) { request.RequestStatus = ""; }
                if (request.FormType == null) { request.FormType = ""; }
                string joinedDefects = string.Join(",", request.DefectReason);
                string joinedSystems = string.Join(",", request.Systems);
                string joinedReplys = string.Join(",", request.ReplyTypes);
                string joinedRequests = string.Join(",", request.RequestType);
                if (request.Comments == null) { request.Comments = ""; }
                if (request.XREF1 == null) { request.XREF1 = ""; }
                if (request.XREF2 == null) { request.XREF2 = ""; }
                if (request.LOBType == null) { request.LOBType = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", request.CreateDate);
                cmdUpdate.Parameters.AddWithValue("@val2", request.CreateTick);
                cmdUpdate.Parameters.AddWithValue("@val3", request.ModifiedDate);
                cmdUpdate.Parameters.AddWithValue("@val4", request.ModifiedTick);
                cmdUpdate.Parameters.AddWithValue("@val5", request.SamAccount);
                cmdUpdate.Parameters.AddWithValue("@val6", request.DisplayName);
                cmdUpdate.Parameters.AddWithValue("@val7", request.ReferenceNumber);
                cmdUpdate.Parameters.AddWithValue("@val8", request.NewRequest);
                cmdUpdate.Parameters.AddWithValue("@val9", request.TotalUsers);
                cmdUpdate.Parameters.AddWithValue("@val10", request.RequestStatus);
                cmdUpdate.Parameters.AddWithValue("@val11", request.FormType);
                cmdUpdate.Parameters.AddWithValue("@val12", joinedRequests);
                cmdUpdate.Parameters.AddWithValue("@val13", joinedDefects);
                cmdUpdate.Parameters.AddWithValue("@val14", joinedSystems);
                cmdUpdate.Parameters.AddWithValue("@val15", joinedReplys);
                cmdUpdate.Parameters.AddWithValue("@val16", request.Comments);
                cmdUpdate.Parameters.AddWithValue("@val17", request.XREF1);
                cmdUpdate.Parameters.AddWithValue("@val18", request.XREF2);
                cmdUpdate.Parameters.AddWithValue("@val19", request.LOBType);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the Service Catalog Table
        public int AddServiceCatalogRecord(ServiceCatalogItemModel passedModel, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (Services, SubSystem, Description, RequestSource, AssignmentGroup, ExpediteProcess, SLA, ArcherAppID, ITPMAppID, Hints, Tasks, Domain, Article, ProvisioningTeam, URL, AutomationStatus, Environment, ID) " +
                    "VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9,@val10,@val11,@val12,@val13,@val14,@val15,@val16,@val17,@val18)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (passedModel.Services == null) { passedModel.Services = ""; }
                if (passedModel.SubSystem == null) { passedModel.SubSystem = ""; }
                if (passedModel.Description == null) { passedModel.Description = ""; }
                if (passedModel.RequestSource == null) { passedModel.RequestSource = ""; }
                if (passedModel.AssignmentGroup == null) { passedModel.AssignmentGroup = ""; }
                if (passedModel.ExpediteProcess == null) { passedModel.ExpediteProcess = ""; }
                if (passedModel.SLA == null) { passedModel.SLA = ""; }
                if (passedModel.ArcherAppID == null) { passedModel.ArcherAppID = ""; }
                if (passedModel.ITPMAppID == null) { passedModel.ITPMAppID = ""; }
                if (passedModel.Hints == null) { passedModel.Hints = ""; }
                if (passedModel.Tasks == null) { passedModel.Tasks = ""; }
                if (passedModel.Domain == null) { passedModel.Domain = new(); }
                string joinedDomains = string.Join(";", passedModel.Domain);
                if (passedModel.Article == null) { passedModel.Article = ""; }
                if (passedModel.ProvisioningTeam == null) { passedModel.ProvisioningTeam = ""; }
                if (passedModel.URL == null) { passedModel.URL = ""; }
                if (passedModel.Environment == null) { passedModel.Environment = ""; }
                if (passedModel.AutomationStatus == null) { passedModel.AutomationStatus = "Manual"; }
                cmdUpdate.Parameters.AddWithValue("@val1", passedModel.Services);
                cmdUpdate.Parameters.AddWithValue("@val2", passedModel.SubSystem);
                cmdUpdate.Parameters.AddWithValue("@val3", passedModel.Description);
                cmdUpdate.Parameters.AddWithValue("@val4", passedModel.RequestSource);
                cmdUpdate.Parameters.AddWithValue("@val5", passedModel.AssignmentGroup);
                cmdUpdate.Parameters.AddWithValue("@val6", passedModel.ExpediteProcess);
                cmdUpdate.Parameters.AddWithValue("@val7", passedModel.SLA);
                cmdUpdate.Parameters.AddWithValue("@val8", passedModel.ArcherAppID);
                cmdUpdate.Parameters.AddWithValue("@val9", passedModel.ITPMAppID);
                cmdUpdate.Parameters.AddWithValue("@val10", passedModel.Hints);
                cmdUpdate.Parameters.AddWithValue("@val11", passedModel.Tasks);
                cmdUpdate.Parameters.AddWithValue("@val12", joinedDomains);
                cmdUpdate.Parameters.AddWithValue("@val13", passedModel.Article);
                cmdUpdate.Parameters.AddWithValue("@val14", passedModel.ProvisioningTeam);
                cmdUpdate.Parameters.AddWithValue("@val15", passedModel.URL);
                cmdUpdate.Parameters.AddWithValue("@val16", passedModel.AutomationStatus);
                cmdUpdate.Parameters.AddWithValue("@val17", passedModel.Environment);
                cmdUpdate.Parameters.AddWithValue("@val18", passedModel.ID);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the Factool Expected Table
        public int AddFactoolExpectedRecord(FactoolExpectedModel passedModel, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (_iD, ClassName, StartDate, N_Number, PERSONA, ExpectedUsers, ExpectedApps, ExpectedTouchpoints, ClassOwner) " +
                    "VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                if (passedModel.ClassName == null) { passedModel.ClassName = ""; }
                if (passedModel.StartDate == null) { passedModel.StartDate = ""; }
                if (passedModel.N_Number == null) { passedModel.N_Number = ""; }
                if (passedModel.PERSONA == null) { passedModel.PERSONA = ""; }
                if (passedModel.ExpectedUsers == null) { passedModel.ExpectedUsers = ""; }
                if (passedModel.ExpectedApps == null) { passedModel.ExpectedApps = ""; }
                if (passedModel.ExpectedTouchpoints == null) { passedModel.ExpectedTouchpoints = ""; }
                if (passedModel.ClassOwner == null) { passedModel.ClassOwner = ""; }

                cmdUpdate.Parameters.AddWithValue("@val1", passedModel._iD);
                cmdUpdate.Parameters.AddWithValue("@val2", passedModel.ClassName);
                cmdUpdate.Parameters.AddWithValue("@val3", passedModel.StartDate);
                cmdUpdate.Parameters.AddWithValue("@val4", passedModel.N_Number);
                cmdUpdate.Parameters.AddWithValue("@val5", passedModel.PERSONA);
                cmdUpdate.Parameters.AddWithValue("@val6", passedModel.ExpectedUsers);
                cmdUpdate.Parameters.AddWithValue("@val7", passedModel.ExpectedApps);
                cmdUpdate.Parameters.AddWithValue("@val8", passedModel.ExpectedTouchpoints);
                cmdUpdate.Parameters.AddWithValue("@val9", passedModel.ClassOwner);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the Service Catalog Sources Table
        public int AddServiceCatalogSourceRecord(ServiceCatalogSourceItemModel passedModel, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (RequestSource, URL) " +
                    "VALUES (@val1,@val2)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (passedModel.RequestSource == null) { passedModel.RequestSource = ""; }
                if (passedModel.URL == null) { passedModel.URL = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", passedModel.RequestSource);
                cmdUpdate.Parameters.AddWithValue("@val2", passedModel.URL);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the Service Catalog Groups Table
        public int AddServiceCatalogGroupRecord(string passedGroup, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (GroupName) " +
                    "VALUES (@val1)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (passedGroup == null) { passedGroup = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", passedGroup);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the Service Catalog Groups Table
        public int AddServiceCatalogEnvironmentRecord(string passedEnvironment, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (EnvironmentName) " +
                    "VALUES (@val1)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (passedEnvironment == null) { passedEnvironment = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", passedEnvironment);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the FactoolRequest Status Table
        public int AddFactoolRequestStatus(FacToolRequest request, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (ReferenceNumber, SentStatus, RequestState, TouchPoints, TimesReturned, SLAStart, CompletionDate, SLACompletionTime, AgentComments, AgentsWorked) " +
                    "VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9,@val10)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (request.CreateDate == null) { DateTime createDate = DateTime.UtcNow; request.CreateDate = createDate.ToString("yyyy/MM/dd h:mm:ss.fff"); }
                if (request.SentStatus == null) { request.SentStatus = "Received"; }
                if (request.RequestState == null) { request.RequestState = "Pre-Approval"; }
                if (request.TouchPoints == null) { request.TouchPoints = "0"; }
                if (request.TimesReturned == null) { request.TimesReturned = "0"; }
                if (request.SLAStart == null) { request.SLAStart = "Pre-Approval"; }
                if (request.CompletionDate == null) { request.CompletionDate = "Incomplete"; }
                if (request.SLACompletionTime == null) { request.SLACompletionTime = "Incomplete"; }
                if (request.AgentComments == null) { request.AgentComments = ""; }
                if (request.AgentsWorked == null) { request.AgentsWorked = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", request.ReferenceNumber);
                cmdUpdate.Parameters.AddWithValue("@val2", request.SentStatus);
                cmdUpdate.Parameters.AddWithValue("@val3", request.RequestState);
                cmdUpdate.Parameters.AddWithValue("@val4", request.TouchPoints);
                cmdUpdate.Parameters.AddWithValue("@val5", request.TimesReturned);
                cmdUpdate.Parameters.AddWithValue("@val6", request.SLAStart);
                cmdUpdate.Parameters.AddWithValue("@val7", request.CompletionDate);
                cmdUpdate.Parameters.AddWithValue("@val8", request.SLACompletionTime);
                cmdUpdate.Parameters.AddWithValue("@val9", request.AgentComments);
                cmdUpdate.Parameters.AddWithValue("@val10", request.AgentsWorked);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the FactoolRequest History Table
        public int AddFactoolRequestHistory(FactoolRequestHistory request, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (ReferenceNumber, DisplayName, ModifiedTick, DateModified, State, Changes) " +
                    "VALUES (@val1,@val2,@val3,@val4,@val5,@val6)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                if (request.DateModified == null) { DateTime createDate = DateTime.UtcNow; request.DateModified = createDate.ToString("yyyy/MM/dd h:mm:ss.fff"); }
                if (request.DisplayName == null) { UserPrincipal userPrincipal = UserPrincipal.Current; request.DisplayName = userPrincipal.Name; }
                if (request.Changes == null) { request.Changes = ""; }
                cmdUpdate.Parameters.AddWithValue("@val1", request.ReferenceNumber);
                cmdUpdate.Parameters.AddWithValue("@val2", request.DisplayName);
                cmdUpdate.Parameters.AddWithValue("@val3", request.ModifiedTick);
                cmdUpdate.Parameters.AddWithValue("@val4", request.DateModified);
                cmdUpdate.Parameters.AddWithValue("@val5", request.State);
                cmdUpdate.Parameters.AddWithValue("@val6", request.Changes);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the Access Now Data Table
        public int AddAccessNowDataRecord(AccessNowDataItemModel passedRequest, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (request_id, requested_on, request_type, status, closed_on, asset, access, sub_request_type, account_name, requestor_name, requestor_employee_id, ticket_id, ticket_status, mail_to, mail_sent_on, mail_sent_on_date) " +
                    "VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9,@val10,@val11,@val12,@val13,@val14,@val15,@val16)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                if (passedRequest.Requested_on == null) { passedRequest.Requested_on = ""; }
                if (passedRequest.Request_type == null) { passedRequest.Request_type = ""; }
                if (passedRequest.Status == null) { passedRequest.Status = ""; }
                if (passedRequest.Closed_on == null) { passedRequest.Closed_on = ""; }
                if (passedRequest.Asset == null) { passedRequest.Asset = ""; }
                if (passedRequest.Access == null) { passedRequest.Access = ""; }
                if (passedRequest.Sub_request_type == null) { passedRequest.Sub_request_type = ""; }
                if (passedRequest.Account_name == null) { passedRequest.Account_name = ""; }
                if (passedRequest.Requestor_name == null) { passedRequest.Requestor_name = ""; }
                if (passedRequest.Requestor_employee_id == null) { passedRequest.Requestor_employee_id = ""; }
                if (passedRequest.Ticket_id == null) { passedRequest.Ticket_id = ""; }
                if (passedRequest.Ticket_status == null) { passedRequest.Ticket_status = ""; }
                if (passedRequest.Mail_to == null) { passedRequest.Mail_to = ""; }
                if (passedRequest.Mail_sent_on == null) { passedRequest.Mail_sent_on = ""; }
                if (passedRequest.Mail_sent_on_date == null) { passedRequest.Mail_sent_on_date = ""; }

                cmdUpdate.Parameters.AddWithValue("@val1", passedRequest.Request_id);
                cmdUpdate.Parameters.AddWithValue("@val2", passedRequest.Requested_on);
                cmdUpdate.Parameters.AddWithValue("@val3", passedRequest.Request_type);
                cmdUpdate.Parameters.AddWithValue("@val4", passedRequest.Status);
                cmdUpdate.Parameters.AddWithValue("@val5", passedRequest.Closed_on);
                cmdUpdate.Parameters.AddWithValue("@val6", passedRequest.Asset);
                cmdUpdate.Parameters.AddWithValue("@val7", passedRequest.Access);
                cmdUpdate.Parameters.AddWithValue("@val8", passedRequest.Sub_request_type);
                cmdUpdate.Parameters.AddWithValue("@val9", passedRequest.Account_name);
                cmdUpdate.Parameters.AddWithValue("@val10", passedRequest.Requestor_name);
                cmdUpdate.Parameters.AddWithValue("@val11", passedRequest.Requestor_employee_id);
                cmdUpdate.Parameters.AddWithValue("@val12", passedRequest.Ticket_id);
                cmdUpdate.Parameters.AddWithValue("@val13", passedRequest.Ticket_status);
                cmdUpdate.Parameters.AddWithValue("@val14", passedRequest.Mail_to);
                cmdUpdate.Parameters.AddWithValue("@val15", passedRequest.Mail_sent_on);
                cmdUpdate.Parameters.AddWithValue("@val16", passedRequest.Mail_sent_on_date);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the Access Now Data Table in Bulk
        public int AddAccessNowDataRecordBulk(List<AccessNowDataItemModel> passedRequestList, string table)
        {
            try
            {
                ConnectToDatabase();
                int returnValue = -1;

                SqlBulkCopy objbulk = new(conn);
                objbulk.DestinationTableName = table;

                DataTable tbl = new DataTable();
                tbl.Columns.Add(new DataColumn("request_id", typeof(long)));
                tbl.Columns.Add(new DataColumn("requested_on", typeof(string)));
                tbl.Columns.Add(new DataColumn("request_type", typeof(string)));
                tbl.Columns.Add(new DataColumn("status", typeof(string)));
                tbl.Columns.Add(new DataColumn("closed_on", typeof(string)));
                tbl.Columns.Add(new DataColumn("asset", typeof(string)));
                tbl.Columns.Add(new DataColumn("access", typeof(string)));
                tbl.Columns.Add(new DataColumn("sub_request_type", typeof(string)));
                tbl.Columns.Add(new DataColumn("account_name", typeof(string)));
                tbl.Columns.Add(new DataColumn("requestor_name", typeof(string)));
                tbl.Columns.Add(new DataColumn("requestor_employee_id", typeof(string)));
                tbl.Columns.Add(new DataColumn("ticket_id", typeof(string)));
                tbl.Columns.Add(new DataColumn("ticket_status", typeof(string)));
                tbl.Columns.Add(new DataColumn("mail_to", typeof(string)));
                tbl.Columns.Add(new DataColumn("mail_sent_on", typeof(string)));
                tbl.Columns.Add(new DataColumn("mail_sent_on_date", typeof(string)));

                foreach (AccessNowDataItemModel currentItem in passedRequestList)
                {
                    DataRow dr = tbl.NewRow();
                    dr["request_id"] = currentItem.Request_id;
                    dr["requested_on"] = currentItem.Requested_on;
                    dr["request_type"] = currentItem.Request_type;
                    dr["status"] = currentItem.Status;
                    dr["closed_on"] = currentItem.Closed_on;
                    dr["asset"] = currentItem.Asset;
                    dr["access"] = currentItem.Access;
                    dr["sub_request_type"] = currentItem.Sub_request_type;
                    dr["account_name"] = currentItem.Account_name;
                    dr["requestor_name"] = currentItem.Requestor_name;
                    dr["requestor_employee_id"] = currentItem.Requestor_employee_id;
                    dr["ticket_id"] = currentItem.Ticket_id;
                    dr["ticket_status"] = currentItem.Ticket_status;
                    dr["mail_to"] = currentItem.Mail_to;
                    dr["mail_sent_on"] = currentItem.Mail_sent_on;
                    dr["mail_sent_on_date"] = currentItem.Mail_sent_on_date;
                    tbl.Rows.Add(dr);
                }


                objbulk.ColumnMappings.Add("request_id", "request_id");
                objbulk.ColumnMappings.Add("requested_on", "requested_on");
                objbulk.ColumnMappings.Add("request_type", "request_type");
                objbulk.ColumnMappings.Add("status", "status");
                objbulk.ColumnMappings.Add("closed_on", "closed_on");
                objbulk.ColumnMappings.Add("asset", "asset");
                objbulk.ColumnMappings.Add("access", "access");
                objbulk.ColumnMappings.Add("sub_request_type", "sub_request_type");
                objbulk.ColumnMappings.Add("account_name", "account_name");
                objbulk.ColumnMappings.Add("requestor_name", "requestor_name");
                objbulk.ColumnMappings.Add("requestor_employee_id", "requestor_employee_id");
                objbulk.ColumnMappings.Add("ticket_id", "ticket_id");
                objbulk.ColumnMappings.Add("ticket_status", "ticket_status");
                objbulk.ColumnMappings.Add("mail_to", "mail_to");
                objbulk.ColumnMappings.Add("mail_sent_on", "mail_sent_on");
                objbulk.ColumnMappings.Add("mail_sent_on_date", "mail_sent_on_date");

                objbulk.WriteToServer(tbl);

                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the FactoolRequest History Table
        public int AddAccessNowHistory(AccessNowHistoryItemModel passedHistory, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (ModifiedBy, ModifiedDate, Changes) " +
                    "VALUES (@val1,@val2,@val3)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                if (passedHistory.ModifiedBy == null) { UserPrincipal userPrincipal = UserPrincipal.Current; passedHistory.ModifiedBy = userPrincipal.Name; }
                if (passedHistory.ModifiedDate == null) { DateTime createDate = DateTime.UtcNow; passedHistory.ModifiedDate = createDate.ToString("yyyy/MM/dd h:mm:ss.fff"); }
                if (passedHistory.Changes == null) { passedHistory.Changes = ""; }

                cmdUpdate.Parameters.AddWithValue("@val1", passedHistory.ModifiedBy);
                cmdUpdate.Parameters.AddWithValue("@val2", passedHistory.ModifiedDate);
                cmdUpdate.Parameters.AddWithValue("@val3", passedHistory.Changes);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the AccessNow Closed Ticket Table
        public int AddAccessNowCompletedTicket(AccessNowClosedTicketItemModel closedTicket, string table)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (TicketNumber, FinishedState, Comments, ClosedBy, DateClosed) " +
                    "VALUES (@val1,@val2,@val3,@val4,@val5)";

                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                if (closedTicket.TicketNumber == null) { closedTicket.TicketNumber = ""; }
                if (closedTicket.FinishedState == null) { closedTicket.FinishedState = ""; }
                if (closedTicket.Comments == null) { closedTicket.Comments = ""; }
                if (closedTicket.ClosedBy == null) { closedTicket.ClosedBy = ""; }
                if (closedTicket.DateClosed == null) { closedTicket.DateClosed = ""; }

                cmdUpdate.Parameters.AddWithValue("@val1", closedTicket.TicketNumber);
                cmdUpdate.Parameters.AddWithValue("@val2", closedTicket.FinishedState);
                cmdUpdate.Parameters.AddWithValue("@val3", closedTicket.Comments);
                cmdUpdate.Parameters.AddWithValue("@val4", closedTicket.ClosedBy);
                cmdUpdate.Parameters.AddWithValue("@val5", closedTicket.DateClosed);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the FactoolRequest Sync History Table
        public int AddFactoolConfig(string table, string name, string value, string displayorder, string modifiedby, string modified)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (Name, Value, DisplayOrder, ModifiedBy, Modified) VALUES (@val1,@val2,@val3,@val4,@val5)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", name);
                cmdUpdate.Parameters.AddWithValue("@val2", value);
                cmdUpdate.Parameters.AddWithValue("@val3", displayorder);
                cmdUpdate.Parameters.AddWithValue("@val4", modifiedby);
                cmdUpdate.Parameters.AddWithValue("@val5", modified);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Add User Configuration Record
        public int AddUserConfigurationRecord(string table, string Username, string FontScale)
        {
            try
            {
                ConnectToDatabase();

                string strCommand = "INSERT INTO " + table + " (Username, FontScale) VALUES (@val1,@val2)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;
                cmdUpdate.Parameters.AddWithValue("@val1", Username);
                cmdUpdate.Parameters.AddWithValue("@val2", FontScale);
                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();
                Disconnect();
                return returnValue;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return -1;
            }
        }

        //Adds a Record to the AccessNow Operations Table
        public void AddAccessNowOperationsRecord(AccessNowRequestModel.SQLObject AccessNowObject, string table)
        {
            try
            {
                ConnectToDatabase();
                string strCommand = "INSERT INTO " + table + " " +
                    "(request_id," +
                    "requested_on," +
                    "request_type," +
                    "status," +
                    "closed_on," +
                    "asset," +
                    "access," +
                    "sub_request_type," +
                    "account_name," +
                    "requestor_name," +
                    "requestor_employee_id," +
                    "ticket_id," +
                    "ticket_status," +
                    "mail_to," +
                    "mail_sent_on," +
                    "mail_sent_on_date," +
                    "TicketCount," +
                    "Category," +
                    "Type," +
                    "SLADays)" +
                    "VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9,@val10,@val11,@val12,@val13,@val14,@val15,@val16,@val17,@val18,@val19,@val20)";
                SqlCommand cmdUpdate = new();
                cmdUpdate.Connection = conn;
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.CommandText = strCommand;

                cmdUpdate.Parameters.AddWithValue("@val1", AccessNowObject.request_id);
                cmdUpdate.Parameters.AddWithValue("@val2", AccessNowObject.requested_on);
                cmdUpdate.Parameters.AddWithValue("@val3", AccessNowObject.request_type);
                cmdUpdate.Parameters.AddWithValue("@val4", AccessNowObject.status);
                cmdUpdate.Parameters.AddWithValue("@val5", AccessNowObject.closed_on);
                cmdUpdate.Parameters.AddWithValue("@val6", AccessNowObject.asset);
                cmdUpdate.Parameters.AddWithValue("@val7", AccessNowObject.access);
                cmdUpdate.Parameters.AddWithValue("@val8", AccessNowObject.sub_request_type);
                cmdUpdate.Parameters.AddWithValue("@val9", AccessNowObject.account_name);
                cmdUpdate.Parameters.AddWithValue("@val10", AccessNowObject.requestor_name);
                cmdUpdate.Parameters.AddWithValue("@val11", AccessNowObject.requestor_employee_id);
                cmdUpdate.Parameters.AddWithValue("@val12", AccessNowObject.ticket_id);
                cmdUpdate.Parameters.AddWithValue("@val13", AccessNowObject.ticket_status);
                cmdUpdate.Parameters.AddWithValue("@val14", AccessNowObject.mail_to);
                cmdUpdate.Parameters.AddWithValue("@val15", AccessNowObject.mail_sent_on);
                cmdUpdate.Parameters.AddWithValue("@val16", AccessNowObject.mail_sent_on_date);
                cmdUpdate.Parameters.AddWithValue("@val17", AccessNowObject.TicketCount);
                cmdUpdate.Parameters.AddWithValue("@val18", AccessNowObject.Category);
                cmdUpdate.Parameters.AddWithValue("@val19", AccessNowObject.Type);
                cmdUpdate.Parameters.AddWithValue("@val20", AccessNowObject.SLADays);

                int returnValue = -1;
                returnValue = cmdUpdate.ExecuteNonQuery();

                Disconnect();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }
        #endregion
    }
}