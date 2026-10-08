using System.Diagnostics;
using TEHCleaner.UI;

namespace TEHCleaner;

public partial class AboutForm : Form
{
    public AboutForm()
    {
        InitializeComponent();
        ApplyTheme();
        TrySetIcon();
    }

    private void ApplyTheme()
    {
        UiTheme.Apply(this);
        UiTheme.StyleSecondaryButton(btnClose);
        BackColor = UiTheme.Surface;
        lblName.ForeColor = UiTheme.TextPrimary;
        lblVersion.ForeColor = UiTheme.TextMuted;
        lblText.ForeColor = UiTheme.TextSecondary;
        lblAuthor.ForeColor = UiTheme.TextSecondary;
        linkSite.LinkColor = UiTheme.Accent;
        linkSite.ActiveLinkColor = UiTheme.AccentDark;
        linkSite.VisitedLinkColor = UiTheme.Accent;
    }

    private void TrySetIcon()
    {
        try
        {
            Icon? appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Icon = appIcon;
            pictureLogo.Image = appIcon?.ToBitmap();
        }
        catch { }
    }

    private void LinkSite_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = "https://tehadm.ru/", UseShellExecute = true });
        }
        catch { }
    }

    private void BtnClose_Click(object? sender, EventArgs e) => Close();
}
