// Minimal service worker: caches pages so the driver page can still
// open (from cache) if the connection drops. Not a real push server -
// just an offline shell cache.
var CACHE_NAME = 'tas-driver-v1';

self.addEventListener('install', function (event) {
    self.skipWaiting();
});

self.addEventListener('activate', function (event) {
    event.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', function (event) {
    // Let POST requests (send GPS point, upload POD photo) pass through untouched
    if (event.request.method !== 'GET') {
        return;
    }
    event.respondWith(
        caches.open(CACHE_NAME).then(function (cache) {
            return fetch(event.request).then(function (response) {
                if (response && response.status === 200) {
                    cache.put(event.request, response.clone());
                }
                return response;
            }).catch(function () {
                return cache.match(event.request);
            });
        })
    );
});
