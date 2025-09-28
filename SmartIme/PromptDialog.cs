namespace SmartIme
{
    public partial class PromptDialog : Form
    {
        // 公共属性，用于获取用户输入的值
        public string ResultText => txtInput.Text;
        public PromptDialog(string inputText, string title = "修改应用程序显示名称", string promptText = "请输入应用程序显示名称:")
        {

            InitializeComponent();

            Text = title;
            lblPrompt.Text = promptText;
            txtInput.Text = inputText;

            Size = new Size(400, 170);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            txtInput.Text = inputText;

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            Load += (s, ev) =>
            {
                txtInput.SelectionStart = txtInput.Text.IndexOf("-") + 2;
                txtInput.SelectionLength = txtInput.Text.Length;
            };
        }

        // 控件声明
        private Label lblPrompt;
        private TextBox txtInput;
        private Button btnOK;
        private Button btnCancel;
        private void InitializeComponent()
        {
            lblPrompt = new Label
            {
                Text = "请输入应用程序显示名称:",
                Left = 20,
                Top = 15,
                Width = 360,
            };

            txtInput = new TextBox
            {
                Left = 20,
                Top = 45,
                Width = 340,

            };

            btnOK = new Button
            {
                Text = "确定",
                DialogResult = DialogResult.OK,
                Left = 190,
                Top = 88,
                Width = 80,
                Height = 30

            };

            btnCancel = new Button
            {
                Text = "取消",
                DialogResult = DialogResult.Cancel,
                Left = 285,
                Top = 88,
                Width = 80,
                Height = 30,
            };

            Controls.AddRange([lblPrompt, txtInput, btnOK, btnCancel]);
            //Padding = new Padding(10);
        }
    }

}
//public partial class PromptDialog : Form
//{
//    // 公共属性，用于获取用户输入的值
//    public string ResultText => txtInput.Text;

//    // 构造函数
//    public PromptDialog(string inputText, string title = "修改应用程序显示名称", string promptText = "请输入应用程序显示名称:")
//    {
//        InitializeComponent();
//        this.Text = title;
//        lblPrompt.Text = promptText;
//        txtInput.Text = inputText;

//        // 设置对话框属性
//        this.FormBorderStyle = FormBorderStyle.FixedDialog;
//        this.MaximizeBox = false;
//        this.MinimizeBox = false;
//        this.StartPosition = FormStartPosition.CenterParent;
//        this.AcceptButton = btnOK;
//        this.CancelButton = btnCancel;
//    }

//    private void InitializeComponent()
//    {
//        // 创建控件
//        lblPrompt = new Label();
//        txtInput = new TextBox();
//        btnOK = new Button();
//        btnCancel = new Button();

//        // 设置控件属性
//        lblPrompt.Location = new Point(12, 15);
//        lblPrompt.AutoSize = true;
//        lblPrompt.Text = "请输入:";

//        txtInput.Location = new Point(12, 40);
//        txtInput.Size = new Size(260, 20);
//        txtInput.TabIndex = 0;

//        btnOK.Location = new Point(116, 75);
//        btnOK.Size = new Size(75, 23);
//        btnOK.TabIndex = 1;
//        btnOK.Text = "确定";
//        btnOK.DialogResult = DialogResult.OK;

//        btnCancel.Location = new Point(197, 75);
//        btnCancel.Size = new Size(75, 23);
//        btnCancel.TabIndex = 2;
//        btnCancel.Text = "取消";
//        btnCancel.DialogResult = DialogResult.Cancel;

//        // 设置窗体属性
//        this.ClientSize = new Size(284, 111);
//        this.Controls.AddRange(new Control[] { lblPrompt, txtInput, btnOK, btnCancel });
//        this.Padding = new Padding(10);
//    }

//    // 控件声明
//    private Label lblPrompt;
//    private TextBox txtInput;
//    private Button btnOK;
//    private Button btnCancel;
//}


