using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HearMeRoar.ViewModels
{
    public class AltBaseViewModel : ContentPage, INotifyPropertyChanged
    {
        //private string myAdID = "ca-app-pub-2977343587298168/6549534277";
        //private string myAdID = "ca-app-pub-2977343587298168/1024026224";
        //private string myAdID = "ca-app-pub-2977343587298168/2189295473";
        //private string demoAdID = "ca-app-pub-3940256099942544/6300978111";

        string debugConfigDataLocation = "http://incrediblegeeks.com/uploads/CampaignMsg.txt";
        public string DebugConfigDataLocation { get => debugConfigDataLocation; }

        string debugConfigLocalLocation = "HearMeRoar.CampaignMsg.txt";
        public string DebugConfigLocalLocation { get => debugConfigLocalLocation; }


        public async Task SayStuffAsync(string Message)
        {
            CrossLocale crossLocale;

            Random rnd = new Random();

            int r = rnd.Next(0, 1);
            //randomize accent
            switch (r)
            {
                case 0:
                    crossLocale.Country = "RU";
                    crossLocale.Language = "ru-RU-Standard-A";
                    break;
                default:
                    crossLocale.Country = "RU";
                    crossLocale.Language = "ru-RU-Standard-B";
                    break;
            }


            await CrossTextToSpeech.Current.Speak(Message.Replace(".", "'.\r\n"), crossLocale, 1f, 1f);
        }

        public double ScreenWidth
        {
            get
            {
                //to be used for both height and width as the main image is square (600x600)
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                double dRatio = 600 / mainDisplayInfo.Width;
                double iWid = mainDisplayInfo.Width * dRatio;

                if (mainDisplayInfo.Width <= 1200)
                {
                    return ((DeviceDisplay.MainDisplayInfo.Height / 2) * .72) - 30;  //30 pixel buffer and 72dpi
                }
                else
                {

                    if (mainDisplayInfo.Width > mainDisplayInfo.Height)
                    {
                        return ((DeviceDisplay.MainDisplayInfo.Width / 2) * .72) - 30;
                    }
                    else
                    {
                        return ((DeviceDisplay.MainDisplayInfo.Height / 2) * .72) - 130;  //100 pixel buffer and 72dpi
                    }
                }

            }
        }

        public int _minScreenHeight = 800;
        public int _minScreenHeightReducer = 80;
        public int _midScreenWidth = 1200;
        public int _midScreenWidthReducer = 30;
        public int _maxScreenHorizontalReducer = 130;
        public int _maxScreenVerticalReducer = 300;
        public double _fontSizeRatio = .012;
        public int _minfontSize = 14;
        public int _maxfontSize = 40;
        public double ImageWidth
        {
            get
            {
                //to be used for both height and width as the main image is square (600x600)
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                double dRatio = 600 / mainDisplayInfo.Width;
                double iWid = mainDisplayInfo.Width * dRatio;

                if (mainDisplayInfo.Height <= _minScreenHeight)
                {
                    return ((DeviceDisplay.MainDisplayInfo.Width) * .72) - _minScreenHeightReducer;  //30 pixel buffer and 72dpi
                }
                else if (mainDisplayInfo.Width <= _midScreenWidth)
                {
                    double s = ((DeviceDisplay.MainDisplayInfo.Width / 2) * .72) - _midScreenWidthReducer;  //30 pixel buffer and 72dpi
                    if (s > DeviceDisplay.MainDisplayInfo.Width) { s = DeviceDisplay.MainDisplayInfo.Width - _midScreenWidthReducer; }
                    return s;
                }
                else
                {
                    if (mainDisplayInfo.Width > mainDisplayInfo.Height)
                    {
                        return ((DeviceDisplay.MainDisplayInfo.Height / 2) * .72) - _maxScreenHorizontalReducer;
                    }
                    else
                    {
                        return ((DeviceDisplay.MainDisplayInfo.Height / 2) * .72) - _maxScreenHorizontalReducer;  //100 pixel buffer and 72dpi
                    }
                }

            }
        }

        public double ImageHeight
        {
            get
            {
                //to be used for both height and width as the main image is square (600x600)
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                double dRatio = 600 / mainDisplayInfo.Width;
                double iWid = mainDisplayInfo.Width * dRatio;

                if (mainDisplayInfo.Height <= _minScreenHeight)
                {
                    return ((DeviceDisplay.MainDisplayInfo.Width) * .72) - _minScreenHeightReducer;  //30 pixel buffer and 72dpi
                }
                else if (mainDisplayInfo.Width <= _midScreenWidth)
                {
                    double s = ((DeviceDisplay.MainDisplayInfo.Width / 2) * .72) - _midScreenWidthReducer;  //30 pixel buffer and 72dpi
                    if (s > DeviceDisplay.MainDisplayInfo.Width) { s = DeviceDisplay.MainDisplayInfo.Width - _midScreenWidthReducer; }
                    return s;
                }
                else
                {
                    if (mainDisplayInfo.Width > mainDisplayInfo.Height)
                    {
                        return ((DeviceDisplay.MainDisplayInfo.Height / 2) * .72) - _maxScreenHorizontalReducer;
                    }
                    else
                    {
                        return ((DeviceDisplay.MainDisplayInfo.Height / 2) * .72) - _maxScreenHorizontalReducer;  //100 pixel buffer and 72dpi
                    }
                }

            }
        }

        public double ButtonWidth
        {
            get
            {
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

                if (mainDisplayInfo.Width > mainDisplayInfo.Height)
                {
                    return ((DeviceDisplay.MainDisplayInfo.Width / 2) * .72) - _midScreenWidthReducer;  //30 pixel buffer and 72dpi
                }
                else
                {
                    return ((DeviceDisplay.MainDisplayInfo.Width) * .72) - _midScreenWidthReducer;  //30 pixel buffer and 72dpi
                }
            }
        }

        public double ButtonFontSize
        {
            get
            {
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

                double sz = mainDisplayInfo.Height * .005;
                if (sz < 14) { sz = 14; }
                if (sz > 24) { sz = 24; }

                return (sz);

            }
        }

        public double ButtonHeight
        {
            get
            {
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

                double sz = mainDisplayInfo.Height * .012;
                if (sz < 36) { sz = 36; }
                if (sz > 80) { sz = 80; }


                return (sz);

            }
        }


        public double TextFontSize
        {
            get
            {
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

                double sz = mainDisplayInfo.Height * _fontSizeRatio;
                if (sz < _minfontSize) { sz = _minfontSize; }
                if (sz > _maxfontSize) { sz = _maxfontSize; }

                return (sz);
            }
        }




        bool showShare = false;
        public bool ShowShare
        {
            get
            {
                if (ShowRunning) { return false; }
                else { return showShare; }
            }
            set
            {
                SetProperty(ref showShare, value);
                CallBindingToggle();
            }
        }

        bool showRunning = false;
        public bool ShowRunning
        {
            get { return showRunning; }
            set
            {
                SetProperty(ref showRunning, value);
                if (!showRunning) { ShowShare = showShare; }
                CallBindingToggle();
            }
        }


        public bool IsDisabled
        {
            get { return !showRunning; }
        }



        string pageTitle = string.Empty;
        public string PageTitle
        {
            get { return pageTitle; }
            set { SetProperty(ref pageTitle, value); }
        }

        string sNotificationMsg = string.Empty;
        public string NotificationText
        {
            get { return sNotificationMsg; }
            set
            {
                SetProperty(ref sNotificationMsg, value);
                OnPropertyChanged("NotificationTextDisplay");
            }
        }

        string sNotificationColor = "Black";
        public string NotificationColor
        {
            get { return sNotificationColor; }
            set
            {
                SetProperty(ref sNotificationColor, value);
                OnPropertyChanged("NotificationColor");
            }
        }

        string generateMsgButtonText = string.Empty;
        public string GenerateMsgButtonText
        {
            get { return generateMsgButtonText; }
            set { SetProperty(ref generateMsgButtonText, value); }
        }

        string shareOriginalFileName = "ShareBackground2.jpg";
        public string ShareOriginalFileName
        {
            get { return shareOriginalFileName; }
        }

        string shareFileName = "ShareBackground2.jpg";
        public string ShareFileName
        {
            get { return shareFileName; }
            set
            {
                SetProperty(ref shareFileName, value);
                OnPropertyChanged("ImageWidth");
            }
        }

        SKBitmap shareBitmap;
        public SKBitmap ShareBitmap
        {
            get { return shareBitmap; }
            set { SetProperty(ref shareBitmap, value); }
        }


        ImageSource appBitmap;
        public ImageSource AppBitmap
        {
            get { return appBitmap; }
            set { SetProperty(ref appBitmap, value); }
        }


        string appTitle = string.Empty;
        public string AppTitle
        {
            get { return appTitle; }
            set { SetProperty(ref appTitle, value); }
        }


        string emailListAPI = string.Empty;
        public string EmailListAPI
        {
            get { return emailListAPI; }
            set { SetProperty(ref emailListAPI, value); }
        }

        string grammarAPI = string.Empty;
        public string GrammarAPI
        {
            get { return grammarAPI; }
            set { SetProperty(ref grammarAPI, value); }
        }


        string subjectAPI = string.Empty;
        public string SubjectAPI
        {
            get { return subjectAPI; }
            set { SetProperty(ref subjectAPI, value); }
        }


        WebView testWebSource;
        public WebView TestWebSource
        {
            get { return testWebSource; }
            set { SetProperty(ref testWebSource, value); }
        }


        public string SendEmailButtonText
        {
            get
            {
                return "Send as Email";
            }
        }



        public string sSettings;




        string sFullMessageText = string.Empty;
        public string FullMessageText
        {
            get
            {
                return sFullMessageText;
            }
            set
            {
                SetProperty(ref sFullMessageText, value);

                ShowRunning = false;
            }
        }

        string sFullMessageText_EN = string.Empty;
        public string FullMessageText_EN
        {
            get
            {
                return sFullMessageText_EN.ToUpper();
            }
            set
            {
                SetProperty(ref sFullMessageText_EN, value);
            }
        }



        string sFullSubjectText = string.Empty;
        public string FullSubjectText
        {
            get
            {
                return sFullSubjectText;
            }
            set
            {
                SetProperty(ref sFullSubjectText, value);
            }
        }

        string sFullSubjectText_EN = string.Empty;
        public string FullSubjectText_EN
        {
            get
            {
                return sFullSubjectText_EN.ToUpper();
            }
            set
            {
                SetProperty(ref sFullSubjectText_EN, value);
            }
        }


        public string FullMessageTextDisplay
        {
            get
            {
                if (showEnglish)
                {
                    return sFullMessageText.Replace("[", "").Replace("]", "");
                }
                else
                {
                    return sFullMessageText_EN.Replace("[", "").Replace("]", "");
                }
            }
        }


        public string FullMessageTextDisplay_EN
        {
            get
            {
                return sFullMessageText_EN.ToUpper().Replace("[", "").Replace("]", "");
            }
        }




        public string FullSubjectTextDisplay
        {
            get
            {
                return sFullSubjectText.Replace("[", "").Replace("]", "");
            }
        }


        public string FullSubjectTextDisplay_EN
        {
            get
            {
                return sFullSubjectText_EN.ToUpper().Replace("[", "").Replace("]", "");
            }
        }

        public string NotificationTextDisplay
        {
            get
            {
                return sNotificationMsg;
            }
        }



        string msgTranslationName;
        public string MsgTranslationName
        {
            get { return msgTranslationName; }
            set { SetProperty(ref msgTranslationName, value); }
        }

        bool showEnglish = false;
        public bool ShowEnglish
        {
            get
            {
                return showEnglish;
            }
            set
            {
                SetProperty(ref showEnglish, value);
                TranslationText = TranslationText;
                CallBindingToggle();
            }
        }

        public void CallBindingToggle()
        {
            OnPropertyChanged("FullSubjectText");
            OnPropertyChanged("FullSubjectTextDisplay");
            OnPropertyChanged("FullSubjectTextDisplay_EN");
            OnPropertyChanged("FullSubjectText_EN");
            OnPropertyChanged("FullMessageTextDisplay");
            OnPropertyChanged("TranslationText");
            OnPropertyChanged("FullMessageText");
            OnPropertyChanged("IsDisabled");
            OnPropertyChanged("ShowShare");
            OnPropertyChanged("ShowEnglish");
            OnPropertyChanged("ShowRunning");
            OnPropertyChanged("TranslationText");
        }

        public void CallBindingComplete()
        {
            ShowRunning = false;
            OnPropertyChanged("FullSubjectText");
            OnPropertyChanged("FullSubjectTextDisplay");
            OnPropertyChanged("FullSubjectTextDisplay_EN");
            OnPropertyChanged("FullSubjectText_EN");
            OnPropertyChanged("FullMessageTextDisplay");
            OnPropertyChanged("TranslationText");
            OnPropertyChanged("FullMessageText");
            OnPropertyChanged("IsDisabled");
            OnPropertyChanged("ShowShare");
            OnPropertyChanged("ShowEnglish");
            OnPropertyChanged("ShowRunning");
            OnPropertyChanged("TranslationText");
        }

        public void CallBindingStart()
        {
            ShowRunning = true;
            OnPropertyChanged("FullSubjectText");
            OnPropertyChanged("FullSubjectTextDisplay");
            OnPropertyChanged("FullSubjectTextDisplay_EN");
            OnPropertyChanged("FullSubjectText_EN");
            OnPropertyChanged("FullMessageTextDisplay");
            OnPropertyChanged("TranslationText");
            OnPropertyChanged("FullMessageText");
            OnPropertyChanged("IsDisabled");
            OnPropertyChanged("ShowShare");
            OnPropertyChanged("ShowEnglish");
            OnPropertyChanged("ShowRunning");
            OnPropertyChanged("TranslationText");
        }

        string translationText = "English";
        public string TranslationText
        {
            get { return translationText; }
            set
            {
                if (showEnglish != true)
                {
                    translationText = "English";
                }
                else
                {
                    translationText = "Russian";
                }
                SetProperty(ref translationText, translationText);
            }
        }

        public List<String> lstEmails;


        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected bool SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (value == null ? field != null : !value.Equals(field))
            {
                field = value;

                var handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }
    }
}
