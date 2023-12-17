using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TaskRunway
{
    public partial class quickActions : Form
    {
        public event Action AddItemsToContextMenu;
        public event Action RemoveItemsFromContextMenu;

        public quickActions()
        {
            InitializeComponent();
            LoadQuickActionsFromFile(); // Load saved items when the form is created
        }

        // Method to populate listBox1 with items that are not in listBox2
        public void PopulateListBox1(ListBox.ObjectCollection items)
        {
            foreach (var item in items)
            {
                if (!listBox2.Items.Contains(item))
                {
                    listBox1.Items.Add(item);
                }
            }
        }

        public void SetAvailableActions(ListBox.ObjectCollection itemsFromForm1)
        {
            listBox1.Items.Clear();
            foreach (var item in itemsFromForm1)
            {
                if (!listBox2.Items.Contains(item))
                {
                    listBox1.Items.Add(item);
                }
            }
        }

        // Add items to listBox2 and remove them from listBox1
        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var selectedItem in listBox1.SelectedItems.Cast<object>().ToList())
            {
                listBox2.Items.Add(selectedItem);
                listBox1.Items.Remove(selectedItem);
            }

            // Raise the AddItemsToContextMenu event when items are added
            AddItemsToContextMenu?.Invoke();

            // Save items when added
            SaveQuickActionsToFile();
        }

        // Remove items from listBox2 and add them back to listBox1
        private void button6_Click(object sender, EventArgs e)
        {
            foreach (var selectedItem in listBox2.SelectedItems.Cast<object>().ToList())
            {
                listBox1.Items.Add(selectedItem);
                listBox2.Items.Remove(selectedItem);
            }

            // Raise the RemoveItemsFromContextMenu event when items are removed
            RemoveItemsFromContextMenu?.Invoke();

            // Save items when removed
            SaveQuickActionsToFile();
        }

        private void ConfigureQuickActions()
        {
            quickActions quickActionsForm = new quickActions();
            quickActionsForm.PopulateListBox1(listBox1.Items);
            if (quickActionsForm.ShowDialog() == DialogResult.OK)
            {
                // Optionally, update the context menu here
            }
        }

        // Method to save the items in listBox2 to a text file
        private void SaveQuickActionsToFile()
        {
            List<string> quickActions = listBox2.Items.Cast<string>().ToList();
            File.WriteAllLines("quickactions_config.txt", quickActions);
        }

        // Method to load saved quick actions from a text file
        private void LoadQuickActionsFromFile()
        {
            listBox2.Items.Clear();
            if (File.Exists("quickactions_config.txt"))
            {
                listBox2.Items.AddRange(File.ReadAllLines("quickactions_config.txt"));
            }
        }

        // Method to get the selected quick actions from listBox2
        public IEnumerable<string> GetSelectedQuickActions()
        {
            return listBox2.Items.Cast<string>();
        }

        private void quickActions_Load(object sender, EventArgs e)
        {
            // This method can be used for any initialization logic when the form loads.
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // This method can be used to handle the SelectedIndexChanged event of listBox2 if needed.
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            // Get the selected items from listBox2
            List<object> selectedItems = listBox2.SelectedItems.Cast<object>().ToList();

            // Add the selected items back to listBox1
            listBox1.Items.AddRange(selectedItems.ToArray());

            // Remove the selected items from listBox2
            foreach (object selectedItem in selectedItems)
            {
                listBox2.Items.Remove(selectedItem);
            }

            // Raise the RemoveItemsFromContextMenu event when items are removed
            RemoveItemsFromContextMenu?.Invoke();

            // Save items when removed
            SaveQuickActionsToFile();
        }
    }
}
