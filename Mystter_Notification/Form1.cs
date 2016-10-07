using System;
using System.Threading;
using System.Windows.Forms;
using CoreTweet;

namespace Mystter_Notification {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        bool HideForm = true;
        Tokens twitter;

        private void Form1_Load(object sender, EventArgs e) {
            var streamThread = new Thread(new ThreadStart(Streaming));
            streamThread.Start();

            if (HideForm) Opacity = 0;
        }

        private void Form1_Shown(object sender, EventArgs e) {
            if (HideForm) Hide();
        }

        private void Streaming() {
            var stream = twitter.
        }

        private void ShowBalloonTip(string text, string title, ToolTipIcon icon, int timeout) {
            notifyIcon1.BalloonTipText = text;
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipIcon = icon;
            notifyIcon1.ShowBalloonTip(timeout);
        }

        private void button1_Click(object sender, EventArgs e) {
            ShowBalloonTip("Text", "Title", ToolTipIcon.Warning, 1000);

        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }
    }
}
