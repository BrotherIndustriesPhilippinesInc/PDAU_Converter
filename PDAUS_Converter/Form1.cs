using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Net;
using System.Net.Sockets;
using Microsoft.Office.Core;
using System.Diagnostics;

namespace PDAUS_Converter
{
    public partial class Form1 : Form
    {
        // PUBLIC LOCATION \\apbiphsh07\D0_ShareBrotherGroup\19_BPS\17_Installer\PDAUS\PDAUS Background Worker\

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
        SqlCommand cmd;
        SqlCommand cmd2;
        SqlCommand cmdFinal;
        SqlCommand cmdApproval;
        SqlCommand cmd3;

        //CHANGE THIS WEB SERVER INCASE OF MIGRATION
        string webServer = "apbiphbpswb01";
        string portServer = "8080";
        string dateTimeNow = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");


        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var filePath = @"C:\PDAU_Worker\log.txt";
            string message = "";

            try
            {
                con.Open();

                //Checking of Excel to PDF (Request)
                CheckRequestPDF();

                //Checking of Excel for Final PDF (Request)
                CheckFinalPDF();

                con.Close();
            }
            catch (SqlException ex)
            {
                message = $"Log entry at {DateTime.Now}: No database connection.";
                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine(message);
                }
            }
        }
        private void CheckRequestPDF()
        {

            con.Close();
            con.Open();
            cmd = new SqlCommand("SELECT * FROM SCI_Request WHERE SCIForProcess = 1", con);
            cmd.ExecuteNonQuery();

            txtStatus.Text = "Working. Please do not close";

            SqlDataReader dr = null;
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string RequestID = dr["RequestID"].ToString();
                string RequestExcelFile = dr["SCIExcel"].ToString();
                string RequestSection = dr["RequestSection"].ToString();
                string RequestType = dr["RequestType"].ToString();

                string sciDocNumber = dr["SCINo"].ToString();
                string validity = dr["Validity"].ToString();
                string validityDate = dr["ValidityDate"].ToString();
                string title = dr["Title"].ToString();

                string issuanceDate = "TBA";


                string outputPDF = Path.GetFileNameWithoutExtension(RequestExcelFile) + ".pdf";
                string outputPath = @"\\" + webServer + @"\SCI\" + RequestSection + @"\Request\" + RequestID + @"\" + outputPDF;
                string workbookPath = @"\\" + webServer + @"\SCI\" + RequestSection + @"\Request\" + RequestID + @"\" + RequestExcelFile;

                notifyIcon1.BalloonTipText = "Converting Excel Attachment to PDF";
                //notifyIcon1.ShowBalloonTip(1000);



                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = null;
                Excel.Worksheet xlWorkSheet = null;
                Excel.Worksheet sheet;


                xlApp = new Excel.Application();
                xlApp.Visible = false;
                xlApp.DisplayAlerts = false;
                try
                {
                    xlWorkBook = xlApp.Workbooks.Open(workbookPath);
                }
                catch (Exception ex)
                {
                    if (xlWorkSheet != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);
                        xlWorkSheet = null;
                    }

                    if (xlWorkBook != null)
                    {
                        xlWorkBook.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
                        xlWorkBook = null;
                    }

                    if (xlApp != null)
                    {
                        xlApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                        xlApp = null;
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    continue;

                }

                bool sheetExist;

                try
                {
                    sheet = xlWorkBook.Sheets["Main"] as Excel.Worksheet;
                    sheetExist = true;
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                }
                catch (Exception)
                {
                    sheetExist = false;
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
                }



                if (sheetExist) // TRUE
                {
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets["Main"];
                    xlWorkSheet.Hyperlinks.Delete();

                    //SECTION
                    ((Excel.Range)xlWorkSheet.Cells[3, 4]).Value = RequestSection;
                    //ISSUANCE DATA
                    ((Excel.Range)xlWorkSheet.Cells[4, 3]).Value = issuanceDate;
                    //SCI NUMBER
                    ((Excel.Range)xlWorkSheet.Cells[4, 5]).Value = sciDocNumber;
                    //VALIDITY
                    ((Excel.Range)xlWorkSheet.Cells[4, 8]).Value = validity;
                    //VALIDITY DATE
                    ((Excel.Range)xlWorkSheet.Cells[4, 11]).Value = validityDate;
                    //TITLE
                    ((Excel.Range)xlWorkSheet.Cells[5, 2]).Value = title;

                    xlApp.DisplayAlerts = false;
                    xlWorkBook.SaveAs(workbookPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing);
                    xlWorkBook.Close(0);
                    xlApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);

                    // Create COM Objects
                    Microsoft.Office.Interop.Excel.Application excelApplication;
                    Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

                    // Create new instance of Excel
                    excelApplication = new Microsoft.Office.Interop.Excel.Application();


                    // Make the process invisible to the user
                    excelApplication.ScreenUpdating = false;


                    // Make the process silent
                    excelApplication.DisplayAlerts = false;


                    // Open the workbook that you wish to export to PDF
                    excelWorkbook = excelApplication.Workbooks.Open(workbookPath);

                    // If the workbook failed to open, stop, clean up, and bail out
                    if (excelWorkbook == null)
                    {
                        excelApplication.Quit();
                        //excelApplication = null;
                        //excelWorkbook = null;
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
                    }

                    try
                    {
                        // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                        excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
                    }
                    catch (System.Exception)
                    {

                    }
                    finally
                    {
                        // Close the workbook, quit the Excel, and clean up regardless of the results...
                        excelWorkbook.Close(0);
                        //Marshal.ReleaseComObject(excelWorkbook);
                        excelApplication.Quit();

                        //excelApplication = null;
                        //excelWorkbook = null;

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);

                    }

                    cmd2 = new SqlCommand("UPDATE SCI_Request SET SCIForProcess= 0, SCIPDF='" + outputPDF + "' WHERE RequestID ='" + RequestID + "'  ", con);
                    cmd2.ExecuteNonQuery();


                }
                else // FALSE
                {

                    cmd2 = new SqlCommand("UPDATE SCI_Request SET SCIForProcess= 0, Status = 'DECLINED',Location='Requestor', Implement = 99 WHERE RequestID ='" + RequestID + "'  ", con);
                    cmd2.ExecuteNonQuery();

                    cmd2 = new SqlCommand("UPDATE SCI_Approval SET RejectedAt='System', AdminReject= 'System', AdminRejectDate = '" + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt") + "' WHERE RequestID ='" + RequestID + "'  ", con);
                    cmd2.ExecuteNonQuery();

                    cmd2 = new SqlCommand("INSERT INTO SCI_RequestLogs (TransactionDate,RequestID,Event,AccountName) VALUES ('" + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt") + "','" + RequestID + "','System Declined due to wrong template. Please use the system template.','System')", con);
                    cmd2.ExecuteNonQuery();

                    txtStatus.Text = "System Auto Declining";

                    Process.Start("http://" + webServer + ":" + portServer + "/pdaus/email/SCIEmail_systemDecline.php?reqID=" + RequestID + "");

                }
            }
            con.Close();
            txtStatus.Text = "Idle";
        }
        private void CheckFinalPDF()
        {

            con.Close();
            con.Open();
            cmdFinal = new SqlCommand("SELECT * FROM SCI_Request WHERE ForFinalSCI = 1", con);
            cmdFinal.ExecuteNonQuery();

            txtStatus.Text = "Working. Please do not close";

            SqlDataReader drFinal = null;
            drFinal = cmdFinal.ExecuteReader();
            while (drFinal.Read())
            {
                string RequestID = drFinal["RequestID"].ToString();
                string RequestExcelFile = drFinal["SCIExcel"].ToString();
                string RequestSection = drFinal["RequestSection"].ToString();
                string RequestType = drFinal["RequestType"].ToString();
                string sciDocNumber = drFinal["SCINo"].ToString();
                string revNo = drFinal["RevNo"].ToString();
                string RequestDate = drFinal["RequestDate"].ToString();



                //string outputPDF = Path.GetFileNameWithoutExtension(RequestExcelFile) + ".pdf";
                string outputPDF = sciDocNumber + "-" + revNo + ".pdf";
                string outputPath = @"\\" + webServer + @"\SCI\" + RequestSection + @"\MainData\" + sciDocNumber + @"\" + outputPDF;
                string workbookPath = @"\\" + webServer + @"\SCI\" + RequestSection + @"\Request\" + RequestID + @"\" + RequestExcelFile;


                cmdApproval = new SqlCommand("SELECT * FROM SCI_Approval WHERE RequestID='" + RequestID + "'", con);
                cmdApproval.ExecuteNonQuery();
                SqlDataReader drApproval = null;
                drApproval = cmdApproval.ExecuteReader();
                while (drApproval.Read())
                {

                    string Requestor_ADID = drApproval["Requestor_ADID"].ToString();
                    string SPV_ADID = drApproval["SPV_ADID"].ToString();
                    string MGR_ADID = drApproval["MGR_ADID"].ToString();

                    string SPV_DATE = drApproval["SPV_date"].ToString();
                    string MGR_DATE = drApproval["MGR_date"].ToString();

                    notifyIcon1.BalloonTipText = "Converting Excel SCI to SCI PDF";
                    //notifyIcon1.ShowBalloonTip(1000);

                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook;
                    Excel.Worksheet xlWorkSheet;

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(workbookPath);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets["Main"];

                    try
                    {
                        string today = DateTime.Now.ToString("MM-dd-yyyy");

                        var spvDate = Convert.ToDateTime(SPV_DATE).ToString("M/d/yyyy");
                        var mgrDate = Convert.ToDateTime(MGR_DATE).ToString("M/d/yyyy");
                        var requestDate = Convert.ToDateTime(RequestDate).ToString("M/d/yyyy");

                        //ISSUANCE DATE
                        ((Excel.Range)xlWorkSheet.Cells[4, 3]).Value = today;
                        //MGR ADID
                        ((Excel.Range)xlWorkSheet.Cells[3, 10]).Value = MGR_ADID + Environment.NewLine + mgrDate;
                        //SPV ADID
                        ((Excel.Range)xlWorkSheet.Cells[3, 11]).Value = SPV_ADID + Environment.NewLine + spvDate;
                        //REQUESTOR ADID
                        ((Excel.Range)xlWorkSheet.Cells[3, 12]).Value = Requestor_ADID + Environment.NewLine + requestDate;
                        //SCI NO
                        ((Excel.Range)xlWorkSheet.Cells[4, 5]).Value = sciDocNumber + "-" + revNo;
                    }
                    catch (Exception ex)
                    {

                    }


                    try
                    {
                        Microsoft.Office.Interop.Excel.Range oRangesss = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[3, 9];
                        float Leftsss = (float)((double)oRangesss.Left) - 35;
                        float Topsss = (float)((double)oRangesss.Top) - 39;
                        xlWorkSheet.Shapes.AddPicture(@"\\apbiphbpswb01\D$\xampp\htdocs\pdaus\assets\img\certifiedV7.png", Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Leftsss, Topsss, 100, 100);
                    }
                    catch
                    {

                    }


                    xlApp.DisplayAlerts = false;
                    xlWorkBook.SaveAs(workbookPath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                            Type.Missing, Type.Missing);
                    xlWorkBook.Close(0);
                    xlApp.Quit();

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);




                    // Create COM Objects
                    Microsoft.Office.Interop.Excel.Application excelApplication;
                    Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

                    // Create new instance of Excel
                    excelApplication = new Microsoft.Office.Interop.Excel.Application();


                    // Make the process invisible to the user
                    excelApplication.ScreenUpdating = false;


                    // Make the process silent
                    excelApplication.DisplayAlerts = false;


                    // Open the workbook that you wish to export to PDF
                    excelWorkbook = excelApplication.Workbooks.Open(workbookPath);


                    // If the workbook failed to open, stop, clean up, and bail out
                    if (excelWorkbook == null)
                    {
                        excelApplication.Quit();
                        //excelApplication = null;
                        //excelWorkbook = null;

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);
                    }


                    try
                    {
                        // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                        excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);
                    }
                    catch (System.Exception)
                    {

                    }
                    finally
                    {

                        excelWorkbook.Close(0);
                        excelApplication.Quit();

                        //excelApplication = null;
                        //excelWorkbook = null;

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApplication);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelWorkbook);

                    }

                    cmd2 = new SqlCommand("UPDATE SCI_Request SET ForFinalSCI= 0 WHERE RequestID ='" + RequestID + "'  ", con);
                    cmd2.ExecuteNonQuery();

                    cmd3 = new SqlCommand("UPDATE SCI_MainData SET SCIFile= '" + outputPDF + "' WHERE SCINo ='" + sciDocNumber + "'  ", con);
                    cmd3.ExecuteNonQuery();

                }
            }

            con.Close();
            txtStatus.Text = "Idle";
        }

        private void modifyExcel()
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open("");
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets["Main"];

            //SECTION
            ((Excel.Range)xlWorkSheet.Cells[3, 4]).Value = "PE";
            //ISSUANCE DATA
            ((Excel.Range)xlWorkSheet.Cells[4, 3]).Value = "2023-10-30";
            //SCI NUMBER
            ((Excel.Range)xlWorkSheet.Cells[4, 5]).Value = "SCI-BPS-0020-01";
            //VALIDITY
            ((Excel.Range)xlWorkSheet.Cells[4, 8]).Value = "Temporary";
            //VALIDITY DATE
            ((Excel.Range)xlWorkSheet.Cells[4, 11]).Value = "2024-10-23";
            //TITLE
            ((Excel.Range)xlWorkSheet.Cells[5, 2]).Value = "The 3 Musketeers";



            xlApp.DisplayAlerts = false;
            xlWorkBook.SaveAs(@"\\apbiphbpswb01\d$\xampp\htdocs\pdaus\SCI\Special_Check_Item.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);
            xlWorkBook.Close();
            xlApp.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkSheet);
            //this.Close();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.BalloonTipText = "PDAUS Background System Still Running";
                notifyIcon1.ShowBalloonTip(1000);
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.ShowInTaskbar = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void exitProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string msg = "Do you want to close the PDAU Background Worker ?";
            DialogResult result = MessageBox.Show(msg, "Close Confirmation",
                MessageBoxButtons.YesNo/*Cancel*/, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)

                Application.ExitThread();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //testOnly();

            this.ActiveControl = null;
            timeNow.Text = DateTime.Now.ToString("hh:mm:ss tt").ToUpper();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (var ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        //MessageBox.Show(ip.ToString());
            //    }
            //}
            //var date1 = Convert.ToDateTime("December 20, 2023 10:30 AM").ToString("yyyy-M-d");
            //MessageBox.Show("http://" + webServer + ":" + portServer + "/pdaus/email/SCIEmail_systemDecline.php?reqID=" + test1 + "");

            //Process[] processes = Process.GetProcessesByName("excel");
            //foreach (var process in processes)
            //{
            //    try
            //    {
            //        process.Kill();
            //        Console.WriteLine($"Terminated process: {process.ProcessName} (ID: {process.Id})");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error terminating process {process.Id}: {ex.Message}");
            //    }
            //}
            //MessageBox.Show("Success");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (MessageBox.Show("Do you want to close the PDAU Backgroud Worker ?",
                     "Close Confirmation",
                      MessageBoxButtons.YesNo,
                      MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timeNow.Text = DateTime.Now.ToString("hh:mm:ss tt").ToUpper();

            // FOR ABOLISHMENT NOTIFICATION
            //P-TOUCH SECTION
            if (timeNow.Text == "05:00:00 AM")
            {
                System.Threading.Thread.Sleep(1000);
                txtStatus.Text = "Sending Notification";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/email/abolishment_notification.php?section=PT");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";
            }

            //QA-SECTION
            if (timeNow.Text == "05:10:00 AM")
            {
                System.Threading.Thread.Sleep(1000);
                txtStatus.Text = "Sending Notification";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/email/abolishment_notification.php?section=QA");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";
            }

            //PE-SECTION
            if (timeNow.Text == "05:20:00 AM")
            {
                System.Threading.Thread.Sleep(1000);
                txtStatus.Text = "Sending Notification";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/email/abolishment_notification.php?section=PE");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";
            }

            //TC-SECTION
            if (timeNow.Text == "05:30:00 AM")
            {
                System.Threading.Thread.Sleep(1000);
                txtStatus.Text = "Sending Notification";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/email/abolishment_notification.php?section=TC");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";
            }

            //IC-SECTION
            if (timeNow.Text == "05:40:00 AM")
            {
                System.Threading.Thread.Sleep(1000);
                txtStatus.Text = "Sending Notification";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/email/abolishment_notification.php?section=IC");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";
            }


            // ************ SYSTEM ABOLISHMENT *****************
            //P-TOUCH SECTION
            if (timeNow.Text == "12:00:00 AM")
            {
                txtStatus.Text = "Auto Abolishment Running";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/process/auto_abolishment.php?section=PT");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";

            }

            //QA SECTION
            if (timeNow.Text == "12:10:00 AM")
            {
                txtStatus.Text = "Auto Abolishment Running";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/process/auto_abolishment.php?section=QA");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";

            }

            //PE SECTION
            if (timeNow.Text == "12:20:00 AM")
            {
                txtStatus.Text = "Auto Abolishment Running";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/process/auto_abolishment.php?section=PE");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";

            }

            //TC SECTION
            if (timeNow.Text == "12:30:00 AM")
            {
                txtStatus.Text = "Auto Abolishment Running";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/process/auto_abolishment.php?section=TC");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";

            }

            //IC SECTION
            if (timeNow.Text == "12:40:00 AM")
            {
                txtStatus.Text = "Auto Abolishment Running";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/process/auto_abolishment.php?section=IC");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";

            }

            //AUTO CANCELLATION FOR OPEN DECLINED REQUEST -- ADDED BY LEMY 03/12/25
            if (timeNow.Text == "08:49:00 AM")
            {
                txtStatus.Text = "Auto Cancellation Running";
                Process.Start("http://" + webServer + ":" + portServer + "/pdaus/process/auto_cancellation.php");
                System.Threading.Thread.Sleep(10000);
                txtStatus.Text = "Idle";

            }
        }

        private void testOnly()
        {
            string currentmonth = DateTime.Today.ToString("MM");
            string year = DateTime.Today.ToString("yyyy");
            string transnum = "202506474_DOC";

            string result = transnum.Substring(0, transnum.Length - 4);
            int lastNum = 0;
            string yearNow;
            string monthNow;
            int resultz = Int32.Parse(result);
            lastNum = resultz % 1000 + 1;
            yearNow = transnum.ToString().Substring(0, 4);
            monthNow = transnum.ToString().Substring(4, 2);
            MessageBox.Show(lastNum.ToString());
            MessageBox.Show(resultz.ToString());
        }
    }
}