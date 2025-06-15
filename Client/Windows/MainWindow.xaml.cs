using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using VoiceAssistant.Classes.Recognition;
using VoiceAssistant.Features.ProcessInterop;
using VoiceAssistant.Features.Recognition.Interfaces;
using VoiceAssistant.Features.Recognition.Services;
using VoiceAssistant.Grps;

namespace VoiceAssistant.Windows
{
    public sealed partial class MainWindow : Window
    {
        private readonly IVoiceRecognizer _recognizer;

        private bool isListening = false;

        public MainWindow()
        {
            InitializeComponent();

            var grpcClient = new SpeechRecognizerGrpcClient("http://localhost:50051");
            var processManager = new StreamingGrpcProcessManager(grpcClient);
            var recognitionManager = new RecognitionManager(processManager);
            var grpcCoordinator = new GrpcProcessCoordinator(processManager);

            _recognizer = new VoiceRecognitionService(
                recognitionManager,
                grpcCoordinator,
                new SpeechResultHandler()
            );

            _recognizer.OnSpeechRecognized += Recognizer_OnSpeechRecognized;

            _recognizer.OnVolumeChanged += Recognizer_OnVolumeChanged;

            _recognizer.OnRecognitionStopped += Recognizer_OnRecognitionStopped;

            _recognizer.OnCommandResult += Recognizer_OnCommandResult;

            this.Closed += MainWindow_Closed;
        }
        private void UpdateMyButtonContent(bool isListening)
        {
            myButton.Content = isListening ? "Зупинити слухання" : "Почати слухати";
        }
        private async void Recognizer_OnCommandResult(string text)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                commandOutputTextBlock.Text = text;
            });

            await Task.Delay(10000);

            DispatcherQueue.TryEnqueue(() =>
            {
                commandOutputTextBlock.Text = "";
            });
        }

        private void Recognizer_OnSpeechRecognized(string text)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_recognizer.IsModelReady)
                {
                    myTextBlock.Text = text;
                }
            });
        }
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isListening)
            {
                _recognizer.StartRecognition();
                myTextBlock.Text = "Я слухаю. Скажіть «стоп», щоб зупинити слухання";
                isListening = true;
            }
            else
            {
                _recognizer.PauseRecognition();
                isListening = false;
            }
            UpdateMyButtonContent(isListening);
        }
        private void Recognizer_OnRecognitionStopped(string obj)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                myTextBlock.Text += "\n[Прослуховування зупинено]";
                
                isListening = false;
                UpdateMyButtonContent(isListening);
            });
        }

        private void Recognizer_OnVolumeChanged(double volume)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                volumeTextBlock.Text = $"Гучність: {volume: 0.00}";

                animatedSphere.Update(volume);
            });
        }
        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            _recognizer.OnSpeechRecognized -= Recognizer_OnSpeechRecognized;
            _recognizer.OnVolumeChanged -= Recognizer_OnVolumeChanged;
            _recognizer.OnRecognitionStopped -= Recognizer_OnRecognitionStopped;
            _recognizer.OnCommandResult -= Recognizer_OnCommandResult;

            if (_recognizer is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
