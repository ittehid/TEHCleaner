using System.Diagnostics;
using System.Text;
using TEHCleaner.Cleaners;
using TEHCleaner.Models;
using TEHCleaner.Services;
using TEHCleaner.UI;

namespace TEHCleaner;

public partial class MainForm : Form
{
    private readonly AppLogger _logger = new();
    private readonly IReadOnlyList<ICleanerTask> _tasks;
    private readonly Dictionary<string, ScanResult> _scanResults = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, CleanupResult> _cleanupResults = new(StringComparer.OrdinalIgnoreCase);
    private CancellationTokenSource? _operationCts;
    private bool _busy;

    public MainForm()
    {
        InitializeComponent();
        _tasks = CleanupCatalog.Create(_logger);

        ApplyTheme();
        TrySetApplicationIcon();
        PopulateGrid();
        ApplyAdministratorState();
        SelectRecommendedTasks();
        UpdateSummary();
        RefreshDetailsForCurrentRow();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _operationCts?.Dispose();
            _logger.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_busy && e.CloseReason == CloseReason.UserClosing)
        {
            DialogResult result = MessageBox.Show(
                this,
                "Сейчас выполняется операция. Отменить её?",
                "TEHCleaner",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _operationCts?.Cancel();
                lblStatus.Text = "Отмена операции…";
            }

            e.Cancel = true;
            return;
        }

