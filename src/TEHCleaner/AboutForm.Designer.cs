namespace TEHCleaner;

partial class AboutForm
{
    private System.ComponentModel.IContainer? components = null;
    private PictureBox pictureLogo;
    private Label lblName;
    private Label lblVersion;
    private Label lblText;
    private LinkLabel linkSite;
    private Label lblAuthor;
    private Button btnClose;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pictureLogo = new PictureBox();
        lblName = new Label();
        lblVersion = new Label();
        lblText = new Label();
        linkSite = new LinkLabel();
        lblAuthor = new Label();
        btnClose = new Button();
        ((System.ComponentModel.ISupportInitialize)pictureLogo).BeginInit();
        SuspendLayout();
        // 
        // pictureLogo
        // 
        pictureLogo.Location = new Point(20, 20);
        pictureLogo.Name = "pictureLogo";
        pictureLogo.Size = new Size(72, 72);
        pictureLogo.SizeMode = PictureBoxSizeMode.Zoom;
        pictureLogo.TabIndex = 0;
        pictureLogo.TabStop = false;
        // 
        // lblName
        // 
        lblName.AutoSize = true;
        lblName.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
        lblName.Location = new Point(108, 18);
        lblName.Name = "lblName";
        lblName.Size = new Size(141, 32);
        lblName.TabIndex = 1;
        lblName.Text = "TEHCleaner";
        // 
        // lblVersion
        // 
        lblVersion.AutoSize = true;
        lblVersion.ForeColor = Color.DimGray;
        lblVersion.Location = new Point(111, 54);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new Size(74, 15);
        lblVersion.TabIndex = 2;
        lblVersion.Text = "Версия 2.1.2";
        // 
        // lblText
        // 
        lblText.Location = new Point(20, 112);
        lblText.Name = "lblText";
        lblText.Size = new Size(390, 61);
        lblText.TabIndex = 3;
        lblText.Text = "Безопасный очиститель Windows 10/11. Анализирует данные до удаления, не трогает пароли, cookies, закладки, автозагрузку и ассоциации файлов.";
        // 
        // linkSite
        // 
        linkSite.AutoSize = true;
        linkSite.Location = new Point(20, 190);
        linkSite.Name = "linkSite";
        linkSite.Size = new Size(98, 15);
        linkSite.TabIndex = 4;
        linkSite.TabStop = true;
        linkSite.Text = "https://tehadm.ru/";
        linkSite.LinkClicked += LinkSite_LinkClicked;
        // 
        // lblAuthor
        // 
        lblAuthor.AutoSize = true;
        lblAuthor.Location = new Point(20, 219);
        lblAuthor.Name = "lblAuthor";
        lblAuthor.Size = new Size(170, 15);
        lblAuthor.TabIndex = 5;
        lblAuthor.Text = "Разработчик: Alexandr Gedz";
        // 
        // btnClose
        // 
        btnClose.Location = new Point(326, 210);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(84, 29);
        btnClose.TabIndex = 6;
        btnClose.Text = "Закрыть";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Click += BtnClose_Click;
        // 
        // AboutForm
        // 
        AcceptButton = btnClose;
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(430, 258);
        Controls.Add(btnClose);
        Controls.Add(lblAuthor);
        Controls.Add(linkSite);
        Controls.Add(lblText);
        Controls.Add(lblVersion);
        Controls.Add(lblName);
        Controls.Add(pictureLogo);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "О TEHCleaner";
        ((System.ComponentModel.ISupportInitialize)pictureLogo).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
