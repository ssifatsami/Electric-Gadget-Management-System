using Electric_Gadget_Management.BLL.Services;
using ElectricGadget.Web.Models.Entities;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Electric_Gadget_Management
{
    public partial class Form1 : Form
    {
        private readonly AuthService _authService;
        private TextBox txtEmail = null!, txtPassword = null!;

        public Form1()
        {
            InitializeComponent();
            _authService = new AuthService();
            this.AutoScaleMode = AutoScaleMode.None;
            this.DoubleBuffered = true;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Login - Electric Gadget";
            this.Size = new Size(1100, 700);
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main container for centering
            TableLayoutPanel mainLayout = new TableLayoutPanel {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1000F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 600F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            this.Controls.Add(mainLayout);

            // Card Panel
            Panel mainCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            mainLayout.Controls.Add(mainCard, 1, 1);
            
            mainCard.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    Rectangle rect = new Rectangle(0, 0, mainCard.Width - 1, mainCard.Height - 1);
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };
            ApplyRoundedRegion(mainCard, 20);

            // Split card into two halves
            TableLayoutPanel cardLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            cardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            mainCard.Controls.Add(cardLayout);

            // --- LEFT PANEL ---
            Panel pnlLeft = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(244, 246, 251) };
            cardLayout.Controls.Add(pnlLeft, 0, 0);

            FlowLayoutPanel flpLeft = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(40, 70, 20, 50)
            };
            pnlLeft.Controls.Add(flpLeft);

            Label lblLogo = new Label { Text = "⚡ Electric Gadget", Font = new Font("Segoe UI Bold", 13), ForeColor = Color.Black, AutoSize = true, Margin = new Padding(0, 0, 0, 90) };
            flpLeft.Controls.Add(lblLogo);

            Label lblTitle1 = new Label { Text = "Smart Gadgets,", Font = new Font("Segoe UI", 21, FontStyle.Bold), ForeColor = Color.FromArgb(20, 25, 40), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            Label lblTitle2 = new Label { Text = "Better Living", Font = new Font("Segoe UI", 21, FontStyle.Bold), ForeColor = Color.FromArgb(24, 90, 255), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            flpLeft.Controls.Add(lblTitle1);
            flpLeft.Controls.Add(lblTitle2);

            Label lblDesc = new Label { Text = "Quality products. Trusted service.\nBetter life.", Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(100, 110, 120), AutoSize = true };
            flpLeft.Controls.Add(lblDesc);

            // --- RIGHT PANEL ---
            Panel pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            cardLayout.Controls.Add(pnlRight, 1, 0);

            // Centering grid for the right panel content
            TableLayoutPanel rightCenter = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 };
            rightCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rightCenter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            rightCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rightCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            rightCenter.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rightCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            pnlRight.Controls.Add(rightCenter);

            FlowLayoutPanel flpRight = new FlowLayoutPanel {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                BackColor = Color.White
            };
            rightCenter.Controls.Add(flpRight, 1, 1);

            // Top Header
            Panel pnlHeader = new Panel { Size = new Size(400, 30), Margin = new Padding(0, 0, 0, 40) };
            Label lblSignUp = new Label { Text = "Sign up", Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(24, 90, 255), AutoSize = true, Cursor = Cursors.Hand };
            Label lblTopRight = new Label { Text = "Don't have an account?", Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, AutoSize = true };
            
            pnlHeader.Controls.Add(lblSignUp);
            pnlHeader.Controls.Add(lblTopRight);
            
            // Align to right
            lblSignUp.Location = new Point(400 - lblSignUp.PreferredWidth, 0);
            lblTopRight.Location = new Point(lblSignUp.Left - lblTopRight.PreferredWidth - 5, 0);
            
            lblSignUp.Click += (s, e) => { new RegisterForm().Show(); this.Hide(); };
            flpRight.Controls.Add(pnlHeader);

            // Welcome texts
            Label lblWelcome = new Label { Text = "Welcome back", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(20, 25, 40), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            Label lblSub = new Label { Text = "Login to continue", Font = new Font("Segoe UI", 11), ForeColor = Color.FromArgb(120, 130, 140), AutoSize = true, Margin = new Padding(0, 0, 0, 40) };
            flpRight.Controls.Add(lblWelcome);
            flpRight.Controls.Add(lblSub);

            // Inputs
            Panel pnlEmail = CreateInputPanel("Email address", "Enter your email", false, out txtEmail);
            pnlEmail.Margin = new Padding(0, 0, 0, 20);
            flpRight.Controls.Add(pnlEmail);

            Panel pnlPassword = CreateInputPanel("Password", "Enter your password", true, out txtPassword);
            pnlPassword.Margin = new Padding(0, 0, 0, 10);
            flpRight.Controls.Add(pnlPassword);

            // Forgot Password
            Panel pnlForgot = new Panel { Size = new Size(400, 25), Margin = new Padding(0, 0, 0, 30) };
            Label lblForgot = new Label { Text = "Forgot password?", Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(24, 90, 255), AutoSize = true, Cursor = Cursors.Hand };
            lblForgot.Location = new Point(400 - lblForgot.PreferredWidth, 0);
            pnlForgot.Controls.Add(lblForgot);
            flpRight.Controls.Add(pnlForgot);

            // Login Button
            Button btnLogin = new Button { Text = "Login", Size = new Size(400, 50), BackColor = Color.FromArgb(24, 90, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 12), Cursor = Cursors.Hand, Margin = new Padding(0) };
            btnLogin.FlatAppearance.BorderSize = 0;
            ApplyRoundedRegion(btnLogin, 10);
            btnLogin.Click += btnLogin_Click;
            flpRight.Controls.Add(btnLogin);

            // Exit button
            Button btnExit = new Button { Text = "✕", Size = new Size(40, 40), Location = new Point(this.Width - 40, 0), BackColor = Color.Transparent, ForeColor = Color.FromArgb(150, 150, 150), FlatStyle = FlatStyle.Flat, Font = new Font("Arial", 14, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnExit);
            btnExit.BringToFront();
        }

        private Panel CreateInputPanel(string labelText, string placeholder, bool isPassword, out TextBox txtOut)
        {
            Panel pnlWrapper = new Panel { Size = new Size(400, 75), Margin = new Padding(0) };

            Label lbl = new Label { Text = labelText, ForeColor = Color.FromArgb(40, 40, 40), Font = new Font("Segoe UI Semibold", 9), Location = new Point(0, 0), AutoSize = true };
            pnlWrapper.Controls.Add(lbl);

            Panel pnlBox = new Panel { Size = new Size(400, 48), Location = new Point(0, 25), BackColor = Color.White };
            pnlBox.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(210, 215, 220), 1))
                {
                    Rectangle rect = new Rectangle(0, 0, pnlBox.Width - 1, pnlBox.Height - 1);
                    int radius = 8;
                    GraphicsPath path = new GraphicsPath();
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseFigure();
                    e.Graphics.DrawPath(pen, path);
                }
            };
            
            TextBox txt = new TextBox { Location = new Point(15, 14), Size = new Size(370, 25), BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11), ForeColor = Color.FromArgb(150, 150, 150), Text = placeholder };
            
            txt.Enter += (s, e) => { if (txt.Text == placeholder) { txt.Text = ""; txt.ForeColor = Color.Black; if (isPassword) txt.UseSystemPasswordChar = true; } };
            txt.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txt.Text)) { txt.Text = placeholder; txt.ForeColor = Color.FromArgb(150, 150, 150); if (isPassword) txt.UseSystemPasswordChar = false; } };

            pnlBox.Controls.Add(txt);
            pnlWrapper.Controls.Add(pnlBox);

            txtOut = txt;
            return pnlWrapper;
        }

        private void ApplyRoundedRegion(Control ctrl, int radius)
        {
            IntPtr ptr = CreateRoundRectRgn(0, 0, ctrl.Width, ctrl.Height, radius, radius);
            ctrl.Region = Region.FromHrgn(ptr);
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            try
            {
                string email = txtEmail.Text == "✉   Enter your email" || txtEmail.Text == "Enter your email" ? "" : txtEmail.Text;
                string pass = txtPassword.Text == "🔒   Enter your password" || txtPassword.Text == "Enter your password" ? "" : txtPassword.Text;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass)) { MessageBox.Show("Fields required!"); return; }
                var result = _authService.Login(email, pass);
                if (result.IsSuccess && result.User != null) 
                { 
                    Form dashboard;
                    if (result.User.Role == "Super Admin") dashboard = new SuperAdminDashboard(result.User);
                    else if (result.User.Role == "Admin") dashboard = new AdminDashboard(result.User);
                    else dashboard = new CustomerDashboard(result.User);
                    
                    dashboard.Show(); 
                    this.Hide(); 
                }
                else MessageBox.Show(result.Message);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("error_log.txt", ex.ToString());
                MessageBox.Show("Error logged to error_log.txt");
            }
        }
    }
}
