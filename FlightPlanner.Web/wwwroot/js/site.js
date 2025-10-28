// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Global Loading Spinner Management
(function () {
    'use strict';

    // Get or create the loading overlay
    function getLoadingOverlay() {
        let overlay = document.getElementById('global-loading-overlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'global-loading-overlay';
            overlay.innerHTML = `
                <div class="loading-content">
                    <div class="loading-spinner"></div>
                    <div class="loading-text">Carregando...</div>
                </div>
            `;
            document.body.appendChild(overlay);
        }
        return overlay;
    }

    // Show loading spinner
    function showLoading() {
        const overlay = getLoadingOverlay();
        overlay.classList.add('show');
    }

    // Hide loading spinner
    function hideLoading() {
        const overlay = getLoadingOverlay();
        overlay.classList.remove('show');
    }

    // Make functions globally available
    window.showLoading = showLoading;
    window.hideLoading = hideLoading;

    // Auto-show loading on form submissions
    document.addEventListener('submit', function (e) {
        // Only show loading for forms that will navigate away or make server requests
        const form = e.target;
        if (form && form.tagName === 'FORM') {
            // Don't show loading for forms with data-no-loading attribute
            if (!form.hasAttribute('data-no-loading')) {
                showLoading();
            }
        }
    });

    // Auto-show loading on navigation links (except # links and external links)
    document.addEventListener('click', function (e) {
        const link = e.target.closest('a');
        if (link && link.href) {
            const href = link.getAttribute('href');
            // Show loading for internal navigation links
            if (href &&
                !href.startsWith('#') &&
                !href.startsWith('javascript:') &&
                !link.hasAttribute('data-no-loading') &&
                !link.hasAttribute('target') &&
                link.hostname === window.location.hostname) {
                showLoading();
            }
        }
    });

    // Hide loading when page is fully loaded
    window.addEventListener('load', function () {
        hideLoading();
    });

    // Hide loading on page show (for back/forward navigation)
    window.addEventListener('pageshow', function () {
        hideLoading();
    });

    // Hide loading if there's an error
    window.addEventListener('error', function () {
        hideLoading();
    });

    // Hide loading on beforeunload cancellation
    let isUnloading = false;
    window.addEventListener('beforeunload', function () {
        isUnloading = true;
    });

    // If page is still here after a short delay, hide loading
    setInterval(function () {
        if (!isUnloading && document.readyState === 'complete') {
            hideLoading();
        }
    }, 1000);

})();
