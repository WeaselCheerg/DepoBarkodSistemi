using System;
using System.Windows.Forms;
using System.ComponentModel; // WFO1000 hatasını çözmek için gerekli kütüphane eklendi

namespace WinFormsApp34
{
    public class UrunEkleForm : Form
    {
        // Form1'in aradığı isme göre uyarlandı ve WFO1000 hatasını gizlemek için etiketlendi
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OlusturulanQRYolu { get; set; }

        public UrunEkleForm()
        {
            this.Text = "Yeni Ürün Ekle";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
        }
    }
}