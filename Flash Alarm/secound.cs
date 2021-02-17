using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading.Tasks;
using System;
using Android.Media;

namespace Flash_Alarm
{
    [Activity(Label = "secound")]
    public class secound : Activity
    {
        protected MediaPlayer player;
        public TextView NumberDisplay { get; private set; }
        public TextView ButtonDisplay { get; private set; }
        public TextView Number { get; private set; }

        public bool button = true;
        public int ans;

        Random rnd = new Random();

        public void StartPlayer(string filePath)
        {
            if (player == null)
            {
                player = new MediaPlayer();
                player.SetDataSource(filePath);
                player.Prepare();
                player.Start();
            }
            else
            {
                player.Reset();
                player.SetDataSource(filePath);
                player.Prepare();
                player.Start();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.secound);

            ButtonDisplay = FindViewById<Button>(Resource.Id.activity_button);
            Number = FindViewById<EditText>(Resource.Id.number);
            NumberDisplay = FindViewById<TextView>(Resource.Id.numberdisplay);

            Button activity_button = FindViewById<Button>(Resource.Id.activity_button);
            activity_button.Click += Activity_button_Click;

            StartPlayer(this.Intent.GetStringExtra("Message"));
        }

        private async void Start()
        {
            Button activity_button = FindViewById<Button>(Resource.Id.activity_button);
            activity_button.Enabled = false;

            Number.Text = "";

            int num;
            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                num = rnd.Next(1, 10);
                sum += num;
                NumberDisplay.Text = num.ToString();
                await Task.Delay(1000);
                NumberDisplay.Text = "";
                await Task.Delay(50);
            }
            NumberDisplay.Text = "答えを入力してください";
            ans = sum;

            activity_button.Enabled = true;
        }

        private void Judge()
        {
            if (Number.Text == ans.ToString())
            {
                NumberDisplay.Text = "正解です";
                player.Reset();
            }
            else
            {
                NumberDisplay.Text = "それじゃあダメなんよ";
            }
        }

        private void Activity_button_Click(object sender, System.EventArgs e)
        {
            if (button)
            {
                Start();
                button = false;
                ButtonDisplay.Text = "入力";
            }
            else
            {
                Judge();
                ButtonDisplay.Text = "スタート";
                button = true;
            }
        }
    }
}