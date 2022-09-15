using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.TextToSpeech;
using System.Threading;
using System;

namespace AppEyeRead
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private String textToSpeech = "Ciao, io sono ai rid. Puoi fermare la mia parlata cliccando sulla parte alta dello schermo. " +
                                                  " Premendo il bottone centrale a destra " +
                                                  "userai la versione con fotocamera, premendo il bottone centrale a  " +
                                                  "sinistra userai la penna per scannerizzare. In caso di errore ti basta " +
                                                  "chiudere e riaprire l’app o cambiare la versione in opzioni. Cliccando una volta sugli elementi ti" +
                                                  "dirò cosa hai premuto, cliccando una seconda volta lo cliccherai davvero";

        private CancellationTokenSource cts;
        private Boolean isSpeaking = true;
        private int tapPhoto = 0;
        private int tapPencil = 0;
        
        public MainPage()
        {
            InitializeComponent();
            Homepage_Presentation();
            tapPhoto = 0;
        }

        private async void Homepage_Presentation()
        {
            /*
             * Ho scoperto che di default va a guardare la lingua di sistema. Perciò il problema si pone quando si vogliono scannerizzare lingue diverse da quelle di sistema!"
             */
             cts = new CancellationTokenSource();
             await TextToSpeech.SpeakAsync(textToSpeech, cts.Token); // così parla in italiano
             isSpeaking = !isSpeaking;
        }

        private async void HomePageCamera(Object sender, EventArgs e)
        {


            if (!isSpeaking)
            {
                tapPhoto++;

                if (tapPhoto == 1)
                {
                    await TextToSpeech.SpeakAsync("Versione con fotocamera");
                    tapPencil = 0;
                }

                else
                {
                    Navigation.PushAsync(new PhotoScanPage());
                }
            }
                
        }

        private async void HomePagePencil(Object sender, EventArgs e)
        {

            if (!isSpeaking)
            {
                tapPencil++;

                if (tapPencil == 1)
                {
                    await TextToSpeech.SpeakAsync("Versione con penna");
                    tapPhoto = 0;
                }

                else
                {
                    
                }
            }

        }
        private void Tap(object sender, EventArgs args)
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();

        }
    }
}
