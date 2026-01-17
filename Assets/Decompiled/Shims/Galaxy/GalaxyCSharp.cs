// Shim for GOG Galaxy SDK
// Original: GalaxyCSharp.dll
// Purpose: Provide compilation stubs for Galaxy.Api namespace
// Note: All methods throw NotImplementedException at runtime

using System;

namespace Galaxy.Api
{
    /// <summary>
    /// GOG Galaxy API entry point (STUB)
    /// This is a compilation stub. Calling these methods at runtime will throw NotImplementedException.
    /// </summary>
    public static class GalaxyInstance
    {
        /// <summary>
        /// Initialize Galaxy API (STUB - throws NotImplementedException)
        /// </summary>
        public static void Init(string clientId, string clientSecret)
        {
            throw new NotImplementedException("GOG Galaxy SDK not available. This is a compilation stub.");
        }

        /// <summary>
        /// Shutdown Galaxy API (STUB - safe to call, does nothing)
        /// </summary>
        public static void Shutdown()
        {
            // Safe no-op
        }

        /// <summary>
        /// Process Galaxy API data (STUB - safe to call, does nothing)
        /// </summary>
        public static void ProcessData()
        {
            // Safe no-op
        }

        /// <summary>
        /// Get User API (STUB - throws NotImplementedException)
        /// </summary>
        public static IUser User()
        {
            throw new NotImplementedException("GOG Galaxy SDK not available. This is a compilation stub.");
        }
    }

    /// <summary>
    /// Galaxy User API interface (STUB)
    /// </summary>
    public interface IUser
    {
        void SignIn();
    }

    /// <summary>
    /// Galaxy authentication listener base class (STUB)
    /// </summary>
    public abstract class GlobalAuthListener
    {
        // Base class for authentication callbacks
        // Original SDK may have virtual methods here
    }
}
