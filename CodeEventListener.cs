using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Composition; // Required for MEF exports if used later
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost; // Required for IComponentModel

namespace CodeSymphony
{
    public class CodeEventListener
    {
        private MusicEngine musicEngine;
        private int typingCounter = 0;
        // private ITextBuffer textBuffer; // No longer needed, OnTextChanged receives sender as ITextBuffer
        // private IWpfTextView textView;   // No longer needed, context is managed by TextViewCreationListener

        public CodeEventListener(MusicEngine musicEngine)
        {
            this.musicEngine = musicEngine ?? throw new ArgumentNullException(nameof(musicEngine));
        }

        // InitializeAsync method is removed as its functionality is superseded by TextViewCreationListener

        public void OnTextChanged(object sender, TextContentChangedEventArgs e)
        {
            if (e == null) return;
            // The 'sender' is the ITextBuffer that raised the event.
            // ITextBuffer buffer = sender as ITextBuffer;
            // if (buffer == null) return; // Or handle error

            foreach (var change in e.Changes)
            {
                AnalyzeTextChange(change);
            }
        }

        private void AnalyzeTextChange(ITextChange change)
        {
            if (change == null) return;

            // Typing rhythm
            typingCounter++;
            if (typingCounter % 4 == 0)
            {
                musicEngine.PlayChord("typing");
            }

            // Brace completion
            if (IsBraceCompletion(change.NewText))
            {
                musicEngine.PlayChord("brace_complete");
            }

            // Method completion (simplified)
            if (IsMethodCompletion(change))
            {
                musicEngine.PlayChord("method_complete");
            }

            // Potentially other analyses:
            // - Error detection might require deeper integration with error lists or Roslyn.
            // - Build success is typically an IDE-level event, not a text change event.
        }

        private bool IsBraceCompletion(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            // Check for common auto-completed pairs.
            // The prompt used text.Contains("{}"), but that would trigger on user typing "{}" not just auto-completion.
            // A more robust check might be needed, but following prompt for now.
            return text.Contains("{}") || text.Contains("()") || text.Contains("[]");
        }

        private bool IsMethodCompletion(ITextChange change)
        {
            if (change == null || string.IsNullOrEmpty(change.NewText)) return false;

            // This is a very simplified check. Real method completion detection would be more complex.
            // For example, it might involve checking the surrounding code or specific editor actions.
            // The prompt's logic: change.NewText.Contains("()") && (change.NewText.Contains("public") || change.NewText.Contains("private") || change.NewText.Contains("function"))
            // This would trigger if the user types out a full method signature including "()".
            // A common scenario for "method_complete" might be after intellisense adds "()" or a full snippet.
            bool containsParentheses = change.NewText.Contains("()");
            bool containsAccessModifierOrKeyword = change.NewText.Contains("public") ||
                                                   change.NewText.Contains("private") ||
                                                   change.NewText.Contains("protected") || // Added protected
                                                   change.NewText.Contains("internal") ||  // Added internal
                                                   change.NewText.Contains("function"); // For JS or other languages

            return containsParentheses && containsAccessModifierOrKeyword;
        }

        // If this class were to be IDisposable:
        // public void Dispose()
        // {
        //     if (this.textBuffer != null)
        //     {
        //         this.textBuffer.Changed -= OnTextChanged;
        //         this.textBuffer = null;
        //     }
        //     this.textView = null;
        //     // musicEngine is a singleton, managed elsewhere.
        // }
    }
}
