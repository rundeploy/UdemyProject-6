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
    public partial class frmPublishers : Form
    {

        OleDbConnection connPub;
        OleDbCommand command;
        OleDbDataAdapter adapterPub;
        DataTable tablePub;
        CurrencyManager managerPub;
        bool connOK = true;
        

        public frmPublishers()
        {
            InitializeComponent();
        }

        private void frmPublishers_Load(object sender, EventArgs e)
        {

            try
            {
                var connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\PROJETOS\UdemyCourse (Alex)\UdemyProject 6\UdemyExercice6\Books.accdb; Persist Security Info=False;";
                connPub = new OleDbConnection(connString);
                connPub.Open();

                command = new OleDbCommand("SELECT * FROM PUBLISHERS", connPub);
                
                adapterPub = new OleDbDataAdapter();
                tablePub = new DataTable();

                adapterPub.SelectCommand = command;
                adapterPub.Fill(tablePub);

                txtPublishersID.DataBindings.Add("Text", tablePub, "PubID");
                txtName.DataBindings.Add("Text", tablePub, "Name");
                txtCompanyName.DataBindings.Add("Text", tablePub, "Company_Name");
                txtAddress.DataBindings.Add("Text", tablePub, "Address");
                txtCity.DataBindings.Add("Text", tablePub, "City");
                txtState.DataBindings.Add("Text", tablePub, "State");
                txtZip.DataBindings.Add("Text", tablePub, "Zip");
                txtTelephone.DataBindings.Add("Text", tablePub, "Telephone");
                txtFax.DataBindings.Add("Text", tablePub, "Fax");
                txtComments.DataBindings.Add("Text", tablePub, "Comments");

                managerPub = (CurrencyManager)BindingContext[tablePub];
                
                setState("View");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connOK = false;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            managerPub.Position++;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            managerPub.Position--;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                setState("Edit");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Editing error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            
        }

        private void setState(String state)
        {
            switch (state)
            {
                case "View":
                    txtPublishersID.ReadOnly = true;
                    txtName.ReadOnly = true;
                    txtCompanyName.ReadOnly = true;
                    txtAddress.ReadOnly = true;
                    txtCity.ReadOnly = true;
                    txtState.ReadOnly = true;
                    txtZip.ReadOnly = true;
                    txtTelephone.ReadOnly = true;
                    txtFax.ReadOnly = true;
                    txtComments.ReadOnly = true;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    btnPrevious.Enabled = true;
                    btnNext.Enabled = true;
                    btnEdit.Enabled = true;
                    btnAdd.Enabled = true;
                    btnDelete.Enabled = true;
                    btnDone.Enabled = true;
                    lblWrongInput.Visible = false;

                    break;

                default:

                    txtPublishersID.ReadOnly = false;
                    txtName.ReadOnly = false;
                    txtCompanyName.ReadOnly = false;
                    txtAddress.ReadOnly = false;
                    txtCity.ReadOnly = false;
                    txtState.ReadOnly = false;
                    txtZip.ReadOnly = false;
                    txtTelephone.ReadOnly = false;
                    txtFax.ReadOnly = false;
                    txtComments.ReadOnly = false;
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                    btnPrevious.Enabled = false;
                    btnNext.Enabled = false;
                    btnEdit.Enabled = false;
                    btnAdd.Enabled = false;
                    btnDelete.Enabled = false;
                    btnDone.Enabled = false;
                    txtName.Focus();

                    break;
            }
        }

        

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!validInput())
            {
                return;
            }

            try
            {
                MessageBox.Show("Record saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                setState("View");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setState("View");
            
        }

        private bool validInput()
        {
            string message = "";
            bool isValid = true;

            if (txtCompanyName.Text.Trim().Equals(""))
            {
                message = "Company name can't be empty" + "\r\n";
                txtCompanyName.Focus();
                isValid = false;
            }
            if (txtName.Text.Trim().Equals(""))
            {
                message += "Name can't be empty" + "\r\n";
                txtName.Focus();
                isValid = false;
            }

            
            if (!isValid)
            {
                MessageBox.Show(message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            return isValid;
        }


       

        private void txtTelephone_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == 8 || e.KeyChar == 13)
            {
                e.Handled = false;
                lblWrongInput.Visible = false;

                txtInput_KeyPress( sender,  e);
            }
            else
            {
                e.Handled = true;
                lblWrongInput.Visible = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                setState("Add");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Adding record error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult response;
            response = MessageBox.Show("Are you sure you want to delete this record?", "Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (response == DialogResult.No)
            {
                return;
            }
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error deleting record", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if(e.KeyChar == 13)
            {
                switch (textBox.Name)
                {
                    case "txtName":
                        txtCompanyName.Focus();
                        break;
                    case "txtCompanyName":
                        txtAddress.Focus();
                        break;
                    case "txtAddress":
                        txtCity.Focus();
                        break;
                    case "txtCity":
                        txtState.Focus();
                        break;
                    case "txtState":
                        txtZip.Focus();
                        break;
                    case "txtZip":
                        txtTelephone.Focus();
                        break;
                    case "txtTelephone":
                        txtFax.Focus();
                        break; ;
                    case "txtFax":
                        txtComments.Focus();
                        break; ;
                    case "txtComments":
                        btnSave.Focus();
                        break;

                }
            }
        }

        private void frmClosing(object sender, FormClosingEventArgs e)
        {
            if (connOK)
            {
                connPub.Close();
                connPub.Dispose();
                command.Dispose();
                adapterPub.Dispose();
                tablePub.Dispose();
            }

        }
    }
}
