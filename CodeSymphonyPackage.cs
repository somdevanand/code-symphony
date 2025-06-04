using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task; // Explicit alias for Task
using System.ComponentModel.Design; // Required for OleMenuCommandService / MenuCommand

namespace CodeSymphony
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CodeSymphonyPackage.PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)] // To support potential future menus
    public sealed class CodeSymphonyPackage : AsyncPackage
    {
        // NOTE: Replace "YOUR-GUID-HERE" with an actual new GUID in the final implementation.
        // For this step, I will generate one. Let's assume it is:
        public const string PackageGuidString = "f5d8f5d9-3de2-46b8-9e4a-a23f12f999a3"; // Example GUID

        private MusicEngine musicEngine;
        // CodeEventListener field is removed as TextViewCreationListener now handles event listener registration.

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // MusicEngine is a singleton, its instance is created on first access.
            // This line ensures the MusicEngine is initialized and can be used if the package
            // itself needs to interact with it directly, or just to ensure it's ready.
            this.musicEngine = MusicEngine.Instance;

            // The CodeEventListener is no longer initialized here.
            // Its functionality is now managed by TextViewCreationListener,
            // which creates instances of CodeEventListener for each text view.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // MusicEngine is IDisposable. As a singleton, its lifecycle might be tied to the AppDomain.
                // If MusicEngine.Instance.Dispose() is intended to be called when the package unloads,
                // this.musicEngine?.Dispose(); could be called.
                // However, typically singletons manage their own lifecycle or are disposed of at AppDomain unload.
                // For now, we assume MusicEngine's own IDisposable implementation is sufficient if it's ever explicitly disposed.

                // No specific disposal needed for codeEventListener here anymore.
            }
            base.Dispose(disposing);
        }
    }
}
