using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IsoToUsb
{
    public class MainForm : Form
    {
        private TextBox txtIso;
        private Button btnBrowseIso;
        private ComboBox cboDrives;
        private Button btnRefreshDrives;
        private Button btnStart;
        private Label lblStatus;

        public MainForm()
        {
            Text = "Project ISO 9660 (BETA)";
            Width = 600;
            Height = 200;
            StartPosition = FormStartPosition.CenterScreen;

            // Make the window fixed-size so it cannot be resized
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            InitializeComponents();
            RefreshDrives();
        }

        private void InitializeComponents()
        {
            txtIso = new TextBox { Left = 12, Top = 12, Width = 450 };
            // let the button autosize so its text is never squished
            btnBrowseIso = new Button { Left = 470, Top = 10, AutoSize = true, Text = "Browse..." };
            btnBrowseIso.Click += BtnBrowseIso_Click;

            cboDrives = new ComboBox { Left = 12, Top = 50, Width = 450, DropDownStyle = ComboBoxStyle.DropDownList };
            // let the refresh button autosize as well
            btnRefreshDrives = new Button { Left = 470, Top = 48, AutoSize = true, Text = "Refresh" };
            btnRefreshDrives.Click += (s, e) => RefreshDrives();

            // make the Start button a bit taller so the label isn't cramped
            btnStart = new Button { Left = 12, Top = 90, Width = 560, Height = 36, Text = "Start", TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
            btnStart.Click += BtnStart_Click;

            lblStatus = new Label { Left = 12, Top = 130, Width = 560, Text = "Ready" };

            Controls.Add(txtIso);
            Controls.Add(btnBrowseIso);
            Controls.Add(cboDrives);
            Controls.Add(btnRefreshDrives);
            Controls.Add(btnStart);
            Controls.Add(lblStatus);
        }

        private void BtnBrowseIso_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "ISO files (*.iso)|*.iso|All files (*.*)|*.*";
            dlg.Title = "Select ISO file";
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                txtIso.Text = dlg.FileName;
            }
        }

        private void RefreshDrives()
        {
            cboDrives.Items.Clear();
            var drives = DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Removable)
                .Select(d => new { d.Name, Label = string.IsNullOrEmpty(d.VolumeLabel) ? "Removable" : d.VolumeLabel })
                .ToList();

            foreach (var d in drives)
                cboDrives.Items.Add($"{d.Name} ({d.Label})");

            if (cboDrives.Items.Count > 0)
                cboDrives.SelectedIndex = 0;
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            var isoPath = txtIso.Text.Trim();
            if (string.IsNullOrEmpty(isoPath) || !File.Exists(isoPath))
            {
                MessageBox.Show(this, "Please select a valid ISO file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cboDrives.SelectedIndex < 0)
            {
                MessageBox.Show(this, "Please select a target USB drive.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // For now just simulate the operation
            lblStatus.Text = "Starting... (simulation)";
            btnStart.Enabled = false;

            // In a later step we'll implement the actual writing logic with proper permissions and safety checks.
            System.Threading.Tasks.Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1500);
                this.Invoke(() =>
                {
                    lblStatus.Text = "Done (simulation).";
                    btnStart.Enabled = true;
                });
            });
        }
    }
}
