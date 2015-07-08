using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Runtime.Serialization;
using System.Configuration;
using Newtonsoft.Json;




namespace Dashboard
{
    /// <summary>
    /// Summary description for DashboardWebService
    /// </summary>
    [WebService(Namespace = "http://localhost")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DashboardWebService : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // method return json of customers escelated messages from IMS Q
        public string getEscalatedCustomersIncidents()
        {

            using (SqlConnection conn = this.getConnection())
            {
                String sqlStr = "SELECT Incident_ID,Incident_Number,Incident_Year,Component,Priority,Processor,Customer FROM Customer_Messages WHERE Escalation='true' AND Processing_Org='DS IMS'";
                List<CustomerIncident> customerIncidentList = new List<CustomerIncident>();

                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Cannot open connection in getEscalatedCustomersIncidents method " + e.Message);

                }

                SqlCommand cmd = new SqlCommand(sqlStr, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CustomerIncident msg = new CustomerIncident()
                        {
                            incidentID = reader[0].ToString(),
                            incidentNumber = reader[1].ToString(),
                            incidentYear = reader[2].ToString(),
                            component = reader[3].ToString(),
                            priority = reader[4].ToString(),
                            processor = reader[5].ToString(),
                            customer = reader[6].ToString()
                        };
                        customerIncidentList.Add(msg);
                    }
                }
                catch (Exception e)
                {

                    DashboardLogger.writeToLogFile("Could not excute command in getEscalatedCustomersIncidents method " + e.Message);
                }

                string json = new JavaScriptSerializer().Serialize(customerIncidentList);
                return json;

            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // method return json string of customers messages from IMS Q that IMS developer requested to call back to customer
        public string getCallToCustomersIncidents()
        {

            using (SqlConnection conn = this.getConnection())
            {
                String sqlStr = "SELECT Incident_ID,Incident_Number,Incident_Year,Component,Priority,Processor,Customer FROM Customer_Messages WHERE Customer_Callback='true' AND Processing_Org='DS IMS'";
                List<CustomerIncident> customerIncidentList = new List<CustomerIncident>();

                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Cannot open connection in getCallToCustomersIncidents mthod " + e.Message);

                }

                SqlCommand cmd = new SqlCommand(sqlStr, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CustomerIncident msg = new CustomerIncident()
                        {
                            incidentID = reader[0].ToString(),
                            incidentNumber = reader[1].ToString(),
                            incidentYear = reader[2].ToString(),
                            component = reader[3].ToString(),
                            priority = reader[4].ToString(),
                            processor = reader[5].ToString(),
                            customer = reader[6].ToString()

                        };
                        customerIncidentList.Add(msg);
                    }
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Could not excute command in getCallToCustomersIncidents method " + e.Message);
                }

                string json = new JavaScriptSerializer().Serialize(customerIncidentList);
                return json;

            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // method return json string of customers messages from IMS Q that that curently in VH prio 
        public String getVHCustomersIncidents()
        {

            using (SqlConnection conn = this.getConnection())
            {
                String sqlStr = "SELECT Incident_ID,Incident_Number,Incident_Year,Component,Priority,Processor,Customer FROM Customer_Messages WHERE Priority ='Very high' AND Processing_Org='DS IMS'";
                List<CustomerIncident> customerVHIncidentList = new List<CustomerIncident>();

                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Cannot open connection in getVHCustomersIncidents method " + e.Message);

                }

                SqlCommand cmd = new SqlCommand(sqlStr, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CustomerIncident msg = new CustomerIncident()
                        {
                            incidentID = reader[0].ToString(),
                            incidentNumber = reader[1].ToString(),
                            incidentYear = reader[2].ToString(),
                            component = reader[3].ToString(),
                            priority = reader[4].ToString(),
                            processor = reader[5].ToString(),
                            customer = reader[6].ToString()

                        };
                        customerVHIncidentList.Add(msg);
                    }
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Could not excute command in getVHCustomersIncidents method " + e.Message);
                }

                string json = new JavaScriptSerializer().Serialize(customerVHIncidentList);
                return json;

            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String getTimeSinceLastReaction()
        {
            using (SqlConnection connTslr = this.getConnection())
            {
                String tslrStr = "SELECT Incident_ID,Incident_Number,Incident_Year,Component,Priority,Processor,Customer,Time_of_Last_Reaction, Processing_Org,Description FROM Customer_Messages WHERE Status <> 'On Hold'";
                List<CustomerIncident> tslrIncidentsList = new List<CustomerIncident>();

                try
                {
                    connTslr.Open();
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Cannot open connection in getTimeSinceLastReaction method" + e.Message);

                }

                SqlCommand cmdTslr = new SqlCommand(tslrStr, connTslr);
                cmdTslr.CommandType = CommandType.Text;

                try
                {
                    SqlDataReader readerTslr = cmdTslr.ExecuteReader();
                    DateTime now = DateTime.Now;

                    while (readerTslr.Read())
                    {
                        String strDate = readerTslr.GetString(7);
                        String procOrg = readerTslr.GetString(8);
                        if ("".Equals(strDate)) continue;

                        DateTime tslr = Convert.ToDateTime(strDate);
                        int tslrDays = (int)((now - tslr).TotalDays);

                        if (((tslrDays > 3) && "DS IMS".Equals(procOrg)) || ((tslrDays >= 7) && !("DS IMS".Equals(procOrg))))
                        {
                            CustomerIncident msg = new CustomerIncident()
                            {
                                incidentID = readerTslr[0].ToString(),
                                incidentNumber = readerTslr[1].ToString(),
                                incidentYear = readerTslr[2].ToString(),
                                component = readerTslr[3].ToString(),
                                priority = readerTslr[4].ToString(),
                                processor = readerTslr[5].ToString(),
                                customer = readerTslr[6].ToString(),
                                organization = readerTslr[8].ToString(),
                                description = readerTslr[9].ToString(),
                                tslr = tslrDays
                            };
                            tslrIncidentsList.Add(msg);
                        }
                    }
                }
                catch (Exception e)
                {
                    return e.Message;
                    //DashboardLogger.writeToLogFile("Could not excute command in getTimeSinceLastReaction method " + e.Message);      
                }

                string json = new JavaScriptSerializer().Serialize(tslrIncidentsList);
                return json;

            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // method return json string of internal messages that should be closed by IMS developer due to the fact that the messsage is on Author Action more than 7 days
        public String getInternalIncidentsShouldBeClosed()
        {

            using (SqlConnection conn = this.getConnection())
            {
                String sqlStr = "SELECT Incident_Number,Component, Priority, Processor, Processor_ID,Change_On, Status FROM Internal_Messages WHERE Status='Author Action'";
                List<InternalIncident> internalIncidentsShouldBeClosedList = new List<InternalIncident>();


                SqlCommand cmd = new SqlCommand(sqlStr, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Cannot open connection in getInternalIncidentsShouldBeClosed method" + e.Message);
                }


                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        String[] splitChangeOn = reader[5].ToString().Split(' ');
                        String changeOnDateStr = splitChangeOn[0];
                        DateTime lastChangeOnDate = Convert.ToDateTime(changeOnDateStr);

                        TimeSpan difference = DateTime.Now - lastChangeOnDate;

                        if (difference.TotalDays > 7)
                        {
                            InternalIncident msg = new InternalIncident()
                            {
                                incidentNumber = reader[0].ToString(),
                                component = reader[1].ToString(),
                                priority = reader[2].ToString(),
                                processor = reader[3].ToString(),
                                processorID = reader[4].ToString(),
                                changeOn = reader[5].ToString(),
                                status = reader[6].ToString()

                            };

                            internalIncidentsShouldBeClosedList.Add(msg);
                        }

                    }
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Could not excute command in getInternalIncidentsShouldBeClosed method " + e.Message);
                }

                string json = new JavaScriptSerializer().Serialize(internalIncidentsShouldBeClosedList);
                return json;
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // method return json string of the Avg. PCC number for questions 2-4
        public String getPCC()
        {
            using (SqlConnection conn = this.getConnection())
            {
                string json = "";
                String sqlStr = "SELECT [PCC_Average_Q2-4] FROM PCC_Support_Organisation WHERE Calender_Month = 'Overall Result'";
                Pcc msg = null;

                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Cannot open connection in getPCC method" + e.Message);
                }

                SqlCommand cmd = new SqlCommand(sqlStr, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    String pccString = reader.HasRows ? reader[0].ToString() : String.Empty;
                    msg = new Pcc() { pcc = pccString };
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Could not excute command in getPCC method " + e.Message);
                }

                json = new JavaScriptSerializer().Serialize(msg);
                return json;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // method return json string of the Solution Share for portal platform
        public String getSolutionShare()
        {
            using (SqlConnection connOldSol = this.getConnection())
            {
                String solSqlStr = "SELECT SUM (Solved_Incidents_Product_Support), SUM (Solved_Incidents) FROM Solution_Share";
                string json = "";
                SqlConnection connSol = null;

                try
                {
                    connSol = this.getConnection();
                    connSol.Open();
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Cannot open connection in getSolutionShare method" + e.Message);
                }

                try
                {
                    SqlCommand cmdSol = new SqlCommand(solSqlStr, connSol);
                    cmdSol.CommandType = CommandType.Text;
                    SqlDataReader readerSol = cmdSol.ExecuteReader();
                    readerSol.Read();

                    float Solved_Incidents_Product_Support = readerSol.GetInt32(0);
                    float Solved_Incidents = readerSol.GetInt32(1);

                    SolutionShare msg = new SolutionShare()
                    {
                        solutionShare = (Solved_Incidents_Product_Support) / (Solved_Incidents) * 100
                    };

                    connSol.Close();
                    json = new JavaScriptSerializer().Serialize(msg);
                }
                catch (InvalidCastException ice)
                {
                    DashboardLogger.writeToLogFile("Could not cast from string to float in getSolutionShare method " + ice.Message);
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Could not excute command in getSolutionShare method " + e.Message);
                }

                return json;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // method return json string of the Solution Share for portal platform
        public String getBestSupportMorningTeam()
        {
            using (SqlConnection conn = this.getConnection("MorningConnectionString"))
            {
                String sqlStr = "SELECT team, operation, COUNT(*) FROM SupportMorning GROUP BY team, operation";
                //
                List<BestSupportMorningTeamEntry> addHintList = new List<BestSupportMorningTeamEntry>();
                List<BestSupportMorningTeamEntry> rerouteList = new List<BestSupportMorningTeamEntry>();
                List<BestSupportMorningTeamEntry> forwardList = new List<BestSupportMorningTeamEntry>();
                List<BestSupportMorningTeamEntry> overallList = new List<BestSupportMorningTeamEntry>();
                //
                Dictionary<String, Int32> overAllByTeamDictionary = new Dictionary<String, Int32>();
                List<List<BestSupportMorningTeamEntry>> winnerList = new List<List<BestSupportMorningTeamEntry>>();

                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    return e.Message;
                    //DashboardLogger.writeToLogFile("Cannot open connection in getBestSupportMorningTeam method" + e.Message);
                }

                SqlCommand cmd = new SqlCommand(sqlStr, conn);
                cmd.CommandType = CommandType.Text;

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();

                    while (reader.Read())
                    {
                        BestSupportMorningTeamEntry entry = new BestSupportMorningTeamEntry()
                        {
                            team = reader[0].ToString(),
                            operation = reader[1].ToString(),
                            count = reader.GetInt32(2),
                        };

                        if ("addHint".Equals(entry.operation))
                        {
                            addHintList.Add(entry);
                        }
                        else if ("reroute".Equals(entry.operation))
                        {
                            rerouteList.Add(entry);
                        }
                        else if ("forwardIMS".Equals(entry.operation))
                        {
                            forwardList.Add(entry);
                        }

                        if (overAllByTeamDictionary.ContainsKey(entry.team))
                        {
                            overAllByTeamDictionary[entry.team] = overAllByTeamDictionary[entry.team] + entry.count;
                        }
                        else
                        {
                            overAllByTeamDictionary.Add(entry.team, entry.count);
                        }

                    }

                    conn.Close();

                    // order the teams by count in desending order for each operation
                    addHintList = addHintList.OrderByDescending(o => o.count).ThenBy(o => o.count).ToList();
                    rerouteList = rerouteList.OrderByDescending(o => o.count).ThenBy(o => o.count).ToList();
                    forwardList = forwardList.OrderByDescending(o => o.count).ThenBy(o => o.count).ToList();

                    // order the teams by count in desending order for ALL operation
                    foreach (KeyValuePair<String, Int32> kv in overAllByTeamDictionary)
                    {
                        BestSupportMorningTeamEntry overAllEntry = new BestSupportMorningTeamEntry()
                        {
                            team = kv.Key,
                            operation = "ALL",
                            count = kv.Value,
                        };

                        overallList.Add(overAllEntry);
                    }

                    overallList = overallList.OrderByDescending(o => o.count).ThenBy(o => o.count).ToList();

                    // add results in final winner list - the [0] position for each inner list represents the winner
                    winnerList.Add(overallList);
                    winnerList.Add(addHintList);
                    winnerList.Add(rerouteList);
                    winnerList.Add(forwardList);

                }
                catch (Exception e)
                {
                    return e.Message;
                    //DashboardLogger.writeToLogFile("Could not excute command in getBestSupportMorningTeam method " + e.Message);      
                }

                string json = new JavaScriptSerializer().Serialize(winnerList);
                return json;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        // method to return json string of total messages in PS/IMS/DEV queue
        public String getTotalNUmberOfCustomerMessagesInAllQueses()
        {
            using (SqlConnection conn = this.getConnection())
            {
                string json = "";
                NumberOfMessagesInQueues msgs = null;
                String IMS = "SELECT COUNT (Incident_ID) FROM Customer_Messages WHERE Processing_Org = 'DS IMS'";
                String DEVELOPMENT = "SELECT COUNT (Incident_ID) FROM Customer_Messages WHERE Processing_Org = 'DS Development'";
                String PS_AMER = "SELECT COUNT (Incident_ID) FROM Customer_Messages WHERE Processing_Org = 'PS VGSC AMER'";
                String PS_APJ = "SELECT COUNT (Incident_ID) FROM Customer_Messages WHERE Processing_Org = 'PS VGSC APJ'";
                String PS_EMEA = "SELECT COUNT (Incident_ID) FROM Customer_Messages WHERE Processing_Org = 'PS VGSC EMEA'";
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Cannot open connection in getTotalNUmberOfCustomerMessagesInAllQueses method" + e.Message);
                }

                SqlCommand IMSCount = new SqlCommand(IMS, conn);
                IMSCount.CommandType = CommandType.Text;

                SqlCommand DevCount = new SqlCommand(DEVELOPMENT, conn);
                DevCount.CommandType = CommandType.Text;

                SqlCommand AmerCount = new SqlCommand(PS_AMER, conn);
                AmerCount.CommandType = CommandType.Text;

                SqlCommand APJCount = new SqlCommand(PS_APJ, conn);
                APJCount.CommandType = CommandType.Text;

                SqlCommand EMEACount = new SqlCommand(PS_EMEA, conn);
                EMEACount.CommandType = CommandType.Text;
                try
                {
                    SqlDataReader IMSreader = IMSCount.ExecuteReader();
                    IMSreader.Read();
                    String IMSresult = IMSreader[0].ToString();
                    int ims = Int32.Parse(IMSresult);

                    conn.Close();
                    conn.Open();

                    SqlDataReader DEVReader = DevCount.ExecuteReader();
                    DEVReader.Read();
                    String DEVresult = DEVReader[0].ToString();
                    int dev = Int32.Parse(DEVresult);

                    conn.Close();
                    conn.Open();

                    SqlDataReader AmerReader = AmerCount.ExecuteReader();
                    AmerReader.Read();
                    String amerResult = AmerReader[0].ToString();
                    int amer = Int32.Parse(amerResult);

                    conn.Close();
                    conn.Open();

                    SqlDataReader APJReader = APJCount.ExecuteReader();
                    APJReader.Read();
                    String apjResult = APJReader[0].ToString();
                    int apj = Int32.Parse(apjResult);

                    conn.Close();
                    conn.Open();

                    SqlDataReader EMEAReader = EMEACount.ExecuteReader();
                    EMEAReader.Read();
                    String emeaResult = EMEAReader[0].ToString();
                    int emea = Int32.Parse(emeaResult);

                    conn.Close();

                    msgs = new NumberOfMessagesInQueues();
                    msgs.numOfMsgsDev = dev.ToString();
                    msgs.numOfMsgsIMS = ims.ToString();
                    msgs.numOfMsgsPSAmer = amer.ToString();
                    msgs.numOfMsgsPSApj = apj.ToString();
                    msgs.numOfMsgsPSEmea = emea.ToString();
                    json = new JavaScriptSerializer().Serialize(msgs);

                }
                catch (Exception e)
                {
                    DashboardLogger.writeToLogFile("Could not excute command in getTotalNUmberOfCustomerMessagesInAllQueses method " + e.Message);
                }

                return json;
            }
        }

        private SqlConnection getConnection(String name)
        {
            string cs = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            SqlConnection conn = new SqlConnection(cs);
            return conn;
        }

        private SqlConnection getConnection()
        {
            string cs = ConfigurationManager.ConnectionStrings["DashboradConnectionString"].ConnectionString;
            //SqlConnection conn = new SqlConnection(@"Network Library=DBMSSOCN;Data Source=iltlvh57.tlv.sap.corp,1433;database=Support_Dashboard;User id=kmcadm;Password=Gauss2004;");
            SqlConnection conn = new SqlConnection(cs);
            //DashboardLogger.writeToLogFile("Create new connection " + cs);
            return conn;
        }

        public class BestSupportMorningTeamEntry
        {
            public String team { get; set; }
            public String operation { get; set; }
            public Int32 count { get; set; }
        }

        public class CustomerIncident
        {
            public String incidentID { get; set; }
            public String incidentNumber { get; set; }
            public String incidentYear { get; set; }
            public String component { get; set; }
            public String priority { get; set; }
            public String processor { get; set; }
            public String customer { get; set; }
            public String organization { get; set; }
            public String description { get; set; }
            public double tslr { get; set; }
        }

        public class InternalIncident
        {
            public String incidentNumber { get; set; }
            public String component { get; set; }
            public String priority { get; set; }
            public String processor { get; set; }
            public String processorID { get; set; }
            public String changeOn { get; set; }
            public String status { get; set; }
        }

        public class SolutionShare
        {
            public float solutionShare { get; set; }
        }

        public class Pcc
        {
            public String pcc { get; set; }
        }

        public class NumberOfMessagesInQueues
        {
            public String numOfMsgsIMS { get; set; }
            public String numOfMsgsDev { get; set; }
            public String numOfMsgsPSAmer { get; set; }
            public String numOfMsgsPSApj { get; set; }
            public String numOfMsgsPSEmea { get; set; }
        }
    }
}
