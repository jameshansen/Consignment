using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Multi_Express_Consignment
{
    public partial class add_item_to_cpo : Form
    {

        private static consignment_purchase_order m_parent = null;

        public add_item_to_cpo(consignment_purchase_order calledBy)
        {
            InitializeComponent();
            m_parent = calledBy;
        }

        private void add_item_to_cpo_Shown(object sender, EventArgs e)
        {
            // Lock window size
            this.MaximumSize = new System.Drawing.Size(this.Width, this.Height);
            this.MinimumSize = new System.Drawing.Size(this.Width, this.Height);
            
            // Consignment Code on Title            
            this.Text = this.Text + " #" + m_parent.getconsignment_code();

            // Drop Down Share Type Default
            input_share_type.SelectedIndex = input_share_type.FindString(@"Value");
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_parent.addItem(input_description.Text, input_price_minimum.Text, input_price_suggested.Text, input_share.Text, input_share_type.Text, input_desc_brand.Text, input_desc_gender.Text, input_desc_garment.Text, input_desc_material.Text, input_desc_colour.Text);
        }

        private void add_item_to_cpo_Load(object sender, EventArgs e)
        {

        }


    }
}
