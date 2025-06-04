import * as vscode from 'vscode';
import { MusicEngine } from './MusicEngine'; // Assuming MusicEngine.ts is in the same directory

let musicEngine: MusicEngine;
let typingCounter = 0;

// Variables to hold current configuration values
let enableMusic: boolean;
let volume: number;

export function activate(context: vscode.ExtensionContext) {
	console.log('Code Symphony VSCode is now active!');

	// Initialize MusicEngine with the extension's path for sound file resolution
	musicEngine = new MusicEngine(context.extensionPath);
	console.log('Code Symphony MusicEngine initialized.');

	// Read initial configuration
	let config = vscode.workspace.getConfiguration('codeSymphony');
	enableMusic = config.get<boolean>('enableMusic', true);
	volume = config.get<number>('volume', 0.3);
	console.log(`Initial configuration: enableMusic=${enableMusic}, volume=${volume}`);

	const textChangeListener = vscode.workspace.onDidChangeTextDocument(event => {
		if (!musicEngine) return;

		if (!event.contentChanges || event.contentChanges.length === 0) {
			return;
		}
		const change = event.contentChanges[0];

		// 1. Typing Rhythm
		if (change.text.length === 1 && change.rangeLength === 0 && !change.text.match(/\s/)) {
			typingCounter++;
			if (enableMusic && typingCounter % 4 === 0) {
				console.log('Event: Typing rhythm detected');
				musicEngine.playChord('typing', volume).catch(console.error);
			}
		} else if (change.text.length > 1 || change.rangeLength > 0) {
			typingCounter = 0;
		}

		// 2. Brace Completion
		// Check if new text itself is a pair or if it completes a pair with surrounding text.
		const newText = change.text;
		const rangeEnd = change.range.end;
		let braceCompleted = false;

		if (newText === '{}' || newText === '()' || newText === '[]') {
			braceCompleted = true;
		} else if (vscode.window.activeTextEditor && vscode.window.activeTextEditor.document === event.document) {
			// Check for VS Code's auto-closing feature
			// This requires checking the character immediately after the inserted text
			const charAfterInserted = event.document.getText(new vscode.Range(rangeEnd, rangeEnd.translate(0, 1)));
			if ((newText === '{' && charAfterInserted === '}') ||
				(newText === '(' && charAfterInserted === ')') ||
				(newText === '[' && charAfterInserted === ']'))
			{
				braceCompleted = true;
			}
		}

		if (braceCompleted) {
			if (enableMusic) {
				console.log('Event: Brace completion detected');
				musicEngine.playChord('braceComplete', volume).catch(console.error);
			}
			typingCounter = 0;
		}

		// 3. Basic Method/Function Completion (Heuristic)
		// Looks for patterns like "() {" or "=> {" typed, or "function name()".
		// This is a very simplified heuristic and might need refinement.
		const lineText = event.document.lineAt(change.range.start.line).text;
		// Heuristic 1: User types "()" and the line potentially ends with "()", "() {", or "){"
		if (newText.includes('()')) {
			if (lineText.match(/\)\s*\{?$/) || lineText.endsWith(")")) {
				if (enableMusic) {
					console.log('Event: Method/Function completion detected (typed "()")');
					musicEngine.playChord('methodComplete', volume).catch(console.error);
				}
				typingCounter = 0;
			}
		}
		else if (newText.includes('=>')) {
			if (lineText.match(/=>\s*\{?$/) || lineText.endsWith("=>")) {
				if (enableMusic) {
					console.log('Event: Method/Function completion detected (typed "=>")');
					musicEngine.playChord('methodComplete', volume).catch(console.error);
				}
				typingCounter = 0;
			}
		}
		// This is tricky because '}' is used for many blocks.
		// A more robust check might involve AST parsing or more context.
		// For now, we'll rely on the above or consider if a specific pattern ends with '}'.
		// Example: if (newText === '}' && lineText.match(/\(\s*\)\s*\{[^\}]*$/)) { // after () { ... }
		// This is getting complex for simple heuristics.
	});

	// Listener for configuration changes
	const configChangeListener = vscode.workspace.onDidChangeConfiguration(event => {
		if (event.affectsConfiguration('codeSymphony')) {
			config = vscode.workspace.getConfiguration('codeSymphony');
			enableMusic = config.get<boolean>('enableMusic', true);
			volume = config.get<number>('volume', 0.3);
			console.log('Code Symphony configuration updated: enableMusic=' + enableMusic + ', volume=' + volume);
		}
	});

	context.subscriptions.push(textChangeListener, configChangeListener);
	console.log('Text change and configuration change listeners subscribed.');
}

export function deactivate() {
	console.log('Code Symphony VSCode deactivated.');
}
