using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Linq;

namespace ConsumeFix
{
    public partial class Form1 : Form
    {
       
        //keep track of all fund/symbol highest/lowest prices
        internal Dictionary<string, float> allFundsBuy = new Dictionary<string, float>();
        internal Dictionary<string, float> allFundsSell = new Dictionary<string , float>();
        //all kv pairs for current FIX message
        Dictionary<int, string> dict = new Dictionary<int, string>();
        int currentKey = 0;
        Boolean isOrder = false;
        Fund currentFund = new Fund();
        Orders currentOrders = new Orders();

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    string[] lines = File.ReadAllLines(file);
                    currentKey = 0;
                    foreach (string line in lines)
                    {
                        dict.Clear();
                        currentFund = new Fund();
                        currentOrders = new Orders();
                        var items = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Split(new[] { '=' }));
                        foreach (var item in items)
                        {
                            Console.Write("Item 0 key is: " + item[0] + "Item 1 value is: " + item[1]);
                            Int32.TryParse(item[0], out currentKey);
                            if (item[1].Equals("D"))
                            {
                                isOrder = true;
                            }
                            //44 is price
                            if(isOrder && currentKey == 44)
                            {
                              currentOrders.price = float.Parse(item[1]); 
                            }
                            //1 is account
                            else if(isOrder && currentKey == 1)
                            {
                               currentFund.accountName = item[1];
                            }
                            //55 is symobl
                            else if(isOrder && currentKey == 55)
                            {
                               currentOrders.symbol = item[1]; 
                            }
                            //54 is side
                            else if(isOrder && currentKey == 54)
                            {
                               currentOrders.side = item[1];
                            }
                            dict.Add(currentKey, item[1]);
                        }

                        //1 is buy
                        if(isOrder && currentOrders.side.Equals("1") && allFundsBuy.ContainsKey(currentFund.accountName + ":" + currentOrders.symbol))
                        {
                            if (allFundsBuy[currentFund.accountName + ":" + currentOrders.symbol] < currentOrders.price)
                            {
                                allFundsBuy[currentFund.accountName + ":" + currentOrders.symbol] = currentOrders.price;
                            }
                        }
                        //2 is sell
                        else if(isOrder && currentOrders.side.Equals("2") & allFundsSell.ContainsKey(currentFund.accountName+ ":" + currentOrders.symbol))
                        {
                            if (allFundsSell[currentFund.accountName + ":" + currentOrders.symbol] > currentOrders.price)
                            {
                                allFundsSell[currentFund.accountName + ":" + currentOrders.symbol] = currentOrders.price;
                            }
                        }
                        //1 is buy
                        else if (isOrder && currentOrders.side.Equals("1") && !allFundsBuy.ContainsKey(currentFund.accountName + ":" + currentOrders.symbol))
                        {
                            allFundsBuy[currentFund.accountName + ":" + currentOrders.symbol] = currentOrders.price;

                        }
                        //2 is sell
                        else if (isOrder && currentOrders.side.Equals("2") & !allFundsSell.ContainsKey(currentFund.accountName + ":" + currentOrders.symbol))
                        {
                            allFundsSell[currentFund.accountName + ":" + currentOrders.symbol] = currentOrders.price;
                        }
                        
                        dict.Clear();
                        isOrder = false;
                        currentFund = new Fund();
                        currentOrders = new Orders();
                    }
                    
                }
                catch (IOException)
                {
                }
                catch(System.ArgumentException)
                {
                    if(allFundsSell.ContainsKey(currentFund.accountName + ":" + currentOrders.symbol))
                    {
                        allFundsSell.Remove(currentFund.accountName + ":" + currentOrders.symbol);
                    }
                    else if(allFundsBuy.ContainsKey(currentFund.accountName + ":" + currentOrders.symbol))
                    {
                        allFundsBuy.Remove(currentFund.accountName + ":" + currentOrders.symbol);
                    }
                        
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in dict)
                    {
                        sb.AppendFormat("{0} - {1}{2}", item.Key, item.Value, Environment.NewLine);
                    }

                    MessageBox.Show("Duplicate Key Found or bad fix message: " + sb.ToString().TrimEnd() + " Duplicate/bad key is: " + currentKey.ToString());
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(var kvp in allFundsBuy)
            {
                Console.WriteLine("Account & Symobl = {0}, Price = {1}", kvp.Key, kvp.Value);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var kvp in allFundsSell)
            {
                Console.WriteLine("Account & Symobl = {0}, Price = {1}", kvp.Key, kvp.Value);
            }
        }
    }
}
