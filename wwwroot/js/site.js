/**
 * SecureChat - Site-wide JavaScript functionality
 */

// Wait for document ready
$(document).ready(function() {
    // Initialize theme
    initializeTheme();
    
    // Set up UI interactions and event handlers
    setupEventHandlers();
});

/**
 * Initialize the theme based on user preference or system setting
 */
function initializeTheme() {
    const themeToggle = $('#themeToggle');
    const icon = themeToggle.find('i');
    
    // Update the theme icon based on current theme
    function updateThemeIcon() {
        const theme = document.documentElement.getAttribute('data-theme');
        if (theme === 'dark') {
            icon.removeClass('moon outline').addClass('sun outline');
        } else {
            icon.removeClass('sun outline').addClass('moon outline');
        }
    }
    
    // Initial icon update
    updateThemeIcon();
    
    // Theme toggle functionality
    themeToggle.on('click', function() {
        const currentTheme = document.documentElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        
        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);
        updateThemeIcon();
    });
}

/**
 * Set up global event handlers and UI interactions
 */
function setupEventHandlers() {
    // Initialize Semantic UI components
    $('.ui.dropdown').dropdown();
    $('.ui.accordion').accordion();
    $('.ui.modal').modal();
    
    // Handle form validation
    $('.ui.form')
        .form({
            fields: {
                sessionId: {
                    identifier: 'id',
                    rules: [
                        {
                            type: 'empty',
                            prompt: 'Please enter a session ID'
                        }
                    ]
                }
            }
        });
}

/**
 * Format a timestamp as a human-readable string
 * @param {Date} date - The date to format
 * @returns {string} Formatted date string
 */
function formatTimestamp(date) {
    if (!date) date = new Date();
    
    return date.toLocaleTimeString([], { 
        hour: '2-digit', 
        minute: '2-digit'
    });
}

/**
 * Create a toast notification
 * @param {string} message - The notification message
 * @param {string} type - The notification type (info, success, warning, error)
 */
function notify(message, type = 'info') {
    // Ensure we have a container for notifications
    let container = $('#notifications');
    if (container.length === 0) {
        $('body').append('<div id="notifications" class="ui container"></div>');
        container = $('#notifications');
    }
    
    // Create the message element
    const icon = getIconForType(type);
    const toastHtml = `
        <div class="ui ${type} message">
            <i class="${icon} icon"></i>
            <div class="content">
                <div class="message">${message}</div>
            </div>
        </div>
    `;
    
    // Add to container
    const toast = $(toastHtml);
    container.append(toast);
    
    // Automatically remove after delay
    setTimeout(() => {
        toast.fadeOut(300, function() {
            $(this).remove();
        });
    }, 5000);
}

/**
 * Get the appropriate icon for notification type
 * @param {string} type - The notification type
 * @returns {string} Icon class
 */
function getIconForType(type) {
    switch (type) {
        case 'success':
            return 'check circle';
        case 'warning':
            return 'exclamation triangle';
        case 'error':
            return 'times circle';
        case 'info':
        default:
            return 'info circle';
    }
}