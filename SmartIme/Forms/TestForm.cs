namespace SmartIme.Forms
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // TSFWapper.GetCurrentLang(out string[] langs);
            // listBox1.Items.AddRange(langs);
            short[] langsIDs = TSFWapper.GetLangIDs();
            // listBox1.Items.AddRange(langsIDs.Select(i => i.ToString()).ToArray());
            // foreach (short lan in langsIDs)
            // {
            //     string[] imeList = TSFWapper.GetInputMethodList(lan);
            //     listBox1.Items.AddRange(imeList);
            // }
            MessageBox.Show(short.Parse("0409", System.Globalization.NumberStyles.HexNumber).ToString());
            var arr = TSFWapper.GetInputMethodList(Convert.ToInt16("0x0409", 16));
            listBox1.Items.AddRange(arr);
        }
    }
}
