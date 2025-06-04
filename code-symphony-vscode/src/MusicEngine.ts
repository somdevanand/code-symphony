import * as ps from 'play-sound';
import * as path from 'path'; // For constructing paths
import * as vscode from 'vscode'; // For getting extension path

// --- ChordProgression Class ---
export class ChordProgression {
    private chords: string[][];
    private currentIndex: number = 0;

    constructor(chordNames: string[]) {
        this.chords = chordNames.map(name => this.getChordNotes(name)).filter(chord => chord.length > 0);
    }

    public getNextChord(): string[] | null {
        if (this.chords.length === 0) {
            return null;
        }
        const chord = this.chords[this.currentIndex];
        this.currentIndex = (this.currentIndex + 1) % this.chords.length;
        return chord;
    }

    private getChordNotes(chordName: string): string[] {
        // Note names can correspond to audio file names (e.g., "C4.wav")
        // Octaves are important for distinct sounds.
        switch (chordName) {
            case "Cmaj": return ["C4", "E4", "G4"];
            case "Amin": return ["A3", "C4", "E4"];
            case "Fmaj": return ["F3", "A3", "C4"];
            case "Gmaj": return ["G3", "B3", "D4"];
            case "Cmaj7": return ["C4", "E4", "G4", "B4"];
            case "Gmaj7": return ["G3", "B3", "D4", "F#4"];
            case "C#min": return ["C#4", "E4", "G#4"]; // Used for error
            // Direct notes for buildSuccessSound example
            case "C4": return ["C4"];
            case "E4": return ["E4"];
            case "G4": return ["G4"];
            case "C5": return ["C5"];
            default:
                // Allow single notes to be passed as "chord names" for progressions like buildSuccess
                if (MusicEngine.noteFrequencies[chordName]) { // Check if it's a known note
                     return [chordName];
                }
                console.warn(`MusicEngine: Unknown chord name or note: ${chordName}`);
                return [];
        }
    }
}

// --- MusicEngine Class ---
export class MusicEngine {
    private player = ps({});
    private eventChords: Map<string, ChordProgression>;
    private soundFilesPath: string;

    // Note frequencies (static member)
    public static noteFrequencies: { [key: string]: number } = {
        'A3': 220.00, 'A#3': 233.08, 'B3': 246.94,
        'C4': 261.63, 'C#4': 277.18, 'D4': 293.66, 'D#4': 311.13,
        'E4': 329.63, 'F4': 349.23, 'F#4': 369.99, 'G4': 392.00,
        'G#4': 415.30, 'A4': 440.00, 'A#4': 466.16, 'B4': 493.88,
        'C5': 523.25, 'C#5': 554.37, 'D5': 587.33, 'D#5': 622.25,
        'E5': 659.25, 'F5': 698.46, 'F#5': 739.99, 'G5': 783.99,
        'G#5': 830.61, 'A5': 880.00,
        // Added missing notes for chords from description
        'F3': 174.61,
        'G3': 196.00,
    };

    constructor(extensionPath?: string) {
        if (extensionPath) {
            this.soundFilesPath = path.join(extensionPath, 'sounds');
        } else {
            // Fallback or default path if extensionPath is not provided
            // This might be an issue if the extension context isn't available during instantiation
            this.soundFilesPath = path.join(__dirname, '..', 'sounds'); // Assumes sounds dir is at project root relative to out/src
            console.warn("MusicEngine: extensionPath not provided, soundFilesPath might be incorrect.");
        }
        this.eventChords = new Map<string, ChordProgression>();
        this.setupChordMappings();
    }

    private setupChordMappings(): void {
        this.eventChords.set("typing", new ChordProgression(["Cmaj", "Gmaj", "Amin", "Fmaj"])); // Common pop progression
        this.eventChords.set("braceComplete", new ChordProgression(["Cmaj7"]));
        this.eventChords.set("methodComplete", new ChordProgression(["Gmaj7"]));
        this.eventChords.set("errorSound", new ChordProgression(["C#min"]));
        // Example of a progression with direct notes for build success
        this.eventChords.set("buildSuccessSound", new ChordProgression(["C4", "E4", "G4", "C5"]));
    }

    public getFrequency(noteName: string): number {
        return MusicEngine.noteFrequencies[noteName] || 0;
    }

    public async playChord(eventType: string, volume: number = 0.3): Promise<void> {
        const progression = this.eventChords.get(eventType);
        if (!progression) {
            console.warn(`MusicEngine: No chord progression found for event: ${eventType}`);
            return;
        }

        const chordNotes = progression.getNextChord();
        if (!chordNotes || chordNotes.length === 0) {
            console.warn(`MusicEngine: No notes found for next chord in event: ${eventType}`);
            return;
        }

        console.log(`MusicEngine: Event '${eventType}', playing chord: ${chordNotes.join(', ')}`);

        for (const noteName of chordNotes) {
            // It's better to use path.join for cross-platform compatibility
            const filePath = path.join(this.soundFilesPath, `${noteName}.wav`);

            console.log(`MusicEngine: Attempting to play ${filePath} (volume: ${volume})`);

            // Note: play-sound typically requires actual sound player CLIs to be installed (mplayer, afplay, etc.)
            // The volume adjustment might also depend on the specific player being used by play-sound.
            this.player.play(filePath, { afplay: ['-v', volume.toString()], mplayer: ['-volume', Math.round(volume * 100).toString()] }, (err: any) => {
                if (err) {
                    // It's common for this to error if sound files don't exist or no player is found
                    console.error(`MusicEngine: Failed to play ${filePath}. Error: ${err.message || err}. Ensure sound files exist and a compatible audio player (e.g., mplayer, afplay) is installed and in PATH.`);
                } else {
                    // console.log(`MusicEngine: Successfully started playing ${filePath}`); // This can be too verbose
                }
            });
            // Adding a small delay between notes in a chord to avoid them all starting at the exact same microsecond
            // This can make chords sound a bit more natural (arpeggiated slightly) or help with player queuing.
            await new Promise(resolve => setTimeout(resolve, 50)); // 50ms delay
        }
    }
}

// Example of how to get the extension path, to be used when instantiating MusicEngine from extension.ts
// In extension.ts:
// import { MusicEngine } from './MusicEngine';
// let musicEngine: MusicEngine;
// export function activate(context: vscode.ExtensionContext) {
//     musicEngine = new MusicEngine(context.extensionPath);
//     // ... rest of activation
// }
// musicEngine.playChord("typing");
