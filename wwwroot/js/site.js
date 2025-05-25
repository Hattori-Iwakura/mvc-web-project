// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function() {
    let currentSlide = 0;
    const slides = document.querySelectorAll('.slide');
    const dots = document.querySelectorAll('.dot');
    
    // Initialize first slide
    slides[0].classList.add('active');
    
    function showSlide(index) {
        // Remove active class from current slide and dot
        slides[currentSlide].classList.remove('active');
        dots[currentSlide].classList.remove('active');
        
        // Update current slide index
        currentSlide = index;
        
        // Add active class to new slide and dot
        slides[currentSlide].classList.add('active');
        dots[currentSlide].classList.add('active');
    }
    
    // Auto advance slides every 5 seconds
    setInterval(() => {
        let nextSlide = (currentSlide + 1) % slides.length;
        showSlide(nextSlide);
    }, 5000);
    
    // Add click handlers to dots
    dots.forEach((dot, index) => {
        dot.addEventListener('click', () => {
            showSlide(index);
        });
    });
});

// Mobile menu toggle
document.addEventListener('DOMContentLoaded', function() {
    const mobileMenuToggle = document.querySelector('.mobile-menu-toggle');
    const navLinks = document.querySelector('.nav-links');
    
    if (mobileMenuToggle && navLinks) {
        mobileMenuToggle.addEventListener('click', function() {
            navLinks.classList.toggle('active');
        });
    }
    
    // Close mobile menu when clicking outside
    document.addEventListener('click', function(event) {
        if (!event.target.closest('.nav-wrapper') && navLinks.classList.contains('active')) {
            navLinks.classList.remove('active');
        }
    });
    
    // Handle dropdowns on mobile
    const dropdowns = document.querySelectorAll('.dropdown');
    dropdowns.forEach(dropdown => {
        if (window.innerWidth <= 768) {
            dropdown.addEventListener('click', function(e) {
                const content = this.querySelector('.dropdown-content');
                dropdowns.forEach(d => {
                    if (d !== dropdown) {
                        d.querySelector('.dropdown-content').style.display = 'none';
                    }
                });
                content.style.display = content.style.display === 'block' ? 'none' : 'block';
                e.stopPropagation();
            });
        }
    });
});
