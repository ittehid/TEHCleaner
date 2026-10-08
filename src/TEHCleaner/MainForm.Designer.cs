namespace TEHCleaner;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private MenuStrip menuStrip;
    private ToolStripMenuItem fileMenu;
    private ToolStripMenuItem saveReportMenu;
    private ToolStripMenuItem openLogsMenu;
    private ToolStripMenuItem openCurrentLogMenu;
    private ToolStripMenuItem openRegistryBackupsMenu;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripMenuItem exitMenu;
    private ToolStripMenuItem helpMenu;
    private ToolStripMenuItem homeMenu;
    private ToolStripMenuItem aboutMenu;
    private Panel headerPanel;
    private Label lblSubtitle;
    private Label lblVersion;
    private Label lblAdmin;
    private Button btnAdmin;
    private Button btnRecommended;
    private Button btnClear;
    private CheckBox chkShowAdvanced;
    private CheckBox chkRestorePoint;
    private DataGridView gridTasks;
    private DataGridViewCheckBoxColumn colSelect;
    private DataGridViewTextBoxColumn colCategory;
    private DataGridViewTextBoxColumn colName;
    private DataGridViewTextBoxColumn colFoundItems;
    private DataGridViewTextBoxColumn colFoundSize;
    private DataGridViewTextBoxColumn colCleanedItems;
    private DataGridViewTextBoxColumn colCleanedSize;
    private DataGridViewTextBoxColumn colStatus;
    private Label lblTaskInfo;
    private Label lblDetailsHeader;
    private DataGridView gridDetails;
    private DataGridViewTextBoxColumn colDetailArea;
    private DataGridViewTextBoxColumn colDetailPath;
    private DataGridViewTextBoxColumn colDetailItems;
    private DataGridViewTextBoxColumn colDetailSize;
    private DataGridViewTextBoxColumn colDetailSkipped;
    private Panel footerPanel;
    private Button btnAnalyze;
    private Button btnClean;
    private Button btnCancel;
    private ProgressBar progressBar;
    private Label lblStatus;
    private Label lblSelected;
    private Label lblFound;

    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
        DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
        menuStrip = new MenuStrip();
        fileMenu = new ToolStripMenuItem();
        saveReportMenu = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        openLogsMenu = new ToolStripMenuItem();
        openCurrentLogMenu = new ToolStripMenuItem();
        openRegistryBackupsMenu = new ToolStripMenuItem();
        toolStripSeparator2 = new ToolStripSeparator();
        exitMenu = new ToolStripMenuItem();
        helpMenu = new ToolStripMenuItem();
        homeMenu = new ToolStripMenuItem();
        aboutMenu = new ToolStripMenuItem();
        headerPanel = new Panel();
        btnAdmin = new Button();
        lblAdmin = new Label();
        lblVersion = new Label();
        lblSubtitle = new Label();
        btnRecommended = new Button();
        btnClear = new Button();
        chkShowAdvanced = new CheckBox();
        chkRestorePoint = new CheckBox();
        gridTasks = new DataGridView();
        colSelect = new DataGridViewCheckBoxColumn();
        colCategory = new DataGridViewTextBoxColumn();
        colName = new DataGridViewTextBoxColumn();
        colFoundItems = new DataGridViewTextBoxColumn();
        colFoundSize = new DataGridViewTextBoxColumn();
        colCleanedItems = new DataGridViewTextBoxColumn();
        colCleanedSize = new DataGridViewTextBoxColumn();
        colStatus = new DataGridViewTextBoxColumn();
        lblTaskInfo = new Label();
        lblDetailsHeader = new Label();
        gridDetails = new DataGridView();
        colDetailArea = new DataGridViewTextBoxColumn();
        colDetailPath = new DataGridViewTextBoxColumn();
        colDetailItems = new DataGridViewTextBoxColumn();
        colDetailSize = new DataGridViewTextBoxColumn();
        colDetailSkipped = new DataGridViewTextBoxColumn();
        footerPanel = new Panel();
        lblFound = new Label();
        lblSelected = new Label();
        lblStatus = new Label();
        progressBar = new ProgressBar();
        btnCancel = new Button();
        btnClean = new Button();
        btnAnalyze = new Button();
        menuStrip.SuspendLayout();
        headerPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridTasks).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridDetails).BeginInit();
        footerPanel.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip
        // 
        menuStrip.ImageScalingSize = new Size(28, 28);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, helpMenu });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Padding = new Padding(18, 7, 0, 7);
        menuStrip.Size = new Size(1435, 48);
        menuStrip.TabIndex = 0;
        // 
        // fileMenu
        // 
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { saveReportMenu, toolStripSeparator1, openLogsMenu, openCurrentLogMenu, openRegistryBackupsMenu, toolStripSeparator2, exitMenu });
        fileMenu.Name = "fileMenu";
        fileMenu.Size = new Size(80, 34);
        fileMenu.Text = "Файл";
        // 
        // saveReportMenu
        // 
        saveReportMenu.Enabled = false;
        saveReportMenu.Name = "saveReportMenu";
        saveReportMenu.Size = new Size(419, 40);
        saveReportMenu.Text = "Сохранить отчёт об очистке…";
        saveReportMenu.Click += SaveReportMenu_Click;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(416, 6);
        // 
        // openLogsMenu
        // 
        openLogsMenu.Name = "openLogsMenu";
        openLogsMenu.Size = new Size(419, 40);
        openLogsMenu.Text = "Открыть папку логов";
        openLogsMenu.Click += OpenLogsMenu_Click;
        // 
        // openCurrentLogMenu
        // 
        openCurrentLogMenu.Name = "openCurrentLogMenu";
        openCurrentLogMenu.Size = new Size(419, 40);
        openCurrentLogMenu.Text = "Открыть текущий лог";
        openCurrentLogMenu.Click += OpenCurrentLogMenu_Click;
        // 
        // openRegistryBackupsMenu
        // 
        openRegistryBackupsMenu.Name = "openRegistryBackupsMenu";
        openRegistryBackupsMenu.Size = new Size(419, 40);
        openRegistryBackupsMenu.Text = "Резервные копии реестра";
        openRegistryBackupsMenu.Click += OpenRegistryBackupsMenu_Click;
        // 
        // toolStripSeparator2
        // 
        toolStripSeparator2.Name = "toolStripSeparator2";
        toolStripSeparator2.Size = new Size(416, 6);
        // 
        // exitMenu
        // 
        exitMenu.Name = "exitMenu";
        exitMenu.Size = new Size(419, 40);
        exitMenu.Text = "Выход";
        exitMenu.Click += ExitMenu_Click;
        // 
        // helpMenu
        // 
        helpMenu.DropDownItems.AddRange(new ToolStripItem[] { homeMenu, aboutMenu });
        helpMenu.Name = "helpMenu";
        helpMenu.Size = new Size(113, 34);
        helpMenu.Text = "Помощь";
        // 
        // homeMenu
        // 
        homeMenu.Name = "homeMenu";
        homeMenu.Size = new Size(261, 40);
        homeMenu.Text = "TEHADM.ru";
        homeMenu.Click += HomeMenu_Click;
        // 
        // aboutMenu
        // 
        aboutMenu.Name = "aboutMenu";
        aboutMenu.Size = new Size(261, 40);
        aboutMenu.Text = "О программе";
        aboutMenu.Click += AboutMenu_Click;
        // 
        // headerPanel
        // 
        headerPanel.BackColor = Color.FromArgb(28, 37, 54);
        headerPanel.Controls.Add(btnAdmin);
        headerPanel.Controls.Add(lblAdmin);
        headerPanel.Controls.Add(lblVersion);
        headerPanel.Controls.Add(lblSubtitle);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 48);
        headerPanel.Margin = new Padding(5, 5, 5, 5);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(1435, 102);
        headerPanel.TabIndex = 1;
        // 
        // btnAdmin
        // 
        btnAdmin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAdmin.FlatStyle = FlatStyle.Flat;
        btnAdmin.ForeColor = Color.White;
        btnAdmin.Location = new Point(1092, 46);
        btnAdmin.Margin = new Padding(5, 5, 5, 5);
        btnAdmin.Name = "btnAdmin";
        btnAdmin.Size = new Size(329, 47);
        btnAdmin.TabIndex = 4;
        btnAdmin.Text = "Запустить администратором";
        btnAdmin.UseVisualStyleBackColor = true;
        btnAdmin.Click += BtnAdmin_Click;
        // 
        // lblAdmin
        // 
        lblAdmin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblAdmin.ForeColor = Color.FromArgb(190, 201, 218);
        lblAdmin.Location = new Point(749, 63);
        lblAdmin.Margin = new Padding(5, 0, 5, 0);
        lblAdmin.Name = "lblAdmin";
        lblAdmin.Size = new Size(323, 30);
        lblAdmin.TabIndex = 3;
        lblAdmin.Text = "Обычный режим";
        lblAdmin.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblVersion
        // 
        lblVersion.AutoSize = true;
        lblVersion.BackColor = Color.FromArgb(48, 63, 86);
        lblVersion.ForeColor = Color.White;
        lblVersion.Location = new Point(653, 55);
        lblVersion.Margin = new Padding(5, 0, 5, 0);
        lblVersion.Name = "lblVersion";
        lblVersion.Padding = new Padding(10, 4, 10, 4);
        lblVersion.Size = new Size(86, 38);
        lblVersion.TabIndex = 2;
        lblVersion.Text = "v2.1.2";
        // 
        // lblSubtitle
        // 
        lblSubtitle.AutoSize = true;
        lblSubtitle.ForeColor = Color.FromArgb(190, 201, 218);
        lblSubtitle.Location = new Point(28, 63);
        lblSubtitle.Margin = new Padding(5, 0, 5, 0);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(615, 30);
        lblSubtitle.TabIndex = 1;
        lblSubtitle.Text = "Безопасная очистка Windows 10/11 • анализ перед удалением";
        // 
        // btnRecommended
        // 
        btnRecommended.Location = new Point(24, 159);
        btnRecommended.Margin = new Padding(5, 5, 5, 5);
        btnRecommended.Name = "btnRecommended";
        btnRecommended.Size = new Size(275, 47);
        btnRecommended.TabIndex = 2;
        btnRecommended.Text = "Рекомендуемый набор";
        btnRecommended.UseVisualStyleBackColor = true;
        btnRecommended.Click += BtnRecommended_Click;
        // 
        // btnClear
        // 
        btnClear.Location = new Point(310, 159);
        btnClear.Margin = new Padding(5, 5, 5, 5);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(161, 47);
        btnClear.TabIndex = 3;
        btnClear.Text = "Снять выбор";
        btnClear.UseVisualStyleBackColor = true;
        btnClear.Click += BtnClear_Click;
        // 
        // chkShowAdvanced
        // 
        chkShowAdvanced.AutoSize = true;
        chkShowAdvanced.Location = new Point(499, 168);
        chkShowAdvanced.Margin = new Padding(5, 5, 5, 5);
        chkShowAdvanced.Name = "chkShowAdvanced";
        chkShowAdvanced.Size = new Size(275, 34);
        chkShowAdvanced.TabIndex = 4;
        chkShowAdvanced.Text = "Расширенные операции";
        chkShowAdvanced.UseVisualStyleBackColor = true;
        chkShowAdvanced.CheckedChanged += ChkShowAdvanced_CheckedChanged;
        // 
        // chkRestorePoint
        // 
        chkRestorePoint.AutoSize = true;
        chkRestorePoint.Location = new Point(816, 168);
        chkRestorePoint.Margin = new Padding(5, 5, 5, 5);
        chkRestorePoint.Name = "chkRestorePoint";
        chkRestorePoint.Size = new Size(414, 34);
        chkRestorePoint.TabIndex = 5;
        chkRestorePoint.Text = "Точка восстановления перед очисткой";
        chkRestorePoint.UseVisualStyleBackColor = true;
        // 
        // gridTasks
        // 
        gridTasks.AllowUserToAddRows = false;
        gridTasks.AllowUserToDeleteRows = false;
        gridTasks.AllowUserToResizeRows = false;
        gridTasks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        gridTasks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        gridTasks.BackgroundColor = Color.White;
        dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle1.BackColor = SystemColors.Control;
        dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
        gridTasks.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
        gridTasks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        gridTasks.Columns.AddRange(new DataGridViewColumn[] { colSelect, colCategory, colName, colFoundItems, colFoundSize, colCleanedItems, colCleanedSize, colStatus });
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle2.BackColor = SystemColors.Window;
        dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle2.Padding = new Padding(2, 1, 2, 1);
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
        gridTasks.DefaultCellStyle = dataGridViewCellStyle2;
        gridTasks.Location = new Point(24, 222);
        gridTasks.Margin = new Padding(5, 5, 5, 5);
        gridTasks.MultiSelect = false;
        gridTasks.Name = "gridTasks";
        gridTasks.RowHeadersVisible = false;
        gridTasks.RowHeadersWidth = 72;
        gridTasks.RowTemplate.Height = 27;
        gridTasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridTasks.Size = new Size(1386, 455);
        gridTasks.TabIndex = 6;
        gridTasks.CellValueChanged += GridTasks_CellValueChanged;
        gridTasks.CurrentCellDirtyStateChanged += GridTasks_CurrentCellDirtyStateChanged;
        gridTasks.SelectionChanged += GridTasks_SelectionChanged;
        // 
        // colSelect
        // 
        colSelect.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        colSelect.HeaderText = "✓";
        colSelect.MinimumWidth = 34;
        colSelect.Name = "colSelect";
        colSelect.Width = 39;
        // 
        // colCategory
        // 
        colCategory.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colCategory.HeaderText = "Категория";
        colCategory.MinimumWidth = 95;
        colCategory.Name = "colCategory";
        colCategory.ReadOnly = true;
        colCategory.Width = 156;
        // 
        // colName
        // 
        colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        colName.HeaderText = "Очистка";
        colName.MinimumWidth = 180;
        colName.Name = "colName";
        colName.ReadOnly = true;
        // 
        // colFoundItems
        // 
        colFoundItems.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colFoundItems.HeaderText = "Объекты";
        colFoundItems.MinimumWidth = 82;
        colFoundItems.Name = "colFoundItems";
        colFoundItems.ReadOnly = true;
        colFoundItems.Width = 116;
        // 
        // colFoundSize
        // 
        colFoundSize.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colFoundSize.HeaderText = "Объём";
        colFoundSize.MinimumWidth = 86;
        colFoundSize.Name = "colFoundSize";
        colFoundSize.ReadOnly = true;
        colFoundSize.Width = 108;
        // 
        // colCleanedItems
        // 
        colCleanedItems.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colCleanedItems.HeaderText = "Удалено";
        colCleanedItems.MinimumWidth = 82;
        colCleanedItems.Name = "colCleanedItems";
        colCleanedItems.ReadOnly = true;
        colCleanedItems.Width = 116;
        // 
        // colCleanedSize
        // 
        colCleanedSize.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colCleanedSize.HeaderText = "Освобождено";
        colCleanedSize.MinimumWidth = 105;
        colCleanedSize.Name = "colCleanedSize";
        colCleanedSize.ReadOnly = true;
        colCleanedSize.Width = 170;
        // 
        // colStatus
        // 
        colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colStatus.HeaderText = "Статус";
        colStatus.MinimumWidth = 105;
        colStatus.Name = "colStatus";
        colStatus.ReadOnly = true;
        colStatus.Width = 120;
        // 
        // lblTaskInfo
        // 
        lblTaskInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblTaskInfo.AutoEllipsis = true;
        lblTaskInfo.ForeColor = Color.DimGray;
        lblTaskInfo.Location = new Point(26, 688);
        lblTaskInfo.Margin = new Padding(5, 0, 5, 0);
        lblTaskInfo.Name = "lblTaskInfo";
        lblTaskInfo.Size = new Size(1382, 32);
        lblTaskInfo.TabIndex = 7;
        lblTaskInfo.Text = "Выберите строку, чтобы увидеть описание и подробности результата.";
        // 
        // lblDetailsHeader
        // 
        lblDetailsHeader.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblDetailsHeader.AutoSize = true;
        lblDetailsHeader.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblDetailsHeader.Location = new Point(24, 726);
        lblDetailsHeader.Margin = new Padding(5, 0, 5, 0);
        lblDetailsHeader.Name = "lblDetailsHeader";
        lblDetailsHeader.Size = new Size(148, 30);
        lblDetailsHeader.TabIndex = 8;
        lblDetailsHeader.Text = "Что очищено";
        // 
        // gridDetails
        // 
        gridDetails.AllowUserToAddRows = false;
        gridDetails.AllowUserToDeleteRows = false;
        gridDetails.AllowUserToResizeRows = false;
        gridDetails.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        gridDetails.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        gridDetails.BackgroundColor = Color.FromArgb(248, 249, 251);
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle3.BackColor = SystemColors.Control;
        dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
        gridDetails.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
        gridDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        gridDetails.Columns.AddRange(new DataGridViewColumn[] { colDetailArea, colDetailPath, colDetailItems, colDetailSize, colDetailSkipped });
        dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle4.BackColor = SystemColors.Window;
        dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
        dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle4.Padding = new Padding(2, 1, 2, 1);
        dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
        gridDetails.DefaultCellStyle = dataGridViewCellStyle4;
        gridDetails.Location = new Point(24, 761);
        gridDetails.Margin = new Padding(5, 5, 5, 5);
        gridDetails.MultiSelect = false;
        gridDetails.Name = "gridDetails";
        gridDetails.ReadOnly = true;
        gridDetails.RowHeadersVisible = false;
        gridDetails.RowHeadersWidth = 72;
        gridDetails.RowTemplate.Height = 24;
        gridDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridDetails.Size = new Size(1386, 117);
        gridDetails.TabIndex = 9;
        // 
        // colDetailArea
        // 
        colDetailArea.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        colDetailArea.FillWeight = 35F;
        colDetailArea.HeaderText = "Область";
        colDetailArea.MinimumWidth = 130;
        colDetailArea.Name = "colDetailArea";
        colDetailArea.ReadOnly = true;
        // 
        // colDetailPath
        // 
        colDetailPath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        colDetailPath.FillWeight = 65F;
        colDetailPath.HeaderText = "Путь / действие";
        colDetailPath.MinimumWidth = 180;
        colDetailPath.Name = "colDetailPath";
        colDetailPath.ReadOnly = true;
        // 
        // colDetailItems
        // 
        colDetailItems.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colDetailItems.HeaderText = "Удалено";
        colDetailItems.MinimumWidth = 75;
        colDetailItems.Name = "colDetailItems";
        colDetailItems.ReadOnly = true;
        colDetailItems.Width = 138;
        // 
        // colDetailSize
        // 
        colDetailSize.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colDetailSize.HeaderText = "Освобождено";
        colDetailSize.MinimumWidth = 100;
        colDetailSize.Name = "colDetailSize";
        colDetailSize.ReadOnly = true;
        colDetailSize.Width = 193;
        // 
        // colDetailSkipped
        // 
        colDetailSkipped.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        colDetailSkipped.HeaderText = "Пропущено";
        colDetailSkipped.MinimumWidth = 85;
        colDetailSkipped.Name = "colDetailSkipped";
        colDetailSkipped.ReadOnly = true;
        colDetailSkipped.Width = 171;
        // 
        // footerPanel
        // 
        footerPanel.Controls.Add(lblFound);
        footerPanel.Controls.Add(lblSelected);
        footerPanel.Controls.Add(lblStatus);
        footerPanel.Controls.Add(progressBar);
        footerPanel.Controls.Add(btnCancel);
        footerPanel.Controls.Add(btnClean);
        footerPanel.Controls.Add(btnAnalyze);
        footerPanel.Dock = DockStyle.Bottom;
        footerPanel.Location = new Point(0, 893);
        footerPanel.Margin = new Padding(5, 5, 5, 5);
        footerPanel.Name = "footerPanel";
        footerPanel.Size = new Size(1435, 122);
        footerPanel.TabIndex = 10;
        // 
        // lblFound
        // 
        lblFound.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblFound.AutoSize = true;
        lblFound.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblFound.Location = new Point(220, 35);
        lblFound.Margin = new Padding(5, 0, 5, 0);
        lblFound.Name = "lblFound";
        lblFound.Size = new Size(286, 30);
        lblFound.TabIndex = 6;
        lblFound.Text = "Сначала выполните анализ";
        // 
        // lblSelected
        // 
        lblSelected.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblSelected.AutoSize = true;
        lblSelected.Location = new Point(24, 35);
        lblSelected.Margin = new Padding(5, 0, 5, 0);
        lblSelected.Name = "lblSelected";
        lblSelected.Size = new Size(121, 30);
        lblSelected.TabIndex = 5;
        lblSelected.Text = "Выбрано: 0";
        // 
        // lblStatus
        // 
        lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lblStatus.Location = new Point(24, 5);
        lblStatus.Margin = new Padding(5, 0, 5, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(662, 30);
        lblStatus.TabIndex = 4;
        lblStatus.Text = "Готово к работе";
        // 
        // progressBar
        // 
        progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        progressBar.Location = new Point(24, 70);
        progressBar.Margin = new Padding(5, 5, 5, 5);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(662, 32);
        progressBar.TabIndex = 3;
        // 
        // btnCancel
        // 
        btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancel.Enabled = false;
        btnCancel.Location = new Point(1221, 50);
        btnCancel.Margin = new Padding(5, 5, 5, 5);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(189, 52);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "Отмена";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += BtnCancel_Click;
        // 
        // btnClean
        // 
        btnClean.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnClean.BackColor = Color.FromArgb(24, 119, 242);
        btnClean.FlatStyle = FlatStyle.Flat;
        btnClean.ForeColor = Color.White;
        btnClean.Location = new Point(941, 50);
        btnClean.Margin = new Padding(5, 5, 5, 5);
        btnClean.Name = "btnClean";
        btnClean.Size = new Size(270, 52);
        btnClean.TabIndex = 1;
        btnClean.Text = "Очистить выбранное";
        btnClean.UseVisualStyleBackColor = false;
        btnClean.Click += BtnClean_Click;
        // 
        // btnAnalyze
        // 
        btnAnalyze.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnAnalyze.Location = new Point(714, 50);
        btnAnalyze.Margin = new Padding(5, 5, 5, 5);
        btnAnalyze.Name = "btnAnalyze";
        btnAnalyze.Size = new Size(217, 52);
        btnAnalyze.TabIndex = 0;
        btnAnalyze.Text = "Анализировать";
        btnAnalyze.UseVisualStyleBackColor = true;
        btnAnalyze.Click += BtnAnalyze_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(168F, 168F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1435, 1015);
        Controls.Add(gridDetails);
        Controls.Add(lblDetailsHeader);
        Controls.Add(lblTaskInfo);
        Controls.Add(gridTasks);
        Controls.Add(chkRestorePoint);
        Controls.Add(chkShowAdvanced);
        Controls.Add(btnClear);
        Controls.Add(btnRecommended);
        Controls.Add(footerPanel);
        Controls.Add(headerPanel);
        Controls.Add(menuStrip);
        Font = new Font("Segoe UI", 9F);
        MainMenuStrip = menuStrip;
        Margin = new Padding(5, 5, 5, 5);
        MinimumSize = new Size(1312, 862);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "TEHCleaner 2.1.2";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gridTasks).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridDetails).EndInit();
        footerPanel.ResumeLayout(false);
        footerPanel.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
