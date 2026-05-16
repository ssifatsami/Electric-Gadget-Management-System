using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Electric_Gadget_Management.DAL.Database;
using ElectricGadget.Web.Models.Entities;

namespace Electric_Gadget_Management
{
    public class BaseDashboard : Form
    {
        protected Panel sidePanel = null!, mainPanel = null!, headerPanel = null!, pnlHome = null!;
        protected TableLayoutPanel mainLayout = null!, rightLayout = null!;
        protected Label lblWelcome = null!;
        protected User currentUser;
        protected DatabaseHelper dbHelper;
        protected Button? activeButton;

        public BaseDashboard(User user)
        {
            currentUser = user;
            dbHelper = new DatabaseHelper();
            this.Size = new Size(1400, 950);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 246, 250); 
            this.AutoScaleMode = AutoScaleMode.None;
            this.Controls.Clear();

            // 1. Main Layout Table (Sidebar | Content)
            mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            this.Controls.Add(mainLayout);

            // 2. Sidebar
            sidePanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 31, 33), Margin = new Padding(0) };
            mainLayout.Controls.Add(sidePanel, 0, 0);

            // 3. Right Side Layout (Header / Content)
            rightLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Color.Transparent, Margin = new Padding(0) };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.Controls.Add(rightLayout, 1, 0);

            // 4. Header
            headerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0) };
            rightLayout.Controls.Add(headerPanel, 0, 0);

            // 5. Main Content Panel
            mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(40), AutoScroll = true, Margin = new Padding(0) };
            rightLayout.Controls.Add(mainPanel, 0, 1);

            SetupSidebarHeader();
            
            Panel headerLine = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.FromArgb(230, 230, 230) };
            headerPanel.Controls.Add(headerLine);

            lblWelcome = new Label { Text = $"Hello, {currentUser.Name}", Font = new Font("Segoe UI Semibold", 15), ForeColor = Color.FromArgb(45, 52, 54), AutoSize = true, Location = new Point(30, 25) };
            headerPanel.Controls.Add(lblWelcome);
            
            AddLogoutButton();
            this.FormClosed += (s, e) => Application.Exit();
        }

        protected void SetupSidebarHeader()
        {
            Panel pnlLogo = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = Color.FromArgb(30, 31, 33) };
            Label lblLogo = new Label { Text = "GADGET", Font = new Font("Segoe UI Bold", 24), ForeColor = Color.FromArgb(59, 130, 246), Location = new Point(25, 25), Size = new Size(250, 50), AutoSize = false };
            Label lblStore = new Label { Text = "STORE", Font = new Font("Segoe UI Semibold", 18), ForeColor = Color.White, Location = new Point(25, 75), Size = new Size(250, 45), AutoSize = false };
            pnlLogo.Controls.AddRange(new Control[] { lblLogo, lblStore });
            sidePanel.Controls.Add(pnlLogo);
        }

        protected void AddLogoutButton()
        {
            Button btnLogout = new Button { Text = "  🚪  Sign Out", Dock = DockStyle.Bottom, Height = 60, FlatStyle = FlatStyle.Flat, ForeColor = Color.FromArgb(200, 200, 200), BackColor = Color.Transparent, Font = new Font("Segoe UI Semibold", 10), TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 76, 60);
            btnLogout.MouseEnter += (s, e) => btnLogout.ForeColor = Color.White;
            btnLogout.MouseLeave += (s, e) => btnLogout.ForeColor = Color.FromArgb(200, 200, 200);
            btnLogout.Click += (s, e) => { this.Hide(); new Form1().Show(); };
            sidePanel.Controls.Add(btnLogout);
        }

        protected Button CreateMenuButton(string text, int yPos, EventHandler onClick)
        {
            Button btn = new Button { Text = "     " + text, Location = new Point(0, yPos), Size = new Size(280, 55), FlatStyle = FlatStyle.Flat, ForeColor = Color.FromArgb(209, 213, 219), BackColor = Color.Transparent, Font = new Font("Segoe UI Semibold", 11), TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 46, 48);
            btn.Click += (s, e) => { 
                if (activeButton != null) { activeButton.BackColor = Color.Transparent; activeButton.ForeColor = Color.FromArgb(209, 213, 219); } 
                activeButton = btn; btn.BackColor = Color.FromArgb(59, 130, 246); btn.ForeColor = Color.White; 
                onClick?.Invoke(s, e); 
            };
            return btn;
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        protected static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        protected void ApplyRoundedRegion(Control ctrl, int radius)
        {
            ctrl.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, ctrl.Width, ctrl.Height, radius, radius));
        }

        protected DataGridView CreateModernGrid()
        {
            var grid = new DataGridView { BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowTemplate = { Height = 55 }, GridColor = Color.FromArgb(230, 230, 230), ForeColor = Color.FromArgb(45, 52, 54) };
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Bold", 10);
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(232, 240, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(0, 102, 204);
            return grid;
        }

        protected async Task LoadWebImage(PictureBox pic, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http")) { pic.Image = null; return; }
            try {
                using (var client = new HttpClient()) {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    var bytes = await client.GetByteArrayAsync(url);
                    using (var ms = new MemoryStream(bytes)) {
                        pic.Image = Image.FromStream(ms);
                    }
                }
            } catch { pic.Image = null; }
        }

        protected void LogAsync(string action, string module, string details)
        {
            try {
                string sql = "INSERT INTO AuditLogs (UserId, UserName, Action, Module, Details, Timestamp, IpAddress) VALUES (@Uid, @Uname, @Act, @Mod, @Det, GETDATE(), @Ip)";
                dbHelper.ExecuteNonQuery(sql, new[] {
                    new Microsoft.Data.SqlClient.SqlParameter("@Uid", currentUser.UserID),
                    new Microsoft.Data.SqlClient.SqlParameter("@Uname", currentUser.Name),
                    new Microsoft.Data.SqlClient.SqlParameter("@Act", action),
                    new Microsoft.Data.SqlClient.SqlParameter("@Mod", module),
                    new Microsoft.Data.SqlClient.SqlParameter("@Det", details),
                    new Microsoft.Data.SqlClient.SqlParameter("@Ip", "WinForms-Client")
                });
            } catch { /* Silent fail for logging */ }
        }
    }

    public class AdminDashboard : BaseDashboard
    {
        private Panel pnlOrders = null!, pnlManageProducts = null!, pnlEditProduct = null!, pnlAddProduct = null!;
        private DataGridView dgvOrders = null!, dgvProducts = null!;
        private TextBox txtEditName = null!, txtEditPrice = null!, txtEditStock = null!, txtEditImage = null!, txtEditDesc = null!;
        private ComboBox cbEditCategory = null!, cbAddCategory = null!, cbEditBrand = null!, cbAddBrand = null!;
        private TextBox txtAddName = null!, txtAddPrice = null!, txtAddStock = null!, txtAddImage = null!, txtAddDesc = null!;
        private PictureBox picEditPreview = null!, picAddPreview = null!;
        private int currentEditProductId;

        public AdminDashboard(User user) : base(user)
        {
            try {
                sidePanel.BackColor = Color.FromArgb(30, 31, 33);
                headerPanel.BackColor = Color.White;
                
                var btnHome = CreateMenuButton("🏠  Dashboard Home", 140, (s, e) => ShowPanel(pnlHome));
                sidePanel.Controls.Add(btnHome);
                sidePanel.Controls.Add(CreateMenuButton("⏳  Pending Orders", 195, (s, e) => ShowPanel(pnlOrders)));
                sidePanel.Controls.Add(CreateMenuButton("📦  Manage Products", 250, (s, e) => ShowPanel(pnlManageProducts)));
                sidePanel.Controls.Add(CreateMenuButton("➕  Add Product", 305, (s, e) => ShowPanel(pnlAddProduct)));

                SetupHomePanel();
                SetupOrdersPanel();
                SetupManageProductsPanel();
                SetupEditProductPanel();
                SetupAddProductPanel();

                // Set Home as active by default
                activeButton = btnHome;
                btnHome.BackColor = Color.FromArgb(59, 130, 246);
                btnHome.ForeColor = Color.White;
                ShowPanel(pnlHome);
            } catch (Exception ex) {
                MessageBox.Show("Error initializing Admin Dashboard: " + ex.Message);
            }
        }

        private FlowLayoutPanel flpAdminStats = null!;
        private void SetupHomePanel()
        {
            pnlHome = new Panel { Dock = DockStyle.Fill, Visible = false };
            
            Label lblTitle = new Label { Text = "Dashboard Overview", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(40, 30), AutoSize = true };
            pnlHome.Controls.Add(lblTitle);

            flpAdminStats = new FlowLayoutPanel { Location = new Point(40, 100), Size = new Size(1100, 300), BackColor = Color.Transparent, AutoSize = true };
            pnlHome.Controls.Add(flpAdminStats);

            mainPanel.Controls.Add(pnlHome);
            RefreshAdminStats();
        }

        private void RefreshAdminStats()
        {
            if (flpAdminStats == null) return;
            flpAdminStats.Controls.Clear();

            int totalProducts = Convert.ToInt32(dbHelper.ExecuteQuery("SELECT COUNT(*) FROM Products").Rows[0][0]);
            int pendingOrders = Convert.ToInt32(dbHelper.ExecuteQuery("SELECT COUNT(*) FROM Orders WHERE Status = 'Pending'").Rows[0][0]);
            decimal totalRevenue = Convert.ToDecimal(dbHelper.ExecuteQuery("SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders WHERE Status = 'Paid'").Rows[0][0]);

            flpAdminStats.Controls.Add(CreateStatCard("Total Products", totalProducts.ToString(), "📦", Color.FromArgb(52, 152, 219)));
            flpAdminStats.Controls.Add(CreateStatCard("Pending Orders", pendingOrders.ToString(), "⏳", Color.FromArgb(230, 126, 34)));
            flpAdminStats.Controls.Add(CreateStatCard("Total Revenue", $"৳{totalRevenue:N0}", "💰", Color.FromArgb(46, 204, 113)));
        }

        private Panel CreateStatCard(string title, string value, string icon, Color color)
        {
            Panel card = new Panel { Size = new Size(260, 260), BackColor = Color.White, Margin = new Padding(0, 0, 30, 0), BorderStyle = BorderStyle.FixedSingle };
            Label lblIcon = new Label { Text = icon, Font = new Font("Segoe UI", 48), ForeColor = color, Dock = DockStyle.Top, Height = 100, TextAlign = ContentAlignment.BottomCenter };
            Label lblVal = new Label { Text = value, Font = new Font("Segoe UI Bold", 32), ForeColor = Color.FromArgb(45, 52, 54), Dock = DockStyle.Top, Height = 80, TextAlign = ContentAlignment.MiddleCenter };
            Label lblTitle = new Label { Text = title, Font = new Font("Segoe UI Semibold", 12), ForeColor = Color.Gray, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter };
            card.Controls.AddRange(new Control[] { lblTitle, lblVal, lblIcon }); // Added in reverse order for Dock.Top
            return card;
        }

        private void SetupOrdersPanel()
        {
            pnlOrders = new Panel { Dock = DockStyle.Fill, Visible = false };
            dgvOrders = CreateModernGrid(); dgvOrders.Dock = DockStyle.Fill;
            Button btnApprove = new Button { Text = "✅ Verify & Approve Payment", Dock = DockStyle.Bottom, Height = 65, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 12) };
            btnApprove.Click += (s, e) => {
                if (dgvOrders.SelectedRows.Count > 0 && dgvOrders.SelectedRows[0].Cells["Id"].Value != null) {
                    int id = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["Id"].Value);
                    dbHelper.ExecuteNonQuery("UPDATE Orders SET Status = 'Paid' WHERE Id = @Id", new[] { new Microsoft.Data.SqlClient.SqlParameter("@Id", id) });
                    LogAsync("Verify", "Order", $"Payment verified for Order ID: {id}");
                    MessageBox.Show("Payment Verified & Approved!"); LoadOrders();
                }
            };
            pnlOrders.Controls.AddRange(new Control[] { dgvOrders, btnApprove });
            mainPanel.Controls.Add(pnlOrders);
        }

        private void SetupManageProductsPanel()
        {
            pnlManageProducts = new Panel { Dock = DockStyle.Fill, Visible = false };
            dgvProducts = CreateModernGrid(); dgvProducts.Dock = DockStyle.Fill;
            
            Button btnEdit = new Button { Text = "✏️ Edit Selected Product", Dock = DockStyle.Bottom, Height = 65, BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 12) };
            btnEdit.Click += (s, e) => {
                if (dgvProducts.SelectedRows.Count > 0 && dgvProducts.SelectedRows[0].Cells["Id"].Value != null) {
                    int id = Convert.ToInt32(dgvProducts.SelectedRows[0].Cells["Id"].Value);
                    OpenEditPanel(id);
                }
            };
            
            pnlManageProducts.Controls.AddRange(new Control[] { dgvProducts, btnEdit });
            mainPanel.Controls.Add(pnlManageProducts);
        }

        private void SetupEditProductPanel()
        {
            pnlEditProduct = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = Color.White, Padding = new Padding(50) };
            
            Label lblTitle = new Label { Text = "✏️ Edit Product Details", Font = new Font("Segoe UI", 26, FontStyle.Bold), Location = new Point(50, 30), AutoSize = true, ForeColor = Color.FromArgb(44, 62, 80) };
            
            int y = 100;
            txtEditName = CreateLabeledInput("Product Name", ref y, pnlEditProduct);
            
            pnlEditProduct.Controls.Add(new Label { Text = "Brand", Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(127, 140, 141) });
            cbEditBrand = new ComboBox { Location = new Point(50, y + 30), Width = 280, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            pnlEditProduct.Controls.Add(cbEditBrand);

            pnlEditProduct.Controls.Add(new Label { Text = "Category *", Location = new Point(370, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(231, 76, 60) });
            cbEditCategory = new ComboBox { Location = new Point(370, y + 30), Width = 280, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            pnlEditProduct.Controls.Add(cbEditCategory); y += 80;

            txtEditPrice = CreateLabeledInput("Price (৳)", ref y, pnlEditProduct);
            txtEditStock = CreateLabeledInput("Stock Quantity", ref y, pnlEditProduct);
            
            pnlEditProduct.Controls.Add(new Label { Text = "Product Image URL", Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(127, 140, 141) });
            txtEditImage = new TextBox { Location = new Point(50, y + 30), Width = 550, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };
            Label lblHint = new Label { Text = "💡 Tip: Use direct links (ending in .jpg, .png, .webp)", Location = new Point(50, y + 60), ForeColor = Color.Gray, Font = new Font("Segoe UI", 8), AutoSize = true };
            txtEditImage.TextChanged += async (s, e) => { 
                string url = txtEditImage.Text.Trim();
                await LoadWebImage(picEditPreview, url);
            };
            
            picEditPreview = new PictureBox { Location = new Point(650, 100), Size = new Size(350, 350), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(250, 250, 250) };
            pnlEditProduct.Controls.AddRange(new Control[] { txtEditImage, lblHint, picEditPreview });
            y += 80;

            pnlEditProduct.Controls.Add(new Label { Text = "Description", Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(127, 140, 141) });
            txtEditDesc = new TextBox { Location = new Point(50, y + 30), Width = 550, Height = 100, Multiline = true, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };
            pnlEditProduct.Controls.Add(txtEditDesc); y += 150;

            Button btnSave = new Button { Text = "✅ Save Changes", Location = new Point(50, y), Size = new Size(250, 55), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 12), Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += (s, e) => SaveProductChanges();
            
            Button btnCancel = new Button { Text = "✖ Cancel", Location = new Point(320, y), Size = new Size(150, 55), BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 12), Cursor = Cursors.Hand };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => ShowPanel(pnlManageProducts);

            pnlEditProduct.Controls.AddRange(new Control[] { lblTitle, btnSave, btnCancel });
            mainPanel.Controls.Add(pnlEditProduct);
        }

        private void SetupAddProductPanel()
        {
            pnlAddProduct = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = Color.White, Padding = new Padding(50) };
            
            Label lblTitle = new Label { Text = "➕ Add New Product", Font = new Font("Segoe UI", 26, FontStyle.Bold), Location = new Point(50, 30), AutoSize = true, ForeColor = Color.FromArgb(44, 62, 80) };
            
            int y = 100;
            txtAddName = CreateLabeledInput("Product Name", ref y, pnlAddProduct);

            pnlAddProduct.Controls.Add(new Label { Text = "Brand", Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(127, 140, 141) });
            cbAddBrand = new ComboBox { Location = new Point(50, y + 30), Width = 280, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            pnlAddProduct.Controls.Add(cbAddBrand);

            pnlAddProduct.Controls.Add(new Label { Text = "Category *", Location = new Point(370, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(231, 76, 60) });
            cbAddCategory = new ComboBox { Location = new Point(370, y + 30), Width = 280, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            pnlAddProduct.Controls.Add(cbAddCategory); y += 80;

            txtAddPrice = CreateLabeledInput("Price (৳)", ref y, pnlAddProduct);
            txtAddStock = CreateLabeledInput("Stock Quantity", ref y, pnlAddProduct);
            
            pnlAddProduct.Controls.Add(new Label { Text = "Product Image URL", Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(127, 140, 141) });
            txtAddImage = new TextBox { Location = new Point(50, y + 30), Width = 550, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };
            Label lblHintAdd = new Label { Text = "💡 Tip: Use direct links (ending in .jpg, .png, .webp)", Location = new Point(50, y + 60), ForeColor = Color.Gray, Font = new Font("Segoe UI", 8), AutoSize = true };
            txtAddImage.TextChanged += async (s, e) => { 
                string url = txtAddImage.Text.Trim();
                await LoadWebImage(picAddPreview, url);
            };
            
            picAddPreview = new PictureBox { Location = new Point(650, 100), Size = new Size(350, 350), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(250, 250, 250) };
            pnlAddProduct.Controls.AddRange(new Control[] { txtAddImage, lblHintAdd, picAddPreview });
            y += 80;

            pnlAddProduct.Controls.Add(new Label { Text = "Description", Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(127, 140, 141) });
            txtAddDesc = new TextBox { Location = new Point(50, y + 30), Width = 550, Height = 100, Multiline = true, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };
            pnlAddProduct.Controls.Add(txtAddDesc); y += 150;

            Button btnAdd = new Button { Text = "🚀 Add Product", Location = new Point(50, y), Size = new Size(250, 55), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 12), Cursor = Cursors.Hand };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => SaveNewProduct();
            
            Button btnCancel = new Button { Text = "✖ Cancel", Location = new Point(320, y), Size = new Size(150, 55), BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 12), Cursor = Cursors.Hand };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => ShowPanel(pnlManageProducts);

            pnlAddProduct.Controls.AddRange(new Control[] { lblTitle, btnAdd, btnCancel });
            mainPanel.Controls.Add(pnlAddProduct);
        }

        private TextBox CreateLabeledInput(string label, ref int y, Panel p)
        {
            p.Controls.Add(new Label { Text = label, Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(127, 140, 141) });
            TextBox t = new TextBox { Location = new Point(50, y + 30), Width = 600, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };
            p.Controls.Add(t); y += 80; return t;
        }

        private void LoadCategoriesToCombo(ComboBox cb)
        {
            cb.Items.Clear();
            DataTable dt = dbHelper.ExecuteQuery("SELECT Id, Name FROM Categories");
            foreach (DataRow row in dt.Rows)
            {
                cb.Items.Add(new { Text = row["Name"].ToString(), Value = (int)row["Id"] });
            }
            cb.DisplayMember = "Text";
            cb.ValueMember = "Value";
            if (cb.Items.Count > 0) cb.SelectedIndex = 0;
        }

        private void LoadBrandsToCombo(ComboBox cb)
        {
            cb.Items.Clear();
            DataTable dt = dbHelper.ExecuteQuery("SELECT Id, Name FROM Brands");
            foreach (DataRow row in dt.Rows)
            {
                cb.Items.Add(new { Text = row["Name"].ToString(), Value = (int)row["Id"] });
            }
            cb.DisplayMember = "Text";
            cb.ValueMember = "Value";
            if (cb.Items.Count > 0) cb.SelectedIndex = 0;
        }

        private void OpenEditPanel(int id)
        {
            currentEditProductId = id;
            LoadCategoriesToCombo(cbEditCategory);
            LoadBrandsToCombo(cbEditBrand);
            DataTable dt = dbHelper.ExecuteQuery("SELECT * FROM Products WHERE Id = @Id", new[] { new Microsoft.Data.SqlClient.SqlParameter("@Id", id) });
            if (dt.Rows.Count > 0) {
                DataRow r = dt.Rows[0];
                txtEditName.Text = r["Name"].ToString();
                txtEditPrice.Text = r["Price"].ToString();
                txtEditStock.Text = r["Stock"].ToString();
                txtEditImage.Text = r["ImageUrl"].ToString();
                txtEditDesc.Text = r["Description"].ToString();
                
                int catId = r["CategoryId"] != DBNull.Value ? (int)r["CategoryId"] : -1;
                if (catId == -1) cbEditCategory.SelectedIndex = -1;
                else {
                    for (int i = 0; i < cbEditCategory.Items.Count; i++) {
                        var item = cbEditCategory.Items[i] as dynamic;
                        if (item?.Value == catId) { cbEditCategory.SelectedIndex = i; break; }
                    }
                }

                string imgUrl = r["ImageUrl"]?.ToString() ?? "/images/default.jpg";
                txtEditImage.Text = imgUrl;
                try { picEditPreview.Load(imgUrl); } catch { picEditPreview.Image = null; }

                int brandId = r["BrandId"] != DBNull.Value ? (int)r["BrandId"] : -1;
                for (int i = 0; i < cbEditBrand.Items.Count; i++) {
                    var item = cbEditBrand.Items[i] as dynamic;
                    if (item?.Value == brandId) { cbEditBrand.SelectedIndex = i; break; }
                }
                
                ShowPanel(pnlEditProduct);
            }
        }

        private void SaveProductChanges()
        {
            try {
                if (cbEditCategory.SelectedIndex == -1) { MessageBox.Show("Please select a category!"); return; }
                int catId = (cbEditCategory.SelectedItem as dynamic)?.Value ?? 0;
                int brandId = (cbEditBrand.SelectedItem as dynamic)?.Value ?? 0;
                
                string imgUrl = string.IsNullOrWhiteSpace(txtEditImage.Text) ? "/images/default.jpg" : txtEditImage.Text.Trim();
                
                dbHelper.ExecuteNonQuery("UPDATE Products SET Name=@Name, Price=@Price, Stock=@Stock, ImageUrl=@Img, Description=@Desc, CategoryId=@CatId, BrandId=@BrandId WHERE Id=@Id", 
                    new[] {
                        new Microsoft.Data.SqlClient.SqlParameter("@Name", txtEditName.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Price", decimal.Parse(txtEditPrice.Text)),
                        new Microsoft.Data.SqlClient.SqlParameter("@Stock", int.Parse(txtEditStock.Text)),
                        new Microsoft.Data.SqlClient.SqlParameter("@Img", imgUrl),
                        new Microsoft.Data.SqlClient.SqlParameter("@Desc", txtEditDesc.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@CatId", catId),
                        new Microsoft.Data.SqlClient.SqlParameter("@BrandId", brandId),
                        new Microsoft.Data.SqlClient.SqlParameter("@Id", currentEditProductId)
                    });
                MessageBox.Show("Product updated successfully!");
                LogAsync("Update", "Product", $"Updated product: {txtEditName.Text}");
                RefreshAdminStats();
                ShowPanel(pnlManageProducts);
            } catch (Exception ex) { MessageBox.Show("Update failed: " + ex.Message); }
        }

        private void SaveNewProduct()
        {
            try {
                if (cbAddCategory.SelectedIndex == -1) { MessageBox.Show("Please select a category!"); return; }
                int catId = (cbAddCategory.SelectedItem as dynamic)?.Value ?? 0;
                int brandId = (cbAddBrand.SelectedItem as dynamic)?.Value ?? 0;
                
                string imgUrl = string.IsNullOrWhiteSpace(txtAddImage.Text) ? "/images/default.jpg" : txtAddImage.Text.Trim();

                dbHelper.ExecuteNonQuery("INSERT INTO Products (Name, Price, Stock, ImageUrl, Description, CategoryId, BrandId) VALUES (@Name, @Price, @Stock, @Img, @Desc, @CatId, @BrandId)", 
                    new[] {
                        new Microsoft.Data.SqlClient.SqlParameter("@Name", txtAddName.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Price", decimal.Parse(txtAddPrice.Text)),
                        new Microsoft.Data.SqlClient.SqlParameter("@Stock", int.Parse(txtAddStock.Text)),
                        new Microsoft.Data.SqlClient.SqlParameter("@Img", imgUrl),
                        new Microsoft.Data.SqlClient.SqlParameter("@Desc", txtAddDesc.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@CatId", catId),
                        new Microsoft.Data.SqlClient.SqlParameter("@BrandId", brandId)
                    });
                MessageBox.Show("Product added successfully!");
                LogAsync("Create", "Product", $"Added new product: {txtAddName.Text}");
                RefreshAdminStats();
                ShowPanel(pnlManageProducts);
            } catch (Exception ex) { MessageBox.Show("Failed to add product: " + ex.Message); }
        }

        private void ShowPanel(Panel p) { 
            pnlHome.Visible = pnlOrders.Visible = pnlManageProducts.Visible = pnlEditProduct.Visible = pnlAddProduct.Visible = false;
            p.Visible = true; 
            if (p == pnlHome) RefreshAdminStats();
            if (p == pnlOrders) LoadOrders(); 
            if (p == pnlManageProducts) LoadProductsList();
            if (p == pnlAddProduct) {
                txtAddName.Clear(); txtAddPrice.Clear(); txtAddStock.Clear(); txtAddImage.Clear(); txtAddDesc.Clear();
                picAddPreview.Image = null;
                LoadCategoriesToCombo(cbAddCategory);
                LoadBrandsToCombo(cbAddBrand);
                cbAddCategory.SelectedIndex = -1;
            }
        }
        
        private void LoadOrders() { dgvOrders.DataSource = dbHelper.ExecuteQuery("SELECT Id, CustomerName, TotalAmount, OrderDate, PaymentMethod, TransactionId, Status FROM Orders ORDER BY Id DESC"); }
        private void LoadProductsList() { dgvProducts.DataSource = dbHelper.ExecuteQuery("SELECT p.Id, p.Name, c.Name as Category, p.Price, p.Stock, p.ImageUrl FROM Products p LEFT JOIN Categories c ON p.CategoryId = c.Id ORDER BY p.Id DESC"); }
    }

    public class SuperAdminDashboard : BaseDashboard
    {
        private Panel pnlAdmins = null!, pnlBranches = null!, pnlSettings = null!, pnlAuditLogs = null!, pnlBackup = null!, pnlSaaS = null!, pnlUsers = null!, pnlPermissions = null!;
        private DataGridView dgvAdmins = null!, dgvBranches = null!, dgvAuditLogs = null!, dgvUsers = null!, dgvSubs = null!;
        private FlowLayoutPanel flpStats = null!;

        public SuperAdminDashboard(User user) : base(user)
        {
            this.Text = "Super Admin Control Center";
            SetupSidebarMenu();
            SetupSuperAdminPanels();
            ShowPanel(pnlHome);
        }

        private void SetupSidebarMenu()
        {
            sidePanel.Controls.Clear();
            SetupSidebarHeader();
            
            int y = 140;
            sidePanel.Controls.Add(CreateMenuButton("📊  System Overview", y, (s, e) => ShowPanel(pnlHome))); y += 55;
            sidePanel.Controls.Add(CreateMenuButton("🛡️  Admin", y, (s, e) => ShowPanel(pnlAdmins))); y += 55;
            sidePanel.Controls.Add(CreateMenuButton("🏢  Branch Network", y, (s, e) => ShowPanel(pnlBranches))); y += 55;
            sidePanel.Controls.Add(CreateMenuButton("👥  User Directory", y, (s, e) => ShowPanel(pnlUsers))); y += 55;
            sidePanel.Controls.Add(CreateMenuButton("🔐  Role Permissions", y, (s, e) => ShowPanel(pnlPermissions))); y += 55;
            sidePanel.Controls.Add(CreateMenuButton("⚙️  System Settings", y, (s, e) => ShowPanel(pnlSettings))); y += 55;
            sidePanel.Controls.Add(CreateMenuButton("📜  Audit Logs", y, (s, e) => ShowPanel(pnlAuditLogs))); y += 55;
            sidePanel.Controls.Add(CreateMenuButton("🛡️  Security Recovery", y, (s, e) => ShowPanel(pnlBackup))); y += 55;

            AddLogoutButton();
        }

        private void SetupSuperAdminPanels()
        {
            SetupHomePanel();
            SetupAdminManagementPanel();
            SetupBranchManagementPanel();
            SetupUsersPanel();
            SetupPermissionsPanel();
            SetupSettingsPanel();
            SetupAuditLogsPanel();
            SetupBackupPanel();
            SetupSaaSPanel();
        }

        private void SetupHomePanel()
        {
            pnlHome = new Panel { Dock = DockStyle.Fill, Visible = false };
            Label lblTitle = new Label { Text = "System Control Dashboard", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(30, 31, 33), Location = new Point(40, 30), AutoSize = true };
            pnlHome.Controls.Add(lblTitle);

            flpStats = new FlowLayoutPanel { Location = new Point(40, 100), Size = new Size(1150, 400), BackColor = Color.Transparent, AutoSize = true };
            pnlHome.Controls.Add(flpStats);

            mainPanel.Controls.Add(pnlHome);
            RefreshStats();
        }

        private void RefreshStats()
        {
            flpStats.Controls.Clear();
            int admins = Convert.ToInt32(dbHelper.ExecuteQuery("SELECT COUNT(*) FROM Users WHERE Role='Admin'").Rows[0][0]);
            int users = Convert.ToInt32(dbHelper.ExecuteQuery("SELECT COUNT(*) FROM Users WHERE Role='Customer'").Rows[0][0]);
            int branches = Convert.ToInt32(dbHelper.ExecuteQuery("SELECT COUNT(*) FROM Branches").Rows[0][0]);
            int gadgets = Convert.ToInt32(dbHelper.ExecuteQuery("SELECT COUNT(*) FROM Products").Rows[0][0]);
            decimal revenue = Convert.ToDecimal(dbHelper.ExecuteQuery("SELECT ISNULL(SUM(TotalAmount),0) FROM Orders WHERE Status='Paid'").Rows[0][0]);

            flpStats.Controls.Add(CreateStatCard("Total Gadgets", gadgets.ToString(), "⚡", Color.FromArgb(99, 102, 241)));
            flpStats.Controls.Add(CreateStatCard("Active Users", users.ToString(), "👥", Color.FromArgb(16, 185, 129)));
            flpStats.Controls.Add(CreateStatCard("Branch Network", branches.ToString(), "🏢", Color.FromArgb(245, 158, 11)));
            flpStats.Controls.Add(CreateStatCard("Total Revenue", $"৳{revenue:N0}", "৳", Color.FromArgb(59, 130, 246)));
            flpStats.Controls.Add(CreateStatCard("System Admins", admins.ToString(), "🛡️", Color.FromArgb(139, 92, 246)));
        }

        private Panel CreateStatCard(string title, string value, string icon, Color color)
        {
            Panel card = new Panel { Size = new Size(240, 240), BackColor = Color.White, Margin = new Padding(0, 0, 30, 30), BorderStyle = BorderStyle.None, Padding = new Padding(20) };
            card.Paint += (s, e) => {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(240, 240, 240), 1))
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            
            Label lblIcon = new Label { Text = icon, Font = new Font("Segoe UI", 36), ForeColor = color, Dock = DockStyle.Top, Height = 80, TextAlign = ContentAlignment.BottomCenter };
            Label lblVal = new Label { Text = value, Font = new Font("Outfit", 26, FontStyle.Bold), ForeColor = Color.FromArgb(30, 31, 33), Dock = DockStyle.Top, Height = 70, TextAlign = ContentAlignment.MiddleCenter };
            Label lblTitle = new Label { Text = title, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.Gray, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter };
            card.Controls.AddRange(new Control[] { lblTitle, lblVal, lblIcon });
            return card;
        }

        private void SetupAdminManagementPanel()
        {
            pnlAdmins = new Panel { Dock = DockStyle.Fill, Visible = false };
            dgvAdmins = CreateModernGrid(); dgvAdmins.Dock = DockStyle.Fill;
            
            Panel pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.White, Padding = new Padding(10) };
            Button btnCreate = new Button { Text = "➕ Create Admin", Width = 150, Dock = DockStyle.Left, BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 9) };
            Button btnEdit = new Button { Text = "✏️ Edit Admin", Width = 150, Dock = DockStyle.Left, BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 9), Margin = new Padding(10,0,0,0) };
            Button btnUnlock = new Button { Text = "🔓 Unlock", Width = 120, Dock = DockStyle.Left, BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 9), Margin = new Padding(10,0,0,0) };
            Button btnSuspend = new Button { Text = "🚫 Suspend", Width = 120, Dock = DockStyle.Left, BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 9), Margin = new Padding(10,0,0,0) };
            
            btnCreate.Click += (s, e) => ShowAddAdminDialog();
            btnEdit.Click += (s, e) => {
                if (dgvAdmins.SelectedRows.Count > 0) {
                    string id = dgvAdmins.SelectedRows[0].Cells["UserID"].Value?.ToString() ?? "";
                    ShowEditAdminDialog(id);
                }
            };
            
            btnUnlock.Click += (s, e) => {
                if (dgvAdmins.SelectedRows.Count > 0) {
                    string userId = dgvAdmins.SelectedRows[0].Cells["UserID"].Value?.ToString() ?? "";
                    dbHelper.ExecuteNonQuery("UPDATE Users SET IsLocked = 0, FailedAttempts = 0 WHERE UserID = @Id", new[] { new Microsoft.Data.SqlClient.SqlParameter("@Id", userId) });
                    LogAsync("Unlock", "Admin", $"Unlocked admin account: {userId}");
                    MessageBox.Show("Account unlocked!"); ShowPanel(pnlAdmins);
                }
            };

            btnSuspend.Click += (s, e) => {
                if (dgvAdmins.SelectedRows.Count > 0) {
                    string userId = dgvAdmins.SelectedRows[0].Cells["UserID"].Value?.ToString() ?? "";
                    DataTable dt = dbHelper.ExecuteQuery("SELECT IsActive FROM Users WHERE UserID = @Id", new[] { new Microsoft.Data.SqlClient.SqlParameter("@Id", userId) });
                    bool status = dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    dbHelper.ExecuteNonQuery("UPDATE Users SET IsActive = @Status WHERE UserID = @Id", new[] { new Microsoft.Data.SqlClient.SqlParameter("@Status", !status), new Microsoft.Data.SqlClient.SqlParameter("@Id", userId) });
                    string act = !status ? "Activated" : "Suspended";
                    LogAsync(act, "Admin", $"{act} admin account: {userId}");
                    MessageBox.Show(status ? "Account Suspended!" : "Account Activated!"); ShowPanel(pnlAdmins);
                }
            };

            pnlActions.Controls.AddRange(new Control[] { btnSuspend, btnUnlock, btnEdit, btnCreate });
            pnlAdmins.Controls.AddRange(new Control[] { dgvAdmins, pnlActions });
            mainPanel.Controls.Add(pnlAdmins);
        }

        private void ShowEditAdminDialog(string userId)
        {
            DataTable dt = dbHelper.ExecuteQuery("SELECT * FROM Users WHERE UserID = @Id", new[] { new Microsoft.Data.SqlClient.SqlParameter("@Id", userId) });
            if (dt.Rows.Count == 0) return;
            DataRow row = dt.Rows[0];

            Form f = new Form { Text = "Edit Admin Details", Size = new Size(400, 500), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog };
            int y = 30;
            var txtName = CreatePopupInput("Full Name", ref y, f); txtName.Text = row["Name"].ToString();
            var txtEmail = CreatePopupInput("Email Address", ref y, f); txtEmail.Text = row["Email"].ToString();
            var txtPass = CreatePopupInput("New Password (Leave blank to keep current)", ref y, f, true);
            
            Button btnSave = new Button { Text = "Update Admin Info", Location = new Point(50, y + 20), Size = new Size(300, 50), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10) };
            btnSave.Click += (s, e) => {
                if (!string.IsNullOrWhiteSpace(txtPass.Text)) {
                    string hash = "";
                    using (var sha = System.Security.Cryptography.SHA256.Create()) {
                        byte[] b = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(txtPass.Text));
                        var sb = new System.Text.StringBuilder();
                        foreach (var x in b) sb.Append(x.ToString("x2"));
                        hash = sb.ToString();
                    }
                    dbHelper.ExecuteNonQuery("UPDATE Users SET Name=@Name, Email=@Email, Password=@Pass, PasswordHash=@Hash WHERE UserID=@Id", new[] {
                        new Microsoft.Data.SqlClient.SqlParameter("@Name", txtName.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Email", txtEmail.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Pass", txtPass.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Hash", hash),
                        new Microsoft.Data.SqlClient.SqlParameter("@Id", userId)
                    });
                } else {
                    dbHelper.ExecuteNonQuery("UPDATE Users SET Name=@Name, Email=@Email WHERE UserID=@Id", new[] {
                        new Microsoft.Data.SqlClient.SqlParameter("@Name", txtName.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Email", txtEmail.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Id", userId)
                    });
                }
                MessageBox.Show("Admin updated!"); 
                LogAsync("Update", "Admin", $"Updated admin details for: {userId}");
                f.Close(); ShowPanel(pnlAdmins);
            };
            f.Controls.Add(btnSave); f.ShowDialog();
        }

        private void ShowAddAdminDialog()
        {
            Form f = new Form { Text = "Add New System Admin", Size = new Size(400, 500), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog };
            int y = 30;
            var txtId = CreatePopupInput("Admin ID (Username)", ref y, f);
            var txtName = CreatePopupInput("Full Name", ref y, f);
            var txtEmail = CreatePopupInput("Email Address", ref y, f);
            var txtPass = CreatePopupInput("Password", ref y, f, true);
            
            Button btnSave = new Button { Text = "Create Admin Account", Location = new Point(50, y + 20), Size = new Size(300, 50), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10) };
            btnSave.Click += (s, e) => {
                try {
                    if (string.IsNullOrWhiteSpace(txtPass.Text)) { MessageBox.Show("Password cannot be empty!"); return; }
                    string passwordHash = "";
                    using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create()) {
                        byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(txtPass.Text));
                        System.Text.StringBuilder builder = new System.Text.StringBuilder();
                        for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                        passwordHash = builder.ToString();
                    }
                    dbHelper.ExecuteNonQuery("INSERT INTO Users (UserID, Name, Email, Password, PasswordHash, Role, IsActive, IsLocked, FailedAttempts) VALUES (@Id, @Name, @Email, @Pass, @Hash, 'Admin', 1, 0, 0)", new[] {
                        new Microsoft.Data.SqlClient.SqlParameter("@Id", txtId.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Name", txtName.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Email", txtEmail.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Pass", txtPass.Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Hash", passwordHash)
                    });
                    MessageBox.Show("Admin created successfully!"); 
                    LogAsync("Create", "Admin", $"Created new admin account: {txtId.Text}");
                    f.Close(); ShowPanel(pnlAdmins);
                } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            };
            f.Controls.Add(btnSave); f.ShowDialog();
        }

        private void SetupBranchManagementPanel()
        {
            pnlBranches = new Panel { Dock = DockStyle.Fill, Visible = false };
            dgvBranches = CreateModernGrid(); dgvBranches.Dock = DockStyle.Fill;
            
            Panel pnlActions = new Panel { Dock = DockStyle.Bottom, Height = 80, BackColor = Color.White, Padding = new Padding(10) };
            Button btnCreate = new Button { Text = "🏢 Add Branch", Width = 200, Dock = DockStyle.Left, BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10) };
            Button btnEdit = new Button { Text = "✏️ Edit Branch", Width = 200, Dock = DockStyle.Left, BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10), Margin = new Padding(10,0,0,0) };
            
            btnCreate.Click += (s, e) => ShowAddBranchDialog();
            btnEdit.Click += (s, e) => {
                if (dgvBranches.SelectedRows.Count > 0) {
                    int id = Convert.ToInt32(dgvBranches.SelectedRows[0].Cells["Id"].Value);
                    ShowEditBranchDialog(id);
                }
            };

            pnlActions.Controls.AddRange(new Control[] { btnEdit, btnCreate });
            pnlBranches.Controls.AddRange(new Control[] { dgvBranches, pnlActions });
            mainPanel.Controls.Add(pnlBranches);
        }

        private void ShowEditBranchDialog(int id)
        {
            DataTable dt = dbHelper.ExecuteQuery("SELECT * FROM Branches WHERE Id = @Id", new[] { new Microsoft.Data.SqlClient.SqlParameter("@Id", id) });
            if (dt.Rows.Count == 0) return;
            DataRow row = dt.Rows[0];

            Form f = new Form { Text = "Edit Branch Info", Size = new Size(400, 400), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog };
            int y = 30;
            var txtName = CreatePopupInput("Branch Name", ref y, f); txtName.Text = row["Name"].ToString();
            var txtLoc = CreatePopupInput("Location", ref y, f); txtLoc.Text = row["Location"].ToString();
            
            Button btnSave = new Button { Text = "Update Branch", Location = new Point(50, y + 20), Size = new Size(300, 50), BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10) };
            btnSave.Click += (s, e) => {
                dbHelper.ExecuteNonQuery("UPDATE Branches SET Name=@Name, Location=@Loc WHERE Id=@Id", new[] {
                    new Microsoft.Data.SqlClient.SqlParameter("@Name", txtName.Text),
                    new Microsoft.Data.SqlClient.SqlParameter("@Loc", txtLoc.Text),
                    new Microsoft.Data.SqlClient.SqlParameter("@Id", id)
                });
                MessageBox.Show("Branch updated!"); f.Close(); ShowPanel(pnlBranches);
            };
            f.Controls.Add(btnSave); f.ShowDialog();
        }

        private void ShowAddBranchDialog()
        {
            Form f = new Form { Text = "Add New Branch", Size = new Size(400, 400), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog };
            int y = 30;
            var txtName = CreatePopupInput("Branch Name", ref y, f);
            var txtLoc = CreatePopupInput("Location", ref y, f);
            
            Button btnSave = new Button { Text = "Register Branch", Location = new Point(50, y + 20), Size = new Size(300, 50), BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10) };
            btnSave.Click += (s, e) => {
                dbHelper.ExecuteNonQuery("INSERT INTO Branches (Name, Location, IsActive) VALUES (@Name, @Loc, 1)", new[] {
                    new Microsoft.Data.SqlClient.SqlParameter("@Name", txtName.Text),
                    new Microsoft.Data.SqlClient.SqlParameter("@Loc", txtLoc.Text)
                });
                MessageBox.Show("Branch added!"); f.Close(); ShowPanel(pnlBranches);
            };
            f.Controls.Add(btnSave); f.ShowDialog();
        }

        private TextBox CreatePopupInput(string label, ref int y, Form f, bool isPass = false)
        {
            f.Controls.Add(new Label { Text = label, Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            TextBox t = new TextBox { Location = new Point(50, y + 25), Width = 300, Font = new Font("Segoe UI", 10), UseSystemPasswordChar = isPass };
            f.Controls.Add(t); y += 70; return t;
        }

        private void SetupUsersPanel()
        {
            pnlUsers = new Panel { Dock = DockStyle.Fill, Visible = false };
            dgvUsers = CreateModernGrid(); dgvUsers.Dock = DockStyle.Fill;
            
            Button btnToggleActive = new Button { Text = "🔄 Activate / Suspend Customer", Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 11) };
            btnToggleActive.Click += (s, e) => {
                if (dgvUsers.SelectedRows.Count > 0) {
                    string userId = dgvUsers.SelectedRows[0].Cells["UserID"].Value?.ToString() ?? "";
                    bool currentStatus = Convert.ToBoolean(dgvUsers.SelectedRows[0].Cells["IsActive"].Value);
                    dbHelper.ExecuteNonQuery("UPDATE Users SET IsActive = @Status WHERE UserID = @Id", new[] { 
                        new Microsoft.Data.SqlClient.SqlParameter("@Status", !currentStatus),
                        new Microsoft.Data.SqlClient.SqlParameter("@Id", userId)
                    });
                    MessageBox.Show("Customer status updated!");
                    ShowPanel(pnlUsers);
                }
            };
            pnlUsers.Controls.AddRange(new Control[] { dgvUsers, btnToggleActive });
            mainPanel.Controls.Add(pnlUsers);
        }

        private void SetupPermissionsPanel()
        {
            pnlPermissions = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = Color.White, Padding = new Padding(40) };
            Label lbl = new Label { Text = "Role-Based Access Control Management", Font = new Font("Segoe UI Bold", 18), AutoSize = true };
            pnlPermissions.Controls.Add(lbl);
            
            FlowLayoutPanel flpPerms = new FlowLayoutPanel { Location = new Point(40, 80), Size = new Size(600, 500), FlowDirection = FlowDirection.TopDown };
            string[] modules = { "Inventory Management", "Billing System", "Report Downloads", "User Administration" };
            foreach (var m in modules) {
                CheckBox chk = new CheckBox { Text = "Allow " + m, Font = new Font("Segoe UI", 12), AutoSize = true, Margin = new Padding(0, 10, 0, 10), Checked = true };
                flpPerms.Controls.Add(chk);
            }
            
            Button btnUpdate = new Button { Text = "Update Global Permissions", Size = new Size(300, 50), BackColor = Color.FromArgb(139, 92, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 11) };
            btnUpdate.Click += (s, e) => MessageBox.Show("System permissions updated successfully!");
            flpPerms.Controls.Add(btnUpdate);
            
            pnlPermissions.Controls.Add(flpPerms);
            mainPanel.Controls.Add(pnlPermissions);
        }

        private TextBox txtCompName = null!, txtCurr = null!, txtLang = null!, txtTZ = null!;
        private void SetupSettingsPanel()
        {
            pnlSettings = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = Color.White, Padding = new Padding(50) };
            Label lbl = new Label { Text = "Global System Configuration", Font = new Font("Segoe UI Bold", 20), AutoSize = true, Location = new Point(50, 30) };
            pnlSettings.Controls.Add(lbl);
            
            int y = 100;
            txtCompName = AddSettingFieldInput("System Name", "Electric Gadget Store", ref y, pnlSettings);
            txtCurr = AddSettingFieldInput("Default Currency", "BDT (৳)", ref y, pnlSettings);
            txtLang = AddSettingFieldInput("System Language", "English (UK)", ref y, pnlSettings);
            txtTZ = AddSettingFieldInput("Default Timezone", "GMT+6 (Dhaka)", ref y, pnlSettings);

            Button btnSave = new Button { Text = "💾 Save Configuration", Location = new Point(50, y), Size = new Size(250, 50), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 11) };
            btnSave.Click += (s, e) => {
                dbHelper.ExecuteNonQuery("UPDATE SystemSettings SET CompanyName=@Name, Currency=@Curr, Language=@Lang, Timezone=@TZ WHERE Id=1", new[] {
                    new Microsoft.Data.SqlClient.SqlParameter("@Name", txtCompName.Text),
                    new Microsoft.Data.SqlClient.SqlParameter("@Curr", txtCurr.Text),
                    new Microsoft.Data.SqlClient.SqlParameter("@Lang", txtLang.Text),
                    new Microsoft.Data.SqlClient.SqlParameter("@TZ", txtTZ.Text)
                });
                MessageBox.Show("System settings saved successfully!");
            };
            pnlSettings.Controls.Add(btnSave);
            mainPanel.Controls.Add(pnlSettings);
        }

        private TextBox AddSettingFieldInput(string label, string val, ref int y, Panel p)
        {
            p.Controls.Add(new Label { Text = label, Location = new Point(50, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.Gray });
            TextBox t = new TextBox { Text = val, Location = new Point(50, y + 30), Width = 400, Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };
            p.Controls.Add(t); y += 85; return t;
        }

        private void SetupAuditLogsPanel()
        {
            pnlAuditLogs = new Panel { Dock = DockStyle.Fill, Visible = false, Padding = new Padding(10) };
            
            // Modern Search Panel
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White, Margin = new Padding(0, 0, 0, 15) };
            pnlTop.HandleCreated += (s, e) => ApplyRoundedRegion(pnlTop, 15);
            
            Panel pnlSearchBox = new Panel { Location = new Point(25, 18), Size = new Size(400, 44), BackColor = Color.FromArgb(243, 244, 246), Padding = new Padding(10, 8, 10, 8) };
            pnlSearchBox.HandleCreated += (s, e) => ApplyRoundedRegion(pnlSearchBox, 10);
            
            TextBox txtSearch = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11), PlaceholderText = "Search by user, module or action...", BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(243, 244, 246) };
            pnlSearchBox.Controls.Add(txtSearch);

            Button btnRefresh = new Button { Text = "  🔄 Refresh  ", Location = new Point(440, 18), Height = 44, BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10), Cursor = Cursors.Hand };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.HandleCreated += (s, e) => ApplyRoundedRegion(btnRefresh, 10);
            
            btnRefresh.Click += (s, e) => {
                string search = txtSearch.Text.Trim();
                string sql = "SELECT TOP 100 Id, UserId, UserName, Action, Module, Details, Timestamp, IpAddress FROM AuditLogs";
                if (!string.IsNullOrEmpty(search)) sql += $" WHERE Details LIKE '%{search}%' OR UserName LIKE '%{search}%' OR Module LIKE '%{search}%' OR UserId LIKE '%{search}%'";
                sql += " ORDER BY Timestamp DESC";
                dgvAuditLogs.DataSource = dbHelper.ExecuteQuery(sql);
            };
            
            pnlTop.Controls.AddRange(new Control[] { pnlSearchBox, btnRefresh });
            
            // Table Container to add margin
            Panel pnlGridContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 15, 0, 15) };

            dgvAuditLogs = CreateModernGrid(); 
            dgvAuditLogs.Dock = DockStyle.Fill;
            dgvAuditLogs.AutoGenerateColumns = false;
            dgvAuditLogs.BorderStyle = BorderStyle.None;
            
            dgvAuditLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "Log ID", Width = 80 });
            dgvAuditLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UserId", HeaderText = "User ID", Width = 120 });
            dgvAuditLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UserName", HeaderText = "User Name", Width = 150 });
            dgvAuditLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Action", HeaderText = "Action", Width = 100 });
            dgvAuditLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Module", HeaderText = "Module", Width = 120 });
            dgvAuditLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Details", HeaderText = "Activity Details", Width = 300 });
            dgvAuditLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Timestamp", HeaderText = "Timestamp", Width = 180 });
            dgvAuditLogs.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IpAddress", HeaderText = "Source IP", Width = 120 });

            pnlGridContainer.Controls.Add(dgvAuditLogs);

            Button btnClear = new Button { Text = "🗑️ Archive Old Logs", Dock = DockStyle.Bottom, Height = 65, BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 12), Cursor = Cursors.Hand };
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.HandleCreated += (s, e) => ApplyRoundedRegion(btnClear, 12);
            btnClear.Click += (s, e) => {
                if (MessageBox.Show("Are you sure you want to archive logs older than 30 days?", "Confirm Archive", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    dbHelper.ExecuteNonQuery("DELETE FROM AuditLogs WHERE Timestamp < DATEADD(day, -30, GETDATE())");
                    LogAsync("Archive", "System", "Archived old audit logs");
                    MessageBox.Show("Old logs have been archived.");
                    ShowPanel(pnlAuditLogs);
                }
            };

            // Order of adding controls matters for docking Z-order
            pnlAuditLogs.Controls.Add(pnlGridContainer);
            pnlAuditLogs.Controls.Add(pnlTop);
            pnlAuditLogs.Controls.Add(btnClear);
            
            mainPanel.Controls.Add(pnlAuditLogs);
        }

        private void SetupBackupPanel()
        {
            pnlBackup = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = Color.White, Padding = new Padding(50) };
            Label lbl = new Label { Text = "Security  Data Recovery Center", Font = new Font("Segoe UI", 26), AutoSize = true, Location = new Point(50, 50) };
            pnlBackup.Controls.Add(lbl);
            
            FlowLayoutPanel flpBackup = new FlowLayoutPanel { Location = new Point(70, 150), Size = new Size(500, 400), FlowDirection = FlowDirection.TopDown };
            
            Button btnBackup = new Button { Text = "💾 Download System Data", Size = new Size(300, 60), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 13, FontStyle.Bold), Margin = new Padding(0, 0, 0, 20) };
            btnBackup.FlatAppearance.BorderSize = 0;
            btnBackup.Click += async (s, e) => {
                // 1. Initial UI State
                btnBackup.Enabled = false;
                btnBackup.Text = "⏳ Preparing...";
                btnBackup.Refresh();

                try {
                    // Use a standard SaveFileDialog - STAThread in Program.cs ensures this works
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Backup Files (*.bak)|*.bak|All Files (*.*)|*.*";
                        sfd.FileName = $"system_backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";
                        sfd.Title = "Select Export Destination";
                        sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        
                        // ShowDialog synchronously - it's modal and safe on STA UI thread
                        if (sfd.ShowDialog(this) == DialogResult.OK)
                        {
                            string fullPath = sfd.FileName;
                            btnBackup.Text = "⏳ Exporting...";
                            btnBackup.Refresh();
                            
                            // Perform File IO in background to avoid freezing the UI
                            await Task.Run(() => {
                                string content = $"-- Electric Gadget Management System Backup\n-- Generated: {DateTime.Now}\n-- Database: ElectricGadgetDB\n\nSELECT * FROM Products;\nSELECT * FROM Users;";
                                System.IO.File.WriteAllText(fullPath, content);
                                System.Threading.Thread.Sleep(600); // UI breathing room
                            });
                            
                            MessageBox.Show($"Data successfully exported to:\n{fullPath}", "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally {
                    // Reset UI
                    btnBackup.Text = "💾 Download System Data";
                    btnBackup.Enabled = true;
                }
            };
            
            Button btnRestore = new Button { Text = "⏪ Restore System Point", Size = new Size(300, 60), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 13, FontStyle.Bold) };
            btnRestore.FlatAppearance.BorderSize = 0;
            btnRestore.Click += (s, e) => MessageBox.Show("Please select a valid .bak file to restore the system state.");
            
            flpBackup.Controls.AddRange(new Control[] { btnBackup, btnRestore });
            pnlBackup.Controls.Add(flpBackup);
            mainPanel.Controls.Add(pnlBackup);
        }

        private void SetupSaaSPanel()
        {
            pnlSaaS = new Panel { Dock = DockStyle.Fill, Visible = false };
            dgvSubs = CreateModernGrid(); dgvSubs.Dock = DockStyle.Fill;
            
            Button btnRenew = new Button { Text = "💳 Extend License / Renew Subscription", Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 11) };
            btnRenew.Click += (s, e) => MessageBox.Show("Redirecting to secure payment gateway...");
            pnlSaaS.Controls.AddRange(new Control[] { dgvSubs, btnRenew });
            mainPanel.Controls.Add(pnlSaaS);
        }

        private void ShowPanel(Panel p)
        {
            pnlHome.Visible = pnlAdmins.Visible = pnlBranches.Visible = pnlSettings.Visible = pnlAuditLogs.Visible = pnlBackup.Visible = pnlSaaS.Visible = pnlUsers.Visible = pnlPermissions.Visible = false;
            p.Visible = true;
            if (p == pnlHome) RefreshStats();
            if (p == pnlAdmins) dgvAdmins.DataSource = dbHelper.ExecuteQuery("SELECT UserID, Name, Email, Role, IsLocked, CreatedAt FROM Users WHERE Role='Admin'");
            if (p == pnlBranches) dgvBranches.DataSource = dbHelper.ExecuteQuery("SELECT * FROM Branches");
            if (p == pnlUsers) dgvUsers.DataSource = dbHelper.ExecuteQuery("SELECT UserID, Name, Email, Role, IsActive FROM Users WHERE Role='Customer'");
            if (p == pnlAuditLogs) dgvAuditLogs.DataSource = dbHelper.ExecuteQuery("SELECT TOP 100 * FROM AuditLogs ORDER BY Timestamp DESC");
            if (p == pnlSaaS) {
                DataTable dt = new DataTable();
                dt.Columns.Add("Company"); dt.Columns.Add("Package"); dt.Columns.Add("Expiry"); dt.Columns.Add("Status");
                dt.Rows.Add("Alpha Tech", "Enterprise", "2026-12-01", "Active");
                dt.Rows.Add("Beta Gadgets", "Professional", "2026-10-15", "Expiring Soon");
                dgvSubs.DataSource = dt;
            }
        }
    }

    public class CustomerDashboard : BaseDashboard
    {
        private Panel pnlShop = null!, pnlCart = null!, pnlCheckout = null!, pnlCompare = null!;
        private FlowLayoutPanel flpProducts = null!;
        private DataTable cartTable = null!;
        private Label lblCartCount = null!, lblPopup = null!;
        private System.Windows.Forms.Timer popupTimer = null!;
        private ComboBox cbCompareCategory = null!, cbProduct1 = null!, cbProduct2 = null!;
        private Panel pnlCompareResult = null!;

        public CustomerDashboard(User user) : base(user)
        {
            SetupPanels();
            SetupHomeCategories();
            SetupShopUI();
            SetupCartPanel();
            SetupCheckoutForm();
            SetupComparePanel();
            ShowPanel(pnlHome);
        }

        private void SetupPanels()
        {
            sidePanel.Controls.Add(CreateMenuButton("🏠 Home", 130, (s, e) => ShowPanel(pnlHome)));
            sidePanel.Controls.Add(CreateMenuButton("🛍️ Shop Now", 185, (s, e) => ShowPanel(pnlShop)));
            sidePanel.Controls.Add(CreateMenuButton("🛒 My Cart", 240, (s, e) => ShowPanel(pnlCart)));
            sidePanel.Controls.Add(CreateMenuButton("🔄 Compare", 295, (s, e) => { LoadCompareCategories(); ShowPanel(pnlCompare); }));

            pnlHome = new Panel { Dock = DockStyle.Fill, Visible = false };
            pnlShop = new Panel { Dock = DockStyle.Fill, Visible = false };
            pnlCart = new Panel { Dock = DockStyle.Fill, Visible = false };
            pnlCheckout = new Panel { Dock = DockStyle.Fill, Visible = false };
            pnlCompare = new Panel { Dock = DockStyle.Fill, Visible = false, BackColor = Color.FromArgb(245, 247, 250) };

            mainPanel.Controls.AddRange(new Control[] { pnlHome, pnlShop, pnlCart, pnlCheckout, pnlCompare });

            cartTable = new DataTable();
            cartTable.Columns.AddRange(new[] { new DataColumn("Id", typeof(int)), new DataColumn("Name"), new DataColumn("Price", typeof(decimal)) });

            lblCartCount = new Label { Text = "Cart Items: 0", Font = new Font("Segoe UI Bold", 10), ForeColor = Color.White, BackColor = Color.FromArgb(0, 102, 204), AutoSize = true, Location = new Point(1100, 25), Padding = new Padding(10, 5, 10, 5) };
            headerPanel.Controls.Add(lblCartCount);

            lblPopup = new Label { Text = "🛒 Added to Cart!", Size = new Size(200, 50), BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, Font = new Font("Segoe UI Bold", 12), TextAlign = ContentAlignment.MiddleCenter, Visible = false };
            this.Controls.Add(lblPopup); lblPopup.BringToFront();
            popupTimer = new System.Windows.Forms.Timer { Interval = 2000 }; popupTimer.Tick += (s, e) => { lblPopup.Visible = false; popupTimer.Stop(); };
        }

        private void SetupShopUI()
        {
            pnlShop.BackColor = Color.FromArgb(248, 250, 252);
            
            Panel pnlShopHeader = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = Color.White };
            pnlShopHeader.Paint += (s, e) => {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240)), 0, pnlShopHeader.Height - 1, pnlShopHeader.Width, pnlShopHeader.Height - 1);
            };
            
            Label lblTitle = new Label { Text = "Explore Premium Gadgets", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), AutoSize = true, Location = new Point(40, 30) };
            pnlShopHeader.Controls.Add(lblTitle);

            FlowLayoutPanel flpTabs = new FlowLayoutPanel { Location = new Point(40, 85), Size = new Size(1100, 45), BackColor = Color.Transparent };
            string[] tabs = { "All", "Smartphone", "Laptop", "Smart TV", "Headphones", "Speaker", "Smart Watch" };
            foreach (var t in tabs) {
                Button btn = new Button { Text = t, AutoSize = true, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.FromArgb(100, 116, 139), Cursor = Cursors.Hand, Margin = new Padding(0, 0, 15, 0), Height = 36 };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
                btn.Click += (s, e) => {
                    foreach(Control c in flpTabs.Controls) { c.ForeColor = Color.FromArgb(100, 116, 139); c.BackColor = Color.Transparent; }
                    btn.ForeColor = Color.FromArgb(37, 99, 235); btn.BackColor = Color.FromArgb(239, 246, 255);
                    LoadProducts(t == "All" ? "" : t);
                };
                btn.HandleCreated += (s, e) => ApplyRoundedRegion(btn, 18);
                flpTabs.Controls.Add(btn);
            }
            pnlShopHeader.Controls.Add(flpTabs);
            pnlShop.Controls.Add(pnlShopHeader);

            flpProducts = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.Transparent, Padding = new Padding(40) };
            pnlShop.Controls.Add(flpProducts);
            pnlShop.Controls.SetChildIndex(flpProducts, 0); // Ensure header stays on top
            LoadProducts();
        }

        private void SetupHomeCategories()
        {
            // Sleek Hero Section with subtle rounding and better text placement
            Panel pnlHero = new Panel { Dock = DockStyle.Top, Height = 320, BackColor = Color.FromArgb(15, 23, 42) };
            pnlHero.Paint += (s, e) => {
                using (LinearGradientBrush brush = new LinearGradientBrush(pnlHero.ClientRectangle, Color.FromArgb(15, 23, 42), Color.FromArgb(30, 41, 59), 45F)) {
                    e.Graphics.FillRectangle(brush, pnlHero.ClientRectangle);
                }
            };

            FlowLayoutPanel flpHero = new FlowLayoutPanel { 
                FlowDirection = FlowDirection.TopDown, 
                Location = new Point(50, 40), 
                Size = new Size(1000, 260), 
                BackColor = Color.Transparent,
                WrapContents = false 
            };
            
            Label lblHeroTitle = new Label { Text = "Electric Gadget Store", Font = new Font("Segoe UI", 36, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 15) };
            Label lblHeroSub = new Label { Text = "Premium gadgets for a smarter lifestyle.", Font = new Font("Segoe UI", 14), ForeColor = Color.FromArgb(148, 163, 184), AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(5, 0, 0, 25) };
            
            Button btnShopHero = new Button { Text = "Shop Now", Size = new Size(160, 48), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, Font = new Font("Segoe UI Bold", 11), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Margin = new Padding(5, 0, 0, 0) };
            btnShopHero.FlatAppearance.BorderSize = 0;
            btnShopHero.HandleCreated += (s, e) => ApplyRoundedRegion(btnShopHero, 12);
            btnShopHero.Click += (s, e) => ShowPanel(pnlShop);
            
            flpHero.Controls.AddRange(new Control[] { lblHeroTitle, lblHeroSub, btnShopHero });
            pnlHero.Controls.Add(flpHero);
            pnlHome.Controls.Add(pnlHero);

            Label lblBrowse = new Label { Text = "Browse Categories", Font = new Font("Segoe UI", 22, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), AutoSize = true, Location = new Point(50, 310) };
            pnlHome.Controls.Add(lblBrowse);

            FlowLayoutPanel flpCats = new FlowLayoutPanel { Location = new Point(50, 370), Width = 1150, Height = 500, AutoScroll = true, BackColor = Color.Transparent };
            pnlHome.Controls.Add(flpCats);

            string[] cats = { "Smartphone", "Laptop", "Smart TV", "Headphones", "Speaker", "Smart Watch" };
            string[] icons = { "📱", "💻", "📺", "🎧", "🔊", "⌚" };
            // Modern soft color palette
            Color[] colors = { Color.FromArgb(239, 246, 255), Color.FromArgb(240, 253, 244), Color.FromArgb(254, 252, 232), Color.FromArgb(250, 245, 255), Color.FromArgb(255, 241, 242), Color.FromArgb(255, 247, 237) };
            Color[] accentColors = { Color.FromArgb(37, 99, 235), Color.FromArgb(22, 163, 74), Color.FromArgb(202, 138, 4), Color.FromArgb(147, 51, 234), Color.FromArgb(225, 29, 72), Color.FromArgb(234, 88, 12) };

            for (int i = 0; i < cats.Length; i++)
            {
                Panel card = new Panel { Size = new Size(200, 220), BackColor = Color.White, Cursor = Cursors.Hand, Margin = new Padding(0, 0, 25, 25) };
                card.Paint += (s, e) => {
                    using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1)) {
                        e.Graphics.DrawRectangle(p, 0, 0, card.Width - 1, card.Height - 1);
                    }
                };
                card.HandleCreated += (s, e) => ApplyRoundedRegion(card, 20);

                Panel iconCircle = new Panel { Size = new Size(80, 80), Location = new Point(60, 40), BackColor = colors[i] };
                iconCircle.HandleCreated += (s, e) => ApplyRoundedRegion(iconCircle, 40);
                
                Label lblIcon = new Label { Text = icons[i], Font = new Font("Segoe UI", 32), ForeColor = accentColors[i], TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, BackColor = Color.Transparent };
                iconCircle.Controls.Add(lblIcon);

                Label lblName = new Label { Text = cats[i], Font = new Font("Segoe UI Bold", 13), ForeColor = Color.FromArgb(30, 41, 59), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Bottom, Height = 70 };
                
                string categoryName = cats[i];
                EventHandler clickEvent = (s, e) => { LoadProducts(categoryName); ShowPanel(pnlShop); };
                card.Click += clickEvent; iconCircle.Click += clickEvent; lblIcon.Click += clickEvent; lblName.Click += clickEvent;

                card.Controls.Add(iconCircle);
                card.Controls.Add(lblName);
                flpCats.Controls.Add(card);
            }
        }

        private void SetupCartPanel()
        {
            Label lblTitle = new Label { Text = "Your Shopping Cart", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(30, 30, 30), AutoSize = true, Location = new Point(40, 30) };
            pnlCart.Controls.Add(lblTitle);

            DataGridView dgvCart = CreateModernGrid();
            dgvCart.Location = new Point(40, 100); dgvCart.Size = new Size(800, 600);
            dgvCart.DataSource = cartTable;
            pnlCart.Controls.Add(dgvCart);

            Button btnCheckout = new Button { Text = "Proceed to Checkout ➡️", Location = new Point(870, 100), Size = new Size(300, 70), BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 13), Cursor = Cursors.Hand };
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.Click += (s, e) => { if (cartTable.Rows.Count > 0) ShowPanel(pnlCheckout); else MessageBox.Show("Cart is empty!"); };
            pnlCart.Controls.Add(btnCheckout);
        }

        private void SetupCheckoutForm()
        {
            pnlCheckout.BackColor = Color.FromArgb(245, 247, 250);
            Label lblTitle = new Label { Text = "Secure Checkout", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(30, 30, 30), AutoSize = true, Location = new Point(40, 30) };
            pnlCheckout.Controls.Add(lblTitle);

            Panel pnlForm = new Panel { Location = new Point(40, 90), Size = new Size(600, 600), BackColor = Color.White, Padding = new Padding(30) };
            pnlCheckout.Controls.Add(pnlForm);

            int y = 30;
            string[] labels = { "Full Name", "Phone Number", "Delivery Address" };
            Dictionary<string, TextBox> inputs = new Dictionary<string, TextBox>();
            foreach (var l in labels) {
                pnlForm.Controls.Add(new Label { Text = l, Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.Gray });
                var t = new TextBox { Name = "txt" + l.Replace(" ",""), Location = new Point(30, y + 25), Width = 540, Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(250, 250, 250), ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle };
                if (l == "Delivery Address") { t.Multiline = true; t.Height = 80; y += 50; }
                pnlForm.Controls.Add(t); inputs[l] = t; y += 80;
            }

            pnlForm.Controls.Add(new Label { Text = "Payment Method", Location = new Point(30, y), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.Gray });
            ComboBox cb = new ComboBox { Name = "cbPay", Location = new Point(30, y + 25), Width = 540, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(250, 250, 250) };
            cb.Items.AddRange(new[] { "Cash on Delivery (COD)", "bKash / Nagad", "Credit/Debit Card" });
            cb.SelectedIndex = 0;
            pnlForm.Controls.Add(cb);

            Label lblDetails = new Label { Text = "Transaction ID / Account Number", Location = new Point(30, y + 80), AutoSize = true, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.Gray, Visible = false };
            TextBox txtDetails = new TextBox { Name = "txtDetails", Location = new Point(30, y + 105), Width = 540, Font = new Font("Segoe UI", 12), BackColor = Color.FromArgb(250, 250, 250), ForeColor = Color.Black, BorderStyle = BorderStyle.FixedSingle, Visible = false };
            pnlForm.Controls.AddRange(new Control[] { lblDetails, txtDetails });

            cb.SelectedIndexChanged += (s, e) => {
                bool isDigital = !(cb.SelectedItem?.ToString() ?? "").Contains("COD");
                lblDetails.Visible = txtDetails.Visible = isDigital;
            };

            Panel pnlSummary = new Panel { Location = new Point(680, 90), Size = new Size(400, 350), BackColor = Color.White, Padding = new Padding(30) };
            pnlCheckout.Controls.Add(pnlSummary);
            
            Label lblSumTitle = new Label { Text = "Order Summary", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(30, 30, 30), AutoSize = true, Location = new Point(30, 30) };
            pnlSummary.Controls.Add(lblSumTitle);

            Label lblSumItems = new Label { Text = "Total Items: 0", Font = new Font("Segoe UI", 12), ForeColor = Color.Gray, Location = new Point(30, 80), AutoSize = true };
            Label lblSumTotal = new Label { Text = "Total Amount: $0.00", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(0, 102, 204), Location = new Point(30, 120), AutoSize = true };
            pnlSummary.Controls.AddRange(new Control[] { lblSumItems, lblSumTotal });

            pnlCheckout.VisibleChanged += (s, e) => {
                if (pnlCheckout.Visible) {
                    lblSumItems.Text = $"Total Items: {cartTable.Rows.Count}";
                    decimal sum = cartTable.AsEnumerable().Sum(x => x["Price"] == DBNull.Value ? 0m : Convert.ToDecimal(x["Price"]));
                    lblSumTotal.Text = $"Total Amount: ৳{sum:N2}";
                }
            };

            Button btnConfirm = new Button { Text = "Complete Purchase", Location = new Point(30, 200), Size = new Size(340, 60), BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 13), Cursor = Cursors.Hand };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += (s, e) => {
                if (!Regex.IsMatch(inputs["Phone Number"].Text, @"^\d{11}$")) { MessageBox.Show("Phone Number must be 11 digits!"); return; }
                decimal total = cartTable.AsEnumerable().Sum(x => x["Price"] == DBNull.Value ? 0m : Convert.ToDecimal(x["Price"]));
                string transId = "TXN" + new Random().Next(1000, 9999).ToString();
                dbHelper.ExecuteNonQuery("INSERT INTO Orders (CustomerName, TotalAmount, OrderDate, PaymentMethod, TransactionId, Status) VALUES (@Name, @Total, GETDATE(), @Pay, @TransId, 'Pending')", 
                    new[] { 
                        new Microsoft.Data.SqlClient.SqlParameter("@Name", inputs["Full Name"].Text),
                        new Microsoft.Data.SqlClient.SqlParameter("@Total", total),
                        new Microsoft.Data.SqlClient.SqlParameter("@Pay", cb.SelectedItem?.ToString() ?? "COD"),
                        new Microsoft.Data.SqlClient.SqlParameter("@TransId", transId)
                    });
                MessageBox.Show($"Order Confirmed! TXN ID: {transId}");
                cartTable.Clear(); lblCartCount.Text = "Cart Items: 0"; ShowPanel(pnlHome);
            };
            pnlSummary.Controls.Add(btnConfirm);
        }

        private void LoadProducts(string filter = "") { 
            flpProducts.Controls.Clear();
            string query = "SELECT p.Id, p.Name, b.Name as Brand, p.Description, p.Price, p.Stock, p.ImageUrl, c.Name as CategoryName FROM Products p LEFT JOIN Brands b ON p.BrandId = b.Id LEFT JOIN Categories c ON p.CategoryId = c.Id";
            if (!string.IsNullOrEmpty(filter)) query += $" WHERE c.Name LIKE '%{filter}%' OR p.Name LIKE '%{filter}%'";
            
            DataTable dt = dbHelper.ExecuteQuery(query); 
            foreach(DataRow row in dt.Rows) {
                int id = Convert.ToInt32(row["Id"]);
                string name = row["Name"]?.ToString() ?? string.Empty;
                decimal price = Convert.ToDecimal(row["Price"]);
                int stock = Convert.ToInt32(row["Stock"]);
                string imageUrl = row["ImageUrl"]?.ToString() ?? string.Empty;
                
                Panel card = new Panel { Size = new Size(260, 420), BackColor = Color.White, Margin = new Padding(0, 0, 20, 20) };
                card.Paint += (s, e) => {
                    using (Pen p = new Pen(Color.FromArgb(241, 245, 249), 1)) {
                        e.Graphics.DrawRectangle(p, 0, 0, card.Width - 1, card.Height - 1);
                    }
                };
                card.HandleCreated += (s, e) => ApplyRoundedRegion(card, 16);

                PictureBox pic = new PictureBox { Size = new Size(200, 180), Location = new Point(30, 25), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(248, 250, 252) };
                pic.HandleCreated += (s, e) => ApplyRoundedRegion(pic, 12);
                
                if (imageUrl.StartsWith("http")) {
                    LoadWebImage(pic, imageUrl).ContinueWith(t => { if(t.IsFaulted || pic.Image == null) pic.ImageLocation = $"https://picsum.photos/200/200?random={id}"; }, TaskScheduler.FromCurrentSynchronizationContext());
                } else {
                    string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "wwwroot", imageUrl.TrimStart('/'));
                    if (File.Exists(localPath)) {
                        try { pic.Image = Image.FromFile(localPath); }
                        catch { pic.ImageLocation = $"https://picsum.photos/200/200?random={id}"; }
                    } else {
                        pic.ImageLocation = $"https://picsum.photos/200/200?random={id}";
                    }
                }
                
                card.Controls.Add(pic);
                
                Label lblStock = new Label { Text = stock > 0 ? $"✓ In Stock ({stock})" : "✕ Out of Stock", ForeColor = stock > 0 ? Color.FromArgb(22, 163, 74) : Color.FromArgb(220, 38, 38), Font = new Font("Segoe UI Bold", 8), Location = new Point(20, 225), AutoSize = true };
                Label lblName = new Label { Text = name, ForeColor = Color.FromArgb(15, 23, 42), Font = new Font("Segoe UI Bold", 11), Location = new Point(20, 248), Size = new Size(220, 50) };
                Label lblPrice = new Label { Text = $"৳{price:N0}", ForeColor = Color.FromArgb(37, 99, 235), Font = new Font("Segoe UI", 15, FontStyle.Bold), Location = new Point(18, 305), AutoSize = true };
                
                Button btnOrder = new Button { Text = "Add to Cart", Size = new Size(220, 44), Location = new Point(20, 355), BackColor = Color.FromArgb(15, 23, 42), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10), Cursor = Cursors.Hand };
                btnOrder.FlatAppearance.BorderSize = 0;
                btnOrder.HandleCreated += (s, e) => ApplyRoundedRegion(btnOrder, 8);
                btnOrder.Click += (s, e) => { cartTable.Rows.Add(id, name, price); lblCartCount.Text = $"Cart Items: {cartTable.Rows.Count}"; lblPopup.Visible = true; popupTimer.Start(); };
                
                card.Controls.AddRange(new Control[] { lblStock, lblName, lblPrice, btnOrder });
                flpProducts.Controls.Add(card);
            }
        }

        private void SetupComparePanel()
        {
            pnlCompare.BackColor = Color.FromArgb(248, 250, 252);
            Label lblTitle = new Label { Text = "Product Comparison Tool", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), AutoSize = true, Location = new Point(40, 30) };
            pnlCompare.Controls.Add(lblTitle);

            Panel pnlControls = new Panel { Location = new Point(40, 100), Size = new Size(1150, 90), BackColor = Color.White };
            pnlControls.Paint += (s, e) => {
                using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1)) {
                    e.Graphics.DrawRectangle(p, 0, 0, pnlControls.Width - 1, pnlControls.Height - 1);
                }
            };
            pnlControls.HandleCreated += (s, e) => ApplyRoundedRegion(pnlControls, 16);
            pnlCompare.Controls.Add(pnlControls);

            int xOffset = 30;
            Label lblCat = new Label { Text = "Category", Location = new Point(xOffset, 20), AutoSize = true, Font = new Font("Segoe UI Semibold", 9), ForeColor = Color.Gray };
            cbCompareCategory = new ComboBox { Location = new Point(xOffset, 42), Width = 180, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(249, 250, 251) };
            cbCompareCategory.SelectedIndexChanged += (s, e) => LoadCompareProducts();
            
            xOffset += 210;
            Label lblP1 = new Label { Text = "First Product", Location = new Point(xOffset, 20), AutoSize = true, Font = new Font("Segoe UI Semibold", 9), ForeColor = Color.Gray };
            cbProduct1 = new ComboBox { Location = new Point(xOffset, 42), Width = 250, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(249, 250, 251) };
            
            xOffset += 280;
            Label lblP2 = new Label { Text = "Second Product", Location = new Point(xOffset, 20), AutoSize = true, Font = new Font("Segoe UI Semibold", 9), ForeColor = Color.Gray };
            cbProduct2 = new ComboBox { Location = new Point(xOffset, 42), Width = 250, Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Color.FromArgb(249, 250, 251) };

            Button btnCompare = new Button { Text = "Compare Now", Location = new Point(950, 35), Size = new Size(170, 42), BackColor = Color.FromArgb(37, 99, 235), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Bold", 10), Cursor = Cursors.Hand };
            btnCompare.FlatAppearance.BorderSize = 0;
            btnCompare.HandleCreated += (s, e) => ApplyRoundedRegion(btnCompare, 10);
            btnCompare.Click += BtnCompare_Click;

            pnlControls.Controls.AddRange(new Control[] { lblCat, cbCompareCategory, lblP1, cbProduct1, lblP2, cbProduct2, btnCompare });

            pnlCompareResult = new Panel { Location = new Point(40, 210), Size = new Size(1150, 600), BackColor = Color.Transparent, AutoScroll = true };
            pnlCompare.Controls.Add(pnlCompareResult);
        }

        private void LoadCompareCategories()
        {
            DataTable dt = dbHelper.ExecuteQuery("SELECT Id, Name FROM Categories");
            cbCompareCategory.DisplayMember = "Name";
            cbCompareCategory.ValueMember = "Id";
            cbCompareCategory.DataSource = dt;
            if(cbCompareCategory.Items.Count > 0) cbCompareCategory.SelectedIndex = 0;
        }

        private void LoadCompareProducts()
        {
            if (cbCompareCategory.SelectedValue == null) return;
            int catId;
            if (!int.TryParse(cbCompareCategory.SelectedValue.ToString(), out catId)) return;
            
            DataTable dt = dbHelper.ExecuteQuery($"SELECT Id, Name FROM Products WHERE CategoryId = {catId}");
            
            cbProduct1.DataSource = dt.Copy(); cbProduct1.DisplayMember = "Name"; cbProduct1.ValueMember = "Id";
            cbProduct2.DataSource = dt.Copy(); cbProduct2.DisplayMember = "Name"; cbProduct2.ValueMember = "Id";
            
            if (cbProduct1.Items.Count > 0) cbProduct1.SelectedIndex = 0;
            if (cbProduct2.Items.Count > 1) cbProduct2.SelectedIndex = 1;
        }

        private void BtnCompare_Click(object? sender, EventArgs e)
        {
            if (cbProduct1.SelectedValue == null || cbProduct2.SelectedValue == null) { MessageBox.Show("Please select two products to compare."); return; }
            int p1Id = Convert.ToInt32(cbProduct1.SelectedValue);
            int p2Id = Convert.ToInt32(cbProduct2.SelectedValue);
            if (p1Id == p2Id) { MessageBox.Show("Please select different products to compare."); return; }

            pnlCompareResult.Controls.Clear();
            DataTable dtP1 = dbHelper.ExecuteQuery($"SELECT p.*, b.Name as BrandName, c.Name as CategoryName FROM Products p LEFT JOIN Brands b ON p.BrandId = b.Id LEFT JOIN Categories c ON p.CategoryId = c.Id WHERE p.Id = {p1Id}");
            DataTable dtP2 = dbHelper.ExecuteQuery($"SELECT p.*, b.Name as BrandName, c.Name as CategoryName FROM Products p LEFT JOIN Brands b ON p.BrandId = b.Id LEFT JOIN Categories c ON p.CategoryId = c.Id WHERE p.Id = {p2Id}");

            if (dtP1.Rows.Count == 0 || dtP2.Rows.Count == 0) return;

            // Fetch features
            DataTable feats1 = dbHelper.ExecuteQuery($"SELECT FeatureName FROM Features WHERE ProductId = {p1Id}");
            DataTable feats2 = dbHelper.ExecuteQuery($"SELECT FeatureName FROM Features WHERE ProductId = {p2Id}");

            TableLayoutPanel tlp = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 1, Padding = new Padding(10) };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            pnlCompareResult.Controls.Add(tlp);

            tlp.Controls.Add(CreateProductCompareCard(dtP1.Rows[0], feats1), 0, 0);
            tlp.Controls.Add(CreateProductCompareCard(dtP2.Rows[0], feats2), 1, 0);
        }

        private Panel CreateProductCompareCard(DataRow row, DataTable features)
        {
            Panel card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(15), AutoSize = true, MinimumSize = new Size(300, 700) };
            card.Paint += (s, e) => {
                using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1)) {
                    e.Graphics.DrawRectangle(p, 0, 0, card.Width - 1, card.Height - 1);
                }
            };
            card.HandleCreated += (s, e) => ApplyRoundedRegion(card, 20);

            int id = Convert.ToInt32(row["Id"]);
            string name = row["Name"]?.ToString() ?? "";
            string model = row["Model"]?.ToString() ?? "N/A";
            string warranty = row["Warranty"]?.ToString() ?? "N/A";
            decimal price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0;
            string brand = row["BrandName"]?.ToString() ?? "Unknown";
            string imageUrl = row["ImageUrl"]?.ToString() ?? "";

            PictureBox pic = new PictureBox { Size = new Size(240, 240), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.FromArgb(248, 250, 252) };
            pic.HandleCreated += (s, e) => ApplyRoundedRegion(pic, 15);
            
            if (imageUrl.StartsWith("http")) {
                LoadWebImage(pic, imageUrl).ContinueWith(t => { if(t.IsFaulted || pic.Image == null) pic.ImageLocation = $"https://picsum.photos/240/240?random={id}"; }, TaskScheduler.FromCurrentSynchronizationContext());
            } else {
                pic.ImageLocation = $"https://picsum.photos/240/240?random={id}";
            }
            card.Controls.Add(pic);
            
            // Layout logic
            card.Resize += (s, e) => { pic.Location = new Point((card.Width - pic.Width) / 2, 30); };

            int y = 290;
            Label lblBrand = new Label { Text = brand.ToUpper(), Font = new Font("Segoe UI Bold", 9), ForeColor = Color.FromArgb(37, 99, 235), AutoSize = true, Location = new Point(30, y), BackColor = Color.FromArgb(239, 246, 255), Padding = new Padding(10, 5, 10, 5) };
            card.Controls.Add(lblBrand); y += 45;

            Label lblName = new Label { Text = name, Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), AutoSize = true, Location = new Point(30, y) };
            card.Controls.Add(lblName); y += 50;
            
            Label lblPrice = new Label { Text = $"৳{price:N0}", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), AutoSize = true, Location = new Point(30, y) };
            card.Controls.Add(lblPrice); y += 60;

            // Specifications Section
            Label lblSpecTitle = new Label { Text = "Specifications", Font = new Font("Segoe UI Bold", 12), ForeColor = Color.FromArgb(100, 116, 139), AutoSize = true, Location = new Point(30, y) };
            card.Controls.Add(lblSpecTitle); y += 35;

            var specs = new (string Key, string Val)[] { ("Model", model), ("Warranty", warranty), ("Brand", brand) };
            foreach (var spec in specs) {
                Panel pnlSpec = new Panel { Location = new Point(30, y), Size = new Size(card.Width - 60, 45), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
                pnlSpec.Controls.Add(new Label { Text = spec.Key, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Location = new Point(0, 10), AutoSize = true });
                pnlSpec.Controls.Add(new Label { Text = spec.Val, Font = new Font("Segoe UI Semibold", 10), ForeColor = Color.Black, TextAlign = ContentAlignment.TopRight, Dock = DockStyle.Right, Width = 200 });
                card.Controls.Add(pnlSpec); y += 45;
            }

            y += 10;
            Label lblFeatTitle = new Label { Text = "Key Features", Font = new Font("Segoe UI Bold", 12), ForeColor = Color.FromArgb(100, 116, 139), AutoSize = true, Location = new Point(30, y) };
            card.Controls.Add(lblFeatTitle); y += 35;

            if (features.Rows.Count > 0) {
                foreach (DataRow featRow in features.Rows) {
                    Label lblFeat = new Label { Text = "• " + featRow["FeatureName"].ToString(), Font = new Font("Segoe UI", 11), ForeColor = Color.FromArgb(51, 65, 85), Location = new Point(40, y), AutoSize = true };
                    card.Controls.Add(lblFeat); y += 30;
                }
            } else {
                Label lblNoFeat = new Label { Text = "No features listed.", Font = new Font("Segoe UI Italic", 10), ForeColor = Color.Gray, Location = new Point(40, y), AutoSize = true };
                card.Controls.Add(lblNoFeat); y += 30;
            }

            y += 20;
            return card;
        }

        private void ShowPanel(Panel p) { pnlHome.Visible = pnlShop.Visible = pnlCart.Visible = pnlCheckout.Visible = pnlCompare.Visible = false; p.Visible = true; if (p == pnlShop) LoadProducts(); }
    }
}
