using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;

namespace CustomJob
{
    class CoreRetrieve
    {
        public bool bError = false;
        public bool bTransactionStart = false;
        public OracleConnection objConn = null;
        public OracleDataReader objDataReader = null;
        public OracleTransaction transaction = null;

        /// <summary>
        /// Contructure
        /// </summary>
        public CoreRetrieve()
        {
        }

        /// <summary>
        /// 取得資料庫連線
        /// </summary>
        public void doConnectDB(string sConnStr)
        {

            try
            {
                objConn = new OracleConnection(sConnStr);
            }
            catch (Exception e)
            {
                throw e;
            }


        }   

        /// <summary>
        /// 釋放資料庫連線
        /// </summary>
        public void doDisconnectDB()
        {
            if (objDataReader != null)
            {
                objDataReader.Close();
                objDataReader = null;
            }

            if (objConn != null)
            {
                if (objConn.State != ConnectionState.Closed)
                {
                    objConn.Close();
                }

                objConn = null;
            }

        }

        /// <summary>
        /// 開始交易
        /// </summary>
        public void BeginTransaction()
        {
            if (objConn.State != ConnectionState.Open)
            {
                objConn.Open();
            }

            transaction = objConn.BeginTransaction(IsolationLevel.ReadCommitted);
            bTransactionStart = true;
        }

        /// <summary>
        /// 交易寫入
        /// </summary>
        public void Commit()
        {
            transaction.Commit();
            transaction.Dispose();
            transaction = null;
            bTransactionStart = false;
        }

        /// <summary>
        /// 交易回復
        /// </summary>
        public void Rollback()
        {
            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
            bTransactionStart = false;
        }

        /// <summary>
        /// 取得資料表列(以Dictionary存放各欄位資料)
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> doTableDataQuery(string sSQL)
        {

            List<Dictionary<string, string>> objList = new List<Dictionary<string, string>>();

            OracleCommand cmd = new OracleCommand(sSQL, objConn);
            
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            //如果Connection物件沒有打開，就自動幫忙打開。
            if (objConn.State != ConnectionState.Open)
            {
                objConn.Open();
            }

            using (objDataReader = cmd.ExecuteReader())
            {
                while (objDataReader.Read())
                {

                    Dictionary<string, string> dicData = new Dictionary<string, string>();

                    for (int i = 0; i < objDataReader.FieldCount; i++)
                    {
                        dicData[objDataReader.GetName(i)] = Convert.ToString(objDataReader[i]);
                    }

                    objList.Add(dicData);

                }

                objDataReader.Close();  //一定用完就要關閉，不然Connection關閉之後，仍然該次的Connection會殘留住。

            }

            return objList;

        }   

