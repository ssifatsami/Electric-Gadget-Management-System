using Electric_Gadget_Management.BLL.Services;
using ElectricGadget.Web.Models.Entities;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Electric_Gadget_Management
{
    public partial class RegisterForm : Form
    {
        private readonly AuthService _authService;
        private TextBox txtName = null!, txtEmail = null!, txtPhone = null!, txtPassword = null!, txtConfirmPassword = null!;

        public RegisterForm()
        {
            InitializeComponent();
            _authService = new AuthService();
            this.AutoScaleMode = AutoScaleMode.None;
            this.DoubleBuffered = true;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Sign Up - Electric Gadget";
            this.Size = new Size(1100, 800);
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
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 680F));
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

            Label lblLogo = new Label { Text = "⚡ Electric Gadget", Font = new Font("Segoe UI Bold", 13), ForeColor = Color.Black, AutoSize = true, Margin = new Padding(0, 0, 0, 140) };
            flpLeft.Controls.Add(lblLogo);

            Label lblTitle1 = new Label { Text = "Smart Gadgets,", Font = new Font("Segoe UI", 21, FontStyle.Bold), ForeColor = Color.FromArgb(20, 25, 40), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            Label lblTitle2 = new Label { Text = "Better Living", Font = new Font("Segoe UI", 21, FontStyle.Bold), ForeColor = Color.FromArgb(24, 90, 255), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            flpLeft.Controls.Add(lblTitle1);
            flpLeft.Controls.Add(lblTitle2);

            Label lblDesc = new Label { Text = "Create an account and explore\nthe latest electric gadgets.", Font = new Font("Segoe UI", 12), ForeColor = Color.FromArgb(100, 110, 120), AutoSize = true };
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
            Panel pnlHeader = new Panel { Size = new Size(400, 30), Margin = new Padding(0, 0, 0, 20) };
            Label lblSignIn = new Label { Text = "Log in", Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(24, 90, 255), AutoSize = true, Cursor = Cursors.Hand };
            Label lblTopRight = new Label { Text = "Already have an account?", Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, AutoSize = true };
            
            pnlHeader.Controls.Add(lblSignIn);
            pnlHeader.Controls.Add(lblTopRight);
            
            // Align to right
            lblSignIn.Location = new Point(400 - lblSignIn.PreferredWidth, 0);
            lblTopRight.Location = new Point(lblSignIn.Left - lblTopRight.PreferredWidth - 5, 0);
            
            lblSignIn.Click += (s, e) => { new Form1().Show(); this.Hide(); };
            flpRight.Controls.Add(pnlHeader);

            // Welcome texts
            Label lblWelcome = new Label { Text = "Create your account", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.FromArgb(20, 25, 40), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
            Label lblSub = new Label { Text = "Sign up to get started", Font = new Font("Segoe UI", 11), ForeColor = Color.FromArgb(120, 130, 140), AutoSize = true, Margin = new Padding(0, 0, 0, 30) };
            flpRight.Controls.Add(lblWelcome);
            flpRight.Controls.Add(lblSub);

            // Inputs
            Panel pnlName = CreateInputPanel("Full Name", "👤   Enter your full name", false, out txtName);
            pnlName.Margin = new Padding(0, 0, 0, 10);
            flpRight.Controls.Add(pnlName);

            Panel pnlEmail = CreateInputPanel("Email Address", "✉   Enter your email address", false, out txtEmail);
            pnlEmail.Margin = new Padding(0, 0, 0, 10);
            flpRight.Controls.Add(pnlEmail);

            Panel pnlPhone = CreateInputPanel("Phone Number (Optional)", "📞   Enter your phone number", false, out txtPhone);
            pnlPhone.Margin = new Padding(0, 0, 0, 10);
            flpRight.Controls.Add(pnlPhone);

            Panel pnlPassword = CreateInputPanel("Password", "🔒   Create a password", true, out txtPassword);
            pnlPassword.Margin = new Padding(0, 0, 0, 10);
            flpRight.Controls.Add(pnlPassword);

            Panel pnlConfirmPassword = CreateInputPanel("Confirm Password", "🔒   Confirm your password", true, out txtConfirmPassword);
            pnlConfirmPassword.Margin = new Padding(0, 0, 0, 15);
            flpRight.Controls.Add(pnlConfirmPassword);

            // Checkbox
            CheckBox chkTerms = new CheckBox { Text = "I agree to the Terms of Service and Privacy Policy", Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(40, 40, 40), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };
            flpRight.Controls.Add(chkTerms);

            // Sign Up Button
            Button btnRegister = new Button { Text = "Sign Up", Size = new Size(400, 45), BackColor = Color.FromArgb(24, 90, 255), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 12), Cursor = Cursors.Hand, Margin = new Padding(0) };
            btnRegister.FlatAppearance.BorderSize = 0;
            ApplyRoundedRegion(btnRegister, 10);
            btnRegister.Click += btnRegister_Click;
            flpRight.Controls.Add(btnRegister);

            // Exit button
            Button btnExit = new Button { Text = "✕", Size = new Size(40, 40), Location = new Point(this.Width - 40, 0), BackColor = Color.Transparent, ForeColor = Color.FromArgb(150, 150, 150), FlatStyle = FlatStyle.Flat, Font = new Font("Arial", 14, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnExit);
            btnExit.BringToFront();
        }

        private Panel CreateInputPanel(string labelText, string placeholder, bool isPassword, out TextBox txtOut)
        {
            Panel pnlWrapper = new Panel { Size = new Size(400, 68), Margin = new Padding(0) };

            Label lbl = new Label { Text = labelText, ForeColor = Color.FromArgb(40, 40, 40), Font = new Font("Segoe UI Semibold", 9), Location = new Point(0, 0), AutoSize = true };
            pnlWrapper.Controls.Add(lbl);

            Panel pnlBox = new Panel { Size = new Size(400, 42), Location = new Point(0, 22), BackColor = Color.White };
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
            
            TextBox txt = new TextBox { Location = new Point(15, 10), Size = new Size(370, 25), BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 11), ForeColor = Color.FromArgb(150, 150, 150), Text = placeholder };
            
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

        private void btnRegister_Click(object? sender, EventArgs e)
        {
            string name = txtName.Text == "👤   Enter your full name" || txtName.Text == "Enter your full name" ? "" : txtName.Text;
            string email = txtEmail.Text == "✉   Enter your email address" || txtEmail.Text == "Enter your email address" ? "" : txtEmail.Text;
            string pass = txtPassword.Text == "🔒   Create a password" || txtPassword.Text == "Create a password" ? "" : txtPassword.Text;
            string confirmPass = txtConfirmPassword.Text == "🔒   Confirm your password" || txtConfirmPassword.Text == "Confirm your password" ? "" : txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email)) { MessageBox.Show("Please fill all required fields!"); return; }
            if (pass != confirmPass) { MessageBox.Show("Passwords do not match!"); return; }

            // Auto-generate UserID in background using Email or Random
            string generatedId = "USER_" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper();

            var newUser = new User { UserID = generatedId, Name = name, Email = email, Password = pass, CreatedAt = DateTime.Now };
            var result = _authService.Register(newUser);

            if (result.IsSuccess) { MessageBox.Show("Registration Successful! You can now login with your email."); new Form1().Show(); this.Close(); }
            else MessageBox.Show(result.Message);
        }

        private void btnBack_Click(object? sender, EventArgs e) { new Form1().Show(); this.Close(); }
    }
}
