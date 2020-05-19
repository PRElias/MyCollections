'use strict';

importScripts('https://storage.googleapis.com/workbox-cdn/releases/5.1.2/workbox-sw.js');

if (workbox) {
    console.log(`Yay! Workbox is loaded 🎉 for service-work.js`);
} else {
    console.log(`Boo! Workbox didn't load 😬 for service-work.js`);
}

workbox.routing.registerRoute(
    /.*\.(?:png|jpg|jpeg|svg|gif|js|css|woff|woff2|html)/g,
    new workbox.strategies.CacheFirst({
        cacheName: "static-cache",
        cacheableResponse: {
            statuses: [0, 200]
        }
    })
);

self.addEventListener('activate', (event) => {
  event.waitUntil((async () => {
    // See https://developers.google.com/web/updates/2017/02/navigation-preload
    if ('navigationPreload' in self.registration) {
      await self.registration.navigationPreload.enable();
    }
  })());

  self.clients.claim();
});

self.addEventListener('fetch', (event) => {
  if (event.request.mode === 'navigate') {
    event.respondWith((async () => {
      try {
        const preloadResponse = await event.preloadResponse;
        if (preloadResponse) {
          return preloadResponse;
        }

        const networkResponse = await fetch(event.request);
        return networkResponse;
      } catch (error) {
        console.log('Fetch failed; returning offline page instead.', error);

        const cache = await caches.open(CACHE_NAME);
        const cachedResponse = await cache.match(OFFLINE_URL);
        return cachedResponse;
      }
    })());
  }
});