using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WEBCHAT
{
    public partial class _Default : Page
    {
        //initializing data base entity
        Database1Entities dt = new Database1Entities();
        AESEncryption AES = new AESEncryption();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
                LoadView();
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (txtMessage.Text.Length > 0)
            {
                Table msg = new Table();
                byte[] inputText = Encoding.ASCII.GetBytes(txtMessage.Text);
                DateTime startTime = DateTime.Now;
                msg.message = AES.EncryptText(txtMessage.Text, "");
                DateTime endTime = DateTime.Now;
                double diffTime = (endTime - startTime).TotalMilliseconds;
               
                msg.cdate = DateTime.Now;
                //adding message record to the table
                dt.Tables.Add(msg);
                //saving into database
                dt.SaveChanges();
                //clearing textbox
                txtMessage.Text = string.Empty;
            }

            //loading all chat messages
            LoadView();
        }

        private void LoadView()
        {
            //collection object for showing into list view
            List<Table> lstMessage = new List<Table>();
            //getting all chat messages from db
            var dd = dt.Tables.ToList();
            //loop for decrypt message 
            foreach (var item in dd)
            {
                Table tbl = new Table();
                byte[] inputText = Encoding.ASCII.GetBytes(item.message);
                DateTime startTime = DateTime.Now;
                tbl.message = AES.DecryptText(item.message, "");
                DateTime endTime = DateTime.Now;
                double diffTime = (endTime - startTime).TotalMilliseconds;
                tbl.cdate = item.cdate;
                lstMessage.Add(tbl);
            }

            //Datalist View - for displaying all messages
            dlMessages.DataSource = lstMessage;
            dlMessages.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //delete all chat messages from db
            foreach (var item in dt.Tables.ToList())
            {
                dt.Tables.Remove(item);
                dt.SaveChanges();
            }
            LoadView();
        }
    }
}
