using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// La plantilla de elemento Página en blanco está documentada en http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace speechRecog1
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SpeechRecognizer recognizer;

        private const string productName = "Visual C#";
        private const int GREEN_LED_PIN = 6;      
        private const int BEDROOM_LIGHT_PIN = 13;        
        private const string TAG_TARGET = "target";       
        private const string TAG_CMD = "cmd";
        private const string TAG_DEVICE = "device";
        private const string STATE_ON = "ON";
        private const string STATE_OFF = "OFF";
        private const string DEVICE_LED = "LED";
        private const string DEVICE_LIGHT = "LIGHT";
        private const string COLOR_RED = "RED";
        private const string COLOR_GREEN = "GREEN";
        private const string TARGET_BEDROOM = "BEDROOM";
        private const string TARGET_PORCH = "PORCH";

        public MainPage()
        {
            
            this.InitializeComponent();

            initializeSpeechRecognizer();
        }

        private async void initializeSpeechRecognizer()
        {
        

            // Initialize recognizer
            recognizer = new SpeechRecognizer();

            // Set event handlers
            recognizer.StateChanged += RecognizerStateChanged;
            recognizer.ContinuousRecognitionSession.ResultGenerated += RecognizerResultGenerated;

            // Load Grammer file constraint
            string fileName = String.Format("Grammar\\grammar.xml");
            StorageFile grammarContentFile = await Package.Current.InstalledLocation.GetFileAsync(fileName);

            SpeechRecognitionGrammarFileConstraint grammarConstraint = new SpeechRecognitionGrammarFileConstraint(grammarContentFile);

            // Add to grammer constraint
            recognizer.Constraints.Add(grammarConstraint);

            // Compile grammer
            SpeechRecognitionCompilationResult compilationResult = await recognizer.CompileConstraintsAsync();

            Debug.WriteLine("Status: " + compilationResult.Status.ToString());

            // If successful, display the recognition result.
            if (compilationResult.Status == SpeechRecognitionResultStatus.Success)
            {
                System.Diagnostics.Debug.WriteLine("Result: " + compilationResult.ToString());

                await recognizer.ContinuousRecognitionSession.StartAsync();
            }
            else
            {
                Debug.WriteLine("Status: " + compilationResult.Status);
            }
        }

        private void RecognizerStateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            Debug.WriteLine("Speech recognizer state: " + args.State.ToString());
        }

        private void RecognizerResultGenerated(SpeechContinuousRecognitionSession session, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            // Output debug strings
            Debug.WriteLine(args.Result.Status);
            Debug.WriteLine(args.Result.Text);

            int count = args.Result.SemanticInterpretation.Properties.Count;

            Debug.WriteLine("Count: " + count);
            Debug.WriteLine("Tag: " + args.Result.Constraint.Tag);

            // Check for different tags and initialize the variables
            String target = args.Result.SemanticInterpretation.Properties.ContainsKey(TAG_TARGET) ?
                            args.Result.SemanticInterpretation.Properties[TAG_TARGET][0].ToString() :
                            "";

            String cmd = args.Result.SemanticInterpretation.Properties.ContainsKey(TAG_CMD) ?
                            args.Result.SemanticInterpretation.Properties[TAG_CMD][0].ToString() :
                            "";

            String device = args.Result.SemanticInterpretation.Properties.ContainsKey(TAG_DEVICE) ?
                            args.Result.SemanticInterpretation.Properties[TAG_DEVICE][0].ToString() :
                            "";

            // Whether state is on or off
            bool isOn = cmd.Equals(STATE_ON);

            Debug.WriteLine("Target: " + target + ", Command: " + cmd + ", Device: " + device);

            // First check which device the user refers to
            if (device.Equals(DEVICE_LED))
            {
                // Check what color is specified
                if (target.Equals(COLOR_RED))
                {
                    Debug.WriteLine("RED LED " + (isOn ? STATE_ON : STATE_OFF));                                        
                }
                else if (target.Equals(COLOR_GREEN))
                {
                    Debug.WriteLine("GREEN LED " + (isOn ? STATE_ON : STATE_OFF));                    
                }
                else
                {
                    Debug.WriteLine("Unknown Target");
                }
            }
            else if (device.Equals(DEVICE_LIGHT))
            {
                // Check target location
                if (target.Equals(TARGET_BEDROOM))
                {
                    Debug.WriteLine("BEDROOM LIGHT " + (isOn ? STATE_ON : STATE_OFF));
                    
                }
                else if (target.Equals(TARGET_PORCH))
                {
                    Debug.WriteLine("PORCH LIGHT " + (isOn ? STATE_ON : STATE_OFF));

                    // Insert code to control Porch light
                }
                else
                {
                    Debug.WriteLine("Unknown Target");
                }
            }
            else
            {
                Debug.WriteLine("Unknown Device");
            }

        }
    }
}
