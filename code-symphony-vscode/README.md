# Code Symphony - A Musical Coding Experience for VS Code

Code Symphony is a Visual Studio Code extension that aims to make your coding sessions more enjoyable and inspiring by playing musical chords and progressions in response to your coding actions. As you type, complete structures, and (soon) build your project, Code Symphony provides an ambient musical backdrop to your work.

## Features

*   **Typing Rhythm**: Plays a gentle chord progression as you type, providing a subtle background rhythm.
*   **Code Structure Sounds**: Distinct sounds for completing common code structures:
    *   Brace completion (`{}`, `()`, `[]`)
    *   Method/function definition completion (detects patterns like `function foo() {` or `const bar = () => {`)
*   **Configurable**: Enable/disable sounds and adjust volume through VS Code settings.
*   **Build Status Sounds**: (Planned) Sounds for build success and failure.
*   **Error Indication**: (Planned) Subtle sound to indicate errors or warnings.

## Requirements

*   **Visual Studio Code**: Version 1.75.0 or newer (as specified in `package.json` engines).
*   **Audio Playback Support**:
    *   This extension uses the `play-sound` library. For audio to play, you need a command-line audio player installed and available in your system's PATH.
    *   Common players supported by `play-sound` include:
        *   **Windows**: `mplayer`, `afplay` (if Windows Subsystem for Linux with appropriate tools), `powershell` (for native beeps, less ideal for complex sounds).
        *   **macOS**: `afplay` (comes pre-installed), `mplayer`.
        *   **Linux**: `mplayer`, `aplay`, `mpg123`, `omxplayer`, `vlc`.
    *   Without a compatible player, the extension will run but no sound will be produced. Errors from `play-sound` will be logged to the VS Code Developer Console.

## Building from Source

To build the Code Symphony extension from source:

1.  **Clone the Repository**:
    ```bash
    git clone <repository-url>
    cd code-symphony-vscode
    ```
2.  **Install Dependencies**:
    ```bash
    npm install
    ```
3.  **Compile TypeScript**:
    The TypeScript code is typically compiled using the command specified in `package.json` (e.g., `npm run compile` or `npm run vscode:prepublish`).
    ```bash
    npm run compile
    ```
    This will transpile the TypeScript files from `src/` to JavaScript in the `out/` directory.
4.  **Package the Extension**:
    To create a `.vsix` package for installation:
    ```bash
    npx vsce package
    ```
    This command uses the `vsce` (Visual Studio Code Extensions) tool. If you don't have it installed globally, you can install it with `npm install -g @vscode/vsce`. (Note: it's also listed as a devDependency, so `npx vsce package` from the project root after `npm install` should work).

## Installation

There are a couple of ways to install Code Symphony:

*   **From Marketplace (Future)**:
    Once published, you will be able to search for "Code Symphony" in the VS Code Extensions view (Ctrl+Shift+X or Cmd+Shift+X) and click "Install".

*   **From a `.vsix` File (Manual Installation)**:
    1.  If you have built the extension from source (see above) or downloaded a `.vsix` file.
    2.  Open Visual Studio Code.
    3.  Go to the Extensions view (Ctrl+Shift+X or Cmd+Shift+X).
    4.  Click the "..." menu (Views and More Actions...) in the top-right corner of the Extensions view.
    5.  Select "Install from VSIX..." and choose the `code-symphony-vscode-X.X.X.vsix` file.
    6.  VS Code will install the extension and prompt you to reload if necessary.

## Usage

Once installed and enabled, Code Symphony activates automatically.
*   **Automatic Sounds**: As you perform actions like typing or completing code structures, corresponding sounds will play based on your configuration.
*   **Output/Logs**: Check the VS Code Developer Tools console (Help > Toggle Developer Tools) for log messages from Code Symphony, including information about played sounds or errors.

## Configuration

You can configure Code Symphony through the VS Code settings:

1.  Open Settings:
    *   Press `Ctrl+,` (comma) or `Cmd+,` on macOS.
    *   Or, go to File > Preferences > Settings (Code > Settings > Settings on macOS).
