using HearMeRoar;
using HearMeRoar.Services;
using HearMeRoar.ViewModels;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;

namespace HearMeRoar.ViewModels
{
    public class AltMainViewModel : AltBaseViewModel
    {

        string shareTitle = "Voz";
        string shareFileName = "Voz.png";
        public string Config = string.Empty;

        int salutationOccurence = 15;
        int salutationCount = 100;

        int endOccurence = 3;
        int endCount = 10;

        public AltMainViewModel()
        {

            ShowRunning = true;
            CallBindingStart();
            PageTitle = AppTitle;
            GenerateMsgButtonText = "Continue";
            GenerateMsg = new Command(() => GenerateMsgTxt());
            CreateVoiceMessage = new Command(() => SayMessage());
            CopyText = new Command(() => CopyToClipboard());
            ShareMessageMeme = new Command(() => shareMessageImage());
            ShareMessageText = new Command(() => shareMessageEmail());

            //set default image
            shareBackground = BitmapExtensions.LoadBitmapResource(GetType(), "HearMeRoar.ShareBackground2.jpg");
            
            GetStringPoolData();

            CreateMessageImage(true);

        }


        public async Task SendSms(string messageText, string recipient)
        {
            var message = new SmsMessage(messageText, new[] { recipient });
            await Sms.ComposeAsync(message);
        }

        public ICommand ToggleLanguage => new Command(OnToggleLanguage);
        private void OnToggleLanguage()
        {
            ShowEnglish = !ShowEnglish;
            //FullMessageTextDisplay = FullMessageTextDisplay;

            CallBindingToggle();
        }



