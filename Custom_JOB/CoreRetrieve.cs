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
        /// ���o��Ʈw�s�u
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
        /// �����Ʈw�s�u
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
        /// �}�l���
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
        /// ����g�J
        /// </summary>
        public void Commit()
        {
            transaction.Commit();
            transaction.Dispose();
            transaction = null;
            bTransactionStart = false;
        }

        /// <summary>
        /// ����^�_
        /// </summary>
        public void Rollback()
        {
            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
            bTransactionStart = false;
        }

        /// <summary>
        /// ���o��ƪ�C(�HDictionary�s��U�����)
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

            //�p�GConnection����S�����}�A�N�۰��������}�C
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

                objDataReader.Close();  //�@�w�Χ��N�n�����A���MConnection��������A���M�Ӧ���Connection�|�ݯd��C

            }

            return objList;

        }   

        /// <summary>
        /// ���o��ƪ�C
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public DataTable doTableDataFill(string sSQL)
        {

            DataTable dtResult = new DataTable();

            //�p�GConnection����S�����}�A�N�۰��������}�C
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
        /// ���o��ƪ�C
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

                //�p�GConnection����S�����}�A�N�۰��������}�C
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
        /// ���o�P�@�h�`�I���UTag���
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
                //�p�G��Ʀ��V�X��@�P�ƦX�����`�I��ơA�i�H�Φ��P�_�u���@�h���
                if (it.Current.SelectDescendants(XPathNodeType.Element, false).Count == 0)
                {
                    sTagName = it.Current.Name;

                    if (htResult.ContainsKey(sTagName))
                    {
                        throw new Exception("���|" + sXPath + "�s�b����TagName[" + sTagName + "]");
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
        /// ���o���w���|�U�㦳�h�����Ӫ����
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

                //�����@�㵧���
                while (itDetail.MoveNext())
                {
                    sTagName = itDetail.Current.Name;

                    if (htResult.ContainsKey(sTagName))
                    {
                        throw new Exception("���|" + sXPath + "�l���s�b����TagName[" + sTagName + "]");
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
        /// ���o��ƪ�C,�óz�LDictionary�x�s�P�ϥ�Parameter�Ѽ�.
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public DataTable doParameterDataTableFill(string sSQL, Dictionary<string, object> pParaDict)
        {
            DataTable dtResult = new DataTable();
            try
            {
                //�p�GConnection����S�����}�A�N�۰��������}�C
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
