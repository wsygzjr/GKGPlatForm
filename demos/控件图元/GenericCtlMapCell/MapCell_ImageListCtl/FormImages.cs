using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Griffins.Graph;

namespace Griffins.Map.CtlMapCell.Generic
{
    partial class FormImages : Form
    {
        public FormImages()
        {
            InitializeComponent();
            imageGroup = new GirffinsImageGroup();
            this.groupBoxKey.Enabled = false;
            this.ultraButtonRemove.Enabled = false;
            this.ultraButtonUpdate.Enabled = false;
        }

        private GirffinsImageGroup imageGroup;
        public GirffinsImageGroup ImageGroup
        {
            get { return imageGroup; }
            set 
            {
                if (value == null)
                {
                    imageGroup.Clear();
                }
                else
                {
                    imageGroup.Clear();
                    foreach (GirffinsImageGroupItem imageGroupItem in value)
                        imageGroup.Add(imageGroupItem);
                }
                fillImages();
            }
        }

        private void fillImages()
        {
            this.listView1.Items.Clear();
            this.imageList1.Images.Clear();
            int imageIndex = -1;
            foreach (GirffinsImageGroupItem imageGroupItem in imageGroup)
            {
                Image image = GirffinsImageGroupItem.LoadImageFromBytes(imageGroupItem.ImageData);
                this.imageList1.Images.Add(image);
                imageIndex++;

                ListViewItem item = new ListViewItem();
                item.Text = imageGroupItem.ImgKey;
                item.ImageIndex = imageIndex;
                item.Tag = imageGroupItem;

                this.listView1.Items.Add(item);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                this.groupBoxKey.Enabled = false;
                this.textBoxKey.Text = "";
                this.ultraButtonRemove.Enabled = false;
                this.ultraButtonUpdate.Enabled = false;
                return;
            }
            else
            {
                ListViewItem item = listView1.SelectedItems[0];
                this.groupBoxKey.Enabled = true;
                this.textBoxKey.Text = item.Text;
                this.ultraButtonRemove.Enabled = true;
                this.ultraButtonUpdate.Enabled = true;
            }
        }

        private void textBoxKey_TextChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            ListViewItem item = listView1.SelectedItems[0];
            item.Text = this.textBoxKey.Text.Trim();
            ((GirffinsImageGroupItem)(item.Tag)).ImgKey = item.Text;
        }

        private void ultraButtonRemove_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            ListViewItem item = listView1.SelectedItems[0];
            imageGroup.Remove((GirffinsImageGroupItem)(item.Tag));
            this.listView1.Items.Remove(item);
        }

        private void ultraButtonUpdate_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            ListViewItem item = listView1.SelectedItems[0];
            GirffinsImageGroupItem imageGroupItem = (GirffinsImageGroupItem)(item.Tag);
            string fileName = openFileDialog1.FileName;
            Image image = Image.FromFile(fileName);
            imageGroupItem.ImageData = GirffinsImageGroupItem.WriteImageToBytes(image);
            int imageIndex = item.ImageIndex;
            this.imageList1.Images.Add(image);
            Image image1 = this.imageList1.Images[this.imageList1.Images.Count - 1];
            this.imageList1.Images[imageIndex] = image1;
            this.imageList1.Images.RemoveAt(this.imageList1.Images.Count - 1);
        }

        private void ultraButtonAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string fileName = openFileDialog1.FileName;
            Image image = Image.FromFile(fileName);

            GirffinsImageGroupItem imageGroupItem = new GirffinsImageGroupItem();
            imageGroupItem.ImgKey = getNewImgKey();
            imageGroupItem.ImageData = GirffinsImageGroupItem.WriteImageToBytes(image);
            this.imageGroup.Add(imageGroupItem);

            this.imageList1.Images.Add(image);

            ListViewItem item = new ListViewItem();
            item.Text = imageGroupItem.ImgKey;
            item.ImageIndex = this.imageList1.Images.Count-1;
            item.Tag = imageGroupItem;
            this.listView1.Items.Add(item);
        }

        private string getNewImgKey()
        {
            int index = 1;
            while (true)
            {
                string imgName = "Key" + index.ToString();
                bool hasIt = false;
                foreach (GirffinsImageGroupItem imageGroupItem in imageGroup)
                {
                    if (string.Compare(imageGroupItem.ImgKey, imgName) == 0)
                    {
                        hasIt = true;
                        break;
                    }
                }
                if (!hasIt)
                    return imgName;
                index++;
            }
        }
    }
}