using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UdemyExercice6
{
    public partial class frmAuthors : Form
    {
        public frmAuthors()
        {
            InitializeComponent();
        }

        OleDbConnection booksConn;
        OleDbCommand authorsComm;
        OleDbDataAdapter authorsAdapter;
        DataTable authorsTable;
        CurrencyManager authorsManager;
        bool dbError = true;

        public string AppState { get; set; }

        OleDbCommandBuilder builderComm;


        private void frmAuthors_Load(object sender, EventArgs e)
        {
            try
            {
                var connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\PROJETOS\UdemyCourse (Alex)\UdemyProject 6\UdemyExercice6\Books.accdb; 
                                Persist Security Info = False; ";
                booksConn = new OleDbConnection(connString);
                booksConn.Open();
                authorsComm = new OleDbCommand("SELECT * from Authors order By Author", booksConn);
                authorsAdapter = new OleDbDataAdapter();
                authorsTable = new DataTable();
                authorsAdapter.SelectCommand = authorsComm;
                authorsAdapter.Fill(authorsTable);

                txtAuthorID.DataBindings.Add("Text", authorsTable, "AU_ID");
                txtAuthorName.DataBindings.Add("Text", authorsTable, "Author");
                txtAuthorBorn.DataBindings.Add("Text", authorsTable, "Year_Born");
                authorsManager = (CurrencyManager)BindingContext[authorsTable];

                setAppState("View");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
  
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            authorsManager.Position--;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            authorsManager.Position++;
        }

        private void frmClosing(object sender, FormClosingEventArgs e)
        {
            if (!dbError)
            {
                booksConn.Close();
                booksConn.Dispose();
                authorsComm.Dispose();
                authorsAdapter.Dispose();
                authorsTable.Dispose();
            }
            
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }
            try
            {
                authorsManager.EndCurrentEdit();
                builderComm = new OleDbCommandBuilder(authorsAdapter);

                if(AppState == "Edit")
                {
                    var authRow = authorsTable.Select("Au_ID = " + txtAuthorID.Text);

                    if (String.IsNullOrEmpty((txtAuthorBorn.Text)))
                        authRow[0]["Year_Born"] = DBNull.Value;
                    else
                        authRow[0]["Year_Born"] = txtAuthorBorn.Text;

                    authorsAdapter.Update(authorsTable);
                    txtAuthorBorn.DataBindings.Add("Text", authorsTable, "Year_Born");
                }
                else
                {
                    var savedRecord = txtAuthorName.Text;
                    authorsTable.DefaultView.Sort = "Author";
                    //when adding a new author it goes to positino 0
                    //and after adding second author it would not allow
                    //because of the Au_ID
                    authorsManager.Position = authorsTable.DefaultView.Find(savedRecord);
                    authorsAdapter.Update(authorsTable);
                }
                
                MessageBox.Show("Record saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                setAppState("View");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving Record", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult response;
            response = MessageBox.Show("Ayre you sure you want do delete this record", "Delete",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if ( response == DialogResult.No)
            {
                return;
            }
            try
            {
                authorsManager.RemoveAt(authorsManager.Position);
                builderComm = new OleDbCommandBuilder(authorsAdapter);
                authorsAdapter.Update(authorsTable);
                AppState = "Delete";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error deleting record", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  
        }

        private void setAppState(string appState)
        {
            switch (appState)
            {
                case "View":
                    
                    txtAuthorName.ReadOnly = true;
                    txtAuthorBorn.ReadOnly = true;
                    btnPrevious.Enabled = true;
                    btnNext.Enabled = true;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    btnAddNew.Enabled = true;
                    btnDelete.Enabled = true;
                    btnDone.Enabled = true;
                    txtAuthorName.TabStop = false;
                    txtAuthorBorn.TabStop = false;

                    break;
                default: //add and edit states
                    
                    txtAuthorName.ReadOnly = false;
                    txtAuthorBorn.ReadOnly = false;
                    btnPrevious.Enabled = false;
                    btnNext.Enabled = false;
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                    btnAddNew.Enabled = false;
                    btnDelete.Enabled = false;
                    btnDone.Enabled = false;
                    txtAuthorName.Focus();
                    break;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            txtAuthorBorn.DataBindings.Clear(); //unbound
            setAppState("Edit");
            AppState = "Edit";

        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                //var count = authorsManager.Count;
                //authorsManager.Position = count++;
                //authorsManager.Position = authorsManager.Count+1;
                
                authorsManager.AddNew();
                
                setAppState("Add");
                AppState = "Add";
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error adding new record", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setAppState("View");
        }

        private void txtAuthorBorn_Keypress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == 8)
            {
                e.Handled = false;
                lblWrongInput.Visible = false;
            }
            else
            {
                e.Handled = true;
                lblWrongInput.Visible = true;
            }
        }

        private bool ValidateInput()
        {
            string message = "";
            int inputYear, currentYear;
            bool allOK = true;
            if (txtAuthorName.Text.Trim().Equals(""))
            {
                message = "Author's name is required" + "\r\n";
                txtAuthorName.Focus();
                allOK = false;
            }
            if (!txtAuthorBorn.Text.Trim().Equals(""))
            {
                inputYear = Convert.ToInt32(txtAuthorBorn.Text);
                currentYear = DateTime.Now.Year;
                if (inputYear >= currentYear)
                {
                    message += "Invalid Year";
                    txtAuthorBorn.Focus();
                    allOK = false;
                }
            }

            if (!allOK)
            {
                MessageBox.Show(message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return allOK;
        }

        private void txtAuthorName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtAuthorBorn.Focus();
            }

        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            authorsManager.Position = 0;
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            authorsManager.Position = authorsManager.Count - 1;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
