using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace _4._6
{
    public partial class Form1 : Form
    {
    
        private BindingList<Student> studentList;
        private BindingSource bindingSource;

        public Form1()
        {
            InitializeComponent();
            InitializeData();
            SetupBindings();
            RegisterEvents();
        }

        private void InitializeData()
        {
        
            studentList = new BindingList<Student>
            {
                new Student { StudentId = "SV01", FullName = "Nguyễn Văn A", Age = 20, Gender = "Nam", AvatarPath = "" },
                new Student { StudentId = "SV02", FullName = "Trần Thị B", Age = 21, Gender = "Nữ", AvatarPath = "" }
            };

            bindingSource = new BindingSource();
            bindingSource.DataSource = studentList;

            dgvStudents.DataSource = bindingSource;

            dgvStudents.Columns["StudentId"].HeaderText = "Mã SV";
            dgvStudents.Columns["FullName"].HeaderText = "Họ tên";
            dgvStudents.Columns["FullName"].Width = 150;
            dgvStudents.Columns["Age"].HeaderText = "Tuổi";
            dgvStudents.Columns["Gender"].HeaderText = "Giới tính";
            dgvStudents.Columns["AvatarPath"].HeaderText = "Đường dẫn ảnh";
            dgvStudents.Columns["AvatarPath"].Width = 150;
        }

        private void SetupBindings()
        {
           
            txtStudentId.DataBindings.Clear();
            txtFullName.DataBindings.Clear();
            txtAge.DataBindings.Clear();
            cboGender.DataBindings.Clear();

   
            txtStudentId.DataBindings.Add("Text", bindingSource, "StudentId", true, DataSourceUpdateMode.OnPropertyChanged);
            txtFullName.DataBindings.Add("Text", bindingSource, "FullName", true, DataSourceUpdateMode.OnPropertyChanged);
            txtAge.DataBindings.Add("Text", bindingSource, "Age", true, DataSourceUpdateMode.OnPropertyChanged);
            cboGender.DataBindings.Add("Text", bindingSource, "Gender", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void RegisterEvents()
        {
   
            bindingSource.CurrentChanged += BindingSource_CurrentChanged;

        
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnChooseAvatar.Click += BtnChooseAvatar_Click;
            btnExportCSV.Click += BtnExportCSV_Click;

            BindingSource_CurrentChanged(null, null);
        }

        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
          
            if (bindingSource.Current is Student currentStudent)
            {
                try
                {
                    
                    if (!string.IsNullOrEmpty(currentStudent.AvatarPath) && File.Exists(currentStudent.AvatarPath))
                    {
                        picAvatar.Image = Image.FromFile(currentStudent.AvatarPath);
                    }
                    else
                    {
                        picAvatar.Image = null; 
                    }
                }
                catch
                {
                    picAvatar.Image = null;
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
           
            var newStudent = new Student
            {
                StudentId = "SV_NEW",
                FullName = "Nguyễn Văn C",
                Age = 18,
                Gender = "Nam",
                AvatarPath = ""
            };
            bindingSource.Add(newStudent);
            bindingSource.MoveLast(); 
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current != null)
            {
                var confirmResult = MessageBox.Show("Bạn có chắc muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    bindingSource.RemoveCurrent();
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
           
            bindingSource.EndEdit();
            bindingSource.ResetCurrentItem();
            MessageBox.Show("Đã cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnChooseAvatar_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current is Student currentStudent)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    
                    ofd.Filter = "Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp";
                    ofd.Title = "Chọn ảnh đại diện";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        
                        currentStudent.AvatarPath = ofd.FileName;
                        bindingSource.ResetCurrentItem();

                      
                        picAvatar.Image = Image.FromFile(ofd.FileName);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sinh viên trong danh sách trước!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnExportCSV_Click(object sender, EventArgs e)
        {
            if (studentList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
               
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.Title = "Xuất file CSV";
                sfd.FileName = "DanhSachSinhVien.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                       
                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false, new UTF8Encoding(true)))
                        {
                            
                            sw.WriteLine("Mã SV,Họ tên,Tuổi,Giới tính,Đường dẫn ảnh");

                         
                            foreach (var student in studentList)
                            {
                              
                                string fullName = student.FullName.Contains(",") ? $"\"{student.FullName}\"" : student.FullName;
                                sw.WriteLine($"{student.StudentId},{fullName},{student.Age},{student.Gender},{student.AvatarPath}");
                            }
                        }
                        MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

    
    public class Student
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string AvatarPath { get; set; }
    }
}