2.  Search for "Code Symphony" in the search bar.
3.  Adjust the following settings:
    *   **`codeSymphony.enableMusic`**:
        *   Type: `boolean`
        *   Default: `true`
        *   Description: Enable or disable all sounds from Code Symphony.
    *   **`codeSymphony.volume`**:
        *   Type: `number`
        *   Default: `0.3`
        *   Minimum: `0.0` (silent)
        *   Maximum: `1.0` (full volume configured for the player)
        *   Description: Master volume for Code Symphony sounds.

Changes to settings are applied dynamically.

## Known Issues/Limitations

*   **Actual Sound Files**: The `sounds/` directory currently contains empty placeholder `.wav` files. To hear actual music/sounds, these placeholders need to be replaced with real audio files corresponding to the note names (e.g., `C4.wav`, `E4.wav`).
*   **Dependency on External Audio Players**: The `play-sound` library relies on external command-line audio players (e.g., `mplayer`, `afplay`). If a compatible player is not installed or not in the system's PATH, sounds will not play.
*   **Event Detection Granularity**: The detection for events like "method completion" is based on simple text heuristics and may not cover all programming languages or coding styles perfectly.
*   **Sound Variety and Customization**: Sounds, chords, and specific event-to-sound mappings are currently hardcoded in `MusicEngine.ts`. More advanced customization options are planned.
*   **Build and Error Sounds**: Sounds for build success/failure and specific error indications are planned features and not yet implemented in the VS Code version.

## Contributing

Contributions are welcome! If you have ideas for new features, improvements, bug fixes, or actual sound files, please consider the following:

1.  Fork the repository.
2.  Create a new branch for your feature or fix.
3.  Make your changes.
4.  Submit a pull request with a clear description of your changes.

## License

This project is provisionally licensed under the MIT License. (A formal LICENSE file will be added in a future step).

## Packaging and Publishing

This section describes how to package the extension into a `.vsix` file for local installation or distribution, and outlines the steps for publishing to the Visual Studio Code Marketplace.

### Packaging

To package the extension into a `.vsix` file:

1.  **Install Dependencies**: Ensure you have installed all Node.js dependencies:
    ```bash
    npm install
    ```
2.  **Run the Package Script**: Use the npm script defined in `package.json`:
    ```bash
    npm run package
    ```
    This command utilizes `vsce package --no-dependencies` to bundle the extension. It will create a file named `code-symphony-vscode-[version].vsix` in the project root directory (e.g., `code-symphony-vscode-0.0.1.vsix`). This `.vsix` file can then be installed into VS Code as described in the "Installation" section.

### Publishing to VS Code Marketplace (Outline)

Publishing the extension makes it available to all VS Code users through the official Marketplace. This process requires you to have a Publisher ID.

**Prerequisites**:

*   **`vsce` Tool**: Ensure `@vscode/vsce` is installed (it's a devDependency, so `npm install` followed by using `npx vsce` or the npm script `npm run package` should cover it for local use. For global use, or if you prefer: `npm install -g @vscode/vsce`).
*   **Azure DevOps Organization**: You'll need an Azure DevOps organization to create a publisher.
*   **Personal Access Token (PAT)**: Generate a PAT from your Azure DevOps organization with the "Marketplace (publish)" scope.
*   **Publisher ID**: Create a publisher ID through the Visual Studio Marketplace portal. Your `package.json` file must include this `publisher` ID (e.g., the "JulesDeveloper" placeholder needs to be updated to your actual ID).

**General Steps (to be performed by you with your credentials)**:

1.  **Update `package.json`**:
    *   Ensure the `publisher` field in `package.json` is set to your unique publisher ID.
    *   It's recommended to increment the `version` number for each new release.
2.  **Login (Optional, if not using PAT for all operations)**:
    *   You can login to vsce with your publisher name:
        ```bash
        npx vsce login <your-publisher-name>
        ```
3.  **Package (if not already done)**:
    *   `npm run package`
4.  **Publish**:
    *   Using a Personal Access Token (PAT) is recommended:
        ```bash
        npx vsce publish --pat <your-personal-access-token>
        ```
    *   Alternatively, if you are logged in:
        ```bash
        npx vsce publish
        ```

**Further Information**:

*   For detailed and up-to-date instructions, please refer to the official VS Code documentation: [Publishing Extensions](https://code.visualstudio.com/api/working-with-extensions/publishing-extension)
