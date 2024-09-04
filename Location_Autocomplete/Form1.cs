using Nancy.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Location_Autocomplete
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cancellationTokenSource;

        public Form1()
        {
            InitializeComponent();
            comboBox1.TextChanged += ComboBox1_TextChanged;
        }

        private async void ComboBox1_TextChanged(object sender, EventArgs e)
        {
            // Cancel any ongoing task
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            if (string.IsNullOrEmpty(comboBox1.Text))
                return;

            try
            {
                var suggestions = await GetSuggestionsAsync(comboBox1.Text, _cancellationTokenSource.Token);
                if (suggestions != null)
                {
                    UpdateAutoComplete(suggestions);
                }
            }
            catch (TaskCanceledException)
            {
                // Task was canceled, ignore this exception.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching autocomplete suggestions: {ex.Message}");
            }
        }

        private async Task<AutoCompleteStringCollection> GetSuggestionsAsync(string input, CancellationToken cancellationToken)
        {
            string strUrl = $@"https://api.geoapify.com/v1/geocode/autocomplete?text={Uri.EscapeDataString(input)}&apiKey=55ce3f0485d441ae914ba6a9dbcddb8f";
            var client = new RestClient(strUrl);
            var request = new RestRequest();

            var response = await client.ExecuteGetAsync(request, cancellationToken);
            if (!response.IsSuccessful)
            {
                
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            dynamic jsonResponse = jss.Deserialize<dynamic>(response.Content);
            var stringCollection = new AutoCompleteStringCollection();

            if (jsonResponse != null && jsonResponse["features"] != null)
            {
                foreach (dynamic result in jsonResponse["features"])
                {
                    stringCollection.Add(result["properties"]["formatted"].ToString());
                }
            }

            return stringCollection;
        }

        private void UpdateAutoComplete(AutoCompleteStringCollection suggestions)
        {
            comboBox1.Items.Clear();
            comboBox1.AutoCompleteCustomSource = suggestions;
            for (int i = 0; i < suggestions.Count; i++)
            {
                comboBox1.Items.Add(suggestions[i]);
            }
            comboBox1.Select(comboBox1.Text.Length, 0); // Keeps the cursor at the end
        }
    }
}
