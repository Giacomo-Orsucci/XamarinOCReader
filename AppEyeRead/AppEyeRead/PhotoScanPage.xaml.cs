using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AppEyeRead;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.TextToSpeech;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace AppEyeRead
{
    public partial class PhotoScanPage : CarouselPage
    {
        private Button CaptureButton;
        private Button SelectButton;
        private Button Save;
        private Button NotSave;

        private Image Image;
        private Label Text;
        private Entry Title;

        private Boolean isSpeaking = true;
        private int tapCapture = 0;
        private int tapSelect = 0;
        private int tapSave = 0;
        private int tapNotSave = 0;

        private CancellationTokenSource cts;



        public PhotoScanPage()
        {
            InitializeComponent();
            BuildUI();   
        }

        
        private void BuildUI()
        {

            isSpeaking = true;
            tapCapture = 0;
            tapSelect = 0;
            tapSave = 0;
            tapNotSave = 0;


            CaptureButton = new Button
            {
                Text = "Cattura",
                Margin = new Thickness(50, 50, 50, 50),
                BackgroundColor = Color.Aquamarine,
            };



            SelectButton = new Button
            {
                Text = "Seleziona",
                Margin = new Thickness(50, 50, 50, 50),
                BackgroundColor = Color.Aquamarine,
            };

            Grid grid = new Grid
            {
                RowDefinitions = 
                { 
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star)}, 
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star)},
                },

                ColumnDefinitions =
                {
                    new ColumnDefinition ()
                }
                
            };


            grid.Children.Add(CaptureButton, 0, 0);
            grid.Children.Add(SelectButton, 0, 1);

            ScanContent.Content = grid;
            this.CurrentPage = ScanContent;

            Events1();
        }

        private void RefreshUI()
        {
            Image = new Image
            {   
               HeightRequest = 200,
              
            };

            Text = new Label
            {

            };

            Title = new Entry
            {
                Placeholder = "Nuova nota",
            };

            Save = new Button
            {
                Text = "Salva",
                BackgroundColor = Color.Aquamarine,
                MinimumHeightRequest = 100
            };

            NotSave = new Button
            {
                Text = "Non salvare",
                BackgroundColor = Color.Aquamarine,
                MinimumHeightRequest = 100

            };

            Events2();

            ScrollView scrollView = new ScrollView
            {
                Content = Text 

            };


            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto}
                },

                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
                },

            };

            TapGestureRecognizer TGR = new TapGestureRecognizer();

            TGR.Tapped += Tap;

            grid.Children.Add(Save, 0, 0);
            grid.Children.Add(NotSave, 1, 0);


            StackLayout stack = new StackLayout
            {

                Children = { Title, Image, scrollView, grid }
            };

            ScanContent.Content = stack;

            stack.GestureRecognizers.Add(TGR);
          
        }
 
        private void Clear()
        {
            ScanContent.Content = null;
        }

        private async void Capture_OnClicked(object sender, EventArgs args)
        {
            
                tapCapture++;

                if (tapCapture == 1)
                {
                    await TextToSpeech.SpeakAsync("Cattura foto");
                    tapSelect = 0;
                }

                else
                {
                     Clear();

                     var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                     {
                         PhotoSize = PhotoSize.Small
                     });

                    RefreshUI();
                    Update(photo);   
                }
        }
        private async void Select_OnClicked(object sender, EventArgs args)
        {
            
                tapSelect++;

                if (tapSelect == 1)
                {
                    await TextToSpeech.SpeakAsync("Seleziona foto dalla galleria");
                    tapCapture = 0;
                }

                else
                {
                    Clear();
                    var photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Small
                    });
                    Update(photo);

                    RefreshUI();
                }  
        }
        private async void NotSave_OnClicked(object sender, EventArgs args)
        {
            if (!isSpeaking)
            {
                tapNotSave++;

                if (tapNotSave == 1)
                {
                    await TextToSpeech.SpeakAsync("Non salvare");
                    tapSave = 0;
                }
                else
                {
                    await TextToSpeech.SpeakAsync("Nota non salvata, puoi continuare con le scansioni");
                    Clear();
                    BuildUI();

                }
            }

        }
        private async void Save_OnClicked(object sender, EventArgs args)
        {
            if (!isSpeaking)
            {
                tapSave++;

                if (tapSave == 1)
                {
                    await TextToSpeech.SpeakAsync("Salva la nota");
                    tapNotSave = 0;
                }
                else
                {
                    await TextToSpeech.SpeakAsync("Nota salvata, trovi tutte le note salvate svuaipando a destra");

                }
            }
        }

        private void Update(MediaFile photo)
        {
            Task.Run(async () =>
            {
                var text = await Ocr.GetTextAsync(photo.GetStreamWithImageRotatedForExternalStorage());
                Device.BeginInvokeOnMainThread(() => Text.Text = text);


                cts = new CancellationTokenSource();
                await TextToSpeech.SpeakAsync("Clicca poco sopra al centro dello schermo per interrompere la lettura.     " + text, cts.Token);
                isSpeaking = !isSpeaking;
            });

            Image.Source = ImageSource.FromStream(photo.GetStream);
        }
        private void Events1()
        {
            CaptureButton.Clicked += Capture_OnClicked;
            SelectButton.Clicked += Select_OnClicked;
        }
        private void Events2()
        {
            NotSave.Clicked += NotSave_OnClicked;
            Save.Clicked += Save_OnClicked;
            
        }
        private void Tap(object sender, EventArgs args)
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();

        }
    }
}