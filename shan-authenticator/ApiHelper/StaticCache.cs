#region USING

using Shan.Authentication.API.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Threading;

#endregion USING
namespace Shan.Authentication.API.Web.ApiHelper
{
    /// <summary>
    /// Caching context during redirects
    /// </summary>
    public class MSALStaticCache
    {
        private static Dictionary<string, byte[]> staticCache = new Dictionary<string, byte[]>();

        private static ReaderWriterLockSlim SessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private readonly string userId = string.Empty;
        private readonly string cacheId = string.Empty;
        private readonly HttpContext httpContext = null;
        private ITokenCache cache;

        /// <summary>
        /// Constructor to preserve the user context
        /// </summary>
        /// <param name="userId"> User's object Id to connect the context </param>
        /// <param name="httpContext"> HttpContext being cached </param>
        public MSALStaticCache(string userId, HttpContext httpContext)
        {
            // not object, we want the SUB
            this.userId = userId;
            cacheId = this.userId + DataConstant.TokenCache;
            this.httpContext = httpContext;
        }

        /// <summary>
        /// To enable the persistence of cache and register the event handlers
        /// </summary>
        /// <param name="cache"> The token cache </param>
        /// <returns></returns>
        public ITokenCache EnablePersistence(ITokenCache cache)
        {
            this.cache = cache;
            cache.SetBeforeAccess(BeforeAccessNotification);
            cache.SetAfterAccess(AfterAccessNotification);
            return cache;
        }

        /// <summary>
        /// To deserialize the persisted user data from cache
        /// </summary>
        /// <param name="args"></param>
        public void Load(TokenCacheNotificationArgs args)
        {
            // Lock the cache in order to avoid over-write
            SessionLock.EnterReadLock();

            // If data is available for the requested session, deserialize it to respond
            byte[] blob = staticCache.ContainsKey(cacheId) ? staticCache[cacheId] : null;
            if (blob != null)
            {
                args.TokenCache.DeserializeMsalV3(blob);
            }

            // Unlock the cache
            SessionLock.ExitReadLock();
        }

        /// <summary>
        /// Serialize the token before adding to cache
        /// </summary>
        /// <param name="args"> The argument that contains the token cache </param>
        public void Persist(TokenCacheNotificationArgs args)
        {
            // Lock the cache in order to avoid over-write
            SessionLock.EnterWriteLock();

            // Reflect the changes in the persistent store
            staticCache[cacheId] = args.TokenCache.SerializeMsalV3();

            // Unlock the cache
            SessionLock.ExitWriteLock();
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load(args);
        }

        // Triggered right after MSAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (args.HasStateChanged)
            {
                Persist(args);
            }
        }
    }
}