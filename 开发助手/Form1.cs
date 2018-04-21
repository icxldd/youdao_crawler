using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 开发助手.API;
using 开发助手.YOUDAO;

namespace 开发助手
{

    /*
     * txtv1 注释
     * txtv2 翻译
     */
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void txtv1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                string v1 = string.Format("/****************************************************************************************************{0}*********************************************************************************************************/", txtv1.Text);
                mtxtv1.Text = v1;
                Clipboard.SetDataObject(v1, true);
            }
        }

        private void txtv2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {

                Core pV1 = new Core();

                string v1 = pV1._get(txtv2.Text);



                Clipboard.SetDataObject(v1, true);
            }
        }
    }
}
