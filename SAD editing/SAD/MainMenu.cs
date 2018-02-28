﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace SAD
{
    public partial class MainMenu : Form
    {
        MySqlConnection conn;

        public Form prevForm { get; set; }

        public MainMenu()
        {
            InitializeComponent();
            conn = new MySqlConnection("SERVER=localhost; DATABASE=cafetiere; uid=root; pwd=root;");
            lbl_user.Text = "Welcome! ";
            lbl_type.Text = SAD.Login.DisplayUserDetails.usertype;
            lbl_name.Text = SAD.Login.DisplayUserDetails.name;
        }
        
        private Timer tm = new Timer();

        void tm_Tick(object sender, EventArgs e)
        {
            timeText.Text = DateTime.Now.ToLongTimeString();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            tm.Tick += new EventHandler(tm_Tick);
            tm.Interval = 1000;
            tm.Enabled = true;

            timeText.Text = DateTime.Now.ToLongTimeString();

            btnDash.BackColor = Color.FromArgb(192, 57, 43);

            if (SAD.Login.DisplayUserDetails.usertype.ToLower() == "staff")
            {
                btn_profiling.Enabled = false;
                btn_product.Enabled = false;
                btn_profiling.BackColor = Color.Gray;
                btn_product.BackColor = Color.Gray;
            }

            logDate();
            restock();
            notif_expiry();
        }

        public void notif_expiry()
        {
            String notifexpiryquery = "SELECT SUM(itemQuantity) as num_expired FROM items_inventory " +
                                      "WHERE current_date() > itemExpiry AND itemStatus = 'Available' AND itemType = 'Ingredient';";

            MySqlCommand comm = new MySqlCommand(notifexpiryquery, conn);
            comm.CommandText = notifexpiryquery;

            conn.Open();
            MySqlDataReader drdNotif = comm.ExecuteReader();

            while (drdNotif.Read())
            {
                string numExpired = drdNotif["num_expired"].ToString();

                if (numExpired != "")
                {
                    MessageBox.Show("You have " + numExpired + " expired item(s).", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
               
            }
            conn.Close();

        }


        public void restock()
        {
            String restockquery = "SELECT itemInvID, item_ID, name, itemQuantity, itemStatus, itemType, date_format(itemExpiry, '%m/%d/%y') as itemExpiry FROM items " +
                                  "INNER JOIN items_inventory ON items.itemsID = items_inventory.item_ID AND itemStatus like 'Unavailable' AND (itemExpiry > current_date() OR itemExpiry = '0000-00-00');";
            conn.Open();
            MySqlCommand comm = new MySqlCommand(restockquery, conn);
            comm.CommandText = restockquery;
            MySqlDataAdapter adp = new MySqlDataAdapter(comm);
            conn.Close();
            DataTable dt_log = new DataTable();
            adp.Fill(dt_log);

            restock_items.DataSource = dt_log;

            restock_items.Columns["item_ID"].Visible = false;
            restock_items.Columns["itemInvID"].Visible = false;
            restock_items.Columns["name"].HeaderText = "Name";
            restock_items.Columns["itemQuantity"].HeaderText = "Quantity";
            restock_items.Columns["itemStatus"].HeaderText = "Status";
            restock_items.Columns["itemType"].HeaderText = "Type";
            restock_items.Columns["itemExpiry"].HeaderText = "Expiration Date";

        }

        public void logDate()
        {
            DateTime logdate;
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT max(logdate) as LastLog FROM inventorylog;";
            MySqlDataReader rdr = cmd.ExecuteReader();
            while(rdr.Read())
            {
                logdate = DateTime.Parse(rdr["LastLog"].ToString());

                logdate_lbl.Text = logdate.ToString("MMM dd yyyy");
            }
            conn.Close();
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            prevForm.Show();
            this.Close();
        }

        private void btn_settings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.prevForm = this;
            settings.Show();
            this.Hide();
        }

        private void btn_profiling_Click(object sender, EventArgs e)
        {
            profiling_panel.Visible = true;
            profiling_panel.Enabled = true;
            restock_items.Visible = false;
            profiling_panel.Size = new Size(640, 529);
            profiling_panel.Location = new Point(139, 91);
        }

        private void btn_orders_Click(object sender, EventArgs e)
        {

            sales_panel.Visible = true;
            sales_panel.Enabled = true;

            sales_panel.Size = new Size(643, 498);
            sales_panel.Location = new Point(139, 91);
            
        }
        
        private void btn_product_Click(object sender, EventArgs e)
        {
            Product product = new Product();
            product.prevForm = this;
            product.Show();
            this.Hide();
        }

        private void report_btn_Click(object sender, EventArgs e)
        {
            Reports reports = new Reports();
            reports.prevForm = this;
            reports.Show();
            this.Hide();
        }

        private void btn_inventory_Click(object sender, EventArgs e)
        {
            InventoryTransaction inventory = new InventoryTransaction();
            inventory.prevForm = this;
            inventory.Show();
            this.Hide();
            
        }

        private void btn_staff_Click(object sender, EventArgs e)
        {
            Staff staff = new Staff();
            staff.prevForm = this;
            staff.Show();
            this.Hide();
        }

        private void btn_supplier_Click(object sender, EventArgs e)
        {
            Supplier supplier = new Supplier();
            supplier.prevForm = this;
            supplier.Show();
            this.Hide();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            profiling_panel.Visible = false;
            restock_items.Visible = true;
        }

        private void sales_btn_back_Click(object sender, EventArgs e)
        {
            sales_panel.Visible = false;
        }

        private void order_btn_Click(object sender, EventArgs e)
        {
            Order order = new Order();
            order.prevForm = this;
            order.Show();
            this.Hide();
        }

        private void restock_items_SelectionChanged(object sender, EventArgs e)
        {
            this.restock_items.ClearSelection();
        }
    }
}
