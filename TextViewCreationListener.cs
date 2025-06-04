using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text; // Required for ITextBuffer
// using Microsoft.VisualStudio.Shell; // Not strictly needed if using MusicEngine.Instance

namespace CodeSymphony
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")] // Apply to all text-based content types. Could be "code".
    [TextViewRole(PredefinedTextViewRoles.Editable)] // Only for editable text views.
    internal class TextViewCreationListener : IWpfTextViewCreationListener
    {
        // If we needed to import services, we'd use [Import] here.
        // For example:
        // [Import]
        // internal ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            // Ensure MusicEngine.Instance is available.
            var musicEngineInstance = MusicEngine.Instance;
            if (musicEngineInstance == null)
            {
                // Log or handle the error: MusicEngine singleton is not available.
                // This might happen if the package hasn't loaded or MusicEngine failed to initialize.
                // Depending on error handling strategy, could throw or output to VS activity log.
                System.Diagnostics.Debug.WriteLine("CodeSymphony: MusicEngine.Instance is null in TextViewCreated.");
                return;
            }

            // Create a new CodeEventListener for this specific text view.
            // This instance is different from any that might be created in CodeSymphonyPackage.
            // We are primarily interested in its OnTextChanged method.
            // The original CodeEventListener has an InitializeAsync method, but that was for
            // package-level setup (getting services, interacting with AsyncPackage).
            // Here, we just need the text change analysis logic.
            var codeEventListener = new CodeEventListener(musicEngineInstance);

            if (textView != null && textView.TextBuffer != null)
            {
                // Subscribe to the text buffer's Changed event using the new listener's handler.
                textView.TextBuffer.Changed += codeEventListener.OnTextChanged;

                // Handle view closure to unsubscribe, preventing memory leaks.
                // This is crucial because codeEventListener is created per view.
                textView.Closed += (sender, args) =>
                {
                    if (textView != null && textView.TextBuffer != null)
                    {
                        textView.TextBuffer.Changed -= codeEventListener.OnTextChanged;
                    }
                    // If CodeEventListener were IDisposable, and it made sense to dispose it here:
                    // (codeEventListener as IDisposable)?.Dispose();
                    // However, for the current CodeEventListener, its main job is event handling,
                    // and unhooking the event is the primary cleanup. MusicEngine is a singleton.
                };
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CodeSymphony: TextView or TextBuffer is null in TextViewCreated.");
            }
        }
    }
}
