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
                MessageBox.Show(ex.Message);
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
            
            setState("default");
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

                    break;
            }
           
        }

        

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (validateInput())
            {
                return;
            }
            setState("View");
            MessageBox.Show("Record saved", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setState("View");
            
        }

        private bool validateInput()
        {
            string message = "";
            bool errored = false;

            if (txtCompanyName.Text.Trim().Equals(""))
            {
                message = "Company name can't be empty" + "\r\n";
                txtCompanyName.Focus();
                errored = true;
            }
            if (txtName.Text.Trim().Equals(""))
            {
                message += "Name can't be empty" + "\r\n";
                txtName.Focus();
                errored = true;
            }

            



            if (errored)
            {
                MessageBox.Show(message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            return errored;
        }


        private void frmClosing(object sender, FormClosingEventArgs e)
        {
            connPub.Close();
            connPub.Dispose();
            command.Dispose();
            adapterPub.Dispose();
            tablePub.Dispose();
        }
    }
}
