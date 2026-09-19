using System;
using System.Windows.Forms;
using InventoryManagementSystem.BLL;
using InventoryManagementSystem.Entity;

namespace InventoryManagementSystem.UI
{
    public partial class frmLogin : Form
    {
        private readonly UserBLL _userBLL = new UserBLL();

        public UserEntity AuthenticatedUser { get; private set; }

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            LoginResult result = _userBLL.Login(username, password);

            switch (result.Status)
            {
                case LoginStatus.Success:
                    AuthenticatedUser = result.User;
                    DialogResult = DialogResult.OK;
                    Close();
                    return;

                case LoginStatus.ValidationError:
                    MessageBox.Show(this, result.Message, "Login",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case LoginStatus.InactiveUser:
                    MessageBox.Show(this, result.Message, "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case LoginStatus.SystemError:
                    MessageBox.Show(this, result.Message, "Login Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case LoginStatus.InvalidCredentials:
                default:
                    MessageBox.Show(this, "Invalid username or password.", "Login Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            txtPassword.Clear();
            txtPassword.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
