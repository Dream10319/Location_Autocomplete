using Nancy.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Location_Autocomplete
{
    public partial class Form1 : Form
    {   
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Thread th = new Thread(new ThreadStart(() =>
            {
                this.Invoke(new Action(() =>
                {
                    List<string> data = new List<string>();
                    if (!string.IsNullOrEmpty(textBox1.Text))
                    {
                        string strUrl = string.Format($@"https://api.geoapify.com/v1/geocode/autocomplete?text={textBox1.Text}&apiKey=55ce3f0485d441ae914ba6a9dbcddb8f");
                        RestClient client = new RestClient(strUrl);
                        RestRequest request = new RestRequest();
                        string strReturn = client.ExecuteGet(request).Content;
                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        dynamic response = jss.Deserialize<dynamic>(strReturn);
                        if (response != null)
                        {
                            if (response["features"] != null)
                            {
                                dynamic results = response["features"];
                                foreach (dynamic result in results)
                                {
                                    data.Add(result["properties"]["formatted"].ToString());
                                }
                                textBox1.Values = data.ToArray();
                            }
                        }
                    }
                }));
            }));
            th.Start();
        }
    }
}
