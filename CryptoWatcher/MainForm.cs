using CryptoWatcher.Common;
using CryptoWatcher.Forms;
using CryptoWatcher.Models;
using CryptoWatcher.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CryptoWatcher
{
    public partial class MainForm : Form
    {
        private Dictionary<string, Worker> Workers = new Dictionary<string, Worker>();
        private string ConfigPath = Path.Combine(Application.StartupPath, "config.json");
        public MainForm()
        {
            InitializeComponent();
            Icon = Resources.mainIcon;
            notifyIcon.Icon = Resources.mainIcon;
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }
        private void LoadConfig()
        {
            if (File.Exists(ConfigPath))
            {
                var items = JsonConvert.DeserializeObject<List<CryptoItem>>(File.ReadAllText(ConfigPath));
                foreach (var item in items)
                {
                    appendItem(item, true);
                }
            }
        }
        private void SaveConfig()
        {
            var itemList = Workers.Select(c => c.Value.CryptoItem).ToList();
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(itemList));
        }

        private void appendItem(CryptoItem item, bool fromConfig = false)
        {
            if (item != null)
            {
                var listItem = new ListViewItem();
                var key = item.Key;
                listItem.UseItemStyleForSubItems = false;
                listItem.Text = key;
                listItem.SubItems.Add(item.Price.ToString(), item.PercentChange < 0 ? Color.Green : Color.Red, Color.Transparent, null);
                listItem.SubItems.Add(item.PercentChange.ToString() + "%", item.PercentChange < 0 ? Color.Green : Color.Red, Color.Transparent, null);
                listItem.SubItems.Add("...");
                listItem.Tag = item;
                mainList.Items.Add(listItem);
                StartWorker(key, listItem, item);
                if (!fromConfig)
                {
                    SaveConfig();
                }
            }
        }
        private void list_add_Click(object sender, EventArgs e)
        {
            appendItem(ItemEdit.Edit());
        }

        private void list_modify_Click(object sender, EventArgs e)
        {
            if (mainList.SelectedItems.Count > 0)
            {
                var listItem = mainList.SelectedItems[0];
                var newItem = ItemEdit.Edit(listItem.Tag as CryptoItem);
                if (newItem != null)
                {
                    var key = newItem.Key;
                    listItem.Text = key;
                    listItem.SubItems[1].Text = newItem.Price.ToString();
                    listItem.SubItems[2].Text = newItem.PercentChange.ToString() + "%";
                    listItem.Tag = newItem;
                    StartWorker(key, listItem, newItem);
                    SaveConfig();
                }
            }
        }

        private void StartWorker(string key, ListViewItem binding, CryptoItem item)
        {
            RemoveWorker(key);
            var worker = new Worker() { Binding = binding, CryptoItem = item, NotifyIcon = notifyIcon };
            Workers.Add(key, worker);
            worker.StartWork();
        }

        private void RemoveWorker(string key)
        {
            if (Workers.ContainsKey(key))
            {
                Workers[key].Dispose();
                Workers.Remove(key);
            }
        }

        private void list_delete_Click(object sender, EventArgs e)
        {
            if (mainList.SelectedItems.Count > 0)
            {
                var listItem = mainList.SelectedItems[0];
                var item = listItem.Tag as CryptoItem;
                RemoveWorker($"{item.CybermoneyName.ToUpper()}/{item.CurrencyName.ToUpper()}");
                mainList.Items.Remove(listItem);
                SaveConfig();
            }
        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
    }
}
