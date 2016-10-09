using System;
using System.Threading;
using System.Windows.Forms;
using CoreTweet;
using CoreTweet.Streaming;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Media;

namespace Mystter_Notification {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        bool HideForm = true;
        Tokens twitter;
        string BalloonClickedUrl = "";

        private void Form1_Load(object sender, EventArgs e) {
            TwitterInit();

            var streamThread = new Thread(new ThreadStart(Streaming));
            streamThread.Start();

            if (HideForm)
                Opacity = 0;
        }

        private void TwitterInit() {
            twitter = Tokens.Create(SecretKeys.ConsumerKey, SecretKeys.ConsumerSecret, SecretKeys.AccessToken, SecretKeys.AccessTokenSecret);
        }

        private void Form1_Shown(object sender, EventArgs e) {
            if (HideForm)
                Hide();
        }

        private void Streaming() {
            
        }


        private delegate void ShowBalloonTipCallBack(string text, string title, int timeout = 1000, ToolTipIcon icon = ToolTipIcon.None);
        private void ShowBalloonTipAsync(string text, string title, int timeout = 1000, ToolTipIcon icon = ToolTipIcon.None) {
            if (InvokeRequired) {
                var method = new ShowBalloonTipCallBack(ShowBalloonTipAsync);
                Invoke(method, new object[] { text, title, timeout, icon });
            } else {
                ShowBalloonTip(text, title, timeout, icon);
            }
        }

        private void ShowBalloonTip(string text, string title, int timeout = 1000, ToolTipIcon icon = ToolTipIcon.None, string url = "") {
            notifyIcon1.BalloonTipText = text;
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipIcon = icon;
            notifyIcon1.ShowBalloonTip(timeout);
            BalloonClickedUrl = url;
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(BalloonClickedUrl))
                Process.Start(BalloonClickedUrl);
        }
    }
}
