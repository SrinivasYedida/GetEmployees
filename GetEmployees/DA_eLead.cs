using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace GetEmployees
{
    class DA_eLead
    {
        public static string[] CheckVSTokenExpiry()
        {
            string[] res = new string[2];
            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AXELConnectionString"].ConnectionString);
                SqlCommand SqlCommnad = new SqlCommand("[USP_AXEL_ELEAD_TOKENEXPIRE]", Connection);
                SqlCommnad.CommandType = CommandType.StoredProcedure;
                SqlCommnad.Parameters.Add(new SqlParameter("@Token", SqlDbType.VarChar, 5000));
                SqlCommnad.Parameters["@Token"].Direction = ParameterDirection.Output;
                SqlCommnad.Parameters.Add(new SqlParameter("@IsExpire", SqlDbType.Int));
                SqlCommnad.Parameters["@IsExpire"].Direction = ParameterDirection.Output;
                Connection.Open();
                SqlCommnad.ExecuteNonQuery();
                Connection.Close();
                res[0] = SqlCommnad.Parameters["@IsExpire"].Value.ToString();
                res[1] = SqlCommnad.Parameters["@Token"].Value.ToString();
                return res;
            }
            catch (Exception ee)
            {
                return res;
            }
        }
        public static int InsertVSToken(string Token, string TokenExpiryTime, string TokenType)
        {
            int num = 0;
            List<VSDealers> Dealernfo = new List<VSDealers>();
            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AXELConnectionString"].ConnectionString);
                SqlCommand SqlCommnad = new SqlCommand("[USP_AXEL_ELEAD_GENERATE_TOKEN_ACTION]", Connection);
                SqlCommnad.CommandType = CommandType.StoredProcedure;
                SqlCommnad.Parameters.Add(new SqlParameter("@Token_value", SqlDbType.VarChar));
                SqlCommnad.Parameters["@Token_value"].Value = Token;
                SqlCommnad.Parameters.Add(new SqlParameter("@Token_ExpireTime", SqlDbType.VarChar));
                SqlCommnad.Parameters["@Token_ExpireTime"].Value = TokenExpiryTime;
                SqlCommnad.Parameters.Add(new SqlParameter("@Token_Type", SqlDbType.VarChar));
                SqlCommnad.Parameters["@Token_Type"].Value = TokenType;
                SqlCommnad.Parameters.Add(new SqlParameter("@Err_No", SqlDbType.Int));
                SqlCommnad.Parameters["@Err_No"].Direction = ParameterDirection.Output;
                Connection.Open();
                SqlCommnad.ExecuteNonQuery();
                Connection.Close();
                num = Convert.ToInt32(SqlCommnad.Parameters["@Err_No"].Value.ToString());
                return num;
            }
            catch (Exception ee)
            {
                return num;
            }
        }
        //public static int VSDBInsertion(String resultData, int AiQDlrId, int SrcType, string ProcName)
        //{
        //    SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AIConnectionString"].ConnectionString);
        //    SqlCommand SqlCommnad = new SqlCommand(ProcName, Connection);
        //    SqlCommnad.CommandType = CommandType.StoredProcedure;
        //    //SqlCommnad.CommandTimeout=
        //    SqlCommnad.Parameters.Add(new SqlParameter("@XML", SqlDbType.Xml));
        //    SqlCommnad.Parameters["@XML"].Value = resultData;
        //    SqlCommnad.Parameters.Add(new SqlParameter("@AiQDlrId", SqlDbType.Int));
        //    SqlCommnad.Parameters["@AiQDlrId"].Value = AiQDlrId;
        //    SqlCommnad.Parameters.Add(new SqlParameter("@VSSrctype", SqlDbType.Int));
        //    SqlCommnad.Parameters["@VSSrctype"].Value = SrcType;
        //    SqlCommnad.Parameters.Add(new SqlParameter("@LogIdentity", SqlDbType.Int));
        //    SqlCommnad.Parameters["@LogIdentity"].Direction = ParameterDirection.Output;
        //    //SqlCommnad.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int));
        //    //SqlCommnad.Parameters["@CreatedBy"].Value = UserId;
        //    Connection.Open();
        //    int i = SqlCommnad.ExecuteNonQuery();
        //    int Identity = Convert.ToInt32(SqlCommnad.Parameters["@LogIdentity"].Value.ToString());
        //    Connection.Close();
        //    return Identity;
        //}

        public static int LogInsertion(int DmsID, string XML, string ErrCode, string ErrMsg, string FileType,string FileSubType,string ISFILESAME,DateTime EndUserTime,string EndUserStartTime, int DmsSourceType, int CreatedBy)
        {
            int Identity = 0;
            List<VSDealers> Dealernfo = new List<VSDealers>();
            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AXELConnectionString"].ConnectionString);
                SqlCommand SqlCommnad = new SqlCommand("[USP_DMS_ELEADS_LOG_ACTION_DELTA]", Connection);
                SqlCommnad.CommandType = CommandType.StoredProcedure;
                SqlCommnad.Parameters.Add(new SqlParameter("@DmsId", SqlDbType.Int));
                SqlCommnad.Parameters["@DmsId"].Value = DmsID;
                SqlCommnad.Parameters.Add(new SqlParameter("@XML", SqlDbType.VarChar,200000));
                SqlCommnad.Parameters["@XML"].Value = XML;
                SqlCommnad.Parameters.Add(new SqlParameter("@ErrCode", SqlDbType.VarChar));
                SqlCommnad.Parameters["@ErrCode"].Value = ErrCode;
                SqlCommnad.Parameters.Add(new SqlParameter("@ErrMsg", SqlDbType.VarChar));
                SqlCommnad.Parameters["@ErrMsg"].Value = ErrMsg;
                SqlCommnad.Parameters.Add(new SqlParameter("@FileType", SqlDbType.Char));
                SqlCommnad.Parameters["@FileType"].Value = FileType;
                SqlCommnad.Parameters.Add(new SqlParameter("@FileSubType", SqlDbType.Char));
                SqlCommnad.Parameters["@FileSubType"].Value = FileSubType;
                SqlCommnad.Parameters.Add(new SqlParameter("@ISFILESAME", SqlDbType.Char));
                SqlCommnad.Parameters["@ISFILESAME"].Value = ISFILESAME;
                SqlCommnad.Parameters.Add(new SqlParameter("@EndUserTime", SqlDbType.DateTime));
                SqlCommnad.Parameters["@EndUserTime"].Value = EndUserTime;
                SqlCommnad.Parameters.Add(new SqlParameter("@EndUserStartTime", SqlDbType.NVarChar));
                SqlCommnad.Parameters["@EndUserStartTime"].Value = EndUserStartTime;
                SqlCommnad.Parameters.Add(new SqlParameter("@dmsSourceType", SqlDbType.Int));
                SqlCommnad.Parameters["@dmsSourceType"].Value = DmsSourceType;
                SqlCommnad.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int));
                SqlCommnad.Parameters["@CreatedBy"].Value = CreatedBy;
                SqlCommnad.Parameters.Add(new SqlParameter("@LOGID", SqlDbType.Int));
                SqlCommnad.Parameters["@LOGID"].Direction = ParameterDirection.Output;
                Connection.Open();
                int i = SqlCommnad.ExecuteNonQuery();
                Identity = Convert.ToInt32(SqlCommnad.Parameters["@LOGID"].Value);
                Connection.Close();
            }
            catch (Exception ee)
            {

            }
            return Identity;
        }


        public static int ErrorLogInsertion(int DmsID, string ErrCode, string ErrMsg, string FileType, string FileSubType, DateTime EndUserTime, string EndUserStartTime, int DmsSourceType, int CreatedBy)
        {
            int Identity = 0;
            List<VSDealers> Dealernfo = new List<VSDealers>();
            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AXELConnectionString"].ConnectionString);
                SqlCommand SqlCommnad = new SqlCommand("[USP_DMS_ELEADS_ERRORLOG_ACTION_DELTA]", Connection);
                SqlCommnad.CommandType = CommandType.StoredProcedure;
                SqlCommnad.Parameters.Add(new SqlParameter("@DmsId", SqlDbType.Int));
                SqlCommnad.Parameters["@DmsId"].Value = DmsID;             
                SqlCommnad.Parameters.Add(new SqlParameter("@ErrCode", SqlDbType.VarChar));
                SqlCommnad.Parameters["@ErrCode"].Value = ErrCode;
                SqlCommnad.Parameters.Add(new SqlParameter("@ErrMsg", SqlDbType.VarChar));
                SqlCommnad.Parameters["@ErrMsg"].Value = ErrMsg;
                SqlCommnad.Parameters.Add(new SqlParameter("@FileType", SqlDbType.Char));
                SqlCommnad.Parameters["@FileType"].Value = FileType;
                SqlCommnad.Parameters.Add(new SqlParameter("@FileSubType", SqlDbType.Char));
                SqlCommnad.Parameters["@FileSubType"].Value = FileSubType;               
                SqlCommnad.Parameters.Add(new SqlParameter("@EndUserTime", SqlDbType.DateTime));
                SqlCommnad.Parameters["@EndUserTime"].Value = EndUserTime;
                SqlCommnad.Parameters.Add(new SqlParameter("@EndUserStartTime", SqlDbType.NVarChar));
                SqlCommnad.Parameters["@EndUserStartTime"].Value = EndUserStartTime;
                SqlCommnad.Parameters.Add(new SqlParameter("@dmsSourceType", SqlDbType.Int));
                SqlCommnad.Parameters["@dmsSourceType"].Value = DmsSourceType;
                SqlCommnad.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int));
                SqlCommnad.Parameters["@CreatedBy"].Value = CreatedBy;
                SqlCommnad.Parameters.Add(new SqlParameter("@LOGID", SqlDbType.Int));
                SqlCommnad.Parameters["@LOGID"].Direction = ParameterDirection.Output;
                Connection.Open();
                int i = SqlCommnad.ExecuteNonQuery();
                Identity = Convert.ToInt32(SqlCommnad.Parameters["@LOGID"].Value);
                Connection.Close();
            }
            catch (Exception ee)
            {

            }
            return Identity;
        }
        public static List<CdkDealers> GetDMSActiveDealers(int GROUP_ID,int DST_ID,string FILESUBTYPE)
        {
            List<CdkDealers> DealerInfo = new List<CdkDealers>();
            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AXELConnectionString"].ConnectionString);
                SqlCommand SqlCommnad = new SqlCommand("[USP_GET_DMS_ACTIVE_DEALERS_DELTA]", Connection);
                SqlCommnad.CommandType = CommandType.StoredProcedure;

                SqlCommnad.Parameters.Add(new SqlParameter("@GROUP_ID", SqlDbType.Int));
                SqlCommnad.Parameters["@GROUP_ID"].Value = GROUP_ID;

                SqlCommnad.Parameters.Add(new SqlParameter("@DST_ID", SqlDbType.Int));
                SqlCommnad.Parameters["@DST_ID"].Value = DST_ID;

                SqlCommnad.Parameters.Add(new SqlParameter("@FILESUBTYPE", SqlDbType.Char,1));
                SqlCommnad.Parameters["@FILESUBTYPE"].Value = FILESUBTYPE;

                SqlCommnad.Parameters.Add(new SqlParameter("@LAST_LOG_ENDUSERTIME", SqlDbType.VarChar,50));
                SqlCommnad.Parameters["@LAST_LOG_ENDUSERTIME"].Direction = ParameterDirection.Output;

                DealerInfo = DAGetList.GetListFromCommand<CdkDealers>(SqlCommnad);

                return DealerInfo;
            }
            catch (Exception ee)
            {
                return DealerInfo;
            }
        }

        public static List<CompanyPositions> GetCompanyPositions(int DmsId)
        {
            List<CompanyPositions> CompanyPositions = new List<CompanyPositions>();
            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AXELConnectionString"].ConnectionString);
                SqlCommand SqlCommnad = new SqlCommand("[USP_GET_POSITIONS_HASEMPLOYEES]", Connection);
                SqlCommnad.CommandType = CommandType.StoredProcedure;
                SqlCommnad.Parameters.Add(new SqlParameter("@DmsId", SqlDbType.Int));
                SqlCommnad.Parameters["@DmsId"].Value = DmsId;
                CompanyPositions = DAGetList.GetListFromCommand<CompanyPositions>(SqlCommnad);

                return CompanyPositions;
            }
            catch (Exception ee)
            {
                return CompanyPositions;
            }
        }

        public static List<PositionsDeltaAction> PositionsDeltaAction(string FILETYPE,string FILESUBTYPE,int FILETYPEDURATION)
        {
            List<PositionsDeltaAction> CompanyPositions = new List<PositionsDeltaAction>();
            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AXELConnectionString"].ConnectionString);
                SqlCommand SqlCommnad = new SqlCommand("[USP_ELEADS_POSITIONS_DELTA_ACTION]", Connection);
                SqlCommnad.CommandType = CommandType.StoredProcedure;

                SqlCommnad.Parameters.Add(new SqlParameter("@FILETYPE", SqlDbType.Char, 1));
                SqlCommnad.Parameters["@FILETYPE"].Value = FILETYPE;

                SqlCommnad.Parameters.Add(new SqlParameter("@FILESUBTYPE", SqlDbType.Char, 1));
                SqlCommnad.Parameters["@FILESUBTYPE"].Value = FILESUBTYPE;

                SqlCommnad.Parameters.Add(new SqlParameter("@FILETYPEDURATION", SqlDbType.Int));
                SqlCommnad.Parameters["@FILETYPEDURATION"].Value = FILETYPEDURATION;

                CompanyPositions = DAGetList.GetListFromCommand<PositionsDeltaAction>(SqlCommnad);

                return CompanyPositions;
            }
            catch (Exception ee)
            {
                return CompanyPositions;
            }
        }

          public static List<PositionsDeltaAction> EmployeesDeltaAction(string FILETYPE,string FILESUBTYPE,int FILETYPEDURATION)
            {
            List<PositionsDeltaAction> CompanyPositions = new List<PositionsDeltaAction>();
            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AXELConnectionString"].ConnectionString);
                SqlCommand SqlCommnad = new SqlCommand("[USP_ELEADS_EMPLOYEE_DELTA_ACTION]", Connection);
                SqlCommnad.CommandType = CommandType.StoredProcedure;

                SqlCommnad.Parameters.Add(new SqlParameter("@FILETYPE", SqlDbType.Char, 1));
                SqlCommnad.Parameters["@FILETYPE"].Value = FILETYPE;

                SqlCommnad.Parameters.Add(new SqlParameter("@FILESUBTYPE", SqlDbType.Char, 1));
                SqlCommnad.Parameters["@FILESUBTYPE"].Value = FILESUBTYPE;

                SqlCommnad.Parameters.Add(new SqlParameter("@FILETYPEDURATION", SqlDbType.Int));
                SqlCommnad.Parameters["@FILETYPEDURATION"].Value = FILETYPEDURATION;

                CompanyPositions = DAGetList.GetListFromCommand<PositionsDeltaAction>(SqlCommnad);

                return CompanyPositions;
            }
            catch (Exception ee)
            {
                return CompanyPositions;
            }
           }
    }
}
 