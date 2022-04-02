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
            }
        }

        bool showRunning = false;
        public bool ShowRunning
        {
            get { return showRunning; }
            set
            {
                SetProperty(ref showRunning, value);
                OnPropertyChanged("ShowShare");
            }
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



        public string sSalutePool;
        public string sSubjPool;
        public string sBodyPool;
        public string sClosePool;
        public string sSignPool;
        public string sThesPool;
        public string sSettings;

        FormattedString messageSalutation;
        public string MessageSalutation_EN;
        public FormattedString MessageSalutation
        {
            get { return messageSalutation; }
            set
            {
                SetProperty(ref messageSalutation, value);
                OnPropertyChanged("SendEmailButtonText");
            }
        }

        FormattedString fsBody1;
        public string Body1_EN;
        public FormattedString Body1
        {
            get { return fsBody1; }
            set { SetProperty(ref fsBody1, value); }
        }

        FormattedString fsEmailSubjectText;
        public string EmailSubjectText_EN;
        public FormattedString EmailSubjectText
        {
            get { return fsEmailSubjectText; }
            set { SetProperty(ref fsEmailSubjectText, value); }
        }

        FormattedString fsMsgCloseText;
        public string MsgCloseText_EN;
        public FormattedString MsgCloseText
        {
            get { return fsMsgCloseText; }
            set { SetProperty(ref fsMsgCloseText, value); }
        }

        FormattedString fsMsgSignText;
        public string MsgSignText_EN;
        public FormattedString MsgSignText
        {
            get { return fsMsgSignText; }
            set { SetProperty(ref fsMsgSignText, value); }
        }



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

                OnPropertyChanged("FullMessageTextDisplay");
                OnPropertyChanged("FullMessageText");
            }
        }

        string sFullMessageText_EN = string.Empty;
        public string FullMessageText_EN
        {
            get
            {
                return sFullMessageText_EN;
            }
            set
            {
                SetProperty(ref sFullMessageText_EN, value);

                OnPropertyChanged("FullMessageTextDisplay_EN");
                OnPropertyChanged("FullMessageText_EN");
            }
        }


        public string FullMessageTextDisplay
        {
            get
            {
                return sFullMessageText.Replace("[", "").Replace("]", "");
            }
        }


        public string FullMessageTextDisplay_EN
        {
            get
            {
                return sFullMessageText_EN.Replace("[", "").Replace("]", "");
            }
        }

        public string NotificationTextDisplay
        {
            get
            {
                return sNotificationMsg;
            }
        }


        public string Thesaurusify(string input)
        {
            string output = input;

            try
            {
                if (ThesaurusEntries != null)
                {
                    if (ThesaurusEntries.InsultFilterItems.Count > 0)
                    {
                        foreach (SlangFilter.SlangFilterItem item in ThesaurusEntries.InsultFilterItems)
                        {
                            try
                            {
                                output = output.Replace(item.WordKey, item.WordReplace);
                            }
                            catch (Exception ex)
                            {
                                //do something
                            }
                        }
                    }
                }
            }
            finally { }

            return output;
        }

        string msgTranslationName;
        public string MsgTranslationName
        {
            get { return msgTranslationName; }
            set { SetProperty(ref msgTranslationName, value); }
        }


        public List<String> lstEmails;

        public List<String> lstSalutePool;

        public List<String> lstSubjPool;

        public List<String> lstBodyPool;

        public List<String> lstClosePool;

        public List<String> lstSignPool;


        public List<String> lstSalutePool_EN;

        public List<String> lstSubjPool_EN;

        public List<String> lstBodyPool_EN;

        public List<String> lstClosePool_EN;

        public List<String> lstSignPool_EN;



        public SlangFilter ThesaurusEntries;
        public SlangFilter ThesaurusEntries_EN;
        public class SlangFilter
        {
            public List<SlangFilterItem> InsultFilterItems = new List<SlangFilterItem>();

            public class SlangFilterItem
            {
                public SlangFilterItem(string WordKey, string WordReplace)
                {
                    wordKey = WordKey;
                    wordReplace = WordReplace;
                }

                string wordKey = string.Empty;
                public string WordKey
                {
                    get { return wordKey; }
                    set { wordKey = value; }
                }

                string wordReplace = string.Empty;
                public string WordReplace
                {
                    get { return wordReplace; }
                    set { wordReplace = value; }
                }

            }
        }

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