        /// <summary>
        /// 取得資料表列
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public DataTable doTableDataFill(string sSQL)
        {

            DataTable dtResult = new DataTable();

            //如果Connection物件沒有打開，就自動幫忙打開。
            if (objConn.State != ConnectionState.Open)
            {
                objConn.Open();
            }

            OracleCommand sqlCmd = new OracleCommand();
            
            //((Oracle.DataAccess.Client.OracleCommand)sqlCmd).InitialLONGFetchSize = -1;

            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sSQL;
            sqlCmd.Connection = objConn;

            if (transaction != null)
            {
                sqlCmd.Transaction = transaction;
            }

            OracleDataAdapter sqlAdapter = new OracleDataAdapter();
            sqlAdapter.SelectCommand = sqlCmd;
            dtResult.Clear();
            sqlAdapter.Fill(dtResult);

            sqlCmd.Dispose();
            sqlAdapter.Dispose();

            return dtResult;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtTable"></param>
        /// <returns></returns>
        public int doTableDataApplyChange(DataSet ds, string sSQL, string sTableName)
        {

            int iUpdateCount = 0;

            OracleDataAdapter adapter = new OracleDataAdapter();
            adapter.SelectCommand = new OracleCommand(sSQL, objConn);
            OracleCommandBuilder builder = new OracleCommandBuilder(adapter);

            if (objConn.State != ConnectionState.Open)
            {
                objConn.Open();
            }

            //OracleTransaction transaction = objConn.BeginTransaction();
            
            try
            {
                if (transaction != null)
                {
                    adapter.SelectCommand.Transaction = transaction;
                }

                iUpdateCount = adapter.Update(ds, sTableName);
                //transaction.Commit();
            }
            catch (Exception e)
            {
                //transaction.Rollback();
                throw e;
            }
            finally
            {
                //conn.Close();
            }

            return iUpdateCount;

        }

        /// <summary>
        /// 取得資料表列
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public int doTableDataUpdate(string sSQL)
        {

            List<Dictionary<string, string>> objList = new List<Dictionary<string, string>>();

            OracleCommand cmd;

            try
            {
                cmd = new OracleCommand(sSQL, objConn);

                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }

                //如果Connection物件沒有打開，就自動幫忙打開。
                if (objConn.State != ConnectionState.Open)
                {
                    objConn.Open();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return cmd.ExecuteNonQuery();

        }  

        /// <summary>
        /// 取得同一層節點中各Tag資料
        /// </summary>
        /// <param name="it"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetTagValues(XPathNavigator nav, string sXPath)
        {
            string sTagName;
            Dictionary<string, string> htResult = new Dictionary<string, string>();
            XPathNodeIterator it = nav.Select(sXPath);
            while (it.MoveNext())
            {
                //如果資料有混合單一與複合式的節點資料，可以用此判斷只取一層資料
                if (it.Current.SelectDescendants(XPathNodeType.Element, false).Count == 0)
                {
                    sTagName = it.Current.Name;

                    if (htResult.ContainsKey(sTagName))
                    {
                        throw new Exception("路徑" + sXPath + "存在重複TagName[" + sTagName + "]");
                    }
                    else
                    {
                        htResult[sTagName] = it.Current.Value;
                    }
                }
            }

            return htResult;
        }   

        /// <summary>
        /// 取得指定路徑下具有多筆明細的資料
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public List<Dictionary<string, string>> GetDetailValueList(XPathNavigator nav, string sXPath)
        {

            string sTagName;

            Dictionary<string, string> htResult;
            List<Dictionary<string, string>> htlResult = new List<Dictionary<string, string>>();

            XPathNodeIterator it = nav.Select(sXPath);

            while (it.MoveNext())
            {

                XPathNodeIterator itDetail = it.Current.SelectDescendants(XPathNodeType.Element, false);

                htResult = new Dictionary<string, string>();

                //收集一整筆資料
                while (itDetail.MoveNext())
                {
                    sTagName = itDetail.Current.Name;

                    if (htResult.ContainsKey(sTagName))
                    {
                        throw new Exception("路徑" + sXPath + "子項存在重複TagName[" + sTagName + "]");
                    }
                    else
                    {
                        htResult[sTagName] = itDetail.Current.Value;
                    }

                }

                htlResult.Add(htResult);

            }

            return htlResult;

        }

        /// <summary>
        /// 取得資料表列,並透過Dictionary儲存與使用Parameter參數.
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public DataTable doParameterDataTableFill(string sSQL, Dictionary<string, object> pParaDict)
        {
            DataTable dtResult = new DataTable();
            try
            {
                //如果Connection物件沒有打開，就自動幫忙打開。
                if (objConn.State != ConnectionState.Open)
                {
                    objConn.Open();
                }

                OracleCommand sqlCmd = new OracleCommand();
                sqlCmd.CommandType = CommandType.Text;
                foreach (KeyValuePair<string, object> sPara in pParaDict)
                {
                    sqlCmd.Parameters.Add(new OracleParameter(sPara.Key, sPara.Value));
                }
                sqlCmd.CommandText = sSQL;
                sqlCmd.Connection = objConn;
                //ForDebug Use
                string query = sqlCmd.CommandText;
                foreach (OracleParameter p in sqlCmd.Parameters)
                {
                    query = query.Replace(":" + p.ParameterName, p.Value.ToString());
                }
                OracleDataAdapter sqlAdapter = new OracleDataAdapter();
                sqlAdapter.SelectCommand = sqlCmd;
                dtResult.Clear();
                sqlAdapter.Fill(dtResult);
                sqlCmd.Dispose();
                sqlAdapter.Dispose();
            }
            catch (Exception e)
            {
                bError = true;  
                throw e;
            }
            return dtResult;
        }
    }

}
