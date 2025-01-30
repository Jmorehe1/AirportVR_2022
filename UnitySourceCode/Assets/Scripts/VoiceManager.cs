 using UnityEngine;
 using UnityEngine.Windows.Speech;
 using System.Text;
 using System;
 public class VoiceManager : MonoBehaviour
 {
     private string[] m_Keywords;
 
     private KeywordRecognizer m_Recognizer;
 
     void Start()
     {
         m_Keywords = new string[] { "take", "drop" };
         m_Recognizer = new KeywordRecognizer(m_Keywords, ConfidenceLevel.Low);
         m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
         m_Recognizer.Start();
     }
 
     private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
     {
         print("OnPhraseRecognized called");
         StringBuilder builder = new StringBuilder();
         builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
         builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
         builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
         Debug.Log(builder.ToString());
     }
 }