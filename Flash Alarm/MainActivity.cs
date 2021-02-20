using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Plugin.FilePicker;
using System.IO;
using System;
using Android.Util;
using Android.Text.Format;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;

namespace Flash_Alarm
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected MediaPlayer player;

        public TextView filename { get; private set; }
        public TextView timeDisplay { get; private set; }
        public TextView Alarm { get; private set; }
        public string filepath;
        public DateTime alarmtime;
        public bool aswitch = false;
        public bool timeswitch = false;
        public bool fileswitch = false;

        public async void timer()
        {
            bool a = true;
            while (a)
            {
                if (aswitch)
                {
                    if (alarmtime.ToShortTimeString() == DateTime.Now.ToShortTimeString())
                    {
                        var intent = new Intent(this, typeof(secound));
                        intent.PutExtra("Message", this.filepath);
                        StartActivity(intent);
                        a = false;
                    }
                }
                await Task.Delay(1000);
            }
        }

        private void AlarmSwitch()
        {
            Button AlarmSwitch = FindViewById<Button>(Resource.Id.@alarmswitch);
            if (timeswitch && fileswitch)
            {
                AlarmSwitch.Enabled = true;
                aswitch = !aswitch;
                Alarm.Text = aswitch ? "ON" : "OFF";
            }
            else
            {
                AlarmSwitch.Enabled = false;
                aswitch = false;
                Alarm.Text = "OFF";
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            timeDisplay = FindViewById<TextView>(Resource.Id.time);
            filename = FindViewById<TextView>(Resource.Id.filename);
            Alarm = FindViewById<Button>(Resource.Id.alarmswitch);

            Button TimeSet = FindViewById<Button>(Resource.Id.timebutton);
            TimeSet.Click += TimeSet_Click;

            Button FileSet = FindViewById<Button>(Resource.Id.filebutton);
            FileSet.Click += FileSet_Click;

            Button Alarmswitch = FindViewById<Button>(Resource.Id.@alarmswitch);
            Alarmswitch.Click += AlarmSwitch_Click;

            timer();
            AlarmSwitch();
        }

        private void AlarmSwitch_Click(object sender, EventArgs e)
        {
            AlarmSwitch();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
        {
            public static readonly string TAG = "MyTimePickerFragment";
            Action<DateTime> timeSelectedHandler = delegate { };

            public static TimePickerFragment NewInstance(Action<DateTime> onTimeSelected)
            {
                TimePickerFragment frag = new TimePickerFragment();
                frag.timeSelectedHandler = onTimeSelected;
                return frag;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                DateTime currentTime = DateTime.Now;
                bool is24HourFormat = DateFormat.Is24HourFormat(Activity);
                TimePickerDialog dialog = new TimePickerDialog
                    (Activity, this, currentTime.Hour, currentTime.Minute, is24HourFormat);
                return dialog;
            }

            public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
            {
                DateTime currentTime = DateTime.Now;
                DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, 0);
                Log.Debug(TAG, selectedTime.ToLongTimeString());
                timeSelectedHandler(selectedTime);
            }
        }

        private async void FileSet_Click(object sender, EventArgs e)
        {
            bool x;
            var file = await CrossFilePicker.Current.PickFile();
            try
            {
                player = new MediaPlayer();
                player.SetDataSource(file.FilePath);
                player.Prepare();
                player.Start();
                x = true;
            }
            catch (Exception)
            {
                x = false;
                filename.Text = "このファイルは再生できません";
                fileswitch = false;
                AlarmSwitch();
            }
            player.Reset();
            if (x)
            {
                filepath = file.FilePath;
                filename.Text = file.FileName;
                fileswitch = true;
                if (!aswitch)
                {
                    AlarmSwitch();
                }
            }
        }

        private void TimeSet_Click(object sender, EventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(
                delegate (DateTime times)
            {
                timeDisplay.Text = times.ToShortTimeString();
                alarmtime = times;
                timeswitch = true;
            });
            frag.Show(FragmentManager, TimePickerFragment.TAG);
            if (!aswitch)
            {
                AlarmSwitch();
            }
        }
    }
}