        //to get remote data file
        public async void GetStringPoolData()
        {
            //URL for the content or JSON data.
            string myURL = DebugConfigDataLocation;
            string sJSONData = string.Empty;
            //Getting the content from the web
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var content = await client.GetStringAsync(myURL);
                    sJSONData = content;
                    //save data locally 
                    set_post_data(content);
                }
                catch (Exception ex)
                {
                    //unable to retrieve file from internet
                    var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainViewModel)).Assembly;
                    Stream stream = assembly.GetManifestResourceStream(DebugConfigLocalLocation);
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        sJSONData = reader.ReadToEnd();
                    }
                    set_post_data(sJSONData);
                }

                readJSONData(sJSONData);

            }
        }

        private void readJSONData(string JSONData)
        {
            string sLang = "RU";
            string sLang_EN = "EN";
            JObject objData = JObject.Parse(JSONData);

            sSettings = objData["Settings"].ToString();

            //Settings
            try
            {
                NotificationText = GetSettingFromJSON(sSettings, "Disclaimer");
                int.TryParse(GetSettingFromJSON(sSettings, "exclamationOccurence"), out salutationOccurence);
                int.TryParse(GetSettingFromJSON(sSettings, "exclamationCount"), out salutationCount);
                int.TryParse(GetSettingFromJSON(sSettings, "endOccurence"), out endOccurence);
                int.TryParse(GetSettingFromJSON(sSettings, "endCount"), out endCount);

                double.TryParse(GetSettingFromJSON(sSettings, "fontSizeRatio"), out _fontSizeRatio);
                int.TryParse(GetSettingFromJSON(sSettings, "maxfontSize"), out _maxfontSize);
                int.TryParse(GetSettingFromJSON(sSettings, "maxScreenHorizontalReducer"), out _maxScreenHorizontalReducer);
                int.TryParse(GetSettingFromJSON(sSettings, "maxScreenVerticalReducer"), out _maxScreenVerticalReducer);
                int.TryParse(GetSettingFromJSON(sSettings, "midScreenWidth"), out _midScreenWidth);
                int.TryParse(GetSettingFromJSON(sSettings, "midScreenWidthReducer"), out _midScreenWidthReducer);
                int.TryParse(GetSettingFromJSON(sSettings, "minfontSize"), out _minfontSize);
                int.TryParse(GetSettingFromJSON(sSettings, "minScreenHeight"), out _minScreenHeight);
                int.TryParse(GetSettingFromJSON(sSettings, "minScreenHeightReducer"), out _minScreenHeightReducer);


                EmailListAPI = GetSettingFromJSON(sSettings, "emailListAPI");
                GrammarAPI = GetSettingFromJSON(sSettings, "grammarAPI");
                SubjectAPI = GetSettingFromJSON(sSettings, "subjectAPI");

                OnPropertyChanged("ButtonWidth");
                OnPropertyChanged("ButtonHeight");
                OnPropertyChanged("TextFontSize");
            }
            finally
            {
                OnPropertyChanged("ShowRunning");
                OnPropertyChanged("FullMessageTextDisplay");
                OnPropertyChanged("FullMessageText");
            }

        }



        private List<String> ParseDataFromJSON(string sJSON, string FieldName)
        {
            var array = JArray.Parse(sJSON);
            List<String> sRet = new List<string>();

            int i = 0;
            foreach (var item in array)
            {
                try
                {
                    // CorrectElements  
                    sRet.Add(item[FieldName].ToString());
                }
                catch (Exception ex)
                {
                    //do something
                }
                i += 1;
            }
            return sRet;
        }


        private string GetSettingFromJSON(string sJSON, string FieldName)
        {
            var array = JArray.Parse(sJSON);
            string sRet = "";

            int i = 0;
            foreach (var item in array)
            {
                try
                {
                    // CorrectElements  
                    if (item[FieldName] != null)
                    {
                        sRet = item[FieldName].ToString();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    //do something
                }
                i += 1;
            }
                

            return sRet;

        }
                

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
        public async Task<string> GetEmailsAsync()
        {
            string sRet = "503";

            //URL for the content or JSON data.
            string sEmailAPI = EmailListAPI;

            string sJSONData = string.Empty;
            //Getting the content from the web
            var baseAddress = new Uri(sEmailAPI);
            //var cookieContainer = new CookieContainer();
            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient())
            {
                ///****************************************************/
                //var content = new FormUrlEncodedContent(new[]
                //{
                //    new KeyValuePair<string, string>("foo", "bar")
                //});

                //cookieContainer.Add(baseAddress, new Cookie("CookieName", "cookie_value"));
                //try
                //{
                //    var result = await client.GetAsync(baseAddress, HttpCompletionOption.ResponseHeadersRead);
                //    result.EnsureSuccessStatusCode();
                //}
                //catch (Exception ex)
                //{
                //    //error
                //}

                /***************************************************/

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        string sContentData = await client.GetStringAsync(baseAddress);
                        sJSONData = sContentData;
                        break;
                    }
                    catch (Exception ex)
                    {
                        //unable to retrieve file from internet
                        Task.Delay(1000).Wait();
                    }
                }

                if (sJSONData == string.Empty)
                {
                    sRet = "503";
                }
                else
                {
                    sRet = sJSONData;
                }
            }

            return sRet;
        }


        public async Task<string> GetMessagesAsync()
        {
            string sRet = "503";

            //URL for the content or JSON data.
            string sGrammarAPI = GrammarAPI;

            string sJSONData = string.Empty;
            //Getting the content from the web
            var baseAddress = new Uri(sGrammarAPI);
            //var cookieContainer = new CookieContainer();
            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        string sContentData = await client.GetStringAsync(baseAddress);
                        sJSONData = sContentData;
                        break;
                    }
                    catch (Exception ex)
                    {
                        //unable to retrieve file from internet
                        Task.Delay(1000).Wait();
                    }
                }

                if (sJSONData == string.Empty)
                {
                    sRet = "503";
                }
                else
                {
                    sRet = sJSONData;
                }
            }

            return sRet;
        }


        public async Task<string> GetSubjectAsync()
        {
            string sRet = "503";

            //URL for the content or JSON data.
            string sSubjectAPI = SubjectAPI;

            string sJSONData = string.Empty;
            //Getting the content from the web
            var baseAddress = new Uri(sSubjectAPI);
            //var cookieContainer = new CookieContainer();
            //using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        string sContentData = await client.GetStringAsync(baseAddress);
                        sJSONData = sContentData;
                        break;
                    }
                    catch (Exception ex)
                    {
                        //unable to retrieve file from internet
                        Task.Delay(1000).Wait();
                    }
                }

                if (sJSONData == string.Empty)
                {
                    sRet = "503";
                }
                else
                {
                    sRet = sJSONData;
                }
            }

            return sRet;
        }

        //save the data locally
        public void set_post_data(string json)
        {
            //assigning Application.Current.Properties data 
            Application.Current.Properties["hear_me_roar_data"] = json;
        }

        public string get_post_data()
        {
            return Application.Current.Properties["hear_me_roar_data"].ToString();
        }


        public ICommand CopyText { private set; get; }
        public ICommand CreateVoiceMessage { private set; get; }
        public ICommand GenerateMsg { private set; get; }
        public ICommand ShareMessageMeme { private set; get; }
        public ICommand ShareMessageText { private set; get; }
        public interface IGetFileStream
        {
            MemoryStream getStream();
        }

        async void GenerateMsgTxt()
        {
            NotificationColor = "Black";
            GenerateMsgButtonText = "Generate New Message";

            getNewMessage();

            int iLen = FullMessageText.Length;
            if(iLen > 950)
            {
                while(iLen > 950)
                {
                    getNewMessage();
                    iLen = FullMessageText.Length;
                }
            }

            getNewSubject();

            iLen = FullSubjectText.Length;
            if (iLen > 950)
            {
                while (iLen > 950)
                {
                    getNewSubject();
                    iLen = FullSubjectText.Length;
                }
            }

            Thread.Sleep(100);

            if (!ShowShare)
            {
                ShowShare = true;
            }
            CallBindingComplete();
        }



        public virtual async void SayMessage()
        {
            NotificationColor = "Black";
            string sMsg = FullMessageText;
            SayStuffAsync((sMsg));
        }


        public virtual async void CopyToClipboard()
        {
            NotificationColor = "Black";
            string sMsg = FullMessageText;
            await Clipboard.SetTextAsync((sMsg));
        }

        SKBitmap shareBackground;
        public virtual async void shareMessageImage()
        {
            NotificationColor = "Black";
            //do the actual sharing
            string sharefile = ShareFileName;

            try
            {
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = shareTitle,
                    File = new ShareFile(sharefile),
                    PresentationSourceBounds = new System.Drawing.Rectangle(0, 0, 1, 1)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public virtual async void getNewMessage()
        {
            NotificationColor = "Black";
            ShowRunning = true;
            CallBindingStart();
            try
            {
                string sMessage = await GetMessagesAsync();

                if (sMessage != "503")
                {
                    JObject o = JObject.Parse(sMessage);

                    //string s = o["grammar"].ToString();
                    //JObject x = JObject.Parse(s);

                    FullMessageText_EN = o["grammar"][1].ToString();
                    FullMessageText = o["grammar"][2].ToString();
                    NotificationText = String.Empty;
                    CreateMessageImage();
                }
                else
                {
                    NotificationText = "Server Busy";
                    NotificationColor = "DarkRed";
                    OnPropertyChanged("TextFontSize");
                    OnPropertyChanged("NotificationColor");
                }
            }
            catch (Exception ex)
            {
                // Some other exception occurred  
            }
        }

        public virtual async void getNewSubject()
        {
            NotificationColor = "Black";
            ShowRunning = true;
            CallBindingStart();
            try
            {
                string sMessage = await GetSubjectAsync();

                if (sMessage != "503")
                {
                    JObject o = JObject.Parse(sMessage);

                    string s = o["grammar"].ToString();

                    FullSubjectText_EN = s[0].ToString();
                    FullSubjectText = s[1].ToString();
                    NotificationText = String.Empty;
                }
                else
                {
                    NotificationText = "Server Busy";
                    NotificationColor = "DarkRed";
                    OnPropertyChanged("TextFontSize");
                    OnPropertyChanged("NotificationColor");
                }
            }
            catch (Exception ex)
            {
                // Some other exception occurred  
            }
        }


        public virtual async void shareMessageEmail()
        {
            NotificationColor = "Black";
            ShowRunning = true;
            CallBindingStart();
            try
            {
                string sEmails = await GetEmailsAsync();

                if (sEmails != "503")
                {
                    lstEmails = new List<string>();

                    JObject o = JObject.Parse(sEmails);

                    var sO = o["emails"];

                    foreach (var item in sO)
                    {
                        lstEmails.Add(item.ToString());
                    }

                    var message = new EmailMessage
                    {
                        Subject = FullSubjectTextDisplay,
                        Body = FullMessageTextDisplay,
                        Bcc = lstEmails
                    };
                    await Email.ComposeAsync(message);
                }
                else
                {
                    NotificationText = "Server Busy";
                    NotificationColor = "DarkRed";
                    OnPropertyChanged("TextFontSize");
                    OnPropertyChanged("NotificationColor");
                }
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device  
                NotificationText = "Email not supported/enabled on this device for this app";
                NotificationColor = "DarkRed";
            }
            catch (Exception ex)
            {
                // Some other exception occurred  
            }
        }

        public void CreateMessageImage(bool loadEmpty = false)
        {
            string sFullMsgText = "";
            if (loadEmpty == false)
            {
                sFullMsgText = FullMessageText;
            }
            int memeWidth = 600;
            int memeHeight = 650;
            int footerBuffer = 30;
            int insultHeight = 0;
            float textSize = 25;
            float textFooterSize = 15;
            int insultBuffer = 10;
            int rowMaxWidth = 590;

            //clean up extra spaces
            sFullMsgText = sFullMsgText.Replace("   ", " ").Replace("  ", " ").Replace(" .", ".");


            //update shell menu
            AppShell AppShellInstance = Xamarin.Forms.Shell.Current as AppShell;

            //make an image/meme out of the generated insult and an image for the background
            shareBackground = BitmapExtensions.LoadBitmapResource(GetType(), "HearMeRoar.ShareBackground2.jpg");


            // Create full-sized bitmap for copy
            ShareBitmap = new SKBitmap(shareBackground.Width, shareBackground.Height);

            using (SKCanvas canvas = new SKCanvas(ShareBitmap))
            {
                canvas.Clear();

                // Draw background image in full size
                canvas.DrawBitmap(shareBackground, new SKPoint());

                // Create an SKPaint object to display the text
                SKPaint textPaint = new SKPaint
                {
                    Style = SKPaintStyle.StrokeAndFill,
                    StrokeWidth = 1,
                    FakeBoldText = false,
                    Color = SKColors.White,
                    TextSize = textSize
                };

                // Create an SKPaint object to display the text for image footer
                SKPaint textPaintFooter = new SKPaint
                {
                    Style = SKPaintStyle.StrokeAndFill,
                    StrokeWidth = 1,
                    FakeBoldText = false,
                    Color = SKColors.Silver,
                    TextSize = textFooterSize,
                    Typeface = SKTypeface.FromFamilyName("sans-serif-thin")
                };

                //use custom font
                SKTypeface typeface;
                {
                    typeface = SKTypeface.FromFamilyName("sans-serif-thin");
                }

                textPaint.Typeface = typeface;

                // Find the text bounds
                SKRect textBounds = new SKRect();

                //Check width of insult text for word wrap
                float textWidth = textPaint.MeasureText(sFullMsgText);
                float maxTextWidth = textPaint.MeasureText(sFullMsgText);

                List<string> wrappedTextLines = new List<string>();
                sFullMsgText = ToUpperFirstLetter(sFullMsgText);
                wrappedTextLines = WrapText(sFullMsgText, textPaint, (rowMaxWidth - insultBuffer));

                //cycle through each line of wrapped text and determine widest line of text in order to center
                int numLines = wrappedTextLines.Count;
                foreach (var item in wrappedTextLines)
                {
                    float lineWidth = textPaint.MeasureText(item.Trim());
                    if (lineWidth > maxTextWidth)
                    {
                        maxTextWidth = lineWidth;
                    }
                }


                insultHeight = (int)(numLines * textSize);

                //again cycle through, but this time write it out with the info we now have
                int lineNumber = 1;
                foreach (var item in wrappedTextLines)
                {
                    string lineText = item.Trim();
                    //set default text size
                    textPaint.TextSize = textSize;
                    float lineWidth = textPaint.MeasureText(lineText.Trim());
                    //if width of line is too long work down the text size until it fits the width
                    textPaint.MeasureText(item, ref textBounds);
                    while (textBounds.Width > memeWidth - insultBuffer)
                    {
                        textPaint.TextSize = textPaint.TextSize - 1;
                        textPaint.MeasureText(item, ref textBounds);
                    }

                    //Check dimensions of final insult text for centering
                    textPaint.MeasureText(item, ref textBounds);
                    // Calculate offsets to center the text on the screen
                    float xText = 10;//memeWidth / 2 - textBounds.MidX;
                    float yText = memeHeight - ((memeHeight) / 2) - (insultHeight / 2) + (lineNumber + textSize) - footerBuffer;

                    //line buffer
                    yText = yText + ((lineNumber - 1) * textSize);
                    //yText = yText + 50;


                    // And draw the text
                    canvas.DrawText(lineText, xText, yText, textPaint);
                    lineNumber += 1;
                }

                //draw footer
                string footerText = "We are legion. We do not forgive. We do not Forget. Expect us. #OpRussia";//"STOP ALL WARS";

                //Check dimensions of final insult text for centering
                textPaintFooter.MeasureText(footerText, ref textBounds);

                //float footerWidth = textPaintFooter.MeasureText(footerText.Trim());
                // Calculate offsets to center the text on the screen
                float xFooter = memeWidth / 2 - (textBounds.MidX);
                float yFooter = memeHeight - footerBuffer;
                canvas.DrawText(footerText, xFooter, yFooter, textPaintFooter);
            }

            // Create SKCanvasView to view result
            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;


            //crop here




            //save bitmap to file
            // create an image and then get the PNG (or any other) encoded data
            using (var image = SKImage.FromBitmap(ShareBitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            { // save the data to a stream
                using (Stream fileStream = File.Open(System.IO.Path.Combine(FileSystem.AppDataDirectory, "x" + shareFileName), FileMode.Create))
                {
                    data.SaveTo(fileStream);    //fileStream.Write(Contents, 0, Contents.Length);
                }
            }



            MemoryStream myMemoryStream = GetStream(ShareBitmap);
            Stream myStream = myMemoryStream;

            //ImageSource.FromStream(() => stream2)
            Image bitmap = new Image { Source = ImageSource.FromStream(() => myStream) };
            AppBitmap = bitmap.Source;

            ShareFileName = System.IO.Path.Combine(FileSystem.AppDataDirectory, "x" + shareFileName);


            FullMessageText = sFullMsgText;
            OnPropertyChanged("FullMessageTextDisplay");
        }


        public static string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }

        static List<string> WrapText(string text, SKPaint textPaint, float imgWidth)
        {
            string[] originalLines = text.Split(new string[] { " " }, StringSplitOptions.None);

            List<string> wrappedLines = new List<string>();

            System.Text.StringBuilder actualLine = new System.Text.StringBuilder();
            System.Text.StringBuilder actualLineTemp = new System.Text.StringBuilder();
            System.Text.StringBuilder tempLine = new System.Text.StringBuilder();

            double actualWidth = 0;

            foreach (var item in originalLines)
            {
                float wordLength = textPaint.MeasureText(item.Trim());

                actualLineTemp.Append(item.Trim() + " ");
                actualWidth = textPaint.MeasureText(actualLineTemp.ToString().Trim());

                if (actualWidth > imgWidth)
                {
                    wrappedLines.Add(tempLine.ToString().Trim());
                    actualLine.Clear();

                    actualLineTemp.Clear();
                    actualLineTemp.Append(item.Trim() + " ");
                    actualWidth = 0;

                    actualLine.Append(item.Trim() + " ");
                }
                else
                {
                    actualLine.Append(item.Trim() + " ");
                    tempLine = actualLine;
                }
            }

            if (actualLine.Length > 0)
                wrappedLines.Add(actualLine.ToString().Trim());

            return wrappedLines;
        }


        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            canvas.DrawBitmap(ShareBitmap, info.Rect);//BitmapStretch.Uniform);
        }

        private static string GetRandomEntry(Random rnd, List<String> lstDataPool)
        {
            return lstDataPool[rnd.Next(0, lstDataPool.Count - 1)];
        }

        public ICommand OpenWebCommand { get; }

        public MemoryStream GetStream(SKBitmap bitmap)
        {
            MemoryStream x = new MemoryStream();
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            { // save the data to a stream
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);
                    x = stream;
                }
            }

            return x;
        }
    }


    public abstract class AltViewModelBase : ObservableProperty
    {
        public Dictionary<string, ICommand> Commands { get; protected set; }

        public AltViewModelBase()
        {
            Commands = new Dictionary<string, ICommand>();
        }

        public static byte[] GetBytes(string url)
        {
            byte[] arry;
            using (var webClient = new WebClient())
            {
                arry = webClient.DownloadData(url);
            }
            return arry;
        }
    }
    
}