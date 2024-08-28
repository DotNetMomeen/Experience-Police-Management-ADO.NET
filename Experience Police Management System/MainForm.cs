using Experience_Police_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Experience_Police_Management_System
{
    public partial class MainForm : Form
    {
        string conStr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        int intPoliceId = 0;
        string strPreviousImage = "";
        bool defaultImage = true;
        OpenFileDialog ofd = new OpenFileDialog();

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadAllPoliceList();
            LoadAllDesignationCmb();
            Clear();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

            ofd.Filter = "Images(.jpg,.png,.png)|*.png;*.jpg; *.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBoxPolice.Image = new Bitmap(ofd.FileName);
                if (intPoliceId == 0)
                {
                    defaultImage = false;
                    strPreviousImage = "";
                }

            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            pictureBoxPolice.Image = new Bitmap(Application.StartupPath + "DefaultImage.png");
            defaultImage = true;
            strPreviousImage = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateMasterDetailForm())
            {
                int empId = 0;
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("PoliceAddOrEdit", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PoliceId", intPoliceId);
                    cmd.Parameters.AddWithValue("@PoliceCode", txtPoliceCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@PoliceName", txtPoliceName.Text.Trim());
                    cmd.Parameters.AddWithValue("@DesignationId", Convert.ToInt16(cmbDesignation.SelectedValue));
                    cmd.Parameters.AddWithValue("@DateOfBirth", DateOfBirth.Value);
                    cmd.Parameters.AddWithValue("@IsPermanent", CheckStatus.Checked ? "True" : "False");
                    cmd.Parameters.AddWithValue("@Gender", rbtnMale.Checked ? "Male" : "Female");
                    if (defaultImage)
                    {
                        cmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    }

                    else if (intPoliceId > 0 && strPreviousImage != "")
                    {
                        cmd.Parameters.AddWithValue("@ImagePath", strPreviousImage);
                        if (ofd.FileName != strPreviousImage)
                        {
                            var filename = Application.StartupPath + "Images" + strPreviousImage;
                            if (pictureBoxPolice.Image != null)
                            {
                                pictureBoxPolice.Image.Dispose();
                                pictureBoxPolice.Image = null;
                                System.IO.File.Delete(filename);
                            }
                        }

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ImagePath", SaveImage(ofd.FileName));
                    }
                    empId = Convert.ToInt16(cmd.ExecuteScalar());
                }
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    foreach (DataGridViewRow item in dgvExperiences.Rows)
                    {
                        if (item.IsNewRow) break;
                        else
                        {
                            SqlCommand cmd = new SqlCommand("PoliceExperienceAddAndEdit", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ExperienceId", Convert.ToInt32(item.Cells["dgvExperienceId"].Value == DBNull.Value ? "0" : item.Cells["dgvExperienceId"].Value));
                            cmd.Parameters.AddWithValue("@PoliceId", empId);
                            cmd.Parameters.AddWithValue("@PoliceStationName", item.Cells["dgvCompanyName"].Value);
                            cmd.Parameters.AddWithValue("@YearsWorked", item.Cells["dgvYearsWorked"].Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                LoadAllPoliceList();
                Clear();
                MessageBox.Show("Submitted Successfully");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete this record?", "Master Details", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string image = "";
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter("ViewPoliceByPoliceId", con);
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sda.SelectCommand.Parameters.AddWithValue("@PoliceId", intPoliceId);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    DataRow dr = ds.Tables[0].Rows[0];
                    if (dr["ImagePath"] != DBNull.Value)
                    {
                        image = dr["ImagePath"].ToString();
                        var filename = Application.StartupPath + "Images" + image;
                        if (pictureBoxPolice.Image != null)
                        {
                            pictureBoxPolice.Image.Dispose();
                            pictureBoxPolice.Image = null;
                            System.IO.File.Delete(filename);
                        }

                    }
                    SqlCommand cmd = new SqlCommand("PoliceExperienceDelete", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PoliceId", intPoliceId);
                    sda.Dispose();
                    cmd.ExecuteNonQuery();
                    LoadAllPoliceList();
                    Clear();
                    MessageBox.Show("Deleted Successfully");
                }
                // File.Delete(filePath);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("ViewAllPolice", con);
                sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sda.Fill(dt);

                List<PoliceViewModel> PoliceList = new List<PoliceViewModel>();
                PoliceViewModel PoliceVM;

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        PoliceVM = new PoliceViewModel(PoliceList);
                        PoliceVM.PoliceId = Convert.ToInt32(dt.Rows[i]["PoliceId"]);
                        PoliceVM.PoliceCode = dt.Rows[i]["PoliceCode"].ToString();
                        PoliceVM.PoliceName = dt.Rows[i]["PoliceName"].ToString();
                        PoliceVM.DateOfBirth = Convert.ToDateTime(dt.Rows[i]["DateOfBirth"].ToString());
                        PoliceVM.Gender = dt.Rows[i]["Gender"].ToString();
                        PoliceVM.IsPermanent = Convert.ToBoolean(dt.Rows[i]["IsPermanent"].ToString());
                        PoliceVM.TotalExperience = Convert.ToInt32(dt.Rows[i]["TotalExperience"]);
                        PoliceVM.DesignationTitle = dt.Rows[i]["DesignationTitle"].ToString();
                        PoliceVM.ImagePath = Application.StartupPath + "Images" + dt.Rows[i]["ImagePath"].ToString();
                        PoliceList.Add(PoliceVM);

                    }


                    using (PoliceReport report = new PoliceReport(PoliceList))
                    {
                        report.ShowDialog();
                    }
                }
            }
        }

      


        private void LoadAllPoliceList()
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                //con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("ViewAllPolice", con);
                sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dt.Columns.Add("Image", Type.GetType("System.Byte[]"));
                foreach (DataRow drPlist in dt.Rows)
                {
                    drPlist["Image"] = File.ReadAllBytes(Application.StartupPath + "Images" + drPlist["ImagePath"].ToString());
                }
                dgvPoliceList.RowTemplate.Height = 80;
                dgvPoliceList.DataSource = dt;

                ((DataGridViewImageColumn)dgvPoliceList.Columns[dgvPoliceList.Columns.Count - 1]).ImageLayout = DataGridViewImageCellLayout.Stretch;

                sda.Dispose();
            }
        }
        private void LoadAllDesignationCmb()
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM Designation", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                DataRow topRow = dt.NewRow();
                topRow[0] = 0;
                topRow[1] = "-----Select-----";
                dt.Rows.InsertAt(topRow, 0);
                cmbDesignation.ValueMember = "DesignationId";
                cmbDesignation.DisplayMember = "DesignationTitle";
                cmbDesignation.DataSource = dt;
            }
        }

        private void Clear()
        {
            txtPoliceCode.Text = "";
            txtPoliceName.Text = "";
            cmbDesignation.SelectedIndex = 0;
            DateOfBirth.Value = DateTime.Now;
            rbtnMale.Checked = true;
            CheckStatus.Checked = true;
            intPoliceId = 0;
            btnDelete.Enabled = false;
            btnSave.Text = "Save";
            string imagePath = Path.Combine(Application.StartupPath, "Images", "DefaultImage.png");

            if (File.Exists(imagePath))
            {
                pictureBoxPolice.Image = Image.FromFile(imagePath);
            }
            else
            {
                MessageBox.Show($"Image not found: {imagePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Optionally set a default image or handle the error
                pictureBoxPolice.Image = null; // Or set a default image
            }

            defaultImage = true;
            if (dgvExperiences.DataSource == null)
            {
                dgvExperiences.Rows.Clear();
            }
            else
            {
                dgvExperiences.DataSource = (dgvExperiences.DataSource as DataTable).Clone();
            }
        }

        bool ValidateMasterDetailForm()
        {
            bool isValid = true;
            if (txtPoliceName.Text.Trim() == "")
            {
                MessageBox.Show("Police name is required");
                isValid = false;
            }
            return isValid;
        }

        string SaveImage(string imgPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(imgPath);
            string ext = Path.GetExtension(imgPath);
            fileName = fileName.Length <= 15 ? fileName : fileName.Substring(0, 15);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + ext;
            pictureBoxPolice.Image.Save(Application.StartupPath + "Images" + fileName);
            return fileName;
        }

        public void SavePoliceImage(string fileName)
        {
            string directoryPath = Path.Combine(Application.StartupPath, "Images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, fileName);

            try
            {
                if (pictureBoxPolice.Image != null)
                {
                    // Save the image
                    pictureBoxPolice.Image.Save(filePath);
                    MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No image to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void dgvExperiences_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DataGridViewRow dgvRow = dgvExperiences.CurrentRow;
            if (dgvRow.Cells["dgvExperienceId"].Value != DBNull.Value)
            {
                if (MessageBox.Show("Are you sure to delete this record?", "Master Details", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("ExperienceDelete", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExperienceId", dgvRow.Cells["dgvExperienceId"].Value);
                        cmd.ExecuteNonQuery();
                    }

                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

    

    

       

        private void dgvPoliceList_DoubleClick(object sender, EventArgs e)
        {
            if (dgvPoliceList.CurrentRow.Index != -1)
            {
                DataGridViewRow dgvRow = dgvPoliceList.CurrentRow;
                intPoliceId = Convert.ToInt32(dgvRow.Cells[0].Value);
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();
                    SqlDataAdapter sda = new SqlDataAdapter("ViewPoliceByPoliceId", con);
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sda.SelectCommand.Parameters.AddWithValue("@PoliceId", intPoliceId);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    //--Master---
                    DataRow dr = ds.Tables[0].Rows[0];
                    txtPoliceCode.Text = dr["PoliceCode"].ToString();
                    txtPoliceName.Text = dr["PoliceName"].ToString();
                    cmbDesignation.SelectedValue = Convert.ToInt32(dr["DesignationId"].ToString());
                    DateOfBirth.Value = Convert.ToDateTime(dr["DateOfBirth"].ToString());
                    if (Convert.ToBoolean(dr["IsPermanent"].ToString()))
                    {
                        CheckStatus.Checked = true;
                    }
                    else
                    {
                        CheckStatus.Checked = false;
                    }
                    if ((dr["Gender"].ToString().Trim()) == "Male")
                    {
                        rbtnMale.Checked = true;
                    }
                    else
                    {
                        rbtnMale.Checked = false;
                    }
                    if ((dr["Gender"].ToString().Trim()) == "Female")
                    {
                        rbtnFemale.Checked = true;
                    }
                    else
                    {
                        rbtnFemale.Checked = false;
                    }
                    if (dr["ImagePath"] == DBNull.Value)
                    {
                        pictureBoxPolice.Image = new Bitmap(Application.StartupPath + "imagesnoimage.png");
                    }
                    else
                    {
                        string image = dr["ImagePath"].ToString();
                        pictureBoxPolice.Image = new Bitmap(Application.StartupPath + "images" + dr["ImagePath"].ToString());
                        strPreviousImage = dr["ImagePath"].ToString();
                        defaultImage = false;
                    }
                    //--Details---
                    

                    dgvExperiences.AutoGenerateColumns = false;
                    dgvExperiences.DataSource = ds.Tables[1];
                    btnDelete.Enabled = true;
                    btnSave.Text = "Update";
                    tabControl1.SelectedIndex = 0;
                }
            }

        }

        private void dgvPoliceList_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DataGridViewRow dgvRow = dgvExperiences.CurrentRow;
            if (dgvRow.Cells["dgvExperienceId"].Value != DBNull.Value)
            {
                if (MessageBox.Show("Are you sure to delete this record?", "Master Details", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("ExperienceDelete", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ExperienceId", dgvRow.Cells["dgvExperienceId"].Value);
                        cmd.ExecuteNonQuery();
                    }

                }
                else
                {
                    e.Cancel = true;
                }
            }

        }
    }
}
