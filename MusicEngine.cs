using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq; // Added for Select and ToArray

namespace CodeSymphony
{
    public class MusicEngine : IDisposable
    {
        private WaveOutEvent waveOut;
        private MixingSampleProvider mixer;
        private Dictionary<string, ChordProgression> eventChords;

        private static readonly Lazy<MusicEngine> lazyInstance = new Lazy<MusicEngine>(() => new MusicEngine());
        public static MusicEngine Instance => lazyInstance.Value;

        private MusicEngine()
        {
            InitializeAudio();
            SetupChordMappings();
        }

        private void InitializeAudio()
        {
            waveOut = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
            {
                ReadFully = true // Important for continuous playback
            };
            waveOut.Init(mixer);
            waveOut.Play();
        }

        private void SetupChordMappings()
        {
            eventChords = new Dictionary<string, ChordProgression>
            {
                { "typing", new ChordProgression(new[] { "C", "G", "Am", "F" }) }, // Simple progression for typing
                { "brace_complete", new ChordProgression(new[] { "C" }) },        // Single C major chord
                { "method_complete", new ChordProgression(new[] { "G" }) },       // Single G major chord
                { "class_complete", new ChordProgression(new[] { "F" }) },        // Single F major chord
                { "error", new ChordProgression(new[] { "E" }) },                 // Single E major chord (can be minor for dissonance)
                { "build_success", new ChordProgression(new[] { "C", "F", "G", "C" }) } // Uplifting progression
            };
        }

        public void PlayChord(string eventType, float volume = 0.3f)
        {
            if (eventChords.TryGetValue(eventType, out var progression))
            {
                string[] notes = progression.GetNextChord();
                if (notes != null)
                {
                    var chordSampleProvider = GenerateChord(notes, volume);
                    mixer.AddMixerInput(chordSampleProvider);
                }
            }
        }

        private ISampleProvider GenerateChord(string[] notes, float volume)
        {
            var providers = new List<ISampleProvider>();
            foreach (var note in notes)
            {
                float frequency = GetFrequency(note);
                if (frequency > 0)
                {
                    var oscillator = new SignalGenerator(mixer.WaveFormat.SampleRate, mixer.WaveFormat.Channels)
                    {
                        Gain = volume,
                        Frequency = frequency,
                        Type = SignalGeneratorType.Sin // Using Sine wave for a softer tone
                    };

                    // ADSR Settings: Attack 0.1s, Decay 0.3s, Sustain Level 0.5, Release 0.8s
                    var adsr = new AdsrSampleProvider(oscillator.ToSampleProvider())
                    {
                        AttackSeconds = 0.05f, // Quick attack
                        DecaySeconds = 0.2f,   // Short decay
                        SustainLevel = 0.6f,  // Moderate sustain
                        ReleaseSeconds = 0.3f // Moderate release
                    };

                    // We need to take a finite duration for the note
                    providers.Add(adsr.Take(TimeSpan.FromSeconds(1.0))); // Play each note for 1 second
                }
            }

            if (!providers.Any())
            {
                // Return a silence provider if no valid notes are found to prevent errors
                return new SilenceProvider(mixer.WaveFormat).ToSampleProvider();
            }

            // Mix all notes of the chord together
            return new MixingSampleProvider(providers);
        }

        private float GetFrequency(string note)
        {
            // Basic frequencies for one octave (C4 to B4)
            // More comprehensive mapping might be needed for richer chords
            switch (note.ToUpper())
            {
                case "C": return 261.63f; // C4
                case "C#": return 277.18f;
                case "D": return 293.66f;
                case "D#": return 311.13f;
                case "E": return 329.63f;
                case "F": return 349.23f;
                case "F#": return 369.99f;
                case "G": return 392.00f;
                case "G#": return 415.30f;
                case "A": return 440.00f; // A4
                case "A#": return 466.16f;
                case "B": return 493.88f;
                default: return 0; // Unknown note
            }
        }

        public void Dispose()
        {
            waveOut?.Stop();
            waveOut?.Dispose();
            waveOut = null;
            // Mixer does not require explicit disposal beyond what WaveOutEvent handles
        }
    }

    public class ChordProgression
    {
        private string[][] chords;
        private int currentIndex = 0;

        public ChordProgression(string[] chordNames)
        {
            chords = chordNames.Select(GetChordNotes).Where(c => c != null).ToArray();
        }

        public string[] GetNextChord()
        {
            if (chords == null || chords.Length == 0) return null;
            string[] chord = chords[currentIndex];
            currentIndex = (currentIndex + 1) % chords.Length;
            return chord;
        }

        private string[] GetChordNotes(string chordName)
        {
            // Basic major chords. Can be expanded.
            switch (chordName.ToUpper())
            {
                case "C": return new[] { "C", "E", "G" };       // C Major
                case "AM": return new[] { "A", "C", "E" };      // A Minor
                case "F": return new[] { "F", "A", "C" };       // F Major
                case "G": return new[] { "G", "B", "D" };       // G Major
                case "E": return new[] { "E", "G#", "B" };      // E Major
                default: return null; // Unknown chord
            }
        }
    }
}