        base.OnFormClosing(e);
    }

    private void TrySetApplicationIcon()
    {
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
        catch { }
    }

    private void ApplyTheme()
    {
        UiTheme.Apply(this);
        UiTheme.StyleMenu(menuStrip);
        UiTheme.StyleSecondaryButton(btnRecommended);
        UiTheme.StyleSecondaryButton(btnClear);
        UiTheme.StyleSecondaryButton(btnAnalyze);
        UiTheme.StylePrimaryButton(btnClean);
        UiTheme.StyleDangerButton(btnCancel);
        UiTheme.StyleAccentOutlineButton(btnAdmin);
        UiTheme.StyleGrid(gridTasks);
        UiTheme.StyleGrid(gridDetails);

        colFoundItems.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colFoundSize.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colCleanedItems.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colCleanedSize.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        colFoundItems.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        colFoundSize.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        colCleanedItems.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        colCleanedSize.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

        BackColor = UiTheme.Background;
        menuStrip.BackColor = UiTheme.Surface;
        headerPanel.BackColor = UiTheme.Surface;
        footerPanel.BackColor = UiTheme.Surface;

        lblSubtitle.Font = new Font("Segoe UI Semibold", 10.25F, FontStyle.Regular, GraphicsUnit.Point);
        lblSubtitle.ForeColor = UiTheme.TextPrimary;

        lblVersion.BackColor = UiTheme.AccentSoft;
        lblVersion.ForeColor = UiTheme.AccentDark;
        lblVersion.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Regular, GraphicsUnit.Point);

        lblAdmin.ForeColor = UiTheme.TextMuted;
        lblTaskInfo.ForeColor = UiTheme.TextMuted;
        lblStatus.ForeColor = UiTheme.TextMuted;
        lblSelected.ForeColor = UiTheme.TextSecondary;
        lblFound.ForeColor = UiTheme.TextPrimary;
        lblDetailsHeader.ForeColor = UiTheme.TextPrimary;
        lblDetailsHeader.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Regular, GraphicsUnit.Point);

        btnAdmin.Text = "Запустить от администратора";

        headerPanel.Paint += HeaderPanel_Paint;
        footerPanel.Paint += FooterPanel_Paint;
        headerPanel.Resize += (_, _) => LayoutHeader();
        Resize += (_, _) => LayoutHeader();
        LayoutHeader();

        gridTasks.CellFormatting -= GridTasks_CellFormatting;
        gridTasks.CellFormatting += GridTasks_CellFormatting;
        gridDetails.CellFormatting -= GridDetails_CellFormatting;
        gridDetails.CellFormatting += GridDetails_CellFormatting;
        gridTasks.CellToolTipTextNeeded -= GridTasks_CellToolTipTextNeeded;
        gridTasks.CellToolTipTextNeeded += GridTasks_CellToolTipTextNeeded;
        gridDetails.CellToolTipTextNeeded -= GridDetails_CellToolTipTextNeeded;
        gridDetails.CellToolTipTextNeeded += GridDetails_CellToolTipTextNeeded;
    }

    private void LayoutHeader()
    {
        const int left = 24;
        const int top = 22;
        headerPanel.Height = 76;

        lblSubtitle.AutoSize = true;
        lblSubtitle.Location = new Point(left, top + 2);

        lblVersion.AutoSize = true;
        lblVersion.Location = new Point(lblSubtitle.Right + 12, top);

        lblAdmin.AutoSize = true;
        lblAdmin.TextAlign = ContentAlignment.MiddleRight;
        int rightPadding = 18;
        int centerY = 24;

        if (btnAdmin.Visible)
        {
            btnAdmin.Size = new Size(235, 34);
            btnAdmin.Location = new Point(headerPanel.ClientSize.Width - btnAdmin.Width - rightPadding, 18);
            lblAdmin.Location = new Point(btnAdmin.Left - lblAdmin.PreferredWidth - 14, centerY + 4);
        }
        else
        {
            lblAdmin.Location = new Point(headerPanel.ClientSize.Width - lblAdmin.PreferredWidth - rightPadding, centerY + 4);
        }
    }

    private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
    {
        using Pen pen = new(UiTheme.Border);
        e.Graphics.DrawLine(pen, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
    }

    private void FooterPanel_Paint(object? sender, PaintEventArgs e)
    {
        using Pen pen = new(UiTheme.Border);
        e.Graphics.DrawLine(pen, 0, 0, footerPanel.Width, 0);
    }

    private void PopulateGrid()
    {
        gridTasks.Rows.Clear();

        foreach (ICleanerTask task in _tasks)
        {
            int rowIndex = gridTasks.Rows.Add(
                false,
                task.Category,
                task.Name,
                "—",
                "—",
                "—",
                "—",
                task.IsAvailable ? "Не анализировалось" : "Не найдено");

            DataGridViewRow row = gridTasks.Rows[rowIndex];
            row.Tag = task;
            row.Cells[colSelect.Index].ReadOnly = !task.IsAvailable;
            row.Cells[colName.Index].ToolTipText = task.Description;
            if (!task.IsAvailable)
                row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
        }

        ApplyAdvancedFilter();
        if (gridTasks.Rows.Count > 0)
            gridTasks.Rows[0].Selected = true;
    }

    private void ApplyAdministratorState()
    {
        bool admin = ElevationService.IsAdministrator();
        lblAdmin.Text = admin ? "● Администратор" : "○ Обычный режим";
        btnAdmin.Visible = !admin;
        chkRestorePoint.Enabled = admin;
        chkRestorePoint.Text = admin
            ? "Точка восстановления перед очисткой"
            : "Точка восстановления (нужен администратор)";
        LayoutHeader();
    }

    private void SelectRecommendedTasks()
    {
        bool admin = ElevationService.IsAdministrator();
        foreach (DataGridViewRow row in gridTasks.Rows)
        {
            if (row.Tag is not ICleanerTask task) continue;
            row.Cells[colSelect.Index].Value =
                task.IsAvailable &&
                task.Recommended &&
                (!task.Advanced || chkShowAdvanced.Checked) &&
                (!task.RequiresAdministrator || admin);
        }
        UpdateSummary();
    }

    private void ClearSelection()
    {
        foreach (DataGridViewRow row in gridTasks.Rows)
            row.Cells[colSelect.Index].Value = false;
        UpdateSummary();
    }

    private IReadOnlyList<ICleanerTask> GetSelectedTasks()
    {
        List<ICleanerTask> result = [];
        foreach (DataGridViewRow row in gridTasks.Rows)
        {
            if (row.Tag is not ICleanerTask task || !task.IsAvailable) continue;
            bool selected = Convert.ToBoolean(row.Cells[colSelect.Index].Value ?? false);
            if (selected) result.Add(task);
        }
        return result;
    }

    private DataGridViewRow? FindRow(ICleanerTask task) =>
        gridTasks.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => ReferenceEquals(r.Tag, task));

    private async void BtnAnalyze_Click(object? sender, EventArgs e)
    {
        if (_busy) return;
        IReadOnlyList<ICleanerTask> selected = GetSelectedTasks();
        if (selected.Count == 0)
        {
            MessageBox.Show(this, "Выберите хотя бы один пункт для анализа.", "TEHCleaner", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        await AnalyzeAsync(selected);
    }

    private async Task AnalyzeAsync(IReadOnlyList<ICleanerTask> selected)
    {
        BeginOperation("Анализ системы…");
        _scanResults.Clear();
        long totalBytes = 0;
        int completed = 0;

        try
        {
            CancellationToken token = _operationCts!.Token;
            await _logger.InfoAsync($"=== Анализ начат. Выбрано задач: {selected.Count} ===");

            foreach (ICleanerTask task in selected)
            {
                token.ThrowIfCancellationRequested();
                SetRowStatus(task, "Анализ…");
                ClearPreviousCleanupResult(task);

                try
                {
                    ScanResult result = await task.ScanAsync(token);
                    _scanResults[task.Id] = result;
                    totalBytes += result.Bytes;
                    UpdateScanRow(task, result);
                    await _logger.InfoAsync($"[{task.Name}] найдено {result.Items:N0}, {FormatHelper.Bytes(result.Bytes)}, пропущено {result.Skipped:N0}");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    SetRowStatus(task, "Ошибка анализа");
                    await _logger.ErrorAsync($"[{task.Name}] ошибка анализа: {ex.Message}");
                }

                completed++;
                SetProgress(completed, selected.Count);
            }

            lblFound.Text = $"Найдено: {FormatHelper.Bytes(totalBytes)}";
            lblStatus.Text = "Анализ завершён";
        }
        catch (OperationCanceledException)
        {
            lblStatus.Text = "Анализ отменён";
            await _logger.WarningAsync("Анализ отменён пользователем.");
        }
        finally
        {
            EndOperation();
        }
    }

    private async void BtnClean_Click(object? sender, EventArgs e)
    {
        if (_busy) return;
        IReadOnlyList<ICleanerTask> selected = GetSelectedTasks();
        if (selected.Count == 0)
        {
            MessageBox.Show(this, "Выберите хотя бы один пункт очистки.", "TEHCleaner", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        bool needsAdmin = selected.Any(t => t.RequiresAdministrator) || chkRestorePoint.Checked;
        if (needsAdmin && !ElevationService.IsAdministrator())
        {
            DialogResult restart = MessageBox.Show(
                this,
                "Среди выбранных операций есть действия, требующие прав администратора.\n\nПерезапустить TEHCleaner от имени администратора?",
                "Требуется повышение прав",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (restart == DialogResult.Yes && ElevationService.RelaunchAsAdministrator())
                Close();
            return;
        }

        if (selected.Any(t => !_scanResults.ContainsKey(t.Id)))
        {
            await AnalyzeAsync(selected);
            if (_operationCts?.IsCancellationRequested == true) return;
        }

        long plannedBytes = selected.Sum(t => _scanResults.TryGetValue(t.Id, out ScanResult? r) ? r.Bytes : 0);
        DialogResult confirm = MessageBox.Show(
            this,
            $"Будет выполнено операций: {selected.Count}\nНайдено данных: {FormatHelper.Bytes(plannedBytes)}\n\nПродолжить очистку?",
            "Подтверждение очистки",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        BeginOperation("Очистка…");
        long freed = 0;
        int removed = 0;
        int completed = 0;

        try
        {
            CancellationToken token = _operationCts!.Token;
            await _logger.InfoAsync($"=== Очистка начата. Выбрано задач: {selected.Count} ===");

            if (chkRestorePoint.Checked)
            {
                lblStatus.Text = "Создание точки восстановления…";
                RestorePointService restorePoint = new(_logger);
                (bool Success, string Message) restoreResult = await restorePoint.TryCreateAsync(token);
                if (!restoreResult.Success)
                {
                    DialogResult continueWithoutRestore = MessageBox.Show(
                        this,
                        $"Точку восстановления создать не удалось:\n\n{restoreResult.Message}\n\nПродолжить очистку без неё?",
                        "Точка восстановления",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    if (continueWithoutRestore != DialogResult.Yes) return;
                }
            }

            foreach (ICleanerTask task in selected)
            {
                token.ThrowIfCancellationRequested();
                lblStatus.Text = $"Очистка: {task.Name}";
                SetRowStatus(task, "Очистка…");

                try
                {
                    CleanupResult result = await task.CleanAsync(token);
                    freed += result.BytesFreed;
                    removed += result.ItemsRemoved;
                    _scanResults.Remove(task.Id);
                    _cleanupResults[task.Id] = result;
                    SetCleanupRow(task, result);
                    await _logger.InfoAsync($"[{task.Name}] результат: removed={result.ItemsRemoved}, skipped={result.Skipped}, freed={FormatHelper.Bytes(result.BytesFreed)}");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    CleanupResult failed = CleanupResult.Failed(ex.Message);
                    _cleanupResults[task.Id] = failed;
                    SetCleanupRow(task, failed);
                    await _logger.ErrorAsync($"[{task.Name}] ошибка очистки: {ex.Message}");
                }

                completed++;
                SetProgress(completed, selected.Count);
            }

            lblFound.Text = $"Освобождено: {FormatHelper.Bytes(freed)}";
            lblStatus.Text = $"Готово • удалено {removed:N0} объектов";
            saveReportMenu.Enabled = _cleanupResults.Count > 0;
            RefreshDetailsForCurrentRow();
            await _logger.InfoAsync($"=== Очистка завершена. Удалено {removed:N0}, освобождено {FormatHelper.Bytes(freed)} ===");

            MessageBox.Show(
                this,
                $"Очистка завершена.\n\nУдалено: {removed:N0} объектов\nОсвобождено: {FormatHelper.Bytes(freed)}\n\nПодробности видны в блоке «Что очищено».",
                "TEHCleaner",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
            lblStatus.Text = "Очистка отменена";
            saveReportMenu.Enabled = _cleanupResults.Count > 0;
            RefreshDetailsForCurrentRow();
            await _logger.WarningAsync("Очистка отменена пользователем.");
        }
        finally
        {
            EndOperation();
        }
    }

    private void BeginOperation(string status)
    {
        _busy = true;
        _operationCts?.Dispose();
        _operationCts = new CancellationTokenSource();
        progressBar.Value = 0;
        lblStatus.Text = status;
        btnAnalyze.Enabled = false;
        btnClean.Enabled = false;
        btnRecommended.Enabled = false;
        btnClear.Enabled = false;
        chkShowAdvanced.Enabled = false;
        btnCancel.Enabled = true;
    }

    private void EndOperation()
    {
        _busy = false;
        btnAnalyze.Enabled = true;
        btnClean.Enabled = true;
        btnRecommended.Enabled = true;
        btnClear.Enabled = true;
        chkShowAdvanced.Enabled = true;
        btnCancel.Enabled = false;
        lblSelected.Text = $"Выбрано: {GetSelectedTasks().Count}";
    }

    private void SetProgress(int completed, int total)
    {
        progressBar.Value = total <= 0 ? 0 : Math.Clamp(completed * 100 / total, 0, 100);
    }

    private void UpdateScanRow(ICleanerTask task, ScanResult result)
    {
        DataGridViewRow? row = FindRow(task);
        if (row is null) return;

        row.Cells[colFoundItems.Index].Value = result.Items > 0 ? result.Items.ToString("N0") : "—";
        row.Cells[colFoundSize.Index].Value = result.Bytes > 0 ? FormatHelper.Bytes(result.Bytes) : "—";
        row.Cells[colFoundItems.Index].ToolTipText = result.Items > 0
            ? task.Id == "registry-history" ? $"Найдено записей реестра: {result.Items:N0}" : $"Найдено объектов: {result.Items:N0}"
            : "Объекты не найдены";
        row.Cells[colFoundSize.Index].ToolTipText = result.Bytes > 0
            ? $"Объём найденных данных: {FormatHelper.Bytes(result.Bytes)}"
            : task.Id == "registry-history" ? "Для записей реестра размер не рассчитывается" : "Объём: 0 Б";

        row.Cells[colCleanedItems.Index].Value = "—";
        row.Cells[colCleanedSize.Index].Value = "—";
        row.Cells[colCleanedItems.Index].ToolTipText = string.Empty;
        row.Cells[colCleanedSize.Index].ToolTipText = string.Empty;
        row.Cells[colStatus.Index].Value = !result.IsAvailable
            ? "Не найдено"
            : result.Skipped > 0 ? $"Готово • пропущено {result.Skipped:N0}" : "Готово";

        row.Cells[colStatus.Index].ToolTipText = BuildStatusToolTip(result.Note, result.Skipped);
    }

    private void SetCleanupRow(ICleanerTask task, CleanupResult result)
    {
        DataGridViewRow? row = FindRow(task);
        if (row is null) return;

        row.Cells[colCleanedItems.Index].Value = result.ItemsRemoved > 0 ? result.ItemsRemoved.ToString("N0") : "—";
        row.Cells[colCleanedSize.Index].Value = result.BytesFreed > 0 ? FormatHelper.Bytes(result.BytesFreed) : "—";
        row.Cells[colCleanedItems.Index].ToolTipText = result.Success
            ? task.Id == "registry-history" ? $"Удалено записей реестра: {result.ItemsRemoved:N0}" : $"Удалено объектов: {result.ItemsRemoved:N0}"
            : result.Note ?? "Операция не выполнена";
        row.Cells[colCleanedSize.Index].ToolTipText = result.Success
            ? result.BytesFreed > 0 ? $"Освобождено: {FormatHelper.Bytes(result.BytesFreed)}" : task.Id == "registry-history" ? "Для записей реестра размер не рассчитывается" : "Освобождено: 0 Б"
            : result.Note ?? "Операция не выполнена";

        row.Cells[colStatus.Index].Value = result.Success
            ? result.Skipped > 0 ? $"Очищено • пропущено {result.Skipped:N0}" : "Очищено"
            : "Не выполнено";
        row.Cells[colStatus.Index].ToolTipText = BuildStatusToolTip(result.Note, result.Skipped);
    }

    private static string BuildStatusToolTip(string? note, int skipped)
    {
        List<string> lines = [];
        if (!string.IsNullOrWhiteSpace(note)) lines.Add(note);
        if (skipped > 0) lines.Add($"Пропущено объектов: {skipped:N0}");
        return string.Join(Environment.NewLine, lines);
    }

    private void ClearPreviousCleanupResult(ICleanerTask task)
    {
        _cleanupResults.Remove(task.Id);
        DataGridViewRow? row = FindRow(task);
        if (row is not null)
        {
            row.Cells[colCleanedItems.Index].Value = "—";
            row.Cells[colCleanedSize.Index].Value = "—";
        }
        saveReportMenu.Enabled = _cleanupResults.Count > 0;
    }

    private void SetRowStatus(ICleanerTask task, string status)
    {
        DataGridViewRow? row = FindRow(task);
        if (row is not null) row.Cells[colStatus.Index].Value = status;
    }

    private void BtnCancel_Click(object? sender, EventArgs e) => _operationCts?.Cancel();

    private void BtnRecommended_Click(object? sender, EventArgs e) => SelectRecommendedTasks();

    private void BtnClear_Click(object? sender, EventArgs e) => ClearSelection();

    private void ChkShowAdvanced_CheckedChanged(object? sender, EventArgs e) => ApplyAdvancedFilter();

    private void ApplyAdvancedFilter()
    {
        gridTasks.CurrentCell = null;
        foreach (DataGridViewRow row in gridTasks.Rows)
        {
            if (row.Tag is not ICleanerTask task) continue;
            bool visible = chkShowAdvanced.Checked || !task.Advanced;
            row.Visible = visible;
            if (!visible) row.Cells[colSelect.Index].Value = false;
        }
        UpdateSummary();
    }

    private void GridTasks_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == colSelect.Index) UpdateSummary();
    }

    private void GridTasks_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (gridTasks.IsCurrentCellDirty)
            gridTasks.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void GridTasks_SelectionChanged(object? sender, EventArgs e) => RefreshDetailsForCurrentRow();

    private void RefreshDetailsForCurrentRow()
    {
        gridDetails.Rows.Clear();
        if (gridTasks.CurrentRow?.Tag is not ICleanerTask task)
        {
            lblTaskInfo.Text = "Выберите строку, чтобы увидеть описание и подробности результата.";
            lblDetailsHeader.Text = "Подробности";
            return;
        }

        lblTaskInfo.Text = $"{task.Name}: {task.Description}  [{BuildFlags(task)}]";

        if (!_cleanupResults.TryGetValue(task.Id, out CleanupResult? result))
        {
            lblDetailsHeader.Text = "Подробности";
            int placeholderIndex = gridDetails.Rows.Add("—", "После очистки здесь появятся конкретные очищенные пути.", "—", "—", "—");
            gridDetails.Rows[placeholderIndex].Cells[colDetailPath.Index].ToolTipText =
                "После выполнения очистки здесь будут показаны фактические пути и результат по каждому из них.";
            return;
        }

        lblDetailsHeader.Text = $"Что очищено — {task.Name}";

        if (result.Details is { Count: > 0 })
        {
            foreach (CleanupDetail detail in result.Details)
            {
                int rowIndex = gridDetails.Rows.Add(
                    detail.Area,
                    detail.Path,
                    detail.ItemsRemoved > 0 ? detail.ItemsRemoved.ToString("N0") : "—",
                    detail.BytesFreed > 0 ? FormatHelper.Bytes(detail.BytesFreed) : "—",
                    detail.Skipped > 0 ? detail.Skipped.ToString("N0") : "—");

                DataGridViewRow detailRow = gridDetails.Rows[rowIndex];
                detailRow.Cells[colDetailArea.Index].ToolTipText = detail.Area;
                detailRow.Cells[colDetailPath.Index].ToolTipText = detail.Path;
                detailRow.Cells[colDetailItems.Index].ToolTipText = $"Удалено объектов: {detail.ItemsRemoved:N0}";
                detailRow.Cells[colDetailSize.Index].ToolTipText = $"Освобождено: {FormatHelper.Bytes(detail.BytesFreed)}";
                detailRow.Cells[colDetailSkipped.Index].ToolTipText = $"Пропущено: {detail.Skipped:N0}";
            }
        }
        else
        {
            string detailText = result.Note ?? (result.Success
                ? "Операция выполнена; файловые объекты не удалялись."
                : "Нет подробностей.");

            int rowIndex = gridDetails.Rows.Add(
                result.Success ? task.Name : "Не выполнено",
                detailText,
                result.ItemsRemoved > 0 ? result.ItemsRemoved.ToString("N0") : "—",
                result.BytesFreed > 0 ? FormatHelper.Bytes(result.BytesFreed) : "—",
                result.Skipped > 0 ? result.Skipped.ToString("N0") : "—");
            gridDetails.Rows[rowIndex].Cells[colDetailPath.Index].ToolTipText = detailText;
        }
    }

    private static string BuildFlags(ICleanerTask task)
    {
        List<string> flags = [];
        if (task.Recommended) flags.Add("рекомендуется");
        if (task.Advanced) flags.Add("расширенная");
        if (task.RequiresAdministrator) flags.Add("администратор");
        return flags.Count == 0 ? "стандартная" : string.Join(" • ", flags);
    }

    private void GridTasks_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        if (gridTasks.Rows[e.RowIndex].Tag is not ICleanerTask task)
            return;

        if (e.ColumnIndex == colStatus.Index)
        {
            string text = Convert.ToString(e.Value) ?? string.Empty;
            e.CellStyle.ForeColor = text.Contains("Ошибка", StringComparison.OrdinalIgnoreCase) || text.Contains("Не выполнено", StringComparison.OrdinalIgnoreCase)
                ? UiTheme.Danger
                : text.Contains("Очищено", StringComparison.OrdinalIgnoreCase)
                    ? UiTheme.Success
                    : text.Contains("Анализ", StringComparison.OrdinalIgnoreCase) || text.Contains("Очистка", StringComparison.OrdinalIgnoreCase)
                        ? UiTheme.AccentDark
                        : text.Contains("Не найдено", StringComparison.OrdinalIgnoreCase)
                            ? UiTheme.TextMuted
                            : UiTheme.TextSecondary;
            e.CellStyle.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Regular, GraphicsUnit.Point);
        }
        else if (e.ColumnIndex == colFoundItems.Index || e.ColumnIndex == colFoundSize.Index ||
                 e.ColumnIndex == colCleanedItems.Index || e.ColumnIndex == colCleanedSize.Index)
        {
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            e.CellStyle.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Regular, GraphicsUnit.Point);
        }
        else if (e.ColumnIndex == colCategory.Index)
        {
            e.CellStyle.ForeColor = UiTheme.TextSecondary;
            e.CellStyle.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Regular, GraphicsUnit.Point);
        }
        else if (e.ColumnIndex == colName.Index)
        {
            e.CellStyle.ForeColor = task.IsAvailable ? UiTheme.TextPrimary : UiTheme.TextMuted;
        }
    }

    private void GridDetails_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        if (e.ColumnIndex == colDetailArea.Index)
        {
            e.CellStyle.ForeColor = UiTheme.TextSecondary;
            e.CellStyle.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Regular, GraphicsUnit.Point);
        }
        else if (e.ColumnIndex == colDetailItems.Index || e.ColumnIndex == colDetailSize.Index || e.ColumnIndex == colDetailSkipped.Index)
        {
            e.CellStyle.Font = new Font("Segoe UI Semibold", 8F, FontStyle.Regular, GraphicsUnit.Point);
        }
    }

    private void GridTasks_CellToolTipTextNeeded(object? sender, DataGridViewCellToolTipTextNeededEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;
        e.ToolTipText = Convert.ToString(gridTasks.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText) ?? string.Empty;
    }

    private void GridDetails_CellToolTipTextNeeded(object? sender, DataGridViewCellToolTipTextNeededEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
            return;
        e.ToolTipText = Convert.ToString(gridDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText) ?? string.Empty;
    }

    private void UpdateSummary()
    {
        IReadOnlyList<ICleanerTask> selected = GetSelectedTasks();
        lblSelected.Text = $"Выбрано: {selected.Count}";
        long bytes = selected.Sum(t => _scanResults.TryGetValue(t.Id, out ScanResult? r) ? r.Bytes : 0);
        lblFound.Text = bytes > 0 ? $"Найдено: {FormatHelper.Bytes(bytes)}" : "Сначала выполните анализ";
    }

    private void SaveReportMenu_Click(object? sender, EventArgs e)
    {
        if (_cleanupResults.Count == 0) return;

        using SaveFileDialog dialog = new()
        {
            Title = "Сохранить отчёт TEHCleaner",
            Filter = "Текстовый файл (*.txt)|*.txt|Все файлы (*.*)|*.*",
            FileName = $"TEHCleaner-report-{DateTime.Now:yyyy-MM-dd_HH-mm}.txt",
            AddExtension = true,
            DefaultExt = "txt"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            StringBuilder report = new();
            report.AppendLine("TEHCleaner 2.1.2 — отчёт об очистке");
            report.AppendLine($"Дата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            report.AppendLine(new string('=', 72));

            long totalBytes = 0;
            int totalItems = 0;
            int totalSkipped = 0;

            foreach (ICleanerTask task in _tasks)
            {
                if (!_cleanupResults.TryGetValue(task.Id, out CleanupResult? result)) continue;

                totalBytes += result.BytesFreed;
                totalItems += result.ItemsRemoved;
                totalSkipped += result.Skipped;

                report.AppendLine();
                report.AppendLine($"[{task.Category}] {task.Name}");
                report.AppendLine($"Статус: {(result.Success ? "выполнено" : "не выполнено")}");
                report.AppendLine($"Удалено объектов: {result.ItemsRemoved:N0}");
                report.AppendLine($"Освобождено: {FormatHelper.Bytes(result.BytesFreed)}");
                report.AppendLine($"Пропущено: {result.Skipped:N0}");
                if (!string.IsNullOrWhiteSpace(result.Note)) report.AppendLine($"Примечание: {result.Note}");

                if (result.Details is { Count: > 0 })
                {
                    report.AppendLine("Подробности:");
                    foreach (CleanupDetail detail in result.Details)
                    {
                        report.AppendLine($"  - {detail.Area}");
                        report.AppendLine($"    {detail.Path}");
                        report.AppendLine($"    удалено: {detail.ItemsRemoved:N0}; освобождено: {FormatHelper.Bytes(detail.BytesFreed)}; пропущено: {detail.Skipped:N0}");
                    }
                }
            }

            report.AppendLine();
            report.AppendLine(new string('=', 72));
            report.AppendLine($"ИТОГО: удалено {totalItems:N0} объектов; освобождено {FormatHelper.Bytes(totalBytes)}; пропущено {totalSkipped:N0}");
            File.WriteAllText(dialog.FileName, report.ToString(), new UTF8Encoding(true));
            Process.Start(new ProcessStartInfo { FileName = dialog.FileName, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Не удалось сохранить отчёт:\n\n{ex.Message}", "TEHCleaner", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAdmin_Click(object? sender, EventArgs e)
    {
        if (ElevationService.RelaunchAsAdministrator()) Close();
    }

    private void OpenLogsMenu_Click(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(_logger.LogDirectory);
        Process.Start(new ProcessStartInfo { FileName = _logger.LogDirectory, UseShellExecute = true });
    }

    private void OpenCurrentLogMenu_Click(object? sender, EventArgs e)
    {
        if (!File.Exists(_logger.CurrentLogPath))
            File.WriteAllText(_logger.CurrentLogPath, string.Empty);
        Process.Start(new ProcessStartInfo { FileName = _logger.CurrentLogPath, UseShellExecute = true });
    }

    private void OpenRegistryBackupsMenu_Click(object? sender, EventArgs e)
    {
        Directory.CreateDirectory(RegistryBackupService.RootDirectory);
        Process.Start(new ProcessStartInfo { FileName = RegistryBackupService.RootDirectory, UseShellExecute = true });
    }

    private void HomeMenu_Click(object? sender, EventArgs e) => OpenUrl("https://tehadm.ru/");

    private void AboutMenu_Click(object? sender, EventArgs e)
    {
        using AboutForm form = new();
        form.ShowDialog(this);
    }

    private static void OpenUrl(string url)
    {
        try { Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true }); }
        catch { }
    }

    private void ExitMenu_Click(object? sender, EventArgs e) => Close();
}
