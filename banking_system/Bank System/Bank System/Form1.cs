using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bank_System
{
    public partial class Form1 : Form
    {
        static string connectionString = "Server=localhost\\SQLEXPRESS; Database=BankDB; Integrated Security=True";
        SqlConnection con = new SqlConnection(connectionString);
        private void loadData(string query, DataGridView d)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adap.Fill(dt);
            d.DataSource = dt;
            con.Close();
        }
        private int executeQuery(string query, RichTextBox lbl)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                int rows = cmd.ExecuteNonQuery();
                con.Close();
                return rows;
            }
            catch (Exception e)
            {
                string t = e.Message;
                if (t.Contains("NULL"))
                {
                    int indStart = t.IndexOf('\''), indEnd = t.IndexOf(',');
                    t = "The attribute " + t.Substring(indStart + 1, indEnd - indStart - 2).ToLower() + " does not accept a NULL value.";
                }
                else if (t.Contains("PRIMARY KEY"))
                {
                    t = "The table already has a record with the same primary key.";
                }
                else if (t.Contains("Invalid column name"))
                {
                    int indStart = t.IndexOf('\''), indEnd = t.IndexOf('.');
                    t = "The value \"" + t.Substring(indStart + 1, indEnd - indStart - 2).ToLower() + "\" is not a valid value for its field.";
                }
                else if (t.Contains("String or binary data would be truncated"))
                {
                    int indStart1 = t.IndexOf("column '"), indEnd1 = t.IndexOf("'."), indStart2 = t.IndexOf(':'), indEnd2 = t.LastIndexOf('\'');
                    t = "The value you entered for " + t.Substring(indStart1 + 8, indEnd1 - indStart1 - 8).ToLower() + " is too long. The maximum length is " + (indEnd2 - indStart2 - 3).ToString() + " characters.";
                }
                else if (t.Contains("FOREIGN KEY"))
                {
                    int indStart = t.LastIndexOf('_'), indEnd = t.IndexOf('.');
                    t = "This " + t.Substring(indStart + 1, indEnd - indStart - 2).ToLower() + " does not exist.";
                }
                else if (t.Contains("REFERENCE"))
                {
                    int indStart = t.IndexOf("dbo."), indEnd = t.LastIndexOf('"');
                    t = "This record cannot be modifed because there is a " + t.Substring(indStart+4, indEnd-indStart-4).ToLower() + " that depends on it.";
                }
                lbl.Text = t;
                con.Close();
                return -1;
            }
        }
        private void performInsert(string table, string[] values, bool[] hasQuotes, int n, DataGridView d, RichTextBox lbl)
        {
            string query = "INSERT INTO " + table + " VALUES (";
            for (int i = 0; i < n; i++)
            {
                if (values[i] == "") query += "NULL";
                else if (hasQuotes[i]) query += "'" + values[i] + "'";
                else query += values[i];
                if (i != n - 1) query += ",";
            }
            query += ");";
            int rows = executeQuery(query, lbl);
            if (rows == -1) lbl.Text += "\nInsertion failed.";
            else lbl.Text = "Insertion successful!";
            loadData("SELECT * FROM " + table, d);
        }
        private void performUpdate(string table, string p1, string p2, string v1, string v2, bool q1, bool q2, string op, DataGridView d, RichTextBox lbl)
        {
            if (v1 == "") v1 = "NULL";
            else if (q1) v1 = "'" + v1 + "'";
            if (v2 == "") v2 = "NULL";
            else if (q2) v2 = "'" + v2 + "'";
            string query = "UPDATE " + table + " SET " + p2 + "=" + v2 + " WHERE " + p1 + op + v1;
            int rows = executeQuery(query, lbl);
            if (rows == -1) lbl.Text += "\nUpdate failed.";
            else if (rows == 0) lbl.Text = "No records were updated.";
            else lbl.Text = "Update successful! " + rows.ToString() + " records were updated.";
            loadData("SELECT * FROM " + table, d);
        }
        private void performDelete(string table, string p, string v, bool q, string op, DataGridView d, RichTextBox lbl)
        {
            if (v == "") v = "NULL";
            else if (q) v = "'" + v + "'";
            string query = "DELETE FROM " + table + " WHERE " + p + op + v;
            int rows = executeQuery(query, lbl);
            if (rows == -1) lbl.Text += "\nDeletion failed.";
            else if (rows == 0) lbl.Text = "No records were deleted.";
            else lbl.Text = "Deletion successful! " + rows.ToString() + " records were deleted.";
            loadData("SELECT * FROM " + table, d);
        }
        private void loadAllTables()
        {
            loadData("SELECT * FROM BANK", dataBank);
            loadData("SELECT * FROM BRANCH", dataBranch);
            loadData("SELECT * FROM LOAN", dataLoan);
            loadData("SELECT * FROM CUSTOMER", dataCustomer);
            loadData("SELECT * FROM ACCOUNT", dataAccount);
            loadData("SELECT * FROM BORROWS", dataBorrows);
            loadData("SELECT * FROM HAS", dataHas);
        }
        public Form1()
        {
            InitializeComponent();
            loadAllTables();
            txtBankUpdateP1.SelectedIndex = txtBankUpdateP2.SelectedIndex = txtBankUpdateO.SelectedIndex = txtBankDeleteP.SelectedIndex = txtBankDeleteO.SelectedIndex = 0;
            txtBranchUpdateP1.SelectedIndex = txtBranchUpdateP2.SelectedIndex = txtBranchUpdateO.SelectedIndex = txtBranchDeleteP.SelectedIndex = txtBranchDeleteO.SelectedIndex = 0;
            txtLoanUpdateP1.SelectedIndex = txtLoanUpdateP2.SelectedIndex = txtLoanUpdateO.SelectedIndex = txtLoanDeleteP.SelectedIndex = txtLoanDeleteO.SelectedIndex = 0;
            txtCustomerUpdateP1.SelectedIndex = txtCustomerUpdateP2.SelectedIndex = txtCustomerUpdateO.SelectedIndex = txtCustomerDeleteP.SelectedIndex = txtCustomerDeleteO.SelectedIndex = 0;
            txtAccountUpdateP1.SelectedIndex = txtAccountUpdateP2.SelectedIndex = txtAccountUpdateO.SelectedIndex = txtAccountDeleteP.SelectedIndex = txtAccountDeleteO.SelectedIndex = 0;
            txtBorrowsUpdateP1.SelectedIndex = txtBorrowsUpdateP2.SelectedIndex = txtBorrowsUpdateO.SelectedIndex = txtBorrowsDeleteP.SelectedIndex = txtBorrowsDeleteO.SelectedIndex = 0;
            txtHasUpdateP1.SelectedIndex = txtHasUpdateP2.SelectedIndex = txtHasUpdateO.SelectedIndex = txtHasDeleteP.SelectedIndex = txtHasDeleteO.SelectedIndex = 0;
            txtBankUpdateP2.SelectedIndex = 2;
        }
        private void btnBankInsert_Click(object sender, EventArgs e)
        {
            string[] args = { txtBankBankCode.Text, txtBankBankName.Text, txtBankAddress.Text };
            bool[] q = {false, true, true};
            performInsert("BANK", args, q, 3, dataBank, lblBankMessage);
        }
        private void btnBankUpdate_Click(object sender, EventArgs e)
        {
            performUpdate("BANK", txtBankUpdateP1.SelectedItem.ToString(), txtBankUpdateP2.SelectedItem.ToString(), txtBankUpdateV1.Text, txtBankUpdateV2.Text, 
                txtBankUpdateP1.SelectedIndex != 0, txtBankUpdateP2.SelectedIndex != 0, txtBankUpdateO.SelectedItem.ToString(), dataBank, lblBankMessage);
        }
        private void btnBankDelete_Click(object sender, EventArgs e)
        {
            performDelete("BANK", txtBankDeleteP.SelectedItem.ToString(), txtBankDeleteV.Text, txtBankDeleteP.SelectedIndex != 0, txtBankDeleteO.SelectedItem.ToString(), dataBank, lblBankMessage);
        }
        private void btnBranchInsert_Click(object sender, EventArgs e)
        {
            string[] args = { txtBranchBankCode.Text, txtBranchBranchNumber.Text, txtBranchAddress.Text };
            bool[] q = { false, false, true };
            performInsert("BRANCH", args, q, 3, dataBranch, lblBranchMessage);
        }
        private void btnBranchUpdate_Click(object sender, EventArgs e)
        {
            performUpdate("BRANCH", txtBranchUpdateP1.SelectedItem.ToString(), txtBranchUpdateP2.SelectedItem.ToString(), txtBranchUpdateV1.Text, txtBranchUpdateV2.Text,
                txtBranchUpdateP1.SelectedIndex == 2, txtBranchUpdateP2.SelectedIndex == 2, txtBranchUpdateO.SelectedItem.ToString(), dataBranch, lblBranchMessage);
        }
        private void btnBranchDelete_Click(object sender, EventArgs e)
        {
            performDelete("BRANCH", txtBranchDeleteP.SelectedItem.ToString(), txtBranchDeleteV.Text, txtBranchDeleteP.SelectedIndex == 2, txtBranchDeleteO.SelectedItem.ToString(), dataBranch, lblBranchMessage);
        }
        private void btnLoanInsert_Click(object sender, EventArgs e)
        {
            string[] args = { txtLoanBankCode.Text, txtLoanBranchNumber.Text, txtLoanLoanNumber.Text, txtLoanLoanType.Text };
            bool[] q = { false, false, false, true };
            performInsert("LOAN", args, q, 4, dataLoan, lblLoanMessage);
        }
        private void btnLoanUpdate_Click(object sender, EventArgs e)
        {
            performUpdate("LOAN", txtLoanUpdateP1.SelectedItem.ToString(), txtLoanUpdateP2.SelectedItem.ToString(), txtLoanUpdateV1.Text, txtLoanUpdateV2.Text,
                    txtLoanUpdateP1.SelectedIndex == 3, txtLoanUpdateP2.SelectedIndex == 3, txtLoanUpdateO.SelectedItem.ToString(), dataLoan, lblLoanMessage);
        }
        private void btnLoanDelete_Click(object sender, EventArgs e)
        {
            performDelete("LOAN", txtLoanDeleteP.SelectedItem.ToString(), txtLoanDeleteV.Text, txtLoanDeleteP.SelectedIndex == 3, txtLoanDeleteO.SelectedItem.ToString(), dataLoan, lblLoanMessage);
        }
        private void btnCustomerInsert_Click(object sender, EventArgs e)
        {
            string[] args = { txtCustomerSSN.Text, txtCustomerName.Text, txtCustomerPhone.Text, txtCustomerAddress.Text };
            bool[] q = { false, true, true, true };
            performInsert("CUSTOMER", args, q, 4, dataCustomer, lblCustomerMessage);
        }
        private void btnCustomerUpdate_Click(object sender, EventArgs e)
        {
            performUpdate("CUSTOMER", txtCustomerUpdateP1.SelectedItem.ToString(), txtCustomerUpdateP2.SelectedItem.ToString(), txtCustomerUpdateV1.Text, txtCustomerUpdateV2.Text,
                    txtCustomerUpdateP1.SelectedIndex != 0, txtCustomerUpdateP2.SelectedIndex != 0, txtCustomerUpdateO.SelectedItem.ToString(), dataCustomer, lblCustomerMessage);
        }
        private void btnCustomerDelete_Click(object sender, EventArgs e)
        {
            performDelete("CUSTOMER", txtCustomerDeleteP.SelectedItem.ToString(), txtCustomerDeleteV.Text, txtCustomerDeleteP.SelectedIndex != 0, txtCustomerDeleteO.SelectedItem.ToString(), dataCustomer, lblCustomerMessage);
        }
        private void btnAccountInsert_Click(object sender, EventArgs e)
        {
            string[] args = { txtAccountNumber.Text, txtAccountSSN.Text, txtAccountType.Text, txtAccountBalance.Text };
            bool[] q = { false, false, true, false };
            performInsert("ACCOUNT", args, q, 4, dataAccount, lblAccountMessage);
        }
        private void btnAccountUpdate_Click(object sender, EventArgs e)
        {
            performUpdate("ACCOUNT", txtAccountUpdateP1.SelectedItem.ToString(), txtAccountUpdateP2.SelectedItem.ToString(), txtAccountUpdateV1.Text, txtAccountUpdateV2.Text,
                    txtAccountUpdateP1.SelectedIndex == 2, txtAccountUpdateP2.SelectedIndex == 2, txtAccountUpdateO.SelectedItem.ToString(), dataAccount, lblAccountMessage);
        }
        private void btnAccountDelete_Click(object sender, EventArgs e)
        {
            performDelete("ACCOUNT", txtAccountDeleteP.SelectedItem.ToString(), txtAccountDeleteV.Text, txtAccountDeleteP.SelectedIndex == 2, txtAccountDeleteO.SelectedItem.ToString(), dataAccount, lblAccountMessage);
        }
        private void btnBorrowsInsert_Click(object sender, EventArgs e)
        {
            string[] args = { txtBorrowsBankCode.Text, txtBorrowsBranchNumber.Text, txtBorrowsLoanNumber.Text, txtBorrowsSSN.Text, txtBorrowsAmount.Text, txtBorrowsStartDate.Text };
            bool[] q = { false, false, false, false, false, true };
            performInsert("BORROWS", args, q, 6, dataBorrows, lblBorrowsMessage);
        }
        private void btnBorrowsUpdate_Click(object sender, EventArgs e)
        {
            performUpdate("BORROWS", txtBorrowsUpdateP1.SelectedItem.ToString(), txtBorrowsUpdateP2.SelectedItem.ToString(), txtBorrowsUpdateV1.Text, txtBorrowsUpdateV2.Text,
                    txtBorrowsUpdateP1.SelectedIndex == 5, txtBorrowsUpdateP2.SelectedIndex == 5, txtBorrowsUpdateO.SelectedItem.ToString(), dataBorrows, lblBorrowsMessage);
        }
        private void btnBorrowsDelete_Click(object sender, EventArgs e)
        {
            performDelete("BORROWS", txtBorrowsDeleteP.SelectedItem.ToString(), txtBorrowsDeleteV.Text, txtBorrowsDeleteP.SelectedIndex == 5, txtBorrowsDeleteO.SelectedItem.ToString(), dataBorrows, lblBorrowsMessage);
        }
        private void btnHasInsert_Click(object sender, EventArgs e)
        {
            string[] args = { txtHasBankCode.Text, txtHasBranchNumber.Text, txtHasSSN.Text };
            bool[] q = { false, false, false };
            performInsert("HAS", args, q, 3, dataHas, lblHasMessage);
        }
        private void btnHasUpdate_Click(object sender, EventArgs e)
        {
            performUpdate("HAS", txtHasUpdateP1.SelectedItem.ToString(), txtHasUpdateP2.SelectedItem.ToString(), txtHasUpdateV1.Text, txtHasUpdateV2.Text,
                false, false, txtHasUpdateO.SelectedItem.ToString(), dataHas, lblHasMessage);
        }
        private void btnHasDelete_Click(object sender, EventArgs e)
        {
            performDelete("HAS", txtHasDeleteP.SelectedItem.ToString(), txtHasDeleteV.Text, false, txtHasDeleteO.SelectedItem.ToString(), dataHas, lblHasMessage);
        }
        private void resetDatabase()
        {
            DialogResult dialogResult = MessageBox.Show("This will restore all the tables in the database to their original form. Any records that were altered will " +
                "be restored and any records that were added will be deleted.\n\nAre you sure you want to continue?", "WARNING", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                con.Open();
                string query = "DELETE FROM HAS; DELETE FROM BORROWS; DELETE FROM ACCOUNT; DELETE FROM LOAN; DELETE FROM CUSTOMER; DELETE FROM BRANCH; DELETE FROM BANK; insert into bank VALUES(1, 'Al-Ahly', 'Cairo'),(2, 'QNB', 'Giza'),(3, 'Al-Qahira', 'Alexandria'),(4, 'HSBC', 'Cairo'),(5, 'CIB', 'Giza');insert into branch VALUES(1, 1, 'Zamalek'),(2, 1, 'Al-Ahram'),(3, 1, 'Sidi Bishr'),(4, 1, 'Mohandessin'),(5, 1, 'Al-Ahram'),(1, 2, 'New Cairo'),(2, 2, '6th of October City'),(3, 2, 'Bahary'),(1, 3, 'Nasr City');insert into customer VALUES(1, 'Adham', '01012345678', '6th of October City'),(2, 'Ahmed', '01117140568', '6th of October City'),(3, 'Yahya', '01228743294', 'Alexandria'),(4, 'Afnan', '01538234989', 'Al-Haram'),(5, 'Abdullah', '01027318281', 'Bolak Ad-Dakrour'),(6, 'Mohamed', '01138721897', 'Mohandessin'),(7, 'Ali', '01198217298', 'The 5th Settlement'),(8, 'Hassan', '01098212345', 'Nasr City'),(9, 'Khaled', '01234567890', 'Maadi'),(10, 'Omar', '01123456789', 'Heliopolis'),(11, 'Hana', '01512345678', 'Zamalek'),(12, 'Sara', '01011112222', 'Garden City'),(13, 'Tamer', '01222223333', 'Downtown'),(14, 'Noor', '01133334444', 'Dokki'),(15, 'Laila', '01544445555', 'Shubra'),(16, 'Mona', '01055556666', 'Mokattam'),(17, 'Ayman', '01166667777', 'Ain Shams'),(18, 'Farah', '01277778888', 'Giza'),(19, 'Ziad', '01588889999', 'Obour City'),(20, 'Rana', '01099990000', '6th of October City'),(21, 'Basel', '01200001111', 'Sheikh Zayed'),(22, 'Nadine', '01111112222', 'New Cairo'),(23, 'Salma', '01522223333', 'Al Rehab City');INSERT INTO loan (bankcode, branchnumber, loannumber, type) VALUES(1, 1, 1, 'Personal Loan'),(1, 2, 1, 'Personal Loan'),(1, 2, 2, 'Business Loan'),(1, 3, 1, 'Home Loan'),(1, 3, 2, 'Car Loan'),(2, 1, 1, 'Personal Loan'),(2, 1, 2, 'Home Loan'),(2, 2, 1, 'Car Loan'),(2, 2, 2, 'Education Loan'),(2, 2, 3, 'Personal Loan'),(3, 1, 1, 'Business Loan'),(3, 1, 2, 'Home Loan'),(3, 1, 3, 'Education Loan'),(3, 2, 1, 'Personal Loan'),(4, 1, 1, 'Personal Loan'),(4, 1, 2, 'Home Loan'),(4, 1, 3, 'Education Loan'),(5, 1, 1, 'Car Loan'),(5, 1, 2, 'Business Loan');INSERT INTO account (accountnumber, ssn, type, balance) VALUES(1, 1, 'checking', 3218.66),(2, 1, 'savings', 148.71),(3, 2, 'checking', 5021.45),(4, 2, 'savings', 210.89),(5, 3, 'checking', 1450.00),(6, 3, 'investment', 10000.00),(7, 4, 'savings', 560.23),(8, 5, 'checking', 3250.12),(9, 5, 'investment', 7580.55),(10, 6, 'checking', 1800.75),(11, 6, 'savings', 300.40),(12, 6, 'business', 15000.00),(13, 7, 'savings', 820.50),(14, 8, 'checking', 1220.75),(15, 9, 'checking', 670.89),(16, 9, 'savings', 400.10),(17, 9, 'investment', 12500.75),(18, 10, 'checking', 2750.30),(19, 10, 'savings', 600.45),(20, 11, 'savings', 920.80),(21, 12, 'checking', 3050.10),(22, 12, 'investment', 7000.00),(23, 13, 'business', 20000.00),(24, 14, 'checking', 4120.55),(25, 15, 'savings', 170.25),(26, 15, 'investment', 3500.00),(27, 16, 'checking', 2230.70),(28, 17, 'savings', 510.55),(29, 18, 'checking', 1100.00),(30, 19, 'savings', 250.75),(31, 20, 'checking', 2950.85),(32, 21, 'savings', 375.40),(33, 21, 'business', 12500.00),(34, 22, 'checking', 3850.90),(35, 23, 'savings', 180.60),(36, 23, 'investment', 1500.00);INSERT INTO borrows VALUES(1, 1, 1, 1, 8000, '2024-05-01'),(2, 1, 1, 1, 10000, '2024-03-15'),(1, 2, 2, 2, 8000, '2024-05-01'),(2, 1, 1, 2, 10000, '2024-03-15'),(1, 2, 2, 3, 7500, '2023-07-10'),(2, 2, 3, 4, 12000, '2024-01-05'),(5, 1, 1, 4, 9000, '2023-11-20'),(1, 2, 1, 7, 8000, '2024-02-25'),(2, 1, 2, 7, 11000, '2023-12-30'),(3, 2, 1, 8, 9500, '2023-10-01'),(3, 1, 2, 8, 9500, '2023-10-01'),(2, 2, 1, 8, 9500, '2023-10-01'),(3, 1, 3, 11, 13000, '2024-03-20'),(2, 1, 2, 13, 10500, '2024-02-28'),(3, 1, 2, 13, 8300, '2023-12-12'),(4, 1, 3, 14, 8800, '2024-03-30'),(2, 1, 2, 17, 10500, '2024-02-28'),(3, 1, 2, 18, 8300, '2023-12-12'),(4, 1, 3, 20, 8800, '2024-03-30'),(5, 1, 2, 20, 9200, '2023-04-22'),(1, 2, 2, 22, 11000, '2024-05-14'),(2, 2, 2, 22, 9500, '2023-11-07'),(5, 1, 1, 23, 9200, '2023-04-22');INSERT INTO has VALUES(1, 1, 1),(4, 1, 1),(5, 1, 1),(1, 2, 1),(2, 2, 1),(3, 2, 1),(1, 3, 1),(1, 1, 2),(4, 1, 2),(5, 1, 2),(1, 2, 2),(3, 2, 2),(1, 3, 2),(1, 1, 3),(4, 1, 3),(5, 1, 3),(1, 2, 3),(2, 2, 3),(3, 2, 3),(2, 1, 4),(3, 1, 4),(4, 1, 4),(5, 1, 4),(1, 2, 4),(2, 2, 4),(1, 3, 4),(1, 1, 5),(2, 1, 5),(5, 1, 5),(3, 2, 5),(1, 3, 5),(1, 1, 6),(2, 1, 6),(5, 1, 6),(1, 2, 6),(2, 2, 6),(2, 1, 7),(3, 1, 7),(4, 1, 7),(5, 1, 7),(1, 2, 7),(2, 2, 7),(2, 1, 8),(3, 1, 8),(4, 1, 8),(3, 2, 8),(1, 3, 8),(4, 1, 9),(5, 1, 9),(1, 2, 9),(2, 2, 9),(3, 2, 9),(1, 3, 9),(1, 1, 10),(3, 1, 10),(5, 1, 10),(1, 2, 10),(3, 2, 10),(1, 3, 10),(1, 1, 11),(5, 1, 11),(1, 2, 11),(2, 2, 11),(1, 1, 12),(2, 1, 12),(5, 1, 12),(1, 2, 12),(2, 2, 12),(1, 1, 13),(2, 1, 13),(1, 2, 13),(2, 2, 13),(1, 1, 14),(2, 1, 14),(3, 1, 14),(1, 2, 14),(1, 3, 14),(1, 1, 15),(2, 1, 15),(3, 1, 15),(2, 2, 15),(3, 2, 15),(2, 1, 16),(3, 1, 16),(1, 2, 16),(2, 2, 16),(3, 2, 16),(1, 3, 16),(1, 1, 17),(2, 1, 17),(5, 1, 17),(1, 2, 17),(2, 2, 17),(3, 2, 17),(2, 1, 18),(3, 1, 18),(4, 1, 18),(5, 1, 18),(1, 2, 18),(1, 1, 19),(2, 1, 19),(5, 1, 19),(3, 2, 19),(1, 3, 19),(1, 1, 20),(2, 1, 20),(3, 1, 20),(1, 1, 21),(2, 1, 21),(3, 1, 21),(2, 2, 21),(3, 2, 21),(1, 3, 21),(1, 1, 22),(5, 1, 22),(1, 2, 22),(1, 3, 22),(1, 1, 23),(2, 1, 23),(3, 1, 23),(2, 2, 23),(3, 2, 23),(1, 3, 23);"; SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
                loadAllTables();
            }

        }
        private void btnBankRestore_Click(object sender, EventArgs e)
        {
            resetDatabase();
        }
        private void btnBranchRestore_Click(object sender, EventArgs e)
        {
            resetDatabase();
        }
        private void btnLoanRestore_Click(object sender, EventArgs e)
        {
            resetDatabase();
        }
        private void btnCustomerRestore_Click(object sender, EventArgs e)
        {
            resetDatabase();
        }
        private void btnAccountRestore_Click(object sender, EventArgs e)
        {
            resetDatabase();
        }
        private void btnBorrowRestore_Click(object sender, EventArgs e)
        {
            resetDatabase();
        }
        private void btnHasRestore_Click(object sender, EventArgs e)
        {
            resetDatabase();
        }
        private void btnReportsMostBorrowed_Click(object sender, EventArgs e)
        {
            string query = "SELECT LOAN.TYPE AS [Loan Type], COUNT(BORROWS.SSN) AS [Number of Current Borrowers] FROM BORROWS RIGHT JOIN " +
                "LOAN ON BORROWS.BANKCODE = LOAN.BANKCODE AND BORROWS.BRANCHNUMBER=LOAN.BRANCHNUMBER AND " +
                "BORROWS.LOANNUMBER=LOAN.LOANNUMBER GROUP BY LOAN.TYPE ORDER BY [Number of Current Borrowers] DESC";
            loadData(query, dataReports);
        }
        private void btnReportsRichestCustomers_Click(object sender, EventArgs e)
        {
            string query = "SELECT CUSTOMER.NAME AS [Customer], ISNULL(SUM(BALANCE), 0) AS [Total Balance] FROM ACCOUNT RIGHT JOIN CUSTOMER " +
                "ON ACCOUNT.SSN=CUSTOMER.SSN GROUP BY CUSTOMER.SSN, CUSTOMER.NAME ORDER BY SUM(BALANCE) DESC";
            loadData(query, dataReports);
        }
        private void btnReportsNumOfCustomers_Click(object sender, EventArgs e)
        {
            string query = "SELECT BANK.NAME AS [Bank Name], BRANCH.ADDRESS AS [Branch Address], COUNT(HAS.SSN) AS [Number of Customers] " +
                "FROM BANK JOIN BRANCH ON BANK.BANKCODE = BRANCH.BANKCODE LEFT JOIN HAS ON BANK.BANKCODE = HAS.BANKCODE AND BRANCH.BRANCHNUMBER = HAS.BRANCHNUMBER " +
                "GROUP BY BANK.BANKCODE, BRANCH.BRANCHNUMBER, BANK.NAME, BRANCH.ADDRESS ORDER BY [Number of Customers] DESC";
            loadData(query, dataReports);
        }
        private void btnReportsTotalLoan_Click(object sender, EventArgs e)
        {
            string query = "SELECT CUSTOMER.NAME AS [Customer], ISNULL(SUM(AMOUNT),0) AS Total FROM CUSTOMER LEFT JOIN BORROWS " +
                "ON CUSTOMER.SSN = BORROWS.SSN GROUP BY CUSTOMER.SSN, CUSTOMER.NAME ORDER BY Total DESC";
            loadData(query, dataReports);
        }
        private void btnReportsEarliestLoans_Click(object sender, EventArgs e)
        {
            string query = "SELECT B1.NAME AS [Bank Name], BR1.ADDRESS AS [Branch Address], LOAN.TYPE AS [Loan Type], CUSTOMER.NAME AS [Customer Name], START_DATE AS [Start Date] FROM BORROWS AS BO1, CUSTOMER, LOAN, BANK AS B1, " +
                "BRANCH AS BR1 WHERE B1.BANKCODE=BR1.BANKCODE AND BR1.BANKCODE=LOAN.BANKCODE AND BR1.BRANCHNUMBER=LOAN.BRANCHNUMBER AND " +
                "BO1.BANKCODE=B1.BANKCODE AND BO1.BRANCHNUMBER=BR1.BRANCHNUMBER AND BO1.LOANNUMBER=LOAN.LOANNUMBER AND BO1.SSN = CUSTOMER.SSN AND EXISTS" +
                "(SELECT BANKCODE, BRANCHNUMBER, MIN(START_DATE) FROM BORROWS AS B2 GROUP BY BANKCODE, BRANCHNUMBER HAVING " +
                "MIN(START_DATE)=BO1.START_DATE AND BR1.BANKCODE = B2.BANKCODE AND BR1.BRANCHNUMBER=B2.BRANCHNUMBER)";
            loadData(query, dataReports);
        }
        private void btnReportsHighestLoans_Click(object sender, EventArgs e)
        {
            string query = "SELECT B1.NAME AS [Bank Name], BR1.ADDRESS AS [Branch Address], LOAN.TYPE AS [Loan Type], CUSTOMER.NAME AS [Customer Name], BO1.AMOUNT AS [Amount Borrowed] FROM BORROWS AS BO1, CUSTOMER, LOAN, BANK AS B1, " +
                "BRANCH AS BR1 WHERE B1.BANKCODE=BR1.BANKCODE AND BR1.BANKCODE=LOAN.BANKCODE AND BR1.BRANCHNUMBER=LOAN.BRANCHNUMBER AND " +
                "BO1.BANKCODE=B1.BANKCODE AND BO1.BRANCHNUMBER=BR1.BRANCHNUMBER AND BO1.LOANNUMBER=LOAN.LOANNUMBER AND BO1.SSN = CUSTOMER.SSN AND " +
                "EXISTS (SELECT BANKCODE, BRANCHNUMBER, MAX(AMOUNT) FROM BORROWS AS B2 GROUP BY BANKCODE, BRANCHNUMBER HAVING MAX(AMOUNT)=BO1.AMOUNT " +
                "AND BR1.BANKCODE = B2.BANKCODE AND BR1.BRANCHNUMBER=B2.BRANCHNUMBER)";
            loadData(query, dataReports);
        }
    }
}
