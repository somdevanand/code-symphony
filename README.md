# Code Symphony - A Musical Coding Experience for Visual Studio

Code Symphony is a Visual Studio extension that aims to make your coding sessions more enjoyable and inspiring by playing musical chords and progressions in response to your coding actions. As you type, complete structures, and build your project, Code Symphony provides an ambient musical backdrop to your work.

## Features

*   **Typing Rhythm**: Plays a gentle chord progression as you type, providing a subtle background rhythm.
*   **Code Structure Sounds**: Distinct sounds for completing common code structures:
    *   Brace completion (`{}`, `()`, `[]`)
    *   Method/function definition completion (detects keywords like `public`, `private` followed by `()`)
*   **Build Status**: (Planned) Sounds for build success and failure.
*   **Error Indication**: (Planned) Subtle sound to indicate errors or warnings.

## Setup

To get Code Symphony up and running in your Visual Studio environment, follow these steps:

1.  **Visual Studio Version**:
    *   Requires Visual Studio 2022 or newer.

2.  **.NET Framework**:
    *   The project targets .NET Framework 4.7.2. Ensure this version (or a compatible newer version) is available on your system.

3.  **Visual Studio SDK**:
    *   You must have the Visual Studio SDK installed as part of your Visual Studio setup. If not, modify your Visual Studio installation to include the "Visual Studio extension development" workload.

4.  **Building the Extension**:
    *   Clone this repository to your local machine.
    *   Open `CodeSymphony.sln` (Note: This file will be added in a future step) in Visual Studio.
    *   Build the solution (Ctrl+Shift+B or Build > Build Solution). This will produce a `CodeSymphony.vsix` file in the `bin\Debug` or `bin\Release` directory.

5.  **Installation**:
    *   Close all instances of Visual Studio.
    *   Double-click the generated `CodeSymphony.vsix` file to install the extension.
    *   Follow the on-screen prompts.
    *   Re-open Visual Studio.

## Usage

Once installed, Code Symphony activates automatically when you open a text editor within Visual Studio.
*   **Automatic Sounds**: As you perform actions like typing or completing code structures (e.g., typing `()` after a method name, or having braces `{}` auto-completed), corresponding sounds will play.
*   **Configuration**: (Planned) Future versions may include options to customize sounds, volume, and active event triggers via the Visual Studio Options dialog.

## Known Issues/Limitations

*   **Sound Customization**: Sounds, chords, and volume levels are currently hardcoded in `MusicEngine.cs`. Customization options are planned.
*   **Event Detection Granularity**: The detection for events like "method completion" is based on simple text heuristics (e.g., presence of "public" and "()") and may not cover all scenarios or language syntaxes perfectly.
*   **Resource Intensive Operations**: Playing many sounds in rapid succession (e.g., very fast typing or large paste operations) might have a minor performance impact, though efforts are made to keep it lightweight.
*   **Build and Error Sounds**: Sounds for build success/failure and error indications are planned features and not yet implemented.

## Contributing

Contributions are welcome! If you have ideas for new features, improvements, or bug fixes, please consider the following:

1.  Fork the repository.
2.  Create a new branch for your feature or fix.
3.  Make your changes.
4.  Submit a pull request with a clear description of your changes.

## License

This project is provisionally licensed under the MIT License. (A formal LICENSE file will be added in a future step).
