using HearMeRoar.Services;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;

namespace HearMeRoar.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        string shareTitle = "Voz";
        string shareFileName = "Voz.png";
        public string Config = string.Empty;

        int salutationOccurence = 15;
        int salutationCount = 100;

        int endOccurence = 3;
        int endCount = 10;

        public MainViewModel()
        {

            ShowRunning = true;
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

            ShowRunning = false;
        }


        public async Task SendSms(string messageText, string recipient)
        {
            var message = new SmsMessage(messageText, new[] { recipient });
            await Sms.ComposeAsync(message);
        }


        private void readJSONData(string JSONData)
        {
            string sLang = "RU";
            string sLang_EN = "EN";
            JObject objData = JObject.Parse(JSONData);

            sSalutePool = objData["SalutationPool"].ToString();
            sSubjPool = objData["SubjectPool"].ToString();
            sBodyPool = objData["BodyPool"].ToString();
            sClosePool = objData["ClosingPool"].ToString();
            sSignPool = objData["SignaturPool"].ToString();
            sThesPool = objData["ThesaurusPool"].ToString();
            sSettings = objData["Settings"].ToString();

            lstSalutePool = ParseDataFromJSON(sSalutePool, sLang);
            lstSubjPool = ParseDataFromJSON(sSubjPool, sLang);
            lstBodyPool = ParseDataFromJSON(sBodyPool, sLang);
            lstClosePool = ParseDataFromJSON(sClosePool, sLang);
            lstSignPool = ParseDataFromJSON(sSignPool, sLang);



            lstSalutePool_EN = ParseDataFromJSON(sSalutePool, sLang_EN);
            lstSubjPool_EN = ParseDataFromJSON(sSubjPool, sLang_EN);
            lstBodyPool_EN = ParseDataFromJSON(sBodyPool, sLang_EN);
            lstClosePool_EN = ParseDataFromJSON(sClosePool, sLang_EN);
            lstSignPool_EN = ParseDataFromJSON(sSignPool, sLang_EN);

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

                OnPropertyChanged("ButtonWidth");
                OnPropertyChanged("ButtonHeight");
                OnPropertyChanged("TextFontSize");
            }
            finally { }


            ThesaurusEntries = new SlangFilter();
            ThesaurusEntries = ParseSlangFilterJSON(sThesPool, sLang + "_Key", sLang + "_Replace", ThesaurusEntries);


            ThesaurusEntries_EN = new SlangFilter();
            ThesaurusEntries_EN = ParseSlangFilterJSON(sThesPool, sLang_EN + "_Key", sLang_EN + "_Replace", ThesaurusEntries);
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
                    sRet = item[FieldName].ToString();
                }
                catch (Exception ex)
                {
                    //do something
                }
                i += 1;
            }

            return sRet;

        }

        private string GetFullMsgText_EN(string sJSON, string sEntry, string sLangTypeIn, string sLangTypeOut)
        {
            string sRet = "";

            var array = JArray.Parse(sJSON);
            foreach (var item in array)
            {
                try
                {
                    string s = item[sLangTypeIn].ToString();
                    if (s.Trim() == sEntry.Trim())
                    {
                        sRet = item[sLangTypeOut].ToString();
                        break;
                    }
                }
                catch (Exception e)
                {
                    //do something or do nothing
                }
                finally
                {
                }
            }

            return sRet;
        }

        private SlangFilter ParseSlangFilterJSON(string sJSON, string slangWordKey, string slangWordReplace, SlangFilter dict)
        {
            var array = JArray.Parse(sJSON);
            string sWordKey = String.Empty;
            string sWordReplace = String.Empty;
            int i = 0;
            foreach (var item in array)
            {
                try
                {
                    sWordKey = String.Empty;
                    sWordReplace = String.Empty;
                    try
                    {
                        sWordKey = item[slangWordKey].ToString();
                        sWordReplace = item[slangWordReplace].ToString();

                        dict.InsultFilterItems.Add(new SlangFilter.SlangFilterItem(sWordKey, sWordReplace));
                    }
                    catch (Exception e)
                    {
                        //do something or do nothing
                    }
                    finally
                    {
                    }
                }
                catch (Exception ex)
                {
                    //do something
                }
                i += 1;
            }

            return dict;

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
            Random rnd = new Random();
            ShareFileName = ShareOriginalFileName;
            NotificationText = string.Empty;


            //randomize the insult pool records
            if (rnd.Next(0, salutationCount) <= salutationOccurence)
            {
                MessageSalutation = GetRandomEntry(rnd, lstSalutePool) + " ";
            }
            else
            {
                MessageSalutation = string.Empty;
            }

            EmailSubjectText = GetRandomEntry(rnd, lstSubjPool) + " ";

            Body1 = GetRandomEntry(rnd, lstBodyPool) + " ";

            MsgCloseText = GetRandomEntry(rnd, lstClosePool) + " ";

            MsgSignText = GetRandomEntry(rnd, lstSignPool) + " ";



            MessageSalutation_EN = GetFullMsgText_EN(sSalutePool, MessageSalutation.ToString(), "RU", "EN");
            EmailSubjectText_EN = GetFullMsgText_EN(sSubjPool, EmailSubjectText.ToString(), "RU", "EN");
            Body1_EN = GetFullMsgText_EN(sBodyPool, Body1.ToString(), "RU", "EN");
            MsgCloseText_EN = GetFullMsgText_EN(sClosePool, MsgCloseText.ToString(), "RU", "EN");
            MsgSignText_EN = GetFullMsgText_EN(sSignPool, MsgSignText.ToString(), "RU", "EN");


            FullMessageText_EN = MessageSalutation_EN.ToString() + "\r\n\r\n" + Body1_EN.ToString() + "\r\n\r\n" + MsgCloseText_EN.ToString() + "\r\n\r\n" + MsgSignText_EN.ToString();
            OnPropertyChanged("FullMessageTextDisplay_EN");

            CreateMessageImage();
            ShowShare = true;
        }


        public virtual async void SayMessage()
        {
            NotificationColor = "Black";
            string sMsg = MessageSalutation.ToString() + EmailSubjectText.ToString() + Body1.ToString() + MsgCloseText.ToString() + MsgSignText.ToString();
            SayStuffAsync(Thesaurusify(sMsg));
        }


        public virtual async void CopyToClipboard()
        {
            NotificationColor = "Black";
            string sMsg = MessageSalutation.ToString() + EmailSubjectText.ToString() + Body1.ToString() + MsgCloseText.ToString() + MsgSignText.ToString();
            await Clipboard.SetTextAsync(Thesaurusify(sMsg));
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


        public virtual async void shareMessageEmail()
        {
            NotificationColor = "Black";
            ShowRunning = true;
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
                        Subject = EmailSubjectText.ToString(),
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
            ShowRunning = false;
        }

        public void CreateMessageImage(bool loadEmpty = false)
        {
            string sFullMsgText = "";
            if (loadEmpty == false)
            {
                sFullMsgText = MessageSalutation.ToString() + "\r\n\r\n" + Body1.ToString() + "\r\n\r\n" + MsgCloseText.ToString() + "\r\n\r\n" + MsgSignText.ToString();
            }
            int memeWidth = 600;
            int memeHeight = 300;
            int footerBuffer = 30;
            int insultHeight = 0;
            float textSize = 25;
            float textFooterSize = 10;
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
                    float yText = memeHeight - ((memeHeight - footerBuffer) / 2) - (insultHeight / 2) + (lineNumber + textSize) - footerBuffer;

                    //line buffer
                    yText = yText + ((lineNumber - 1) * textSize);
                    yText = yText + 50;


                    // And draw the text
                    canvas.DrawText(lineText, xText, yText, textPaint);
                    lineNumber += 1;
                }

                //draw footer
                string footerText = "";//"STOP ALL WARS";

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



            //save bitmap to file
            // create an image and then get the PNG (or any other) encoded data
            using (var image = SKImage.FromBitmap(ShareBitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 80))
            { // save the data to a stream
                using (Stream fileStream = File.Open(System.IO.Path.Combine(FileSystem.AppDataDirectory, shareFileName), FileMode.Create))
                {
                    data.SaveTo(fileStream);    //fileStream.Write(Contents, 0, Contents.Length);
                }
            }



            MemoryStream myMemoryStream = GetStream(ShareBitmap);
            Stream myStream = myMemoryStream;

            //ImageSource.FromStream(() => stream2)
            Image bitmap = new Image { Source = ImageSource.FromStream(() => myStream) };
            AppBitmap = bitmap.Source;

            ShareFileName = System.IO.Path.Combine(FileSystem.AppDataDirectory, shareFileName);


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


    public abstract class ViewModelBase : ObservableProperty
    {
        public Dictionary<string, ICommand> Commands { get; protected set; }

        public ViewModelBase()
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

    public class ObservableProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class Extensions
    {
        public static T[] Append<T>(this T[] array, T item)
        {
            if (array == null)
            {
                return new T[] { item };
            }

            T[] result = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = array[i];
            }

            result[array.Length] = item;
            return result;
        }
    }
}