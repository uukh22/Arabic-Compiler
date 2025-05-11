namespace FinalProjectOfCompiler_Arabic_
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtInput = new TextBox();
            btnAnalyze = new Button();
            lstTokens = new ListBox();
            lstParsers = new ListBox();
            lblInput = new Label();
            lblTokens = new Label();
            lblParsers = new Label();
            chkToggleMode = new CheckBox();
            SuspendLayout();
            // 
            // txtInput
            // 
            txtInput.Location = new Point(12, 35);
            txtInput.Multiline = true;
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(1104, 150);
            txtInput.TabIndex = 0;
            // 
            // btnAnalyze
            // 
            btnAnalyze.Location = new Point(12, 200);
            btnAnalyze.Name = "btnAnalyze";
            btnAnalyze.Size = new Size(150, 30);
            btnAnalyze.TabIndex = 1;
            btnAnalyze.Text = "Analyze Code";
            btnAnalyze.Click += btnAnalyze_Click;
            // 
            // lstTokens
            // 
            lstTokens.Location = new Point(12, 280);
            lstTokens.Name = "lstTokens";
            lstTokens.Size = new Size(499, 144);
            lstTokens.TabIndex = 2;
            // 
            // lstParsers
            // 
            lstParsers.Location = new Point(530, 280);
            lstParsers.Name = "lstParsers";
            lstParsers.Size = new Size(1045, 144);
            lstParsers.TabIndex = 3;
            // 
            // lblInput
            // 
            lblInput.Location = new Point(12, 9);
            lblInput.Name = "lblInput";
            lblInput.Size = new Size(100, 23);
            lblInput.TabIndex = 0;
            lblInput.Text = "Input Code:";
            // 
            // lblTokens
            // 
            lblTokens.Location = new Point(12, 250);
            lblTokens.Name = "lblTokens";
            lblTokens.Size = new Size(100, 23);
            lblTokens.TabIndex = 2;
            lblTokens.Text = "Tokens:";
            // 
            // lblParsers
            // 
            lblParsers.Location = new Point(530, 250);
            lblParsers.Name = "lblParsers";
            lblParsers.Size = new Size(100, 23);
            lblParsers.TabIndex = 3;
            lblParsers.Text = "Parsers:";
            // 
            // chkToggleMode
            // 
            chkToggleMode.Location = new Point(1200, 35);
            chkToggleMode.Name = "chkToggleMode";
            chkToggleMode.Size = new Size(150, 30);
            chkToggleMode.TabIndex = 4;
            chkToggleMode.Text = "Dark Mode";
            chkToggleMode.CheckedChanged += chkToggleMode_CheckedChanged;
            // 
            // Form1
            // 
            ClientSize = new Size(1658, 450);
            Controls.Add(lblInput);
            Controls.Add(txtInput);
            Controls.Add(btnAnalyze);
            Controls.Add(lblTokens);
            Controls.Add(lstTokens);
            Controls.Add(lblParsers);
            Controls.Add(lstParsers);
            Controls.Add(chkToggleMode);
            Name = "Form1";
            Text = "Arabic Compiler";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.ListBox lstTokens;
        private System.Windows.Forms.ListBox lstParsers;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.Label lblTokens;
        private System.Windows.Forms.Label lblParsers;
        private System.Windows.Forms.CheckBox chkToggleMode;

        private void chkToggleMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkToggleMode.Checked)
            {
                // Apply Dark Mode
                this.BackColor = Color.Black;
                this.ForeColor = Color.White;
                txtInput.BackColor = Color.Gray;
                txtInput.ForeColor = Color.White;
                lstTokens.BackColor = Color.Gray;
                lstTokens.ForeColor = Color.White;
                lstParsers.BackColor = Color.Gray;
                lstParsers.ForeColor = Color.White;
                btnAnalyze.BackColor = Color.DimGray;
                btnAnalyze.ForeColor = Color.White;
            }
            else
            {
                // Apply Light Mode
                this.BackColor = Color.White;
                this.ForeColor = Color.Black;
                txtInput.BackColor = Color.White;
                txtInput.ForeColor = Color.Black;
                lstTokens.BackColor = Color.White;
                lstTokens.ForeColor = Color.Black;
                lstParsers.BackColor = Color.White;
                lstParsers.ForeColor = Color.Black;
                btnAnalyze.BackColor = Color.LightGray;
                btnAnalyze.ForeColor = Color.Black;
            }
        }
    }
}
