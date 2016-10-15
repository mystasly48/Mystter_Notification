using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using CoreTweet;
using CoreTweet.Streaming;
using Mystter_Notification.Properties;

namespace Mystter_Notification {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        bool HideForm = true;
        Tokens twitter;
        string BalloonClickedUrl = "";
        const string SCREEN_NAME = "30msl";
        const string TWITTER_URL = "https://twitter.com/";

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
            var eventStream = twitter.Streaming.UserAsObservable().Publish();
            eventStream.OfType<EventMessage>().Where(m => m.Source.ScreenName != SCREEN_NAME).Subscribe(m => ReceivedEventMessage(m));
            eventStream.Connect();

            var replyStream = twitter.Streaming.FilterAsObservable(track: "@" + SCREEN_NAME).Publish();
            replyStream.OfType<StatusMessage>().Subscribe(m => ReceivedReplyMessage(m));
            replyStream.Connect();
        }

        private void ReceivedReplyMessage(StatusMessage m) {
            ShowBalloonTipAsync(m.Status.Text, $"Reply : {m.Status.User.Name} @({m.Status.User.ScreenName})");
            BalloonClickedUrl = TWITTER_URL + m.Status.User.ScreenName + "/status/" + m.Status.Id;
        }

        private void ReceivedEventMessage(EventMessage m) {
            var user = $"{m.Source.Name} (@{m.Source.ScreenName})";
            var userAndEvent = GetEventName(m.Event) + " : " + user;
            var targetStatusText = (m.TargetStatus != null) ? m.TargetStatus.Text : null;
            var targetListName = (m.TargetList != null) ? m.TargetList.Name : null;
            if (m.Event == EventCode.Follow) {
                ShowBalloonTipAsync(userAndEvent);
                BalloonClickedUrl = TWITTER_URL + m.Source.ScreenName;
            } else if (m.Event == EventCode.Favorite || m.Event == EventCode.Unfavorite || m.Event == EventCode.QuotedTweet || m.Event == EventCode.FavoritedRetweet || m.Event == EventCode.RetweetedRetweet) {
                ShowBalloonTipAsync(targetStatusText, userAndEvent);
                BalloonClickedUrl = TWITTER_URL + m.TargetStatus.User.ScreenName + "/status/" + m.TargetStatus.Id;
            } else if (m.Event == EventCode.ListMemberAdded || m.Event == EventCode.ListMemberRemoved || m.Event == EventCode.ListUserSubscribed || m.Event == EventCode.ListUserUnsubscribed) {
                ShowBalloonTipAsync(targetListName, userAndEvent);
                BalloonClickedUrl = TWITTER_URL + m.Source.ScreenName + "/lists/" + m.TargetList.Slug;
            } else {
                ShowBalloonTipAsync(m.Event.ToString());
            }
        }

        private delegate void ShowBalloonTipCallBack(string text = "", string title = "", int timeout = 3000, ToolTipIcon icon = ToolTipIcon.None);
        private void ShowBalloonTipAsync(string title, string text = "", int timeout = 3000, ToolTipIcon icon = ToolTipIcon.None) {
            if (InvokeRequired) {
                var method = new ShowBalloonTipCallBack(ShowBalloonTipAsync);
                Invoke(method, new object[] { text, title, timeout, icon });
            } else {
                ShowBalloonTip(text, title, timeout, icon);
            }
        }

        private void ShowBalloonTip(string text = "", string title = "", int timeout = 3000, ToolTipIcon icon = ToolTipIcon.None, string url = "") {
            notifyIcon1.BalloonTipText = text;
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipIcon = icon;
            notifyIcon1.ShowBalloonTip(timeout);
            BalloonClickedUrl = url;
            PlaySound(Resources.se);
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(BalloonClickedUrl))
                Process.Start(BalloonClickedUrl);
        }

        private string GetEventName(EventCode code) {
            switch (code) {
                case EventCode.Follow:
                    return "Followed";
                case EventCode.Favorite:
                    return "Liked";
                case EventCode.Unfavorite:
                    return "Unliked";
                case EventCode.QuotedTweet:
                    return "Quoted";
                case EventCode.ListMemberAdded:
                    return "Listed";
                case EventCode.ListMemberRemoved:
                    return "Unlisted";
                case EventCode.ListUserSubscribed:
                    return "Subscribed";
                case EventCode.ListUserUnsubscribed:
                    return "Unsubscribed";
                case EventCode.FavoritedRetweet:
                    return "Retweet Favorited";
                case EventCode.RetweetedRetweet:
                    return "Retweet Retweeted";
                default:
                    return null;
            }
        }

        private void PlaySound(Stream sound) {
            var player = new SoundPlayer(sound);
            player.Play();
        }
    }
}
